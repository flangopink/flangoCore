using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace flangoCore
{
	public class SkillTreeWithXP
	{
		public SkillTreeDef tree;
		public int unlockedLevels;
		public float xp;
	}

	public class CompProperties_Skills : CompProperties 
	{ 
		public CompProperties_Skills() => compClass = typeof(CompSkills);
	}

    public class CompSkills : ThingComp
	{
		private List<SkillDef> learnedSkills = new List<SkillDef>();
		public List<SkillDef> LearnedSkills => learnedSkills;

		//private List<SkillTreeWithXP> trees = new List<SkillTreeWithXP>();
		//public List<SkillTreeWithXP> Trees => trees;

		private Pawn Pawn => (Pawn)parent;

		public override void PostSpawnSetup(bool respawningAfterLoad)
		{
			base.PostSpawnSetup(respawningAfterLoad);
			/*if (learnedSkills == null)
			{
				learnedSkills = new List<SkillDef>();
			}*/
		}

		public void GiveSkill(SkillDef skillDef)
		{
			Log.Message("Trying to give " + skillDef.LabelCap + " to " + Pawn.LabelCap);

			if (!learnedSkills.Any((SkillDef s) => s == skillDef))
			{
				if (!skillDef.hediffs.NullOrEmpty())
				{
					if (Pawn.health != null)
					{
						foreach (HediffDef h in skillDef.hediffs)
						{
							if (!Pawn.health.hediffSet.HasHediff(h))
							{
								Hediff hediff = HediffMaker.MakeHediff(h, Pawn);
								hediff.Severity = h.initialSeverity;
								Pawn.health.AddHediff(hediff);
							}
						}
					}
					else Log.Error("Pawn's HealthTracker is null.");
				}

				if (!skillDef.abilities.NullOrEmpty())
				{
					if (Pawn.abilities == null) Pawn.abilities = new Pawn_AbilityTracker(Pawn);

					foreach (AbilityDef ab in skillDef.abilities)
					{
						if (!Pawn.abilities.abilities.Any(x => x.def == ab))
						{
							Pawn.abilities.GainAbility(ab);
						}
					}
				}
				learnedSkills.Add(skillDef);
			}
		}

		public bool HasSkill(SkillDef skillDef)
		{
			foreach (SkillDef learnedSkill in learnedSkills)
			{
				if (learnedSkill == skillDef)
				{
					return true;
				}
			}
			return false;
		}

		public override string CompInspectStringExtra()
		{
			return string.Empty;
		}

		public override void PostExposeData()
		{
			base.PostExposeData();
			Scribe_Collections.Look(ref learnedSkills, "learnedSkills", LookMode.Deep);
			//Scribe_Collections.Look(ref trees, "trees", LookMode.Deep);

			/*if (learnedSkills == null)
			{
				learnedSkills = new List<SkillDef>();
			}*/
		}
	}
}
