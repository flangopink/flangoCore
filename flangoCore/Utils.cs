using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using UnityEngine;
using Verse.AI;

namespace flangoCore
{
    public enum ExplosionShape
    {
        Normal,
        Star,
        CrossPlus,
        CrossX,
        Ring,
        RandomAdjacent
    }

    public static class Utils
    {
        private const float R = 0.9f;
        public static readonly Color DefaultRingColor = new(0.8f, 0.49f, 0.43f);

        public static bool InTheSameRoom(IntVec3 locA, IntVec3 locB, Map map) => locA.GetRoom(map) is Room room && (room == null || room == locB.GetRoom(map));

        #region - Explosions -

        // Main
        public static void DoExplosionOnCell(IntVec3 cell, Map map, float radius = 0.9f, DamageDef damageDef = null, Thing instigator = null, int damage = -1, float armorPen = -1, SoundDef soundDef = null, ThingDef weapon = null, ThingDef projectile = null, Thing intendedTarget = null, ThingDef preExpSpawnThingDef = null, float preExpSpawnChance = 0, int preExpSpawnThingCount = 0, ThingDef postExpSpawnThingDef = null, float postExpSpawnChance = 0, int postExpSpawnThingCount = 0, bool applyDamageToExpCellsNeighbors = false, float chanceToStartFire = 0, bool damageFalloff = false, float? direction = null)
        {
            damageDef ??= DamageDefOf.Bomb;
            GenExplosion.DoExplosion(cell, map, radius, damageDef, instigator, damage, armorPen, soundDef, weapon, projectile, intendedTarget, postExpSpawnThingDef, postExpSpawnChance, postExpSpawnThingCount, null, applyDamageToExpCellsNeighbors, preExpSpawnThingDef, preExpSpawnChance, preExpSpawnThingCount, chanceToStartFire, damageFalloff, direction);
        }

        public static void DoExplosion(this Thing t, float radius = 0.9f, ExplosionShape shape = 0, DamageDef damageDef = null, Thing instigator = null, int damage = -1, float armorPen = -1, SoundDef soundDef = null, ThingDef weapon = null, ThingDef projectile = null, Thing intendedTarget = null, ThingDef preExpSpawnThingDef = null, float preExpSpawnChance = 0, int preExpSpawnThingCount = 0, ThingDef postExpSpawnThingDef = null, float postExpSpawnChance = 0, int postExpSpawnThingCount = 0, bool applyDamageToExpCellsNeighbors = false, float chanceToStartFire = 0, bool damageFalloff = false, float? direction = null)
        {
            IntVec3 pos = t.Position;
            Map map = t.Map;

            damageDef ??= DamageDefOf.Bomb;
            instigator ??= t;

            void Kaboom(IntVec3 c, float r = 0.9f) => DoExplosionOnCell(c, map, r, damageDef, instigator, damage, armorPen, soundDef, weapon, projectile, intendedTarget, preExpSpawnThingDef, preExpSpawnChance, preExpSpawnThingCount, postExpSpawnThingDef, postExpSpawnChance, postExpSpawnThingCount, applyDamageToExpCellsNeighbors, chanceToStartFire, damageFalloff, direction);

            IntVec3 offset;
            switch (shape)
            {
                case ExplosionShape.Normal:
                    Kaboom(pos, radius);
                    break;

                case ExplosionShape.Star:
                    Kaboom(pos, R);
                    foreach (IntVec3 cell in GenRadial.RadialCellsAround(pos, radius, false))
                    {
                        offset = cell - pos;
                        if (Mathf.Abs(offset.x) == Mathf.Abs(offset.z)
                            || (offset.x == 0 && Mathf.Abs(offset.z) != 1)
                            || (offset.z == 0 && Mathf.Abs(offset.x) != 1))
                        {
                            if (cell.InBounds(map)) Kaboom(cell, R);
                        }
                        else continue;
                    }
                    break;

                case ExplosionShape.CrossPlus:
                    Kaboom(pos, R);
                    foreach (IntVec3 cell in GenRadial.RadialCellsAround(pos, radius, false))
                    {
                        offset = cell - pos;
                        if ((offset.x == 0 && offset.z != 0) || (offset.z == 0 && offset.x != 0))
                        {
                            if (cell.InBounds(map)) Kaboom(cell, R);
                        }
                        else continue;
                    }
                    break;

                case ExplosionShape.CrossX:
                    Kaboom(pos, R);
                    foreach (IntVec3 cell in GenRadial.RadialCellsAround(pos, radius, false))
                    {
                        offset = cell - pos;
                        if (Mathf.Abs(offset.x) == Mathf.Abs(offset.z))
                        {
                            if (cell.InBounds(map)) Kaboom(cell, R);
                        }
                        else continue;
                    }
                    break;

                case ExplosionShape.Ring:
                    foreach (IntVec3 cell in GenRadial.RadialCellsAround(pos, radius, false))
                    {
                        if (cell.InBounds(map)) Kaboom(cell, R);
                    }
                    break;

                case ExplosionShape.RandomAdjacent:
                    Kaboom(pos, R);
                    IntVec3 cellRandom = GenRadial.RadialCellsAround(pos, radius, false).RandomElement();
                    if (cellRandom.InBounds(map)) Kaboom(cellRandom, R);
                    break;
            }
        }

        // Projectiles
        public static void DoExplosion(this Projectile p)
        {
            ThingDef def = p.def;
            p.DoExplosionOnCell(def.projectile.explosionRadius);
        }

        public static void DoExplosionOnCell(this Projectile p, float radius = 0.9f, ExplosionShape shape = 0)
        {
            ThingDef def = p.def;
            p.DoExplosion(radius, shape, def.projectile.damageDef, p.launcher, p.DamageAmount, p.ArmorPenetration, def.projectile.soundExplode, p.equipmentDef, def, p.intendedTarget.Thing, def.projectile.preExplosionSpawnThingDef, def.projectile.preExplosionSpawnChance, def.projectile.preExplosionSpawnThingCount, def.projectile.postExplosionSpawnThingDef, def.projectile.postExplosionSpawnChance, def.projectile.postExplosionSpawnThingCount, def.projectile.applyDamageToExplosionCellsNeighbors, def.projectile.explosionChanceToStartFire, def.projectile.explosionDamageFalloff, p.origin.AngleToFlat(p.destination));
        }

        #endregion

        // Thank you smartkar!
        // Whoever reads this should check out his Athena framework:
        // https://github.com/SmArtKar/AthenaFramework
        public static List<Pawn> GetPawnsInRange(IntVec3 cell, Map map, float maxRange, bool requireLOS = false, bool affectDowned = false)
        {
            List<Pawn> list = new();
            float range = maxRange * maxRange;
            foreach (Pawn pawn in map.mapPawns.AllPawnsSpawned)
            {
                if (pawn.Spawned && (!pawn.Dead || (affectDowned && pawn.Downed)))
                {
                    float pawnDist = pawn.Position.DistanceToSquared(cell);
                    if (pawnDist <= range)
                    {
                        if (requireLOS && !GenSight.LineOfSight(cell, pawn.Position, map)) continue;
                        list.Add(pawn);
                    }
                }
            }
            return list;
        }
        public static List<Thing> GetThingsInRange(IntVec3 cell, Map map, float maxRange, bool requireLOS = false)
        {
            List<Thing> list = new();
            float range = maxRange * maxRange;
            foreach (Thing t in map.spawnedThings)
            {
                if (!t.Destroyed)
                {
                    float pawnDist = t.Position.DistanceToSquared(cell);
                    if (pawnDist <= range) 
                    {
                        if (requireLOS && !GenSight.LineOfSight(cell, t.Position, map)) continue;
                        list.Add(t); 
                    }
                }
            }
            return list;
        }
        public static List<ThingDef> GetThingDefsInRange(IntVec3 cell, Map map, float maxRange, bool requireLOS = false)
        {
            return GetThingsInRange(cell, map, maxRange, requireLOS).Select(x => x.def).ToList();
        }

        public static bool HasComp(this List<CompProperties> defComps, CompProperties comp) => defComps.Contains(comp);
        public static bool HasComp(this ThingDef def, CompProperties comp) => def.comps.Contains(comp);
        public static bool HasComp<T>(this List<CompProperties> defComps) => defComps.OfType<T>().Any();
        public static bool HasComp<T>(this ThingDef def) => def.comps.OfType<T>().Any();
        public static bool HasComp<T>(this HediffDef def) => def.comps.OfType<T>().Any();
      
        public static T RandomByWeight<T>(this Dictionary<T, float> dict)
        {
            float sum = 0;
            float r = Rand.Range(0, dict.Sum(x => x.Value));
            foreach (var item in dict)
            {
                if (r < (sum + item.Value)) return item.Key;
                else sum += item.Value;
            }
            return dict.LastOrDefault().Key;
        }

        public static bool HasBadThingNearby(IntVec3 pos, Map map, float radius, List<ThingDef> other, out List<Thing> result)
        {
            var things = GetThingsInRange(pos, map, radius);
            result = new();
            Thing t;
            for (int i = 0; i < things.Count; i++)
            {
                t = things[i];
                if (t.Position == pos) continue;
                if (other.Contains(t.def)) result.Add(t);
            }
            return result.Count > 0;
        }

        public static void DrawThingOverlayRadius(IntVec3 pos, ThingDef def, float radius, List<Thing> other, SimpleColor lineColor = SimpleColor.Red, Color? ringColor = null)
        {
            if (ringColor != null) GenDraw.DrawRadiusRing(pos, radius, (Color)ringColor);
            int num = 0;
            foreach (Thing item in other)
            {
                if (num++ > 10) break;
                GenDraw.DrawLineBetween(GenThing.TrueCenter(pos, Rot4.North, def.size, def.Altitude), item.TrueCenter(), lineColor);
            }
        }

        /*
        public static List<T> ListMatches<T>(this List<T> mainList, List<T> subList)
        {
            List<T> result = new();
            for (int i = 0; i < mainList.Count; i++)
            {
                for (int j = 0; j < subList.Count; j++)
                {
                    if (mainList[i].Equals(subList[j]))
                    {
                        result.Add(mainList[i]);
                        continue;
                    }
                }
            }
            return result;
        }

        public static List<T> EnumerableMatches<T>(this IEnumerable<T> mainList, IEnumerable<T> subList)
        {
            List<T> result = new();
            foreach (T i in mainList)
            {
                foreach (T j in subList)
                {
                    if (i.Equals(j))
                    {
                        result.Add(i);
                        continue;
                    }
                }
            }
            return result;
        }
        */
    }
}
