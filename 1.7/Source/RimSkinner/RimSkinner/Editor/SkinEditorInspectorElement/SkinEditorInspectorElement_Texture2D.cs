using UnityEngine;
using Verse;

namespace RimSkinner.Editor
{
    public sealed class SkinEditorInspectorElement_Texture2D : SkinEditorInspectorElement
    {
        private readonly string label;
        private readonly Func<Texture2D?> getter;
        private readonly Action<Texture2D?> setter;

        private string? error;
        private Task<string?>? pendingSelection;
        public override string Label => label.Translate();
        public override float Height => error == null ? 126f : 176f;
        protected override object? OverrideValue { get => getter(); set => setter((Texture2D?)value); }
        
        public SkinEditorInspectorElement_Texture2D(string label, Func<Texture2D?> getter, Action<Texture2D?> setter, Func<GraphicData, object> original)
            : base(original) 
        { 
            this.label = label; 
            this.getter = getter; 
            this.setter = setter; 
        }

        protected override void DrawValue(Rect rect, GraphicData data, bool useOriginal, bool originalChanged)
        {
            var texture = getter() ?? (Texture2D)originalGetter(data);
            GUI.DrawTexture(new Rect(rect.x, rect.y, rect.width, 64f), texture, ScaleMode.ScaleToFit);
           
            if (!useOriginal)
            {
                bool previousEnabled = GUI.enabled;
                try
                {
                    GUI.enabled = previousEnabled && pendingSelection == null;
                    if (Widgets.ButtonText(new Rect(rect.x, rect.y + 68f, rect.width, 26f), "RimSkinner.Editor.LoadPng".Translate())
                        && pendingSelection == null)
                    {
                        try { pendingSelection = SkinPngFilePicker.OpenAsync(); }
                        catch (Exception ex) { ReportError(ex); }
                    }
                }
                finally { GUI.enabled = previousEnabled; }
            }
            if (pendingSelection?.IsCompleted == true)
            {
                var completed = pendingSelection;
                pendingSelection = null;
                try
                {
                    string? file = completed.GetAwaiter().GetResult();
                    if (file != null && !useOriginal)
                    {
                        if (!string.Equals(System.IO.Path.GetExtension(file), ".png", StringComparison.OrdinalIgnoreCase))
                            throw new ArgumentException("Select a PNG file.");
                        Texture2D loaded = Texture2D.LoadFromFile(file);
                        if (loaded == null) throw new InvalidOperationException("Could not load the PNG file.");
                        loaded.name = System.IO.Path.GetFileName(file);
                        setter(loaded);
                        error = null;
                    }
                }
                catch (Exception ex) { ReportError(ex); }
            }

            if (error != null) 
                Widgets.Label(new Rect(rect.x, rect.y + 98f, rect.width, 48f), error);
        }

        private void ReportError(Exception ex)
        {
            error = ex.GetBaseException().Message;
            Log.Warning($"[RimSkinner] Texture import failed: {ex}");
        }
    }
}