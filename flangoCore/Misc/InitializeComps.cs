using Verse;

namespace flangoCore
{
    [StaticConstructorOnStartup]
    public static class InitializeComps
    {
        static InitializeComps()
        {
            for (int i = 0; i < DefDatabase<ThingDef>.AllDefsListForReading.Count; i++)
            {
                ThingDef def = DefDatabase<ThingDef>.AllDefsListForReading[i];
                var comps = def.comps;

                // Add comp checks here
                if (comps.HasComp<CompFleckEmitterItem>() || comps.HasComp<CompFleckEmitterFlickable>())
                {
                    def.tickerType = TickerType.Normal;
                }
            }
        }
    }
}
