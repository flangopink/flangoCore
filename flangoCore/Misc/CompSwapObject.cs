using Verse;
using System.Collections.Generic;
using UnityEngine;

namespace flangoCore
{
    public class CompProperties_SwapObject : CompProperties
    {
        public ThingDef swapThing;
        public ThingDef swapThingStuff;
        public string commandLabel = "Swap object...";
        public string commandDesc = "Swap current object with another.";
        public string commandIcon = "";

        public CompProperties_SwapObject()
        {
            compClass = typeof(CompSwapObject);
        }

        public override IEnumerable<string> ConfigErrors(ThingDef parentDef)
        {
            foreach (string s in base.ConfigErrors(parentDef))
            {
                yield return s;
            }

            if (swapThing == null)
            {
                yield return parentDef.defName + " with CompSwapObject has null <swapThing>.";
            }
        }
    }

    public class CompSwapObject : ThingComp
    {
        public CompProperties_SwapObject Props => (CompProperties_SwapObject)props;

        public Texture2D Icon => ContentFinder<Texture2D>.Get(Props.commandIcon) ?? BaseContent.BadTex;

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            yield return new Command_Action()
            {
                defaultLabel = Props.commandLabel,
                defaultDesc = Props.commandDesc,
                icon = Icon,
                action = delegate
                {
                    Thing t = ThingMaker.MakeThing(Props.swapThing, Props.swapThingStuff);
                    GenPlace.TryPlaceThing(t, parent.Position, parent.Map, ThingPlaceMode.Direct);
                    Find.Selector.Select(t, false);
                }
            };
        }
    }
}
