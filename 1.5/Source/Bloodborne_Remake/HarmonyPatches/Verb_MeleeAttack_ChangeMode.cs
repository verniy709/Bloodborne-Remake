using HarmonyLib;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Bloodborne_Remake.HarmonyPatches
{
	[StaticConstructorOnStartup]
    public class Verb_MeleeAttack_ChangeMode
	{
		[HarmonyPatch(typeof(Verb_MeleeAttack), "TryCastShot")]
		private static class Verb_MeleeAttack_PreFix
		{
			[HarmonyPrefix]
			private static bool PreFix(Verb_MeleeAttack __instance)
			{
				if(__instance.EquipmentSource is BloodborneWeapon weapon)
                {
					weapon.TryChangeMode(__instance.tool.label);
				}
				return true;
			}
		}
	}
}
