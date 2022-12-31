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
                if (comps.HasComp<CompProperties_FleckEmitterItem>() || comps.HasComp<CompProperties_FleckEmitterFlickable>() 
                    || comps.HasComp<CompProperties_AnimatedWeapon>())
                {
                    def.tickerType = TickerType.Normal;
                    Log.Message("Added Normal ticker type to " + def);
                }
            }
        }
    }
}
