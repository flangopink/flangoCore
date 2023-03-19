using System.Collections.Generic;
using Verse;

namespace flangoCore
{
    public class CompProperties_Draftable : CompProperties
    {
        public BodyPartTagDef equipmentBodyPartTag;

        public CompProperties_Draftable() 
        {
            compClass = typeof(CompDraftable);
        }

        public override IEnumerable<string> ConfigErrors(ThingDef parentDef)
        {
            foreach (string s in base.ConfigErrors(parentDef)) yield return s;
            if (!FlangoCore.settings.enableDraftables) 
            {
                Log.Error(parentDef.defName + " contains CompDraftable, but Draftables are disabled in flangoCore settings. It will not work if spawned.");
            }
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
