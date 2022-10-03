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

    public static class FleckPropsExt
    {
        public static void MakeFleck(this FleckProps fleck, Map map, Vector3 drawPos)
        {
            FleckCreationData dataStatic = FleckMaker.GetDataStatic(drawPos + fleck.offset, map, fleck.fleckDef, fleck.scaleRange.RandomInRange);
            if (fleck.randomRotation) dataStatic.rotation = Rand.Range(0f, 360f);
            map.flecks.CreateFleck(dataStatic);
        }
    }
}
