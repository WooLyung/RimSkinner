using UnityEngine;
using Verse;

namespace RimSkinner.Editor
{
    public sealed class SkinEditorInspectorElement_Bool : SkinEditorInspectorElement_Value<bool>
    {
        public override float Height => 54f;
        
        public SkinEditorInspectorElement_Bool(string label, Func<bool?> getter, Action<bool?> setter, Func<GraphicData, object> original)
            : base(label, getter, setter, original) 
        {
        }
        
        protected override void DrawValue(Rect rect, GraphicData data, bool useOriginal, bool originalChanged)
        {
            bool current = Current(data);

            if (useOriginal) 
                ReadOnly(rect, current.ToString());
            else if (Widgets.ButtonText(rect, current.ToString()))
                Value = !current;
        }
    }
}