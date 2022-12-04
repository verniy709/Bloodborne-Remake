using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace Bloodborne_Remake
{
	public class Verb_MeleeAttackDamage_Area : Verb_MeleeAttackDamage
	{
		DefModExtension_AOEMelee MODData => data ?? (data = EquipmentSource.def.GetModExtension<DefModExtension_AOEMelee>());
		protected override bool TryCastShot()
		{
			Pawn casterPawn = this.CasterPawn;
			if (casterPawn.stances.FullBodyBusy)
			{
				return false;
			}
			Map map = Caster.Map;
			IntVec3 targetPos = currentTarget.Cell;
			bool canAOE = currentTarget.Thing is Pawn && Rand.Chance(MODData.chance);
			bool result = false;
			if (base.TryCastShot())
			{
				result = true;
				TargetInfo targetInfo;
				targetInfo = new TargetInfo(currentTarget.Thing);
				TriggerEffecter(targetInfo);
			}
			Func<ManeuverDef, RulePackDef> func_MeleeCombat = (ManeuverDef maneuver) => maneuver.combatLogRulesHit;
			MethodInfo method_GetNonMissChance = typeof(Verb_MeleeAttack).GetMethod("GetNonMissChance", BindingFlags.Instance | BindingFlags.NonPublic);
			MethodInfo method_GetDodgeChance = typeof(Verb_MeleeAttack).GetMethod("GetDodgeChance", BindingFlags.Instance | BindingFlags.NonPublic);
			MethodInfo method_SoundHitPawn = typeof(Verb_MeleeAttack).GetMethod("SoundHitPawn", BindingFlags.Instance | BindingFlags.NonPublic);
			MethodInfo method_SoundDodge = typeof(Verb_MeleeAttack).GetMethod("SoundDodge", BindingFlags.Instance | BindingFlags.NonPublic);
			MethodInfo method_SoundMiss = typeof(Verb_MeleeAttack).GetMethod("SoundMiss", BindingFlags.Instance | BindingFlags.NonPublic);
			if (canAOE)
			{
                List<Pawn> targets1 = GetTargets(targetPos, map);
				foreach (Pawn target in targets1)
				{
					if (target != null && !target.Dead && (casterPawn.MentalStateDef != MentalStateDefOf.SocialFighting || target.MentalStateDef != MentalStateDefOf.SocialFighting) && (casterPawn.story == null || !casterPawn.story.traits.DisableHostilityFrom(target)))
					{
						target.mindState.meleeThreat = casterPawn;
						target.mindState.lastMeleeThreatHarmTick = Find.TickManager.TicksGame;
					}
					Vector3 drawPos = target.DrawPos;
					SoundDef soundDef;
					LocalTargetInfo localTarget = target;
					if (Rand.Chance((float)method_GetNonMissChance.Invoke(this, new object[] { localTarget })))
					{
						if (!Rand.Chance((float)method_GetDodgeChance.Invoke(this, new object[] { localTarget })))
						{
							result = true;
							soundDef = method_SoundHitPawn.Invoke(this, new object[] { }) as SoundDef;
							if (this.verbProps.impactMote != null)
							{
								MoteMaker.MakeStaticMote(drawPos, map, this.verbProps.impactMote, 1f);
							}
							if (this.verbProps.impactFleck != null)
							{
								FleckMaker.Static(drawPos, map, this.verbProps.impactFleck, 1f);
							}
							TriggerEffecter(target);
							//BattleLogEntry_MeleeCombat battleLogEntry_MeleeCombat = CreateCombatLog(func_MeleeCombat, true);
							result = true;
							DamageWorker.DamageResult damageResult = ApplyMeleeDamageToTarget(target);
							/*if (damageResult.stunned && damageResult.parts.NullOrEmpty<BodyPartRecord>())
							{
								Find.BattleLog.RemoveEntry(battleLogEntry_MeleeCombat);
							}
							else
							{
								damageResult.AssociateWithLog(battleLogEntry_MeleeCombat);
								if (damageResult.deflected)
								{
									battleLogEntry_MeleeCombat.RuleDef = this.maneuver.combatLogRulesDeflect;
									battleLogEntry_MeleeCombat.alwaysShowInCompact = false;
								}
							}*/
						}
						else
						{
							soundDef = method_SoundDodge.Invoke(this, new object[] { target }) as SoundDef;
							MoteMaker.ThrowText(drawPos, map, "TextMote_Dodge".Translate(), 1.9f);
							//CreateCombatLog(func_MeleeCombat, true);
						}
					}
					else
					{
						soundDef = method_SoundMiss.Invoke(this, new object[] { }) as SoundDef;
						//CreateCombatLog(func_MeleeCombat, true);
					}
					soundDef.PlayOneShot(new TargetInfo(target.Position, map, false));
					if (target != null && !target.Dead && target.Spawned)
					{
						target.stances.stagger.StaggerFor(95);
					}
				}
			}
			return result;
		}
        private void TriggerEffecter(TargetInfo targetInfo)
        {
			if(targetInfo.Thing is Pawn pawnD && pawnD.Dead)
            {
				targetInfo = new TargetInfo(pawnD.Corpse);
			}
            if ((targetInfo.Thing is Pawn pawn && pawn.RaceProps.IsFlesh)||((targetInfo.Thing is Corpse corpse && corpse.InnerPawn.RaceProps.IsFlesh)))
            {
                if (MODData.effecter_Normal != null)
                {
                    Effecter effecter = MODData.effecter_Normal.Spawn();
                    effecter.Trigger(targetInfo, targetInfo);
                    effecter.Cleanup();
                }
            }
            else
            {
                if (MODData.effecter_Mechanoid != null)
                {
                    Effecter effecter = MODData.effecter_Mechanoid.Spawn();
                    effecter.Trigger(targetInfo, targetInfo);
                    effecter.Cleanup();
                }
            }
        }

        public virtual List<Pawn> GetTargets(IntVec3 Intv3, Map map)
		{
			return (from x in map.mapPawns.AllPawnsSpawned where IsExtraTargets(x, Intv3) select x).ToList();
		}
		public virtual bool IsExtraTargets(Pawn pawn, IntVec3 Intv3)
		{
			return pawn != currentTarget.Thing && Caster.HostileTo(pawn) && pawn.Position.InHorDistOf(Intv3, data.range) && !pawn.Downed;
		}
		public override void DrawHighlight(LocalTargetInfo target)
		{
			base.DrawHighlight(target);
			if (!target.IsValid) return;
			GenDraw.DrawFieldEdges(GetRangeNow(target.Cell), new Color(79f / 255f, 249f / 255f, 239f / 255f, 0.75f));
			foreach (Pawn x in GetTargets(target.Cell, target.Thing.Map))
			{
				GenDraw.DrawTargetHighlight(new LocalTargetInfo(x));
			}
		}
		private List<IntVec3> GetRangeNow(IntVec3 center)
		{
			rangeNow.Clear();
			foreach(IntVec3 cell in GetRange())
            {
				rangeNow.Add(cell+center);
            }
			return rangeNow;
		}
		private List<IntVec3> GetRange()
		{
			if(range == null)
            {
				range = new List<IntVec3>();
				int num = GenRadial.NumCellsInRadius(MODData.range);
				for (int i = 0; i < num; i++)
				{
                    range.Add(GenRadial.RadialPattern[i]);
				}
			}
			return range;
		}
		private DefModExtension_AOEMelee data;
		private List<Pawn> targets = new List<Pawn>();
		private List<IntVec3> rangeNow = new List<IntVec3>();
		private List<IntVec3> range;
	}
}
