﻿using RimWorld;
using Verse;
using HarmonyLib;
using System.Text;

namespace flangoCore
{
	public class ModExt_CustomDescription : DefModExtension
	{
		public string description;
		public bool affectVerb = true;
		public bool affectInventory = true;
	}

	[HarmonyPatch(typeof(VerbTracker), "CreateVerbTargetCommand")]
	public static class Patch_CreateVerbTargetCommand
	{
		[HarmonyPostfix]
		public static void Postfix(ref Command_VerbTarget __result, Thing ownerThing)
		{
			if (ownerThing.def.HasModExtension<ModExt_CustomDescription>() && ownerThing.def.GetModExtension<ModExt_CustomDescription>().affectVerb)
				__result.defaultDesc = ownerThing.LabelCap + ": " + ownerThing.def.GetModExtension<ModExt_CustomDescription>().description.CapitalizeFirst();
		}
	}

	[HarmonyPatch(typeof(ThingDef), "get_DescriptionDetailed")]
	public static class Patch_DescriptionDetailed
	{
		[HarmonyPrefix]
		public static bool Prefix(ThingDef __instance, ref string __result)
		{
			if (__instance.HasModExtension<ModExt_CustomDescription>() && __instance.GetModExtension<ModExt_CustomDescription>().affectInventory)
            {
				if (__instance.descriptionDetailedCached == null)
				{
					StringBuilder stringBuilder = new StringBuilder();
					stringBuilder.Append(__instance.GetModExtension<ModExt_CustomDescription>().description);
					if (__instance.IsApparel)
					{
						stringBuilder.AppendLine();
						stringBuilder.AppendLine();
						stringBuilder.AppendLine(string.Format("{0}: {1}", "Layer".Translate(), __instance.apparel.GetLayersString()));
						stringBuilder.Append(string.Format("{0}: {1}", "Covers".Translate(), __instance.apparel.GetCoveredOuterPartsString(BodyDefOf.Human)));
						if (__instance.equippedStatOffsets != null && __instance.equippedStatOffsets.Count > 0)
						{
							stringBuilder.AppendLine();
							stringBuilder.AppendLine();
							for (int i = 0; i < __instance.equippedStatOffsets.Count; i++)
							{
								if (i > 0)
								{
									stringBuilder.AppendLine();
								}
								StatModifier statModifier = __instance.equippedStatOffsets[i];
								stringBuilder.Append($"{statModifier.stat.LabelCap}: {statModifier.ValueToStringAsOffset}");
							}
						}
					}
					__result = stringBuilder.ToString();
					return false;
				}
			}
			return true;
		}
	}
}
