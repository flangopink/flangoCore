using System.Collections.Generic;
using Verse;
using UnityEngine;

namespace flangoCore
{
    public class HediffCompProperties_VFX : HediffCompProperties
    {
        public List<FleckProps> onAddFlecks;
        public List<FleckProps> onTickFlecks;
        public List<FleckProps> onRemoveFlecks;
        public Color color;

        public HediffCompProperties_VFX()
        {
            compClass = typeof(HediffComp_VFX);
        }
    }

    public class HediffComp_VFX : HediffComp
    {
        public HediffCompProperties_VFX Props => (HediffCompProperties_VFX)props;

        public override void CompPostPostAdd(DamageInfo? dinfo)
        {
            if (parent.pawn.Dead || Props.onAddFlecks.NullOrEmpty()) return;

            foreach (FleckProps fleck in Props.onAddFlecks)
            {
                if (parent.pawn.Map != null)
                {
                    fleck.MakeFleck(parent.pawn.Map, parent.pawn.DrawPos);
                }
            }
        }

        public override void CompPostTick(ref float severityAdjustment)
        {
            if (parent.pawn.Dead) parent.pawn.health.RemoveHediff(parent);

            if (Props.onTickFlecks.NullOrEmpty()) return;

            foreach (FleckProps fleck in Props.onTickFlecks)
            {
                if (parent.pawn.Map != null && parent.pawn.IsHashIntervalTick(fleck.intervalTicks))
                {
                    fleck.MakeFleck(parent.pawn.Map, parent.pawn.DrawPos);
                }
            }
        }

        public override void CompPostPostRemoved()
        {
            if (Props.onRemoveFlecks.NullOrEmpty() || parent.pawn.Dead) return;

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
