using System.Collections.Generic;
using Verse;
using RimWorld;
using UnityEngine;

namespace flangoCore
{
    public class CompProperties_RemoteExplosive : CompProperties
    {
        public string commandLabel;
        public string commandIcon;
        public int timerTicks;
        public float range = 0.9f;
        public DamageDef damageDef;
        public int damage;
        public float armorPenetration;
        public SoundDef soundDef;
        public ThingDef preExplosionSpawnThing;
        public float preExplosionSpawnChance;
        public int preExplosionSpawnCount;
        public ThingDef postExplosionSpawnThing;
        public float postExplosionSpawnChance;
        public int postExplosionSpawnCount;
        public bool applyDamageToCellNeighbors;
        public float chanceToStartFire;
        public bool damageFalloff;
        public bool destroyOnDetonation;

        public CompProperties_RemoteExplosive()
        {
            compClass = typeof(CompRemoteExplosive);
        }
    }
    public class CompRemoteExplosive : ThingComp
    {
        public CompProperties_RemoteExplosive Props => (CompProperties_RemoteExplosive)props;
        public Texture2D Icon => ContentFinder<Texture2D>.Get(Props.commandIcon) ?? BaseContent.BadTex;

        private int timer = -1;

        public bool bombHasBeenPlanted;
        public bool detonated;

        public override void CompTick()
        {
            base.CompTick();
            if (bombHasBeenPlanted)
            {
                timer--;
                if (!detonated && timer <= 0)
                {
                    detonated = true;
                    parent.DoExplosion(Props.range, 0, Props.damageDef, parent, Props.damage, Props.armorPenetration, Props.soundDef, null, null, null, Props.preExplosionSpawnThing, Props.preExplosionSpawnChance, Props.preExplosionSpawnCount, Props.postExplosionSpawnThing, Props.postExplosionSpawnChance, Props.postExplosionSpawnCount, Props.applyDamageToCellNeighbors, Props.chanceToStartFire, Props.damageFalloff);
                    parent.Destroy();
                }
            }
        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            yield return new Command_Action()
            {
                defaultLabel = Props.commandLabel,
                icon = Icon,
                action = delegate 
                { 
                    bombHasBeenPlanted = true;
                    Messages.Message("fc_ThingWillExplodeInSeconds".Translate(timer.TicksToSeconds()), parent, MessageTypeDefOf.NeutralEvent);
                }
            };
        }

        public override void PostExposeData()
        {
            if (timer == -1) timer = Props.timerTicks;
            base.PostExposeData();
            Scribe_Values.Look(ref timer, "timer", Props.timerTicks);
        }
    }
}
