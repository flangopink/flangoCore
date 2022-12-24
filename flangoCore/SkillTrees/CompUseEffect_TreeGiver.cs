using RimWorld;
using Verse;

namespace flangoCore
{
    public class CompUseEffect_TreeGiver : CompUseEffect
    {
        private CompSkillTreeGiver Comp => parent.GetComp<CompSkillTreeGiver>();
        private SkillTreeDef Tree => Comp.tree;

        public override void DoEffect(Pawn user)
        {
            base.DoEffect(user);
            CompSkills skills = user.TryGetComp<CompSkills>();
            skills.GiveTree(Tree);
            if (PawnUtility.ShouldSendNotificationAbout(user))
            {
                Messages.Message("fc_SkillTreeGiverUsed".Translate(user.LabelShort, Tree.label, user.Named("USER")), user, MessageTypeDefOf.PositiveEvent);
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
            if (skills.HasTree(Tree))
            {
                failReason = "fc_SkillTreeAlreadyLearned".Translate();
                return false;
            }
            return base.CanBeUsedBy(p, out failReason);
        }
    }
}