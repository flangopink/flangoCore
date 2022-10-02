using System.Linq;
using RimWorld;
using UnityEngine;
using Verse;

namespace flangoCore
{
    public class Verb_EquipmentLaunchProjectile : Verb_UseEquipmentAbility
    {
        public override bool ValidateTarget(LocalTargetInfo target, bool showMessages = true)
        {
            return base.ValidateTarget(target, showMessages);
        }

        public virtual ThingDef Projectile
        {
            get
            {
                if (EquipmentSource != null && EquipmentSource is ThingWithComps thing)
                {
                    CompChangeableProjectile comp = thing.GetComp<CompChangeableProjectile>();
                    if (comp != null && comp.Loaded)
                    {
                        return comp.Projectile;
                    }
                }
                return verbProps.defaultProjectile;
            }
        }

        protected override bool TryCastShot()
        {
            if (currentTarget.HasThing && currentTarget.Thing.Map != caster.Map)
            {
                return false;
            }

            ThingDef projDef = Projectile;
            if (projDef == null)
            {
                return false;
            }

            bool gotLine = TryFindShootLineFromTo(caster.Position, currentTarget, out ShootLine line);
            if (verbProps.stopBurstWithoutLos && !gotLine)
            {
                return false;
            }

            var eq = EquipmentSource;

            if (eq != null && eq is ThingWithComps twc1)
            {
                twc1.TryGetCompFast<CompChangeableProjectile>()?.Notify_ProjectileLaunched();
                twc1.TryGetCompFast<CompReloadable>()?.UsedOnce();
            }

            Thing attackOwner = caster;
            Thing attackWeapon = eq is ThingWithComps twc2 ? twc2 : caster;

            CompMannable mannable = caster.TryGetCompFast<CompMannable>();
            if (mannable != null && mannable.ManningPawn != null)
            {
                attackOwner = mannable.ManningPawn;
                attackWeapon = caster;
            }
            Vector3 shotOrigin = caster.DrawPos;
            Projectile proj = (Projectile)GenSpawn.Spawn(projDef, line.Source, caster.Map);
            if (verbProps.ForcedMissRadius > 0.5f)
            {
                float adjustedMissRadius = VerbUtility.CalculateAdjustedForcedMiss(verbProps.ForcedMissRadius, currentTarget.Cell - caster.Position);
                if (adjustedMissRadius > 0.5f)
                {
                    int maxCells = GenRadial.NumCellsInRadius(adjustedMissRadius);
                    Rand.PushState();
                    int cellInd = Rand.Range(0, maxCells);
                    Rand.PopState();
                    if (cellInd > 0)
                    {
                        IntVec3 dest = currentTarget.Cell + GenRadial.RadialPattern[cellInd];
                        ThrowDebugText("ToRadius");
                        ThrowDebugText("Rad\nDest", dest);
                        ProjectileHitFlags hitFlags3 = ProjectileHitFlags.NonTargetWorld;
                        Rand.PushState();
                        if (Rand.Chance(0.5f))
                        {
                            hitFlags3 = ProjectileHitFlags.All;
                        }
                        Rand.PopState();
                        if (!canHitNonTargetPawnsNow)
                        {
                            hitFlags3 &= ~ProjectileHitFlags.NonTargetPawns;
                        }
                        proj.Launch(attackOwner, shotOrigin, dest, currentTarget, hitFlags3, preventFriendlyFire, attackWeapon);
                        return true;
                    }
                }
            }
            ShotReport hitReport = ShotReport.HitReportFor(caster, this, currentTarget);
            Thing hitCover = hitReport.GetRandomCoverToMissInto();
            ThingDef hitCoverDef = hitCover?.def;
            if (!Rand.Chance(hitReport.AimOnTargetChance_IgnoringPosture))
            {
                line.ChangeDestToMissWild(hitReport.AimOnTargetChance_StandardTarget);
                ThrowDebugText("ToWild" + (canHitNonTargetPawnsNow ? "\nchntp" : ""));
                ThrowDebugText("Wild\nDest", line.Dest);
                ProjectileHitFlags hitFlags2 = ProjectileHitFlags.NonTargetWorld;
                Rand.PushState();
                if (Rand.Chance(0.5f) && canHitNonTargetPawnsNow)
                {
                    hitFlags2 |= ProjectileHitFlags.NonTargetPawns;
                }
                Rand.PopState();
                proj.Launch(attackOwner, shotOrigin, line.Dest, currentTarget, hitFlags2, preventFriendlyFire, attackWeapon, hitCoverDef);
                return true;
            }
            Rand.PushState();
            bool hit = !Rand.Chance(hitReport.PassCoverChance);
            Rand.PopState();
            if (currentTarget.Thing != null && currentTarget.Thing.def.category == ThingCategory.Pawn && hit)
            {
                ThrowDebugText("ToCover" + (canHitNonTargetPawnsNow ? "\nchntp" : ""));
                ThrowDebugText("Cover\nDest", hitCover.Position);
                ProjectileHitFlags hitFlags = ProjectileHitFlags.NonTargetWorld;
                if (canHitNonTargetPawnsNow)
                {
                    hitFlags |= ProjectileHitFlags.NonTargetPawns;
                }
                proj.Launch(attackOwner, shotOrigin, hitCover, currentTarget, hitFlags, preventFriendlyFire, attackWeapon, hitCoverDef);
                return true;
            }
            ProjectileHitFlags directHitFlags = ProjectileHitFlags.IntendedTarget;
            if (canHitNonTargetPawnsNow)
            {
                directHitFlags |= ProjectileHitFlags.NonTargetPawns;
            }
            if (!currentTarget.HasThing || currentTarget.Thing.def.Fillage == FillCategory.Full)
            {
                directHitFlags |= ProjectileHitFlags.NonTargetWorld;
            }
            ThrowDebugText("ToHit" + (canHitNonTargetPawnsNow ? "\nchntp" : ""));
            if (currentTarget.Thing != null)
            {
                proj.Launch(attackOwner, shotOrigin, currentTarget, currentTarget, directHitFlags, preventFriendlyFire, attackWeapon, hitCoverDef);
                ThrowDebugText("Hit\nDest", currentTarget.Cell);
            }
            else
            {
                proj.Launch(attackOwner, shotOrigin, line.Dest, currentTarget, directHitFlags, preventFriendlyFire, attackWeapon, hitCoverDef);
                ThrowDebugText("Hit\nDest", line.Dest);
            }
            return true;
        }
        public void ThrowDebugText(string text)
        {
            if (DebugViewSettings.drawShooting)
            {
                MoteMaker.ThrowText(caster.DrawPos, caster.Map, text, -1f);
            }
        }

        public void ThrowDebugText(string text, IntVec3 c)
        {
            if (DebugViewSettings.drawShooting)
            {
                MoteMaker.ThrowText(c.ToVector3Shifted(), caster.Map, text, -1f);
            }
        }

        public override void DrawHighlight(LocalTargetInfo target)
        {
            AbilityDef def = ability.def;
            DrawRadius();
            if (CanHitTarget(target) && IsApplicableTo(target, false))
            {
                if (def.HasAreaOfEffect)
                {
                    if (target.IsValid)
                    {
                        GenDraw.DrawTargetHighlight(target);
                        GenDraw.DrawRadiusRing(target.Cell, def.EffectRadius, Verb_CastAbility.RadiusHighlightColor, null);
                    }
                }
                else
                {
                    GenDraw.DrawTargetHighlight(target);
                }
            }
            if (target.IsValid)
            {
                ability.DrawEffectPreviews(target);
            }
            verbProps.DrawRadiusRing(caster.Position);
            if (target.IsValid)
            {
                GenDraw.DrawTargetHighlight(target);
                float num = HighlightFieldRadiusAroundTarget(out bool flag);
                if (num > 0.2f && TryFindShootLineFromTo(caster.Position, target, out ShootLine shootLine))
                {
                    if (flag)
                    {
                        GenExplosion.RenderPredictedAreaOfEffect(shootLine.Dest, num);
                        return;
                    }
                    GenDraw.DrawFieldEdges((from x in GenRadial.RadialCellsAround(shootLine.Dest, num, true)
                                            where x.InBounds(Find.CurrentMap)
                                            select x).ToList());
                }
            }
        }
        public override float HighlightFieldRadiusAroundTarget(out bool needLOSToCenter)
        {
            needLOSToCenter = true;
            ThingDef projectile = Projectile;
            if (projectile == null)
            {
                return 0f;
            }
            return projectile.projectile.explosionRadius;
        }

        public override bool Available()
        {
            if (!base.Available())
            {
                return false;
            }
            if (CasterIsPawn)
            {
                Pawn casterPawn = CasterPawn;
                if (casterPawn.Faction != Faction.OfPlayer && casterPawn.mindState.MeleeThreatStillThreat && casterPawn.mindState.meleeThreat.Position.AdjacentTo8WayOrInside(casterPawn.Position))
                {
                    return false;
                }
            }
            return Projectile != null;
        }
    }
}
