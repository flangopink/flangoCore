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
				parent.pawn.TakeDamage(new DamageInfo(Props.damageDef, Props.damageAmount, Props.armorPenetration));
				ticksCounter = 0;
			}
		}

        public override void CompTended(float quality, float maxQuality, int batchPosition = 0)
        {
			if (Props.removeOnTend) parent.pawn.health.RemoveHediff(parent);
        }
    }
}
