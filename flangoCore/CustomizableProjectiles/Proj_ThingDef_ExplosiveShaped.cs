using RimWorld;
using Verse;
using System;

namespace flangoCore
{
	public class ThingDef_ProjectileExplosiveShaped : ThingDef
	{
		public ExplosionShape ExplosionShape = 0;
	}

	public enum ExplosionShape
	{
		Normal,
		Star,
		Ring,
		RandomAdjacent
	}

	public class Projectile_ExplosiveShaped : Projectile_Explosive
    {
        public ThingDef_ProjectileExplosiveShaped Def => (ThingDef_ProjectileExplosiveShaped)def;

		protected override void Explode()
		{
			Map map = base.Map;
			Destroy();
			if (def.projectile.explosionEffect != null)
			{
				Effecter effecter = def.projectile.explosionEffect.Spawn();
				effecter.Trigger(new TargetInfo(base.Position, map), new TargetInfo(base.Position, map));
				effecter.Cleanup();
			}
			switch (Def.ExplosionShape) 
			{
				case ExplosionShape.Normal:
					GenExplosion.DoExplosion(base.Position, map, def.projectile.explosionRadius, def.projectile.damageDef, launcher, base.DamageAmount, base.ArmorPenetration, def.projectile.soundExplode, equipmentDef, def, intendedTarget.Thing, def.projectile.postExplosionSpawnThingDef, def.projectile.postExplosionSpawnChance, def.projectile.postExplosionSpawnThingCount, preExplosionSpawnThingDef: def.projectile.preExplosionSpawnThingDef, preExplosionSpawnChance: def.projectile.preExplosionSpawnChance, preExplosionSpawnThingCount: def.projectile.preExplosionSpawnThingCount, applyDamageToExplosionCellsNeighbors: def.projectile.applyDamageToExplosionCellsNeighbors, chanceToStartFire: def.projectile.explosionChanceToStartFire, damageFalloff: def.projectile.explosionDamageFalloff, direction: origin.AngleToFlat(destination));
					break;

				case ExplosionShape.Star:
					GenExplosion.DoExplosion(base.Position, map, 0.9f, def.projectile.damageDef, launcher, base.DamageAmount, base.ArmorPenetration, def.projectile.soundExplode, equipmentDef, def, intendedTarget.Thing, def.projectile.postExplosionSpawnThingDef, def.projectile.postExplosionSpawnChance, def.projectile.postExplosionSpawnThingCount, preExplosionSpawnThingDef: def.projectile.preExplosionSpawnThingDef, preExplosionSpawnChance: def.projectile.preExplosionSpawnChance, preExplosionSpawnThingCount: def.projectile.preExplosionSpawnThingCount, applyDamageToExplosionCellsNeighbors: def.projectile.applyDamageToExplosionCellsNeighbors, chanceToStartFire: def.projectile.explosionChanceToStartFire, damageFalloff: def.projectile.explosionDamageFalloff, direction: origin.AngleToFlat(destination));

					IntVec3 offset;
					foreach (IntVec3 cell in GenRadial.RadialCellsAround(base.Position, def.projectile.explosionRadius, false))
                    {
						offset = base.Position - cell;
						offset = new IntVec3(Math.Abs(offset.x), offset.y, Math.Abs(offset.z));
						if ((offset.x != 0 && offset.z != 0) && ((offset.x % 2 == 1 && offset.z % 2 == 1) || (offset.x % 2 == 0 && offset.z % 2 == 0)))
							GenExplosion.DoExplosion(cell, map, 0.9f, def.projectile.damageDef, launcher, base.DamageAmount, base.ArmorPenetration, def.projectile.soundExplode, equipmentDef, def, intendedTarget.Thing, def.projectile.postExplosionSpawnThingDef, def.projectile.postExplosionSpawnChance, def.projectile.postExplosionSpawnThingCount, preExplosionSpawnThingDef: def.projectile.preExplosionSpawnThingDef, preExplosionSpawnChance: def.projectile.preExplosionSpawnChance, preExplosionSpawnThingCount: def.projectile.preExplosionSpawnThingCount, applyDamageToExplosionCellsNeighbors: def.projectile.applyDamageToExplosionCellsNeighbors, chanceToStartFire: def.projectile.explosionChanceToStartFire, damageFalloff: def.projectile.explosionDamageFalloff, direction: origin.AngleToFlat(destination));
						else continue;
					}
					break;

				case ExplosionShape.Ring:
					foreach (IntVec3 cell in GenRadial.RadialCellsAround(base.Position, def.projectile.explosionRadius, false))
					{
						GenExplosion.DoExplosion(cell, map, 0.9f, def.projectile.damageDef, launcher, base.DamageAmount, base.ArmorPenetration, def.projectile.soundExplode, equipmentDef, def, intendedTarget.Thing, def.projectile.postExplosionSpawnThingDef, def.projectile.postExplosionSpawnChance, def.projectile.postExplosionSpawnThingCount, preExplosionSpawnThingDef: def.projectile.preExplosionSpawnThingDef, preExplosionSpawnChance: def.projectile.preExplosionSpawnChance, preExplosionSpawnThingCount: def.projectile.preExplosionSpawnThingCount, applyDamageToExplosionCellsNeighbors: def.projectile.applyDamageToExplosionCellsNeighbors, chanceToStartFire: def.projectile.explosionChanceToStartFire, damageFalloff: def.projectile.explosionDamageFalloff, direction: origin.AngleToFlat(destination));
					}
					break;

				case ExplosionShape.RandomAdjacent:
					GenExplosion.DoExplosion(base.Position, map, 0.9f, def.projectile.damageDef, launcher, base.DamageAmount, base.ArmorPenetration, def.projectile.soundExplode, equipmentDef, def, intendedTarget.Thing, def.projectile.postExplosionSpawnThingDef, def.projectile.postExplosionSpawnChance, def.projectile.postExplosionSpawnThingCount, preExplosionSpawnThingDef: def.projectile.preExplosionSpawnThingDef, preExplosionSpawnChance: def.projectile.preExplosionSpawnChance, preExplosionSpawnThingCount: def.projectile.preExplosionSpawnThingCount, applyDamageToExplosionCellsNeighbors: def.projectile.applyDamageToExplosionCellsNeighbors, chanceToStartFire: def.projectile.explosionChanceToStartFire, damageFalloff: def.projectile.explosionDamageFalloff, direction: origin.AngleToFlat(destination));

					IntVec3 cellRandom = GenRadial.RadialCellsAround(base.Position, def.projectile.explosionRadius, false).RandomElement();

					GenExplosion.DoExplosion(cellRandom, map, 0.9f, def.projectile.damageDef, launcher, base.DamageAmount, base.ArmorPenetration, def.projectile.soundExplode, equipmentDef, def, intendedTarget.Thing, def.projectile.postExplosionSpawnThingDef, def.projectile.postExplosionSpawnChance, def.projectile.postExplosionSpawnThingCount, preExplosionSpawnThingDef: def.projectile.preExplosionSpawnThingDef, preExplosionSpawnChance: def.projectile.preExplosionSpawnChance, preExplosionSpawnThingCount: def.projectile.preExplosionSpawnThingCount, applyDamageToExplosionCellsNeighbors: def.projectile.applyDamageToExplosionCellsNeighbors, chanceToStartFire: def.projectile.explosionChanceToStartFire, damageFalloff: def.projectile.explosionDamageFalloff, direction: origin.AngleToFlat(destination));
					
					break;

				default:
					Log.Warning("ExplosionShape not set for " + def.defName + ". Defaulting to Normal.");
					break;
			}
		}
	}
}
