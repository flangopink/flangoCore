using System.Collections.Generic;
using Verse;

namespace flangoCore
{
    public class ExtraWithChance
    {
        /*public DamageDef DamageDef;
        public float Damage = 10;
        public float ArmorPenetration = -1;*/

        public float addExtraChance = 0.5f;

        public ExtraDamage extraDamage;
        // <ExtraDamage>
        //   <def>Burn</def>
        //   <amount>10</amount>
        //   <armorPenetration>-1</armorPenetration>
        // </ExtraDamage>

        public bool setOnFire;
        public float fireSize = 0.5f;

        public bool ignoreShields = true;
        public List<ThingDef> affectedShields;
    }
}
