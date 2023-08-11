namespace FunnyExperience.Content.Items.Gear.Affixes.ArmorAffixes
{
	internal class DefenseAffixes
	{
		internal class DefenseAffix : Affix
		{
			public DefenseAffix()
			{
				PossibleTypes = GearType.Helmet | GearType.Chestplate | GearType.Leggings;
			}

			public override string GetTooltip(Player player, Gear gear)
			{
				return $"+{1 + (int)(Value * 5) + gear.ItemLevel / 50} Additional Defense";
			}

			public override void BuffPassive(Player player, Gear gear)
			{
				player.statDefense += 1 + (int)(Value * 5) + gear.ItemLevel / 50;
			}
		}

		internal class EnduranceAffix : Affix
		{
			public EnduranceAffix()
			{
				PossibleTypes = GearType.Helmet | GearType.Chestplate | GearType.Leggings;
			}

			public override string GetTooltip(Player player, Gear gear)
			{
				return $"+{(float)Math.Truncate((Value * 5 + gear.ItemLevel / 50) * 10) / 10}% Damage Reduction";
			}

			public override void BuffPassive(Player player, Gear gear)
			{
				player.endurance += (float)Math.Truncate((Value * 5 + gear.ItemLevel / 50) * 10) / 10 / 100f;
			}
		}
	}
}
