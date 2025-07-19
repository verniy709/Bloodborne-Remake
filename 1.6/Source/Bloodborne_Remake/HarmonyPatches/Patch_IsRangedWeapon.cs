using System.Reflection;
using HarmonyLib;
using Verse;

namespace Bloodborne_Remake.HarmonyPatches
{
    [HarmonyPatch]
    public static class Patch_IsRangedWeapon
    {
        private static MethodBase TargetMethod()
        {
            return typeof(ThingDef).GetProperty("IsRangedWeapon", BindingFlags.Instance | BindingFlags.Public)?.GetGetMethod();
        }

        public static bool Prefix(ref bool __result, ThingDef __instance)
        {
            if (__instance.IsWeapon && __instance.HasModExtension<DefModExtension_BloodborneRemakeConsideredMelee>())
            {
                __result = false; 
                return false;     
            }
            return true; 
        }
    }
}
