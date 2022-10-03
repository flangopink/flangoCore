using UnityEngine;
using Verse;
using Verse.Sound;

namespace flangoCore
{
	[StaticConstructorOnStartup]
	public class DashingPawn : AbilityPawnFlyer
	{
		public bool rope;
		private static readonly string RopeTexPath = "UI/Overlays/Rope";
		private static readonly Material RopeLineMat = MaterialPool.MatFrom(RopeTexPath, ShaderDatabase.Transparent, GenColor.FromBytes(99, 70, 41));

		public override void DrawAt(Vector3 drawLoc, bool flip = false)
		{
			FlyingPawn.DrawAt(GetDrawPos(), flip);

			if (rope && position != Vector3.zero) GenDraw.DrawLineBetween(position, target, AltitudeLayer.PawnRope.AltitudeFor(), RopeLineMat);
		}

		public override void Tick()
		{
			base.Tick();

			if (MapHeld != null && !comp.onDashingFlecks.NullOrEmpty())
			{
				foreach (FleckProps fleck in comp.onDashingFlecks)
				{
					if (Find.TickManager.TicksGame % fleck.intervalTicks == 0)
					{
						fleck.MakeFleck(MapHeld, GetDrawPos());
					}
				}
			}
		}

		private Vector3 GetDrawPos()
		{
			float num = ticksFlying / ticksFlightTime;
			Vector3 vector = position;

			return vector + Vector3.forward * (num - Mathf.Pow(num, 2f)) * comp.altitudeMultiplier;
		}

		protected override void RespawnPawn()
		{
			Pawn dashingPawn = FlyingPawn;
			if (dashingPawn.drafter != null)
			{
				pawnCanFireAtWill = dashingPawn.drafter.FireAtWill;
			}
			base.RespawnPawn();
			comp.dashSoundDef.PlayOneShot(dashingPawn);

			if (MapHeld != null && !comp.onFinishFlecks.NullOrEmpty())
			{
				foreach (FleckProps fleck in comp.onFinishFlecks)
				{
					fleck.MakeFleck(MapHeld, GetDrawPos());
				}
			}

			dashingPawn.meleeVerbs.TryMeleeAttack(new LocalTargetInfo(target.ToIntVec3()).Pawn, null, surpriseAttack: true);
		}
	}

}
