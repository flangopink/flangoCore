using System.Linq;
using RimWorld;
using Verse;

namespace flangoCore
{
    public class Verb_UseEquipmentAbility : Verb_CastAbility
    {
        public new object EquipmentSource
        {
            get
            {
                if (!(ability is EquipmentAbility equipmentAbility))
                {
                    return null;
                }

                return equipmentAbility.sources.First();
            }
        }

        public override bool ValidateTarget(LocalTargetInfo target, bool showMessages = true)
        {
            return base.ValidateTarget(target, showMessages);
        }

        public override void DrawHighlight(LocalTargetInfo target)
        {
            AbilityDef def = ability.def;
            DrawRadius();
            if (CanHitTarget(target) && IsApplicableTo(target, false))
            {
                if (def.HasAreaOfEffect)
                {
                    if (target.IsValid)
                    {
                        GenDraw.DrawTargetHighlight(target);
                        GenDraw.DrawRadiusRing(target.Cell, def.EffectRadius, RadiusHighlightColor, null);
                    }
                }
                else
                {
                    GenDraw.DrawTargetHighlight(target);
                }
            }
            if (target.IsValid)
            {
                ability.DrawEffectPreviews(target);
            }
            verbProps.DrawRadiusRing(caster.Position);
            if (target.IsValid)
            {
                GenDraw.DrawTargetHighlight(target);
                float num = HighlightFieldRadiusAroundTarget(out bool flag);
                if (num > 0.2f && TryFindShootLineFromTo(caster.Position, target, out ShootLine shootLine))
                {
                    if (flag)
                    {
                        GenExplosion.RenderPredictedAreaOfEffect(shootLine.Dest, num);
                        return;
                    }
                    GenDraw.DrawFieldEdges((from x in GenRadial.RadialCellsAround(shootLine.Dest, num, true)
                                            where x.InBounds(Find.CurrentMap)
                                            select x).ToList());
                }
            }
        }

    }
}
