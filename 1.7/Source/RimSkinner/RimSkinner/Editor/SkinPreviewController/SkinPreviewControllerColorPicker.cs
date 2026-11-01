using RimWorld;
using UnityEngine;
using Verse;

namespace RimSkinner.Editor
{
    public sealed class SkinPreviewControllerColorPicker : Dialog_ColorPickerBase
    {
        public override Vector2 InitialSize => new Vector2(600f, 530f);
        private readonly Action<Color> onAccept;
        private readonly Color defaultColor;
        private readonly bool editAlpha;
        private static readonly List<Color> palette = new() { Color.white, Color.black, Color.gray, Color.red, Color.green, Color.blue };
        protected override bool ShowDarklight => false;
        protected override Color DefaultColor => defaultColor;
        protected override List<Color> PickableColors => palette;
        protected override float ForcedColorValue => -1f;
        protected override bool ShowColorTemperatureBar => false;

        public SkinPreviewControllerColorPicker(Color current, Color original, Action<Color> onAccept, bool editAlpha = false)
            : base(Widgets.ColorComponents.All, Widgets.ColorComponents.All)
        {
            color = oldColor = current;
            defaultColor = original;
            this.onAccept = onAccept;
            this.editAlpha = editAlpha;
        }

        protected override void SaveColor(Color selected)
        {
            if (!editAlpha) selected.a = oldColor.a;
            onAccept(selected);
        }
    }
}
