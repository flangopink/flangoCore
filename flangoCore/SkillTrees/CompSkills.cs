using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;

namespace flangoCore
{
	/*public class SkillTreeWithXP
	{
		public SkillTreeDef tree;
		public int unlockedLevels;
		public float xp;
		public float xpMultiplier = 1;
	}*/

	public class CompProperties_Skills : CompProperties 
	{ 
		public CompProperties_Skills() => compClass = typeof(CompSkills);
	}

	//[HotSwap.HotSwappable]
    public class CompSkills : ThingComp
	{
		private List<SkillDef> learnedSkills = new();
		public List<SkillDef> LearnedSkills => learnedSkills;

		public int canUnlockLevel = 1;
		public float xpMultiplier = 1;

		public Dictionary<SkillTreeDef, float> treeXPs = new();
		public SkillTreeDef selectedTree;

		private Pawn Pawn => (Pawn)parent;

		public override void PostSpawnSetup(bool respawningAfterLoad)
		{
			base.PostSpawnSetup(respawningAfterLoad);

			
			//selectedTree = treeXPs.FirstOrDefault().Key;

			Log.Message(string.Join(", ", treeXPs));
			//Log.Message(selectedTree.ToString());
		}

		/*public override void CompTick()
		{
			if (KeyBindingDefOf.Misc12.JustPressed)
            {
				//temp
                foreach (SkillTreeDef tree in DefDatabase<SkillTreeDef>.AllDefs)
                {
                    GiveTree(tree); 
					Log.Message(string.Join(", ", treeXPs));
                    break;
                }
            }
		}*/

        public float GetXP()
        {
			return treeXPs[selectedTree];
        }

        public float GetTreeXP(SkillTreeDef tree)
        {
            return treeXPs[tree];
        }

		public float CalculateXP(float xp, bool ignoreMultiplier)
		{
			return xp * (ignoreMultiplier ? 1 : xpMultiplier);
        }

        public void GiveXPToTree(float xp, SkillTreeDef tree, bool ignoreMultiplier = false)
        {
			if (treeXPs.ContainsKey(tree))
			{
				treeXPs[tree] += CalculateXP(xp, ignoreMultiplier);
			}
        }

		public void TryGiveXPToTree(float xp, SkillTreeDef tree, bool ignoreMultiplier = false, bool warningOnfail = false)
		{
			if (!treeXPs.ContainsKey(tree))
			{
				if (warningOnfail) Log.Warning("Could not give xp to " + Pawn.Name + ". Skill tree not learned: " + tree.label);
				return;
			}
			else GiveXPToTree(xp, tree, ignoreMultiplier);
        }
		
		public void GiveXPToCurrentTree(float xp, bool ignoreMultiplier = false)
		{
			treeXPs[selectedTree] += CalculateXP(xp, ignoreMultiplier);
		}

		public void GiveXPToAllTrees(float xp, bool ignoreMultiplier = false)
		{
			foreach (SkillTreeDef tree in treeXPs.Keys)
			{
				treeXPs[tree] += CalculateXP(xp, ignoreMultiplier);
			}
		}

		public void GiveXPFlagged(float xp, XPSourceFlags flags, bool ignoreMultiplier = false)
		{
			foreach (SkillTreeDef tree in treeXPs.Keys)
			{
				if (tree.xpSources.HasFlag(flags))
				{
					treeXPs[tree] += CalculateXP(xp, ignoreMultiplier);
				}
			}
		}

		public void GiveTree(SkillTreeDef tree)
        {
			if (!treeXPs.ContainsKey(tree))
            {
				treeXPs.Add(tree, 0);
				selectedTree ??= tree;
				((ITab_Pawn_Skills)parent.def.inspectorTabsResolved.FirstOrDefault(x => x is ITab_Pawn_Skills)).UpdateTreeTabs(treeXPs.Keys.ToList());
            }
        }

		public void GiveSkill(SkillDef skillDef)
		{
			//Log.Message("Trying to give " + skillDef.LabelCap + " to " + Pawn.LabelCap);

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

				treeXPs[selectedTree] -= skillDef.cost;
				//if (treeXPs.Keys.First(x => x.levels.First(y => y.skills.Contains(skillDef)).level > currentLevel))
				foreach (SkillTreeDef tree in treeXPs.Keys)
				{
					int skillLevel = 1 + tree.levels.IndexOf(tree.levels.First(x => x.skills.Contains(skillDef)));
					if (skillLevel == canUnlockLevel) canUnlockLevel++;
				}
			}
		}

		public bool HasSkill(SkillDef skillDef)
		{
			return learnedSkills.Contains(skillDef);
        }

        public bool AnyTreesUnlocked()
        {
            return treeXPs.Count > 0;
        }

        public bool HasTree(SkillTreeDef treeDef)
		{
			return treeXPs.ContainsKey(treeDef);
		}

		public override string CompInspectStringExtra()
		{
			return string.Empty;
		}

		public override void PostExposeData()
		{
			base.PostExposeData();
			//Scribe_Values.Look(ref currentXP, "currentXP", 0);
			Scribe_Values.Look(ref xpMultiplier, "xpMultiplier", 1);
			Scribe_Defs.Look(ref selectedTree, "selectedTree");
			Scribe_Collections.Look(ref learnedSkills, "learnedSkills", LookMode.Deep);
			Scribe_Collections.Look(ref treeXPs, "treeXPs", LookMode.Def);

			/*if (learnedSkills == null)
			{
				learnedSkills = new List<SkillDef>();
			}*/
		}
	}
}