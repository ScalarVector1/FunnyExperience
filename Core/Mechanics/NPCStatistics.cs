using FunnyExperience.Core.JSON;
using Newtonsoft.Json;
using System.Runtime.Serialization;
using Terraria.ModLoader.IO;

namespace FunnyExperience.Core.Mechanics{
	/// <summary>
	/// An object representing statistics for an NPC
	/// </summary>
	[DataContract]
	[JsonConverter(typeof(NpcStatisticsConverter))]
	public class NpcStatistics {
		public Modifier ScaleModifier = Modifier.Default;
		public Modifier HealthModifier = Modifier.Default;
		public int Level;
		public int Xp;

		public void ApplyTo(NPC npc){
			//For NPCs that transform, carry over the current life instead of setting it
			bool freshlySpawned = npc.life == npc.lifeMax;

			HealthModifier.ApplyModifier(ref npc.lifeMax);

			if(freshlySpawned)
				npc.life = npc.lifeMax;

			ScaleModifier.ApplyModifier(ref npc.scale);
		}
		
		public virtual TagCompound SaveToTag()
			=> new() {
				["level"] = Level,
				["xp"] = Xp,
				["mod.hp"] = HealthModifier.ToTag(),
				["mod.scale"] = ScaleModifier.ToTag(),
			};
		
		public virtual void LoadFromTag(TagCompound tag) {
			Level = tag.GetInt("level");
			Xp = tag.GetInt("xp");
			HealthModifier = Modifier.FromTag(tag.GetCompound("mod.hp"));
			ScaleModifier = Modifier.FromTag(tag.GetCompound("mod.scale"));
		}
	}
}
