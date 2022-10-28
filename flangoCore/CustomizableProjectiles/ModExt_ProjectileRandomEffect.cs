using Verse;
using System.Collections.Generic;

namespace flangoCore
{
    public class ExtraWithChance
    {
        public float chance = 0.5f;
        public ExtraDamage extraDamage;

        public bool setOnFire;
        public float fireSize = 0.5f;

        public bool ignoreShields = true;
        public List<ThingDef> affectedShields;
    }

    public class HediffWithChance
    {
        public HediffDef hediff;
        public float chance = 0.5f;
        public float severity = 1f;
        public bool addToSelf;
    }

    public class APWithChance
    {
        public float chance = 0.5f; // 50% chance
        public float value = 0.2f; // 20% value
    }

    public class ModExt_ProjectileRandomEffect : DefModExtension
    {
        public List<ExtraWithChance> extras;
        public List<HediffWithChance> hediffs;
        public List<APWithChance> APs;
        public bool sharedChance;
        public bool defaultToZeroAP;
    }
}
