using RimWorld;
using System.Collections.Generic;
using Verse;
using System.Linq;

namespace flangoCore
{
    public class HediffCompProperties_Ability : HediffCompProperties
    {
        public List<AbilityDef> abilities = new List<AbilityDef>();
        public HediffCompProperties_Ability() => compClass = typeof(HediffComp_Ability);
    }

    public class HediffComp_Ability : HediffComp
    {
        public HediffCompProperties_Ability Props => (HediffCompProperties_Ability)props;

        public override void CompPostMake()
        {
            base.CompPostMake();
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

        public override void CompPostPostAdd(DamageInfo? dinfo)
        {
            base.CompPostPostAdd(dinfo);
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

        /*public override void CompPostPostRemoved()
        {
            base.CompPostPostRemoved();
            foreach (Ability ab in Pawn.abilities.abilities)
            {
                if (Props.abilities.Any(x => x == ab.def))
                {
                    Pawn.abilities.TryRemoveHediffAbility(ab, parent);
                }
            }
        }*/
    }
}
