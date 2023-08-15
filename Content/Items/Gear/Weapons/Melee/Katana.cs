using System.Linq;
using Terraria.ID;

namespace FunnyExperience.Content.Items.Gear.Weapons.Melee
{
	internal class Katana : Sword
	{
		public override string Texture => $"{FunnyExperience.ModName}/Assets/Items/Gear/Weapon/Sword/Katana";
	
		public override void SetDefaults()
		{
			base.SetDefaults();
			Item.damage = 20;
			Item.useTime = 35; 
			Item.useAnimation = 35;
			Item.width = 48; 
			Item.height = 54;
		}
	}
}