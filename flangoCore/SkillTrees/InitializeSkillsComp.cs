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
                var comps = def.comps;

                if (comps.HasComp<CompProperties_SkillTreeGiver>())
                {
                    CompProperties_UseEffect props = new(){ compClass = typeof(CompUseEffect_TreeGiver) };
                    def.AddAndResolve(props);
                }

                if (comps.HasComp<CompProperties_SkillTreeXPGiver>())
                {
                    CompProperties_UseEffect props = new(){ compClass = typeof(CompUseEffect_XPGiver) };
                    def.AddAndResolve(props);
                }

                if (def.race?.Humanlike ?? false)
                {
                    if (!def.comps.Any(cp => typeof(CompProperties_Skills).IsAssignableFrom(cp.compClass)))
                    {
                        CompProperties_Skills props = new() { compClass = typeof(CompSkills) };
                        def.AddAndResolve(props);
                    }
                }
            }
        }

        private static void AddAndResolve(this ThingDef def, CompProperties props)
        {
            def.comps.Add(props);
            props.ResolveReferences(def);
            props.PostLoadSpecial(def);
        }
    }
}
