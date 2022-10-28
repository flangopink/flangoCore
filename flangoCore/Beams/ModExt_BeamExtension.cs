using Verse;

namespace flangoCore
{
    public class ModExt_BeamExtension : DefModExtension
    {
        public int beamCount = 1;
        public int pathSteps = 5;
        public int ticksToNextPathStep = 5;
        public float missRadius = 1.9f;
    }
}
