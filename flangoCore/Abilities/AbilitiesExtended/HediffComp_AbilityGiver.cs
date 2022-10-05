using RimWorld;
using System.Collections.Generic;
using Verse;
using System.Linq;

namespace flangoCore
{
    public class HediffCompProperties_AbilityGiver : HediffCompProperties
    {
        public List<AbilityDef> abilities = new List<AbilityDef>();
        public HediffCompProperties_AbilityGiver() => compClass = typeof(HediffComp_AbilityGiver);
    }

    public class HediffComp_AbilityGiver : HediffComp
    {
        public HediffCompProperties_AbilityGiver Props => (HediffCompProperties_AbilityGiver)props;

        public override void CompPostMake()
        {
            base.CompPostMake();
            if (!Props.abilities.NullOrEmpty())
            {
                if (Pawn.abilities == null) Pawn.abilities = new Pawn_AbilityTracker(Pawn);

                foreach (AbilityDef ab in Props.abilities)
                {
                    if (ab.abilityClass != typeof(EquipmentAbility))
                        ab.abilityClass = typeof(EquipmentAbility);

                    if (!Pawn.abilities.abilities.Any(x => x.def == ab))
                    {
                        Pawn.abilities.TryGainHediffAbility(ab, parent);
                    }
                    else
                    {
                        ((EquipmentAbility)Pawn.abilities.abilities.First(x => x.def == ab && x is EquipmentAbility)).sources.Add(parent);
                    }
                }
            }
        }

        public override void CompPostPostAdd(DamageInfo? dinfo)
        {
            base.CompPostPostAdd(dinfo);
            if (!Props.abilities.NullOrEmpty())
            {
                if (Pawn.abilities == null) Pawn.abilities = new Pawn_AbilityTracker(Pawn);

                foreach (AbilityDef ab in Props.abilities)
                {
                    if (!Pawn.abilities.abilities.Any(x => x.def == ab))
                    {
                        Pawn.abilities.TryGainHediffAbility(ab, parent);
                    }
                    else
                    {
                        ((EquipmentAbility)Pawn.abilities.abilities.First(x => x.def == ab && x is EquipmentAbility)).sources.Add(parent);
                    }
                }
            }
        }

        public override void CompPostPostRemoved()
        {
            base.CompPostPostRemoved();
            Pawn.abilities = null;
        }
    }
}
