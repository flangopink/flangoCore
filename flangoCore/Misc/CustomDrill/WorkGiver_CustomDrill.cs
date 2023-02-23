using RimWorld;
using System.Collections.Generic;
using Verse.AI;
using Verse;

namespace flangoCore
{
    public class WorkGiver_CustomDrill : WorkGiver_Scanner
    {
        public override ThingRequest PotentialWorkThingRequest => ThingRequest.ForGroup(ThingRequestGroup.PowerTrader);

        public override PathEndMode PathEndMode => PathEndMode.InteractionCell;

        public override Danger MaxPathDanger(Pawn pawn)
        {
            return Danger.Deadly;
        }

        public override bool ShouldSkip(Pawn pawn, bool forced = false)
        {
            List<Building> allBuildingsColonist = pawn.Map.listerBuildings.allBuildingsColonist;
            for (int i = 0; i < allBuildingsColonist.Count; i++)
            {
                Building building = allBuildingsColonist[i];
                if (building.TryGetComp<CompCustomDrill>() != null)
                {
                    CompPowerTrader comp = building.GetComp<CompPowerTrader>();
                    if ((comp == null || comp.PowerOn) && building.Map.designationManager.DesignationOn(building, DesignationDefOf.Uninstall) == null)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public override bool HasJobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            if (t.Faction != pawn.Faction) return false; 
            if (t is not Building building) return false;
            if (building.IsForbidden(pawn)) return false;
            if (!pawn.CanReserve(building, 1, -1, null, forced)) return false;
            var comp = building.TryGetComp<CompCustomDrill>();
            if (comp == null || (comp != null && !comp.CanDrillNow())) return false;
            if (building.Map.designationManager.DesignationOn(building, DesignationDefOf.Uninstall) != null) return false;
            if (building.IsBurning()) return false;
            return true;
        }

        public override Job JobOnThing(Pawn pawn, Thing t, bool forced = false)
        {
            return JobMaker.MakeJob(DefOf_flangoCore.OperateCustomDrill, t, 1500, checkOverrideOnExpiry: true);
        }
    }
}
