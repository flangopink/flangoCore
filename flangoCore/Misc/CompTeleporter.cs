using System.Collections.Generic;
using RimWorld;
using UnityEngine;
using Verse;
using HotSwap;

namespace flangoCore
{
    public class CompProperties_Teleporter : CompProperties
    {
        public int updateInterval = 60; // if 0 then only initial pawns get affected
        public int cooldownTicks = 300;
        public float range = 0.9f;
        public bool chooseCell;
        public bool requireLOS = true;
        public bool destoyedByEMP;
        public bool destroyAfterUsing;
        public ThingDef linkThing;
        public ThingDef beamMoteDef;
        public int beamMoteDuration = 60;
        public FactionFlags targetFlags;
        public EffecterDef effecterUsed;
        public FleckProps fleckUsed;
        public EffecterDef effecterUsedDest;
        public FleckProps fleckUsedDest;
        public EffecterDef effecterDestroyed;
        public FleckProps fleckDestroyed;
        public string commandLabel = "Destination";
        public string commandDesc = "Choose this teleporter's destination cell.";
        public string commandIcon = null;

        public CompProperties_Teleporter() => compClass = typeof(CompTeleporter);
    }

    [HotSwappable]
    public class CompTeleporter : ThingComp
    {
        public CompProperties_Teleporter Props => (CompProperties_Teleporter)props;
        public Texture2D Icon => !Props.commandIcon.NullOrEmpty() ? ContentFinder<Texture2D>.Get(Props.commandIcon) : BaseContent.BadTex;
        private IntVec3 cell = IntVec3.Invalid;
        private Thing linkedThing;
        private MoteDualAttached mote;
        private bool beaming;
        private int beamTimer;
        private bool onCooldown;
        private int cooldown;

        protected static TargetingParameters TargetingParameters = new() { canTargetLocations = true };

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look(ref cell, "cell");
            Scribe_Values.Look(ref beamTimer, "beamTimer");
            Scribe_Values.Look(ref cooldown, "cooldown");
        }

        public override void CompTick()
        {
            if (Props.updateInterval > 0 && Find.TickManager.TicksGame % Props.updateInterval == 0)
            {
                TryTeleport();
            }
            if (beaming)
            {
                mote = MoteMaker.MakeInteractionOverlay(Props.beamMoteDef, parent, new TargetInfo(cell, parent.MapHeld));
                mote?.Maintain();
                if (--beamTimer <= 0) beaming = false;
            }
            if (onCooldown)
            {
                if (--cooldown <= 0) onCooldown = false;
            }
        }

        public override void PostDestroy(DestroyMode mode, Map previousMap)
        {
            if (Props.effecterDestroyed != null)
            {
                Effecter effecter = Props.effecterDestroyed.Spawn();
                effecter.Trigger(new TargetInfo(parent.Position, previousMap), new TargetInfo(parent.Position, previousMap));
                effecter.Cleanup();
            }
            Props.fleckDestroyed?.MakeFleck(previousMap, parent.Position);
            base.PostDestroy(mode, previousMap);
        }

        public override void PostPreApplyDamage(DamageInfo dinfo, out bool absorbed)
        {
            absorbed = false;
            if (Props.destoyedByEMP && (dinfo.Def == DamageDefOf.EMP || dinfo.Def.externalViolenceForMechanoids))
            {
                if (parent.MapHeld != null)
                {
                    parent.Destroy();
                    if (parent.Destroyed) absorbed = true;
                }
            }
        }

        public void TryTeleport()
        {
            if (cell == IntVec3.Invalid) return;
            if (parent == null || parent.Position == null || parent.Map == null) return; // Just in case
            if (linkedThing != null && linkedThing.Destroyed)
            {
                cell = IntVec3.Invalid;
                linkedThing = null;
                return;
            }

            var pos = parent.Position;
            var map = parent.MapHeld;
            bool succeeded = false;

            foreach (Pawn pawn in Utils.GetPawnsInRange(pos, map, Props.range, Props.requireLOS))
            {
                if (!pawn.TargetFactionValid(Props.targetFlags)) continue;

                pawn.Position = cell;
                pawn.Notify_Teleported();

                if (Props.effecterUsed != null)
                {
                    Effecter effecter = Props.effecterUsed.Spawn();
                    effecter.Trigger(new TargetInfo(pos, map), new TargetInfo(pos, map));
                    effecter.Cleanup();
                }
                if (Props.effecterUsedDest != null)
                {
                    Effecter effecter = Props.effecterUsedDest.Spawn();
                    effecter.Trigger(new TargetInfo(cell, map), new TargetInfo(cell, map));
                    effecter.Cleanup();
                }
                Props.fleckUsed?.MakeFleck(map, pos);
                Props.fleckUsedDest?.MakeFleck(map, cell);
                succeeded = true;
            }
            if (succeeded)
            {
                beaming = true;
                cooldown = Props.cooldownTicks;
                onCooldown = true;
                if (Props.destroyAfterUsing) parent.Destroy();
            }
        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            if (Props.chooseCell)
            {
                yield return new Command_Action()
                {
                    defaultLabel = Props.commandLabel,
                    defaultDesc = Props.commandDesc,
                    icon = Icon,
                    action = delegate
                    {
                        Find.Targeter.BeginTargeting(TargetingParameters, (LocalTargetInfo target) =>
                        {
                            if (Props.linkThing == null)
                            {
                                cell = target.Cell;
                            }
                            if (Props.linkThing != null && target.HasThing && target.Thing.def == Props.linkThing)
                            {
                                linkedThing = target.Thing;
                                cell = target.Cell;
                            }
                            else Messages.Message("MessageTransportPodsDestinationIsInvalid".Translate(), MessageTypeDefOf.RejectInput);
                        });
                    }
                };

                if (linkedThing != null)
                {

                    yield return new Command_Action()
                    {
                        defaultLabel = "Go to linked thing",
                        icon = Icon,
                        action = delegate
                        {
                            CameraJumper.TryJumpAndSelect(linkedThing);
                        }
                    };
                }
            }
        }

        public override string CompInspectStringExtra()
        {
            if (parent.Spawned)
            {
                return (cell == IntVec3.Invalid ? "fc_DestinationNotSet".Translate() : "") + (onCooldown ? "fc_onCooldown_SecondsLeft".Translate(cooldown.TicksToSeconds()) : "");
            }
            return null;
        }

        public override void PostDrawExtraSelectionOverlays()
        {
            GenDraw.DrawRadiusRing(parent.Position, Props.range, Color.white);
            if (cell != IntVec3.Invalid)
            {
                GenDraw.DrawLineBetween(parent.TrueCenter(), cell.ToVector3Shifted(), SimpleColor.White);
                GenDraw.DrawRadiusRing(cell, 0.9f, Color.white);
            }
        }
    }
}
