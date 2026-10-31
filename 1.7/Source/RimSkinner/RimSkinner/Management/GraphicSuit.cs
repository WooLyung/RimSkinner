#pragma warning disable CS8601
namespace RimSkinner.Management
{
    public sealed class GraphicSuit
    {
        private readonly Dictionary<string, GraphicSlot> slots = new();

        public GraphicSuit(Dictionary<string, GraphicSlot> slots)
        {
            foreach (var pair in slots)
                this.slots.TryAdd(pair.Key, pair.Value);
        }

        public bool TryGetSlot(string path, out GraphicSlot slot)
        {
            return slots.TryGetValue(path, out slot);
        }

        public IReadOnlyDictionary<string, GraphicSlot> Slots => slots.AsReadOnly();
    }
}