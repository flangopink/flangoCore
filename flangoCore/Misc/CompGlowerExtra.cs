using System.Collections.Generic;
using RimWorld;
using Verse;
using UnityEngine;
using Verse.Noise;

namespace flangoCore
{
    public class CompProperties_GlowerExtra : CompProperties_Glower
    {
        public List<Color> colors;
        public int intervalTicks = 60;
        public FleckProps fleck;
        public bool random;
        public Color? colorMin;
        public Color? colorMax;

        public CompProperties_GlowerExtra()
        {
            compClass = typeof(CompGlowerExtra);
        }
    }

    public class CompGlowerExtra : CompGlower // does not work.
    {
        public new CompProperties_GlowerExtra Props => (CompProperties_GlowerExtra)props;

        private int curColorInt;
        private CompPowerTrader power;
        private CompFlickable flickable;

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            base.PostSpawnSetup(respawningAfterLoad);
            power = parent.TryGetComp<CompPowerTrader>();
            flickable = parent.TryGetComp<CompFlickable>();
        }

        public override void CompTick()
        {
            if (parent.Map != null && Find.TickManager.TicksGame % Props.intervalTicks == 0)
            {
                Props.fleck?.MakeFleck(parent.Map, parent.DrawPos);

                if (Props.colors.NullOrEmpty() || !power.PowerOn || !flickable.SwitchIsOn) return;

                if (Props.random)
                {
                    Color col = Props.colors.RandomElement();
                    GlowColor = new ColorInt((int)col.r, (int)col.g, (int)col.b, 0);
                }
                else if (Props.colorMin != null && Props.colorMax != null)
                {
                    Color? cmin = Props.colorMin;
                    Color? cmax = Props.colorMax;
                    GlowColor = new ColorInt((int)Rand.Range(cmin.Value.r, cmax.Value.r), (int)Rand.Range(cmin.Value.g, cmax.Value.g), (int)Rand.Range(cmin.Value.b, cmax.Value.b), 0);
                }
                else
                {
                    Color col = Props.colors[curColorInt];
                    GlowColor = new ColorInt((int)col.r, (int)col.g, (int)col.b, 0);
                    curColorInt++;
                    if (curColorInt == Props.colors.Count) curColorInt = 0;
                }
            }
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref curColorInt, "curColorInt");
        }
    }
}
