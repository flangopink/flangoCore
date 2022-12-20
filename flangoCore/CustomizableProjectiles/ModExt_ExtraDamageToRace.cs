using System.Collections.Generic;
using RimWorld;
using Verse;

namespace flangoCore
{
    public class FleshTypeMultipliers 
    {
        public FleshTypeDef fleshTypeDef;
        public float multiplier = 1.5f;
    }

    public class PawnKindMultipliers 
    {
        public PawnKindDef pawnKindDef;
        public float multiplier = 1.5f;
    }

    public class ModExt_ExtraDamageToRace : DefModExtension
    {
        public List<FleshTypeMultipliers> fleshTypes;
        public List<PawnKindMultipliers> pawnKinds;
        public float globalMultiplier;
    }
}
