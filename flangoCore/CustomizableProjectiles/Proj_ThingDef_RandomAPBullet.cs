using RimWorld;
using Verse;
using System.Collections.Generic;

namespace flangoCore
{
    public class ThingDef_RandomAPBullet : ThingDef
    {
        public List<APWithChance> AP;
    }
    public class Projectile_RandomAPBullet : Bullet
    {
        public ThingDef_RandomAPBullet Def => (ThingDef_RandomAPBullet)def;
    }
}
