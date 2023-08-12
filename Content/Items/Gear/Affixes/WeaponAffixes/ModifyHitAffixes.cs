namespace FunnyExperience.Content.Items.Gear.Affixes.WeaponAffixes
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
		internal class KnockbackAffix : Affix
		{
			public KnockbackAffix()
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
				return $"+{GetModifierValue(gear)} Additional Knockback";
			}
		}
	}
}