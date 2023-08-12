using Terraria.GameContent;
using Terraria.GameInput;
using Terraria.UI;

namespace FunnyExperience.Core.Systems
{
	internal class InventoryHijak : ModSystem
	{
		/// <summary>
		/// Controls wheather inventory slot interactions can be triggered. Used to prevent vanilla from doing so
		/// when rendering the default equipment inventory. May have some very rare fringe issues with modded GUI
		/// </summary>
		public static bool LockMouseInteractionSetting;

		public int MapOffset => Main.mapStyle != 1 ? -256 : 0;

		public override void Load()
		{
			On_ItemSlot.Draw_SpriteBatch_ItemArray_int_int_Vector2_Color += OffsetFirstTwo;
			On_Main.DrawInventory += DrawPreview;

			On_ItemSlot.MouseHover_ItemArray_int_int += LockHover;
			On_ItemSlot.OverrideHover_ItemArray_int_int += LockOverride;
			On_ItemSlot.LeftClick_ItemArray_int_int += LockLeft;
			On_ItemSlot.RightClick_ItemArray_int_int += LockRight;
		}

		private void LockHover(On_ItemSlot.orig_MouseHover_ItemArray_int_int orig, Item[] inv, int context, int slot)
		{
			if (!LockMouseInteractionSetting)
				orig(inv, context, slot);
		}

		private void LockOverride(On_ItemSlot.orig_OverrideHover_ItemArray_int_int orig, Item[] inv, int context, int slot)
		{
			if (!LockMouseInteractionSetting)
				orig(inv, context, slot);
		}

		private void LockLeft(On_ItemSlot.orig_LeftClick_ItemArray_int_int orig, Item[] inv, int context, int slot)
		{
			if (!LockMouseInteractionSetting)
				orig(inv, context, slot);
		}

		private void LockRight(On_ItemSlot.orig_RightClick_ItemArray_int_int orig, Item[] inv, int context, int slot)
		{
			if (!LockMouseInteractionSetting)
				orig(inv, context, slot);
		}

		private void DrawPreview(On_Main.orig_DrawInventory orig, Main self)
		{
			Vector2 pos = Main.LocalPlayer.Center + Vector2.UnitY * Main.LocalPlayer.gfxOffY - Main.screenPosition;
			var source = new Rectangle((int)pos.X - 42, (int)pos.Y - 64, 84, 128);
			Main.spriteBatch.Draw(Main.screenTarget, new Rectangle(Main.screenWidth - 186, 436 + MapOffset, 84, 128), source, Color.White);

			Texture2D tex = ModContent.Request<Texture2D>($"{FunnyExperience.ModName}/Assets/EquipFrame").Value;
			Main.spriteBatch.Draw(tex, new Vector2(Main.screenWidth - 192, 424 + MapOffset), Color.White);

			LockMouseInteractionSetting = true;

			orig(self);

			LockMouseInteractionSetting = false;
		}

		private void OffsetFirstTwo(On_ItemSlot.orig_Draw_SpriteBatch_ItemArray_int_int_Vector2_Color orig, SpriteBatch spriteBatch, Item[] inv, int context, int slot, Vector2 position, Color lightColor)
		{
			LockMouseInteractionSetting = false;

			// Move the first 2 inventory slots next to the equipment area
			if (Main.playerInventory && inv == Main.LocalPlayer.inventory && slot < 2)
			{
				position = new Vector2(Main.screenWidth - 240, 430 + slot * 47 + MapOffset);
				context = 21;

				TriggerMouseInteractions(inv, slot, position);

				Texture2D tex = TextureAssets.InventoryBack13.Value;
				spriteBatch.Draw(tex, position, null, new Color(0.25f, 0.25f, 0.5f) * 0.9f, 0, Vector2.Zero, Main.inventoryScale, 0, 0);
			}

			// Move the rest of the hotbar left 1 to compensate
			if (Main.playerInventory && inv == Main.LocalPlayer.inventory && slot >= 2 && slot <= 9)
			{
				position.X -= 47;
				TriggerMouseInteractions(inv, slot, position);
			}

			// Move the armor slots around
			if (Main.playerInventory && inv == Main.LocalPlayer.armor)
			{
				// Re-enable armor slots
				if (slot < 3 && context == 8) // Checking context here is a bit of a hack so we dont double-bind clickboxes with gear that re-draws
				{
					TriggerMouseInteractions(inv, slot, position, slot > 2 ? 10 : 8);
				}

				// Modify the accessory slots
				if (slot >= 3 && slot <= 9 + Main.LocalPlayer.extraAccessorySlots)
				{
					// Only allow 3 slots
					if (slot > 5)
					{
						LockMouseInteractionSetting = true;
						return;
					}

					position.X -= (slot - 3) * 47;
					position.Y = 578 + MapOffset;

					TriggerMouseInteractions(inv, slot, position, 10);
				}

				// Shift down the vanity slots
				if (slot > 9 + Main.LocalPlayer.extraAccessorySlots)
				{
					if (slot > 15 + Main.LocalPlayer.extraAccessorySlots)
					{
						LockMouseInteractionSetting = true;
						return;
					}

					position.Y += 288;
					position.X += 47;

					if (slot > 12 + Main.LocalPlayer.extraAccessorySlots)
					{
						position.Y -= 146;
						position.X -= 47;
					}

					TriggerMouseInteractions(inv, slot, position);
				}
			}

			// Remove the dye slots
			if (Main.playerInventory && inv == Main.LocalPlayer.dye)
			{
				LockMouseInteractionSetting = true;
				return;
			}

			// Re-enable all other slots
			if (inv == Main.LocalPlayer.inventory && slot > 9)
				TriggerMouseInteractions(inv, slot, position);

			LockMouseInteractionSetting = true;

			orig(spriteBatch, inv, context, slot, position, lightColor);
		}

		/// <summary>
		/// Activates appropriate behavior for the mouse hovering over an item slot
		/// </summary>
		/// <param name="inv"></param>
		/// <param name="slot"></param>
		/// <param name="position"></param>
		private void TriggerMouseInteractions(Item[] inv, int slot, Vector2 position, int context = 0)
		{
			// This enables interaction with these slots at the new location.
			// Is it strange this is done in a drawing method? Yes, very.
			// Is this how vanilla does it? yup.
			Rectangle hitbox = new((int)position.X, (int)position.Y, (int)(TextureAssets.InventoryBack.Width() * Main.inventoryScale), (int)(TextureAssets.InventoryBack.Height() * Main.inventoryScale));

			if (hitbox.Contains(Main.MouseScreen.ToPoint()) && !PlayerInput.IgnoreMouseInterface)
			{
				if (!Main.LocalPlayer.inventoryChestStack[slot])
				{
					ItemSlot.LeftClick(inv, context, slot);
					ItemSlot.RightClick(inv, context, slot);

					if (Main.mouseLeftRelease && Main.mouseLeft)
						Recipe.FindRecipes();
				}

				Main.LocalPlayer.mouseInterface = true;
				ItemSlot.OverrideHover(inv, context, slot);
				ItemSlot.MouseHover(inv, context, slot);
			}
		}
	}
}
