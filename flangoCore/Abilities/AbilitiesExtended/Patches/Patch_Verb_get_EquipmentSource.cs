using Verse;
using HarmonyLib;
using System.Linq;

namespace flangoCore
{
    [HarmonyPatch(typeof(Verb), "get_EquipmentSource")]
    public static class Patch_Verb_get_EquipmentSource
    {
        [HarmonyPrefix]
        public static bool Prefix(ref Verb __instance, ref ThingWithComps __result)
        {
            if (__instance is Verb_UseEquipmentAbility verb)
            {
                if (verb.ability is EquipmentAbility equipmentAbility)
                {
                    if (equipmentAbility.sources.OfType<ThingWithComps>().EnumerableNullOrEmpty()) return true;

                    __result = equipmentAbility.sources.OfType<ThingWithComps>().First();
                }
            }
            return true;
        }
    }
}
