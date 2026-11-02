using RimSkinner.Editor;
using RimSkinner.Management;
using UnityEngine;
using Verse;

namespace RimSkinner.Skin
{
    public sealed class SkinGraphicSingle : SkinGraphicData
    {
        private static readonly IReadOnlyList<(int Index, string Name)> textureOptions =
            Array.AsReadOnly(new[] {
                (0, "RimSkinner.Editor.PreviewController.Single")
            });

        public override IReadOnlyList<(int Index, string Name)> TextureOptions => textureOptions;

        private Material? previewMaterial;
        public Texture2D? Texture { get; set; }
        public Texture2D? Mask { get; set; }

        public override SkinGraphicData Clone()
        {
            SkinGraphicSingle clone = new SkinGraphicSingle();

            clone.Color = Color;
            clone.ColorTwo = ColorTwo;
            clone.DrawSize = DrawSize;
            clone.DrawOffset = DrawOffset;
            clone.DrawOffsetNorth = DrawOffsetNorth;
            clone.DrawOffsetEast = DrawOffsetEast;
            clone.DrawOffsetSouth = DrawOffsetSouth;
            clone.DrawOffsetWest = DrawOffsetWest;
            clone.DrawRotated = DrawRotated;
            clone.AllowFlip = AllowFlip;
            clone.FlipExtraRotation = FlipExtraRotation;
            clone.ShaderTypeDefName = ShaderTypeDefName;
            clone.TextureRepeats = TextureRepeats;
            clone.AllowAtlasing = AllowAtlasing;
            clone.RenderQueue = RenderQueue;
            clone.OverlayOpacity = OverlayOpacity;
            clone.AddTopAltitudeBias = AddTopAltitudeBias;
            clone.IgnoreThingDrawColor = IgnoreThingDrawColor;

            clone.Texture = SkinData.CloneTexture2D(Texture);
            clone.Mask = SkinData.CloneTexture2D(Mask);

            return clone;
        }

        public override IEnumerable<SkinEditorInspectorElement> Elements
        {
            get
            {
                yield return new SkinEditorInspectorElement_Texture2D("RimSkinner.Editor.Inspector.Texture", () => Texture, value => Texture = value, original => (original.Graphic.MatAt(Verse.Rot4.North).mainTexture as Texture2D) ?? Texture2D.whiteTexture);
                yield return new SkinEditorInspectorElement_Texture2D("RimSkinner.Editor.Inspector.Mask", () => Mask, value => Mask = value, original => ((original.Graphic.MatAt(Verse.Rot4.North).HasProperty(Verse.ShaderPropertyIDs.MaskTex) ? original.Graphic.MatAt(Verse.Rot4.North).GetTexture(Verse.ShaderPropertyIDs.MaskTex) : null) as Texture2D) ?? Texture2D.whiteTexture);
                foreach (var element in CommonElements) 
                    yield return element;
            }
        }

        public override void DrawPreview(Rect rect, GraphicSlot original, int index, Color color, Color colorTwo, float pixelsPerCell)
        {
            if (Event.current.type != EventType.Repaint || rect.width <= 0f || rect.height <= 0f)
                return;

            var data = original.SourceData;
            var graphic = original.SourceGraphic;
            if (graphic == null) return;
            Material source = graphic.MatAt(Rot4.North);
            var texture = Texture ?? source.mainTexture;
            if (texture == null)
                return;

            previewMaterial ??= new Material(source);
            var shaderDef = ShaderTypeDefName == null ? null : DefDatabase<ShaderTypeDef>.GetNamedSilentFail(ShaderTypeDefName);
            previewMaterial.shader = shaderDef?.Shader ?? source.shader;
            previewMaterial.mainTexture = texture;
            previewMaterial.color = (Color ?? data.color) * ((IgnoreThingDrawColor ?? data.ignoreThingDrawColor) ? UnityEngine.Color.white : color);
            previewMaterial.SetColor(ShaderPropertyIDs.ColorTwo, (ColorTwo ?? data.colorTwo) * ((IgnoreThingDrawColor ?? data.ignoreThingDrawColor) ? UnityEngine.Color.white : colorTwo));
            var originalMask = source.HasProperty(ShaderPropertyIDs.MaskTex) ? source.GetTexture(ShaderPropertyIDs.MaskTex) : null;
            previewMaterial.SetTexture(ShaderPropertyIDs.MaskTex, Mask ?? originalMask);
            previewMaterial.renderQueue = RenderQueue ?? source.renderQueue;

            Vector2 size = DrawSize ?? data.drawSize;
            Vector3 offset = DrawOffsetNorth ?? data.drawOffsetNorth ?? DrawOffset ?? data.drawOffset;
            if (!float.IsFinite(size.x) || !float.IsFinite(size.y) ||
                !float.IsFinite(offset.x) || !float.IsFinite(offset.z))
                return;

            float scale = pixelsPerCell;
            Rect image = new Rect(
                rect.width * 0.5f + offset.x * scale - Mathf.Abs(size.x) * scale * 0.5f,
                rect.height * 0.5f - offset.z * scale - Mathf.Abs(size.y) * scale * 0.5f,
                Mathf.Abs(size.x) * scale, Mathf.Abs(size.y) * scale);
            Rect uv = new Rect(size.x < 0f ? 1f : 0f, size.y < 0f ? 1f : 0f,
                size.x < 0f ? -1f : 1f, size.y < 0f ? -1f : 1f);

            UnityEngine.Color previousColor = GUI.color;
            GUI.BeginGroup(rect);
            try
            {
                GUI.color = UnityEngine.Color.white;
                Graphics.DrawTexture(image, texture, uv, 0, 0, 0, 0, UnityEngine.Color.white, previewMaterial);
            }
            finally
            {
                GUI.color = previousColor;
                GUI.EndGroup();
            }
        }

        internal void ReleasePreview()
        {
            if (previewMaterial != null)
                UnityEngine.Object.Destroy(previewMaterial);
            previewMaterial = null;
        }
    }
}