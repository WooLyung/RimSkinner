using UnityEngine;
using Verse;

namespace RimSkinner.Editor
{
    public sealed class SkinEditorInspectorElement_String : SkinEditorInspectorElement
    {
        private readonly string label;
        private readonly Func<string?> getter;
        private readonly Action<string?> setter;
        public override string Label => label.Translate();
        public override float Height => 54f;
        protected override object? OverrideValue { get => getter(); set => setter((string?)value); }
       
        public SkinEditorInspectorElement_String(string label, Func<string?> getter, Action<string?> setter, Func<GraphicData, object> original)
            : base(original) 
        {
            this.label = label; 
            this.getter = getter; 
            this.setter = setter; 
        }
       
        protected override void DrawValue(Rect rect, GraphicData data, bool useOriginal, bool originalChanged)
        {
            string current = getter() ?? (string)originalGetter(data);

            if (useOriginal)
                Widgets.Label(rect, current);
            else 
            { 
                string next = Widgets.TextField(rect, current);
                if (next != current) 
                    setter(next); 
            }
        }
    }
}