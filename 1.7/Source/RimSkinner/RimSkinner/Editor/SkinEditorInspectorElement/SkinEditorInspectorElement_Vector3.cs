using UnityEngine;
using Verse;
using System.Globalization;

namespace RimSkinner.Editor
{
    public sealed class SkinEditorInspectorElement_Vector3 : SkinEditorInspectorElement_Value<Vector3>
    {
        private readonly string[] buffers = new string[3];
        private Vector3? lastValue;
        public override float Height => 114f;

        public SkinEditorInspectorElement_Vector3(string label, Func<Vector3?> getter, Action<Vector3?> setter, Func<GraphicData, object> original)
            : base(label, getter, setter, original)
        { 
        }

        protected override void DrawValue(Rect rect, GraphicData data, bool useOriginal, bool originalChanged)
        {
            Vector3 current = Current(data);

            if (!lastValue.HasValue || lastValue.Value != current || originalChanged)
            {
                buffers[0] = current.x.ToString(CultureInfo.InvariantCulture);
                buffers[1] = current.y.ToString(CultureInfo.InvariantCulture);
                buffers[2] = current.z.ToString(CultureInfo.InvariantCulture);
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
           
            Widgets.Label(new Rect(rect.x, rect.y + 60f, 18f, 26f), "Z");
            Rect row2 = new Rect(rect.x + 22f, rect.y + 60f, rect.width - 22f, 26f);
           
            if (useOriginal) 
                ReadOnly(row2, buffers[2]);
            else 
                Widgets.TextFieldNumeric(row2, ref current.z, ref buffers[2], float.MinValue, float.MaxValue);
            
            if (!useOriginal && current != before)
                Value = current;
           
            lastValue = current;
        }
    }
}