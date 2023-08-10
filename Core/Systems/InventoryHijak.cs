using Terraria.GameContent;
using Terraria.GameInput;
using Terraria.UI;

namespace FunnyExperience.Core.Systems
{
	internal class InventoryHijak : ModSystem
	{
		public override void Load()
		{
			On_ItemSlot.Draw_SpriteBatch_ItemArray_int_int_Vector2_Color += OffsetFirstTwo;
			On_Main.DrawInventory += DrawPreview;
		}

		private void DrawPreview(On_Main.orig_DrawInventory orig, Main self)
		{
			Vector2 pos = Main.LocalPlayer.Center - Main.screenPosition;
			var source = new Rectangle((int)pos.X - 42, (int)pos.Y - 64, 84, 128);
			Main.spriteBatch.Draw(Main.screenTarget, new Rectangle(Main.screenWidth - 186, 436, 84, 128), source, Color.White);

			var tex = ModContent.Request<Texture2D>($"{FunnyExperience.ModName}/Assets/EquipFrame").Value;
			Main.spriteBatch.Draw(tex, new Vector2(Main.screenWidth - 186, 436), Color.White);

			orig(self);
		}

		private void OffsetFirstTwo(On_ItemSlot.orig_Draw_SpriteBatch_ItemArray_int_int_Vector2_Color orig, SpriteBatch spriteBatch, Item[] inv, int context, int slot, Vector2 position, Color lightColor)
		{
			if (Main.playerInventory && inv == Main.LocalPlayer.inventory && slot < 2)
			{
				position = new Vector2(Main.screenWidth - 240, 430 + slot * 47);
				context = 21;

				// This enables interaction with these slots at the new location.
				// Is it strange this is done in a drawing method? Yes, very.
				// Is this how vanilla does it? yup.
				Rectangle hitbox = new((int)position.X, (int)position.Y, (int)(TextureAssets.InventoryBack.Width() * Main.inventoryScale), (int)(TextureAssets.InventoryBack.Height() * Main.inventoryScale));

				if (hitbox.Contains(Main.MouseScreen.ToPoint()) && !PlayerInput.IgnoreMouseInterface)
				{
					if (!Main.LocalPlayer.inventoryChestStack[slot])
					{
						ItemSlot.LeftClick(inv, 0, slot);
						ItemSlot.RightClick(inv, 0, slot);

						if (Main.mouseLeftRelease && Main.mouseLeft)
							Recipe.FindRecipes();
					}

					Main.LocalPlayer.mouseInterface = true;
					ItemSlot.OverrideHover(inv, 0, slot);
					ItemSlot.MouseHover(inv, 0, slot);
				}

				Texture2D tex = TextureAssets.InventoryBack13.Value;
				spriteBatch.Draw(tex, position, null, new Color(0.25f, 0.25f, 0.5f) * 0.9f, 0, Vector2.Zero, Main.inventoryScale, 0, 0);
			}

			if (Main.playerInventory && inv == Main.LocalPlayer.armor)
			{
				if (slot > 9 + Main.LocalPlayer.extraAccessorySlots)
				{
					position.Y += 288;
					position.X += 47;
				}
			}

			if (Main.playerInventory && inv == Main.LocalPlayer.dye)
			{
				return;
			}

			orig(spriteBatch, inv, context, slot, position, lightColor);
		}
	}
}
