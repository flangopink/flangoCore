using RimWorld;
using Verse;

namespace flangoCore
{
    public class Verb_ShootEquipment : Verb_EquipmentLaunchProjectile
    {
        public override void WarmupComplete()
        {
            base.WarmupComplete();
            if (currentTarget.Thing is Pawn pawn && !pawn.Downed && CasterIsPawn && CasterPawn.skills != null)
            {
                float num = pawn.HostileTo(this.caster) ? 170f : 20f;
                float num2 = verbProps.AdjustedFullCycleTime(this, this.CasterPawn);
                CasterPawn.skills.Learn(SkillDefOf.Shooting, num * num2, false);
            }
        }
    }
}
