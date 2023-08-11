namespace FunnyExperience.Content.Items.Gear.Affixes.WeaponAffixes
{
	public class DamageAffixes
	{
		internal class PiercingAffix : Affix
		{
			public PiercingAffix()
			{
				PossibleTypes = GearType.Sword;
			}

			private int GetModifierValue(Gear gear)
			{
				return 1 + (int)(Value * 5) + gear.ItemLevel / 50;
			}
		
			public override string GetTooltip(Player player, Gear gear)
			{
				return $"+{GetModifierValue(gear)} Armor Penetration";
			}

			public override void BuffPassive(Player player, Gear gear)
			{
				//TODO: This doesn't currently work for overriding the value
				gear.Item.ArmorPenetration += GetModifierValue(gear);
			}
		}
		
		internal class RapidAffix : Affix
		{
			public RapidAffix()
			{
				PossibleTypes = GearType.Sword;
			}

			private int GetModifierValue(Gear gear)
			{
				return 1 + (int)(Value * 5) + gear.ItemLevel / 50;
			}
		
			public override string GetTooltip(Player player, Gear gear)
			{
				return $"+{GetModifierValue(gear)} Attack Speed";
			}

			public override void BuffPassive(Player player, Gear gear)
			{
				//TODO: This doesn't currently work for overriding the value
				gear.Item.useTime -= GetModifierValue(gear);
				gear.Item.useAnimation -= GetModifierValue(gear);
			}
		}
		
		internal class SharpAffix : Affix
		{
			public SharpAffix()
			{
				PossibleTypes = GearType.Sword;
			}

			private int GetModifierValue(Gear gear)
			{
				return 1 + (int)(Value * 5) + gear.ItemLevel / 50;
			}
		
			public override string GetTooltip(Player player, Gear gear)
			{
				return $"+{GetModifierValue(gear)} Additional Damage";
			}

			public override void BuffPassive(Player player, Gear gear)
			{
				//TODO: This doesn't currently work for overriding the value
				gear.Item.damage += GetModifierValue(gear);
			}
		}
	}
}