using RimWorld;
using Verse;
using RimWorld.Planet;
using System.Collections.Generic;
using System.Linq;
using System;

namespace flangoCore
{
    public class EquipmentAbility : Ability
    {
        public EquipmentAbility(Pawn pawn) : base(pawn) {}
        public EquipmentAbility(Pawn pawn, AbilityDef def) : base(pawn, def) 
        {
            if ((FlangoCore.settings.enableAbilityCooldownOnEquipForItems && !sources.NullOrEmpty() && sources.First() is ThingWithComps) 
             || (FlangoCore.settings.enableAbilityCooldownOnEquipForHediffs && !sources.NullOrEmpty() && sources.First() is Hediff))
            {
                CooldownTicksLeft = MaxCastingTicks;
            }
        }
        public EquipmentAbility(Pawn pawn, Precept sourcePrecept, AbilityDef def) : base(pawn, sourcePrecept, def)
        {
            if ((FlangoCore.settings.enableAbilityCooldownOnEquipForItems && !sources.NullOrEmpty() && sources.First() is ThingWithComps)
             || (FlangoCore.settings.enableAbilityCooldownOnEquipForHediffs && !sources.NullOrEmpty() && sources.First() is Hediff))
            {
                CooldownTicksLeft = MaxCastingTicks;
            }
        }

        public List<IExposable> sources = new();

        public int MaxCastingTicks => def.overrideGroupCooldown ? def.cooldownTicksRange.RandomInRange : def.groupDef.cooldownTicks;

        private int TicksUntilCasting = -1;
        public int CooldownTicksLeft
        {
            get => TicksUntilCasting;
            set => TicksUntilCasting = value;
        }

        public override IEnumerable<Command> GetGizmos()
        {
            if (gizmo == null)
            {
                gizmo = (Command)Activator.CreateInstance(def.gizmoClass, this);
                //gizmo.order = def.uiOrder;


                if (!CanCastPowerCheck("Player", out var reason))
                {
                    gizmo.Disable(reason);
                }

                /*if (CooldownTicksLeft == -5)
                {
                    CooldownTicksLeft = MaxCastingTicks;
                    StartCooldown(MaxCastingTicks);
                    gizmo.Disable("AbilityOnCooldown".Translate(TicksUntilCasting.ToStringSecondsFromTicks()));
                }*/
            }
            yield return gizmo;

            if (Prefs.DevMode && CooldownTicksLeft > 0)
            {
                Command_Action command_Action = new()
                {
                    defaultLabel = "Reset cooldown",
                    action = delegate
                    {
                        CooldownTicksLeft = 0;
                    }
                };
                yield return command_Action;
            }
        }

        public override void QueueCastingJob(LocalTargetInfo target, LocalTargetInfo destination)
        {
            base.QueueCastingJob(target, destination);
            CooldownTicksLeft = MaxCastingTicks;
            StartCooldown(MaxCastingTicks);
        }

        public override void AbilityTick()
        {
            if (WorldPawnsUtility.IsWorldPawn(pawn)) return;

            base.AbilityTick();

            if (CooldownTicksLeft > -1 && !Find.TickManager.Paused)
            {
                CooldownTicksLeft--;
                if (!gizmo.disabled)
                {
                    gizmo.Disable("AbilityOnCooldown".Translate(CooldownTicksLeft.ToStringSecondsFromTicks()));
                }
            }
            else if (!Find.TickManager.Paused && gizmo != null && gizmo.disabled)
            {
                gizmo.disabled = false;
            }

            if (!sources.NullOrEmpty())
            {
                if (sources.OfType<Apparel>().Any())
                {
                    foreach (Apparel apparel in sources.OfType<Apparel>().ToList())
                    {
                        if (apparel.Wearer != pawn)
                        {
                            sources.Remove(apparel);
                        }
                    }
                    return;
                }
                
                if (sources.OfType<ThingWithComps>().Any())
                {
                    foreach (ThingWithComps thing in sources.OfType<ThingWithComps>().ToList())
                    {
                        if (pawn.equipment.Primary != thing)
                        {
                            sources.Remove(thing);
                        }
                    }
                    return;
                }

                if (sources.OfType<Hediff>().Any())
                {
                    foreach (Hediff hediff in sources.OfType<Hediff>().ToList())
                    {
                        if (!pawn.health.hediffSet.HasHediff(hediff.def))
                        {
                            sources.Remove(hediff);
                        }
                    }
                    return;
                }
            }
            else
            {
                //Log.Warning($"{def.defName} lost all sources, removing ability");

                pawn.abilities.abilities.Remove(this);
                pawn.abilities.Notify_TemporaryAbilitiesChanged();
            }
        }

        public virtual bool CanCastPowerCheck(string context, out string reason)
        {
            reason = "";
            if (context == "Player" && pawn.Faction != Faction.OfPlayer)
            {
                reason = "CannotOrderNonControlled".Translate();
                return false;
            }
            if (pawn.story.DisabledWorkTagsBackstoryAndTraits.HasFlag(WorkTags.Violent) && verb.verbProps.violent)
            {
                reason = "AbilityDisabled_IncapableOfWorkTag".Translate(pawn.Named("PAWN"), WorkTags.Violent.LabelTranslated());
                return false;
            }
            if (CooldownTicksLeft > 0)
            {
                reason = "fc_PawnAbilityRecharging".Translate(pawn.NameShortColored);
                return false;
            }
            return true;
        }

        public override void ExposeData()
        {
            Scribe_Defs.Look(ref def, "def");
            if (def != null)
            {
                Scribe_Values.Look(ref Id, "Id", -1);
                if (Scribe.mode == LoadSaveMode.LoadingVars && Id == -1)
                {
                    Id = Find.UniqueIDsManager.GetNextAbilityID();
                }

                Scribe_References.Look(ref sourcePrecept, "sourcePrecept");
                Scribe_Values.Look(ref TicksUntilCasting, "TicksUntilCasting", -5);
                Scribe_Collections.Look(ref sources, "sources", LookMode.Value);
                if (Scribe.mode == LoadSaveMode.PostLoadInit)
                {
                    Initialize();
                    if ((FlangoCore.settings.enableAbilityCooldownOnEquipForItems && !sources.NullOrEmpty() && sources.First() is ThingWithComps)
                     || (FlangoCore.settings.enableAbilityCooldownOnEquipForHediffs && !sources.NullOrEmpty() && sources.First() is Hediff))
                    {
                        CooldownTicksLeft = MaxCastingTicks;
                    }
                }
            }
        }
    }
}