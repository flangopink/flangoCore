using RimWorld;
using Verse;
using HarmonyLib;
using Verse.Sound;

namespace flangoCore
{
    [HarmonyPatch(typeof(Projectile), "ImpactSomething")]
    public static class Patch_Projectile_ImpactSomething_Deflect
    {
        [HarmonyPrefix]
        public static bool Prefix(Projectile __instance)
        {
            if (__instance.usedTarget.Thing is Pawn pawn && pawn.equipment != null && pawn.Drafted)
            {
                var ext = pawn.equipment.Primary?.def.GetModExtension<ModExt_DeflectProjectiles>();
                if (ext == null || (ext.cantDeflect != null && ext.cantDeflect.Contains(__instance.def))) return true;

                if (pawn.skills != null)
                {
                    if (FlangoCore.settings.deflectionChanceAffectedByMeleeSkill)
                    {
                        ext.deflectChance += pawn.skills.GetSkill(SkillDefOf.Melee).Level;
                    }
                    if (FlangoCore.settings.deflectionAccuracyAffectedByMeleeSkill)
                    {
                        ext.deflectAccuracy += pawn.skills.GetSkill(SkillDefOf.Melee).Level;
                    }
                }

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
                        if (FlangoCore.settings.enableDeflectionText)
                        {
                            MoteMaker.ThrowText(pawn.Position.ToVector3(), pawn.Map, "fc_projectileDeflected".Translate());
                        }
                    }
                    else if (FlangoCore.settings.enableDeflectionText)
                    {
                        MoteMaker.ThrowText(pawn.Position.ToVector3(), pawn.Map, "fc_projectileBlocked".Translate());
                    }

                    ext.deflectFleck?.MakeFleck(pawn.Map, pawn.DrawPos);

                    return false;
                }
            }
            return true;
        }
    }
}
