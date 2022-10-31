using RimWorld;
using Verse;
using System.Collections.Generic;

namespace flangoCore
{
    public class Projectile_RandomEffectBullet : Bullet
    {
        public ModExt_ProjectileRandomEffect ModExt => def.GetModExtension<ModExt_ProjectileRandomEffect>();

        private float Roll => Rand.Value;
        private float sharedChance;

        public override float ArmorPenetration
        {
            get
            {
                var APs = ModExt.APs;
                if (APs == null) return base.ArmorPenetration;

                if (!APs.NullOrEmpty())
                {
                    var roll = ModExt.sharedChance ? sharedChance : Roll;
                    foreach (APWithChance h in APs)
                    {
                        if (roll < h.chance) return h.value;
                        else roll -= h.chance;
                    }
                    if (ModExt.defaultToZeroAP) return 0;
                }
                return base.ArmorPenetration;
            }
        }

        protected override void Impact(Thing hitThing, bool blockedByShield = false)
        {
            base.Impact(hitThing);
            if (ModExt == null)
            {
                Log.Error($"{def.defName}'s thingClass is {GetType().Name}, but doesn't have ModExt_ProjectileRandomEffect modExtension.");
            }

            // null checks
            if (hitThing != null && hitThing is Pawn hitPawn)
            {
                var hediffs = ModExt.hediffs;
                if (hediffs == null) return;

                Pawn targetPawn;
                sharedChance = Roll;
                float roll;

                if (!hediffs.NullOrEmpty())
                {
                    roll = ModExt.sharedChance ? sharedChance : Roll;
                    foreach (HediffWithChance h in hediffs)
                    {
                        targetPawn = h.addToSelf ? (Pawn)launcher : hitPawn;

                        if (roll < h.chance)
                        {
                            foreach (HediffWithChance heds in hediffs)
                            {
                                if (targetPawn.health.hediffSet.HasHediff(heds.hediff))
                                {
                                    Hediff hed = targetPawn.health.hediffSet.GetFirstHediffOfDef(heds.hediff);
                                    targetPawn.health?.RemoveHediff(hed);
                                }
                            }
                            Hediff hediff = HediffMaker.MakeHediff(h.hediff, targetPawn);
                            hediff.Severity = h.severity;
                            targetPawn.health.AddHediff(hediff);
                            break;
                        }
                        else roll -= h.chance;
                    }
                }

                var extras = ModExt.extras;
                if (extras == null) return;

                if (!extras.NullOrEmpty())
                {
                    roll = ModExt.sharedChance ? sharedChance : Roll;
                    foreach (ExtraWithChance h in extras)
                    {
                        if (roll < h.chance)
                        {
                            if (h.extraDamage != null)
                            {
                                DamageInfo dinfo2 = new DamageInfo(h.extraDamage.def, h.extraDamage.amount, h.extraDamage.AdjustedArmorPenetration(), ExactRotation.eulerAngles.y, launcher, null, equipmentDef, DamageInfo.SourceCategory.ThingOrUnknown, intendedTarget.Thing);
                                hitThing.TakeDamage(dinfo2).AssociateWithLog(new BattleLogEntry_RangedImpact(launcher, hitThing, intendedTarget.Thing, equipmentDef, def, targetCoverDef));
                            }

                            if (!h.setOnFire) break;
                            else
                            {
                                bool canSetOnFire = true;

                                if (!h.ignoreShields && hitPawn.apparel != null)
                                {
                                    List<Apparel> wornApparel = hitPawn.apparel.WornApparel;
                                    if (wornApparel != null)
                                    {
                                        for (int i = 0; i < wornApparel.Count; i++)
                                        {
                                            var shield = wornApparel[i].TryGetComp<CompShield>();
                                            if (shield != null && hitPawn.Drafted)
                                            {
                                                if (shield.Energy > 0)
                                                    canSetOnFire = false;
                                                break;
                                            }
                                            else if (!h.affectedShields.NullOrEmpty() && h.affectedShields.Contains(wornApparel[i].def) && hitPawn.Drafted)
                                            {
                                                canSetOnFire = false;
                                                break;
                                            }
                                        }
                                    }
                                }

                                if (canSetOnFire) FireUtility.TryAttachFire(hitPawn, h.fireSize);
                            }
                            break;
                        }
                        else roll -= h.chance;
                    }
                }
            }
        }
    }
}
