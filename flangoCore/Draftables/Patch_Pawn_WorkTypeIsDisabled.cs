using HarmonyLib;
using RimWorld;
using Verse;

namespace flangoCore
{
    //[HarmonyPatch(typeof(Pawn), "WorkTypeIsDisabled")]
    public static class Patch_Pawn_WorkTypeIsDisabled
    {
        //[HarmonyPostfix]
        public static void RemoveTendFromPawns(WorkTypeDef w, Pawn __instance, ref bool __result)
        {
            if (w == WorkTypeDefOf.Doctor && __instance.IsDraftable() && !__instance.RaceProps.IsMechanoid)
            {
                __result = true;
            }
        }
    }
}
