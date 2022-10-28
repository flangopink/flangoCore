using RimWorld;
using Verse;
using HarmonyLib;

namespace flangoCore
{
    [HarmonyPatch(typeof(CompFlickable), "DoFlick")]
    public static class Patch_CompFlickable_DoFlick
    {
        [HarmonyPostfix]
        public static void FlickTexture_Postfix(CompFlickable __instance)
        {
            Log.Message(__instance.CurrentGraphic.path);
            if (__instance.parent.def.HasModExtension<ModExt_FlickableTexture>())
            {
                Log.Message("boop");
                __instance.parent.graphicInt = __instance.CurrentGraphic;
            }
        }
    }
}
