using FunnyExperience.API.Sorting;
using FunnyExperience.Core.Mechanics;
using System.Collections.Generic;
using System.Linq;
using Terraria.Utilities;

namespace FunnyExperience.Core.Systems.CustomSpawnsSystem
{
	/// <summary>
	/// The central database for NPC statistics
	/// </summary>
	public static class NpcStatisticsRegistry
	{
		public class Entry
		{
			public readonly string NamePrefix;

			public readonly NpcStatistics Stats;

			public Func<short, bool> Requirement;

			public readonly float TableWeight;

			internal Entry(string name, float weight, NpcStatistics stats)
			{
				NamePrefix = name;
				TableWeight = weight;
				Stats = stats;
			}
		}

		private static Dictionary<int, List<Entry>> _registry;

		private static Entry GetEntry(int netID, string name)
		{
			if (_registry.TryGetValue(netID, out var list))
			{
				foreach (var entry in list)
					if (entry.NamePrefix == name)
						return entry;
			}

			return null;
		}

		internal static bool TryGetEntry(int netID, string name, out Entry entry)
		{
			entry = GetEntry(netID, name);
			return entry is not null;
		}

		internal static void Load()
		{
			_registry = new();

			NpcStatisticsRegistration.conditions = new(NPCProgressionRegistry.idsByProgression.Keys
				.Select(p =>
					new KeyValuePair<string, Func<short, bool>>(Enum.GetName(p), CreateProgressionFunction(p))));
		}

		private static Func<short, bool> CreateProgressionFunction(SortingProgression progression)
			=> npc => NPCProgressionRegistry.CanUseEntriesAtProgressionStage(progression, npc,
				Main.gameMenu ? null : Main.LocalPlayer);

		internal static Func<short, bool> CreateProgressionFunction(string jsonProgression)
		{
			Func<short, bool> ret = null;

			foreach (var progression in jsonProgression.Split(';'))
			{
				if (ret is null)
					ret = NpcStatisticsRegistration.conditions[progression];
				else
					ret += NpcStatisticsRegistration.conditions[progression];
			}

			return ret;
		}

		internal static void PostSetupContent()
		{
			NpcStatisticsRegistration.RegisterEntries();
		}

		internal static void Unload()
		{
			_registry?.Clear();
			_registry = null;

			NpcStatisticsRegistration.conditions?.Clear();
			NpcStatisticsRegistration.conditions = null;
		}

		public static Entry GetRandomStats(int type)
		{
			if (!_registry.TryGetValue(type, out var list))
				return null;

			var validEntries = list.Where(e => e.Requirement?.Invoke((short)type) ?? true).ToList();
			if (validEntries.Count == 0)
				return null;

			WeightedRandom<Entry> wRand = new(Main.rand);
			foreach (var entry in validEntries)
				wRand.Add(entry, entry.TableWeight);

			return wRand.Get();
		}

		public static bool HasStats(int type)
			=> _registry.ContainsKey(type);

		public static void CreateEntry(int type, string namePrefix, float weight, NpcStatistics stats,
			Func<short, bool> requirement = null)
		{
			if (namePrefix == "null")
				namePrefix = null;

			if (!_registry.TryGetValue(type, out var list))
				_registry.Add(type, list = new List<Entry>());

			var entry = new Entry(namePrefix, weight, stats) { Requirement = requirement };

			list.Add(entry);
		}
	}
}