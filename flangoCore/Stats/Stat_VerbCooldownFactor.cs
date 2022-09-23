using System;
using HarmonyLib;
using Verse;
using RimWorld;

namespace flangoCore
{
	[HarmonyPatch(typeof(VerbProperties), "AdjustedCooldown", new Type[] { typeof(Verb), typeof(Pawn) })]
	public static class VerbProperties_AdjustedCooldown_Patch
	{
		public static void Postfix(ref float __result, Verb ownerVerb)
		{
			Pawn casterPawn = ownerVerb.CasterPawn;
			if (casterPawn != null)
			{
				__result *= casterPawn.GetStatValue(VCFDefOf.VerbCooldownFactor);
			}
		}
	}

	[DefOf]
	public static class VCFDefOf
	{
		public static StatDef VerbCooldownFactor;
	}
}
