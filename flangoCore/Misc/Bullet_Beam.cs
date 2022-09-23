using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;
using System.Collections.Generic;

namespace flangoCore
{
    public class Bullet_Beam : Bullet
    {
		public List<FleckProps> flecksOnHit;

		protected bool impacted = false;
		private Material mat;

		public override Vector3 ExactPosition => destination.Yto0() + Vector3.up * def.Altitude;

		public override void Draw()
		{
			if (mat == null)
				mat = new Material(def.graphic.MatSingle);
			mat.color = mat.color.ToTransparent(GenMath.InverseParabola(def.graphicData.drawSize.y * ticksToImpact / def.projectile.speed));
			GenDraw.DrawLineBetween(origin, destination, mat, def.graphicData.drawSize.x);
			base.Comps_PostDraw();
		}

		public override void Tick()
		{
			if (!impacted)
			{
				impacted = true;
				Position = DestinationCell;
				ticksToImpact = Mathf.CeilToInt(def.projectile.speed);
				ImpactSomething();
			}
			ticksToImpact--;
			if (!ExactPosition.InBounds(base.Map))
			{
				ticksToImpact++;
				Position = ExactPosition.ToIntVec3();
				base.Destroy();
				return;
			}
			if (ticksToImpact == 60 && Find.TickManager.CurTimeSpeed == TimeSpeed.Normal && def.projectile.soundImpactAnticipate != null)
			{
				def.projectile.soundImpactAnticipate.PlayOneShot(this);
			}
			if (ticksToImpact <= 0)
			{
				if (DestinationCell.InBounds(Map))
				{
					Position = DestinationCell;
				}
				base.Destroy();
			}
		}
		public override void Destroy(DestroyMode mode = DestroyMode.Vanish)
		{
			foreach (FleckProps fleck in flecksOnHit)
			{
				if (Map != null)
				{
					FleckCreationData dataStatic = FleckMaker.GetDataStatic(DrawPos + fleck.offset, Map, fleck.fleckDef, fleck.scaleRange.RandomInRange);
					if (fleck.randomRotation) dataStatic.rotation = Rand.Range(0f, 360f);
					Map.flecks.CreateFleck(dataStatic);
				}
			}
		}
	}
}
