using RimWorld;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace flangoCore
{
    [StaticConstructorOnStartup]
    public class CompRecolorable : CompColorable
    {
        private static readonly Texture2D Icon = ContentFinder<Texture2D>.Get("Designations/Paint");
        private readonly List<Color> cachedColors = new();

        public CompRecolorable()
        {
            cachedColors = DefDatabase<ColorDef>.AllDefsListForReading.Select((ColorDef c) => c.color).ToList();
            cachedColors.AddRange(Find.FactionManager.AllFactionsVisible.Select((Faction f) => f.Color));
            cachedColors.SortByColor((Color c) => c);
        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            Command_ColorIcon c_detonate = new()
            {
                defaultLabel = "Recolor",
                defaultDesc = "Recolor this item.",
                icon = Icon,
                color = Color,
                action = delegate
                {
                    //Find.WindowStack.Add(new Dialog_Recolor(parent));
                    Find.WindowStack.Add(new Dialog_ChooseColor("fc_Recolor".Translate(), Color, cachedColors, delegate(Color color) { SetColor(color); }));
                }
            };
            yield return c_detonate;
        }
    }
    /*
    public class Dialog_Recolor : Window
    {
        protected int r;
        protected int g;
        protected int b;
        protected Color curColor;
        public ThingWithComps thing;
        public Color prevColor;

        public override Vector2 InitialSize => new Vector2(150f, 280f);

        public Dialog_Recolor(ThingWithComps thing)
        {
            forcePause = true;
            doCloseX = true;
            absorbInputAroundWindow = true;
            closeOnAccept = false;
            closeOnClickedOutside = true;
            prevColor = thing.GetComp<CompColorable>().Color;
        }

        protected virtual AcceptanceReport ColorIsValid(string r, string g, string b)
        {
            if (r.Length == 0 || g.Length == 0 || b.Length == 0)
            {
                return false;
            }
            return true;
        }

        public override void DoWindowContents(Rect inRect)
        {
            Text.Font = GameFont.Small;
            bool flag = false;
            if (Event.current.type == EventType.KeyDown && Event.current.keyCode == KeyCode.Return)
            {
                flag = true;
                Event.current.Use();
            }

            GUI.SetNextControlName("RecolorField_R");
            string rb = prevColor.r.ToString();
            Widgets.TextFieldNumericLabeled(new Rect(0f, 15f, inRect.width, 35f), "R", ref r, ref rb, 0, 255);

            GUI.SetNextControlName("RecolorField_G");
            string gb = prevColor.g.ToString();
            Widgets.TextFieldNumericLabeled(new Rect(0f, 65f, inRect.width, 35f), "G", ref r, ref rb, 0, 255);

            GUI.SetNextControlName("RecolorField_B");
            string bb = prevColor.b.ToString();
            Widgets.TextFieldNumericLabeled(new Rect(0f, 115f, inRect.width, 35f), "B", ref r, ref rb, 0, 255);

            Widgets.ColorSelector

            if (!(Widgets.ButtonText(new Rect(15f, inRect.height - 35f - 15f, inRect.width - 15f - 15f, 35f), "OK") || flag)) return;

            AcceptanceReport acceptanceReport = ColorIsValid(rb, gb, bb);
            if (!acceptanceReport.Accepted)
            {
                if (acceptanceReport.Reason.NullOrEmpty())
                    Messages.Message("fc_ColorIsInvalid".Translate(), MessageTypeDefOf.RejectInput, historical: false);
                else Messages.Message(acceptanceReport.Reason, MessageTypeDefOf.RejectInput, historical: false);
            }
            else
            {
                thing.SetColor(new Color(r,g,b));
                Find.WindowStack.TryRemove(this);
            }
        }
    }
    */
}