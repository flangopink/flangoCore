using RimWorld;
using Verse;
using System.Collections.Generic;
using UnityEngine;

namespace flangoCore
{
    public class HediffCombo
    {
        public HediffDef reactWith;
        public HediffDef result;

        public ThingDef spawnThing;
        public float spawnThingRadius = 1.9f;
        public int spawnThingCount = 1;

        public bool isAreaOfEffect;
        public bool useCenterCellForAOE = true;
        public float resultRadius = 2.9f;

        public bool dontRemoveOther;
        public bool dontRemoveSelf;

        public FleckDef reactFleck;

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
                    pawn.hediffSet.hediffs.Remove(pawn.hediffSet.GetFirstHediffOfDef(h));
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
                            {
                                foreach (Pawn p in cell.GetThingList(parent.pawn.Map))
                                {
                                    ApplyCombo(p.health, combo);
                                    MakeFlecks(combo);
                                    DealDamage(combo);
                                }
                            }
                        }
                        else
                        {
                            ApplyCombo(pawn, combo);
                            MakeFlecks(combo);
                            DealDamage(combo);
                        }
                    }
                }
            }
        }

        public void ApplyCombo(Pawn_HealthTracker pawnHealth, HediffCombo combo)
        {
            if (pawnHealth == null) return;
            if (combo.result != null) pawnHealth.AddHediff(combo.result);
            if (!combo.dontRemoveOther) pawnHealth.RemoveHediff(pawnHealth.hediffSet.hediffs.Find(x => x.def == combo.reactWith));
            if (!combo.dontRemoveSelf) pawnHealth.RemoveHediff(parent);
            if (combo.throwText) MoteMaker.ThrowText(parent.pawn.Position.ToVector3() + combo.textOffset, parent.pawn.Map, combo.text, combo.textColor, combo.textDuration);
            if (combo.spawnThing != null)
            {
                if (combo.spawnThingRadius > 0)
                {
                    var cells = GenRadial.RadialCellsAround(parent.pawn.Position, combo.spawnThingRadius, false);
                    for (int i = 0; i < combo.spawnThingCount; i++)
                    {
                        GenSpawn.Spawn(combo.spawnThing, cells.RandomElement(), parent.pawn.Map);
                    }
                }
                else GenSpawn.Spawn(combo.spawnThing, parent.pawn.Position.RandomAdjacentCell8Way(), parent.pawn.Map);
            }
        }

        public void MakeFlecks(HediffCombo combo)
        {
            if (combo.reactFleck != null && parent.pawn.Map != null)
            {
                Map map = parent.pawn.Map;
                FleckCreationData dataStatic = FleckMaker.GetDataStatic(parent.pawn.DrawPos, map, combo.reactFleck);
                map.flecks.CreateFleck(dataStatic);
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
