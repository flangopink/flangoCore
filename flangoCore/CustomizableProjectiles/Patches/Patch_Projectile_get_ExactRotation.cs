﻿using HarmonyLib;
using Verse;
using RimWorld;
using UnityEngine;

namespace flangoCore
{
    [HarmonyPatch(typeof(Projectile), "get_ExactRotation")]
    public static class Patch_Projectile_get_ExactRotation
    {
        [HarmonyPrefix]
        public static bool ExactRotation_Prefix(Projectile __instance, ref Quaternion __result)
        {
            if (__instance.def.HasComp(typeof(CompProjectileVFX)))
            {
                CompProperties_ProjectileVFX props = __instance.TryGetCompFast<CompProjectileVFX>().Props;

                if (props.rotating)
                {
                    __result = Quaternion.AngleAxis((props.counterClockwise ? -1 : 1) * Find.TickManager.TicksGame % 360 * props.rotationSpeed, Vector3.up);
                    return false;
                }
            }
            return true;
        }
    }
}
