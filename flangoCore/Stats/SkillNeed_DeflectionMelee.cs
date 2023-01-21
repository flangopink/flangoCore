using RimWorld;
using Verse;

namespace flangoCore
{
    public class SkillNeed_DeflectionMelee : SkillNeed_BaseBonus
    {
        public float baseValue = 0f;

        private float BonusPerLevel => FlangoCore.settings.DeflectionPerSkill;

        public override float ValueFor(Pawn pawn)
        {
            if (pawn.skills == null) return 0f;
            bool flag = FlangoCore.settings.deflectionChanceAffectedByMeleeSkill;
            return baseValue + (flag ? BonusPerLevel * pawn.skills.GetSkill(skill).Level : 0f);
        }
    }
}
