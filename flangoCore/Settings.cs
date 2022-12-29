using System.Collections.Generic;
using RimWorld;
using Verse;
using UnityEngine;

namespace flangoCore
{
	public class FlangoCore : Mod
	{
		public static Settings settings;

		public FlangoCore(ModContentPack content) : base(content)
		{
			settings = GetSettings<Settings>();
		}

		public override void DoSettingsWindowContents(Rect inRect)
		{
			settings.DoWindowContents(inRect);
		}

		public override string SettingsCategory()
		{
			return "fc_Settings".Translate();
		}
	}

	public class Settings : ModSettings
	{
		public bool optionalPatchTest = true;


		public bool enableVFEPatches = true;

		public bool enableDeflectionText = true;
		public bool deflectionChanceAffectedByMeleeSkill = true;
		public bool deflectionAccuracyAffectedByMeleeSkill = true;

		public bool enableVCF = false;

		public bool enableAbilityCooldownOnEquipForItems = true;
		public bool enableAbilityCooldownOnEquipForHediffs = false;

		private int corpseCapacity = 20;
		public int CorpseCapacity => corpseCapacity;

		private float baseDodgeChance = 0;
		public float BaseDodgeChance
		{
			get
			{
				StatDefOf_flangoCore.RangedDodgeChance.defaultBaseValue = baseDodgeChance/100;
				return baseDodgeChance;
			}
            set => baseDodgeChance = value;
		}

		public float skillTreeUIScale = 1f;



		public void DoWindowContents(Rect canvas)
		{
            Listing_Standard listing_Standard = new() { ColumnWidth = canvas.width / 2f };
            listing_Standard.Begin(canvas);

			listing_Standard.Gap(16f);
			listing_Standard.CheckboxLabeled(Translator.Translate("fc_enableVFEPatches"), ref enableVFEPatches);
			listing_Standard.Indent();
			listing_Standard.Label("fc_VFEPatchesExplained".Translate());
			listing_Standard.Outdent();

			listing_Standard.Gap(16f);
			listing_Standard.CheckboxLabeled(Translator.Translate("fc_enableDeflectionText"), ref enableDeflectionText);
			listing_Standard.CheckboxLabeled(Translator.Translate("fc_deflectionChanceAffectedByMeleeSkill"), ref deflectionChanceAffectedByMeleeSkill);
			listing_Standard.CheckboxLabeled(Translator.Translate("fc_deflectionAccuracyAffectedByMeleeSkill"), ref deflectionAccuracyAffectedByMeleeSkill);
			listing_Standard.Gap(8f);
			listing_Standard.Indent();
			listing_Standard.Label("fc_deflectionChanceExplained".Translate());
			listing_Standard.Outdent();


			listing_Standard.Gap(16f);
			string bDodge = baseDodgeChance.ToString();
			listing_Standard.TextFieldNumericLabeled("fc_baseDodgeChance".Translate(), ref baseDodgeChance, ref bDodge, 0, 100);

			listing_Standard.Gap(16f);
			listing_Standard.CheckboxLabeled(Translator.Translate("fc_enableVCF"), ref enableVCF);

			listing_Standard.Gap(16f);
			listing_Standard.CheckboxLabeled(Translator.Translate("fc_enableAbilityCooldownOnEquipForItems"), ref enableAbilityCooldownOnEquipForItems);
			listing_Standard.CheckboxLabeled(Translator.Translate("fc_enableAbilityCooldownOnEquipForHediffs"), ref enableAbilityCooldownOnEquipForHediffs);

			listing_Standard.Gap(16f);
			string bCorpses = corpseCapacity.ToString();
			listing_Standard.TextFieldNumericLabeled("fc_MassGrave_Capacity".Translate(), ref corpseCapacity, ref bCorpses, 1f);


			listing_Standard.NewColumn();

			listing_Standard.Gap(16f);
			listing_Standard.Label("fc_SkillTreeUIScale".Translate());
			listing_Standard.Slider(skillTreeUIScale, 0.5f, 2f);

			listing_Standard.Gap(16f);
			listing_Standard.CheckboxLabeled("Optional Patch Test", ref optionalPatchTest);

			listing_Standard.End();
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref enableVFEPatches, "enableVFEPatches", true);

			Scribe_Values.Look(ref enableDeflectionText, "enableDeflectionText", true);
			Scribe_Values.Look(ref deflectionChanceAffectedByMeleeSkill, "deflectionChanceAffectedByMeleeSkill", true);
			Scribe_Values.Look(ref deflectionAccuracyAffectedByMeleeSkill, "deflectionAccuracyAffectedByMeleeSkill", true);

			Scribe_Values.Look(ref enableVCF, "enableVCF", false);

			Scribe_Values.Look(ref enableAbilityCooldownOnEquipForItems, "enableAbilityCooldownOnEquipForItems", true);
			Scribe_Values.Look(ref enableAbilityCooldownOnEquipForHediffs, "enableAbilityCooldownOnEquipForHediffs", false);

			Scribe_Values.Look(ref corpseCapacity, "corpseCapacity", 20);
			Scribe_Values.Look(ref baseDodgeChance, "baseDodgeChance", 0);

			Scribe_Values.Look(ref optionalPatchTest, "optionalPatchTest", true);
		}
	}
}
