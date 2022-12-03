using RimWorld;
using Verse;
using UnityEngine;


namespace flangoCore
{
    public class Verb_ShootShotgun : Verb_Shoot
    {
        protected override bool TryCastShot()
		{
            bool num = TryCastShotBase();

            if (num && CasterIsPawn)
			{
				CasterPawn.records.Increment(RecordDefOf.ShotsFired);
			}
            return num;
        }

		bool TryCastShotBase()
        {
            if (currentTarget.HasThing && currentTarget.Thing.Map != caster.Map)
            {
                return false;
            }

            ThingDef projectile = Projectile;
            if (projectile == null)
            {
                return false;
            }

            bool flag = TryFindShootLineFromTo(caster.Position, currentTarget, out ShootLine resultingLine);
            if (verbProps.stopBurstWithoutLos && !flag)
            {
                return false;
            }

            if (EquipmentSource != null)
            {
                EquipmentSource.GetComp<CompChangeableProjectile>()?.Notify_ProjectileLaunched();
                EquipmentSource.GetComp<CompReloadable>()?.UsedOnce();
            }

            lastShotTick = Find.TickManager.TicksGame;
            Thing thing = caster;
            Thing equipment = EquipmentSource;
            CompMannable compMannable = caster.TryGetComp<CompMannable>();
            if (compMannable != null && compMannable.ManningPawn != null)
            {
                thing = compMannable.ManningPawn;
                equipment = caster;
            }

            Vector3 drawPos = caster.DrawPos;

            var modext = equipment.def.GetModExtension<ModExt_ShotgunSpread>();
            for (int i = 0; i < modext.pelletCount; i++)
            {
                Projectile projectile2 = (Projectile)GenSpawn.Spawn(projectile, resultingLine.Source, caster.Map);

                if (verbProps.ForcedMissRadius > 0.5f)
                {
                    float num = verbProps.ForcedMissRadius;
                    Pawn caster;
                    if (thing != null && (caster = (thing as Pawn)) != null)
                    {
                        num *= verbProps.GetForceMissFactorFor(equipment, caster);
                    }

                    float num2 = VerbUtility.CalculateAdjustedForcedMiss(num, currentTarget.Cell - base.caster.Position);
                    if (num2 > 0.5f)
                    {
                        int max = GenRadial.NumCellsInRadius(num2);
                        int num3 = Rand.Range(0, max);
                        if (num3 > 0)
                        {
                            IntVec3 c = currentTarget.Cell + GenRadial.RadialPattern[num3];
                            ProjectileHitFlags projectileHitFlags = ProjectileHitFlags.NonTargetWorld;
                            if (Rand.Chance(0.5f))
                            {
                                projectileHitFlags = ProjectileHitFlags.All;
                            }

                            if (!canHitNonTargetPawnsNow)
                            {
                                projectileHitFlags &= ~ProjectileHitFlags.NonTargetPawns;
                            }

                            projectile2.Launch(thing, drawPos, c, currentTarget, projectileHitFlags, preventFriendlyFire, equipment);
                            continue;
                        }
                    }
                }

                ShotReport shotReport = ShotReport.HitReportFor(caster, this, currentTarget);
                Thing randomCoverToMissInto = shotReport.GetRandomCoverToMissInto();
                ThingDef targetCoverDef = randomCoverToMissInto?.def;
                if (!Rand.Chance(shotReport.AimOnTargetChance_IgnoringPosture))
                {
                    resultingLine.ChangeDestToMissWild(shotReport.AimOnTargetChance_StandardTarget);
                    ProjectileHitFlags projectileHitFlags2 = ProjectileHitFlags.NonTargetWorld;
                    if (Rand.Chance(0.5f) && canHitNonTargetPawnsNow)
                    {
                        projectileHitFlags2 |= ProjectileHitFlags.NonTargetPawns;
                    }

                    projectile2.Launch(thing, drawPos, resultingLine.Dest, currentTarget, projectileHitFlags2, preventFriendlyFire, equipment, targetCoverDef);
                    continue;
                }

                if (currentTarget.Thing != null && currentTarget.Thing.def.category == ThingCategory.Pawn && !Rand.Chance(shotReport.PassCoverChance))
                {
                    ProjectileHitFlags projectileHitFlags3 = ProjectileHitFlags.NonTargetWorld;
                    if (canHitNonTargetPawnsNow)
                    {
                        projectileHitFlags3 |= ProjectileHitFlags.NonTargetPawns;
                    }

                    projectile2.Launch(thing, drawPos, randomCoverToMissInto, currentTarget, projectileHitFlags3, preventFriendlyFire, equipment, targetCoverDef);
                    continue;
                }

                ProjectileHitFlags projectileHitFlags4 = ProjectileHitFlags.IntendedTarget;
                if (canHitNonTargetPawnsNow)
                {
                    projectileHitFlags4 |= ProjectileHitFlags.NonTargetPawns;
                }

                if (!currentTarget.HasThing || currentTarget.Thing.def.Fillage == FillCategory.Full)
                {
                    projectileHitFlags4 |= ProjectileHitFlags.NonTargetWorld;
                }

                if (currentTarget.Thing != null)
                {
                    projectile2.Launch(thing, drawPos, currentTarget, currentTarget, projectileHitFlags4, preventFriendlyFire, equipment, targetCoverDef);
                }
                else
                {
                    projectile2.Launch(thing, drawPos, resultingLine.Dest, currentTarget, projectileHitFlags4, preventFriendlyFire, equipment, targetCoverDef);
                }
            }

            return true;
        }
	}

}
