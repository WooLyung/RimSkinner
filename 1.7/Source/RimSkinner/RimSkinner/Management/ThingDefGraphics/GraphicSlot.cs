using Verse;

namespace RimSkinner.Management
{
    public sealed class GraphicSlot
    {
        public GraphicData SourceData { get; }
        public Graphic? SourceGraphic => SourceData.Graphic;

        public GraphicSlot(GraphicData sourceData)
        {
            SourceData = sourceData;
        }
    }
}
