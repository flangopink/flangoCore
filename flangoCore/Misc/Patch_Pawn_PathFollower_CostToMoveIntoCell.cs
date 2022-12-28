using System.Collections.Generic;
using HarmonyLib;
using RimWorld;
using Verse;
using Verse.AI;

namespace flangoCore
{
    public class ModExt_IgnoreTerrainMovementPenalty : DefModExtension {}


    [HarmonyPatch(typeof(Pawn_PathFollower), "CostToMoveIntoCell")]
    public static class Patch_Pawn_PathFollower_CostToMoveIntoCell
    {
        [HarmonyPostfix]
        public static void Postfix(Pawn pawn, IntVec3 c, ref int __result)
        {
            if (pawn.Map == null || !pawn.def.HasModExtension<ModExt_IgnoreTerrainMovementPenalty>()) return;

            int num = (c.x != pawn.Position.x && c.z != pawn.Position.z) ? pawn.TicksPerMoveDiagonal : pawn.TicksPerMoveCardinal;

            TerrainDef terrainDef = pawn.Map.terrainGrid.TerrainAt(c);

            if (terrainDef == null || terrainDef.passability == Traversability.Impassable && !terrainDef.IsWater)
            {
                num = 10000;
            }
            List<Thing> list = pawn.Map.thingGrid.ThingsListAt(c);
            for (int i = 0; i < list.Count; i++)
            {
                Thing thing = list[i];
                if (thing.def.passability == Traversability.Impassable)
                {
                    num = 10000;
                }
            }
            __result = num;
        }
    }
}
