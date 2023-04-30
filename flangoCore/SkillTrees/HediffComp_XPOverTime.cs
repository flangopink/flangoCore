using Verse;

namespace flangoCore
{
    public class HediffCompProperties_XPOverTime : HediffCompProperties
    {
        public SkillTreeDef tree;
        public float xpAmount;
        public bool ignoreMultiplier;
        public bool giveToAllTrees;
        public int intervalTicks;

        public HediffCompProperties_XPOverTime()
        {
            compClass = typeof(HediffComp_XPOverTime);
        }
    }

    public class HediffComp_XPOverTime : HediffComp
    {
        public HediffCompProperties_XPOverTime Props => (HediffCompProperties_XPOverTime)props;

        private CompSkills skills;

        public override void CompPostPostAdd(DamageInfo? dinfo)
        {
            if (Pawn != null && !Pawn.Dead)
            {
                skills = Pawn.TryGetComp<CompSkills>();
            }
            if (skills == null)
            { 
                Pawn.health.RemoveHediff(parent);
                Log.Warning("Removing " + Def.defName + " from " + Pawn.Name + ": Tried to add \"XP over time\" hediff to a pawn with no CompSkills");
            }
        }

        public override void CompPostTick(ref float severityAdjustment)
        {
            if (Pawn.Dead) Pawn.health.RemoveHediff(parent);
            base.CompPostTick(ref severityAdjustment);
            if (parent.pawn.IsHashIntervalTick(Props.intervalTicks))
            {
                if (Props.giveToAllTrees) skills.GiveXPToAllTrees(Props.xpAmount, Props.ignoreMultiplier);
                else skills.TryGiveXPToTree(Props.xpAmount, Props.tree, Props.ignoreMultiplier);
            }
        }
    }
}
