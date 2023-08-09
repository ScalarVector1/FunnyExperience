using FunnyExperience.Core.Systems;

namespace FunnyExperience.Content.Items.Gear.Affixes.ArmorAffixes
{
	internal class LifeAffix : Affix
	{
		public LifeAffix()
		{
			possibleTypes = GearType.Helmet | GearType.Chestplate | GearType.Leggings;
		}

		public override string GetTooltip(Player player, Gear gear)
		{
			return $"+{10 + (int)(value * 30) + gear.itemLevel / 10} Maximum Life";
		}

		public override void BuffPassive(Player player, Gear gear)
		{
			player.statLifeMax2 += 10 + (int)(value * 30) + gear.itemLevel / 10;
		}
	}

	internal class LifeRegenAffix : Affix
	{
		public LifeRegenAffix()
		{
			possibleTypes = GearType.Helmet | GearType.Chestplate | GearType.Leggings;
		}

		public override string GetTooltip(Player player, Gear gear)
		{
			return $"+{1 + (int)(value * 4) + gear.itemLevel / 40} Life Regeneration";
		}

		public override void BuffPassive(Player player, Gear gear)
		{
			player.lifeRegen += 1 + (int)(value * 4) + gear.itemLevel / 40;
		}
	}

	internal class LifePotionPowerAffix : Affix
	{
		public LifePotionPowerAffix()
		{
			possibleTypes = GearType.Helmet | GearType.Chestplate | GearType.Leggings;
		}

		public override string GetTooltip(Player player, Gear gear)
		{
			return $"Healing potions restore {10 + (int)(value * 10) + gear.itemLevel / 20} more life";
		}

		public override void BuffPassive(Player player, Gear gear)
		{
			player.GetModPlayer<PotionSystem>().healPower += 10 + (int)(value * 10) + gear.itemLevel / 20;
		}
	}

	internal class LifePotionCapAffix : Affix
	{
		public LifePotionCapAffix()
		{
			possibleTypes = GearType.Helmet | GearType.Chestplate | GearType.Leggings;
		}

		public override string GetTooltip(Player player, Gear gear)
		{
			return $"You can hold {1 + (int)Math.Round(value, MidpointRounding.ToEven) + gear.itemLevel / 100} additional healing potions";
		}

		public override void BuffPassive(Player player, Gear gear)
		{
			player.GetModPlayer<PotionSystem>().maxHealing += 1 + (int)Math.Round(value, MidpointRounding.ToEven) + gear.itemLevel / 100;
		}
	}

	internal class LifePotionCooldownAffix : Affix
	{
		public LifePotionCooldownAffix()
		{
			possibleTypes = GearType.Helmet | GearType.Chestplate | GearType.Leggings;
			requiredInfluence = GearInfluence.Lunar;
		}

		public override string GetTooltip(Player player, Gear gear)
		{
			return $"Healing potions are ready {0.5f + value * 0.5f} seconds sooner";
		}

		public override void BuffPassive(Player player, Gear gear)
		{
			player.GetModPlayer<PotionSystem>().healDelay -= (int)(60 * (0.5f + value * 0.5f));
		}
	}
}
