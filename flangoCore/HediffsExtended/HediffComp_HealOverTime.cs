using System.Collections.Generic;
using Verse;

namespace flangoCore
{
	public class HediffCompProperties_HealOverTime : HediffCompProperties
	{
		public int intervalTicks = 60;
		public int healAmount = 5;

		public HediffCompProperties_HealOverTime()
		{
			compClass = typeof(HediffComp_HealOverTime);
		}
	}

	public class HediffComp_HealOverTime : HediffComp
    {
		HediffSet parentHediffs;

		public HediffCompProperties_HealOverTime Props => (HediffCompProperties_HealOverTime)props;

        public override void CompPostPostAdd(DamageInfo? dinfo)
        {
			parentHediffs = parent.pawn.health.hediffSet;
        }

		public override void CompPostTick(ref float severityAdjustment)
        {
            if (parent.pawn.IsHashIntervalTick(Props.intervalTicks))
            {
                List<Hediff_Injury> resultHediffs = new();
                parentHediffs.GetHediffs(ref resultHediffs, (Hediff_Injury x) => x.CanHealNaturally() || x.CanHealFromTending());
                if (resultHediffs.TryRandomElement(out var result))
                {
                    result.Heal(Props.healAmount);
                }
            }
		}
    }
}
