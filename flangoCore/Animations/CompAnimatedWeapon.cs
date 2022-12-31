using Verse;
using System.Collections.Generic;
using UnityEngine;
using HotSwap;

namespace flangoCore
{
    public class AnimProps
    {
        public int length;
        public float startRot;
        public float endRot;
        public Vector3 startOffset = Vector3.zero;
        public Vector3 endOffset = Vector3.zero;
        public bool pause;
    }

    public class CompProperties_AnimatedWeapon : CompProperties
    {
        public List<AnimProps> animProps;
        public bool loop;

        public CompProperties_AnimatedWeapon() => compClass = typeof(CompAnimatedWeapon);
    }

    [HotSwappable]
    public class CompAnimatedWeapon : ThingComp
    {
        public CompProperties_AnimatedWeapon Props => (CompProperties_AnimatedWeapon)props;

        public int currentAnim;
        public int ticksLeft;
        public float rot;
        public Vector3 offset;
        public bool active;

        public int animCount;
        public List<AnimProps> anims;

        public float rotPerTick;
        public Vector3 offsetPerTick;

        /*public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            anims = Props.animProps;
            animCount = anims.Count;
            if (animCount != 0 && currentAnim == 0) SetAnim();
        }

        public override void CompTick()
        {
            if (!active || ParentHolder == null || animCount == 0) return;
            if (ticksLeft == 0) NextAnim();

            rot += rotPerTick;
            offset += offsetPerTick;

            Log.Message(rot.ToString());
            Log.Message(offset.ToString());

            ticksLeft--;
        }*/

        public void Init()
        {
            anims = Props.animProps;
            animCount = anims.Count;
            if (animCount != 0 && currentAnim == 0) SetAnim();
        }

        public void TickAnim()
        {
            if (!active || animCount == 0) return;
            if (ticksLeft == 0) NextAnim();

            if (anims[currentAnim].pause)
            {
                rot = rotPerTick;
                offset = offsetPerTick;
            }
            else
            {
                rot += rotPerTick;
                offset += offsetPerTick;
            }
            ticksLeft--;
        }

        // TODO: Easing, Fix looping, Flecks, Random anims, Combo anims

        public void NextAnim()
        {
            currentAnim++;
            if (currentAnim == animCount)
            {
                if (!Props.loop) active = false;
                currentAnim = 0;
            }
            SetAnim();
        }

        public void SetAnim()
        {
            var anim = anims[currentAnim];
            ticksLeft = anim.length;
            rot = anim.startRot;
            offset = anim.startOffset;
            rotPerTick = anim.pause ? rot : (anim.endRot - rot) / ticksLeft;
            offsetPerTick = anim.pause ? offset : (anim.endOffset - offset) / ticksLeft;
        }

        public override void PostExposeData()
        {
            Scribe_Values.Look(ref currentAnim, "currentAnim");
            Scribe_Values.Look(ref ticksLeft, "ticksLeft");
            Scribe_Values.Look(ref active, "active");
            Scribe_Values.Look(ref rot, "rot");
            Scribe_Values.Look(ref offset, "offset");
            Scribe_Values.Look(ref rotPerTick, "rotPerTick");
            Scribe_Values.Look(ref offsetPerTick, "offsetPerTick");
        }
    }
}
