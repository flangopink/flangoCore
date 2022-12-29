using HarmonyLib;
using Verse;

namespace flangoCore
{
    [HarmonyPatch(typeof(MechanitorUtility), "EverControllable")]
    public static class Patch_MechanitorUtility_EverControllable
    {
        [HarmonyPrefix]
        public static bool EverControllable(Pawn mech, ref bool __result)
        {
            if (mech.IsDraftable())
            {
                __result = true;
                return false;
            }
            return true;
        }
    }
}
