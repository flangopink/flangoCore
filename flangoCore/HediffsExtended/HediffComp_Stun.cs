﻿using Verse;

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
    }
}
