using RimWorld;
using System;
using System.Linq;
using Verse;

namespace flangoCore
{
    public static class AbilityTracker_Extentions
    {
        public static void TryGainEquipmentAbility(this Pawn_AbilityTracker tracker, AbilityDef def, ThingWithComps thing)
        {
            if (!(tracker.abilities.FirstOrFallback(x => x.def == def && x is EquipmentAbility y && y.sources.Contains(thing)) is EquipmentAbility ab))
            {
                ab = Activator.CreateInstance(def.abilityClass, new object[]
                {
                tracker.pawn,
                def,
                }) as EquipmentAbility;
                ab.sources.Add(thing);
                tracker.abilities.Add(ab);
                tracker.Notify_TemporaryAbilitiesChanged();
            }
        }
        /*
        public static void TryRemoveEquipmentAbility(this Pawn_AbilityTracker tracker, AbilityDef def, ThingWithComps thing)
        {
            if (tracker.abilities.FirstOrFallback(x => x.def == def && x is EquipmentAbility y && y.sources.Contains(thing)) is EquipmentAbility ab)
            {
                tracker.abilities.Remove(ab);
                tracker.Notify_TemporaryAbilitiesChanged();
            }
        }
        */
        public static void TryGainHediffAbility(this Pawn_AbilityTracker tracker, AbilityDef def, HediffWithComps hediff)
        {
            if (!(tracker.abilities.FirstOrFallback(x => x.def == def && x is EquipmentAbility y && y.sources.Contains(hediff)) is EquipmentAbility ab))
            {
                ab = Activator.CreateInstance(def.abilityClass, new object[]
                {
                tracker.pawn,
                def,
                }) as EquipmentAbility;
                ab.sources.Add(hediff);
                tracker.abilities.Add(ab);
                tracker.Notify_TemporaryAbilitiesChanged();
            }
        }
        /*
        public static void TryRemoveHediffAbilityByDef(this Pawn_AbilityTracker tracker, AbilityDef def, HediffWithComps hediff)
        {
            if (tracker.abilities.FirstOrFallback(x => x.def == def && x is EquipmentAbility y && y.sources.Contains(hediff)) is EquipmentAbility ab)
            {
                tracker.abilities.Remove(ab);
                tracker.Notify_TemporaryAbilitiesChanged();
            }
        }

        public static void TryRemoveHediffAbility(this Pawn_AbilityTracker tracker, Ability ability, HediffWithComps hediff)
        {
            if (tracker.abilities.FirstOrFallback(x => x == ability && x is EquipmentAbility y && y.sources.Contains(hediff)) is EquipmentAbility ab)
            {
                tracker.abilities.Remove(ab);
                tracker.Notify_TemporaryAbilitiesChanged();
            }
        }

        public static void GainAbility(this Pawn_AbilityTracker tracker, AbilityDef def, Thing source)
        {
            if (!tracker.abilities.Any((Ability a) => a.def == def))
            {
                tracker.abilities.Add(Activator.CreateInstance(def.abilityClass, new object[]
                {
                    tracker.pawn,
                    def,
                    source
                }) as Ability);
            }
        }
        */
    }
}
