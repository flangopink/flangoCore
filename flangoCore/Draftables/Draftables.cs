using System.Collections.Generic;
using Verse;

namespace flangoCore
{
    public static class Draftables
    {
        public static HashSet<Thing> draftables = new();

        public static void AddDraftable(this Thing thing)
        {
            if (!thing.IsDraftable()) draftables.Add(thing);
        }

        public static void RemoveDraftable(this Thing thing)
        {
            if (!thing.IsDraftable()) draftables.Remove(thing);
        }

        public static bool IsDraftable(this Thing thing) => draftables.Contains(thing);

        public static bool IsDraftableControllable(this Pawn pawn) => IsDraftable(pawn) && pawn.Faction != null && pawn.Faction.IsPlayer && pawn.MentalState == null;
    }
}
