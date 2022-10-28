using System;
using Verse;

namespace flangoCore
{
    public class ModExt_ProjectileExplosiveShaped : DefModExtension
    {
        public ExplosionShape shape = 0;
	}
	public enum ExplosionShape
	{
		Normal,
		Star,
		CrossPlus,
		CrossX,
		Ring,
		RandomAdjacent
	}
}