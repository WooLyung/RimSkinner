using RimSkinner.Editor;
using RimSkinner.Management;
using UnityEngine;
using Verse;

namespace RimSkinner.Skin
{
    public sealed class SkinGraphicMulti : SkinGraphicData
    {
        private static readonly IReadOnlyList<(int Index, string Name)> textureOptions =
            Array.AsReadOnly(new[]
            {
                (0, "RimSkinner.Editor.PreviewController.North"),
                (1, "RimSkinner.Editor.PreviewController.East"),
                (2, "RimSkinner.Editor.PreviewController.South"),
                (3, "RimSkinner.Editor.PreviewController.West")
            });

        public override IReadOnlyList<(int Index, string Name)> TextureOptions => textureOptions;

        private readonly Material?[] previewMaterials = new Material?[4];

        public Texture2D? TextureNorth { get; set; }
        public Texture2D? TextureEast { get; set; }
        public Texture2D? TextureSouth { get; set; }
        public Texture2D? TextureWest { get; set; }

        public Texture2D? MaskNorth { get; set; }
        public Texture2D? MaskEast { get; set; }
        public Texture2D? MaskSouth { get; set; }
        public Texture2D? MaskWest { get; set; }

        public override SkinGraphicData Clone()
        {
            SkinGraphicMulti clone = new SkinGraphicMulti();

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

            clone.TextureNorth = SkinData.CloneTexture2D(TextureNorth);
            clone.TextureEast = SkinData.CloneTexture2D(TextureEast);
            clone.TextureSouth = SkinData.CloneTexture2D(TextureSouth);
            clone.TextureWest = SkinData.CloneTexture2D(TextureWest);
            clone.MaskNorth = SkinData.CloneTexture2D(MaskNorth);
            clone.MaskEast = SkinData.CloneTexture2D(MaskEast);
            clone.MaskSouth = SkinData.CloneTexture2D(MaskSouth);
            clone.MaskWest = SkinData.CloneTexture2D(MaskWest);

            return clone;
        }

        public override IEnumerable<SkinEditorInspectorElement> Elements
        {
            get
            {
                yield return new SkinEditorInspectorElement_Texture2D("RimSkinner.Editor.Inspector.TextureNorth", () => TextureNorth, value => TextureNorth = value, original => (original.Graphic.MatAt(Verse.Rot4.North).mainTexture as Texture2D) ?? Texture2D.whiteTexture);
                yield return new SkinEditorInspectorElement_Texture2D("RimSkinner.Editor.Inspector.MaskNorth", () => MaskNorth, value => MaskNorth = value, original => ((original.Graphic.MatAt(Verse.Rot4.North).HasProperty(Verse.ShaderPropertyIDs.MaskTex) ? original.Graphic.MatAt(Verse.Rot4.North).GetTexture(Verse.ShaderPropertyIDs.MaskTex) : null) as Texture2D) ?? Texture2D.whiteTexture);
                yield return new SkinEditorInspectorElement_Texture2D("RimSkinner.Editor.Inspector.TextureEast", () => TextureEast, value => TextureEast = value, original => (original.Graphic.MatAt(Verse.Rot4.East).mainTexture as Texture2D) ?? Texture2D.whiteTexture);
                yield return new SkinEditorInspectorElement_Texture2D("RimSkinner.Editor.Inspector.MaskEast", () => MaskEast, value => MaskEast = value, original => ((original.Graphic.MatAt(Verse.Rot4.East).HasProperty(Verse.ShaderPropertyIDs.MaskTex) ? original.Graphic.MatAt(Verse.Rot4.East).GetTexture(Verse.ShaderPropertyIDs.MaskTex) : null) as Texture2D) ?? Texture2D.whiteTexture);
                yield return new SkinEditorInspectorElement_Texture2D("RimSkinner.Editor.Inspector.TextureSouth", () => TextureSouth, value => TextureSouth = value, original => (original.Graphic.MatAt(Verse.Rot4.South).mainTexture as Texture2D) ?? Texture2D.whiteTexture);
                yield return new SkinEditorInspectorElement_Texture2D("RimSkinner.Editor.Inspector.MaskSouth", () => MaskSouth, value => MaskSouth = value, original => ((original.Graphic.MatAt(Verse.Rot4.South).HasProperty(Verse.ShaderPropertyIDs.MaskTex) ? original.Graphic.MatAt(Verse.Rot4.South).GetTexture(Verse.ShaderPropertyIDs.MaskTex) : null) as Texture2D) ?? Texture2D.whiteTexture);
                yield return new SkinEditorInspectorElement_Texture2D("RimSkinner.Editor.Inspector.TextureWest", () => TextureWest, value => TextureWest = value, original => (original.Graphic.MatAt(Verse.Rot4.West).mainTexture as Texture2D) ?? Texture2D.whiteTexture);
                yield return new SkinEditorInspectorElement_Texture2D("RimSkinner.Editor.Inspector.MaskWest", () => MaskWest, value => MaskWest = value, original => ((original.Graphic.MatAt(Verse.Rot4.West).HasProperty(Verse.ShaderPropertyIDs.MaskTex) ? original.Graphic.MatAt(Verse.Rot4.West).GetTexture(Verse.ShaderPropertyIDs.MaskTex) : null) as Texture2D) ?? Texture2D.whiteTexture);
                foreach (var element in CommonElements) yield return element;
            }
        }

        public override void DrawPreview(Rect rect, GraphicSlot original, int index, Color color, Color colorTwo, float pixelsPerCell)
        {
            if (Event.current.type != EventType.Repaint || rect.width <= 0f || rect.height <= 0f)
                return;

            if (index < 0 || index > 3) return;
            Rot4 rotation = new Rot4(index);
            Texture2D? selectedTexture = index switch { 0 => TextureNorth, 1 => TextureEast, 2 => TextureSouth, _ => TextureWest };
            Texture2D? selectedMask = index switch { 0 => MaskNorth, 1 => MaskEast, 2 => MaskSouth, _ => MaskWest };
            var data = original.SourceData;
            var graphic = original.SourceGraphic;
            if (graphic == null) return;
            Material source = graphic.MatAt(rotation);
            var texture = selectedTexture ?? source.mainTexture;
            if (texture == null)
                return;

            Material previewMaterial = previewMaterials[index] ??= new Material(source);
            var shaderDef = ShaderTypeDefName == null ? null : DefDatabase<ShaderTypeDef>.GetNamedSilentFail(ShaderTypeDefName);
            previewMaterial.shader = shaderDef?.Shader ?? source.shader;
            previewMaterial.mainTexture = texture;
            previewMaterial.color = (Color ?? data.color) * ((IgnoreThingDrawColor ?? data.ignoreThingDrawColor) ? UnityEngine.Color.white : color);
            previewMaterial.SetColor(ShaderPropertyIDs.ColorTwo, (ColorTwo ?? data.colorTwo) * ((IgnoreThingDrawColor ?? data.ignoreThingDrawColor) ? UnityEngine.Color.white : colorTwo));
            var originalMask = source.HasProperty(ShaderPropertyIDs.MaskTex) ? source.GetTexture(ShaderPropertyIDs.MaskTex) : null;
            previewMaterial.SetTexture(ShaderPropertyIDs.MaskTex, selectedMask ?? originalMask);
            previewMaterial.renderQueue = RenderQueue ?? source.renderQueue;

            Vector2 size = DrawSize ?? data.drawSize;
            Vector3? directionalOffset = index switch
            {
                0 => DrawOffsetNorth ?? data.drawOffsetNorth,
                1 => DrawOffsetEast ?? data.drawOffsetEast,
                2 => DrawOffsetSouth ?? data.drawOffsetSouth,
                _ => DrawOffsetWest ?? data.drawOffsetWest
            };
            Vector3 offset = directionalOffset ?? DrawOffset ?? data.drawOffset;
            bool rotate = selectedTexture == null && (DrawRotated ?? data.drawRotated) && graphic.ShouldDrawRotated;
            if (rotation.IsHorizontal && !rotate)
                size = new Vector2(size.y, size.x);
            bool flipped = selectedTexture == null && (AllowFlip ?? data.allowFlip) &&
                ((index == 1 && graphic.EastFlipped) || (index == 3 && graphic.WestFlipped));
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

            if (flipped) { uv.x += uv.width; uv.width = -uv.width; }
            UnityEngine.Color previousColor = GUI.color;
            GUI.BeginGroup(rect);
            Matrix4x4 previousMatrix = GUI.matrix;
            try
            {
                GUI.color = UnityEngine.Color.white;
                if (rotate)
                    UI.RotateAroundPivot(rotation.AsAngle + graphic.DrawRotatedExtraAngleOffset +
                        (flipped ? FlipExtraRotation ?? data.flipExtraRotation : 0f), image.center);
                Graphics.DrawTexture(image, texture, uv, 0, 0, 0, 0, UnityEngine.Color.white, previewMaterial);
            }
            finally
            {
                GUI.matrix = previousMatrix;
                GUI.color = previousColor;
                GUI.EndGroup();
            }
        }

        internal void ReleasePreview()
        {
            for (int i = 0; i < previewMaterials.Length; i++)
            {
                if (previewMaterials[i] != null)
                    UnityEngine.Object.Destroy(previewMaterials[i]);
                previewMaterials[i] = null;
            }
        }
    }
}