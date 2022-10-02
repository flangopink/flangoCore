using System;
using HarmonyLib;
using Verse;
using RimWorld;

namespace flangoCore
{
	[HarmonyPatch(typeof(VerbProperties), "AdjustedCooldown", new Type[] { typeof(Verb), typeof(Pawn) })]
	public static class Patch_VerbProperties_AdjustedCooldown
	{
		public static void Postfix(ref float __result, Verb ownerVerb)
		{
			if (Controller.settings.enableVCF)
			{
				Pawn casterPawn = ownerVerb.CasterPawn;
				if (casterPawn != null && casterPawn.GetStatValue(StatDefOf_flangoCore.VerbCooldownFactor) != 1f)
				{
					__result *= casterPawn.GetStatValue(StatDefOf_flangoCore.VerbCooldownFactor);
				}
			}
		}
	}
}
