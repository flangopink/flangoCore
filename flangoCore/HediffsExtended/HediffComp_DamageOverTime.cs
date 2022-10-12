using Verse;

namespace flangoCore
{
	public class HediffCompProperties_DamageOverTime : HediffCompProperties
	{
		public int damageIntervalTicks = 60;

		public float damageAmount = 1f;

		public float armorPenetration = 0f;

		public DamageDef damageDef;

		public bool removeOnTend;

		public bool stun;
		public float stunDurationSeconds = 5f;
		public bool showStunMote;

		public FleckProps fleck;

		public HediffCompProperties_DamageOverTime()
		{
			compClass = typeof(HediffComp_DamageOverTime);
		}
	}

	public class HediffComp_DamageOverTime : HediffComp
    {
		public int ticksCounter;

		public HediffCompProperties_DamageOverTime Props => (HediffCompProperties_DamageOverTime)props;

		public override void CompExposeData()
		{
			Scribe_Values.Look(ref ticksCounter, "ticksCounter", 0);
		}

		public override void CompPostTick(ref float severityAdjustment)
		{
			ticksCounter++;
			if (ticksCounter > Props.damageIntervalTicks)
			{
				if (parent.pawn.Dead) parent.pawn.health.RemoveHediff(parent);

				parent.pawn.TakeDamage(new DamageInfo(Props.damageDef, Props.damageAmount, Props.armorPenetration));

				if (Props.fleck != null) Props.fleck.MakeFleck(parent.pawn.Map, parent.pawn.DrawPos);

				if (Props.stun && parent.pawn != null && !parent.pawn.Dead)
                {
					parent.pawn.stances.stunner.StunFor(Props.stunDurationSeconds.SecondsToTicks(), parent.pawn, false, Props.showStunMote);
				}

				ticksCounter = 0;
			}
		}

        public override void CompTended(float quality, float maxQuality, int batchPosition = 0)
        {
			if (Props.removeOnTend) parent.pawn.health.RemoveHediff(parent);
        }
    }
}
