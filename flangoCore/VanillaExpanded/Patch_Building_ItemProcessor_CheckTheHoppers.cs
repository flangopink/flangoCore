using System.Collections.Generic;
using RimWorld;
using Verse;

namespace flangoCore
{
    public class Patch_Building_ItemProcessor_CheckTheHoppers
	{
		public static bool Prefix(Building __instance, string itemToCheckFor, ref int ExpectedAmountXIngredient, ref int CurrentAmountXIngredient, ref bool XIngredientComplete)
		{
			var itemProcessor = (ItemProcessor.Building_ItemProcessor)__instance;
			if ((itemProcessor.compItemProcessor.Props.noPowerDestroysProgress && itemProcessor.compPowerTrader != null && !itemProcessor.compPowerTrader.PowerOn) || (itemProcessor.compItemProcessor.Props.noPowerDestroysProgress && itemProcessor.compFuelable != null && !itemProcessor.compFuelable.HasFuel))
			{
				return false;
			}
			bool flag = false;
			for (int i = 0; i < itemProcessor.compItemProcessor.Props.inputSlots.Count; i++)
			{
				if (flag)
				{
					continue;
				}
				Thing thing = null;
				Thing thing2 = null;
				List<Thing> thingList = (itemProcessor.Position + itemProcessor.compItemProcessor.Props.inputSlots[i].RotatedBy(itemProcessor.Rotation)).GetThingList(itemProcessor.Map);
				for (int j = 0; j < thingList.Count; j++)
				{
					Thing thing3 = thingList[j];
					if (DefDatabase<ItemProcessor.CombinationDef>.GetNamed(itemProcessor.thisRecipe).isCategoryRecipe)
					{
						if (thing3.def.IsWithinCategory(ThingCategoryDef.Named(itemToCheckFor)))
						{
							thing = thing3;
						}
					}
					else if (thing3.def.defName == itemToCheckFor)
					{
						thing = thing3;
					}
					
					if (itemProcessor.def.GetModExtension<ModExt_AcceptedHoppers>() == null || itemProcessor.def.GetModExtension<ModExt_AcceptedHoppers>().thingDefs.NullOrEmpty())
					{
						thing3.def = ThingDefOf.Hopper;
						thing2 = thing3;
					}
					else if (itemProcessor.def.GetModExtension<ModExt_AcceptedHoppers>().thingDefs.Contains(thing3.def))
                    {
						thing2 = thing3;
					}
				}
				if (thing == null || thing2 == null)
				{
					continue;
				}
				flag = true;
				if (ExpectedAmountXIngredient != 0)
				{
					int num = ExpectedAmountXIngredient - CurrentAmountXIngredient;
					if (thing.stackCount - num > 0)
					{
						CurrentAmountXIngredient += num;
						if (CurrentAmountXIngredient >= ExpectedAmountXIngredient)
						{
							XIngredientComplete = true;
						}
						itemProcessor.firstItemSwallowedForButchery = thing.def.defName;
						itemProcessor.Notify_StartProcessing();
						thing.stackCount -= num;
						if (thing.stackCount <= 0)
						{
							thing.Destroy();
						}
					}
					else if (thing.stackCount - num <= 0 && !XIngredientComplete)
					{
						CurrentAmountXIngredient += thing.stackCount;
						itemProcessor.firstItemSwallowedForButchery = thing.def.defName;
						itemProcessor.Notify_StartProcessing();
						thing.Destroy();
					}
				}
				else
				{
					itemProcessor.Notify_StartProcessing();
					thing.Destroy();
				}
			}
			return false;
		}
	}
}