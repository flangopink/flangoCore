using System.Collections.Generic;
using RimWorld;
using Verse;
using HarmonyLib;

namespace flangoCore
{
    public class FleshTypeMultipliers 
    {
        public FleshTypeDef fleshTypeDef;
        public float damageMultiplier = 1.5f;
    }

    public class ModExt_ExtraDamageToRace : DefModExtension
    {
        public List<FleshTypeMultipliers> fleshTypes;
    }
}
