using RimWorld;
using Verse;

namespace flangoCore
{
    public class CompUseEffect_XPGiver : CompUseEffect
    {
        private CompSkillTreeXPGiver Comp => parent.GetComp<CompSkillTreeXPGiver>();
        private SkillTreeDef Tree => Comp.tree;

        public override void DoEffect(Pawn user)
        {
            base.DoEffect(user);
            CompSkills skills = user.TryGetComp<CompSkills>();
            float amount = Comp.xpAmount;
            bool ignoreMult = Comp.ignoreMultiplier;
            Log.Message("UseEffect: " + amount);

            if (Comp.giveToAllTrees)
            {
                skills.GiveXPToAllTrees(amount, ignoreMult);
                if (PawnUtility.ShouldSendNotificationAbout(user))
                {
                    float xp = skills.CalculateXP(amount, ignoreMult);
                    Messages.Message("fc_SkillTreeXPGiverUsed_All".Translate(user.Named("USER"), xp), user, MessageTypeDefOf.PositiveEvent);
                }
            }
            else
            {
                float xp1 = skills.GetTreeXP(Tree); // Before use

                skills.GiveXPToTree(amount, Tree, ignoreMult);

                float xp2 = skills.GetTreeXP(Tree); // After use

                if (PawnUtility.ShouldSendNotificationAbout(user))
                {
                    Messages.Message("fc_SkillTreeXPGiverUsed_Tree".Translate(user.Named("USER"), Tree.LabelCap, xp1, xp2), user, MessageTypeDefOf.PositiveEvent);
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