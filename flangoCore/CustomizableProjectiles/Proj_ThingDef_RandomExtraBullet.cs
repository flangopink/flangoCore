using RimWorld;
using Verse;
using System.Collections.Generic;

namespace flangoCore
{
    public class ThingDef_RandomExtraBullet : ThingDef
    {
        public List<ExtraWithChance> extras;
    }

    public class Projectile_RandomExtraBullet : Bullet
    {
        public ThingDef_RandomExtraBullet Def => (ThingDef_RandomExtraBullet)def;

        protected override void Impact(Thing hitThing)
        {
            base.Impact(hitThing);

            // null checks
            if (Def != null && hitThing != null && hitThing is Pawn hitPawn)
            {
                float roll = Rand.Value;
                foreach (ExtraWithChance h in Def.extras)
                {
                    if (roll < h.addExtraChance)
                    {
                        if (h.extraDamage != null)
                        {
                            DamageInfo dinfo2 = new DamageInfo(h.extraDamage.def, h.extraDamage.amount, h.extraDamage.AdjustedArmorPenetration(), ExactRotation.eulerAngles.y, launcher, null, equipmentDef, DamageInfo.SourceCategory.ThingOrUnknown, intendedTarget.Thing);
                            hitThing.TakeDamage(dinfo2).AssociateWithLog(new BattleLogEntry_RangedImpact(launcher, hitThing, intendedTarget.Thing, equipmentDef, def, targetCoverDef));
                        }

                        if (h.setOnFire)
                        {
                            bool canSetOnFire = true;

                            if (!h.ignoreShields)
                            {
                                if (hitPawn.apparel != null)
                                {
                                    List<Apparel> wornApparel = hitPawn.apparel.WornApparel;
                                    if (wornApparel != null)
                                    {
                                        for (int i = 0; i < wornApparel.Count; i++)
                                        {
                                            if (wornApparel[i] is ShieldBelt shield && hitPawn.Drafted)
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
                            }
                            if (canSetOnFire) FireUtility.TryAttachFire(hitPawn, h.fireSize);
                        }
                        break;
                    }
                    else
                    {
                        roll -= h.addExtraChance;
                    }
                }
            }
        }
    }
}
