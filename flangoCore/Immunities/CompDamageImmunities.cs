using System.Collections.Generic;
using Verse;
using UnityEngine;

namespace flangoCore
{
    public class CompProperties_DamageImmunities : CompProperties
    {
        public List<DamageDef> damageDefs;
        public bool throwText = true;
        public Color textColor = Color.white;
        public float textDuration = 3f;
        public Vector3 textOffset = Vector3.zero;

        public CompProperties_DamageImmunities()
        {
            compClass = typeof(CompDamageImmunities);
        }
    }

    public class CompDamageImmunities : ThingComp
    {
        public CompProperties_DamageImmunities Props => (CompProperties_DamageImmunities)props;
    }
}
