using RimWorld;
using Verse;
using UnityEngine;

namespace flangoCore
{
    public class FleckProps 
    {
        public FleckDef fleckDef;
        public int intervalTicks = 1;
        public FloatRange scaleRange = FloatRange.One;
        public Vector3 offset = Vector3.zero;
        public bool randomRotation;
    }
}
