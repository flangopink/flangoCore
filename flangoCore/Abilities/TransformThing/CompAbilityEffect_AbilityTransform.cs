using RimWorld;
using Verse;
using Verse.Sound;
using UnityEngine;
using System.Collections.Generic;

namespace flangoCore
{
	public class CompProperties_AbilityTransform : CompProperties_AbilityEffect
	{
		public CompProperties_AbilityTransform()
		{
			compClass = typeof(CompAbilityEffect_AbilityTransform);
		}

		public bool appliesToPawns;
		public bool resultIsPawn;
		public List<ThingDef> canApplyTo;
		public List<TransformOutcomeOptions> transformOptions;
		public ThingDef thingToSpawn;
		public int resultStackCount = -1;
		public List<FleckProps> flecks;
	}

	public class CompAbilityEffect_AbilityTransform : CompAbilityEffect
    {
		public new CompProperties_AbilityTransform Props => (CompProperties_AbilityTransform)props;

		public override bool CanApplyOn(LocalTargetInfo target, LocalTargetInfo dest)
		{
			if (target.Thing == null || !Props.canApplyTo.Any(x => target.Thing.def == x))
			{
				Messages.Message("MessageTransportPodsDestinationIsInvalid".Translate(), MessageTypeDefOf.RejectInput);
				return false;
			}

			if (Props.appliesToPawns && !Props.availableWhenTargetIsWounded && (target.Pawn.health.hediffSet.BleedRateTotal > 0f || target.Pawn.health.HasHediffsNeedingTend()))
			{
				return false;
			}

			return true;
		}

		public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
		{
			var targetThing = Props.appliesToPawns && (Thing)target is Pawn ? target.Pawn : target.Thing;

			var resultThing = Props.resultIsPawn ? new Pawn() : new Thing();

			if (Props.transformOptions == null) 
			{ 
				Log.Error("transformOptions are empty"); 
				return;
			}

			if (Props.thingToSpawn == null) Props.thingToSpawn = Props.transformOptions[0].thingDef;

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
			}
			soundDef?.PlayOneShot(new TargetInfo(target.Cell, parent.pawn.Map));

			resultThing.def = Props.thingToSpawn;

			if (Props.resultIsPawn && resultThing is Pawn resultPawn)
			{
				resultPawn.kindDef = PawnKindDef.Named(Props.thingToSpawn.ToString());
				//Log.Message($"Trying to spawn {resultPawn.def}, kind: {resultPawn.kindDef}");

				if (resultPawn.def == null || resultPawn.kindDef == null)
				{
					resultPawn.def = ThingDef.Named("Rat");
					resultPawn.kindDef = PawnKindDef.Named("Rat");
					Log.Error("Transform things's thingDef or kindDef was null.");
					return;
				}

				Pawn targetPawn = (Pawn)targetThing;

				PawnGenerationRequest req = new PawnGenerationRequest(resultPawn.kindDef, targetPawn.Faction, PawnGenerationContext.NonPlayer, -1);
				var reqPawn = PawnGenerator.GeneratePawn(req);

				reqPawn.Name = targetPawn.Name ?? targetPawn.Name;

				reqPawn.gender = targetPawn.gender != Gender.None ? targetPawn.gender : Gender.None;

				GenSpawn.Spawn(reqPawn, target.Cell, target.Pawn.Map);

				if (reqPawn.kindDef != resultPawn.kindDef)
					Log.Error($"Something went wrong while spawning a pawn of kind {resultPawn.kindDef}. Instead spawned of kind {reqPawn.kindDef}");
			}
            else
            {
				if (resultThing.def == null)
				{
					resultThing.def = ThingDef.Named("Wood");
					Log.Error("Transform result things's thingDef was null.");
					return;
				}

				if (DefDatabase<PawnKindDef>.GetNamedSilentFail(resultThing.def.defName) != null)
				{
					resultThing.def = ThingDef.Named("Wood");
					Log.Error("Transform result thing is a Pawn, but <resultIsPawn> is false.");
					return;
				}

				Thing thing = ThingMaker.MakeThing(resultThing.def, targetThing.Stuff ?? null);
				thing.stackCount = Props.resultStackCount == -1 ? targetThing.stackCount : Props.resultStackCount;
				GenPlace.TryPlaceThing(thing, target.Cell, target.Thing.Map, ThingPlaceMode.Near);
			}

			if (!Props.flecks.NullOrEmpty())
			{
				foreach (FleckProps fleck in Props.flecks)
				{
					fleck.MakeFleck(parent.pawn.Map, targetThing.Position.ToVector3());
				}
			}

			targetThing.DeSpawn();
		}
	}
}
