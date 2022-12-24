using RimWorld;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.AI;

namespace flangoCore
{
    public class JobDriver_UseItemHold : JobDriver_UseItem
    {
        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOnIncapable(PawnCapacityDefOf.Manipulation);
            this.FailOn(() => !TargetThingA.TryGetComp<CompUsable>().CanBeUsedBy(pawn, out var _));
            yield return Toils_Goto.GotoThing(TargetIndex.A, TargetThingA.def.hasInteractionCell ? PathEndMode.InteractionCell : PathEndMode.OnCell);
            yield return Toils_Ingest.PickupIngestible(TargetIndex.A, pawn);
            yield return UsePickedUpItem(pawn, TargetIndex.A, TargetIndex.B).FailOn((Toil x) => !TargetThingA.Spawned && (pawn.carryTracker == null || pawn.carryTracker.CarriedThing != TargetThingA)).FailOnCannotTouch(TargetIndex.A, PathEndMode.Touch);
            //yield return PrepareToUse(); // makes pawns pick it up then drop it
            yield return Use();
        }

        public Toil UsePickedUpItem(Pawn user, TargetIndex itemInd, TargetIndex useSurfaceInd = TargetIndex.None)
        {
            Toil toil = ToilMaker.MakeToil("UsePickedUpItem");
            toil.FailOnDestroyedNullOrForbidden(TargetIndex.A);
            toil.initAction = delegate
            {
                Pawn actor = toil.actor;
                Thing thing4 = actor.CurJob.GetTarget(itemInd).Thing;
                if (thing4.IsBurning())
                {
                    user.jobs.EndCurrentJob(JobCondition.Incompletable);
                }
                else
                {
                    toil.actor.pather.StopDead();
                    actor.jobs.curDriver.ticksLeftThisToil = useDuration;
                    if (thing4.Spawned)
                    {
                        thing4.Map.physicalInteractionReservationManager.Reserve(user, actor.CurJob, thing4);
                    }
                }
            };
            toil.tickAction = delegate
            {
                user.CurJob.GetTarget(itemInd).Thing.TryGetComp<CompUseEffect>()?.PrepareTick();
                CompUsable compUsable = job.GetTarget(TargetIndex.A).Thing.TryGetComp<CompUsable>();

                if (compUsable != null && warmupMote == null && compUsable.Props.warmupMote != null)
                {
                    warmupMote = MoteMaker.MakeAttachedOverlay(job.GetTarget(TargetIndex.B).Thing, compUsable.Props.warmupMote, Vector3.zero);
                }

                warmupMote?.Maintain();
                if (user != toil.actor)
                {
                    toil.actor.rotationTracker.FaceCell(user.Position);
                }
                else
                {
                    Thing thing3 = toil.actor.CurJob.GetTarget(itemInd).Thing;
                    if (thing3 != null && thing3.Spawned)
                    {
                        toil.actor.rotationTracker.FaceCell(thing3.Position);
                    }
                    else if (useSurfaceInd != 0 && toil.actor.CurJob.GetTarget(useSurfaceInd).IsValid)
                    {
                        toil.actor.rotationTracker.FaceCell(toil.actor.CurJob.GetTarget(useSurfaceInd).Cell);
                    }
                }
                toil.actor.GainComfortFromCellIfPossible();
            };
            toil.WithProgressBarToilDelay(itemInd, useDuration);
            toil.defaultCompleteMode = ToilCompleteMode.Delay;
            toil.FailOnDestroyedOrNull(itemInd);
            toil.AddFinishAction(delegate
            {
                if (user != null && user.CurJob != null)
                {
                    Thing thing = user.CurJob.GetTarget(itemInd).Thing;
                    if (thing != null && user.Map.physicalInteractionReservationManager.IsReservedBy(user, thing))
                    {
                        user.Map.physicalInteractionReservationManager.Release(user, toil.actor.CurJob, thing);
                    }
                }
            });
            toil.handlingFacing = true;
            return toil;
        }
    }
}
