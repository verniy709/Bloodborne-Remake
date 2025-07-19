using HarmonyLib;
using JetBrains.Annotations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Bloodborne_Remake.HarmonyPatches
{
	[UsedImplicitly]
	[StaticConstructorOnStartup]
	public class PatchMain
	{
		static PatchMain()
		{
			Harmony instance = new Harmony("Bloodborne_Remake.HarmonyPatches");
			instance.PatchAll(Assembly.GetExecutingAssembly());
		}
	}
}
