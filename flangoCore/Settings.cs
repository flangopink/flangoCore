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
		public bool enableDraftables = true;

		public bool randomizerEnabled = false;

		// Deflection
        private float baseDeflectionChance = 0;
        public float BaseDeflectionChance
        {
            get
            {
				float val = baseDeflectionChance * 0.01f;
                DefOf_flangoCore.ProjectileDeflectionChance.defaultBaseValue = val;
                return val;
            }
            set => baseDeflectionChance = value;
        }
		public bool deflectionChanceAffectedByMeleeSkill = false;
        private float deflectionPerSkill = 1.5f;
        public float DeflectionPerSkill
        {
            get => deflectionPerSkill * 0.01f;
            set => baseDeflectionChance = value;
        }
        public bool deflectionAccuracyAffectedByMeleeSkill = true;
		public bool blockNonHostileProjectiles = false;
        public bool enableDeflectionText = true;

        // Dodge
        private float baseDodgeChance = 0;
        public float BaseDodgeChance
        {
            get
            {
				float val = baseDodgeChance * 0.01f;
                DefOf_flangoCore.RangedDodgeChance.defaultBaseValue = val;
                return val;
            }
            set => baseDodgeChance = value;
        }
        public bool dodgeScalesWithMovement = true;


        public bool enableVCF = false;

		public bool enableAbilityCooldownOnEquipForItems = true;
		public bool enableAbilityCooldownOnEquipForHediffs = false;

		private int corpseCapacity = 20;
		public int CorpseCapacity => corpseCapacity;



        public float skillTreeUIScale = 1f;

		//public bool enableAnimatedWeapons = true;


        public void DoWindowContents(Rect canvas)
		{
            Listing_Standard listing_Standard = new() { ColumnWidth = canvas.width / 2.1f };
            listing_Standard.Begin(canvas);

			listing_Standard.Gap(16f);
			listing_Standard.CheckboxLabeled(Translator.Translate("fc_enableVFEPatches"), ref enableVFEPatches);
			listing_Standard.Indent();
			listing_Standard.Label("fc_VFEPatchesExplained".Translate());
			listing_Standard.Outdent();

			// Deflection
			listing_Standard.Gap(16f);
            string bDef = baseDeflectionChance.ToString();
            listing_Standard.TextFieldNumericLabeled("fc_baseDeflectionChance".Translate(), ref baseDeflectionChance, ref bDef, 0, 100);
            listing_Standard.CheckboxLabeled(Translator.Translate("fc_deflectionChanceAffectedByMeleeSkill"), ref deflectionChanceAffectedByMeleeSkill);
            string bDefPerLvl = deflectionPerSkill.ToString();
            listing_Standard.TextFieldNumericLabeled("fc_deflectionChancePerMeleeLevel".Translate(), ref deflectionPerSkill, ref bDefPerLvl, 0, 100);
            listing_Standard.CheckboxLabeled(Translator.Translate("fc_deflectionAccuracyAffectedByMeleeSkill"), ref deflectionAccuracyAffectedByMeleeSkill);
			listing_Standard.CheckboxLabeled(Translator.Translate("fc_blockNonHostileProjectiles"), ref blockNonHostileProjectiles);
            listing_Standard.CheckboxLabeled(Translator.Translate("fc_enableDeflectionText"), ref enableDeflectionText);
            listing_Standard.Gap(8f);
			listing_Standard.Indent();
			listing_Standard.Label("fc_deflectionChanceExplained".Translate());
			listing_Standard.Outdent();

            // Dodge
            listing_Standard.Gap(16f);
            string bDodge = baseDodgeChance.ToString();
			listing_Standard.TextFieldNumericLabeled("fc_baseDodgeChance".Translate(), ref baseDodgeChance, ref bDodge, 0, 100);
            listing_Standard.CheckboxLabeled(Translator.Translate("fc_dodgeScalesWithMovement"), ref dodgeScalesWithMovement);

            // Verb Cooldown Factor
            listing_Standard.Gap(16f);
			listing_Standard.CheckboxLabeled(Translator.Translate("fc_enableVCF"), ref enableVCF);

			// Ability Cooldown on Equip
			listing_Standard.Gap(16f);
			listing_Standard.CheckboxLabeled(Translator.Translate("fc_enableAbilityCooldownOnEquipForItems"), ref enableAbilityCooldownOnEquipForItems);
			listing_Standard.CheckboxLabeled(Translator.Translate("fc_enableAbilityCooldownOnEquipForHediffs"), ref enableAbilityCooldownOnEquipForHediffs);

			// Mass Grave
			listing_Standard.Gap(16f);
			string bCorpses = corpseCapacity.ToString();
			listing_Standard.TextFieldNumericLabeled("fc_MassGrave_Capacity".Translate(), ref corpseCapacity, ref bCorpses, 1f);


			listing_Standard.NewColumn();

			// Skill Trees
			listing_Standard.Gap(16f);
			listing_Standard.Label("fc_SkillTreeUIScale".Translate());
            skillTreeUIScale = listing_Standard.Slider(skillTreeUIScale, 0.5f, 2f); // fucking sliders

			// Optional Patches
			listing_Standard.Gap(16f);
			listing_Standard.CheckboxLabeled("Optional Patch Test", ref optionalPatchTest);

			// Optional Patches
			listing_Standard.Gap(16f);
			listing_Standard.CheckboxLabeled("fc_EnableDraftables".Translate(), ref enableDraftables);
            listing_Standard.Label("fc_EnableDraftablesWarning".Translate());

            // Randomizer
            listing_Standard.Gap(16f);
			listing_Standard.CheckboxLabeled("fc_EnableRandomizer".Translate(), ref randomizerEnabled);

			// Animated Weapons (WIP)
            //listing_Standard.Gap(16f);
            //listing_Standard.CheckboxLabeled(Translator.Translate("fc_enableAnimatedWeapons"), ref enableAnimatedWeapons);

            listing_Standard.End();
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref enableVFEPatches, "enableVFEPatches", true);

            Scribe_Values.Look(ref enableDeflectionText, "enableDeflectionText", true);
            Scribe_Values.Look(ref baseDeflectionChance, "baseDeflectionChance", 0);
            Scribe_Values.Look(ref deflectionPerSkill, "deflectionPerSkill", 1.5f);
			Scribe_Values.Look(ref deflectionChanceAffectedByMeleeSkill, "deflectionChanceAffectedByMeleeSkill", false); // Off by default, but added stats work.
			Scribe_Values.Look(ref deflectionAccuracyAffectedByMeleeSkill, "deflectionAccuracyAffectedByMeleeSkill", true);
			Scribe_Values.Look(ref blockNonHostileProjectiles, "blockNonHostileProjectiles", false);

			Scribe_Values.Look(ref enableVCF, "enableVCF", false);

			Scribe_Values.Look(ref enableAbilityCooldownOnEquipForItems, "enableAbilityCooldownOnEquipForItems", true);
			Scribe_Values.Look(ref enableAbilityCooldownOnEquipForHediffs, "enableAbilityCooldownOnEquipForHediffs", false);

			Scribe_Values.Look(ref corpseCapacity, "corpseCapacity", 20);

			Scribe_Values.Look(ref skillTreeUIScale, "skillTreeUIScale", 1f);

			Scribe_Values.Look(ref baseDodgeChance, "baseDodgeChance", 0);
			Scribe_Values.Look(ref dodgeScalesWithMovement, "dodgeScalesWithMovement", true);

            Scribe_Values.Look(ref optionalPatchTest, "optionalPatchTest", true);

            Scribe_Values.Look(ref enableVFEPatches, "enableVFEPatches", true);

            Scribe_Values.Look(ref enableDraftables, "enableDraftables", true);

            Scribe_Values.Look(ref randomizerEnabled, "randomizerEnabled", false);
        }
	}
}