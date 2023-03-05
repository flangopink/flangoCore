using Verse;
using RimWorld;
using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Reflection;
using System;

namespace flangoCore
{
    //[HarmonyPatch(typeof(FloatMenuMakerMap), "CanTakeOrder")]
    public static class Patch_FloatMenuMakerMap_CanTakeOrder
    {
        //[HarmonyPostfix]
        public static void MakePawnControllable(Pawn pawn, ref bool __result)
        {
            if (pawn.IsDraftable() && pawn.Faction != null && (pawn.Faction?.IsPlayer ?? false))
            {
                Log.Warning("" + pawn.IsColonist + pawn.IsColonistPlayerControlled + pawn.IsColonyMech);
                __result = true;
            }
        }
    }

    //[HarmonyPatch(typeof(FloatMenuMakerMap), "AddUndraftedOrders")]
    public static class Patch_FloatMenuMakerMap_AddUndraftedOrders
    {
        //[HarmonyPrefix]
        public static bool AvoidGeneralErrorIfPawnIsAnimal(Pawn pawn)
        {
            return !pawn.IsDraftable();
        }
    }

    //[HarmonyPatch(typeof(FloatMenuMakerMap), "AddDraftedOrders")]
    public static class FloatMenuMakerMap_AddDraftedOrders_Transpiler
    {
        // i have no idea what this one does.
        public static IEnumerable<CodeInstruction> AddDraftedOrders_Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            MethodInfo shouldSkip = AccessTools.Method(typeof(Draftables), "IsDraftableControllable");
            List<CodeInstruction> codes = instructions.ToList();
            FieldInfo pawnField = AccessTools.Field(typeof(FloatMenuMakerMap).GetNestedTypes(AccessTools.all).FirstOrDefault((Type c) => c.Name.Contains("c__DisplayClass8_0")), "pawn");
            FieldInfo skillsField = AccessTools.Field(typeof(Pawn), "skills");
            FieldInfo constructionDefField = AccessTools.Field(typeof(SkillDefOf), "Construction");
            bool patched = false;
            for (int i = 0; i < codes.Count; i++)
            {
                CodeInstruction instr = codes[i];
                if (!patched && codes[i].opcode == OpCodes.Ldloc_0 
                    && CodeInstructionExtensions.LoadsField(codes[i + 1], pawnField, false) 
                    && CodeInstructionExtensions.LoadsField(codes[i + 2], skillsField, false) 
                    && CodeInstructionExtensions.LoadsField(codes[i + 3], constructionDefField, false))
                {
                    patched = true;
                    yield return CodeInstructionExtensions.MoveLabelsFrom(new CodeInstruction(OpCodes.Ldloc_0, null), codes[i]);
                    yield return new CodeInstruction(OpCodes.Ldfld, pawnField);
                    yield return new CodeInstruction(OpCodes.Call, shouldSkip);
                    yield return new CodeInstruction(OpCodes.Brtrue_S, codes[i + 6].operand);
                }
                yield return instr;
            }
            if (!patched)
            {
                Log.Error("FloatMenuMakerMap:AddDraftedOrders Transpiler failed");
            }
        }
    }
}
