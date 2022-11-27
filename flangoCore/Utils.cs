using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Verse;
using UnityEngine;

namespace flangoCore
{
    public class Utils
    {
        public static bool InTheSameRoom(IntVec3 locA, IntVec3 locB, Map map) => locA.GetRoom(map) is Room room && (room == null || room == locB.GetRoom(map));
        
    }
}
