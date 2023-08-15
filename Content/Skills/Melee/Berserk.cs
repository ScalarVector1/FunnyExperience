﻿using FunnyExperience.Content.Buffs;
using FunnyExperience.Content.Items.Gear;
using FunnyExperience.Core.Systems.SkillSystem;

namespace FunnyExperience.Content.Skills.Melee
{
	public class Berserk : Skill
	{
		public Berserk(int duration, int timer, int maxCooldown, int cooldown, int manaCost, GearType weaponType) : base(duration, timer, maxCooldown, cooldown, manaCost, weaponType)
		{
			Duration = duration;
			Cooldown = cooldown;
			MaxCooldown = maxCooldown;
			Cooldown = cooldown;
			ManaCost = manaCost;
		}

		public override void UseSkill(Player player)
		{
			if (!CanUseSkill(player)) 
				return;
			
			player.statMana -= ManaCost;
			player.AddBuff(ModContent.BuffType<CustomRage>(), Duration);
			Timer = Cooldown;
		}

		public override string GetDescription(Player player)
		{
			return "Deal 150% more Damage while taking 5 more damage each time hit";
		}
	}
}