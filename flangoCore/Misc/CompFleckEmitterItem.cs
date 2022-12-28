using Verse;

namespace flangoCore
{
    public class CompProperties_FleckEmitterItem : CompProperties
    {
        public FleckProps fleck;
        public bool emitWhenDropped = true;
        public bool emitWhenEquipped = true;

        public CompProperties_FleckEmitterItem()
        {
            compClass = typeof(CompFleckEmitterItem);
        }
    }
    
    public class CompFleckEmitterItem : ThingComp
    {
        public CompProperties_FleckEmitterItem Props => (CompProperties_FleckEmitterItem)props;

        public override void CompTick()
        {
            base.CompTick();
            if (Props.fleck != null)
            {
                if (parent.Map != null && Find.TickManager.TicksGame % Props.fleck.intervalTicks == 0)
                {
                    if (Props.emitWhenEquipped && parent.ParentHolder is Pawn pawn && pawn.equipment?.Primary == parent)
                    {
                        Props.fleck.MakeFleck(pawn.Map, pawn.DrawPos);
                    }
                    else if (Props.emitWhenDropped)
                    {
                        Props.fleck.MakeFleck(parent.Map, parent.DrawPos); 
                    }
                }
            }
        }
    }
}
