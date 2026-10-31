using Verse;

namespace RimSkinner.Skin
{
    public static class SkinLibrary
    {
        private static readonly Dictionary<ThingDef, List<SkinData>> skins = new();

        public static IReadOnlyList<SkinData> GetSkins(ThingDef def)
        {
            return GetOrCreateList(def).AsReadOnly();
        }

        public static bool TryAdd(SkinData skin)
        {
            ArgumentNullException.ThrowIfNull(skin);
            ArgumentException.ThrowIfNullOrWhiteSpace(skin.Id);
            ArgumentException.ThrowIfNullOrWhiteSpace(skin.ThingDefName);

            ThingDef def = DefDatabase<ThingDef>.GetNamed(skin.ThingDefName);
            var list = GetOrCreateList(def);
            if (list.Any(existing => string.Equals(existing.Id.Trim(), skin.Id.Trim(), StringComparison.OrdinalIgnoreCase)))
                return false;

            list.Add(skin);
            return true;
        }

        private static List<SkinData> GetOrCreateList(ThingDef def)
        {
            ArgumentNullException.ThrowIfNull(def);
            if (def.category != ThingCategory.Building)
                throw new ArgumentException("The ThingDef must represent a building.", nameof(def));

            if (!skins.TryGetValue(def, out var list))
            {
                list = new List<SkinData>();
                skins.Add(def, list);
            }

            return list;
        }
    }
}
