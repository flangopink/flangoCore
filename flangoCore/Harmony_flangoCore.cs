using Verse;
using HarmonyLib;
using System.Linq;

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

            int num = (from c in System.Reflection.Assembly.GetExecutingAssembly().GetTypes() where c.Namespace == "flangoCore" && c.IsClass select c).ToList().Count();

            Log.Message($"<color=#FFC0CB>Launched successfully! Classes: {num}, Harmony patches: {harmony.GetPatchedMethods().Select(Harmony.GetPatchInfo).SelectMany((Patches p) => p.Prefixes.Concat(p.Postfixes).Concat(p.Transpilers)).Count((Patch p) => p.owner == harmony.Id)}\nThank you for using flangoCore!</color>");
        }

        static void VFEPatches(Harmony harmony)
        {
            var hoppersOrig = typeof(ItemProcessor.Building_ItemProcessor).GetMethod("CheckTheHoppers");
            var hoppersPrefix = typeof(Patch_Building_ItemProcessor_CheckTheHoppers).GetMethod("Prefix");

            harmony.Patch(hoppersOrig, new HarmonyMethod(hoppersPrefix));
        }
    }
}
