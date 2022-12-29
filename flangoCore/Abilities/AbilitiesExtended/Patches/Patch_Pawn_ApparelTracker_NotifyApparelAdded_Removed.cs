using System.Linq;
using RimWorld;
using Verse;
using HarmonyLib;

namespace flangoCore
{
    [HarmonyPatch(typeof(Pawn_ApparelTracker), "Notify_ApparelAdded")]
    public static class Patch_Pawn_ApparelTracker_NotifyApparelAdded
    {
        [HarmonyPostfix]
        public static void Notify_ApparelAddedPostfix(Pawn_ApparelTracker __instance, Apparel apparel)
        {
            if (apparel == null || __instance == null) return;

            ModExt_AbilityGiver modext = apparel.def.GetModExtension<ModExt_AbilityGiver>();
            if (modext != null)
            {
                Pawn pawn = __instance.pawn;
                if (!pawn.RaceProps.Humanlike) return;

                if (!modext.abilities.NullOrEmpty())
                {
                    var ab = __instance.pawn.abilities;
                    foreach (AbilityDef def in modext.abilities)
                    {
                        if (def.abilityClass != typeof(EquipmentAbility))
                            def.abilityClass = typeof(EquipmentAbility);

                        if (!ab.abilities.Any(x => x.def == def))
                        {
                            ab.TryGainEquipmentAbility(def, apparel);
                        }
                        else
                        {
                            ((EquipmentAbility)ab.abilities.First(x => x.def == def && x is EquipmentAbility)).sources.Add(apparel);
                        }
                    }
                }
            }
        }
    }

 /*   [HarmonyPatch(typeof(Pawn_ApparelTracker), "Notify_ApparelRemoved")]
    public static class Patch_Pawn_ApparelTracker_NotifyApparelRemoved
    {
        [HarmonyPostfix]
        public static void Notify_ApparelRemovedPostfix(Pawn_ApparelTracker __instance, Apparel apparel)
        {
            ModExt_AbilityGiver modext = apparel.def.GetModExtension<ModExt_AbilityGiver>();
            if (modext != null)
            {
                Pawn pawn = __instance.pawn;
                if (!pawn.RaceProps.Humanlike)
                {
                    return;
                }
                if (!modext.abilities.NullOrEmpty())
                {
                    foreach (AbilityDef def in modext.abilities)
                    {
                        // Don't remove ability if there are multiple apparel/weapons with the same ability
                        if (!__instance.WornApparel.Any(x => x != apparel && x.def.HasModExtension<ModExt_AbilityGiver>() && x.def.GetModExtension<ModExt_AbilityGiver>().abilities.Contains(def))
                            && pawn.equipment.Primary != null && !pawn.equipment.Primary.def.HasModExtension<ModExt_AbilityGiver>() && pawn.equipment.Primary.def.GetModExtension<ModExt_AbilityGiver>().abilities.Contains(def)
                            && !pawn.health.hediffSet.hediffs.Any(x => x.TryGetCompFast<HediffComp_Ability>() != null && x.TryGetCompFast<HediffComp_Ability>().Props.abilities.Contains(def)))
                        {
                            pawn.abilities.TryRemoveEquipmentAbility(def, apparel);
                        }
                    }
                }
            }
        }
    }*/
}