﻿namespace FunnyExperience.Content.Items.Gear.Affixes.WeaponAffixes
{
	public class ModifyHitAffixes
	{
		internal class PiercingAffix : Affix
		{
			public PiercingAffix()
			{
				PossibleTypes = GearType.Sword;
				ModifierType = ModifierType.Added;
			}

			public override float GetModifierValue(Gear gear)
			{
				return 1 + (int)(Value * 5) + gear.ItemLevel / 50;
			}
		
			public override string GetTooltip(Player player, Gear gear)
			{
				return $"+{GetModifierValue(gear)} Armor Penetration";
			}
		}
		internal class AddedKnockbackAffix : Affix
		{
			public AddedKnockbackAffix()
			{
				PossibleTypes = GearType.Sword;
				ModifierType = ModifierType.Added;
			}

			public override float GetModifierValue(Gear gear)
			{
				return 1 + (int)(Value * 5) + gear.ItemLevel / 50;
			}
		
			public override string GetTooltip(Player player, Gear gear)
			{
				return $"+{GetModifierValue(gear)} Added Knockback";
			}
		}
		
		internal class IncreasedKnockbackAffix : Affix
		{
			public IncreasedKnockbackAffix()
			{
				PossibleTypes = GearType.Sword;
				ModifierType = ModifierType.Multiplier;
			}

			public override float GetModifierValue(Gear gear)
			{
				return 1 + (int)(Value * 5) + gear.ItemLevel / 50;
			}
		
			public override string GetTooltip(Player player, Gear gear)
			{
				return $"{GetModifierValue(gear)}% Increased Knockback";
			}
		}
	}
}