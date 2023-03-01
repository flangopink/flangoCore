using HarmonyLib;
using Verse;

namespace flangoCore
{
    //[HarmonyPatch(typeof(Pawn_EquipmentTracker), "DestroyEquipment")]
    public static class Patch_EquipmentTracker_DestroyEquipment
    {
        //[HarmonyPrefix]
        public static bool Prefix(Pawn_EquipmentTracker __instance)
        {
            return !__instance.pawn.IsDraftable();
        }
    }
}
