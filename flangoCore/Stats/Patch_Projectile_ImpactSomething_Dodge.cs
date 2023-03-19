using HarmonyLib;
using Verse;
using RimWorld;

namespace flangoCore
{
    [HarmonyPatch(typeof(Projectile), "ImpactSomething")]
    public static class Patch_Projectile_ImpactSomething_Dodge
    {
        [HarmonyPrefix]
        public static bool Prefix(Projectile __instance)
        {
            if (__instance.usedTarget.Thing is Pawn pawn && __instance.def.projectile.explosionRadius == 0)
            {
                float dodgeChance = pawn.GetStatValue(DefOf_flangoCore.RangedDodgeChance);
                if (dodgeChance == 0) return true;
                float roll = Rand.Value;
                if (roll < dodgeChance)
                {
                    __instance.Destroy();
                    MoteMaker.ThrowText(pawn.Position.ToVector3(), pawn.Map, "TextMote_Dodge".Translate());
                    return false;
                }
            }
            return true;
        }
    }

    [StaticConstructorOnStartup]
    public static class InitBaseDodgeChance
    {
        static InitBaseDodgeChance()
        {
            DefOf_flangoCore.RangedDodgeChance.defaultBaseValue = FlangoCore.settings.BaseDodgeChance;
        }
    }
}
