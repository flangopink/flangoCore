using RimWorld;
using Verse;
using HarmonyLib;
using UnityEngine;

namespace flangoCore
{
	[HarmonyPatch(typeof(ArmorUtility), "ApplyArmor")]
	public static class Patch_ArmorUtility_ApplyArmor_Shred
	{
		public static bool Prefix(ref float damAmount, float armorPenetration, float armorRating, Thing armorThing, ref DamageDef damageDef, Pawn pawn, out bool metalArmor)
		{
			if (damageDef.HasModExtension<ModExt_ArmorShredding>())
			{
				if (armorThing != null)
				{
					metalArmor = armorThing.def.apparel.useDeflectMetalEffect || (armorThing.Stuff != null && armorThing.Stuff.IsMetal);
				}
				else
				{
					metalArmor = pawn.RaceProps.IsMechanoid;
				}
				if (armorThing != null)
				{
					float mult = damageDef.GetModExtension<ModExt_ArmorShredding>().multiplier;
					float f = damAmount * mult;
					armorThing.TakeDamage(new DamageInfo(damageDef, GenMath.RoundRandom(f)));
				}
				float num = Mathf.Max(armorRating - armorPenetration, 0f);
				float value = Rand.Value;
				float num2 = num * 0.5f;
				float num3 = num;
				if (value < num2)
				{
					damAmount = 0f;
				}
				else if (value < num3)
				{
					damAmount = GenMath.RoundRandom(damAmount / 2f);
					if (damageDef.armorCategory == DamageArmorCategoryDefOf.Sharp)
					{
						damageDef = DamageDefOf.Blunt;
					}
				}
				return false;
			}
			metalArmor = false;
			return true;
		}
	}

}
