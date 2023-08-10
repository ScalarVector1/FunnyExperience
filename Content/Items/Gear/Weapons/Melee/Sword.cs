using System.Linq;
using Terraria.ID;

namespace FunnyExperience.Content.Items.Gear.Weapons.Melee
{
	internal class Sword : Gear
	{
		public override string Texture => $"{FunnyExperience.ModName}/Assets/Items/Gear/Weapon/Sword/Base";
	
		public override void SetDefaults()
		{
			Item.damage = 10;
			Item.width = 40; 
			Item.height = 40;
			Item.useStyle = ItemUseStyleID.Swing; 
			Item.useTime = 20; 
			Item.useAnimation = 20;
			Item.autoReuse = true;
			Item.DamageType = DamageClass.Melee;
			Item.knockBack = 6;
			Item.crit = 6;
			Item.UseSound = SoundID.Item1; 
		}

		public override string GenerateName()
		{
			string prefix = Main.rand.Next(5) switch
			{
				0 => "Sharp",
				1 => "Harmonic",
				2 => "Enchanted",
				3 => "Shiny",
				4 => "Strange",
				_ => "Unknown"
			};

			string suffix = Main.rand.Next(5) switch
			{
				0 => "Drape",
				1 => "Dome",
				2 => "Thought",
				3 => "Vision",
				4 => "Maw",
				_ => "Unknown"
			};

			string item = GetType().ToString();
			return rarity switch
			{
				GearRarity.Normal => item,
				GearRarity.Magic => $"{prefix} {item}",
				GearRarity.Rare => $"{prefix} {suffix} {item}",
				GearRarity.Unique => Item.Name,
				_ => "Unknown Item"
			};
		}
	}
}