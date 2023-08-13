using FunnyExperience.Core.Mechanics;
using FunnyExperience.Core.Systems.CustomSpawnsSystem;

namespace FunnyExperience.Content.Edits.Detours
{
	internal static partial class VanillaNpcDetour {
		public static int NetIDOverride{ get; private set; }

		public static bool TransformingNPC{ get; private set; }

		public static NpcStatistics PreTransformStats{ get; private set; }

		public static string PreTransformNamePrefix{ get; private set; }

		public static void NPC_SetDefaultsFromNetId(Terraria.On_NPC.orig_SetDefaultsFromNetId orig, Terraria.NPC self, int id, Terraria.NPCSpawnParams spawnparams){
			//Due to some bad API code, NPCLoader.SetDefaults isn't given the netID properly, so let's do that properly
			NetIDOverride = id;

			orig(self, id, spawnparams);

			NetIDOverride = 0;
		}

		public static void NPC_Transform(Terraria.On_NPC.orig_Transform orig, Terraria.NPC self, int newType){
			//Force the NPC's StatNPC to not update its registry entry
			TransformingNPC = true;

			if(self.TryGetGlobalNPC(out CustomSpawnsSystem stat)){
				PreTransformStats = stat.Stats;
				PreTransformNamePrefix = stat.NamePrefix;
			}

			orig(self, newType);

			PreTransformStats = null;
			PreTransformNamePrefix = null;

			TransformingNPC = false;
		}
	}
}