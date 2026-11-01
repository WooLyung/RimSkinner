using UnityEngine;
using Verse;

namespace RimSkinner.Editor
{
    public sealed class SkinEditorInspectorElement_Color : SkinEditorInspectorElement_Value<Color>
    {
        public override float Height => 54f;

        public SkinEditorInspectorElement_Color(string label, Func<Color?> getter, Action<Color?> setter, Func<GraphicData, object> original)
            : base(label, getter, setter, original)
        { 
        }

        protected override void DrawValue(Rect rect, GraphicData data, bool useOriginal, bool originalChanged)
        {
            Color current = Current(data);
            Widgets.DrawBoxSolid(rect, current);
            Widgets.DrawBox(rect);

            if (!useOriginal && Widgets.ButtonInvisible(rect))
                Find.WindowStack.Add(new SkinPreviewControllerColorPicker(current, (Color)originalGetter(data), value => Value = value, true));
        }
    }
}