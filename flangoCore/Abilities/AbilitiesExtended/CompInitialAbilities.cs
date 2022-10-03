using RimWorld;
using Verse;
using System.Collections.Generic;

namespace flangoCore
{
	public class CompProperties_InitialAbilities : CompProperties
	{
		public List<AbilityDef> initialAbilities;

		public CompProperties_InitialAbilities()
		{
			compClass = typeof(CompInitialAbilities);
		}
	}

	public class CompInitialAbilities : ThingComp
	{
		private bool addAbilitiesOnce = true;

		public CompProperties_InitialAbilities Props => (CompProperties_InitialAbilities)props;

		public override void PostExposeData()
		{
			base.PostExposeData();
			Scribe_Values.Look(ref addAbilitiesOnce, "addAbilitiesOnce", true);
		}

        public override void PostPostMake()
        {
            base.PostPostMake();
			if (addAbilitiesOnce)
			{
				Pawn pawn = parent as Pawn;
				pawn.abilities = new Pawn_AbilityTracker(pawn);
				foreach (AbilityDef ability in Props.initialAbilities)
				{
					pawn.abilities.GainAbility(ability);
				}
				addAbilitiesOnce = false;
			}
		}
	}
}
