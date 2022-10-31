using RimWorld;
using Verse;
using HarmonyLib;

namespace flangoCore
{
    public class ModExt_FlickableTexture : DefModExtension {}


    [HarmonyPatch(typeof(CompFlickable), "DoFlick")]
    public static class Patch_CompFlickable_DoFlick
    {
        [HarmonyPostfix]
        public static void FlickTexture_Postfix(CompFlickable __instance)
        {
            if (__instance.parent.def.HasModExtension<ModExt_FlickableTexture>())
            {
                __instance.parent.graphicInt = __instance.CurrentGraphic;
            }
        }
    }
}
