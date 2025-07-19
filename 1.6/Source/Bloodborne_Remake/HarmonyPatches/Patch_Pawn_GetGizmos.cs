using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HarmonyLib;
using RimWorld;
using Verse;

namespace Bloodborne_Remake.HarmonyPatches
{
    [HarmonyPatch(typeof(Pawn), "GetGizmos")]
    public static class Patch_Pawn_GetGizmos
    {
        public static IEnumerable<Gizmo> Postfix(IEnumerable<Gizmo> __result, Pawn __instance)
        {
            foreach (var g in __result)
                yield return g;

            if (__instance.Drafted &&
                __instance.equipment?.Primary?.def.HasModExtension<DefModExtension_BloodborneRemakeConsideredMelee>() == true)
            {
                yield return new Command_Toggle
                {
                    defaultLabel = "CommandFireAtWillLabel".Translate(),
                    icon = TexCommand.FireAtWill,
                    hotKey = KeyBindingDefOf.Misc6,
                    isActive = () => __instance.drafter?.FireAtWill ?? false,
                    toggleAction = () =>
                    {
                        if (__instance.drafter != null)
                            __instance.drafter.FireAtWill = !__instance.drafter.FireAtWill;
                    }
                };

                Verb verb = __instance.verbTracker?.AllVerbs?
                     .FirstOrDefault(v => v.IsMeleeAttack && v.Available());
                if (verb != null)
                {
                    yield return new Command_VerbTarget
                    {
                        defaultLabel = "CommandMeleeAttack".Translate(),
                        icon = TexCommand.AttackMelee,
                        hotKey = KeyBindingDefOf.Misc1,
                        verb = verb
                    };
                }
            }
        }
    }
}
