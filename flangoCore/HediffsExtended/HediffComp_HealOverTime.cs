using Verse;

namespace flangoCore
{
	public class HediffCompProperties_HealOverTime : HediffCompProperties
	{
		public int healIntervalTicks = 60;

		public int healAmount = 5;

		public HediffCompProperties_HealOverTime()
		{
			compClass = typeof(HediffComp_HealOverTime);
		}
	}

	public class HediffComp_HealOverTime : HediffComp
    {
		public int ticksCounter;

		public HediffCompProperties_HealOverTime Props => (HediffCompProperties_HealOverTime)props;

		public override void CompExposeData()
		{
			Scribe_Values.Look(ref ticksCounter, "ticksCounter", 0);
		}

		public override void CompPostTick(ref float severityAdjustment)
		{
			ticksCounter++;
			if (ticksCounter > Props.healIntervalTicks)
			{
				parent.pawn.HitPoints += Props.healAmount;
				ticksCounter = 0;
			}
		}
    }
}
