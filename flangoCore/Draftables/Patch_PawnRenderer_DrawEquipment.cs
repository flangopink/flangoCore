using HarmonyLib;
using UnityEngine;
using Verse;

namespace flangoCore
{
    [HarmonyPatch(typeof(PawnRenderer), "DrawEquipment")]
    public static class Patch_PawnRenderer_DrawEquipment
    {
        [HarmonyPrefix]
        public static bool DrawEquipment(PawnRenderer __instance, Pawn ___pawn, ref Vector3 rootLoc, ref Rot4 pawnRotation, ref PawnRenderFlags flags)
        {
            if (!___pawn.Dead && ___pawn.def.HasComp(typeof(CompDraftable)))
            {
                var primary = ___pawn.equipment?.Primary;
                if (primary == null) return false;

                if (___pawn.stances.curStance is Stance_Busy stance_Busy && !stance_Busy.neverAimWeapon && stance_Busy.focusTarg.IsValid && (flags & PawnRenderFlags.NeverAimWeapon) == 0)
                {
                    var targ = stance_Busy.focusTarg;
                    Vector3 vector = (!targ.HasThing) ? targ.Cell.ToVector3Shifted() : targ.Thing.DrawPos;
                    float num = 0f;
                    if ((vector - ___pawn.DrawPos).MagnitudeHorizontalSquared() > 0.001f)
                    {
                        num = (vector - ___pawn.DrawPos).AngleFlat();
                    }
                    __instance.DrawEquipmentAiming(primary, rootLoc, num);
                }
                else
                {
                    __instance.DrawEquipmentAiming(primary, rootLoc, pawnRotation.AsAngle);
                }
                return false;
            }
            return true;
        }
    }
}
