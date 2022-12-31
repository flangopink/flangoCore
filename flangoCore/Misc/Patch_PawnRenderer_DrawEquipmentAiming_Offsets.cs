using HarmonyLib;
using UnityEngine;
using Verse;

namespace flangoCore
{
    public class Patch_PawnRenderer_DrawEquipmentAiming_Offsets
    {
        [HarmonyPriority(6969)]
        [HarmonyPrefix]
        public static bool Prefix(Thing eq, ref Vector3 drawLoc, Pawn ___pawn)
        {
            if (eq is ThingWithComps_NoStyle) drawLoc += eq.def.graphicData.DrawOffsetForRot(___pawn.Rotation);
            //else drawLoc += (eq.StyleDef != null && eq.StyleDef.graphicData != null) ? eq.StyleDef.graphicData.DrawOffsetForRot(___pawn.Rotation) : eq.def.graphicData.DrawOffsetForRot(___pawn.Rotation);
            return true;
        }
    }
}
