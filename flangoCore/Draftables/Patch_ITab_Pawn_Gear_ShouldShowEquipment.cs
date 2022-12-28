using HarmonyLib;
using RimWorld;
using Verse;

namespace flangoCore
{
    [HarmonyPatch(typeof(ITab_Pawn_Gear), "ShouldShowEquipment")]
    public static class Patch_ITab_Pawn_Gear_ShouldShowEquipment
    {
        [HarmonyPostfix]
        public static void RemoveTab(Pawn p, ref bool __result)
        {
            if (p.IsDraftable() && !p.RaceProps.IsMechanoid)
            {
                __result = false;
            }
        }
    }
}
