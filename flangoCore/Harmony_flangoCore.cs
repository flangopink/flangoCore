using Verse;
using HarmonyLib;
using System;
using System.Linq;
using RimWorld;
using UnityEngine;

namespace flangoCore
{
    [StaticConstructorOnStartup]
    public class Harmony_flangoCore
    {
        static Harmony_flangoCore()
        {
            Log.Message("<color=#FFC0CB>flangoCore is starting...</color>");

            var harmony = new Harmony("com.flangopink.flangoCore");

            harmony.PatchAll();

            if (ModLister.HasActiveModWithName("Vanilla Expanded Framework") && FlangoCore.settings.enableVFEPatches)
            {
                Log.Message("<color=#FFC0CB>Vanilla Expanded Framework detected. Patching...</color>");
                VFEPatches(harmony);
            }

            if (FlangoCore.settings.enableVCF) EnableVCF(harmony);
            if (FlangoCore.settings.enableDraftables) EnableDraftables(harmony);
            //if (FlangoCore.settings.enableAnimatedWeapons) EnableAnimations(harmony);

            Log.Message($"<color=#FFC0CB>Launched successfully! Harmony patches: {harmony.GetPatchedMethods().Select(Harmony.GetPatchInfo).SelectMany((Patches p) => p.Prefixes.Concat(p.Postfixes).Concat(p.Transpilers)).Count((Patch p) => p.owner == harmony.Id)}\nThank you for using flangoCore!</color>");
        }

        static void VFEPatches(Harmony harmony)
        {
            var hoppersOrig = typeof(ItemProcessor.Building_ItemProcessor).GetMethod("CheckTheHoppers");
            var hoppersPrefix = typeof(Patch_Building_ItemProcessor_CheckTheHoppers).GetMethod("Prefix");

            harmony.Patch(hoppersOrig, new HarmonyMethod(hoppersPrefix));
        }

        static void EnableVCF(Harmony harmony)
        {
            var VCFOrig = typeof(VerbProperties).GetMethod("AdjustedCooldown", new Type[] { typeof(Tool), typeof(Pawn), typeof(Thing) });
            var VCFPrefix = typeof(Patch_VerbProperties_AdjustedCooldown).GetMethod("Prefix");

            harmony.Patch(VCFOrig, new HarmonyMethod(VCFPrefix));
        }

        static void EnableDraftables(Harmony harmony)
        {
            harmony.Patch(typeof(Bill).GetMethod("PawnAllowedToStartAnew"), new HarmonyMethod(typeof(Patch_Bill_PawnAllowedToStartAnew).GetMethod("AvoidBillErrorIfPawnIsNonHuman")));
            harmony.Patch(AccessTools.Method(typeof(DamageWorker_AddInjury), "FinalizeAndAddInjury", new Type[] { typeof(Pawn), typeof(Hediff_Injury), typeof(DamageInfo), typeof(DamageWorker.DamageResult) }), postfix: new HarmonyMethod(typeof(Patch_DamageWorker_AddInjury_FinalizeAndAddInjury).GetMethod("Postfix")));
            harmony.Patch(typeof(Pawn_EquipmentTracker).GetMethod("DestroyEquipment"), new HarmonyMethod(typeof(Patch_EquipmentTracker_DestroyEquipment).GetMethod("Prefix")));
            harmony.Patch(typeof(MechanitorUtility).GetMethod("EverControllable"), new HarmonyMethod(typeof(Patch_MechanitorUtility_EverControllable).GetMethod("EverControllable")));
            harmony.Patch(typeof(Pawn).GetMethod("get_IsColonistPlayerControlled"), postfix: new HarmonyMethod(typeof(Patch_Pawn_IsColonistPlayerControlled).GetMethod("AddPawnAsColonist")));
            harmony.Patch(typeof(Pawn).GetMethod("WorkTypeIsDisabled"), postfix: new HarmonyMethod(typeof(Patch_Pawn_WorkTypeIsDisabled).GetMethod("RemoveTendFromPawns")));
            harmony.Patch(typeof(PawnComponentsUtility).GetMethod("AddAndRemoveDynamicComponents"), postfix: new HarmonyMethod(typeof(Patch_PawnComponentsUtility_AddAndRemoveDynamicComponents).GetMethod("AddDraftability")));
            harmony.Patch(AccessTools.Method(typeof(PawnRenderer), "DrawEquipment"), new HarmonyMethod(typeof(Patch_PawnRenderer_DrawEquipment).GetMethod("DrawEquipment")));
            harmony.Patch(AccessTools.Method(typeof(FloatMenuMakerMap), "CanTakeOrder"), postfix: new HarmonyMethod(typeof(Patch_FloatMenuMakerMap_CanTakeOrder).GetMethod("MakePawnControllable")));
            harmony.Patch(AccessTools.Method(typeof(FloatMenuMakerMap), "AddUndraftedOrders"), new HarmonyMethod(typeof(Patch_FloatMenuMakerMap_AddUndraftedOrders).GetMethod("AvoidGeneralErrorIfPawnIsAnimal")));
            //harmony.Patch(AccessTools.Method(typeof(FloatMenuMakerMap), "AddDraftedOrders"), transpiler: new HarmonyMethod(typeof(FloatMenuMakerMap_AddDraftedOrders_Transpiler).GetMethod("AddDraftedOrders_Transpiler")));
        }

        /*static void EnableAnimations(Harmony harmony)
        {
            // DrawEquipment is private
            harmony.Patch(AccessTools.Method(typeof(PawnRenderer), "DrawEquipment"), new HarmonyMethod(typeof(Patch_PawnRenderer_DrawEquipment_Anim).GetMethod("Prefix"), 100));
            harmony.Patch(typeof(Pawn_EquipmentTracker).GetMethod("Notify_EquipmentAdded"), postfix: new HarmonyMethod(typeof(Patch_Pawn_EquipmentTracker_Notify_EquipmentAdded_Anim).GetMethod("Postfix")));
            harmony.Patch(typeof(Pawn_EquipmentTracker).GetMethod("Notify_EquipmentRemoved"), postfix: new HarmonyMethod(typeof(Patch_Pawn_EquipmentTracker_Notify_EquipmentRemoved_Anim).GetMethod("Postfix")));
            harmony.Patch(typeof(Pawn_EquipmentTracker).GetMethod("EquipmentTrackerTick"), postfix: new HarmonyMethod(typeof(Patch_Pawn_EquipmentTracker_EquipmentTrackerTick).GetMethod("Postfix")));
        }*/
    }
}
