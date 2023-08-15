using FunnyExperience.Core.Systems.TreeSystem;
using System.Linq;
using Terraria.Audio;
using Terraria.ModLoader.IO;

namespace FunnyExperience.Core.Systems
{
	public class AltUseSystem : ModPlayer
	{
		public int AltFunctionCooldown = 0;

		public override void ResetEffects()
		{
			if (AltFunctionCooldown > 0)
				AltFunctionCooldown--;
		}
	}
}
