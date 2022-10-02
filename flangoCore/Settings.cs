using System.Collections.Generic;
using RimWorld;
using Verse;
using UnityEngine;

namespace flangoCore
{
	public class Controller : Mod
	{
		public static Settings settings;

		public Controller(ModContentPack content) : base(content)
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
		public bool enableVFEPatches = true;
		public bool enableDeflectionText = true;
		public bool enableVCF = false;
		public bool enableAbilityCooldownOnEquipForItems = true;
		public bool enableAbilityCooldownOnEquipForHediffs = false;

		private int corpseCapacity = 20;
		public int CorpseCapacity => corpseCapacity;

		public void DoWindowContents(Rect canvas)
		{
            Listing_Standard listing_Standard = new Listing_Standard
            { ColumnWidth = canvas.width / 2f };
            listing_Standard.Begin(canvas);

			listing_Standard.Gap(16f);
			listing_Standard.CheckboxLabeled(Translator.Translate("fc_enableVFEPatches"), ref enableVFEPatches);
			listing_Standard.Label("fc_VFEPatchesExplained".Translate());

			listing_Standard.Gap(16f);
			listing_Standard.CheckboxLabeled(Translator.Translate("fc_enableDeflectionText"), ref enableDeflectionText);

			listing_Standard.Gap(16f);
			listing_Standard.CheckboxLabeled(Translator.Translate("fc_enableVCF"), ref enableVCF);

			listing_Standard.Gap(16f);
			listing_Standard.CheckboxLabeled(Translator.Translate("fc_enableAbilityCooldownOnEquipForItems"), ref enableAbilityCooldownOnEquipForItems);
			listing_Standard.CheckboxLabeled(Translator.Translate("fc_enableAbilityCooldownOnEquipForHediffs"), ref enableAbilityCooldownOnEquipForHediffs);

			//listing_Standard.NewColumn();

			listing_Standard.Gap(16f);
			listing_Standard.Label("fc_MassGrave_Capacity".Translate());
			string b1 = corpseCapacity.ToString();
			listing_Standard.TextFieldNumeric(ref corpseCapacity, ref b1, 1f);

			listing_Standard.End();
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref enableVFEPatches, "enableVFEPatches", true);
			Scribe_Values.Look(ref enableDeflectionText, "enableDeflectionText", true);
			Scribe_Values.Look(ref enableVCF, "enableVCF", false);
			Scribe_Values.Look(ref enableAbilityCooldownOnEquipForItems, "enableAbilityCooldownOnEquipForItems", true);
			Scribe_Values.Look(ref enableAbilityCooldownOnEquipForHediffs, "enableAbilityCooldownOnEquipForHediffs", false);
			Scribe_Values.Look(ref corpseCapacity, "corpseCapacity", 20);
		}
	}
}
