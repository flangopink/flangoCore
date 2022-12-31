using HarmonyLib;
using HotSwap;
using RimWorld;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace flangoCore
{
    [HotSwappable]
    [HarmonyPatch(typeof(PawnRenderer), "DrawEquipment")]
    public class Patch_PawnRenderer_DrawEquipment_Anim
    {
        [HarmonyPriority(100)]
        [HarmonyPrefix]
        public static bool DrawEquipment(PawnRenderer __instance, Pawn ___pawn, ref Vector3 rootLoc, ref Rot4 pawnRotation, ref PawnRenderFlags flags)
        {
            if (___pawn.Dead || !___pawn.Spawned || (___pawn.CurJob != null && ___pawn.CurJob.def.neverShowWeapon))
                return false;

            var primary = ___pawn.equipment?.Primary;
            if (primary?.GetComp<CompAnimatedWeapon>() is CompAnimatedWeapon comp)
            {
                Vector3 drawLoc = new(0f, (pawnRotation == Rot4.North) ? (-0.00289575267f) : 0.03474903f, 0f);
                float equipmentDrawDistanceFactor = ___pawn.ageTracker.CurLifeStage.equipmentDrawDistanceFactor;
                if (___pawn.stances.curStance is Stance_Busy stance_Busy && !stance_Busy.neverAimWeapon && stance_Busy.focusTarg.IsValid && (flags & PawnRenderFlags.NeverAimWeapon) == 0)
                {
                    var targ = stance_Busy.focusTarg;
                    Vector3 vector = (!targ.HasThing) ? targ.Cell.ToVector3Shifted() : targ.Thing.DrawPos;
                    float num = 0f;
                    if ((vector - ___pawn.DrawPos).MagnitudeHorizontalSquared() > 0.001f)
                    {
                        num = (vector - ___pawn.DrawPos).y;
                    }

                    /*Verb currentEffectiveVerb = ___pawn.CurrentEffectiveVerb;
                    if (currentEffectiveVerb != null && currentEffectiveVerb.AimAngleOverride.HasValue)
                    {
                        num = currentEffectiveVerb.AimAngleOverride.Value;
                    }*/
                    drawLoc += rootLoc;// + new Vector3(0f, 0f, 0.4f + primary.def.equippedDistanceOffset).RotatedBy(num) * equipmentDrawDistanceFactor;

                    comp.active = true;
                    var co = comp.offset;
                    float rot = comp.rot;
                    Vector3 offset = Vector3.zero;

                    if (pawnRotation == Rot4.South)
                    {
                        offset = new Vector3(co.z, 0, -co.x);
                    }
                    else if (pawnRotation == Rot4.North)
                    {
                        offset = new Vector3(-co.z, 0, co.x);
                    }
                    else if (pawnRotation == Rot4.West)
                    {
                        offset = new Vector3(-co.x, 0, -co.z);
                    }
                    else if (pawnRotation == Rot4.East)
                    {
                        offset = new Vector3(co.x, 0, co.z);
                    }

                    DrawEquipmentAiming(primary, drawLoc + offset, num + rot, pawnRotation);
                }
                else if (__instance.CarryWeaponOpenly())
                {
                    if (pawnRotation == Rot4.South)
                    {
                        drawLoc += rootLoc + new Vector3(0f, 0f, -0.22f) * equipmentDrawDistanceFactor;
                        __instance.DrawEquipmentAiming(___pawn.equipment.Primary, drawLoc, 143f);
                    }
                    else if (pawnRotation == Rot4.North)
                    {
                        drawLoc += rootLoc + new Vector3(0f, 0f, -0.11f) * equipmentDrawDistanceFactor;
                        __instance.DrawEquipmentAiming(___pawn.equipment.Primary, drawLoc, 143f);
                    }
                    else if (pawnRotation == Rot4.East)
                    {
                        drawLoc += rootLoc + new Vector3(0.2f, 0f, -0.22f) * equipmentDrawDistanceFactor;
                        __instance.DrawEquipmentAiming(___pawn.equipment.Primary, drawLoc, 143f);
                    }
                    else if (pawnRotation == Rot4.West)
                    {
                        drawLoc += rootLoc + new Vector3(-0.2f, 0f, -0.22f) * equipmentDrawDistanceFactor;
                        __instance.DrawEquipmentAiming(___pawn.equipment.Primary, drawLoc, 217f);
                    }
                }
                return false;
            }
            return true;
        }

        public static void DrawEquipmentAiming(Thing eq, Vector3 drawLoc, float aimAngle, Rot4 pawnRot)
        {
            Mesh mesh;
            float num = aimAngle - 90f;
            if (pawnRot == Rot4.North)
            {
                mesh = MeshPool.plane10Flip;
                num += 90;
            }
            else if (pawnRot == Rot4.South)
            {
                mesh = MeshPool.plane10Flip;
                num -= 90;
            }
            else if (pawnRot == Rot4.West)
            {
                mesh = MeshPool.plane10Flip;
            }
            else
            {
                mesh = MeshPool.plane10;
            }

            num %= 360f;
            CompEquippable compEquippable = eq.TryGetComp<CompEquippable>();
            if (compEquippable != null)
            {
                EquipmentUtility.Recoil(eq.def, EquipmentUtility.GetRecoilVerb(compEquippable.AllVerbs), out var drawOffset, out var angleOffset, aimAngle);
                drawLoc += drawOffset;
                num += angleOffset;
            }

            Material material = ((eq.Graphic is not Graphic_StackCount graphic_StackCount) ? eq.Graphic.MatSingleFor(eq) : graphic_StackCount.SubGraphicForStackCount(1, eq.def).MatSingleFor(eq));
            Matrix4x4 matrix = Matrix4x4.TRS(s: new Vector3(eq.Graphic.drawSize.x, 0f, eq.Graphic.drawSize.y), pos: drawLoc, q: Quaternion.AngleAxis(num, Vector3.up));
            Graphics.DrawMesh(mesh, matrix, material, 0);
        }
    }
}
