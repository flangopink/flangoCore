using System.Collections.Generic;
using RimWorld;
using Verse;

namespace flangoCore
{
    public class CompProperties_ProjectileVFX : CompProperties
    {
        public bool rotating;
        public bool counterClockwise;
        public float rotationSpeed = 10f;

        public bool emitFlecks;
        public List<FleckProps> flecks;

        public CompProperties_ProjectileVFX()
        {
            compClass = typeof(CompProjectileVFX);
        }
    }

    public class CompProjectileVFX : ThingComp
    {
        public CompProperties_ProjectileVFX Props => (CompProperties_ProjectileVFX)props;

        public override void CompTick()
        {
            base.CompTick();

            if (Props.emitFlecks)
            {
                foreach (FleckProps fleck in Props.flecks)
                {
                    if (parent.Map != null && Find.TickManager.TicksGame % fleck.intervalTicks == 0)
                    {
                        Map map = parent.Map;
                        FleckCreationData dataStatic = FleckMaker.GetDataStatic(parent.DrawPos + fleck.offset, map, fleck.fleckDef, fleck.scaleRange.RandomInRange);
                        if (fleck.randomRotation) dataStatic.rotation = Rand.Range(0f, 360f);
                        map.flecks.CreateFleck(dataStatic);
                    }
                }
            }
        }
    }
}
