using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using RimWorld;
using Verse;

namespace flangoCore
{
	public class Building_MassGrave : Building_Grave
	{
		public bool CanAcceptCorpses => CorpseCount < FlangoCore.settings.CorpseCapacity;

		public int CorpseCount => innerContainer.Count;

		public int MaxAssignedPawnsCount => Math.Max(1, FlangoCore.settings.CorpseCapacity - CorpseCount);

		public override bool StorageTabVisible => CanAcceptCorpses;

		public override bool Accepts(Thing thing)
		{
			if (!innerContainer.CanAcceptAnyOf(thing) || !GetStoreSettings().AllowedToAccept(thing))
			{
				return false;
			}
			return CanAcceptCorpses;
		}

		public override IEnumerable<Gizmo> GetGizmos()
		{
			Building_MassGrave buildingMassGrave = null;
			IEnumerable<Gizmo> gizmos = buildingMassGrave.GetGizmos();
			foreach (Gizmo gizmo1 in gizmos)
			{
				string str = ((!(gizmo1 is Command_Action commandAction)) ? null : commandAction.defaultLabel);
				if (!(str == "fc_CommandGraveAssignColonistLabel".Translate()))
				{
					yield return gizmo1;
				}
			}
			if (!buildingMassGrave.StorageTabVisible)
			{
				yield break;
			}
			foreach (Gizmo item in StorageSettingsClipboard.CopyPasteGizmosFor(buildingMassGrave.storageSettings))
			{
				yield return item;
			}
		}

		public override string GetInspectString()
		{
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(InspectStringPartsFromComps());
			string[] array = new string[5]
			{
			"fc_MassGrave_Capacity".Translate(),
			": ",
			null,
			null,
			null
			};
			array[2] = CorpseCount.ToString();
			array[3] = "/";
			array[4] = FlangoCore.settings.CorpseCapacity.ToString();
			stringBuilder.Append(string.Concat(array));
			return stringBuilder.ToString();
		}
	}
}
