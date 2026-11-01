using RimSkinner.Management;
using RimSkinner.Skin;
using UnityEngine;
using Verse;

namespace RimSkinner.Editor
{
    public sealed class SkinEditorContent
    {
        public static readonly HashSet<Type> SupportedGraphics = new HashSet<Type> {
            typeof(Graphic_Single),
            typeof(Graphic_Multi)
        };

#pragma warning disable CS8618
        private GraphicSlot graphicSlot;
        private SkinGraphicData target;
        private ThingDef buildingDef;
        private SkinData skin;
        private string graphicPath;
#pragma warning restore CS8618

        public bool IsReady { get; private set; } = false;

        public void Clear()
        {
            IsReady = false;
        }

        public void InitContent(GraphicSlot graphicSlot, SkinGraphicData target, ThingDef buildingDef, SkinData skin, string graphicPath)
        {
            this.graphicSlot = graphicSlot;
            this.target = target.Clone();
            this.buildingDef = buildingDef;
            this.skin = skin;
            this.graphicPath = graphicPath;

            IsReady = true;
        }

        public void DrawContent(Rect rect)
        {
            if (!IsReady)
                return;
            if (rect.height <= 0f)
                return;

            const float gap = 6f;

            Widgets.DrawMenuSection(rect);
            Rect content = rect.ContractedBy(gap);
            if (content.width <= gap || content.height <= 0f)
                return;

            float inspectorWidth = (content.width - gap) * 0.225f;
            DrawInspectorPanel(new Rect(content.xMax - inspectorWidth, content.y, inspectorWidth, content.height));

            float previewWidth = (content.width - inspectorWidth - gap) * 0.4f;
            float controlsHeight = Mathf.Min(120f, content.height * 0.3f);
            DrawPreviewPanel(new Rect(content.x, content.yMax - controlsHeight, previewWidth, controlsHeight));

            DrawPreview();
        }

        private void DrawInspectorPanel(Rect rect)
        {
            Widgets.DrawMenuSection(rect);
        }

        private void DrawPreviewPanel(Rect rect)
        {
            Widgets.DrawMenuSection(rect);
        }

        private void DrawPreview()
        {
        }
    }
}