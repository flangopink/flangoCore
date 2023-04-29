using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace flangoCore
{
    public class CompProperties_ProjectileVFX : CompProperties
    {
        public bool rotating;
        public bool counterClockwise;
        public float rotationSpeed = 10f;
        public bool stopRotatingOnImpact = true;

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
            if (Props.flecks.NullOrEmpty()) return;

            foreach (FleckProps fleck in Props.flecks)
            {
                if (parent.Map != null && parent.IsHashIntervalTick(fleck.intervalTicks))
                {
                    fleck.MakeFleck(parent.Map, parent.DrawPos);
                }
            }
        }
    }
}
