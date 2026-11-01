using UnityEngine;
using Verse;

namespace RimSkinner.Editor
{
    public abstract class SkinEditorInspectorElement_Value<T> : SkinEditorInspectorElement where T : struct
    {
        private readonly string label;
        private readonly Func<T?> getter;
        private readonly Action<T?> setter;
        public override string Label => label.Translate();
        public T? Value { get => getter(); set => setter(value); }
        protected override object? OverrideValue { get => Value; set => Value = (T?)value; }
       
        protected SkinEditorInspectorElement_Value(string label, Func<T?> getter, Action<T?> setter, Func<GraphicData, object> original)
            : base(original)
        {
            this.label = label; 
            this.getter = getter; 
            this.setter = setter; 
        }

        protected T Current(GraphicData data) => Value ?? (T)originalGetter(data);

        protected static void ReadOnly(Rect rect, string text)
        {
            Widgets.DrawBoxSolid(rect, new Color(0f, 0f, 0f, 0.2f));
            Widgets.Label(rect.ContractedBy(3f), text);
        }
    }
}