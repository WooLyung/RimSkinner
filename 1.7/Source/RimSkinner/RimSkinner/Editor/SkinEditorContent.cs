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
        public SkinPreviewController? PreviewController { get; private set; }
        private SkinGraphicData? sourceTarget;
        private List<SkinEditorInspectorElement> inspectorElements = new();
        private Vector2 inspectorScroll;
        private Texture2D? previewFloor;

        public void Clear()
        {
            if (target is SkinGraphicSingle single) 
                single.ReleasePreview();
            if (target is SkinGraphicMulti multi)
                multi.ReleasePreview();

            IsReady = false;
            PreviewController = null;
            sourceTarget = null;
            inspectorElements.Clear();
            inspectorScroll = Vector2.zero;
        }

        public void InitContent(GraphicSlot graphicSlot, SkinGraphicData target, ThingDef buildingDef, SkinData skin, string graphicPath)
        {
            if (IsReady && ReferenceEquals(this.graphicSlot, graphicSlot) && ReferenceEquals(sourceTarget, target) && ReferenceEquals(this.skin, skin))
                return;

            if (this.target is SkinGraphicSingle single) single.ReleasePreview();
            if (this.target is SkinGraphicMulti multi) multi.ReleasePreview();
            this.graphicSlot = graphicSlot;
            sourceTarget = target;
            this.target = target.Clone();
            inspectorElements = this.target.Elements.ToList();
            inspectorScroll = Vector2.zero;
            this.buildingDef = buildingDef;
            this.skin = skin;
            this.graphicPath = graphicPath;

            PreviewController = new SkinPreviewController(graphicSlot, this.target);

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

            float columnX = content.xMax - SkinPreviewController.Width;
            float inspectorHeight = Mathf.Max(0f, content.height - SkinPreviewController.Height - gap);
            if (inspectorHeight > 0f)
                DrawInspectorPanel(new Rect(columnX, content.y, SkinPreviewController.Width, inspectorHeight));

            DrawPreviewControllerPanel(new Rect(columnX, content.yMax - SkinPreviewController.Height,
                SkinPreviewController.Width, SkinPreviewController.Height));

            DrawPreview(new Rect(content.x, content.y, Mathf.Max(0f, columnX - content.x - gap), content.height));
        }

        private void DrawInspectorPanel(Rect rect)
        {
            Widgets.DrawMenuSection(rect);
            Rect inner = rect.ContractedBy(10f);
            Rect title = new Rect(inner.x, inner.y, inner.width, 28f);

            Widgets.DrawOptionUnselected(title);
            TextAnchor previousAnchor = Text.Anchor;
            Text.Anchor = TextAnchor.MiddleCenter;
            Widgets.Label(title, "RimSkinner.Editor.Inspector".Translate());
            Text.Anchor = previousAnchor;

            Rect outer = new Rect(inner.x, title.yMax + 12f, inner.width, inner.height - 40f);
            if (outer.height <= 0f)
                return;

            float height = inspectorElements.Sum(element => element.Height + 16f);
            Rect view = new Rect(0f, 0f, Mathf.Max(1f, outer.width - 16f), height);
            Widgets.BeginScrollView(outer, ref inspectorScroll, view);
            float y = 0f;

            foreach (var element in inspectorElements)
            {
                element.Draw(new Rect(0f, y, Mathf.Max(1f, view.width - 8f), element.Height), graphicSlot.SourceData);
                y += element.Height + 16f;
            }

            Widgets.EndScrollView();
        }

        private void DrawPreviewControllerPanel(Rect rect)
        {
            PreviewController?.Draw(rect);
        }

        private void DrawPreview(Rect rect)
        {
            if (PreviewController == null)
                return;
            if (rect.width <= 0f || rect.height <= 0f)
                return;

            if (Event.current.type != EventType.Repaint)
                return;

            int columns = Mathf.Max(1, buildingDef.size.x) * 3;
            int rows = Mathf.Max(1, buildingDef.size.z) * 3;
            if (target is SkinGraphicMulti && (PreviewController.SelectedIndex == 1 || PreviewController.SelectedIndex == 3))
                (columns, rows) = (rows, columns);
            float pixelsPerCell = Mathf.Min(rect.width / columns, rect.height / rows);
            Rect floorRect = new Rect(
                rect.center.x - columns * pixelsPerCell * 0.5f,
                rect.center.y - rows * pixelsPerCell * 0.5f,
                columns * pixelsPerCell, rows * pixelsPerCell);

            previewFloor ??= ContentFinder<Texture2D>.Get(DefDatabase<TerrainDef>.GetNamed("PavedTile").texturePath);
            Color previousColor = GUI.color;
            try
            {
                GUI.color = DefDatabase<TerrainDef>.GetNamed("PavedTile").DrawColor;
                for (int z = 0; z < rows; z++)
                    for (int x = 0; x < columns; x++)
                        GUI.DrawTextureWithTexCoords(new Rect(floorRect.x + x * pixelsPerCell,
                            floorRect.y + z * pixelsPerCell, pixelsPerCell, pixelsPerCell),
                            previewFloor, new Rect((x % 16) / 16f, (z % 16) / 16f, 1f / 16f, 1f / 16f));

                target.DrawPreview(floorRect, graphicSlot, PreviewController.SelectedIndex,
                    PreviewController.PreviewColor, PreviewController.PreviewColorTwo, pixelsPerCell);
            }
            finally { GUI.color = previousColor; }
        }
    }
}
