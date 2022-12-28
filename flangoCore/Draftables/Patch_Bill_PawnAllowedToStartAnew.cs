using HarmonyLib;
using RimWorld;
using Verse;

namespace flangoCore
{
    [HarmonyPatch(typeof(Bill), "PawnAllowedToStartAnew")]
    public static class Patch_Bill_PawnAllowedToStartAnew
    {
        [HarmonyPrefix]
        public static bool AvoidBillErrorIfPawnIsNonHuman(Pawn p)
        {
            return !p.IsDraftable();
        }
    }
}