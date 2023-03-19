using RimWorld;
using System.Text;
using Verse;

namespace flangoCore
{
    public class StatWorker_RangedDodgeChance : StatWorker
    {
        /*private float GetMoving(StatRequest req) => req.Pawn.health.capacities.GetLevel(PawnCapacityDefOf.Moving);

        public override float GetValueUnfinalized(StatRequest req, bool applyPostProcess = true)
        {
            return base.GetValueUnfinalized(req) * (FlangoCore.settings.dodgeScalesWithMovement ? GetMoving(req) : 1f);
        }

        public override string GetExplanationUnfinalized(StatRequest req, ToStringNumberSense numberSense)
        {
            StringBuilder stringBuilder = new();
            stringBuilder.Append(base.GetExplanationUnfinalized(req, numberSense));
            if (req.Pawn.health.capacities != null)
            {
                stringBuilder.AppendLine(PawnCapacityDefOf.Moving + " " + GetMoving(req).ToStringPercent() + ": x" + GetMoving(req).ToStringPercent());
            }
            return stringBuilder.ToString();
        }*/
    }
}
