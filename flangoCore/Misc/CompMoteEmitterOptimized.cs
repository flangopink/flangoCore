using RimWorld;
using Verse;
using Verse.Sound;
using UnityEngine;
using System.Collections.Generic;

namespace flangoCore
{
    public class CompProperties_MoteEmitterOptimized : CompProperties
    {
        public ThingDef mote;

        public Vector3 offset;

        public Vector3 offsetMin = Vector3.zero;

        public Vector3 offsetMax = Vector3.zero;

        public SoundDef soundOnEmission;

        public int emissionInterval = -1;

        public int ticksSinceLastEmittedMaxOffset;

        public bool maintain;

        public string saveKeysPrefix;

        public Vector3 EmissionOffset => new(Rand.Range(offsetMin.x, offsetMax.x), Rand.Range(offsetMin.y, offsetMax.y), Rand.Range(offsetMin.z, offsetMax.z));

        // Check for bools instead of comps
        public bool hasCompPowerTrader;
        public bool hasCompSendSignalOnCountdown;
        public bool hasCompInitiatable;
        public bool isSkyfaller;

        public int updateInterval = 60;

        public CompProperties_MoteEmitterOptimized()
        {
            compClass = typeof(CompMoteEmitterOptimized);
        }

        public override IEnumerable<string> ConfigErrors(ThingDef parentDef)
        {
            if (mote == null)
            {
                yield return "CompMoteEmitter must have a mote assigned.";
            }
        }
    }

    public class CompMoteEmitterOptimized : ThingComp
    {
        public int ticksSinceLastEmitted;

        protected Mote mote;

        Skyfaller skyfaller;

        private CompProperties_MoteEmitterOptimized Props => (CompProperties_MoteEmitterOptimized)props;

        public override void Initialize(CompProperties props)
        {
            base.Initialize(props);
            if (Props.ticksSinceLastEmittedMaxOffset > 0)
            {
                ticksSinceLastEmitted = Rand.Range(0, Props.ticksSinceLastEmittedMaxOffset);
            }
        }

        public override void CompTick()
        {
            if (!parent.Spawned)
            {
                return;
            }


            if (parent.IsHashIntervalTick(Props.updateInterval))
            {

                if (Props.hasCompPowerTrader)
                {
                    CompPowerTrader comp = parent.GetComp<CompPowerTrader>();
                    if (comp != null && !comp.PowerOn)
                    {
                        return;
                    }
                }

                if (Props.hasCompSendSignalOnCountdown)
                {
                    CompSendSignalOnCountdown comp2 = parent.GetComp<CompSendSignalOnCountdown>();
                    if (comp2 != null && comp2.ticksLeft <= 0)
                    {
                        return;
                    }
                }

                if (Props.hasCompInitiatable)
                {
                    CompInitiatable comp3 = parent.GetComp<CompInitiatable>();
                    if (comp3 != null && !comp3.Initiated)
                    {
                        return;
                    }
                }

                if (Props.isSkyfaller)
                {
                    skyfaller = parent as Skyfaller;
                    if (skyfaller != null && skyfaller.FadingOut)
                    {
                        return;
                    }
                }
            }

            if (Props.emissionInterval != -1 && !Props.maintain)
            {
                if (ticksSinceLastEmitted >= Props.emissionInterval)
                {
                    Emit();
                    ticksSinceLastEmitted = 0;
                }
                else
                {
                    ticksSinceLastEmitted++;
                }
            }
            else if (mote == null || mote.Destroyed)
            {
                Emit();
            }

            if (mote != null && !mote.Destroyed)
            {
                if (Props.isSkyfaller && typeof(MoteAttached).IsAssignableFrom(Props.mote.thingClass) && skyfaller != null)
                {
                    mote.exactRotation = skyfaller.DrawAngle();
                }

                if (Props.maintain)
                {
                    mote.Maintain();
                }
            }
        }

        protected virtual void Emit()
        {
            if (!parent.Spawned)
            {
                Log.Error("Thing tried spawning mote without being spawned!");
                return;
            }

            Vector3 vector = Props.offset;
            if (Props.offsetMin != Vector3.zero || Props.offsetMax != Vector3.zero)
            {
                vector = Props.EmissionOffset;
            }

            if (typeof(MoteAttached).IsAssignableFrom(Props.mote.thingClass))
            {
                mote = MoteMaker.MakeAttachedOverlay(parent, Props.mote, vector);
            }
            else
            {
                Vector3 vector2 = parent.DrawPos + vector;
                if (vector2.InBounds(parent.Map))
                {
                    mote = MoteMaker.MakeStaticMote(vector2, parent.Map, Props.mote);
                }
            }

            if (!Props.soundOnEmission.NullOrUndefined())
            {
                Props.soundOnEmission.PlayOneShot(SoundInfo.InMap(parent));
            }
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref ticksSinceLastEmitted, ((Props.saveKeysPrefix != null) ? (Props.saveKeysPrefix + "_") : "") + "ticksSinceLastEmitted", 0);
        }
    }
}
