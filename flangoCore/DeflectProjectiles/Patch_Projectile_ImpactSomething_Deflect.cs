using RimWorld;
using Verse;
using HarmonyLib;

namespace flangoCore
{
    [HarmonyPatch(typeof(Projectile), "ImpactSomething")]
    public static class Patch_Projectile_ImpactSomething_Deflect
    {
        [HarmonyPrefix]
        public static bool Prefix(Projectile __instance)
        {
            if (__instance.usedTarget.Thing is Pawn pawn)
            {
                float chance = pawn.GetStatValue(DefOf_flangoCore.ProjectileDeflectionChance);

                if (chance == 0) return true;

                bool accAffectedByMelee = FlangoCore.settings.deflectionAccuracyAffectedByMeleeSkill;
                int accuracy = 2;
                if (pawn.skills != null) 
                {
                    var melee = pawn.skills.GetSkill(SkillDefOf.Melee);
                    accuracy = !melee.TotallyDisabled && accAffectedByMelee ? (int)(melee.Level * 0.5f) : 2; 
                }

                float roll = Rand.Value;
                if (roll < chance)
                {
                    //ext.deflectSound?.PlayOneShot(pawn);
                    DefDatabase<SoundDef>.GetNamed("Deflect_Metal");
                    pawn.Drawer.Notify_DamageDeflected(new DamageInfo(__instance.def.projectile.damageDef, 1f));

                    ThingWithComps equipment = pawn.equipment?.Primary;

                    ProjectileHitFlags projectileHitFlags = ProjectileHitFlags.NonTargetWorld;
                    float hitRoll = Rand.Value;
                    if (hitRoll < accuracy)
                    {
                        projectileHitFlags = ProjectileHitFlags.All;
                    }

                    Thing other = __instance.Launcher;
                    __instance.Destroy();
                    if (FlangoCore.settings.blockNonHostileProjectiles && !pawn.Faction.HostileTo(other.Faction))
                    {
                        if (FlangoCore.settings.enableDeflectionText)
                        {
                            MoteMaker.ThrowText(pawn.Position.ToVector3(), pawn.MapHeld, "fc_projectileBlocked".Translate());
                        }
                    }
                    else
                    {
                        Projectile obj = (Projectile)GenSpawn.Spawn(__instance.def, pawn.Position, pawn.MapHeld);
                        obj.Launch(pawn, pawn.Position.ToVector3(), new LocalTargetInfo(other.Position), other, projectileHitFlags, false, equipment);
                        if (FlangoCore.settings.enableDeflectionText)
                        {
                            MoteMaker.ThrowText(pawn.Position.ToVector3(), pawn.MapHeld, "fc_projectileDeflected".Translate());
                        }
                    }

                    pawn.Map.flecks.CreateFleck(FleckMaker.GetDataStatic(pawn.DrawPos, pawn.MapHeld, DefDatabase<FleckDef>.GetNamed("SparkFlash")));

                    return false;
                }
            }
            return true;
        }
    }
}
