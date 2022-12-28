using Verse;

namespace flangoCore
{
    public class CompProperties_Draftable : CompProperties
    {
        public BodyPartTagDef equipmentBodyPartTag;

        public CompProperties_Draftable() 
        {
            compClass = typeof(CompProperties_Draftable);
        }
    }

    public class CompDraftable : ThingComp
    {
        public CompProperties_Draftable Props => (CompProperties_Draftable)props;

        public override void PostSpawnSetup(bool respawningAfterLoad) => parent.AddDraftable();

        public override void PostDeSpawn(Map map) => parent.RemoveDraftable();

        public override void PostDestroy(DestroyMode mode, Map previousMap) => parent.RemoveDraftable();
    }
}
