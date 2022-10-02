using Verse;
using System.Collections.Generic;

namespace flangoCore
{
    public class ModExt_DeflectProjectiles : DefModExtension
    {
        public float deflectChance = 0.4f;
        public float deflectAccuracy = 0.4f;
        public List<ThingDef> cantDeflect;
        public SoundDef deflectSound;
        public FleckDef deflectFleck;
        public bool fleckRandomRotation;
    }
}
