using Verse;
using RimWorld;

namespace flangoCore
{
	public static class Patch_VerbProperties_AdjustedCooldown
	{
		public static void Postfix(ref float __result, Verb ownerVerb)
		{
			Pawn casterPawn = ownerVerb.CasterPawn;
			if (casterPawn != null && casterPawn.GetStatValue(StatDefOf_flangoCore.VerbCooldownFactor) != 1f)
			{
				__result *= casterPawn.GetStatValue(StatDefOf_flangoCore.VerbCooldownFactor);
			}
		}
	}
}
