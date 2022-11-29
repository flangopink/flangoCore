using System.Collections.Generic;
using Verse;
using Verse.Sound;
using RimWorld;
using UnityEngine;

namespace flangoCore
{
    public class CompProperties_RemoteExplosive : CompProperties
    {
        public string commandLabel = "Detonate";
        public string commandDescription = "Make this thing go kaboom.";
        public string commandIcon;
        public int timerTicks;
        public float range = 0.9f;
        public DamageDef damageDef;
        public int damage = -1;
        public float armorPenetration = -1;
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
        public bool destroyOnDetonation = true;
        public bool explodeOnDestroyed = true;
        public bool canBeCancelled;
        public string cancelLabel = "Cancel";
        public string cancelDescription = "Cancel kaboom.";
        public string cancelIcon;
        public SoundDef soundActivated;
        public SoundDef soundCancelled;
        public bool showAlert = false;
        public ExplosionShape explosionShape;
        public SoundDef sustainerSound;
        public bool drawWick = false;
        public EffecterDef explosionEffecter;
        public FleckProps fleck;
        public FleckProps explosionFleck;
        public int explosionFleckCount;

        public CompProperties_RemoteExplosive()
        {
            compClass = typeof(CompRemoteExplosive);
        }
    }
    public class CompRemoteExplosive : ThingComp
    {
        public CompProperties_RemoteExplosive Props => (CompProperties_RemoteExplosive)props;
        public Texture2D Icon => ContentFinder<Texture2D>.Get(Props.commandIcon) ?? BaseContent.BadTex;
        public Texture2D CancelIcon => ContentFinder<Texture2D>.Get(Props.cancelIcon) ?? BaseContent.BadTex;

        private int timer = -1;

        public bool bombHasBeenPlanted;
        public bool detonated;
        protected Sustainer soundSustainer;
        private OverlayHandle? overlayBurningWick;

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            if (timer == -1) timer = Props.timerTicks;
            UpdateOverlays();
        }

        public override void PostPreApplyDamage(DamageInfo dinfo, out bool absorbed)
        {
            absorbed = false;
            if (Props.explodeOnDestroyed && dinfo.Def.ExternalViolenceFor(parent) && dinfo.Amount >= parent.HitPoints)
            {
                if (parent.MapHeld != null)
                {
                    Kaboom();
                    parent.Destroy();
                    if (parent.Destroyed) absorbed = true;
                }
            }
        }

        public override void CompTick()
        {
            base.CompTick();
            if (bombHasBeenPlanted)
            {
                if (soundSustainer == null) StartSustainer();
                else soundSustainer.Maintain();

                if (Props.fleck != null) Props.fleck.MakeFleck(parent.Map, parent.Position);

                timer--;

                if (!detonated && timer <= 0)
                {
                    detonated = true;
                    EndSustainer();
                    Kaboom();
                    for (int i = 0; i < Props.explosionFleckCount; i++) Props.explosionFleck.MakeFleck(parent.Map, parent.Position);
                    if (Props.destroyOnDetonation) parent.Destroy();
                }
            }
        }

        private void UpdateOverlays()
        {
            if (parent.Spawned && Props.drawWick)
            {
                parent.Map.overlayDrawer.Disable(parent, ref overlayBurningWick);
                if (bombHasBeenPlanted) overlayBurningWick = parent.Map.overlayDrawer.Enable(parent, OverlayTypes.BurningWick);
            }
        }

        private void Kaboom()
        {
            if (Props.explosionEffecter != null)
            {
                Effecter effecter = Props.explosionEffecter.Spawn();
                effecter.Trigger(new TargetInfo(parent.PositionHeld, parent.Map), new TargetInfo(parent.PositionHeld, parent.Map));
                effecter.Cleanup();
            }
            parent.DoExplosion(Props.range, Props.explosionShape, Props.damageDef ?? DamageDefOf.Bomb, parent, Props.damage, Props.armorPenetration, Props.soundDef, null, null, null, Props.preExplosionSpawnThing, Props.preExplosionSpawnChance, Props.preExplosionSpawnCount, Props.postExplosionSpawnThing, Props.postExplosionSpawnChance, Props.postExplosionSpawnCount, Props.applyDamageToCellNeighbors, Props.chanceToStartFire, Props.damageFalloff);
        }
        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            Command_Action c_detonate = new Command_Action()
            {
                defaultLabel = Props.commandLabel + (Props.timerTicks > 0 ? $" ({timer.TicksToSeconds():0}{"LetterSecond".Translate()})" : string.Empty),
                defaultDesc = Props.commandDescription,
                icon = Icon,
                disabled = bombHasBeenPlanted,
                action = delegate
                {
                    bombHasBeenPlanted = true;
                    if (Props.showAlert) Messages.Message("fc_ThingWillExplodeInSeconds".Translate(timer.TicksToSeconds()), parent, MessageTypeDefOf.NeutralEvent, false);
                    Props.soundActivated?.PlayOneShot(new TargetInfo(parent.Position, parent.Map));

                    StartSustainer(); 
                    GenExplosion.NotifyNearbyPawnsOfDangerousExplosive(parent, Props.damageDef);
                    UpdateOverlays();
                }
            };
            yield return c_detonate;
            
            if (Props.canBeCancelled && bombHasBeenPlanted) 
            {
                yield return new Command_Action()
                {
                    defaultLabel = Props.cancelLabel,
                    defaultDesc = Props.cancelDescription,
                    icon = CancelIcon,
                    action = delegate
                    {
                        bombHasBeenPlanted = false;
                        timer = Props.timerTicks;
                        Props.soundCancelled?.PlayOneShot(new TargetInfo(parent.Position, parent.Map)); 
                    }
                }; 
            }
        }

        private void StartSustainer()
        {
            SoundInfo info = SoundInfo.InMap(parent, MaintenanceType.PerTick);
            soundSustainer = Props.sustainerSound.TrySpawnSustainer(info);
            foreach (SubSoundDef s in soundSustainer?.def.subSounds) s.muteWhenPaused = true;
        }

        private void EndSustainer()
        {
            if (soundSustainer != null)
            {
                soundSustainer.End();
                soundSustainer = null;
            }
        }

        public override void PostExposeData()
        {
            if (timer == -1) timer = Props.timerTicks;
            base.PostExposeData();
            Scribe_Values.Look(ref timer, "timer", Props.timerTicks);
            Scribe_Values.Look(ref bombHasBeenPlanted, "bombHasBeenPlanted");
        }
    }
}
