using HarmonyLib;
using RimWorld;
using Verse;

namespace flangoCore
{
    //[HarmonyPatch(typeof(Pawn), "get_IsColonistPlayerControlled")]
    public static class Patch_Pawn_IsColonistPlayerControlled
    {
        //[HarmonyPostfix]
        public static void AddPawnAsColonist(Pawn __instance, ref bool __result)
        {
            if (__instance.IsDraftable())
            {
                __result = __instance.Spawned && __instance.HostFaction == null && __instance.Faction == Faction.OfPlayer;
            }
        }
    }
}
