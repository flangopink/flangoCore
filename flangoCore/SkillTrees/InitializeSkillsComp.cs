using System.Linq;
using RimWorld;
using Verse;

namespace flangoCore.SkillTrees
{
    [StaticConstructorOnStartup]
    public static class InitializeAbilityComp
    {
        static InitializeAbilityComp()
        {
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
