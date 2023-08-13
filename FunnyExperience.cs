global using Microsoft.Xna.Framework;
global using Microsoft.Xna.Framework.Graphics;
global using System;
global using Terraria;
global using Terraria.ModLoader;
using FunnyExperience.API.Sorting;
using FunnyExperience.Content.Edits;
using FunnyExperience.Core.Systems.CustomSpawnsSystem;

namespace FunnyExperience
{
	public class FunnyExperience : Mod
	{
		public static FunnyExperience Instance;
		public const string ModName = "FunnyExperience";

		public FunnyExperience()
		{
			Instance = this;
		}
		
		public override void Load(){
			EditsLoader.Load();
			NPCProgressionRegistry.Load();
			NpcStatisticsRegistry.Load();
		}
		
		public override void Unload(){
			NpcStatisticsRegistry.Unload();
			NPCProgressionRegistry.Unload();
		}
		
		public override void PostSetupContent(){
			NpcStatisticsRegistry.PostSetupContent();
		}
	}
}