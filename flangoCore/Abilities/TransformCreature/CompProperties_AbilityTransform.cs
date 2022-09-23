using RimWorld;
using Verse;
using System.Collections.Generic;

namespace flangoCore
{
    public class CompProperties_AbilityTransform : CompProperties_AbilityEffect
    {
        public CompProperties_AbilityTransform()
        {
            compClass = typeof(CompEffect_AbilityTransform);
        }

        public List<ThingDef> canApplyTo;
        public List<TransformOutcomeOptions> transformOptions;
        public ThingDef thingToSpawn;
        public List<FleckProps> flecks;
    }
}
