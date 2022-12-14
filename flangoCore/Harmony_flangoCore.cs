using Verse;
using HarmonyLib;
using System;
using System.Linq;
using RimWorld;

namespace flangoCore
{
    [StaticConstructorOnStartup]
    public class Harmony_flangoCore
    {
        static Harmony_flangoCore()
        {
            Log.Message("<color=#FFC0CB>flangoCore is starting...</color>");

            var harmony = new Harmony("com.flangopink.flangoCore");

            harmony.Patch(AccessTools.Method(typeof(EquipmentUtility), "CanEquip", 
                new Type[] { typeof(Thing), typeof(Pawn), typeof(string).MakeByRefType(), typeof(bool)}), postfix: new HarmonyMethod(typeof(Patch_EquipmentUtility_CanEquip).GetMethod("Postfix")));

            harmony.PatchAll();

            if (ModLister.HasActiveModWithName("Vanilla Expanded Framework") && FlangoCore.settings.enableVFEPatches)
            {
                Log.Message("<color=#FFC0CB>Vanilla Expanded Framework detected. Patching...</color>");
                VFEPatches(harmony);
            }

            if (FlangoCore.settings.enableVCF) EnableVCF(harmony);

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
    }
}
