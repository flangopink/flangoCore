using System.Linq;
using RimWorld;
using Verse;

namespace flangoCore
{
    [StaticConstructorOnStartup]
    public static class InitializeSkillsComp
    {
        static InitializeSkillsComp()
        {
            if (DefDatabase<SkillTreeDef>.AllDefsListForReading.NullOrEmpty()) return;

            for (int i = 0; i < DefDatabase<ThingDef>.AllDefsListForReading.Count; i++)
            {
                ThingDef def = DefDatabase<ThingDef>.AllDefsListForReading[i];

                if (def.race?.Humanlike ?? false)
                {
                    if (!def.comps.Any(cp => typeof(CompProperties_Skills).IsAssignableFrom(cp.compClass)))
                    {
                        CompProperties_Skills props = new CompProperties_Skills()
                        {
                            compClass = typeof(CompSkills)
                        };
                        def.comps.Add(props);
                        props.ResolveReferences(def);
                        props.PostLoadSpecial(def);
                    }
                }
            }
        }
    }
}
