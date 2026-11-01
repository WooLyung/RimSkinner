using UnityEngine;
using Verse;

namespace RimSkinner.Editor
{
    public abstract class SkinEditorInspectorElement
    {
        protected readonly Func<GraphicData, object> originalGetter;
        protected abstract object? OverrideValue { get; set; }

        public abstract string Label { get; }
        public abstract float Height { get; }

        protected SkinEditorInspectorElement(Func<GraphicData, object> originalGetter)
        {
            ArgumentNullException.ThrowIfNull(originalGetter);
            this.originalGetter = originalGetter;
        }

        public void Draw(Rect rect, GraphicData graphicData)
        {
            GameFont previousFont = Text.Font;
            TextAnchor previousAnchor = Text.Anchor;
            bool previousEnabled = GUI.enabled;
            try
            {
                Text.Font = GameFont.Small;
                Text.Anchor = TextAnchor.MiddleLeft;
                bool useOriginal = OverrideValue == null;
                bool previousUseOriginal = useOriginal;
                string originalLabel = "RimSkinner.Editor.Inspector.UseOriginal".Translate();

                Rect toggle = new Rect(rect.xMax - 20f, rect.y + 2f, 20f, 20f);
                Widgets.Label(new Rect(rect.x, rect.y, Mathf.Max(0f, rect.width - 26f), 24f), Label);
                TooltipHandler.TipRegion(toggle, originalLabel);
                Widgets.DrawBoxSolid(toggle, new Color(0.65f, 0.65f, 0.65f));
                Widgets.DrawBoxSolid(toggle.ContractedBy(1f), new Color(0.12f, 0.12f, 0.12f));
                if (useOriginal)
                    Widgets.DrawBoxSolid(toggle.ContractedBy(5f), new Color(0.75f, 0.85f, 0.95f));
                if (Widgets.ButtonInvisible(toggle, false))
                    useOriginal = !useOriginal;

                Text.Font = GameFont.Tiny;
                if (useOriginal != previousUseOriginal)
                    OverrideValue = useOriginal ? null : originalGetter(graphicData);

                GUI.enabled = previousEnabled && !useOriginal;
                DrawValue(new Rect(rect.x, rect.y + 28f, rect.width, rect.height - 28f),
                    graphicData, useOriginal, useOriginal != previousUseOriginal);
            }
            finally
            {
                GUI.enabled = previousEnabled;
                Text.Font = previousFont;
                Text.Anchor = previousAnchor;
            }
        }

        protected abstract void DrawValue(Rect rect, GraphicData graphicData, bool useOriginal, bool originalChanged);
    }
}