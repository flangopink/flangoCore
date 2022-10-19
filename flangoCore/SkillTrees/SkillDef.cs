using RimWorld;
using Verse;
using System.Collections.Generic;
using UnityEngine;

namespace flangoCore
{
    public class SkillDef : Def
    {
        public string iconPath;
        public Texture2D icon;
		public List<SkillDef> prerequisites;
        public List<AbilityDef> abilities;
        public List<HediffDef> hediffs;
		public int reqLevel = 0;
		public float cost = 1;
		public bool spaceAfter; // whatever that means
		public override void PostLoad()
		{
			base.PostLoad();
			LongEventHandler.ExecuteWhenFinished(delegate
			{
				if (!iconPath.NullOrEmpty())
				{
					icon = ContentFinder<Texture2D>.Get(iconPath);
				}
			});
		}
	}
}
