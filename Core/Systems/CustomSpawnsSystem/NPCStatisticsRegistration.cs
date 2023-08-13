using FunnyExperience.API.Sorting;
using FunnyExperience.Core.JSON;
using FunnyExperience.Core.Mechanics;
using FunnyExperience.Core.Utility;
using Newtonsoft.Json;
using ReLogic.OS;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Terraria.ID;

namespace FunnyExperience.Core.Systems.CustomSpawnsSystem
{
	public static partial class NpcStatisticsRegistration
	{
		internal static Dictionary<string, Func<short, bool>> conditions;

		private static void AddRequirement(ref Func<short, bool> existing, Func<short, bool> newRequirement)
		{
			if (existing is null)
				existing = newRequirement;
			else
				existing += newRequirement;
		}

		public static void RegisterEntries()
		{
			string projectFolder = Path.Combine(Platform.Get<IPathService>().GetStoragePath("Terraria"), "tModLoader", "ModSources", nameof(FunnyExperience), "Data", "NPCs");
			CreateProgressionJsons(projectFolder);

			FunnyExperience.Instance.Logger.Debug("Loading internally stored JSONs...");

			using Stream pathsStream = FunnyExperience.Instance.GetFileStream("Data/NPCs/paths.txt");
			using var pathsReader = new StreamReader(pathsStream);
			string[] paths = pathsReader.ReadToEnd().Split("\r\n", StringSplitOptions.RemoveEmptyEntries);

			pathsReader.Dispose();
			pathsStream.Dispose();

			using Stream defaultPathsStream = FunnyExperience.Instance.GetFileStream("Data/NPCs/defaultPaths.txt");
			using var defaultPathsReader = new StreamReader(defaultPathsStream);
			string[] defaultPaths = defaultPathsReader.ReadToEnd().Split("\r\n", StringSplitOptions.RemoveEmptyEntries);
			var defaults = new Dictionary<string, NpcStatisticsDatabaseEntryJson>();

			defaultPathsStream.Dispose();
			defaultPathsReader.Dispose();

			foreach (var defaultPath in defaultPaths)
			{
				Stream jsonStream = FunnyExperience.Instance.GetFileStream("Data/NPCs/" + defaultPath);
				var jsonReader = new StreamReader(jsonStream);
				string json = jsonReader.ReadToEnd();
				int nameIdStart = defaultPath.IndexOf('/');
				string thingName = Path.ChangeExtension(defaultPath[(nameIdStart + 1)..], null);
				defaults.Add(thingName,
					JsonConvert.DeserializeObject<NpcStatisticsDatabaseEntryJson>(json,
						JsonUtils.GetDeserializationSettings()));
				jsonReader.Dispose();
				jsonStream.Dispose();
			}

			foreach (string path in paths)
			{
				Stream jsonStream = FunnyExperience.Instance.GetFileStream("Data/NPCs/" + path);
				StreamReader jsonReader = new StreamReader(jsonStream);
				string json = jsonReader.ReadToEnd();

				int nameIDStart = path.IndexOf('/');
				if (nameIDStart == -1)
				{
					FunnyExperience.Instance.Logger.Warn($"Registry path was invalid: \"{path}\"");
					goto disposeStreams;
				}

				string source = path[..nameIDStart];
				string thingName = Path.ChangeExtension(path[(nameIDStart + 1)..], null);

				int id;
				if (source == "Vanilla")
				{
					if (!NPCID.Search.TryGetId(thingName, out id))
					{
						FunnyExperience.Instance.Logger.Warn(
							$"Registry path \"{path}\" had an invalid vanilla type identifier: \"{thingName}\"");
						goto disposeStreams;
					}
				}
				else if (ModLoader.TryGetMod(source, out Mod mod))
				{
					if (!mod.TryFind(thingName, out ModNPC modNPC))
					{
						FunnyExperience.Instance.Logger.Warn(
							$"Registry path \"{path}\" had an invalid mod type identifier: \"{thingName}\"");
						goto disposeStreams;
					}

					id = modNPC.Type;
				}
				else
				{
					//Mod doesn't exist or wasn't loaded.  Just ignore it
					goto disposeStreams;
				}

				NpcStatisticsDatabaseJson database =
					JsonConvert.DeserializeObject<NpcStatisticsDatabaseJson>(json,
						JsonUtils.GetDeserializationSettings());
				if (database == null) goto disposeStreams;

				if (database.Defaults != null)
				{
					foreach (var defaultEntry in database.Defaults)
					{
						//Registers all defaults in configuration
						if (defaults.TryGetValue(defaultEntry.Name, out NpcStatisticsDatabaseEntryJson defaultValue))
						{
							Func<short, bool> requirement = null;
							if (defaultValue.RequirementKeys is not null)
								requirement =
									NpcStatisticsRegistry.CreateProgressionFunction(defaultValue.RequirementKeys);
							NpcStatisticsRegistry.CreateEntry(id, defaultEntry.Name, defaultValue.Weight,
								defaultValue.Stats, requirement);
						}
					}
				}

				if (defaults.TryGetValue("Base", out NpcStatisticsDatabaseEntryJson value))
				{
					//Registers all creatures bases that have json files
					Func<short, bool> requirement = null;
					if (value.RequirementKeys is not null)
						requirement = NpcStatisticsRegistry.CreateProgressionFunction(value.RequirementKeys);
					NpcStatisticsRegistry.CreateEntry(id, null, 5.0f, value.Stats, requirement);
				}

				foreach (var entry in database.Database)
				{
					//Registers all manual entries
					Func<short, bool> requirement = null;
					if (entry.RequirementKeys is not null)
						requirement = NpcStatisticsRegistry.CreateProgressionFunction(entry.RequirementKeys);

					NpcStatisticsRegistry.CreateEntry(id, entry.NamePrefix, entry.Weight, entry.Stats, requirement);

					//FunnyExperience.Instance.Logger.Debug($"Added statistics entry for NPC \"{Lang.GetNPCNameValue(id)}\", Prefix: {entry.NamePrefix ?? "none"}");
				}

				disposeStreams:
				jsonReader.Dispose();
				jsonStream.Dispose();
			}
		}

		private static void CreateProgressionJsons(string projectFolder)
		{
			FunnyExperience.Instance.Logger.Debug(
				"FunnyExperience project directory found.  Attempting to update Data folder...");

			string pathsFile = Path.Combine(projectFolder, "paths.txt");
			List<string> existingFiles = new(File.ReadAllLines(pathsFile));
			List<string> files = new();
			bool entryWasCreated = false;

			ModNPC m = null;
			Dictionary<short, List<SortingProgression>> progressionDict = NPCProgressionRegistry.idsToProgressions;
			foreach (string file in existingFiles
				         .Concat(NPCProgressionRegistry.idsToProgressions.Keys.Select(k => k < NPCID.Count
					         ? "Vanilla/" + NPCID.Search.GetName(k)
					         : (m = NPCLoader.GetNPC(k)).Mod.Name + "/" + m.FullName[(m.FullName.IndexOf('.') + 1)..])))
			{
				int id = -1;

				string fullPath = Path.Combine(projectFolder, file) + ".json";

				string modName = Path.GetDirectoryName(file);
				Directory.CreateDirectory(Path.Combine(projectFolder, modName));

				string npcName = Path.GetFileName(file);

				bool isVanillaId = modName == "Vanilla" && NPCID.Search.TryGetId(npcName, out id);
				bool isModdedId = !isVanillaId && ModContent.TryFind(file.Replace('/', '.'), out m);
				if (isModdedId)
					id = (short)m.Type;

				if (File.Exists(fullPath))
				{
					JsonConvert.DeserializeObject<NpcStatisticsDatabaseJson>(File.ReadAllText(fullPath),JsonUtils.GetSerializationSettings());
					files.Add(file + ".json");
				}
				else if ((isVanillaId || isModdedId) && progressionDict.TryGetValue((short)id, out var progressions))
				{
					NpcStatistics stats;
					NpcStatisticsDatabaseJson json = new()
					{
						Database = progressions
							.Select(p =>
								(stats = GenerateStats(id, p)) is null
									? null
									: new NpcStatisticsDatabaseEntryJson()
									{
										NamePrefix = null,
										Weight = 1f,
										Stats = stats,
										RequirementKeys = Enum.GetName(p)
									})
							.Where(j => j is not null)
							.ToList()
					};

					if (json.Database.Count > 0)
					{
						//Create the file in the project
						using (var writer = new StreamWriter(File.Open(fullPath, FileMode.CreateNew)))
							writer.Write(JsonConvert.SerializeObject(json, Formatting.Indented,
								JsonUtils.GetSerializationSettings()));

						files.Add(file + ".json");

						entryWasCreated = true;
					}
				}
			}

			//Update the paths file if any new entries were created
			if (!entryWasCreated && existingFiles.Count == files.Count) 
				return;
			
			File.WriteAllLines(pathsFile, files);

			FunnyExperience.Instance.Logger.Warn(
				"New JSON entries have been created.  Re-building the mod will be required!");
		}

		private static NpcStatistics GenerateStats(int type, SortingProgression progression)
		{
			NpcStatistics stats = GenerateBossStats(type);
			
			if (stats is null) 
				return null;
			
			FunnyExperience.Instance.Logger.Debug(
				$"Generated stats for vanilla NPC \"{Lang.GetNPCNameValue(type)}\"");

			return stats;
		}

		private static NpcStatistics GenerateBossStats(int type)
			=> type switch
			{
				NPCID.KingSlime => new() { Level = 10, Xp = 500 },
				NPCID.EyeofCthulhu => new() { Level = 12, Xp = 650 },
				NPCID.EaterofWorldsHead => new() { Level = 18, Xp = 1200 },
				NPCID.EaterofWorldsBody => new() { Level = 18, Xp = 0 },
				NPCID.EaterofWorldsTail => new() { Level = 18, Xp = 0 },
				NPCID.BrainofCthulhu => new() { Level = 18, Xp = 1200 },
				NPCID.DD2DarkMageT1 => new() { Level = 15, Xp = 900 },
				NPCID.QueenBee => new() { Level = 25, Xp = 3500 },
				NPCID.SkeletronHead => new() { Level = 30, Xp = 4550 },
				NPCID.SkeletronHand => new() { Level = 30, Xp = 0 },
				NPCID.WallofFlesh => new() { Level = 35, Xp = 2000 },
				NPCID.WallofFleshEye => new() { Level = 35, Xp = 2000 },
				NPCID.GoblinSummoner => new() { Level = 40, Xp = 3500 },
				NPCID.BloodNautilus => new() { Level = 38, Xp = 2750 },
				NPCID.QueenSlimeBoss => new() { Level = 45, Xp = 6000 },
				NPCID.TheDestroyer => new() { Level = 55, Xp = 8000 },
				NPCID.PirateCaptain => new() { Level = 51, Xp = 2500 },
				NPCID.PirateShip => new() { Level = 53, Xp = 6000, },
				NPCID.PirateShipCannon => new() { Level = 53, Xp = 0, },
				NPCID.DD2OgreT2 => new() { Level = 60, Xp = 9000 },
				NPCID.Spazmatism => new() { Level = 55, Xp = 4000 },
				NPCID.Retinazer => new() { Level = 55, Xp = 4000 },
				NPCID.SkeletronPrime => new() { Level = 55, Xp = 8000 },
				NPCID.PrimeCannon => new() { Level = 55, Xp = 0 },
				NPCID.PrimeLaser => new() { Level = 55, Xp = 0 },
				NPCID.PrimeSaw => new() { Level = 55, Xp = 0 },
				NPCID.PrimeVice => new() { Level = 55, Xp = 0 },
				NPCID.Plantera => new() { Level = 75, Xp = 12000 },
				NPCID.Mothron => new() { Level = 77, Xp = 12500 },
				NPCID.MourningWood => new() { Level = 80, Xp = 15750 },
				NPCID.Pumpking => new() { Level = 82, Xp = 17500 },
				NPCID.Everscream => new() { Level = 80, Xp = 15750 },
				NPCID.SantaNK1 => new() { Level = 81, Xp = 16000 },
				NPCID.IceQueen => new() { Level = 82, Xp = 17500 },
				NPCID.HallowBoss => new() { Level = 86, Xp = 20000 },
				NPCID.Golem => new() { Level = 89, Xp = 22750 },
				NPCID.GolemFistLeft => new() { Level = 89, Xp = 0 },
				NPCID.GolemFistRight => new() { Level = 89, Xp = 0 },
				NPCID.GolemHead => new() { Level = 89, Xp = 0 },
				NPCID.GolemHeadFree => new() { Level = 89, Xp = 0 },
				NPCID.MartianSaucerCore => new() { Level = 85, Xp = 19000 },
				NPCID.MartianSaucerCannon => new() { Level = 85, Xp = 0 },
				NPCID.MartianSaucerTurret => new() { Level = 85, Xp = 0 },
				NPCID.MartianSaucer => new() { Level = 85, Xp = 0 },
				NPCID.DD2DarkMageT3 => new() { Level = 82, Xp = 14000 },
				NPCID.DD2OgreT3 => new() { Level = 83, Xp = 15000 },
				NPCID.DD2Betsy => new() { Level = 90, Xp = 23000 },
				NPCID.DukeFishron => new() { Level = 90, Xp = 25000 },
				NPCID.CultistBoss => new() { Level = 95, Xp = 30000 },
				NPCID.LunarTowerSolar => new() { Level = 97, Xp = 8000 },
				NPCID.LunarTowerNebula => new() { Level = 97, Xp = 8000 },
				NPCID.LunarTowerStardust => new() { Level = 97, Xp = 8000 },
				NPCID.LunarTowerVortex => new() { Level = 97, Xp = 8000 },
				NPCID.MoonLordCore => new() { Level = 100, Xp = 40000 },
				NPCID.MoonLordHead => new() { Level = 100, Xp = 0 },
				NPCID.MoonLordHand => new() { Level = 100, Xp = 0 },
				NPCID.MoonLordFreeEye => new() { Level = 100, Xp = 0 },
				_ => null
			};
	}
}