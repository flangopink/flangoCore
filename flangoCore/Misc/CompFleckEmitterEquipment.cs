using Verse;

namespace flangoCore
{
    public class CompProperties_FleckEmitterEquipment : CompProperties
    {
        public FleckProps fleck;
        public bool emitWhenDropped = true;
        public bool emitWhenEquipped = true;

        public CompProperties_FleckEmitterEquipment()
        {
            compClass = typeof(CompFleckEmitterEquipment);
        }
    }
    
    public class CompFleckEmitterEquipment : ThingComp
    {
        public CompProperties_FleckEmitterEquipment Props => (CompProperties_FleckEmitterEquipment)props;

        public override void CompTick()
        {
            base.CompTick();
            Log.Message(Props.fleck.ToString() + ", " + parent.Map);
            if (Props.fleck != null)
            {
                if (parent.Map != null && Find.TickManager.TicksGame % Props.fleck.intervalTicks == 0)
                {
                    if (Props.emitWhenEquipped && parent.ParentHolder is Pawn pawn && pawn.equipment?.Primary == parent)
                    {
                        Log.Message("1");
                        Props.fleck.MakeFleck(pawn.Map, pawn.DrawPos);
                    }
                    else if (Props.emitWhenDropped)
                    {
                        Log.Message("2");
                        Props.fleck.MakeFleck(parent.Map, parent.DrawPos); 
                    }
                    Log.Message("3");
                }
            }
        }
    }
}
