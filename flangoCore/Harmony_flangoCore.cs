using Verse;
using HarmonyLib;

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


            if (ModLister.HasActiveModWithName("Vanilla Expanded Framework") && Controller.settings.enableVFEPatches)
            {
                Log.Message("<color=#FFC0CB>Vanilla Expanded Framework detected. Patching...</color>");
                VFEPatches(harmony);
            }

            Log.Message("<color=#FFC0CB>Launched successfully!\nThank you for using flangoCore!</color>");
        }

        static void VFEPatches(Harmony harmony)
        {
            var hoppersOrig = typeof(ItemProcessor.Building_ItemProcessor).GetMethod("CheckTheHoppers");
            var hoppersPrefix = typeof(Patch_Building_ItemProcessor_CheckTheHoppers).GetMethod("Prefix");

            harmony.Patch(hoppersOrig, new HarmonyMethod(hoppersPrefix));
        }
    }
}
