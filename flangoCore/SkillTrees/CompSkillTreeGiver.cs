using RimWorld;
using Verse;

namespace flangoCore
{
    public class CompProperties_SkillTreeGiver : CompProperties_Usable
    {
        public SkillTreeDef tree;

        public CompProperties_SkillTreeGiver()
        {
            compClass = typeof(CompSkillTreeGiver);
        }
    }

    public class CompSkillTreeGiver : CompUsable
    {
        public SkillTreeDef tree;

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Defs.Look(ref tree, "tree");
        }

        public override void Initialize(CompProperties props)
        {
            base.Initialize(props);
            if (parent.TryGetComp<CompUseEffect_TreeGiver>() == null)
            {
                parent.def.comps.Add(new CompProperties_UseEffect()
                {
                    compClass = typeof(CompUseEffect_TreeGiver)
                });
            }
            CompProperties_SkillTreeGiver p = (CompProperties_SkillTreeGiver)props;
            tree = p.tree;
        }

        protected override string FloatMenuOptionLabel(Pawn pawn)
        {
            return Props.useLabel + ": " + tree.label;
        }

        public override bool AllowStackWith(Thing other)
        {
            if (!base.AllowStackWith(other)) return false;
            CompSkillTreeGiver comp = other.TryGetComp<CompSkillTreeGiver>();
            return !(comp == null || comp.tree != tree);
        }

        public override void PostSplitOff(Thing piece)
        {
            base.PostSplitOff(piece);
            CompSkillTreeGiver comp = piece.TryGetComp<CompSkillTreeGiver>();
            if (comp != null) comp.tree = tree;
        }
    }
}