using RimWorld;
using Verse;
using Verse.Sound;
using UnityEngine;

namespace flangoCore
{
    public class CompEffect_AbilityTransform : CompAbilityEffect
    {
		public new CompProperties_AbilityTransform Props => (CompProperties_AbilityTransform)props;

		public override bool CanApplyOn(LocalTargetInfo target, LocalTargetInfo dest)
		{
			if (!Props.canApplyTo.Any(x => target.Pawn.def == x))
			{
				Messages.Message("MessageTransportPodsDestinationIsInvalid".Translate(), MessageTypeDefOf.RejectInput);
				return false;
			}

			if (!Props.availableWhenTargetIsWounded && (target.Pawn.health.hediffSet.BleedRateTotal > 0f || target.Pawn.health.HasHediffsNeedingTend()))
			{
				return false;
			}

			return true;
		}

		public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
		{
			Pawn targetPawn = target.Pawn;
			Pawn val = new Pawn();

			if (Props.transformOptions == null) 
			{ 
				Log.Error("transformOptions are empty"); 
				return;
			}

			if (Props.thingToSpawn == null) Props.thingToSpawn = Props.transformOptions[0].thingDef;

			val.def = Props.thingToSpawn;
			val.kindDef = PawnKindDef.Named(Props.thingToSpawn.ToString());
			Log.Message($"Trying to spawn {val.def}, kind: {val.kindDef}");

			if (val.def == null || val.kindDef == null)
			{
				val.def = ThingDef.Named("Rat");
				val.kindDef = PawnKindDef.Named("Rat");
				Log.Error("Transform creature's thingDef or kindDef was null.");
			}

			SoundDef soundDef;
			switch (parent.pawn.gender)
			{
				case Gender.Male:
					soundDef = Props.soundMale ?? Props.sound;
					break;
				case Gender.Female:
					soundDef = Props.soundFemale ?? Props.sound;
					break;
				default:
					soundDef = Props.sound;
					break;
			};

			soundDef?.PlayOneShot(new TargetInfo(target.Cell, parent.pawn.Map));

			PawnGenerationRequest req = new PawnGenerationRequest(val.kindDef, targetPawn.Faction, PawnGenerationContext.NonPlayer, -1);
			var reqPawn = PawnGenerator.GeneratePawn(req);

			reqPawn.Name = targetPawn.Name ?? targetPawn.Name;

			reqPawn.gender = targetPawn.gender != Gender.None ? targetPawn.gender : Gender.None;

			GenSpawn.Spawn(reqPawn, target.Cell, target.Pawn.Map);

			if (reqPawn.kindDef != val.kindDef)
				Log.Error($"Something went wrong while spawning a pawn of kind {val.kindDef}. Instead spawned of kind {reqPawn.kindDef}");

			if (!Props.flecks.NullOrEmpty())
			{
				foreach (FleckProps fleck in Props.flecks)
				{
					fleck.MakeFleck(parent.pawn.Map, targetPawn.Position.ToVector3());
				}
			}

			targetPawn.DeSpawn();
		}
	}
}
