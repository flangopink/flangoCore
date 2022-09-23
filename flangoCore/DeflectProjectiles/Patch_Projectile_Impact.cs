using RimWorld;
using Verse;
using HarmonyLib;
using System.Collections.Generic;

namespace flangoCore
{
    [HarmonyPatch(typeof(Projectile), "Impact")]
    public static class Patch_Projectile_Impact
    {
        [HarmonyPrefix]
        public static bool Prefix(Projectile __instance, ref Thing hitThing)
        {
            if (hitThing is Pawn pawn && pawn.equipment != null && pawn.Drafted)
            {
                var ext = pawn.equipment.Primary?.def.GetModExtension<ModExt_DeflectProjectiles>();
                if (ext == null || (ext.cantDeflect != null && ext.cantDeflect.Contains(__instance.def))) return true;

                float roll = Rand.Value;
                if (roll < ext.deflectChance)
                {
                    __instance.usedTarget = pawn;
                    __instance.intendedTarget = __instance.Launcher;

                    ProjectileHitFlags projectileHitFlags = ProjectileHitFlags.NonTargetWorld;
                    float hitRoll = Rand.Value;
                    if (hitRoll < ext.deflectAccuracy)
                    {
                        projectileHitFlags = ProjectileHitFlags.All;
                    }

                    __instance.Launch(pawn, pawn.DrawPos, __instance.usedTarget, __instance.intendedTarget, projectileHitFlags);

                    if (ext.deflectFleck != null)
                    {
                        var fleck = ext.deflectFleck;
                        Map map = pawn.Map;
                        FleckCreationData dataStatic = FleckMaker.GetDataStatic(pawn.DrawPos, map, fleck);
                        if (ext.fleckRandomRotation) dataStatic.rotation = Rand.Range(0f, 360f);
                        map.flecks.CreateFleck(dataStatic);
                    }

                    return false;
                }
            }
            return true;
        }
    }
}
