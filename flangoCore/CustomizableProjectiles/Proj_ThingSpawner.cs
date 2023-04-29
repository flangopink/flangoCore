using System.Collections.Generic;
using UnityEngine;
using RimWorld;
using Verse;
using HotSwap;

namespace flangoCore
{
    public class ModExt_ProjectileThingSpawner : DefModExtension
    {
        public int delayAfterShot;
        public int delayAfterImpact;
        public IntRange intervalTicks;
        public IntRange shootCount;
        public float shootRange;
        public float shootMinRange;
        public float spawnChance = 1f;
        public float shootChance = 1f;
        public ThingDef spawnThing;
        public ThingDef shootThing;
        public ThingDef impactThing;
        public ThingDef impactThingStuff;
        public EffecterDef effecter;
        public bool startFireOnImpact;
        public bool spawnThingOnFire;
        public bool requireLOS = true;
    }

    //[HotSwappable]
    public class Projectile_ThingSpawner : Projectile
    {
        public ModExt_ProjectileThingSpawner Ext => def.GetModExtension<ModExt_ProjectileThingSpawner>();

        private Thing Equipment => Launcher is Pawn p ? p.equipment.Primary : null;

        private int ticksToSpawn;
        private int delayTicks;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref ticksToSpawn, "ticksToSpawn", 0);
            Scribe_Values.Look(ref delayTicks, "delayTicks", 0);
        }

        public override void Tick()
        {
            base.Tick();
            if (ticksToImpact > 0) 
            {
                delayTicks++;
                if (this.IsHashIntervalTick(Ext.intervalTicks.RandomInRange))
                {
                    if (Map == null || !Position.InBounds(Map)) return;
                    if (delayTicks > Ext.delayAfterShot)
                    {
                        if (Ext.spawnThing != null && Rand.Chance(Ext.spawnChance))
                        {
                            Thing thing = ThingMaker.MakeThing(Ext.spawnThing);
                            GenSpawn.Spawn(thing, Position, Map);
                            if (Ext.spawnThingOnFire)
                            {
                                thing.TryAttachFire(Rand.Range(0.5f, 1f));
                                FireUtility.TryStartFireIn(Position, Map, Rand.Range(0.5f, 1f));
                            }
                        }
                        if (Ext.shootThing != null && Rand.Chance(Ext.shootChance)) ShootProjectiles();
                    }
                }
            }
            if (ticksToSpawn > 0)
            {
                ticksToSpawn--;
                if (ticksToSpawn <= 0) SpawnThing();
            }
        }

        protected override void Impact(Thing hitThing, bool blockedByShield = false)
        {
            if (!Position.InBounds(Map))
            {
                Destroy();
                return;
            }
            if (Ext.startFireOnImpact)
            {
                hitThing?.TryAttachFire(Rand.Range(0.5f, 1f));
                FireUtility.TryStartFireIn(Position, Map, Rand.Range(0.5f, 1f));
            }
            if (blockedByShield || Ext.delayAfterImpact == 0)
            {
                SpawnThing(); // Will destroy projectile
                return;
            }
            landed = true;
            ticksToSpawn = Ext.delayAfterImpact;
        }

        protected virtual void SpawnThing()
        {
            Map map = Map;
            Destroy();
            if (Ext.effecter != null)
            {
                Effecter effecter = Ext.effecter.Spawn();
                effecter.Trigger(new TargetInfo(Position, map), new TargetInfo(Position, map));
                effecter.Cleanup();
            }
            if (Ext.impactThing != null)
            {
                Thing thing = ThingMaker.MakeThing(Ext.impactThing, Ext.impactThingStuff);
                thing.SetFaction(Launcher.Faction);
                GenPlace.TryPlaceThing(thing, Position, map, ThingPlaceMode.Near);
            }
        }

        protected virtual void ShootProjectiles()
        {
            var cells = GenRadial.RadialCellsAround(Position, Ext.shootMinRange, Ext.shootRange);
            int shots = Ext.shootCount.RandomInRange;
            for (int i = 0; i < shots; i++)
            {
                IntVec3 cell = cells.RandomElement();
                Projectile proj = (Projectile)GenSpawn.Spawn(Ext.shootThing, Position, Map);
                proj.Launch(Launcher, cell, cell.GetFirstPawn(Map) ?? usedTarget, ProjectileHitFlags.IntendedTarget, false, Equipment);
            }
        }
    }
}
