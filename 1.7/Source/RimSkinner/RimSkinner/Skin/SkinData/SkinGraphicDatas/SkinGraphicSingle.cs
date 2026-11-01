using UnityEngine;
using static UnityEngine.UI.Image;

namespace RimSkinner.Skin;

public sealed class SkinGraphicSingle : SkinGraphicData
{
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
}
