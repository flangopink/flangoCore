/*using System.Collections.Generic;
using RimWorld;
using Verse;

namespace flangoCore
{
    public class HediffCompProperties_SkillMenuGiver : HediffCompProperties
    {
        public SkillTreeDef tree;

        public HediffCompProperties_SkillMenuGiver()
        {
            compClass = typeof(HediffComp_SkillMenuGiver);
        }
    }

    public class HediffComp_SkillMenuGiver : HediffComp
    {
        public HediffCompProperties_SkillMenuGiver Props => (HediffCompProperties_SkillMenuGiver)props;

        public override void CompPostPostAdd(DamageInfo? dinfo)
        {
            base.CompPostPostAdd(dinfo);
            if (parent.pawn.Dead || Props.tree == null || parent.pawn.def.HasComp(typeof(HediffComp_SkillMenuGiver))) return;

            parent.pawn.def.comps.Add(Props.thingComp.props);
        }

        public override void CompPostTick(ref float severityAdjustment)
        {
            base.CompPostTick(ref severityAdjustment);
            if (parent.pawn.Dead) parent.pawn.health.RemoveHediff(parent);

            if (!Props.onTickFlecks.NullOrEmpty())
            {
                foreach (FleckProps fleck in Props.onTickFlecks)
                {
                    if (parent.pawn.Map != null && Find.TickManager.TicksGame % fleck.intervalTicks == 0)
                    {
                        fleck.MakeFleck(parent.pawn.Map, parent.pawn.DrawPos);
                    }
                }
            }
        }

        public override void CompPostPostRemoved()
        {
            base.CompPostPostRemoved();
            if (parent.pawn.Dead) return;

            if (!Props.onRemoveFlecks.NullOrEmpty())
            {
                foreach (FleckProps fleck in Props.onRemoveFlecks)
                {
                    if (parent.pawn.Map != null)
                    {
                        fleck.MakeFleck(parent.pawn.Map, parent.pawn.DrawPos);
                    }
                }
            }
        }
    }
}
*/