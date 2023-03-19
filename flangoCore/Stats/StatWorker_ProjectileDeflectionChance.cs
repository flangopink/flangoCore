using RimWorld;
using System.Text;
using Verse;

namespace flangoCore
{
    /*public class StatWorker_ProjectileDeflectionChance : StatWorker
    {
        private float DeflectionPerSkill => FlangoCore.settings.deflectionPerSkill * 0.01f;
        private bool ScalesWithMelee => FlangoCore.settings.deflectionChanceAffectedByMeleeSkill;

        public override float GetValueUnfinalized(StatRequest req, bool applyPostProcess = true)
        {
            if (req.HasThing && req.Thing is Pawn pawn)
            {
                float melee = ScalesWithMelee && pawn.skills != null ? pawn.skills.GetSkill(SkillDefOf.Melee).Level * DeflectionPerSkill : 0f; // x1.5
                return base.GetValueUnfinalized(req) + melee;
            }
            return base.GetValueUnfinalized(req);
        }

        public override string GetExplanationUnfinalized(StatRequest req, ToStringNumberSense numberSense)
        {
            StringBuilder stringBuilder = new();
            stringBuilder.Append(base.GetExplanationUnfinalized(req, numberSense));
            if (ScalesWithMelee && req.Pawn.skills != null)
            {
                int lvl = req.Pawn.skills.GetSkill(SkillDefOf.Melee).Level;
                stringBuilder.AppendLine(SkillDefOf.Melee.LabelCap + " " + lvl + ": +" + (lvl * DeflectionPerSkill).ToStringPercent());
            }
            return stringBuilder.ToString();
        }
    }

    [StaticConstructorOnStartup]
    public static class InitBaseDeflectionChance
    {
        static InitBaseDeflectionChance()
        {
            StatDefOf_flangoCore.ProjectileDeflectionChance.defaultBaseValue = FlangoCore.settings.BaseDeflectionChance;
        }
    }*/
}
