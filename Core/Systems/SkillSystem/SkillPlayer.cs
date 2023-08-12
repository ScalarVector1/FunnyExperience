using FunnyExperience.Content.Items.Gear;
using FunnyExperience.Content.Skills.Melee;
using Microsoft.Xna.Framework.Input;
using Terraria.GameInput;

namespace FunnyExperience.Core.Systems.SkillSystem
{
	internal class SkillPlayer : ModPlayer
	{
		private static ModKeybind _skill1Keybind;
		private static ModKeybind _skill2Keybind;
		private static ModKeybind _skill3Keybind;
		private static ModKeybind _skill4Keybind;
		private static ModKeybind _skill5Keybind;
		public Skill[] Skills = new Skill[5];
		
		public override void Load() {
			if (Main.dedServ)
				return;

			_skill1Keybind = KeybindLoader.RegisterKeybind(Mod, "UseSkill1", Keys.Q);
			_skill2Keybind = KeybindLoader.RegisterKeybind(Mod, "UseSkill2", Keys.W);
			_skill3Keybind = KeybindLoader.RegisterKeybind(Mod, "UseSkill3", Keys.E);
			_skill4Keybind = KeybindLoader.RegisterKeybind(Mod, "UseSkill4", Keys.R);
			_skill5Keybind = KeybindLoader.RegisterKeybind(Mod, "UseSkill5", Keys.T);
		}
		
		public override void ProcessTriggers(TriggersSet triggersSet) 
		{
			if (_skill1Keybind.JustPressed)
			{
				Skills[0] = new Berserk(1200, 1200, 1200, 1200, 5, GearType.Sword);
				Skills[0]?.UseSkill(Main.LocalPlayer);
			}
    
			if (_skill2Keybind.JustPressed)
				Skills[1]?.UseSkill(Main.LocalPlayer);
    
			if (_skill3Keybind.JustPressed)
				Skills[2]?.UseSkill(Main.LocalPlayer);
    
			if (_skill4Keybind.JustPressed)
				Skills[3]?.UseSkill(Main.LocalPlayer);
    
			if (_skill5Keybind.JustPressed)
				Skills[4]?.UseSkill(Main.LocalPlayer);
		}
		
		public delegate void OnHitByNPCDelegate(Player player, NPC npc, Player.HurtInfo hurtInfo);
		public static event OnHitByNPCDelegate OnHitByNPCEvent;
		public override void OnHitByNPC(NPC npc, Player.HurtInfo hurtInfo)
		{
			OnHitByNPCEvent?.Invoke(Player, npc, hurtInfo);
		}
	}
}
