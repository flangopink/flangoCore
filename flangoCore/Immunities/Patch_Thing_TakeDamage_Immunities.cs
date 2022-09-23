using HarmonyLib;
using Verse;
using RimWorld;

namespace flangoCore
{
	[HarmonyPatch(typeof(Thing), "TakeDamage")]
	public static class Patch_Thing_TakeDamage_Immunities
	{
		[HarmonyPrefix]
		public static bool Immunities_TakeDamage(Thing __instance, ref DamageWorker.DamageResult __result, DamageInfo dinfo)
		{
			if (!__instance.def.HasComp(typeof(CompDamageImmunities)))
			{
				return true;
			}

			if (__instance is Building || __instance.Destroyed)
			{
				return true;
			}

			if (__instance.TryGetCompFast<CompDamageImmunities>().Props.damageDefs.Contains(dinfo.Def))
			{
				var props = __instance.TryGetCompFast<CompDamageImmunities>().Props;
				__result = new DamageWorker.DamageResult();
				if (props.throwText)
					MoteMaker.ThrowText(__instance.Position.ToVector3() + props.textOffset, __instance.Map, "flangoCore_Immune".Translate(dinfo.Def.label), props.textColor, props.textDuration);

				return false;
			}

			return true;
		}
	}
}
