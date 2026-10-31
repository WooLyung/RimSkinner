using UnityEngine;

namespace RimSkinner.Skin
{
    public abstract class SkinGraphicData
    {
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
    }
}