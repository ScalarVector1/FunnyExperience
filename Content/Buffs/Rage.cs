using FunnyExperience.Core.Systems.SkillSystem;

namespace FunnyExperience.Content.Buffs
{
	class CustomRage : SmartBuff
	{
		public CustomRage() : base("Rage", "Increased damage and greatly increased knockback", false) { }

		public override string Texture => $"{FunnyExperience.ModName}/Assets/Buffs/Base";

		public override void Load()
		{
			SkillPlayer.OnHitByNPCEvent += TakeMoreDamage;
		}

		private void TakeMoreDamage(Player player, NPC npc, Player.HurtInfo hurtInfo)
		{
			if (player.HasBuff<CustomRage>())
				player.statLife -= 5; //Take 5 extra damage when hit
		}

		public override void Update(Player Player, ref int buffIndex)
		{ 
			Player.GetDamage(DamageClass.Generic) += 1.5f; //150% more damage
		}
	}
}