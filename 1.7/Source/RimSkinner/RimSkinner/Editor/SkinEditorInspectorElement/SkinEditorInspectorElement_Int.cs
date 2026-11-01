using UnityEngine;
using Verse;
using System.Globalization;

namespace RimSkinner.Editor
{
    public sealed class SkinEditorInspectorElement_Int : SkinEditorInspectorElement_Value<int>
    {
        private string buffer = string.Empty;
        private int? lastValue;
        public override float Height => 54f;

        public SkinEditorInspectorElement_Int(string label, Func<int?> getter, Action<int?> setter, Func<GraphicData, object> original)
            : base(label, getter, setter, original)
        {
        }

        protected override void DrawValue(Rect rect, GraphicData data, bool useOriginal, bool originalChanged)
        {
            int current = Current(data);
            if (lastValue != current || originalChanged)
                buffer = current.ToString(CultureInfo.InvariantCulture);

            var before = current;
            if (useOriginal)
                ReadOnly(rect, buffer);
            else
                Widgets.TextFieldNumeric(rect, ref current, ref buffer, int.MinValue, int.MaxValue);

            if (!useOriginal && current != before)
                Value = current;
            lastValue = current;
        }
    }
}