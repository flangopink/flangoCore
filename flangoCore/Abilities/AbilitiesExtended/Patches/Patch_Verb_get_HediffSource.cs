using Verse;
using HarmonyLib;
using System.Linq;

namespace flangoCore
{
    [HarmonyPatch(typeof(Verb), "get_HediffSource")]
    public static class Patch_Verb_get_HediffSource
    {
        [HarmonyPrefix]
        public static bool Prefix(ref Verb __instance, ref Hediff __result)
        {
            if (__instance is Verb_UseEquipmentAbility verb)
            {
                if (verb.ability is EquipmentAbility equipmentAbility)
                {
                    if (equipmentAbility.sources.OfType<Hediff>().EnumerableNullOrEmpty()) return true;

                    __result = equipmentAbility.sources.OfType<Hediff>().First();
                }
            }
            return true;
        }
    }
}
