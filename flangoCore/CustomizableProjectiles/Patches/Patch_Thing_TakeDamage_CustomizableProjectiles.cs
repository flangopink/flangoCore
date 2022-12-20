using HarmonyLib;
using Verse;
using RimWorld;
using UnityEngine;
using System.Linq;
using static System.Net.Mime.MediaTypeNames;

namespace flangoCore
{
    [HarmonyPatch(typeof(Thing), "TakeDamage")]
    public static class Patch_Thing_TakeDamage_CustomizableProjectiles
    {
        [HarmonyPrefix]
        public static bool TakeDamage_Prefix(Thing __instance, ref DamageInfo dinfo)
        {
            if (!(__instance is Pawn)) return true;
            Pawn pawn = (Pawn)__instance;

            ModExt_ExtraDamageToRace mExt = dinfo.Def.GetModExtension<ModExt_ExtraDamageToRace>();

            if (mExt != null)
            {
                if (!mExt.fleshTypes.NullOrEmpty())
                {
                    // foreach
                    foreach (FleshTypeMultipliers m in mExt.fleshTypes)
                    {
                        if (m.fleshTypeDef == pawn.RaceProps.FleshType)
                        {
                            dinfo.SetAmount(Mathf.RoundToInt(dinfo.Amount * mExt.globalMultiplier == 0 ? m.multiplier : mExt.globalMultiplier));
                            return true;
                        }
                    }
                }

                if (!mExt.pawnKinds.NullOrEmpty())
                {
                    foreach (PawnKindMultipliers m in mExt.pawnKinds)
                    {
                        if (m.pawnKindDef == pawn.RaceProps.AnyPawnKind)
                        {
                            dinfo.SetAmount(Mathf.RoundToInt(dinfo.Amount * mExt.globalMultiplier == 0 ? m.multiplier : mExt.globalMultiplier));
                            return true;
                        }
                    }
                }
            }
            return true;
        }
    }
}
