using Verse;
using HarmonyLib;
using RimWorld;
using System.Linq;

namespace flangoCore
{
    public class Patch_EquipmentUtility_CanEquip
    {
        [HarmonyPostfix]
        public static void Postfix(Thing thing, Pawn pawn, ref string cantReason, ref bool __result)
        {
            if (__result)
            {
                ThingDef def = thing.def;

                if (def.IsApparel || def.equipmentType == EquipmentType.Primary)
                {
                    ModExt_Requirements ext = def.GetModExtension<ModExt_Requirements>();
                    if (ext == null) return;

                    bool strict = !(ext.anyCategory || ext.anyItemInCategories != CategoryFlags.None);

                    if (!(ext.gender == Gender.None || pawn.gender == ext.gender))
                    {
                        __result = false;
                        cantReason = "fc_CantBeUsedByThisGender".Translate();
                        return;
                    }

                    // KindDef
                    if (!ext.kindDefs.NullOrEmpty() && !ext.kindDefs.Contains(pawn.kindDef))
                    {
                        __result = false;
                        cantReason = "fc_CantBeUsedByThisKindDef".Translate();
                        return;
                    }

                    // Apparel
                    if (!ext.apparelDefs.NullOrEmpty() && pawn.apparel?.WornApparelCount > 0)
                    {
                        var defs = pawn.apparel.WornApparel.Select(x => x.def).ToList();
                        var matches = defs.Intersect(ext.apparelDefs);

                        if ((matches.EnumerableNullOrEmpty() && strict) || matches.Count() == 0 || (!ext.anyItemInCategories.HasFlag(CategoryFlags.Apparel) && matches.Count() != ext.apparelDefs.Count()))
                        {
                            __result = false;
                            cantReason = "fc_MissingRequiredApparel".Translate();
                            return;
                        }
                    }

                    // Hediffs
                    if (!ext.hediffDefs.NullOrEmpty())
                    {
                        var defs = pawn.health.hediffSet.hediffs.Select(x => x.def).ToList();
                        var matches = defs.Intersect(ext.hediffDefs);

                        if ((matches.EnumerableNullOrEmpty() && strict) || matches.Count() == 0 || (!ext.anyItemInCategories.HasFlag(CategoryFlags.Hediffs) && matches.Count() != ext.hediffDefs.Count()))
                        {
                            __result = false;
                            cantReason = "fc_MissingRequiredHediff".Translate();
                            return;
                        }
                    }

                    // Traits
                    if (!ext.traitDefs.NullOrEmpty())
                    {
                        var defs = pawn.story.traits.allTraits.Select(x => x.def).ToList();
                        var matches = defs.Intersect(ext.traitDefs);

                        if ((matches.EnumerableNullOrEmpty() && strict) || matches.Count() == 0 || (!ext.anyItemInCategories.HasFlag(CategoryFlags.Traits) && matches.Count() != ext.traitDefs.Count()))
                        {
                            __result = false;
                            cantReason = "fc_MissingRequiredTrait".Translate();
                            return;
                        }
                    }

                    // Must be capable of
                    if (ext.mustBeCapableOf != WorkTags.None)
                    {
                        if (pawn.story.DisabledWorkTagsBackstoryTraitsAndGenes.HasFlag(ext.mustBeCapableOf))
                        {
                            __result = false;
                            cantReason = "AbilityDisabled_IncapableOfWorkTag".Translate(pawn.Named("PAWN"), ext.mustBeCapableOf.LabelTranslated());
                            return;
                        }
                    }

                    // Skill Requirements
                    if (!ext.skillRequirements.NullOrEmpty())
                    {
                        if (pawn.skills == null)
                        {
                            __result = false; 
                            return;
                        }

                        SkillRequirement s = null;
                        for (int i = 0; i < ext.skillRequirements.Count; i++)
                        {
                            var si = ext.skillRequirements[i];
                            if (!si.PawnSatisfies(pawn))
                            {
                                s = si;
                                break;
                            }
                        }

                        if (s != null)
                        {
                            __result = false;
                            cantReason = "SkillTooLow".Translate(s.skill.skillLabel, pawn.skills.GetSkill(s.skill).Level, s.minLevel);
                            return;
                        }
                    }

                    // Genes
                    if (!ext.geneDefs.NullOrEmpty())
                    {
                        var defs = pawn.genes.GenesListForReading.Select(x => x.def).ToList();
                        var matches = defs.Intersect(ext.geneDefs);

                        if ((matches.EnumerableNullOrEmpty() && strict) || matches.Count() == 0 || (!ext.anyItemInCategories.HasFlag(CategoryFlags.Genes) && matches.Count() != ext.geneDefs.Count()))
                        {
                            __result = false;
                            cantReason = "fc_MissingRequiredGene".Translate();
                            return;
                        }
                    }
                    __result = true;
                }
            }
        }
    }
}
