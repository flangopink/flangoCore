using RimWorld;
using Verse;
using Verse.Sound;
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace flangoCore
{
	public class CompProperties_AbilityTransform : CompProperties_AbilityEffect
	{
		public CompProperties_AbilityTransform()
		{
			compClass = typeof(CompAbilityEffect_AbilityTransform);
		}

		public string optionsIconPath;
		public string resetIconPath;

		public List<ThingDef> canApplyTo;
		public List<TransformOutcomeOptions> transformOptions;
		public List<FleckProps> flecks;
	}

	public class CompAbilityEffect_AbilityTransform : CompAbilityEffect
    {
		public new CompProperties_AbilityTransform Props => (CompProperties_AbilityTransform)props;

		public TransformOutcomeOptions option;

        public override void Initialize(AbilityCompProperties props)
        {
            base.Initialize(props);
			if (option == null) option = Props.transformOptions[0];
		}

        public override bool CanApplyOn(LocalTargetInfo target, LocalTargetInfo dest)
		{
			var itemsOnCell = target.Cell.GetThingList(parent.pawn.Map);
			if (itemsOnCell.NullOrEmpty() || !itemsOnCell.Select(x => x.def).Intersect(Props.canApplyTo).Any())
			{
				Messages.Message("MessageTransportPodsDestinationIsInvalid".Translate(), MessageTypeDefOf.RejectInput);
				return false;
			}
            else if (option != null && target.Cell.GetThingList(parent.pawn.Map).First(x => Props.canApplyTo.Contains(x.def)) is Thing thing && !(thing is Pawn) && thing.stackCount < option.requiredStackCount)
            {
				// Did not meet the stack count requirement.
				Messages.Message("fc_MessageRequiredItemStackCountIs".Translate(option.requiredStackCount), MessageTypeDefOf.RejectInput);
				return false;
			}

			//if (Props.appliesToPawns && !Props.availableWhenTargetIsWounded && (target.Pawn.health.hediffSet.BleedRateTotal > 0f || target.Pawn.health.HasHediffsNeedingTend()))
			if (target.Thing is Pawn pawn && !Props.availableWhenTargetIsWounded && (pawn.health.hediffSet.BleedRateTotal > 0f || pawn.health.HasHediffsNeedingTend()))
			{
				return false;
			}

			return true;
		}

		public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
		{
			if (Props.transformOptions == null)
			{
				Log.Error("<transformOptions> are empty.");
				return;
			}

			var thingToSpawn = option.thingDef;

			//var targetThing = Props.appliesToPawns && (Thing)target is Pawn ? target.Pawn : target.Thing;
			var targetThing = target.Thing != null && target.Thing.GetType() == typeof(Pawn) ? (Pawn)target : target.Cell.GetThingList(parent.pawn.Map).First(x => Props.canApplyTo.Contains(x.def));

			//var resultThing = Props.resultIsPawn ? new Pawn() : new Thing();
			var resultThing = DefDatabase<PawnKindDef>.GetNamedSilentFail(thingToSpawn.defName) != null ? new Pawn() : new Thing();
			resultThing.def = thingToSpawn;

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

			//if (Props.resultIsPawn && resultThing is Pawn resultPawn)
			if (resultThing is Pawn resultPawn)
			{
				resultPawn.kindDef = PawnKindDef.Named(thingToSpawn.defName);
				//Log.Message($"Trying to spawn {resultPawn.def}, kind: {resultPawn.kindDef}");

				if (resultPawn.def == null || resultPawn.kindDef == null)
				{
					resultPawn.def = ThingDef.Named("Rat");
					resultPawn.kindDef = PawnKindDef.Named("Rat");
					Log.Error("Transform result thingDef or kindDef was null.");
					return;
				}

				Faction faction = null;
                switch (option.faction)
                {
                    case ResultFaction.Current:
                        faction = targetThing.Faction;
                        break;
                    case ResultFaction.Neutral:
                        faction = null;
                        break;
                    case ResultFaction.Player:
                        faction = Faction.OfPlayer;
                        break;
                    case ResultFaction.Enemy:
                        faction = Faction.OfPirates;
                        break;
                    default:
                        break;
                }

                PawnGenerationRequest req = new PawnGenerationRequest(resultPawn.kindDef, faction, PawnGenerationContext.NonPlayer, -1);
				var reqPawn = PawnGenerator.GeneratePawn(req);

				if (targetThing is Pawn pawn)
				{
					reqPawn.Name = pawn.Name ?? new NameTriple("New", targetThing.LabelCap, "Creature");
					reqPawn.gender = pawn.gender != Gender.None ? pawn.gender : Gender.None;
				}
                else
                {
					reqPawn.Name = new NameTriple("New", targetThing.LabelCap, "Creature");
					reqPawn.gender = reqPawn.RaceProps.hasGenders ? (Gender)Rand.Range(1, 2) : 0;
				}

				GenSpawn.Spawn(reqPawn, target.Cell, targetThing.Map);

				if (reqPawn.kindDef != resultPawn.kindDef)
					Log.Error($"Something went wrong while spawning a pawn of kind {resultPawn.kindDef}. Instead spawned of kind {reqPawn.kindDef}");

				targetThing.DeSpawn();
			}
            else
            {
				if (resultThing.def == null)
				{
					resultThing.def = ThingDefOf.WoodLog;
					Log.Error("Transform result thingDef was null.");
					return;
				}

				/*if (DefDatabase<PawnKindDef>.GetNamedSilentFail(resultThing.def.defName) != null)
				{
					resultThing.def = ThingDefOf.WoodLog;
					Log.Error("Transform result thing is a Pawn, but <resultIsPawn> is false.");
					return;
				}*/

				Thing thing = ThingMaker.MakeThing(resultThing.def, targetThing.Stuff);
				thing.stackCount = option.resultStackCount == -1 ? targetThing.stackCount : option.resultStackCount;

				if (targetThing.stackCount - option.requiredStackCount == 0) targetThing.DeSpawn();
				else targetThing.stackCount -= option.requiredStackCount;

				GenPlace.TryPlaceThing(thing, target.Cell, parent.pawn.Map, ThingPlaceMode.Near);
			}

			if (!Props.flecks.NullOrEmpty())
			{
				foreach (FleckProps fleck in Props.flecks)
				{
					fleck.MakeFleck(parent.pawn.Map, targetThing.Position.ToVector3());
				}
			}
        }
    }
}
