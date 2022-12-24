using RimWorld;
using System.Linq;
using Verse;

namespace flangoCore
{
    public class CompProperties_SkillTreeXPGiver : CompProperties_Usable
    {
        public SkillTreeDef tree;
        public float xpAmount;
        public bool ignoreMultiplier;
        public bool giveToAllTrees;

        public CompProperties_SkillTreeXPGiver()
        {
            compClass = typeof(CompSkillTreeXPGiver);
        }
    }

    public class CompSkillTreeXPGiver : CompUsable
    {
        public SkillTreeDef tree;
        public float xpAmount;
        public bool ignoreMultiplier;
        public bool giveToAllTrees;

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Defs.Look(ref tree, "tree");
            Scribe_Values.Look(ref xpAmount, "xpAmount");
            Scribe_Values.Look(ref ignoreMultiplier, "ignoreMultiplier");
            Scribe_Values.Look(ref giveToAllTrees, "giveToAllTrees");
        }

        public override void Initialize(CompProperties props)
        {
            base.Initialize(props);
            CompProperties_SkillTreeXPGiver p = (CompProperties_SkillTreeXPGiver)props;
            tree = p.tree;
            xpAmount = p.xpAmount;
            ignoreMultiplier = p.ignoreMultiplier;
            giveToAllTrees = p.giveToAllTrees;
            Log.Message("Init: " + xpAmount);
            Log.Message("Init Comp Count: " + parent.def.comps.Count);
        }

        protected override string FloatMenuOptionLabel(Pawn pawn)
        {
            return Props.useLabel + ": " + tree.label;
        }

        public override bool AllowStackWith(Thing other)
        {
            if (!base.AllowStackWith(other)) return false;
            CompSkillTreeXPGiver comp = other.TryGetComp<CompSkillTreeXPGiver>();
            Log.Message("ASW: " + comp.xpAmount + " - " + xpAmount);
            return !(comp == null || comp.tree != tree || comp.xpAmount != xpAmount || comp.ignoreMultiplier != ignoreMultiplier || comp.giveToAllTrees != giveToAllTrees);
        }

        public override void PostSplitOff(Thing piece)
        {
            base.PostSplitOff(piece);
            CompSkillTreeXPGiver comp = piece.TryGetComp<CompSkillTreeXPGiver>();
            if (comp != null)
            {
                comp.tree = tree;
                comp.xpAmount = xpAmount;
                comp.ignoreMultiplier = ignoreMultiplier;
                comp.giveToAllTrees = giveToAllTrees;
                Log.Message("PSO: " + comp.xpAmount + " - " + xpAmount);
            }
        }
    }
}