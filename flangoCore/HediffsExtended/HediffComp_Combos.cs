using RimWorld;
using Verse;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace flangoCore
{
    public class HediffCombo
    {
        public HediffDef reactWith;
        public HediffDef result;

        public ThingDef spawnThing;
        public ThingDef spawnThingStuff;
        public float spawnThingRadius = 1.9f;
        public int spawnThingCount = 1;
        public int spawnThingStackCount = 1;

        public bool isAreaOfEffect;
        public bool useCenterCellForAOE = true;
        public float resultRadius = 2.9f;

        public bool removeOther = true;
        public bool removeSelf = true;

        public FleckProps reactFleck;

        public DamageDef damageDef;
        public float damageAmount = 0f;
        public float armorPenetration = 0f;

        public bool throwText = true;
        public string text = "Missing Text";
        public Color textColor = Color.white;
        public float textDuration = 3f;
        public Vector3 textOffset = Vector3.zero;
    }

    public class HediffCompProperties_Combos : HediffCompProperties
    {
        public List<HediffCombo> combos;
        public List<HediffDef> incompatibleWith;
        public List<HediffDef> removesHediffs;

        public HediffCompProperties_Combos()
        {
            compClass = typeof(HediffComp_Combos);
        }
    }

    public class HediffComp_Combos : HediffComp
    {
        public HediffCompProperties_Combos Props => (HediffCompProperties_Combos)props;

        public override void CompPostPostAdd(DamageInfo? dinfo)
        {
            if (parent.pawn.Dead) parent.pawn.health.RemoveHediff(parent);
            base.CompPostPostAdd(dinfo);

            Pawn_HealthTracker pawn = parent.pawn.health;

            // Remove self if incompatible with other
            if (!Props.incompatibleWith.NullOrEmpty())
            {
                foreach (HediffDef h in Props.incompatibleWith)
                {
                    if (pawn.hediffSet.GetFirstHediffOfDef(h) != null)
                    {
                        pawn.hediffSet.hediffs.Remove(parent);
                        return;
                    }
                }
            }

            // Remove other hediffs
            if (!Props.removesHediffs.NullOrEmpty())
            {
                foreach (HediffDef h in Props.removesHediffs)
                {
                    var hed = pawn.hediffSet.GetFirstHediffOfDef(h);
                    if (hed != null) pawn.hediffSet.hediffs.Remove(hed);
                }
            }

            if (!Props.combos.NullOrEmpty())
            {
                foreach (HediffCombo combo in Props.combos)
                {
                    // Combine
                    if (pawn.hediffSet.HasHediff(combo.reactWith))
                    {
                        if (combo.isAreaOfEffect)
                        {
                            var cells = GenRadial.RadialCellsAround(parent.pawn.Position, combo.resultRadius, combo.useCenterCellForAOE);
                            foreach (IntVec3 cell in cells)
                                foreach (Pawn p in cell.GetThingList(parent.pawn.Map).Cast<Pawn>())
                                    ApplyCombo(p.health, combo);
                        }
                        else ApplyCombo(pawn, combo);
                    }
                }
            }
        }

        public void ApplyCombo(Pawn_HealthTracker pawnHealth, HediffCombo combo)
        {
            if (pawnHealth == null) return;
            if (combo.result != null) pawnHealth.AddHediff(combo.result);
            if (combo.removeOther) pawnHealth.RemoveHediff(pawnHealth.hediffSet.hediffs.Find(x => x.def == combo.reactWith));
            if (combo.removeSelf) pawnHealth.RemoveHediff(parent);
            if (combo.throwText) MoteMaker.ThrowText(parent.pawn.Position.ToVector3() + combo.textOffset, parent.pawn.Map, combo.text, combo.textColor, combo.textDuration);
            if (parent.pawn.Map != null) combo.reactFleck?.MakeFleck(parent.pawn.Map, parent.pawn.DrawPos);
            DealDamage(combo);
            if (combo.spawnThing != null)
            {
                Thing t = ThingMaker.MakeThing(combo.spawnThing, combo.spawnThingStuff);
                t.stackCount = combo.spawnThingStackCount;
                if (combo.spawnThingRadius > 0)
                {
                    var cells = GenRadial.RadialCellsAround(parent.pawn.Position, combo.spawnThingRadius, false);
                    for (int i = 0; i < combo.spawnThingCount; i++)
                    {
                        GenSpawn.Spawn(t, cells.RandomElement(), parent.pawn.Map);
                    }
                }
                else GenSpawn.Spawn(t, parent.pawn.Position.RandomAdjacentCell8Way(), parent.pawn.Map);
            }
        }

        public void DealDamage(HediffCombo combo)
        {
            if (combo.damageDef != null && combo.damageAmount != 0)
            {
                parent.pawn.TakeDamage(new DamageInfo(combo.damageDef, combo.damageAmount, combo.armorPenetration));
            }
        }
    }
}
