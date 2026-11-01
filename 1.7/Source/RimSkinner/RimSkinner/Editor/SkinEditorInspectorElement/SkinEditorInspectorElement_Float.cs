using UnityEngine;
using Verse;
using System.Globalization;

namespace RimSkinner.Editor
{
    public sealed class SkinEditorInspectorElement_Float : SkinEditorInspectorElement_Value<float>
    {
        private string buffer = string.Empty;
        private float? lastValue;
        public override float Height => 54f;

        public SkinEditorInspectorElement_Float(string label, Func<float?> getter, Action<float?> setter, Func<GraphicData, object> original)
            : base(label, getter, setter, original)
        { 
        }
        protected override void DrawValue(Rect rect, GraphicData data, bool useOriginal, bool originalChanged)
        {
            float current = Current(data);
            if (lastValue != current || originalChanged) 
                buffer = current.ToString(CultureInfo.InvariantCulture);
            
            var before = current;
            if (useOriginal) 
                ReadOnly(rect, buffer);
            else 
                Widgets.TextFieldNumeric(rect, ref current, ref buffer, float.MinValue, float.MaxValue);
            
            if (!useOriginal && current != before) 
                Value = current;
            lastValue = current;
        }
    }
}