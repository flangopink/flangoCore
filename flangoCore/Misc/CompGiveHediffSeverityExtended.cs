using RimWorld;
using Verse;
using System.Collections.Generic;

namespace flangoCore
{
	public class CompProperties_GiveHediffSeverityExtended : CompProperties
	{
		public HediffDef hediff;

		public float range = 4.9f;

		public float severityPerSecond = 0.05f;

		public ChemicalDef chemical;

		public int tickInterval = 87;

		public bool drugExposure;
		public bool allowMechs = true;
		public bool requiresMood;

		public StatDef requiredStat;
		public bool scalesWithStat = true;

		public BodyPartTagDef affectedBodyPartTag;

		public CompProperties_GiveHediffSeverityExtended()
		{
			compClass = typeof(CompGiveHediffSeverityExtended);
		}

        public override void ResolveReferences(ThingDef def)
        {
            base.ResolveReferences(def);
			requiredStat = StatDefOf.PsychicSensitivity;
		}
	}

	public class CompGiveHediffSeverityExtended : ThingComp
	{
		private CompProperties_GiveHediffSeverityExtended Props => (CompProperties_GiveHediffSeverityExtended)props;

		private bool AppliesTo(Pawn pawn)
		{
			if (pawn.GetRoom() != parent.GetRoom())
			{
				return false;
			}
			if (!Props.allowMechs && pawn.RaceProps.IsMechanoid)
			{
				return false;
			}
			if (Props.requiredStat != null && pawn.GetStatValue(Props.requiredStat) <= 0)
			{
				return false;
			}
			if (Props.requiresMood || pawn.needs.mood == null)
			{ 
				return false; 
			}
			return true;
		}

		public override void CompTick()
		{
			if (!parent.Spawned || Find.TickManager.TicksGame % Props.tickInterval != 0)
			{
				return;
			}
			CompRefuelable compRefuelable = parent.TryGetComp<CompRefuelable>();
			CompPowerTrader compPowerTrader = parent.TryGetComp<CompPowerTrader>();
			if ((compRefuelable != null && !compRefuelable.HasFuel) || (compPowerTrader != null && !compPowerTrader.PowerOn))
			{
				return;
			}
			int num = GenRadial.NumCellsInRadius(Props.range);
			for (int i = 0; i < num; i++)
			{
				List<Thing> thingList = (parent.Position + GenRadial.RadialPattern[i]).GetThingList(parent.Map);
				for (int j = 0; j < thingList.Count; j++)
				{
					if (!(thingList[j] is Pawn pawn) || !AppliesTo(pawn))
					{
						continue;
					}
					Hediff firstHediffOfDef = pawn.health.hediffSet.GetFirstHediffOfDef(Props.hediff);
					float num2 = Props.severityPerSecond * 1.45f * (Props.scalesWithStat && Props.requiredStat != null ? pawn.GetStatValue(Props.requiredStat) : 1); // Psych Sens scaling
					if (firstHediffOfDef != null)
					{
						firstHediffOfDef.Severity += num2;
					}
					else
					{
						if (Props.affectedBodyPartTag != null)
						{
							pawn.RaceProps.body.GetPartsWithTag(Props.affectedBodyPartTag).TryRandomElement(out BodyPartRecord part);
							pawn.health.AddHediff(Props.hediff, part).Severity = num2;
						}
                        else pawn.health.AddHediff(Props.hediff).Severity = num2;
					}

					if (!Props.drugExposure)
					{
						continue;
					}

					pawn.mindState.lastTakeRecreationalDrugTick = Find.TickManager.TicksGame;
					if (pawn.needs?.drugsDesire != null)
					{
						pawn.needs.drugsDesire.CurLevel += 0.145000011f;
					}
					if (Props.chemical != null)
					{
						HediffDef addictionHediffDef = Props.chemical.addictionHediff;
						Need need = pawn.needs.AllNeeds.Find((Need x) => x.def == addictionHediffDef.causesNeed);
						if (need != null)
						{
							need.CurLevel += 0.145000011f;
						}
					}
				}
			}
		}
    }
}
