using System.Collections.Generic;
using UnityEngine;

namespace flangoCore
{
	public class SkillTreeLevel
	{
		public List<SkillDef> skills;

		public string label;
		public string iconPath;

		public Texture2D icon;

		public float xpToUnlock = -1; // -1 only prereqs, 0 always available, ...?
	}
}
