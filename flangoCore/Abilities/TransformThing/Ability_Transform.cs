using RimWorld;
using Verse;
using System.Collections.Generic;
using UnityEngine;

namespace flangoCore 
{
    public class Ability_Transform : Ability
    {
        //Ability_TransformDef AbilityDef => (Ability_TransformDef)def;

        public Ability_Transform() : base() {}
        public Ability_Transform(Pawn pawn) : base(pawn) {}
        public Ability_Transform(Pawn pawn, AbilityDef def) : base(pawn, def) {}
        public Ability_Transform(Pawn pawn, Precept sourcePrecept) : base(pawn, sourcePrecept) {}
        public Ability_Transform(Pawn pawn, Precept sourcePrecept, AbilityDef def) : base(pawn, sourcePrecept, def) {}

        public CompProperties_AbilityTransform Props => ((CompAbilityEffect_AbilityTransform)comps.Find(x => x.GetType() == typeof(CompAbilityEffect_AbilityTransform))).Props;
        public Texture2D OptionsIcon => ContentFinder<Texture2D>.Get(Props.optionsIconPath) ?? BaseContent.BadTex;
        public Texture2D ResetIcon => ContentFinder<Texture2D>.Get(Props.resetIconPath) ?? BaseContent.BadTex;

        public int MaxCastingTicks => def.cooldownTicksRange.RandomInRange;// * GenTicks.TicksPerRealSecond;
        private int TicksUntilCasting = -5;
        public int CooldownTicksLeft
        {
            get => TicksUntilCasting;
            set => TicksUntilCasting = value;
        } //Log.Message(value.ToString()); } }

        public override void ExposeData()
        {
            Scribe_Defs.Look(ref def, "def");
            if (def == null)
            {
                return;
            }
            Scribe_Values.Look(ref Id, "Id", -1, false);
            if (Scribe.mode == LoadSaveMode.LoadingVars && Id == -1)
            {
                Id = Find.UniqueIDsManager.GetNextAbilityID();
            }
            Scribe_References.Look(ref sourcePrecept, "sourcePrecept", false);
            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                Initialize();
            }
            Scribe_Values.Look(ref TicksUntilCasting, "EquipmentAbilityTicksUntilcasting", -5);
        }

        public override IEnumerable<Command> GetGizmos()
        {
            if (gizmo == null)
            {
                //var command = (Command_EquipmentAbility)Activator.CreateInstance(def.gizmoClass, this);
                var command = new Command_Transform(this)
                {
                    defaultLabel = def.LabelCap,
                    //order = def.uiOrder,
                    curTicks = CooldownTicksLeft
                };

                if (!CanCastPowerCheck("Player", out string reason)) command.Disable(reason);
                gizmo = command;

                /*if (CooldownTicksLeft == -5) // Min is -1, but this will do a one-time cooldown upon equipping
                {
                    CooldownTicksLeft = MaxCastingTicks;
                    StartCooldown(MaxCastingTicks);
                    gizmo.Disable("AbilityOnCooldown".Translate(TicksUntilCasting.ToStringSecondsFromTicks()));
                }*/
            }
            yield return gizmo;

            // Swapping ability with another from floating menu
            if (CompOfType<CompAbilityEffect_AbilityTransform>().Props.transformOptions is List<TransformOutcomeOptions> options && options.Count > 1)
            {
                yield return new Command_Action()
                {
                    defaultLabel = "fc_TransformOptions".Translate(),
                    defaultDesc = "fc_TransformOptionsDesc".Translate(),
                    icon = OptionsIcon,
                    //order = def.uiOrder + 1,
                    action = () =>
                    {
                        List<FloatMenuOption> list = new List<FloatMenuOption>();

                        foreach(var option in options)
                        {
                            list.Add(new FloatMenuOption(option.label, delegate
                            {
                                gizmo.defaultLabel = option.label;
                                gizmo.icon = option.Icon;
                                CompOfType<CompAbilityEffect_AbilityTransform>().option = option;
                            }));
                        }
                        Find.WindowStack.Add(new FloatMenu(list));
                    }
                };
            }

            // Reset cooldown if devmode is on
            if (DebugSettings.ShowDevGizmos && CooldownTicksLeft > 0 && CanCooldown)
            {
                Command_Action command_Action = new Command_Action
                {
                    defaultLabel = "DEV: Reset cooldown",
                    icon = ResetIcon,
                    action = delegate
                    {
                        CooldownTicksLeft = 0;
                        StartCooldown(0);
                    }
                };
                yield return command_Action;
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
            if (pawn.story.DisabledWorkTagsBackstoryAndTraits.HasFlag(WorkTags.Violent) && def.verbProperties.violent)
            {
                reason = "AbilityDisabled_IncapableOfWorkTag".Translate(pawn.Named("PAWN"), WorkTags.Violent.LabelTranslated());
                return false;
            }
            if (CooldownTicksLeft > 0)
            {
                reason = "fc_PawnAbilityRecharging".Translate(pawn.NameShortColored);
                return false;
            }
            //else if (!Verb.CasterPawn.drafter.Drafted)
            //{
            //    reason = "IsNotDrafted".Translate(new object[]
            //    {
            //        Verb.CasterPawn.Name.ToStringShort
            //    });
            //}

            return true;
        }

        protected override void PreActivate(LocalTargetInfo? target)
        {
            base.PreActivate(target);
            CooldownTicksLeft = MaxCastingTicks;
        }

        public override void AbilityTick()
        {
            if (RimWorld.Planet.WorldPawnsUtility.IsWorldPawn(pawn)) return;

            base.AbilityTick();

            if (CooldownTicksLeft > -1 && !Find.TickManager.Paused)
            {
                CooldownTicksLeft--;
                //Log.Message(CooldownTicksLeft.ToString());

                if (!gizmo.disabled)
                {
                    gizmo.Disable("AbilityOnCooldown".Translate(CooldownTicksLeft.ToStringSecondsFromTicks()));
                }
            }
            else
            {
                if (!Find.TickManager.Paused)
                {
                    if (gizmo != null)
                    {
                        if (gizmo.disabled)
                        {
                            gizmo.disabled = false;
                        }
                    }
                }
            }
        }
    }
}
