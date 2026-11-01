using UnityEngine;
using Verse;
using System.Globalization;

namespace RimSkinner.Editor
{
    public sealed class SkinEditorInspectorElement_Vector2 : SkinEditorInspectorElement_Value<Vector2>
    {
        private readonly string[] buffers = new string[2];
        private Vector2? lastValue;
        public override float Height => 84f;

        public SkinEditorInspectorElement_Vector2(string label, Func<Vector2?> getter, Action<Vector2?> setter, Func<GraphicData, object> original)
            : base(label, getter, setter, original)
        {
        }

        protected override void DrawValue(Rect rect, GraphicData data, bool useOriginal, bool originalChanged)
        {
            Vector2 current = Current(data);

            if (!lastValue.HasValue || lastValue.Value != current || originalChanged)
            {
                buffers[0] = current.x.ToString(CultureInfo.InvariantCulture);
                buffers[1] = current.y.ToString(CultureInfo.InvariantCulture);
            }

            var before = current;
            Widgets.Label(new Rect(rect.x, rect.y + 0f, 18f, 26f), "X");
            Rect row0 = new Rect(rect.x + 22f, rect.y + 0f, rect.width - 22f, 26f);
            
            if (useOriginal) 
                ReadOnly(row0, buffers[0]);
            else
                Widgets.TextFieldNumeric(row0, ref current.x, ref buffers[0], float.MinValue, float.MaxValue);
           
            Widgets.Label(new Rect(rect.x, rect.y + 30f, 18f, 26f), "Y");
            Rect row1 = new Rect(rect.x + 22f, rect.y + 30f, rect.width - 22f, 26f);
           
            if (useOriginal) 
                ReadOnly(row1, buffers[1]);
            else 
                Widgets.TextFieldNumeric(row1, ref current.y, ref buffers[1], float.MinValue, float.MaxValue);
            
            if (!useOriginal && current != before) 
                Value = current;
           
            lastValue = current;
        }
    }
}