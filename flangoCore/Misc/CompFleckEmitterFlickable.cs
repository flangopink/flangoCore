using Verse;
using RimWorld;
using System.Collections.Generic;

namespace flangoCore
{
    public class CompProperties_FleckEmitterFlickable : CompProperties_Flickable
    {
        public FleckProps fleck;
        public bool toggleable;

        public CompProperties_FleckEmitterFlickable()
        {
            compClass = typeof(CompFleckEmitterFlickable);
        }
    }
    public class CompFleckEmitterFlickable : CompFlickable
    {
        public CompProperties_FleckEmitterFlickable Props => (CompProperties_FleckEmitterFlickable)props;

        public override void CompTick()
        {
            base.CompTick();
            if (Props.fleck != null && SwitchIsOn)
            {
                if (parent.Map != null && Find.TickManager.TicksGame % Props.fleck.intervalTicks == 0)
                {
                    Props.fleck.MakeFleck(parent.Map, parent.DrawPos);
                }
            }
        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            if (Props.toggleable)
            {
                foreach (Gizmo item in base.CompGetGizmosExtra())
                {
                    yield return item;
                }
            }
        }
    }
}
