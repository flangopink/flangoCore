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
        public float rotationRate;
        public FloatRange velocityAngle;
        public FloatRange velocitySpeed;
        public int ageTicksOverride = -1;
        public float? solidTimeOverride = null;
        public Color? color;
        public Color? colorMin;
        public Color? colorMax;
    }

    public static class FleckPropsExt
    {
        public static void MakeFleck(this FleckProps fleck, Map map, Vector3 posVector3)
        {
            map.flecks.CreateFleck(GetDataStatic(posVector3 + fleck.offset, fleck));
        }

        public static void MakeFleck(this FleckProps fleck, Map map, IntVec3 posIntVec3)
        {
            MakeFleck(fleck, map, posIntVec3.ToVector3());
        }

        public static FleckCreationData GetDataStatic(Vector3 loc, FleckProps fleck)
        {
            Color? cmin = fleck.colorMin;
            Color? cmax = fleck.colorMax;
            return new FleckCreationData
            {
                spawnPosition = loc,
                def = fleck.fleckDef,
                rotation = fleck.randomRotation ? Rand.Range(0f, 360f) : 0,
                rotationRate = fleck.rotationRate,
                scale = fleck.scaleRange.RandomInRange,
                ageTicksOverride = fleck.ageTicksOverride,
                solidTimeOverride = fleck.solidTimeOverride,
                velocityAngle = fleck.velocityAngle.RandomInRange,
                velocitySpeed = fleck.velocitySpeed.RandomInRange,
                instanceColor = (fleck.color == null && cmin != null && cmax != null) ? new Color(Rand.Range(cmin.Value.r, cmax.Value.r), Rand.Range(cmin.Value.g, cmax.Value.g), Rand.Range(cmin.Value.b, cmax.Value.b)) : Color.white
            };
        }
    }
}
