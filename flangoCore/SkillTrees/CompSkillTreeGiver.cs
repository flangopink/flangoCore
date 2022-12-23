using RimWorld;
using Verse;

namespace flangoCore
{
    public class CompProperties_SkillTreeGiver : CompProperties
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
            CompProperties_SkillTreeGiver p = (CompProperties_SkillTreeGiver)props;
            tree = p.tree;
        }

        protected override string FloatMenuOptionLabel(Pawn pawn)
        {
            return string.Format(Props.useLabel, tree.label);
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