using RimWorld;
using Verse.AI;
using Verse;
using System.Collections.Generic;
using System.Linq;
using System;
using static UnityEngine.Scripting.GarbageCollector;
using Verse.Noise;
using VFECore;

namespace flangoCore
{
    [Flags]
    public enum DecoyTargetFlags
    {
        Hostile = 0,
        Neutral = 1,
        Colony = 2,
        NonFaction = 4,
        NeutralNoNonFaction = 8,
        All = 16
    }

    public class CompProperties_Decoy : CompProperties
    {
        public IntRange jobDurationTicks = new IntRange(300, 480); // 5~8 sec
        public IntRange forcedAttackCount = new IntRange(2, 6);
        public int disappearAfterTicks = 600; // 10 sec
        public int updateInterval = 60; // if 0 then only initial pawns get affected
        public float range = 9.9f;
        public bool requireLOS = true;
        public bool ignoredByOtherDecoys = true;
        public bool destoyedByEMP;
        public DecoyTargetFlags targetFlags;
        public EffecterDef effecterDestroyed;
        public FleckProps fleckDestroyed;

        public CompProperties_Decoy() => compClass = typeof(CompDecoy);
    }

    [HotSwap.HotSwappable]
    public class CompDecoy : ThingComp
    {
        public CompProperties_Decoy Props => (CompProperties_Decoy)props;
        private int timer;
        private IntVec3 lastPos;

        public override void CompTick()
        {
            base.CompTick();
            if (timer <= 0)
            {
                lastPos = parent.Position;
                parent.Destroy();
                return;
            }

            if (Props.updateInterval > 0 && Find.TickManager.TicksGame % Props.updateInterval == 0)
                AggroNearbyPawns();

            timer--;
        }

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            timer = Props.disappearAfterTicks;
            AggroNearbyPawns();
        }

        public override void PostDestroy(DestroyMode mode, Map previousMap)
        {
            if (Props.effecterDestroyed != null)
            {
                Effecter effecter = Props.effecterDestroyed.Spawn();
                effecter.Trigger(new TargetInfo(lastPos, previousMap), new TargetInfo(lastPos, previousMap));
                effecter.Cleanup();
            }
            Props.fleckDestroyed?.MakeFleck(previousMap, lastPos);
            base.PostDestroy(mode, previousMap);
        }

        public override void PostPreApplyDamage(DamageInfo dinfo, out bool absorbed)
        {
            absorbed = false;
            if (Props.destoyedByEMP && (dinfo.Def == DamageDefOf.EMP || dinfo.Def.externalViolenceForMechanoids))
            {
                if (parent.MapHeld != null)
                {
                    parent.Destroy();
                    if (parent.Destroyed) absorbed = true;
                }
            }
        }

        public void AggroNearbyPawns()
        {
            if (parent == null || parent.Position == null || parent.Map == null) return; // Just in case

            //List<Pawn> pawns = GenRadial.RadialCellsAround(parent.Position, Props.range, true).Where(cell => cell.InBounds(parent.Map)).SelectMany(cell => cell.GetThingList(parent.Map)).OfType<Pawn>().ToList();

            foreach (Pawn pawn in Utils.GetPawnsInRange(parent.Position, parent.Map, Props.range, Props.requireLOS))
            {
                if (pawn.CurJob == null || pawn.CurJob.AnyTargetIs(parent) || (Props.ignoredByOtherDecoys && pawn.CurJob?.targetA.Thing?.TryGetComp<CompDecoy>() != null)) return;

                switch (Props.targetFlags)
                {
                    case DecoyTargetFlags.All: 
                        break;
                    case DecoyTargetFlags.NonFaction:
                        if (pawn.Faction == null) break; 
                        else continue;
                    case DecoyTargetFlags.Colony:
                        if (pawn.Faction == Faction.OfPlayer) break;
                        else continue;
                    case DecoyTargetFlags.Neutral:
                        if (pawn.Faction != Faction.OfPlayer && !pawn.Faction.HostileTo(Faction.OfPlayer)) break;
                        else continue;
                    case DecoyTargetFlags.NeutralNoNonFaction:
                        if (pawn.Faction != null && pawn.Faction != Faction.OfPlayer && !pawn.Faction.HostileTo(Faction.OfPlayer)) break;
                        else continue;
                    case DecoyTargetFlags.Hostile:
                        if (pawn.Faction.HostileTo(Faction.OfPlayer)) break;
                        else continue;
                }

                Job job;
                if (pawn.CurrentEffectiveVerb?.verbProps.IsMeleeAttack ?? true)
                {
                    job = JobMaker.MakeJob(JobDefOf.AttackMelee, new LocalTargetInfo(parent));
                }
                else
                {
                    job = JobMaker.MakeJob(JobDefOf.AttackStatic, new LocalTargetInfo(parent));
                    job.maxNumStaticAttacks = Props.forcedAttackCount.RandomInRange;
                    job.endIfCantShootTargetFromCurPos = Props.requireLOS;
                }

                job.expiryInterval = Props.jobDurationTicks.RandomInRange;
                pawn.jobs.StopAll();
                pawn.jobs.StartJob(job, JobCondition.InterruptForced);
            }
        }
    }
}
