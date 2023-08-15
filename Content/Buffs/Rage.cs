using FunnyExperience.Core;
using FunnyExperience.Core.Systems.SkillSystem;

namespace FunnyExperience.Content.Buffs
{
	class CustomRage : SmartBuff
	{
		public CustomRage() : base("Rage", "Increased damage and greatly increased knockback", false) { }

		public override string Texture => $"{FunnyExperience.ModName}/Assets/Buffs/Base";

		public override void Load()
		{
			FunnyExperienceNpcEvents.ModifyHitPlayerEvent += TakeMoreDamage;
		}

		private void TakeMoreDamage(NPC npc, Player target, ref Player.HurtModifiers modifiers)
		{
			modifiers.FinalDamage += 5;
		}

		public override void Update(Player Player, ref int buffIndex)
		{ 
			Player.GetDamage(DamageClass.Generic) += 1.5f; //150% more damage
		}
	}
}