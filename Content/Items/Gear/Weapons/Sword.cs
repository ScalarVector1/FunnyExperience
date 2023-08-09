using System.Linq;
using Terraria.ID;

namespace FunnyExperience.Content.Items.Gear.Weapons
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
		
		public override void PostRoll()
		{
			//These don't work.... Whai?
			Item.damage = GetDamage();
			Item.useTime = GetUseTime();
		}

		private SwordType FindType()
		{
			return Enum.GetValues(typeof(SwordType)).Cast<SwordType>().FirstOrDefault(enumType => Item.Name.Contains(enumType.ToString(), StringComparison.OrdinalIgnoreCase));
		}

		public int GetDamage()
		{
			return FindType() switch
			{
				SwordType.Sword => 10,
				SwordType.Falchion => 15,
				SwordType.Sabre => 20,
				SwordType.Longsword => 25,
				_ => throw new ArgumentOutOfRangeException()
			};
		}
		
		public int GetUseTime()
		{
			return FindType() switch
			{
				SwordType.Sword => 20,
				SwordType.Falchion => 35,
				SwordType.Sabre => 50,
				SwordType.Longsword => 65,
				_ => throw new ArgumentOutOfRangeException()
			};
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

			string item = Main.rand.Next(4) switch
			{
				0 => SwordType.Sword.ToString(),
				1 => SwordType.Falchion.ToString(),
				2 => SwordType.Sabre.ToString(),
				3 => SwordType.Longsword.ToString(),
				_ => "Unknown"
			};

			return rarity switch
			{
				GearRarity.Normal => item,
				GearRarity.Magic => $"{prefix} {item}",
				GearRarity.Rare => $"{prefix} {suffix} {item}",
				GearRarity.Unique => Item.Name,
				_ => "Unknown Item"
			};
		}

		private enum SwordType
		{
			Sword,
			Falchion,
			Sabre,
			Longsword,
		}
	}
}