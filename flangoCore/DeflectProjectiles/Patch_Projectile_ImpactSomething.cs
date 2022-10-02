using RimWorld;
using Verse;
using HarmonyLib;
using Verse.Sound;

namespace flangoCore
{
    [HarmonyPatch(typeof(Projectile), "ImpactSomething")]
    public static class Patch_Projectile_ImpactSomething
    {
        [HarmonyPrefix]
        public static bool Prefix(Projectile __instance)
        {
            if (__instance.usedTarget.Thing is Pawn pawn && pawn.equipment != null && pawn.Drafted)
            {
                var ext = pawn.equipment.Primary?.def.GetModExtension<ModExt_DeflectProjectiles>();
                if (ext == null || (ext.cantDeflect != null && ext.cantDeflect.Contains(__instance.def))) return true;

                float roll = Rand.Value;
                if (roll < ext.deflectChance)
                {
                    ext.deflectSound.PlayOneShot(pawn);
                    pawn.Drawer.Notify_DamageDeflected(new DamageInfo(__instance.def.projectile.damageDef, 1f));

                    ThingWithComps equipment = null;
                    if (pawn.equipment?.Primary != null)
                    {
                        equipment = pawn.equipment.Primary;
                    }

                    ProjectileHitFlags projectileHitFlags = ProjectileHitFlags.NonTargetWorld;
                    float hitRoll = Rand.Value;
                    if (hitRoll < ext.deflectAccuracy)
                    {
                        projectileHitFlags = ProjectileHitFlags.All;
                    }

                    Thing other = __instance.Launcher;
                    __instance.Destroy();
                    if (pawn.Faction.HostileTo(other.Faction))
                    {
                        Projectile obj = (Projectile)GenSpawn.Spawn(__instance.def, pawn.Position, pawn.Map);
                        obj.Launch(pawn, pawn.Position.ToVector3(), new LocalTargetInfo(other.Position), other, projectileHitFlags, false, equipment);
                        if (Controller.settings.enableDeflectionText)
                        {
                            MoteMaker.ThrowText(pawn.Position.ToVector3(), pawn.Map, "fc_projectileDeflected".Translate());
                        }
                    }
                    else if (Controller.settings.enableDeflectionText)
                    {
                        MoteMaker.ThrowText(pawn.Position.ToVector3(), pawn.Map, "fc_projectileBlocked".Translate());
                    }

                    if (ext.deflectFleck != null)
                    {
                        var fleck = ext.deflectFleck;
                        Map map = pawn.Map;
                        FleckCreationData dataStatic = FleckMaker.GetDataStatic(pawn.DrawPos, map, fleck);
                        if (ext.fleckRandomRotation) dataStatic.rotation = Rand.Range(0f, 360f);
                        map.flecks.CreateFleck(dataStatic);
                    }

                    return false;
                }
            }
            return true;
        }
    }
}
