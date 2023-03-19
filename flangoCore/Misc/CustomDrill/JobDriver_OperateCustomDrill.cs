using RimWorld;
using System.Collections.Generic;
using Verse.AI;
using Verse;

namespace flangoCore
{
    public class JobDriver_OperateCustomDrill : JobDriver
    {
        public override bool TryMakePreToilReservations(bool errorOnFailed)
        {
            return pawn.Reserve(job.targetA, job, 1, -1, null, errorOnFailed);
        }

        protected override IEnumerable<Toil> MakeNewToils()
        {
            this.FailOnDespawnedNullOrForbidden(TargetIndex.A);
            this.FailOnBurningImmobile(TargetIndex.A);
            this.FailOnThingHavingDesignation(TargetIndex.A, DesignationDefOf.Uninstall);
            var comp = job.targetA.Thing.TryGetComp<CompCustomDrill>();
            this.FailOn(() => comp == null || (comp != null && !comp.CanDrillNow()));
            yield return Toils_Goto.GotoThing(TargetIndex.A, PathEndMode.InteractionCell);
            Toil work = ToilMaker.MakeToil("MakeNewToils");
            work.tickAction = delegate
            {
                Pawn actor = work.actor;
                ((Building)actor.CurJob.targetA.Thing).GetComp<CompCustomDrill>().DrillWorkDone(actor);
                actor.skills.Learn(SkillDefOf.Mining, 0.065f);
            };
            work.defaultCompleteMode = ToilCompleteMode.Never;
            work.WithEffect(EffecterDefOf.Drill, TargetIndex.A);
            work.FailOnCannotTouch(TargetIndex.A, PathEndMode.InteractionCell);
            work.FailOnDespawnedNullOrForbidden(TargetIndex.A);
            work.activeSkill = () => SkillDefOf.Mining;
            yield return work;
        }
    }
}
