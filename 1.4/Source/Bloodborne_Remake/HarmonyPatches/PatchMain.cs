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
		// Token: 0x06000387 RID: 903 RVA: 0x00013EA8 File Offset: 0x000120A8
		static PatchMain()
		{
			Harmony instance = new Harmony("Bloodborne_Remake_HarmonyPatches");
			instance.PatchAll(Assembly.GetExecutingAssembly());
		}
	}
}
