using FunnyExperience.Content.Edits.Detours;

namespace FunnyExperience.Content.Edits {
	internal static class EditsLoader {
		public static void Load(){
			On_NPC.SetDefaultsFromNetId += VanillaNpcDetour.NPC_SetDefaultsFromNetId;
			On_NPC.Transform += VanillaNpcDetour.NPC_Transform;
		}
	}
}
