using RimWorld;
using Verse;
using System.Collections.Generic;

namespace flangoCore
{
	public class CompProperties_AbilityDash : CompProperties_EffectWithDest
	{
		public CompProperties_AbilityDash()
		{
			compClass = typeof(CompAbilityEffect_Dash);
		}

		public float altitudeMultiplier = 1.5f;

		public SoundDef startSound;
		public SoundDef endSound;

		public bool rope;

		public EffecterDef effecter;
		public List<FleckProps> onStartFlecks;
		public List<FleckProps> onDashingFlecks;
		public List<FleckProps> onFinishFlecks;
	}

	public class CompAbilityEffect_Dash : CompAbilityEffect_WithDest
	{
		public CompProperties_AbilityDash CompProp => (CompProperties_AbilityDash)props;

		public override void Apply(LocalTargetInfo target, LocalTargetInfo dest)
		{
			Map map = Caster.Map;
			DashingPawn dashingPawn = (DashingPawn)PawnFlyer.MakeFlyer(DefDatabase<ThingDef>.GetNamed("DashingPawn"), CasterPawn, target.Cell, CompProp.effecter, CompProp.endSound);
			dashingPawn.ability = parent;

			dashingPawn.target = target.Thing == null ? target.CenterVector3 : target.Thing.InteractionCell.ToVector3();

			if (CompProp.rope) dashingPawn.rope = true;

			GenSpawn.Spawn(dashingPawn, Caster.Position, map);

			if (map != null && !CompProp.onStartFlecks.NullOrEmpty())
			{
				foreach (FleckProps fleck in CompProp.onStartFlecks)
				{
					fleck.MakeFleck(map, Caster.Position.ToVector3());
				}
			}

			base.Apply(target, dest);
		}
	}
}
