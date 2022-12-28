using Verse;
using HarmonyLib;
using System.Linq;

namespace flangoCore
{

    [HarmonyPatch(typeof(DamageWorker_AddInjury), "FinalizeAndAddInjury")]
    public class Patch_DamageWorker_AddInjury_FinalizeAndAddInjury
    {
        [HarmonyPostfix]
        public static void Postfix(Pawn pawn)
        {
            CompDraftable comp = pawn.TryGetComp<CompDraftable>();

            if (comp == null || comp.Props.equipmentBodyPartTag == null) return;

            BodyPartRecord part = pawn.RaceProps.body.GetPartsWithTag(comp.Props.equipmentBodyPartTag).FirstOrDefault();

            if (part == null) return;

            if (pawn.health.hediffSet.PartIsMissing(part))
            {
                pawn.equipment?.DestroyAllEquipment();
            }
        }
    }
}
