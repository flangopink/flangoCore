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
            yield return new Command_ColorIcon()
            {
                defaultLabel = "Recolor",
                defaultDesc = "Recolor this item.",
                icon = Icon,
                color = Color,
                action = delegate
                {
                    Find.WindowStack.Add(new Dialog_ChooseColor("fc_Recolor".Translate(), Color, cachedColors, delegate(Color color) { SetColor(color); }));
                }
            };
        }
    }
}