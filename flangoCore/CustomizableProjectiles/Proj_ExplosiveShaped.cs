using Verse;
using System;
using UnityEngine;

namespace flangoCore
{
	public class Projectile_ExplosiveShaped : Projectile_Explosive
	{
		public ModExt_ProjectileExplosiveShaped ModExt => def.GetModExtension<ModExt_ProjectileExplosiveShaped>();

		void DoExplosion(IntVec3 pos, Map map)
        {
			GenExplosion.DoExplosion(pos, map, 0.9f, def.projectile.damageDef, launcher, base.DamageAmount, base.ArmorPenetration, def.projectile.soundExplode, equipmentDef, def, intendedTarget.Thing, def.projectile.postExplosionSpawnThingDef, def.projectile.postExplosionSpawnChance, def.projectile.postExplosionSpawnThingCount, preExplosionSpawnThingDef: def.projectile.preExplosionSpawnThingDef, preExplosionSpawnChance: def.projectile.preExplosionSpawnChance, preExplosionSpawnThingCount: def.projectile.preExplosionSpawnThingCount, applyDamageToExplosionCellsNeighbors: def.projectile.applyDamageToExplosionCellsNeighbors, chanceToStartFire: def.projectile.explosionChanceToStartFire, damageFalloff: def.projectile.explosionDamageFalloff, direction: origin.AngleToFlat(destination));
		}

		protected override void Explode()
		{
			Map map = Map;
			Destroy();
			if (def.projectile.explosionEffect != null)
			{
				Effecter effecter = def.projectile.explosionEffect.Spawn();
				effecter.Trigger(new TargetInfo(Position, map), new TargetInfo(Position, map));
				effecter.Cleanup();
			}

			IntVec3 offset;
			switch (ModExt.shape) 
			{
				case ExplosionShape.Normal:
					GenExplosion.DoExplosion(Position, map, def.projectile.explosionRadius, def.projectile.damageDef, launcher, base.DamageAmount, base.ArmorPenetration, def.projectile.soundExplode, equipmentDef, def, intendedTarget.Thing, def.projectile.postExplosionSpawnThingDef, def.projectile.postExplosionSpawnChance, def.projectile.postExplosionSpawnThingCount, preExplosionSpawnThingDef: def.projectile.preExplosionSpawnThingDef, preExplosionSpawnChance: def.projectile.preExplosionSpawnChance, preExplosionSpawnThingCount: def.projectile.preExplosionSpawnThingCount, applyDamageToExplosionCellsNeighbors: def.projectile.applyDamageToExplosionCellsNeighbors, chanceToStartFire: def.projectile.explosionChanceToStartFire, damageFalloff: def.projectile.explosionDamageFalloff, direction: origin.AngleToFlat(destination));
					break;

				case ExplosionShape.Star:
					DoExplosion(Position, map);

					foreach (IntVec3 cell in GenRadial.RadialCellsAround(Position, def.projectile.explosionRadius, false))
                    {
						offset = cell - Position;
						if (Mathf.Abs(offset.x) == Mathf.Abs(offset.z) 
							|| (offset.x == 0 && Mathf.Abs(offset.z) != 1)
							|| (offset.z == 0 && Mathf.Abs(offset.x) != 1)) 
						{
							if (cell.InBounds(map))
							{
								DoExplosion(cell, map);
							}
						}
						else continue;
					}
					break;

				case ExplosionShape.CrossPlus:
					DoExplosion(Position, map);
					foreach (IntVec3 cell in GenRadial.RadialCellsAround(Position, def.projectile.explosionRadius, false))
					{
						offset = cell - Position;
						if ((offset.x == 0 && offset.z != 0) || (offset.z == 0 && offset.x != 0))
						{
							if (cell.InBounds(map))
							{
								DoExplosion(cell, map);
							}
						}
						else continue;
					}
					break;

				case ExplosionShape.CrossX:
					DoExplosion(Position, map);

					foreach (IntVec3 cell in GenRadial.RadialCellsAround(Position, def.projectile.explosionRadius, false))
					{
						offset = cell - Position;
						if (Mathf.Abs(offset.x) == Mathf.Abs(offset.z))
						{
							if (cell.InBounds(map))
							{
								DoExplosion(cell, map);
							}
						}
						else continue;
					}
					break;

				case ExplosionShape.Ring:
					foreach (IntVec3 cell in GenRadial.RadialCellsAround(Position, def.projectile.explosionRadius, false))
					{
						if (cell.InBounds(map))
						{
							DoExplosion(cell, map);
						}
					}
					break;

				case ExplosionShape.RandomAdjacent:
					DoExplosion(Position, map);
					IntVec3 cellRandom = GenRadial.RadialCellsAround(Position, def.projectile.explosionRadius, false).RandomElement();
					if (cellRandom.InBounds(map)) DoExplosion(cellRandom, map);
					break;

				default:
					Log.Warning("ExplosionShape not set for " + def.defName + ". Defaulting to Normal.");
					break;
			}
		}
	}
}
