using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using UnityEngine;

namespace flangoCore
{
	public class SkillTreeLevel
	{
		public int level = 0;
		public List<SkillDef> skills;
    }

    public class SkillTreeDef : Def
	{
		public static SkillDef BlankSkill;

		public static int TotalPoints;

		public string tab;

		public string tooltip;

		public string iconPath;

		public List<string> columnIconPaths;

		public int order;

		[Unsaved(false)]
		public int MaxLevel;

		[Unsaved(false)]
		public Texture2D icon;

		[Unsaved(false)]
		public List<Texture2D> columnIcons;

		[Unsaved(false)]
		public bool HasSkills;

		public List<SkillTreeLevel> levels;
		public List<SkillDef> skillDefs;

		public List<string> columnNames;
		public List<string> rowNames;

		[Unsaved(false)]
		public SkillDef[][] skillLevelsInOrder;

		public float textWidth = 100f;

		public override void PostLoad()
		{
			base.PostLoad();
			LongEventHandler.ExecuteWhenFinished(delegate
			{
				if (!iconPath.NullOrEmpty())
				{
					icon = ContentFinder<Texture2D>.Get(iconPath);
				}
				if (!columnIconPaths.NullOrEmpty())
                {
					columnIcons = new List<Texture2D>();
					foreach (string icon in columnIconPaths)
                    {
						columnIcons.Add(ContentFinder<Texture2D>.Get(icon));
                    }
                }
			});
		}

		public override void ResolveReferences()
		{
			base.ResolveReferences();

			if (BlankSkill == null) 
			{ 
				BlankSkill = new SkillDef();
			}
			TotalPoints++;

			skillDefs = new List<SkillDef>();
			foreach (SkillTreeLevel stl in levels)
            {
				foreach (SkillDef skill in stl.skills)
                {
					skill.reqLevel = stl.level;
					skillDefs.Add(skill);
				}
            }
			/*
			foreach (SkillDef skill in DefDatabase<SkillDef>.AllDefsListForReading)
			{
				if (skill.tree == this)
				{
					skills.Add(skill);
				}
			}*/

			MaxLevel = skillDefs.Max((SkillDef s) => s.reqLevel);
			TotalPoints += skillDefs.Count;
			skillLevelsInOrder = new SkillDef[MaxLevel][];

			foreach (IGrouping<int, SkillDef> skillGroup in from s in skillDefs group s by s.reqLevel)
			{
				skillLevelsInOrder[skillGroup.Key - 1] = skillGroup.OrderBy((SkillDef s) => s.reqLevel).SelectMany(delegate (SkillDef s)
				{
					IEnumerable<SkillDef> result;
					if (!s.spaceAfter)
					{
						result = Gen.YieldSingle(s);
					}
					else
					{
						IEnumerable<SkillDef> enumerable = new List<SkillDef> { s, BlankSkill };
						result = enumerable;
					}
					return result;
				}).ToArray();
			}
			HasSkills = skillLevelsInOrder.Any((SkillDef[] arr) => !arr.NullOrEmpty());
			if (!HasSkills)
			{
				return;
			}
			for (int i = 0; i < skillLevelsInOrder.Length; i++)
			{
				SkillDef[] array = skillLevelsInOrder[i];
				if (array == null)
				{
					skillLevelsInOrder[i] = new SkillDef[0];
				}
			}

			/*if (branches.NullOrEmpty() || branches.Any(x => x.skills.NullOrEmpty()))
			{
				Log.Error("Skill Tree branches not defined or at least one branch is empty.");
				return;
			}*/
		}
	}
}
