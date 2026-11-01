using System.Collections;
using System.Reflection;
using Verse;

namespace RimSkinner.Management
{
    public static class ThingDefGraphics
    {
        private static readonly Dictionary<ThingDef, GraphicSuit> cache = new();

        public static bool RemoveCachedGraphics(ThingDef def) => cache.Remove(def);

        public static GraphicSuit GetGraphicSuitTemporary(ThingDef def)
        {
            ArgumentNullException.ThrowIfNull(def);
            if (def.category != ThingCategory.Building)
                throw new ArgumentException("The ThingDef must represent a building.", nameof(def));

            if (cache.TryGetValue(def, out var cached))
                return cached;

            Dictionary<string, GraphicSlot> graphics = new();
            var ancestors = new HashSet<object>(ReferenceEqualityComparer.Instance);
            Visit(def, def.defName);

            var result = new GraphicSuit(graphics);
            return result;

            void Visit(object? value, string path)
            {
                if (value == null)
                    return;

                Type type = value.GetType();
                if (value is Def && !ReferenceEquals(value, def))
                    return;
                if (value is string || type.IsPrimitive || type.IsEnum || value is Type)
                    return;
                if (!ancestors.Add(value))
                    return;

                try
                {
                    if (value is GraphicData graphicData)
                        graphics.Add(path, new GraphicSlot(graphicData));

                    if (value is IEnumerable items)
                    {
                        int index = 0;
                        foreach (object? item in items)
                            Visit(item, $"{path}[{index++}]");
                        return;
                    }

                    if (value is Graphic || type.Namespace?.StartsWith("System", StringComparison.Ordinal) == true || type.Namespace?.StartsWith("Unity", StringComparison.Ordinal) == true)
                        return;

                    foreach (FieldInfo field in type.GetFields(BindingFlags.Instance | BindingFlags.Public))
                        Visit(field.GetValue(value), $"{path}.{field.Name}");
                }
                finally
                {
                    ancestors.Remove(value);
                }
            }
        }

        public static GraphicSuit GetGraphicSuit(ThingDef def)
        {
            ArgumentNullException.ThrowIfNull(def);
            if (def.category != ThingCategory.Building)
                throw new ArgumentException("The ThingDef must represent a building.", nameof(def));

            if (cache.TryGetValue(def, out var cached))
                return cached;

            Dictionary<string, GraphicSlot> graphics = new();
            var ancestors = new HashSet<object>(ReferenceEqualityComparer.Instance);
            Visit(def, def.defName);

            var result = new GraphicSuit(graphics);
            cache.Add(def, result);

            return result;

            void Visit(object? value, string path)
            {
                if (value == null)
                    return;

                Type type = value.GetType();
                if (value is Def && !ReferenceEquals(value, def))
                    return;
                if (value is string || type.IsPrimitive || type.IsEnum || value is Type)
                    return;
                if (!ancestors.Add(value))
                    return;

                try
                {
                    if (value is GraphicData graphicData)
                        graphics.Add(path, new GraphicSlot(graphicData));

                    if (value is IEnumerable items)
                    {
                        int index = 0;
                        foreach (object? item in items)
                            Visit(item, $"{path}[{index++}]");
                        return;
                    }

                    if (value is Graphic || type.Namespace?.StartsWith("System", StringComparison.Ordinal) == true || type.Namespace?.StartsWith("Unity", StringComparison.Ordinal) == true)
                        return;

                    foreach (FieldInfo field in type.GetFields(BindingFlags.Instance | BindingFlags.Public))
                        Visit(field.GetValue(value), $"{path}.{field.Name}");
                }
                finally
                {
                    ancestors.Remove(value);
                }
            }
        }
    }
}

