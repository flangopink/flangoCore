using System.Collections;
using HarmonyLib;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;

namespace flangoCore
{
    public class AbilityPawnFlyer : PawnFlyer
    {
		public Ability ability;

		protected Vector3 position;

		public Vector3 target;

		public Rot4 direction;

		public bool pawnCanFireAtWill = true;

		public CompProperties_AbilityDash comp;

		public override void SpawnSetup(Map map, bool respawningAfterLoad)
		{
			base.SpawnSetup(map, respawningAfterLoad);
			direction = (startVec.x > target.ToIntVec3().x) ? Rot4.West : ((startVec.x < target.ToIntVec3().x) ? Rot4.East : ((startVec.y < target.ToIntVec3().y) ? Rot4.North : Rot4.South));
			comp = ability.CompOfType<CompAbilityEffect_Dash>().CompProp;
		}

		public override void Tick()
		{
			float num = ticksFlying / ticksFlightTime;

			position = Vector3.Lerp(startVec, target, num);

			base.Tick();
		}

		public override void DrawAt(Vector3 drawLoc, bool flip = false)
		{
			FlyingPawn.Drawer.renderer.RenderPawnAt(position, direction);
		}

		protected override void RespawnPawn()
		{
			Position = target.ToIntVec3();
			base.RespawnPawn();
		}

		public override void ExposeData()
		{
			base.ExposeData();
			Scribe_References.Look(ref ability, "ability");
			Scribe_Values.Look(ref direction, "direction");
		}
	}
}
