using System.Collections.Generic;
using System.Linq;
using System;
using RimWorld;
using Verse;
using Verse.Sound;
using UnityEngine;

namespace flangoCore
{
	[StaticConstructorOnStartup]
	public class ITab_Pawn_Skills : ITab
	{
		private readonly Settings settings;
		private Pawn pawn;

		private bool devMode;

		private readonly Dictionary<string, List<SkillTreeDef>> treesByTab; 
		private readonly List<TabRecord> tabs;
		private string curTab;

		private float lastPathsHeight;
		private int pathsPerRow;
		private Vector2 pathsScrollPos;

		private static readonly float[][] skillTreeYOffsets;
		public static CompSkills CompSkills;

		private readonly Dictionary<SkillDef, Vector2> skillPos = new Dictionary<SkillDef, Vector2>();

		private CompSkills compSkills;

		public Vector2 Size => size;

		static ITab_Pawn_Skills()
		{
			skillTreeYOffsets = new float[4][]
			{
				new float[1] { 10f },
				new float[2] { -50f, 10f },
				new float[3] { -50f, 10f, 70f },
				new float[4] { -50f, 10f, 70f, 130f }
			};

			foreach (ThingDef allDef in DefDatabase<ThingDef>.AllDefs)
			{
				RaceProperties race = allDef.race;
				if (race != null && race.Humanlike)
				{
					allDef.inspectorTabs?.Add(typeof(ITab_Pawn_Skills));
					allDef.inspectorTabsResolved?.Add(InspectTabManager.GetSharedInstance(typeof(ITab_Pawn_Skills)));
				}
			}
		}

		public ITab_Pawn_Skills()
		{
			settings = Controller.settings;
			labelKey = "fc_skills".Translate();
			size = new Vector2(650f * settings.skillTreeUIScale, 450f * settings.skillTreeUIScale);

			treesByTab = (from def in DefDatabase<SkillTreeDef>.AllDefs group def by def.tab).ToDictionary((IGrouping<string, SkillTreeDef> group) => group.Key, (IGrouping<string, SkillTreeDef> group) => group.ToList());

			tabs = treesByTab.Select((KeyValuePair<string, List<SkillTreeDef>> kv) => new TabRecord(kv.Key, delegate
			{
				curTab = kv.Key;
			}, () => curTab == kv.Key)).ToList();
			curTab = treesByTab.Keys.FirstOrDefault();
		}

		protected override void UpdateSize()
		{
			base.UpdateSize();
			pathsPerRow = Mathf.FloorToInt(size.x * 0.67f / 200f);
		}

		public override void OnOpen()
		{
			base.OnOpen();
			pawn = (Pawn)Find.Selector.SingleSelectedThing;
			InitCache();
		}

		private void InitCache()
		{
			CompSkills = compSkills = pawn.GetComp<CompSkills>();
			skillPos.Clear();
		}

		protected override void CloseTab()
		{
			base.CloseTab();
			pawn = null;
			CompSkills = compSkills = null;
			skillPos.Clear();
		}

		public static Rect TakeLeftPart(Rect rect, float pixels)
		{
			Rect result = rect.LeftPartPixels(pixels);
			rect.xMin += pixels;
			return result;
		}

		protected override void FillTab()
		{
			if (Find.Selector.SingleSelectedThing is Pawn pawn && this.pawn != pawn)
			{
				this.pawn = pawn;
				InitCache();
			}
			if (devMode && !Prefs.DevMode)
			{
				devMode = false;
			}

			GameFont font = Text.Font;
			TextAnchor anchor = Text.Anchor;

			Rect rectMain = new Rect(Vector2.one * 20f, size - Vector2.one * 40f);
			Rect rectTree = rectMain.ContractedBy(5f);

			if (treesByTab.NullOrEmpty())
			{
				Text.Anchor = TextAnchor.MiddleCenter;
				Text.Font = GameFont.Medium;
				Widgets.DrawMenuSection(rectTree);
				Widgets.Label(rectTree, "No Paths");
			}
			else
			{
				TabDrawer.DrawTabs(new Rect(rectTree.x, rectTree.y + 40f, rectTree.width, rectTree.height), tabs);
				rectTree.yMin += 40f;
				Widgets.DrawMenuSection(rectTree);
				Rect rect8 = new Rect(0f, 0f, rectTree.width - 20f, lastPathsHeight);
				Widgets.BeginScrollView(rectTree, ref pathsScrollPos, rect8);
				DoTrees(rect8);
				Widgets.EndScrollView();
			}

			Text.Font = font;
			Text.Anchor = anchor;
		}

		private void DoTrees(Rect inRect)
		{
			Vector2 position = new Vector2 (inRect.position.x - inRect.width / 8f, inRect.position.y - inRect.height / 7f);
			float num = (inRect.width - (pathsPerRow + 1) * 10f) / pathsPerRow;
			float num2 = 0f;
			int num3 = pathsPerRow;
			foreach (SkillTreeDef def in from tree in treesByTab[curTab] orderby tree.order, tree.label select tree)
			{
				Texture2D texture2D = def.icon;
				float num4 = num / texture2D.width * texture2D.height + 30f;
				Rect rect = new Rect(position, new Vector2(num, num4));
				
				//if (hediff.unlockedPaths.Contains(def))
				//{
					if (def.HasSkills)
					{
						DoTreeSkills(rect, def, skillPos, DoSkill);
					}
				//}
				else
				{
					Widgets.DrawRectFast(rect, new Color(0f, 0f, 0f, 0.55f));
					/*if (devMode)
					{
						Vector2 size = new Vector2(140f, 30f);
						Rect rect2 = new Rect (rect.center - size / 2f, size);
						if (devMode || def.CanPawnUnlock(pawn))
						{
							if (Widgets.ButtonText(rect2, "fc_UnlockTree".Translate()))
							{
								if (!devMode)
								{
									hediff.SpentPoints();
								}
								hediff.UnlockPath(def);
							}
						}
						else
						{
							GUI.color = Color.grey;
							string text = "fc_skillsList".Translate().Resolve() + ": " + def.lockedReason;
							rect2.width = Mathf.Max(rect2.width, Text.CalcSize(text).x + 10f);
							Widgets.ButtonText(rect2, text, drawBackground: true, doMouseoverSound: true, active: false);
							GUI.color = Color.white;
						}
					}*/
					TooltipHandler.TipRegion(rect, () => def.tooltip + "\n\n" + "fc_skillsList".Translate() + "\n" + def.skillDefs.Select((SkillDef s) => s.label).ToLineList("  ", capitalizeItems: true), def.GetHashCode());
				}
				num2 = Mathf.Max(num2, num4 + 10f);
				position.x += num + 10f;
				num3--;
				if (num3 == 0)
				{
					position.x = inRect.x + 10f;
					position.y += num2;
					num3 = pathsPerRow;
					num2 = 0f;
				}
			}
			lastPathsHeight = position.y + num2;
		}

		public static void DoTreeSkills(Rect inRect, SkillTreeDef tree, Dictionary<SkillDef, Vector2> skillPos, Action<Rect, SkillDef> doSkill)
		{
			GameFont font = Text.Font;
			TextAnchor anchor = Text.Anchor;
			//skillTreeYOffsets[1][0] = tree.MaxLevel % 2 == 0 ? 40f : 10f;

			foreach (SkillDef skill in tree.skillDefs)
			{
				List<SkillDef> list = skill.prerequisites;
				if (list == null || !skillPos.ContainsKey(skill))
				{
					continue;
				}
				foreach (SkillDef item in list.Where((SkillDef skillDef) => skillPos.ContainsKey(skillDef)))
				{
					Widgets.DrawLine(skillPos[skill], skillPos[item], CompSkills.HasSkill(item) ? Color.white : Color.grey, 2f);
				}
			}

			for (int i = 0; i < tree.skillLevelsInOrder.Length; i++)
			{
				Rect rect = new Rect(inRect.x + (tree.MaxLevel - 1 + i) * inRect.width / tree.MaxLevel, inRect.y + inRect.height * 0.5f, inRect.width, inRect.height);

				SkillDef[] array = tree.skillLevelsInOrder[i];

				// Column icons
				if (!tree.columnIcons.NullOrEmpty())
				{
					Rect columnIconRect = new Rect(rect.x, rect.y - 100f, 32f, 32f);
					GUI.DrawTexture(columnIconRect, tree.columnIcons[i]);
                    TipSignal tip = new TipSignal
                    {
                        text = tree.columnNames[i],
                    };
					TooltipHandler.TipRegion(columnIconRect, tip);
					Widgets.DrawHighlightIfMouseover(columnIconRect);
				}

				// Row text
				if (!tree.rowNames.NullOrEmpty() && i == 0)
				{
					SkillDef[] awms = tree.skillLevelsInOrder.First(x => x.Length == tree.MaxLevel); // Find an array with most skills
					Text.Font = GameFont.Medium;
					Text.Anchor = TextAnchor.MiddleRight;

					for (int n = 0; n < awms.Length; n++)
					{
						//float textWidth = Text.CalcSize(tree.rowNames[i]).x;
						var textWidth = tree.textWidth;
						float textHeight = Text.CalcHeight(tree.rowNames[n], textWidth);

						Rect rowNameRect = new Rect(rect.x - rect.width / 2.25f, rect.y + skillTreeYOffsets[awms.Length - 1][n], textWidth, textHeight);

						Widgets.Label(rowNameRect, tree.rowNames[n]);
					}

					Text.Font = font;
					Text.Anchor = anchor;
				}

				for (int j = 0; j < array.Length; j++)
				{
					try
					{
						Rect arg = new Rect(rect.x, rect.y + skillTreeYOffsets[array.Length - 1][j], 36f, 36f);
						SkillDef skillDef = array[j];
						if (skillDef != SkillTreeDef.BlankSkill)
						{
							skillPos[skillDef] = arg.center;
							doSkill(arg, skillDef);
						}
					}
					catch (Exception ex)
                    {
						if (ex is IndexOutOfRangeException)
                        {
							Log.Error("Error while constructing skill tree: There cannot be more than 4 rows. Check XML for incorrect <reqLevel> values.");
							return;
                        }
                    }
				}
			}
		}

		public static void DrawSkill(Rect inRect, SkillDef skill)
		{
			Color color = (Mouse.IsOver(inRect) ? GenUI.MouseoverColor : Color.white);
			MouseoverSounds.DoRegion(inRect, SoundDefOf.Mouseover_Command);
			GUI.color = color;
			GUI.DrawTexture(inRect, Command.BGTexShrunk);
			GUI.color = Color.white;
			GUI.DrawTexture(inRect, skill.icon);
		}

		private void DoSkill(Rect inRect, SkillDef skill)
		{
			bool unlockable = false;
			bool flag = false;
			if (!compSkills.HasSkill(skill))
			{
				if (skill.prerequisites.NullOrEmpty() || compSkills.LearnedSkills.Intersect(skill.prerequisites).Count() == skill.prerequisites.Count()) // TODO: this shit
				{
					unlockable = true;
				}
				else
				{
					flag = true;
				}
			}
			if (unlockable)
			{
				QuickSearchWidget.DrawStrongHighlight(inRect.ExpandedBy(10f));
			}
			DrawSkill(inRect, skill);
			if (flag)
			{
				Widgets.DrawRectFast(inRect, new Color(0f, 0f, 0f, 0.6f));
			}
			TooltipHandler.TipRegion(inRect, () => string.Format("{0}\n\n{1}{2}", skill.LabelCap, skill.description, unlockable ? ("\n\n" + "fc_clickToUnlock".Translate().Resolve().ToUpper()) : ""), skill.GetHashCode());
			if (unlockable && Widgets.ButtonInvisible(inRect))
			{
				/*if (!devMode)
				{
					hediff.SpentPoints();
				}*/
				compSkills.GiveSkill(skill);
			}
		}
	}
}
