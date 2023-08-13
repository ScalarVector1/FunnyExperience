using FunnyExperience.API.Sorting;
using FunnyExperience.Content.Edits.Detours;
using FunnyExperience.Core.Mechanics;

namespace FunnyExperience.Core.Systems.CustomSpawnsSystem
{
	public class CustomSpawnsSystem : GlobalNPC
	{
		public NpcStatistics Stats;

		private readonly float _endurance;

		internal string NamePrefix;

		public CustomSpawnsSystem(float endurance)
		{
			_endurance = endurance;
		}

		private bool DelayedStatAssignment { get; set; }

		//--NEEDS LOOKING--//
		//public override bool CloneNewInstances => true;

		public override bool InstancePerEntity => true;

		//AppliesToEntity is called by NPCLoader.SetDefaults... Perfect!
		public override bool AppliesToEntity(NPC entity, bool lateInstantiation)
		{
			int idOverride = VanillaNpcDetour.NetIDOverride;
			int netID = idOverride != 0 ? idOverride : entity.type;
			return lateInstantiation && (NpcStatisticsRegistry.HasStats(netID) ||
			                             NPCProgressionRegistry.NonSeparableWormNPCToHead.ContainsKey(netID));
		}

		public override void SetDefaults(NPC npc)
		{
			//Use "netID" instead of "type" to support the different net types (e.g. slimes and zombies)
			int idOverride = VanillaNpcDetour.NetIDOverride;
			int netID = idOverride != 0 ? idOverride : npc.type;

			//Don't handle the rest of this during initialization
			if (Main.gameMenu)
				return;

			if (VanillaNpcDetour.TransformingNPC && NPCProgressionRegistry.TransformingNPCs.Contains(netID))
			{
				//Transforming NPCs should carry the stats
				Stats = VanillaNpcDetour.PreTransformStats;
			}
			else if (NPCProgressionRegistry.NonSeparableWormNPCToHead.TryGetValue(netID, out _))
			{
				//Use the stats from the head NPC, unless this NPC isn't being spawned by the head
				//Indicate that the NPC needs stats, but getting them should be delayed
				DelayedStatAssignment = true;
				return;
			}
			else
			{
				NpcStatisticsRegistry.Entry entry = NpcStatisticsRegistry.GetRandomStats(netID);
				//--NEEDS LOOKING--//
				//-- Entry was null on 1.4.3 loading which threw null reference --//
				if (entry == null) { return; }

				Stats = entry?.Stats;
				NamePrefix = entry.NamePrefix is null ? "" : (entry.NamePrefix + " ");
			}

			ApplyStatsAndNamePrefix(npc, netID);
		}

		public override void PostAI(NPC npc)
		{
			if (!npc.active || !DelayedStatAssignment)
				return;

			DelayedStatAssignment = false;

			if (Stats is null &&
			    NPCProgressionRegistry.NonSeparableWormNPCToHead.TryGetValue(npc.netID, out int headType))
			{
				//All vanilla worms set the "head" NPC's whoAmI to npc.ai[3] and npc.realLife
				if (npc.realLife >= 0 && npc.realLife == npc.ai[3])
				{
					NPC headNPC = Main.npc[npc.realLife];

					if (headNPC.active && headNPC.type == headType &&
					    headNPC.TryGetGlobalNPC(out CustomSpawnsSystem headStats))
					{
						Stats = headStats.Stats;

						ApplyStatsAndNamePrefix(npc, npc.netID);
					}
				}
			}
		}

		private void ApplyStatsAndNamePrefix(NPC npc, int netID)
		{
			if (Stats is not null)
				Stats.ApplyTo(npc);

			ApplyNamePrefix(npc, netID);
		}

		internal void ApplyNamePrefix(NPC npc, int netID, bool prependLevel = true)
		{
			if (Stats is not null)
			{
				string lvl = prependLevel ? $"[Health: {Stats.HealthModifier}] " : "";
				string netName = Lang.GetNPCNameValue(netID);

				//Ensure that names like "The Groom" end up as "The Large Groom" instead of "Large The Groom"
				// TODO: localization support
				if (netName.StartsWith("The "))
					npc.GivenName = $"{lvl}The {NamePrefix}{netName[4..]}";
				else
					npc.GivenName = $"{lvl}{NamePrefix}{netName}";
			}
		}

		public override void ModifyHitByItem(NPC npc, Player player, Item item, ref NPC.HitModifiers modifiers)
		{
			// ApplyEndurance(ref damage);
			//-- TEMP SOLVE --//
			//-- SEE https://github.com/tModLoader/tModLoader/blob/1.4.4/ExampleMod/Content/Items/Weapons/HitModifiersShowcase.cs#L123 --//
			modifiers.ModifyHitInfo += (ref NPC.HitInfo hitInfo) =>
			{
				hitInfo.Damage = ApplyEndurance(hitInfo.Damage);
			};
		}

		public override void ModifyHitByProjectile(NPC npc, Projectile projectile, ref NPC.HitModifiers modifiers)
		{
			// ApplyEndurance(ref damage);
			//-- TEMP SOLVE --//
			//-- SEE https://github.com/tModLoader/tModLoader/blob/1.4.4/ExampleMod/Content/Items/Weapons/HitModifiersShowcase.cs#L123 --//
			modifiers.ModifyHitInfo += (ref NPC.HitInfo hitInfo) =>
			{
				hitInfo.Damage = ApplyEndurance(hitInfo.Damage);
			};
		}

		public override void ModifyHitNPC(NPC npc, NPC target, ref NPC.HitModifiers modifiers)
		{
			/*
			 * if(target.TryGetGlobalNPC<StatNPC>(out var statNPC))
			 *   statNPC.ApplyEndurance(ref damage);
			 */
			//-- TEMP SOLVE --//
			//-- SEE https://github.com/tModLoader/tModLoader/blob/1.4.4/ExampleMod/Content/Items/Weapons/HitModifiersShowcase.cs#L123 --//
			modifiers.ModifyHitInfo += (ref NPC.HitInfo hitInfo) =>
			{
				hitInfo.Damage = ApplyEndurance(hitInfo.Damage);
			};
		}

		private int ApplyEndurance(int damage)
		{
			return Math.Max(1, (int)(damage * (1f - Math.Min(0.9999f, _endurance))));
		}

		public override void OnKill(NPC npc)
		{
			// npc.TryGetGlobalNPC<StatNPC>(out var statNPC);
			// if(statNPC.stats is not null && !npc.SpawnedFromStatue){
			// 	for(int i = 0; i < Main.maxPlayers; i++){
			// 		Player player = Main.player[i];
			//
			// 		if(!player.active || player.dead || !npc.playerInteraction[i])
			// 			continue;
			//
			// 		StatPlayer statPlayer = player.GetModPlayer<StatPlayer>();
			// 		int count = 1;
			//
			// 		bool hasCount = npc.boss && statPlayer.downedCountsByID.TryGetValue((short)npc.netID, out count);
			// 		//Killing more of the same boss gives the player less and less XP
			// 		int xp = statNPC.stats.xp;
			// 		if(hasCount)
			// 			xp = (int)(xp * 1f / (count + 1));
			//
			// 		//Spawn the experience
			// 		if(Main.netMode == NetmodeID.SinglePlayer)
			// 			ExperienceTracker.SpawnExperience(xp, npc.Center, 6f, player.whoAmI);
			// 		else
			// 			Networking.SendSpawnExperienceOrbs(-1, player.whoAmI, xp, npc.Center, 6f);
			//
			// 		if(npc.boss){
			// 			if(!hasCount)
			// 				statPlayer.downedCountsByID.Add((short)npc.netID, 0);
			//
			// 			statPlayer.downedCountsByID[(short)npc.netID]++;
			// 		}
			// 	}
			// }
		}
	}
}