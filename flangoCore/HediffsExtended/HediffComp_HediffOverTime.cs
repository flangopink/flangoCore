using Verse;

namespace flangoCore
{
	public class HediffCompProperties_HediffOverTime : HediffCompProperties
	{
		public int damageIntervalTicks = 60;

		public float hediffSeverity = 1f;

		public HediffDef hediffDef;

		public bool removeOnTend;

		public HediffCompProperties_HediffOverTime()
		{
			compClass = typeof(HediffComp_HediffOverTime);
		}
	}

	public class HediffComp_HediffOverTime : HediffComp
    {
		public int ticksCounter;

		public HediffCompProperties_HediffOverTime Props => (HediffCompProperties_HediffOverTime)props;

		public override void CompExposeData()
		{
			Scribe_Values.Look(ref ticksCounter, "ticksCounter", 0);
		}

		public override void CompPostTick(ref float severityAdjustment)
		{
			ticksCounter++;
			if (ticksCounter > Props.damageIntervalTicks)
			{
				Hediff hediff = HediffMaker.MakeHediff(Props.hediffDef, parent.pawn);
				hediff.Severity = Props.hediffSeverity;
				parent.pawn.health.AddHediff(hediff);
				ticksCounter = 0;
			}
		}

        public override void CompTended(float quality, float maxQuality, int batchPosition = 0)
        {
			if (Props.removeOnTend) parent.pawn.health.RemoveHediff(parent);
        }
    }
}
