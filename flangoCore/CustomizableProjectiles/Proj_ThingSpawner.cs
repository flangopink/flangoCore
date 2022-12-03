using System.Collections.Generic;
using RimWorld;
using Verse;

namespace flangoCore
{
    public class ModExt_ProjectileThingSpawner : DefModExtension
    {
        public int delayTicks;
        public ThingDef thingDef;
        public ThingDef stuff;
        public EffecterDef effecter;
    }

    public class Projectile_ThingSpawner : Projectile
    {
        public ModExt_ProjectileThingSpawner ModExt => def.GetModExtension<ModExt_ProjectileThingSpawner>();

        private int ticksToSpawn;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref ticksToSpawn, "ticksToSpawn", 0);
        }

        public override void Tick()
        {
            base.Tick();
            if (ticksToSpawn > 0)
            {
                ticksToSpawn--;
                if (ticksToSpawn <= 0)
                {
                    SpawnThing();
                }
            }
        }

        protected override void Impact(Thing hitThing, bool blockedByShield = false)
        {
            if (blockedByShield || ModExt?.delayTicks == 0)
            {
                SpawnThing();
                return;
            }
            landed = true;
            ticksToSpawn = ModExt.delayTicks;
        }

        protected virtual void SpawnThing()
        {
            if (ModExt == null)
            {
                Log.Error("ModExt_ProjectileThingSpawner is missing.");
                return;
            }

            Map map = Map;
            Destroy();
            if (ModExt.effecter != null)
            {
                Effecter effecter = ModExt.effecter.Spawn();
                effecter.Trigger(new TargetInfo(Position, map), new TargetInfo(Position, map));
                effecter.Cleanup();
            }
            Thing thing = ThingMaker.MakeThing(ModExt.thingDef, ModExt.stuff);
            thing.SetFaction(Launcher.Faction);
            GenPlace.TryPlaceThing(thing, Position, map, ThingPlaceMode.Near);
        }
    }
}
