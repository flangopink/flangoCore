using RimWorld;
using Verse;
using HarmonyLib;
using System.Linq;

namespace flangoCore
{
    [HarmonyPatch(typeof(Pawn_EquipmentTracker), "Notify_EquipmentAdded")]
    public static class Patch_Pawn_EquipmentTracker_Notify_EquipmentAdded
    {
        [HarmonyPostfix]
        public static void Notify_EquipmentAddedPostfix(Pawn_EquipmentTracker __instance, ThingWithComps eq)
        {
            if (eq == null || __instance == null)
            {
                return;
            }

            ModExt_AbilityGiver modext = eq.def.GetModExtension<ModExt_AbilityGiver>();
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
                        if (def.abilityClass != typeof(EquipmentAbility)) 
                            def.abilityClass = typeof(EquipmentAbility);

                        if (!__instance.pawn.abilities.abilities.Any(x => x.def == def))
                        {
                            __instance.pawn.abilities.TryGainEquipmentAbility(def, eq);
                        }
                        else
                        {
                            ((EquipmentAbility)__instance.pawn.abilities.abilities.First(x => x.def == def && x is EquipmentAbility)).sources.Add(eq);
                        }
                    }
                }
            }
        }
    }

 /*   [HarmonyPatch(typeof(Pawn_EquipmentTracker), "Notify_EquipmentRemoved")]
    public static class Patch_Pawn_EquipmentTracker_Notify_EquipmentRemoved
    {
        [HarmonyPostfix]
        public static void Notify_EquipmentRemovedPostfix(Pawn_EquipmentTracker __instance, ThingWithComps eq)
        {
            if (eq == null || __instance == null)
            {
                return;
            }

            ModExt_AbilityGiver modext = eq.def.GetModExtension<ModExt_AbilityGiver>();
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
                        // Don't remove ability if there are also apparel with the same ability
                        if (!pawn.apparel.WornApparel.Any(x => x.def.HasModExtension<ModExt_AbilityGiver>() && x.def.GetModExtension<ModExt_AbilityGiver>().abilities.Contains(def))
                            && !pawn.health.hediffSet.hediffs.Any(x => x.TryGetCompFast<HediffComp_Ability>() != null && x.TryGetCompFast<HediffComp_Ability>().Props.abilities.Contains(def)))
                        {
                            pawn.abilities.TryRemoveEquipmentAbility(def, eq);
                        }
                    }
                }
            }
        }
    }*/
}
