using HarmonyLib;
using Verse;
using RimWorld;
using UnityEngine;

namespace flangoCore
{
    [HarmonyPatch(typeof(Projectile), "ImpactSomething")]
    public static class Patch_Projectile_ImpactSomething_Dodge
    {
        [HarmonyPrefix]
        public static bool Prefix(Projectile __instance)
        {
            if (__instance.usedTarget.Thing is Pawn pawn)
            {
                _ = FlangoCore.settings.BaseDodgeChance;
                if (__instance.def.projectile.explosionRadius != 0 && pawn.GetStatValue(StatDefOf_flangoCore.RangedDodgeChance) is float dodgeChance && dodgeChance != 0)
                {
                    float roll = Rand.Value;
                    if (roll < dodgeChance)
                    {
                        __instance.Destroy();
                        MoteMaker.ThrowText(pawn.Position.ToVector3(), pawn.Map, "TextMote_Dodge".Translate());
                        return false;
                    }
                }
            }
            return true;
        }
    }
}
