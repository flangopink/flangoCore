using Verse;
using Verse.Sound;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace flangoCore
{
    public class CompProperties_AudioSource : CompProperties
    {
        public List<SoundDef> soundDefs;
        public bool hideGizmos;
        public bool random;
        public bool canPlay = true;
        public bool canSwitch = true;
        public bool canStop = true;
        public IntRange randomIntervalTicks = new(200, 800);
        public string commandPlayLabel = "Play";
        public string commandPlayDesc = "Play selected sound.";
        public string commandPlayIcon = "";
        public string commandSwitchLabel = "Switch audio...";
        public string commandSwitchDesc = "Switch current audio.";
        public string commandSwitchIcon = "";
        public string commandStopLabel = "Stop";
        public string commandStopDesc = "Stop sound.";
        public string commandStopIcon = "";

        public CompProperties_AudioSource()
        {
            compClass = typeof(CompAudioSource);
        }
    }

    public class CompAudioSource : ThingComp
    {
        public CompProperties_AudioSource Props => (CompProperties_AudioSource)props;

        public Texture2D PlayIcon => ContentFinder<Texture2D>.Get(Props.commandPlayIcon) ?? BaseContent.BadTex;
        public Texture2D SwitchIcon => ContentFinder<Texture2D>.Get(Props.commandSwitchIcon) ?? BaseContent.BadTex;
        public Texture2D StopIcon => ContentFinder<Texture2D>.Get(Props.commandStopIcon) ?? BaseContent.BadTex;

        private SoundDef currentSound;

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);

            if (currentSound == null && !Props.soundDefs.NullOrEmpty()) 
                currentSound = Props.soundDefs.First();
        }

        public override void PostExposeData()
        {
            Scribe_Defs.Look(ref currentSound, "currentSound");
        }

        public override void CompTick()
        {
            if (Props.random && Find.TickManager.TicksGame % Props.randomIntervalTicks.RandomInRange == 0)
            {
                currentSound = Props.soundDefs.RandomElement();
                PlayAtThing(currentSound, parent);
            }
        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            if (Props.hideGizmos || Props.soundDefs.NullOrEmpty()) yield break;

            if (Props.canPlay)
            {
                Command_Action c_play = new()
                {
                    defaultLabel = Props.commandPlayLabel,
                    defaultDesc = Props.commandPlayDesc,
                    icon = PlayIcon,
                    action = delegate
                    {
                        PlayAtThing(currentSound, parent);
                    }
                };
                yield return c_play;
            }

            if (Props.canStop)
            {
                Command_Action c_stop = new()
                {
                    defaultLabel = Props.commandStopLabel,
                    defaultDesc = Props.commandStopDesc,
                    icon = StopIcon,
                    action = delegate
                    {
                        Find.SoundRoot.oneShotManager.PlayingOneShots.FirstOrDefault(x => x.subDef.parentDef == currentSound)?.source?.Stop();
                    }
                };
                yield return c_stop;
            }

            if (Props.soundDefs.Count > 1 && Props.canSwitch)
            {
                Command_Action c_switch = new()
                {
                    defaultLabel = Props.commandSwitchLabel,
                    defaultDesc = Props.commandSwitchDesc,
                    icon = SwitchIcon,
                    action = delegate
                    {
                        List<FloatMenuOption> list = new();

                        foreach (var sound in Props.soundDefs)
                        {
                            list.Add(new FloatMenuOption((string)sound.LabelCap ?? sound.defName, delegate
                            { currentSound = sound; }));
                        }
                        Find.WindowStack.Add(new FloatMenu(list));
                    }
                };
                yield return c_switch;
            }
        }

        private void PlayAtThing(SoundDef def, Thing thing)
        {
            if (def != null && thing.SpawnedOrAnyParentSpawned)
            {
                SoundInfo info = SoundInfo.InMap(new TargetInfo(thing.PositionHeld, thing.MapHeld));
                def.PlayOneShot(info);
            }
        }
    }
}
