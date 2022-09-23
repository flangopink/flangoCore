using RimWorld;
using Verse;
using System.Collections.Generic;

namespace flangoCore
{
    public class ThingDef_RandomHediffBullet : ThingDef
    {
        public List<HediffWithChance> hediffs;
    }

    public class Projectile_RandomHediffBullet : Bullet
    {
        public ThingDef_RandomHediffBullet Def => (ThingDef_RandomHediffBullet)def;

        protected override void Impact(Thing hitThing)
        {
            base.Impact(hitThing);

            // null checks
            if (Def != null && hitThing != null && hitThing is Pawn hitPawn)
            {
                Pawn targetPawn;

                float roll = Rand.Value;
                foreach (HediffWithChance h in Def.hediffs)
                {
                    targetPawn = h.addToSelf ? (Pawn)launcher : hitPawn;

                    if (roll < h.addHediffChance)
                    {
                        foreach (HediffWithChance heds in Def.hediffs)
                        {
                            if (targetPawn.health.hediffSet.HasHediff(heds.hediffToAdd))
                            {
                                Hediff hed = targetPawn.health.hediffSet.GetFirstHediffOfDef(heds.hediffToAdd);
                                targetPawn.health?.RemoveHediff(hed); 
                            }
                        }
                        Hediff hediff = HediffMaker.MakeHediff(h.hediffToAdd, targetPawn);
                        hediff.Severity = h.addHediffSeverity;
                        targetPawn.health.AddHediff(hediff);
                        break;
                    }
                    else
                    {
                        roll -= h.addHediffChance;
                    }
                }
            }
        }
    }
}
