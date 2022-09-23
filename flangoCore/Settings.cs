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
		public bool enableVFEPatches;


		private int corpseCapacity = 20;
		public int CorpseCapacity => corpseCapacity;

		public void DoWindowContents(Rect canvas)
		{
            Listing_Standard listing_Standard = new Listing_Standard
            { ColumnWidth = canvas.width / 2.5f };
            listing_Standard.Begin(canvas);

			listing_Standard.Gap(16f);
			listing_Standard.CheckboxLabeled(Translator.Translate("fc_enableVFEPatches"), ref enableVFEPatches);
			listing_Standard.Label("fc_VFEPatchesExplained".Translate());

			//listing_Standard.NewColumn();

			listing_Standard.Gap(16f);
			//listing_Standard.Label("fc_MassGraves".Translate());
			listing_Standard.Label("fc_MassGravesCapacity".Translate());
			string buffer = corpseCapacity.ToString();
			listing_Standard.TextFieldNumeric(ref corpseCapacity, ref buffer, 1f);

			listing_Standard.End();
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_Values.Look(ref enableVFEPatches, "enableVFEPatches", false);
			Scribe_Values.Look(ref corpseCapacity, "corpseCapacity", 20);
		}
	}
}
