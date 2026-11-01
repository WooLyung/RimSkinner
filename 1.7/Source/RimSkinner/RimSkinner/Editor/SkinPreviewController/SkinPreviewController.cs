using RimSkinner.Management;
using RimSkinner.Skin;
using UnityEngine;
using Verse;

namespace RimSkinner.Editor
{
    public sealed class SkinPreviewController
    {
        private readonly SkinGraphicData graphicData;
        public IReadOnlyList<(int Index, string Name)> TextureOptions => graphicData.TextureOptions;

        private int selectedPosition;
        public int SelectedIndex => TextureOptions.Count == 0 ? -1 : TextureOptions[selectedPosition].Index;
        public string? SelectedName => TextureOptions.Count == 0 ? null : TextureOptions[selectedPosition].Name;

        public Color PreviewColor { get; private set; }
        public Color PreviewColorTwo { get; private set; } = Color.white;
        private readonly Color originalColor;

        public const float Width = 180f;
        public const float Height = 148f;

        public SkinPreviewController(GraphicSlot slot, SkinGraphicData data)
        {
            graphicData = data;
            PreviewColor = originalColor = Color.white;
        }

        public void Draw(Rect rect)
        {
            Widgets.DrawMenuSection(rect);
            Rect inner = rect.ContractedBy(10f);

            TextAnchor previousAnchor = Text.Anchor;
            GameFont previousFont = Text.Font;
            bool previousEnabled = GUI.enabled;

            try
            {
                Text.Font = GameFont.Small;
                Text.Anchor = TextAnchor.MiddleCenter;
                Rect title = new Rect(inner.x, inner.y, inner.width, 28f);
                Widgets.DrawOptionUnselected(title);
                Widgets.Label(title, "RimSkinner.Editor.PreviewController".Translate());

                Rect selector = new Rect(inner.x, title.yMax + 12f, inner.width, 30f);
                GUI.enabled = previousEnabled && TextureOptions.Count > 1;
                
                if (Widgets.ButtonText(new Rect(selector.x, selector.y, 30f, 30f), "<"))
                    selectedPosition = (selectedPosition + TextureOptions.Count - 1) % TextureOptions.Count;
                if (Widgets.ButtonText(new Rect(selector.xMax - 30f, selector.y, 30f, 30f), ">"))
                    selectedPosition = (selectedPosition + 1) % TextureOptions.Count;
               
                GUI.enabled = previousEnabled;
                Widgets.Label(new Rect(selector.x + 36f, selector.y, selector.width - 72f, 30f),
                    SelectedName?.Translate().ToString() ?? "—");

                DrawColor(new Rect(inner.x, selector.yMax + 2f, inner.width, 26f),
                    "RimSkinner.Editor.PreviewController.Color".Translate(), PreviewColor,
                    () => Find.WindowStack.Add(new SkinPreviewControllerColorPicker(PreviewColor, originalColor, value => PreviewColor = value)));
                DrawColor(new Rect(inner.x, selector.yMax + 30f, inner.width, 26f),
                    "RimSkinner.Editor.PreviewController.ColorTwo".Translate(), PreviewColorTwo,
                    () => Find.WindowStack.Add(new SkinPreviewControllerColorPicker(PreviewColorTwo, Color.white, value => PreviewColorTwo = value)));
                

            }
            finally
            {
                Text.Anchor = previousAnchor;
                Text.Font = previousFont;
                GUI.enabled = previousEnabled;
            }
        }

        private static void DrawColor(Rect rect, string label, Color color, Action onClick)
        {
            TextAnchor previousAnchor = Text.Anchor;
            Text.Anchor = TextAnchor.MiddleLeft;
            Widgets.Label(new Rect(rect.x, rect.y, rect.width - 32f, rect.height), label);
            Text.Anchor = previousAnchor;
            Rect swatch = new Rect(rect.xMax - 24f, rect.center.y - 12f, 24f, 24f);

            Widgets.DrawBoxSolid(swatch, color);
            Widgets.DrawBox(swatch);

            if (Widgets.ButtonInvisible(swatch))
                onClick();
        }
    }
}
