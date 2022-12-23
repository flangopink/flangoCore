using KTrie;
using RimWorld;
using Verse;

namespace flangoCore
{
    public class CompProperties_UseEffectXPGiver : CompProperties
    {
        public float xpAmount;
        public bool ignoreMultiplier;
        public bool giveToAllTrees;

        public CompProperties_UseEffectXPGiver()
        {
            compClass = typeof(CompUseEffect_XPGiver);
        }
    }

    public class CompUseEffect_XPGiver : CompUseEffect
    {
        public CompProperties_UseEffectXPGiver Props => (CompProperties_UseEffectXPGiver)props;
        private SkillTreeDef Tree => parent.GetComp<CompSkillTreeXPGiver>().tree;

        public override void DoEffect(Pawn user)
        {
            base.DoEffect(user);
            CompSkills skills = user.TryGetComp<CompSkills>();
            float amount = Props.xpAmount;
            bool ignoreMult = Props.ignoreMultiplier;

            if (Props.giveToAllTrees)
            {
                skills.GiveXPToAllTrees(amount, ignoreMult);
                if (PawnUtility.ShouldSendNotificationAbout(user))
                {
                    float xp = skills.CalculateXP(amount, ignoreMult);
                    Messages.Message("fc_SkillTreeXPGiverUsed_All".Translate(user.LabelShort, xp, user.Named("USER")), user, MessageTypeDefOf.PositiveEvent);
                }
            }
            else
            {
                float xp1 = skills.GetTreeXP(Tree); // Before use

                skills.GiveXPToTree(amount, Tree, ignoreMult);

                float xp2 = skills.GetTreeXP(Tree); // After use

                if (PawnUtility.ShouldSendNotificationAbout(user))
                {
                    Messages.Message("fc_SkillTreeXPGiverUsed_Tree".Translate(user.LabelShort, Tree.LabelCap, xp1, xp2, user.Named("USER")), user, MessageTypeDefOf.PositiveEvent);
                }
            }
        }

        public override bool CanBeUsedBy(Pawn p, out string failReason)
        {
            CompSkills skills = p.TryGetComp<CompSkills>();

            if (skills == null)
            {
                failReason = "fc_CannotLearnSkills".Translate();
                return false;
            }
            if (!skills.AnyTreesUnlocked())
            {
                failReason = "fc_NoSkillTreesLearned".Translate();
                return false;
            }
            if (!skills.HasTree(Tree))
            {
                failReason = "fc_SkillTreeNotLearned".Translate();
                return false;
            }
            return base.CanBeUsedBy(p, out failReason);
        }
    }
}