using HarmonyLib;
using RimWorld;
using Verse;

namespace flangoCore
{
    [HarmonyPatch(typeof(Pawn_ApparelTracker), "Notify_ApparelRemoved")]
    public static class Patch_Pawn_ApparelTracker_Notify_ApparelRemoved
    {
        [HarmonyPostfix]
        public static void Notify_ApparelRemovedPostfix(Pawn_ApparelTracker __instance, Apparel apparel)
        {
            for (int i = 0; i < __instance.WornApparelCount; i++)
            {
                Apparel worn = __instance.WornApparel[i];
                if (worn.def.GetModExtension<ModExt_Requirements>() is ModExt_Requirements ext)
                {
                    if (ext.apparelDefs.Contains(apparel.def))
                    {
                        Log.Message($"Tried to drop {worn} as it requires {apparel}");
                        __instance.TryDrop(worn, out _, __instance.pawn.Position.RandomAdjacentCell8Way());
                    }
                }
            }

            var primary = __instance.pawn.equipment?.Primary;
            if (primary != null && primary.def.GetModExtension<ModExt_Requirements>() is ModExt_Requirements ext2)
            {
                if (ext2.apparelDefs.Contains(apparel.def))
                {
                    Log.Message($"Tried to drop {primary} as it requires {apparel}");
                    __instance.pawn.equipment.TryDropEquipment(primary, out _, __instance.pawn.Position.RandomAdjacentCell8Way());
                }
            }
        }
    }
}
