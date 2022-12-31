using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace flangoCore
{
    //[HotSwap.HotSwappable]
    [StaticConstructorOnStartup]
    public class Dialog_SkillTreeEditor : Window
    {
        private Vector2 windowPosition;

        public override bool IsDebug => true;

        protected override float Margin => 4f;

        private readonly ITab_Pawn_Skills skillsTab;

        private static readonly FloatRange XRange = new(0f, 10f);
        private static readonly FloatRange AbsXRange = new(0f, 300f);

        public Dialog_SkillTreeEditor(ITab_Pawn_Skills tab)
        {
            draggable = true;
            focusWhenOpened = false;
            drawShadow = false;
            closeOnAccept = false;
            closeOnCancel = false;
            preventCameraMotion = false;
            drawInScreenshotMode = false;
            windowPosition = Prefs.DevPalettePosition;
            onlyDrawInDevMode = true;
            skillsTab = tab;
        }

        public override void DoWindowContents(Rect inRect)
        {
            Text.Font = GameFont.Small;
            Widgets.Label(new(inRect.x, inRect.y, inRect.width, 24f), "Skill Tree Editor");
            inRect.yMin += 26f;

            Rect sliderRect = new(inRect.x, inRect.y+20, inRect.width, 30f);
            Widgets.HorizontalSlider(sliderRect, ref skillsTab.GetXOffsets()[0], XRange, "X - 1 Column: " + skillsTab.GetXOffsets()[0], 0.1f);
            sliderRect.y += 36f;
            Widgets.HorizontalSlider(sliderRect, ref skillsTab.GetXOffsets()[1], XRange, "X - 2 Columns: " + skillsTab.GetXOffsets()[1], 0.1f);
            sliderRect.y += 36f;
            Widgets.HorizontalSlider(sliderRect, ref skillsTab.GetXOffsets()[2], XRange, "X - 3+ Columns: " + skillsTab.GetXOffsets()[2], 0.1f);
            sliderRect.y += 36f;
            Widgets.HorizontalSlider(sliderRect, ref skillsTab.GetAbsX()[0], AbsXRange, "Abs X Offset: " + skillsTab.GetAbsX()[0], 1f);
            sliderRect.y += 36f;


            if (!Mathf.Approximately(windowRect.x, windowPosition.x) || !Mathf.Approximately(windowRect.y, windowPosition.y))
            {
                windowPosition = new(windowRect.x, windowRect.y);
                Prefs.DevPalettePosition = windowPosition;
            }
        }

        protected override void SetInitialSizeAndPosition()
        {
            GameFont font = Text.Font;
            Text.Font = GameFont.Small;
            Vector2 vector = new(Text.CalcSize("Skill Tree Editor").x + 48f + 10f, 200f);
            Text.Font = GameFont.Tiny;
            
            windowPosition.x = Mathf.Clamp(windowPosition.x, 0f, UI.screenWidth - vector.x);
            windowPosition.y = Mathf.Clamp(windowPosition.y, 0f, UI.screenHeight - vector.y);
            windowRect = new(windowPosition.x, windowPosition.y, vector.x, vector.y);
            windowRect = windowRect.Rounded();
            Text.Font = font;
        }
    }
}
