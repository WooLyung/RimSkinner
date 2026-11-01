using RimSkinner.Editor;
using RimSkinner.Management;
using UnityEngine;

namespace RimSkinner.Skin
{
    public abstract class SkinGraphicData
    {
        public abstract IReadOnlyList<(int Index, string Name)> TextureOptions { get; }
        public Color? Color { get; set; }
        public Color? ColorTwo { get; set; }
        public Vector2? DrawSize { get; set; }
        public Vector3? DrawOffset { get; set; }
        public Vector3? DrawOffsetNorth { get; set; }
        public Vector3? DrawOffsetEast { get; set; }
        public Vector3? DrawOffsetSouth { get; set; }
        public Vector3? DrawOffsetWest { get; set; }
        public bool? DrawRotated { get; set; }
        public bool? AllowFlip { get; set; }
        public float? FlipExtraRotation { get; set; }
        public string? ShaderTypeDefName { get; set; }
        public bool? TextureRepeats { get; set; }
        public bool? AllowAtlasing { get; set; }
        public int? RenderQueue { get; set; }
        public float? OverlayOpacity { get; set; }
        public bool? AddTopAltitudeBias { get; set; }
        public bool? IgnoreThingDrawColor { get; set; }

        protected IEnumerable<SkinEditorInspectorElement> CommonElements
        {
            get
            {
                yield return new SkinEditorInspectorElement_Color("RimSkinner.Editor.Inspector.Color", () => Color, value => Color = value, original => original.color);
                yield return new SkinEditorInspectorElement_Color("RimSkinner.Editor.Inspector.ColorTwo", () => ColorTwo, value => ColorTwo = value, original => original.colorTwo);
                yield return new SkinEditorInspectorElement_Vector2("RimSkinner.Editor.Inspector.DrawSize", () => DrawSize, value => DrawSize = value, original => original.drawSize);
                yield return new SkinEditorInspectorElement_Vector3("RimSkinner.Editor.Inspector.DrawOffset", () => DrawOffset, value => DrawOffset = value, original => original.drawOffset);
                yield return new SkinEditorInspectorElement_Vector3("RimSkinner.Editor.Inspector.DrawOffsetNorth", () => DrawOffsetNorth, value => DrawOffsetNorth = value, original => original.drawOffsetNorth ?? original.drawOffset);
                yield return new SkinEditorInspectorElement_Vector3("RimSkinner.Editor.Inspector.DrawOffsetEast", () => DrawOffsetEast, value => DrawOffsetEast = value, original => original.drawOffsetEast ?? original.drawOffset);
                yield return new SkinEditorInspectorElement_Vector3("RimSkinner.Editor.Inspector.DrawOffsetSouth", () => DrawOffsetSouth, value => DrawOffsetSouth = value, original => original.drawOffsetSouth ?? original.drawOffset);
                yield return new SkinEditorInspectorElement_Vector3("RimSkinner.Editor.Inspector.DrawOffsetWest", () => DrawOffsetWest, value => DrawOffsetWest = value, original => original.drawOffsetWest ?? original.drawOffset);
                yield return new SkinEditorInspectorElement_Bool("RimSkinner.Editor.Inspector.DrawRotated", () => DrawRotated, value => DrawRotated = value, original => original.drawRotated);
                yield return new SkinEditorInspectorElement_Bool("RimSkinner.Editor.Inspector.AllowFlip", () => AllowFlip, value => AllowFlip = value, original => original.allowFlip);
                yield return new SkinEditorInspectorElement_Float("RimSkinner.Editor.Inspector.FlipExtraRotation", () => FlipExtraRotation, value => FlipExtraRotation = value, original => original.flipExtraRotation);
                yield return new SkinEditorInspectorElement_Shader(() => ShaderTypeDefName, value => ShaderTypeDefName = value, original => original.shaderType?.defName ?? "Cutout");
                yield return new SkinEditorInspectorElement_Bool("RimSkinner.Editor.Inspector.TextureRepeats", () => TextureRepeats, value => TextureRepeats = value, original => original.textureRepeats);
                yield return new SkinEditorInspectorElement_Bool("RimSkinner.Editor.Inspector.AllowAtlasing", () => AllowAtlasing, value => AllowAtlasing = value, original => original.allowAtlasing);
                yield return new SkinEditorInspectorElement_Int("RimSkinner.Editor.Inspector.RenderQueue", () => RenderQueue, value => RenderQueue = value, original => original.renderQueue);
                yield return new SkinEditorInspectorElement_Float("RimSkinner.Editor.Inspector.OverlayOpacity", () => OverlayOpacity, value => OverlayOpacity = value, original => original.overlayOpacity);
                yield return new SkinEditorInspectorElement_Bool("RimSkinner.Editor.Inspector.AddTopAltitudeBias", () => AddTopAltitudeBias, value => AddTopAltitudeBias = value, original => original.addTopAltitudeBias);
                yield return new SkinEditorInspectorElement_Bool("RimSkinner.Editor.Inspector.IgnoreThingDrawColor", () => IgnoreThingDrawColor, value => IgnoreThingDrawColor = value, original => original.ignoreThingDrawColor);
            }
        }

        public abstract SkinGraphicData Clone();
        public abstract IEnumerable<SkinEditorInspectorElement> Elements { get; }
        public abstract void DrawPreview(Rect rect, GraphicSlot original, int index, Color color, Color colorTwo, float pixelsPerCell);
    }
}
