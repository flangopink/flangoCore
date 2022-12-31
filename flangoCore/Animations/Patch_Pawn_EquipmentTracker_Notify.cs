using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using System.Collections.Generic;
using Verse;

namespace flangoCore
{
    [HarmonyPatch(typeof(Pawn_EquipmentTracker), "Notify_EquipmentAdded")]
    public static class Patch_Pawn_EquipmentTracker_Notify_EquipmentAdded_Anim
    {
        [HarmonyPostfix]
        public static void Postfix(ThingWithComps eq)
        {
            if (eq?.GetComp<CompAnimatedWeapon>() is CompAnimatedWeapon comp)
            {
                AnimCache.animCache.Add(eq, comp);
                AnimCache.animCache[eq].Init();
            }
        }
    }

    [HarmonyPatch(typeof(Pawn_EquipmentTracker), "Notify_EquipmentRemoved")]
    public static class Patch_Pawn_EquipmentTracker_Notify_EquipmentRemoved_Anim
    {
        [HarmonyPostfix]
        public static void Postfix(ThingWithComps eq)
        {
            if (eq?.GetComp<CompAnimatedWeapon>() != null)
                AnimCache.animCache.Remove(eq);
        }
    }

    [HarmonyPatch(typeof(Pawn_EquipmentTracker), "EquipmentTrackerTick")]
    public static class Patch_Pawn_EquipmentTracker_EquipmentTrackerTick
    {
        [HarmonyPostfix]
        public static void Postfix(Pawn_EquipmentTracker __instance)
        {
            var pr = __instance.Primary;
            if (pr == null) return;
            if (!AnimCache.animCache.ContainsKey(pr)) // Loads comps if already equipped
            {
                if (__instance.pawn.IsWorldPawn()) return;
                var comp = pr.GetComp<CompAnimatedWeapon>();
                if (comp == null) return;
                AnimCache.animCache[pr] = comp;
                AnimCache.animCache[pr].Init();
            }
            AnimCache.animCache[pr].TickAnim();
        }
    }

    public static class AnimCache
    {
        public static Dictionary<Thing, CompAnimatedWeapon> animCache = new();
    }
}