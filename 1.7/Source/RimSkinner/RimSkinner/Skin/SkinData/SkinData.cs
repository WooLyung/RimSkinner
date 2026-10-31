using RimSkinner.Management;
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
                    default:
                        break;
                }
            }

            return skin;
        }
    }
}
