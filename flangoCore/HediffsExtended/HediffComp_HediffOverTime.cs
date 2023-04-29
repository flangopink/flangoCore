using Verse;

namespace flangoCore
{
	public class HediffCompProperties_HediffOverTime : HediffCompProperties
	{
		public int intervalTicks = 60;
		public float hediffSeverity = 1f;
		public HediffDef hediffDef;
		public bool removeOnTend;
        public FleckProps fleck;

        public HediffCompProperties_HediffOverTime()
		{
			compClass = typeof(HediffComp_HediffOverTime);
		}
	}

	public class HediffComp_HediffOverTime : HediffComp
    {
		public HediffCompProperties_HediffOverTime Props => (HediffCompProperties_HediffOverTime)props;

		public override void CompPostTick(ref float severityAdjustment)
        {
            if (parent.pawn.IsHashIntervalTick(Props.intervalTicks))
            {
				Hediff hediff = HediffMaker.MakeHediff(Props.hediffDef, parent.pawn);
				hediff.Severity = Props.hediffSeverity;
				parent.pawn.health.AddHediff(hediff);
                Props.fleck?.MakeFleck(parent.pawn.Map, parent.pawn.DrawPos);
            }
		}

        public override void CompTended(float quality, float maxQuality, int batchPosition = 0)
        {
			if (Props.removeOnTend) parent.pawn.health.RemoveHediff(parent);
        }
    }
}
