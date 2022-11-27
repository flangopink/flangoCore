using Verse;
using UnityEngine;

namespace flangoCore
{
    public class Graphic_Animated : Graphic_Collection 
    {
        GraphicData_Animated Data;
        public bool IsEnabled { get; set; }
        public int firstGraphicInt;
        public int currentGraphicInt;

        public override void Init(GraphicRequest req)
        {
            base.Init(req);
            Data = new GraphicData_Animated(data);
            IsEnabled = !Data.offByDefault;
            currentGraphicInt = firstGraphicInt = Data.skipFirst ? 1 : 0;
        }

        private Graphic CurrentGraphic
        {
            get
            {
                if (IsEnabled && subGraphics.Length > 1)
                {
                    if (currentGraphicInt > subGraphics.Length - 1) currentGraphicInt = firstGraphicInt;

                    if (Find.TickManager.TicksGame % Data.intervalTicks + (Data.intervalRandomOffset == 0 ? 0 : Rand.Range(-Data.intervalRandomOffset, Data.intervalRandomOffset)) == 0)
                    {
                        Graphic g;
                        if (Data.random)
                        {
                            int r;
                            do r = Rand.Range(Data.skipFirst ? 1 : 0, subGraphics.Length - 1);
                            while (Data.noRepeats && currentGraphicInt == r);
                            currentGraphicInt = r;
                            g = subGraphics[r];
                        }
                        else
                        {
                            g = subGraphics[currentGraphicInt];
                            currentGraphicInt++;
                        }
                        return g;
                    }
                }
                return subGraphics[0];
            }
        }

        public override void DrawWorker(Vector3 loc, Rot4 rot, ThingDef thingDef, Thing thing, float extraRotation)
        {
            CurrentGraphic?.DrawWorker(loc, rot, thingDef, thing, extraRotation);
        }

        public override Graphic GetColoredVersion(Shader newShader, Color newColor, Color newColorTwo)
        {
            return CurrentGraphic;
        }
    }

    public class GraphicData_Animated : GraphicData
    {
        public int intervalTicks = 30;
        public int intervalRandomOffset = 0;
        public bool random;
        public bool noRepeats;
        public bool offByDefault;
        public bool skipFirst;

        public GraphicData graphicData;
        public GraphicData_Animated() { }
        public GraphicData_Animated(GraphicData gd) 
        {
            graphicData = gd;
            graphicClass = typeof(Graphic_Animated); 
        }
    }
}
