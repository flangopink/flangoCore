using System;

namespace flangoCore
{
    [Flags]
    public enum XPSourceFlags
    {
        None = 0,
        Kills = 1,
        OverTime = 2,
        UsingAbilities = 4,
        TargetsAffectedByAbilities = 8,
        Work = 16,
        WorkDone = 32
    }
}
