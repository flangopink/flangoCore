using RimWorld;
using System.Collections.Generic;
using Verse;
using System.Linq;

namespace flangoCore
{
    public class DrillOutputThing
    {
        public ThingDef thingDef;
        public ThingDef stuff;
        public IntRange count = IntRange.one;
        public bool minifyDrill;
        public bool destroyDrill;
        public bool spawnAtDrillCell;
    }

    public class DrillOutput
    {
        public DrillOutputThing thing;
        public float weight;
    }

    public class CompProperties_CustomDrill : CompProperties
    {
        public List<DrillOutput> outputs;
        public List<ThingDef> cantDrillNearThings;
        public float cantDrillNearRadius;
        public float baseWorkAmount = 10000f;
        //public bool minifyWhenFinished;
        //public bool destroyWhenFinished;
        public CompProperties_CustomDrill() => compClass = typeof(CompCustomDrill);
    }

    public class CompCustomDrill : ThingComp
    {
        public CompProperties_CustomDrill Props => (CompProperties_CustomDrill)props;

        private readonly Dictionary<DrillOutputThing, float> outputs = new();

        private List<Thing> badThings = new();

        private CompPowerTrader powerComp;

        private float portionProgress;

        private int lastUsedTick = -99999;

        //private const float WorkPerPortionBase = 10000f;

        public float ProgressToNextPortionPercent => portionProgress / Props.baseWorkAmount;

        public bool cantDrillFlag;

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            powerComp = parent.TryGetComp<CompPowerTrader>();

            if (outputs.NullOrEmpty())
            {
                foreach (DrillOutput d in Props.outputs)
                {
                    outputs.Add(d.thing, d.weight);
                }
            }
        }

        public override void PostExposeData()
        {
            Scribe_Values.Look(ref portionProgress, "portionProgress", 0f);
            Scribe_Values.Look(ref lastUsedTick, "lastUsedTick", 0);
        }

        public void DrillWorkDone(Pawn driller)
        {
            float statValue = driller.GetStatValue(StatDefOf.DeepDrillingSpeed);
            portionProgress += statValue;
            lastUsedTick = Find.TickManager.TicksGame;
            if (portionProgress > Props.baseWorkAmount)
            {
                TryProducePortion(driller);
                portionProgress = 0f;
            }
        }

        public override void PostDeSpawn(Map map)
        {
            portionProgress = 0f;
            lastUsedTick = -99999;
        }

        private void TryProducePortion(Pawn driller = null)
        {
            var output = outputs.RandomByWeight();
            Thing thing = ThingMaker.MakeThing(output.thingDef, output.stuff);
            thing.stackCount = output.count.RandomInRange;

            IntVec3 ppos = parent.Position;
            IntVec3 pint = parent.InteractionCell;
            IntVec3 pos = output.spawnAtDrillCell ? ppos : pint;
            Map map = parent.Map;
            ThingPlaceMode tpm = output.spawnAtDrillCell ? ThingPlaceMode.Direct : ThingPlaceMode.Near;

            if (output.minifyDrill)
            {
                var m = parent.TryMakeMinified();
                GenPlace.TryPlaceThing(m, pos, map, ThingPlaceMode.Near);
            }
            else if (output.destroyDrill) parent.Destroy();

            GenPlace.TryPlaceThing(thing, pos, map, tpm, null, (IntVec3 p) => p != ppos && p != pint);
            
            if (driller != null)
            {
                Find.HistoryEventsManager.RecordEvent(new HistoryEvent(HistoryEventDefOf.Mined, driller.Named(HistoryEventArgsNames.Doer)));
            }
            if (DeepDrillUtility.GetBaseResource(map, ppos) == null)
            {
                Messages.Message("DeepDrillExhaustedNoFallback".Translate(), parent, MessageTypeDefOf.TaskCompletion);
                return;
            }
            //Messages.Message("DeepDrillExhausted".Translate(Find.ActiveLanguageWorker.Pluralize(DeepDrillUtility.GetBaseResource(map, ppos).label)), parent, MessageTypeDefOf.TaskCompletion);

            if (parent.Spawned || parent.Destroyed || parent is MinifiedThing) return;

            /*if (Props.minifyWhenFinished)
            {
                var m = parent.TryMakeMinified();
                GenPlace.TryPlaceThing(m, pos, map, ThingPlaceMode.Near);
            }
            else if (Props.destroyWhenFinished) parent.Destroy();*/
        }

        private List<ThingDef> GetListOfResources()
        {
            var olist = outputs.Keys.ToList();
            List<ThingDef> dlist = new();
            for (int i = 0; i < olist.Count; i++) dlist.Add(olist[i].thingDef);
            return dlist;
        }

        private string GetListOfResourcesLabels()
        {
            return string.Join(", ", GetListOfResources().Select(c => c.LabelCap).ToArray());
        }

        public bool CanDrillNow()
        {
            if (powerComp != null && !powerComp.PowerOn) return false;
            if (cantDrillFlag) return false;
            if (DeepDrillUtility.GetBaseResource(parent.Map, parent.Position) != null) return true;
            return true;
        }

        public bool UsedLastTick() => lastUsedTick >= Find.TickManager.TicksGame - 1;

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            foreach (Gizmo item in base.CompGetGizmosExtra())
            {
                yield return item;
            }
            if (DebugSettings.ShowDevGizmos)
            {
                yield return new Command_Action()
                {
                    defaultLabel = "DEV: Produce portion",
                    action = delegate { TryProducePortion(); }
                };
            }
        }

        public override void CompTickRare()
        {
            if (parent.Spawned)
            {
                var things = Props.cantDrillNearThings;
                if (!things.NullOrEmpty() && Utils.HasThingsNearby(parent.Position, parent.Map, Props.cantDrillNearRadius, things, out var other))
                {
                    badThings = other;
                    cantDrillFlag = true;
                }
                else
                {
                    badThings.Clear();
                    cantDrillFlag = false;
                }
            }
        }

        public override void PostDrawExtraSelectionOverlays()
        {
            if (cantDrillFlag && Find.Selector.SingleSelectedThing == parent)
            {
                Utils.DrawThingOverlayRadius(parent.Position, parent.def, Props.cantDrillNearRadius, badThings, SimpleColor.Red, Utils.DefaultRingColor);
            }
        }

        public override string CompInspectStringExtra()
        {
            if (parent.Spawned)
            {
                return "ResourceBelow".Translate() + ": " + GetListOfResourcesLabels() + "\n"
                    + (cantDrillFlag
                        ? "fc_cantDrillNearSomeObjects".Translate(string.Join(", ", badThings.Select(c => c.LabelCap).ToArray()))
                        : "ProgressToNextPortion".Translate() + ": " + ProgressToNextPortionPercent.ToStringPercent("F0"));
            }
            return null;
        }
    }
}
