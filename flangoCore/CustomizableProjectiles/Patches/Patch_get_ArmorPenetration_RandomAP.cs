using HarmonyLib;
using Verse;
using RimWorld;

namespace flangoCore
{
    [HarmonyPatch(typeof(Projectile), "get_ArmorPenetration")]
    public static class Patch_get_ArmorPenetration_RandomAP
    {
        [HarmonyPrefix]
        public static bool ArmorPen_Prefix(Projectile __instance, ref float __result)
        {
            if (__instance is Projectile_RandomAPBullet bullet)
            {
                ThingDef_RandomAPBullet def = bullet.Def;
                if (def != null)
                {
                    float roll = Rand.Value;
                    foreach (APWithChance h in def.AP)
                    {
                        if (roll < h.APChance)
                        {
                            __result = h.APValue;
                            //Log.Message("Rolled armorpen of " + __result);
                            return false;
                        }
                        else
                        {
                            roll -= h.APChance;
                        }
                    }
                    //Log.Message("Didn't roll armorpen");
                }
            }
            return true;
        }
    }
}
