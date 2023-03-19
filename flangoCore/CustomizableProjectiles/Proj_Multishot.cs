using System.Collections.Generic;
using Verse;
using RimWorld;
using UnityEngine;
using HotSwap;

namespace flangoCore
{
    public class ModExt_ProjectileMultishot : DefModExtension
    {
        public ThingDef shootThing;
        public IntRange shootCount = IntRange.one;
        public float offsetRange;
        public int splitTimerTicks;
        public bool sideBySide;
    }

    [HotSwappable]
    public class Projectile_Multishot : Projectile
    {
        public ModExt_ProjectileMultishot Ext => def.GetModExtension<ModExt_ProjectileMultishot>();
        private Thing Equipment => Launcher is Pawn p ? p.equipment.Primary : null;

        private int splitTicksLeft = -1;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look(ref splitTicksLeft, "splitTicksLeft", -1);
        }

        public override void Tick()
        {
            base.Tick();
            if (splitTicksLeft < Ext.splitTimerTicks)
            {
                splitTicksLeft++;
                if (splitTicksLeft >= Ext.splitTimerTicks)
                {
                    Split();
                    Destroy();
                }
            }
        }

        private void Split()
        {
            var o = origin;
            int shots = Ext.shootCount.RandomInRange;
            for (int i = 0; i < shots; i++)
            {
                origin = o; // So every shot doesn't override the origin
                if (Ext.sideBySide)
                {
                    float offsetDistance = 0.5f;
                    Quaternion launchDirection = Quaternion.LookRotation((destination - origin).Yto0());
                    if (i % 2 == 0)
                    {
                        Vector3 leftOffset = new(-offsetDistance, 0f, 0f);
                        origin += (launchDirection * leftOffset);
                    }
                    else
                    {
                        Vector3 rightOffset = new(offsetDistance, 0f, 0f);
                        origin += (launchDirection * rightOffset);
                    }
                }
                var pos = origin + Gen.RandomHorizontalVector(Ext.offsetRange);
                Projectile proj = (Projectile)GenSpawn.Spawn(Ext.shootThing, Launcher.Position, Map);
                proj.Launch(Launcher, pos, usedTarget, intendedTarget, ProjectileHitFlags.IntendedTarget, false, Equipment);
            }
        }
    }
}
