using HarmonyLib;
using RimWorld;
using Verse;

namespace flangoCore
{
    //[HarmonyPatch(typeof(PawnComponentsUtility), "AddAndRemoveDynamicComponents")]
    public static class Patch_PawnComponentsUtility_AddAndRemoveDynamicComponents
    {
        //[HarmonyPostfix]
        public static void AddDraftability(Pawn pawn)
        {
            if ((pawn.Faction?.IsPlayer ?? false) && pawn.IsDraftable())
            {
                pawn.drafter ??= new Pawn_DraftController(pawn);
                pawn.equipment ??= new Pawn_EquipmentTracker(pawn);
                pawn.relations ??= new Pawn_RelationsTracker(pawn);
            }
        }
    }
}
