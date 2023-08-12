using FunnyExperience.Content.Buffs;
using FunnyExperience.Content.Items.Gear;
using FunnyExperience.Core.Systems.SkillSystem;

namespace FunnyExperience.Content.Skills.Melee
{
	public class Berserk : Skill
	{
		public Berserk(int duration, int timer, int maxCooldown, int cooldown, int manaCost, GearType weaponType) : base(duration, timer, maxCooldown, cooldown, manaCost, weaponType)
		{
			Timer = 1200;
		}

		public override void UseSkill(Player player)
		{
			if (player.statMana < 5) 
				return;
			
			player.statMana -= 5;
			player.AddBuff(ModContent.BuffType<CustomRage>(), Timer);
			throw new NotImplementedException();
		}

		public override string GetDescription(Player player)
		{
			return "Deal 150% more Damage while taking 5 more damage each time hit";
		}
	}
}