using RimWorld;
using System;
using System.Collections.Generic;
using Verse;

namespace flangoCore
{
    public class ModExt_Requirements : DefModExtension
    {
        public bool anyCategory;
        public CategoryFlags anyItemInCategories;
        public List<PawnKindDef> kindDefs;
        public List<ThingDef> apparelDefs;
        public List<HediffDef> hediffDefs;
        public List<TraitDef> traitDefs;
        public WorkTags mustBeCapableOf;
        public List<SkillRequirement> skillRequirements;
        public List<GeneDef> geneDefs;
        public Gender gender = Gender.None;
    }

    [Flags]
    public enum CategoryFlags
    {
        None = 0,
        Apparel = 1,
        Hediffs = 2,
        Traits = 4,
        Genes = 8,
        All = 16
    }
}
