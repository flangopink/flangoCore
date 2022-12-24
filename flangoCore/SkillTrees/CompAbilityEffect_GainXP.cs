using RimWorld;
using Verse;

namespace flangoCore
{
	public class CompProperties_GainXP : CompProperties_EffectWithDest
	{
        public SkillTreeDef tree;
        public float xpAmount;
        public bool ignoreMultiplier;
        public bool giveToAllTrees;

        public CompProperties_GainXP()
		{
			compClass = typeof(CompAbilityEffect_GainXP);
		}
	}

	public class CompAbilityEffect_GainXP : CompAbilityEffect
    {
		public CompProperties_GainXP CompProp => (CompProperties_GainXP)props;

		public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
		{
			CompSkills skills = parent.pawn.TryGetComp<CompSkills>();
            if (CompProp.giveToAllTrees) skills.GiveXPToAllTrees(CompProp.xpAmount, CompProp.ignoreMultiplier);
            else skills.GiveXPToTree(CompProp.xpAmount, CompProp.tree, CompProp.ignoreMultiplier);
		}
	}
}
