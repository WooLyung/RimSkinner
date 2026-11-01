using UnityEngine;
using Verse;

namespace RimSkinner.Editor;

public sealed class SkinEditorInspectorElement_Shader : SkinEditorInspectorElement
{
    private readonly Func<string?> getter;
    private readonly Action<string?> setter;
    public override string Label => "RimSkinner.Editor.Inspector.ShaderTypeDefName".Translate();
    public override float Height => 54f;
    protected override object? OverrideValue { get => getter(); set => setter((string?)value); }

    public SkinEditorInspectorElement_Shader(Func<string?> getter, Action<string?> setter, Func<GraphicData, object> original)
        : base(original) 
    {
        this.getter = getter; 
        this.setter = setter;
    }

    protected override void DrawValue(Rect rect, GraphicData data, bool useOriginal, bool originalChanged)
    {
        string current = getter() ?? (string)originalGetter(data);

        if (useOriginal)
        {
            Widgets.DrawBoxSolid(rect, new Color(0f, 0f, 0f, 0.2f));
            Widgets.Label(rect.ContractedBy(3f), current);
        }
        else if (Widgets.ButtonText(rect, current))
        {
            var options = DefDatabase<ShaderTypeDef>.AllDefsListForReading
                .OrderBy(def => def.defName)
                .Select(def => new FloatMenuOption(def.defName, () => setter(def.defName)))
                .ToList();
            Find.WindowStack.Add(new FloatMenu(options));
        }
    }
}