using RimSkinner.Management;
using UnityEngine;
using Verse;

namespace RimSkinner.Skin
{
    public sealed class SkinData
    {
        public static readonly int CurrentFormatVersion = 1;

        public int FormatVersion { get; set; }

        public string Id { get; init; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string ThingDefName { get; set; } = string.Empty;
        public Dictionary<string, SkinGraphicData> Graphics { get; set; } = new();
        public bool IsCompatible => FormatVersion == CurrentFormatVersion;

        public static SkinData Create(string thingDefName, string id, string name)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(thingDefName);
            ArgumentException.ThrowIfNullOrWhiteSpace(id);
            ThingDef def = DefDatabase<ThingDef>.GetNamed(thingDefName);
            GraphicSuit suit = ThingDefGraphics.GetGraphicSuit(def);
            var skin = new SkinData
            {
                FormatVersion = CurrentFormatVersion,
                Id = id.Trim(),
                Name = name,
                ThingDefName = thingDefName
            };

            foreach (var pair in suit.Slots)
            {
                GraphicData source = pair.Value.SourceData;

                switch (source.graphicClass)
                {
                    case Type type when type == typeof(Graphic_Single):
                        skin.Graphics.Add(pair.Key, new SkinGraphicSingle());
                        break;
                    case Type type when type == typeof(Graphic_Multi):
                        skin.Graphics.Add(pair.Key, new SkinGraphicMulti());
                        break;
                    default:
                        break;
                }
            }

            return skin;
        }

        public static Texture2D? CloneTexture2D(Texture2D? original)
        {
            if (original == null)
                return null;
            
            Texture2D clone = new Texture2D(original.width, original.height, original.format, original.mipmapCount > 1);
            return clone;
        }
    }
}
