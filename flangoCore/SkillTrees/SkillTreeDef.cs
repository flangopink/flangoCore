 using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using UnityEngine;

namespace flangoCore
{
	public class SkillTreeDef : Def
	{
		public static SkillDef BlankSkill;

		public static int TotalPoints;

		public bool unlockWholeLevels;

		public XPSourceFlags xpSources;

		public string tab;

		public string tooltip;

		public string iconPath;

		//public List<string> columnIconPaths;

		public int order;

		[Unsaved(false)]
		public int MaxLevel;

		[Unsaved(false)]
		public Texture2D icon;

		//[Unsaved(false)]
		//public List<Texture2D> columnIcons;

		[Unsaved(false)]
		public bool HasSkills;

		public List<SkillTreeLevel> levels;
		public List<SkillDef> skillDefs;

		//public List<string> columnNames;
		public List<string> rowNames;

		[Unsaved(false)]
		public SkillDef[][] skillLevelsInOrder;
		public SkillDef[] arrayWithMostSkills;

		public float textWidth = 100f;
		public float xOffsetMultiplier = 1f;

		public override void PostLoad()
		{
			base.PostLoad();
			LongEventHandler.ExecuteWhenFinished(delegate
			{
				if (!iconPath.NullOrEmpty())
				{
					icon = ContentFinder<Texture2D>.Get(iconPath);
				}

				foreach (SkillTreeLevel level in levels)
                {
					level.icon = ContentFinder<Texture2D>.Get(level.iconPath);
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

			int stilLevel = 0;
			skillDefs = new List<SkillDef>();
			foreach (SkillTreeLevel stl in levels)
            {
				stilLevel++;
                foreach (SkillDef skill in stl.skills)
                {
					skill.reqLevel = stilLevel;
					skillDefs.Add(skill);
				}
            }

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
			arrayWithMostSkills = skillLevelsInOrder.Aggregate((x, y) => x.Length > y.Length ? x : y);
		}

		public override IEnumerable<string> ConfigErrors()
		{
			foreach (string item in base.ConfigErrors())
			{
				yield return item;
			}
			if (MaxLevel < 2)
			{
				yield return $"Skill tree named {LabelCap} must have at least 2 levels defined. Current max level: {MaxLevel}";
			}
			if (arrayWithMostSkills.Length > 4)
			{
				yield return $"Skill tree named {LabelCap} must not have more than 4 rows defined. Current max skill count: {arrayWithMostSkills.Length}";
			}
		}
	}
}
