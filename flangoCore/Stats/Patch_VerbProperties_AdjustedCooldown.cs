using Verse;
using RimWorld;

namespace flangoCore
{
	public static class Patch_VerbProperties_AdjustedCooldown
	{
		public static bool Prefix(VerbProperties __instance, ref float __result, Tool tool, Pawn attacker, Thing equipment)
        {
            if (tool != null)
            {
                __result = tool.AdjustedCooldown(equipment);
                return false;
            }
            if (equipment != null && !__instance.IsMeleeAttack)
            {
                float num = equipment.GetStatValue(StatDefOf.RangedWeapon_Cooldown);
                if (attacker != null)
                {
                    num *= attacker.GetStatValue(DefOf_flangoCore.VerbCooldownFactor);
                }
                __result = num;
                return false;
            }
            __result = __instance.defaultCooldownTime;
            return false;
        }
    }
}
