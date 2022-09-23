using HarmonyLib;
using Verse;
using RimWorld;
using UnityEngine;

namespace flangoCore
{
    [HarmonyPatch(typeof(Thing), "TakeDamage")]
    public static class Patch_Thing_TakeDamage_CustomizableProjectiles
    {
        [HarmonyPrefix]
        public static bool TakeDamage_Prefix(Thing __instance, ref DamageInfo dinfo)
        {
            ModExt_ExtraDamageToRace mExt = dinfo.Def.GetModExtension<ModExt_ExtraDamageToRace>();

            if (mExt != null && __instance is Pawn)
            {
                foreach (FleshTypeMultipliers f in mExt.fleshTypes)
                {
                    if (f.fleshTypeDef == __instance.def.race.FleshType)
                    {
                        dinfo.SetAmount(Mathf.RoundToInt(dinfo.Amount * f.damageMultiplier));
                    }
                }
            }
            return true;
        }
    }
}
