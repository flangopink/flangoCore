using System;
using System.Collections.Generic;
using RimWorld;
using Verse;
using Verse.Sound;
using UnityEngine;

namespace flangoCore
{
    public class Verb_ShootBeamBurst : Verb
    {
		private List<Vector3> beamHitLocations = new();

		private Effecter endEffecter;

		private Sustainer sustainer;

		private MoteDualAttached mote;

		protected override int ShotsPerBurst => verbProps.burstShotCount;

		public int currentShot;

		private Vector3 currentTargetTruePos;

		public override float? AimAngleOverride
		{
			get
			{
				if (state != VerbState.Bursting)
				{
					return null;
				}
				return (beamHitLocations[currentShot] - caster.DrawPos).AngleFlat();
			}
		}

		protected override bool TryCastShot()
		{
			if (currentTarget.HasThing && currentTarget.Thing.Map != caster.Map)
			{
				return false;
			}
            bool flag = TryFindShootLineFromTo(caster.Position, currentTarget, out ShootLine _);
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

			HitCell(beamHitLocations[currentShot].ToIntVec3());

			if (CasterIsPawn)
			{
				CasterPawn.records.Increment(RecordDefOf.ShotsFired);
			}
			currentShot++;
			return true;
		}

		public override bool TryStartCastOn(LocalTargetInfo castTarg, LocalTargetInfo destTarg, bool surpriseAttack = false, bool canHitNonTargetPawns = true, bool preventFriendlyFire = false, bool nonInterruptingSelfCast = false)
		{
			return base.TryStartCastOn(verbProps.beamTargetsGround ? ((LocalTargetInfo)castTarg.Cell) : castTarg, destTarg, surpriseAttack, canHitNonTargetPawns, preventFriendlyFire, nonInterruptingSelfCast);
		}

		public override void BurstingTick()
		{
			Vector3 vector = beamHitLocations[currentShot];
			Vector3 vector2 = vector - caster.Position.ToVector3Shifted();
			Vector3 normalized = vector2.Yto0().normalized;
			IntVec3 intVec = vector.ToIntVec3();
			IntVec3 intVec2 = GenSight.LastPointOnLineOfSight(caster.Position, intVec, (IntVec3 c) => c.CanBeSeenOverFast(caster.Map), true);
			Vector3 vector3 = vector - intVec.ToVector3Shifted();
			float num = vector2.MagnitudeHorizontal();

			if (intVec2.IsValid)
			{
				num -= (intVec - intVec2).LengthHorizontal;
				vector = caster.Position.ToVector3Shifted() + normalized * num;
				intVec = vector.ToIntVec3();
			}

			Vector3 offsetA = normalized * verbProps.beamStartOffset;

			if (verbProps.beamMoteDef != null)
			{
				mote = MoteMaker.MakeInteractionOverlay(verbProps.beamMoteDef, caster, new TargetInfo(beamHitLocations[currentShot].ToIntVec3(), caster.Map));
			}
			if (mote != null)
			{
				mote.UpdateTargets(new TargetInfo(caster.Position, caster.Map), new TargetInfo(intVec, caster.Map), offsetA, vector3);
				mote.Maintain();
			}

			if (verbProps.beamGroundFleckDef != null && Rand.Chance(verbProps.beamFleckChancePerTick))
			{
				FleckMaker.Static(vector, caster.Map, verbProps.beamGroundFleckDef);
			}
			if (endEffecter == null && verbProps.beamEndEffecterDef != null)
			{
				endEffecter = verbProps.beamEndEffecterDef.Spawn(intVec, caster.Map, vector3);
			}
			if (endEffecter != null)
			{
				endEffecter.offset = vector3;
				endEffecter.EffectTick(new TargetInfo(intVec, caster.Map), TargetInfo.Invalid);
				endEffecter.ticksLeft--;
			}

			if (verbProps.beamLineFleckDef != null)
			{
				float num2 = 1f * num;
				for (int i = 0; i < num2; i++)
				{
					if (Rand.Chance(verbProps.beamLineFleckChanceCurve.Evaluate(i / num2)))
					{
						Vector3 vector4 = i * normalized - normalized * Rand.Value + normalized / 2f;
						FleckMaker.Static(caster.Position.ToVector3Shifted() + vector4, caster.Map, verbProps.beamLineFleckDef);
					}
				}
			}
			sustainer?.Maintain();
		}

		public override void WarmupComplete()
		{
			currentShot = 0;
			beamHitLocations.Clear();
			burstShotsLeft = ShotsPerBurst;
			state = VerbState.Bursting;
			currentTargetTruePos = currentTarget.CenterVector3.Yto0();
			
			SetupStaticTargets();

			TryCastNextBurstShot();
			endEffecter?.Cleanup();

			if (verbProps.soundCastBeam != null)
            {
				sustainer = verbProps.soundCastBeam.TrySpawnSustainer(SoundInfo.InMap(caster, MaintenanceType.PerTick));
			}

			if (currentTarget.Thing is Pawn pawn && !pawn.Downed && CasterIsPawn && CasterPawn.skills != null)
			{
				float snum = (pawn.HostileTo(caster) ? 170f : 20f);
				float snum2 = verbProps.AdjustedFullCycleTime(this, CasterPawn);
				CasterPawn.skills.Learn(SkillDefOf.Shooting, snum * snum2);
			}
		}

		private bool CanHit(Thing thing)
		{
			if (!thing.Spawned)
			{
				return false;
			}
			return !CoverUtility.ThingCovered(thing, caster.Map);
		}

		private void HitCell(IntVec3 cell)
		{
			ApplyDamage(VerbUtility.ThingsToHit(cell, caster.Map, CanHit).RandomElementWithFallback());
		}

		private void ApplyDamage(Thing thing)
		{
			IntVec3 intVec = beamHitLocations[currentShot].ToIntVec3();
			IntVec3 intVec2 = GenSight.LastPointOnLineOfSight(caster.Position, intVec, (IntVec3 c) => c.CanBeSeenOverFast(caster.Map), skipFirstCell: true);
			if (intVec2.IsValid)
			{
				intVec = intVec2;
			}
			Map map = caster.Map;
			if (thing == null || verbProps.beamDamageDef == null)
			{
				return;
			}
			float angleFlat = (currentTarget.Cell - caster.Position).AngleFlat;
			BattleLogEntry_RangedImpact log = new(EquipmentSource, thing, currentTarget.Thing, EquipmentSource.def, null, null);
			DamageInfo dinfo = new(verbProps.beamDamageDef, verbProps.beamDamageDef.defaultDamage, verbProps.beamDamageDef.defaultArmorPenetration, angleFlat, base.EquipmentSource, null, base.EquipmentSource.def, DamageInfo.SourceCategory.ThingOrUnknown, currentTarget.Thing);
			thing.TakeDamage(dinfo).AssociateWithLog(log);
			if (thing.CanEverAttachFire())
			{
				if (Rand.Chance(verbProps.beamChanceToAttachFire))
				{
					thing.TryAttachFire(verbProps.beamFireSizeRange.RandomInRange);
				}
			}
			else if (Rand.Chance(verbProps.beamChanceToStartFire))
			{
				FireUtility.TryStartFireIn(intVec, map, verbProps.beamFireSizeRange.RandomInRange);
			}
		}

		private void SetupStaticTargets()
        {
			for (int i = 0; i <= ShotsPerBurst; i++)
			{
				_ = TryFindShootLineFromTo(caster.Position, currentTarget, out var shootLine);
				ShotReport shotReport = ShotReport.HitReportFor(caster, this, currentTarget);
				Thing randomCoverToMissInto = shotReport.GetRandomCoverToMissInto();
				Vector3 finalStaticShotTarget = currentTargetTruePos;
				if (verbProps.canGoWild && !Rand.Chance(shotReport.AimOnTargetChance_IgnoringPosture))
				{
					shootLine.ChangeDestToMissWild(shotReport.AimOnTargetChance_StandardTarget);
					finalStaticShotTarget = shootLine.Dest.ToVector3Shifted().Yto0();
				}
				if (currentTarget.Thing != null && currentTarget.Thing.def.CanBenefitFromCover && !Rand.Chance(shotReport.PassCoverChance))
				{
					finalStaticShotTarget = randomCoverToMissInto.TrueCenter().Yto0();
				}
				beamHitLocations.Add(finalStaticShotTarget);
			}
        }

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Collections.Look(ref beamHitLocations, "beamHitLocations", LookMode.Value);
			Scribe_Values.Look(ref currentShot, "currentShot", 0);
			if (Scribe.mode == LoadSaveMode.PostLoadInit && beamHitLocations == null)
			{
				beamHitLocations = new List<Vector3>();
			}
		}
	}
}
