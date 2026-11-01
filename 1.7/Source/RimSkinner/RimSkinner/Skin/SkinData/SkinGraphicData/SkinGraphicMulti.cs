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
}
