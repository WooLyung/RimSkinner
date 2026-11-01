using UnityEngine;

namespace RimSkinner.Skin;

public sealed class SkinGraphicMulti : SkinGraphicData
{
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
}
