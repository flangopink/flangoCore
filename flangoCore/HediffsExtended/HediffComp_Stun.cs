using Verse;

namespace flangoCore
{
	public class HediffCompProperties_Stun : HediffCompProperties
	{
		public float stunDurationSeconds = 4f;
		public bool showStunMote;

		public HediffCompProperties_Stun()
		{
			compClass = typeof(HediffComp_Stun);
		}
	}

	public class HediffComp_Stun : HediffComp
    {
		public HediffCompProperties_Stun Props => (HediffCompProperties_Stun)props;

		public override void CompPostPostAdd(DamageInfo? dinfo)
		{
			if (parent.pawn != null && !parent.pawn.Dead)
			{
				parent.pawn.stances.stunner.StunFor(Props.stunDurationSeconds.SecondsToTicks(), parent.pawn, false, Props.showStunMote);
			}
		}

        public override void CompPostTick(ref float severityAdjustment)
        {
            base.CompPostTick(ref severityAdjustment);
			Log.Message("" + (Props.stunDurationSeconds * 60 - parent.ageTicks));
			if (Props.stunDurationSeconds * 60 - parent.ageTicks < 0)
            {
				int ticks;
				if (parent.def.HasComp(typeof(HediffComp_SeverityPerDay)))
				{
					ticks = (int)(parent.Severity / parent.TryGetCompFast<HediffComp_SeverityPerDay>().SeverityChangePerDay() * 60000);
				}
				else if (parent.def.HasComp(typeof(HediffComp_SeverityPerDayExtra)))
				{
					ticks = (int)(parent.Severity / parent.TryGetCompFast<HediffComp_SeverityPerDayExtra>().SeverityChangePerDay() * 60000);
				}
				else return;
				parent.pawn.stances.stunner.StunFor(ticks, parent.pawn, false, Props.showStunMote);
			}
        }
    }
}
