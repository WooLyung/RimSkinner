using UnityEngine;

namespace RimSkinner.Skin;

public sealed class SkinGraphicSingle : SkinGraphicData
{
    public Texture2D? Texture { get; set; }
    public Texture2D? Mask { get; set; }
}
