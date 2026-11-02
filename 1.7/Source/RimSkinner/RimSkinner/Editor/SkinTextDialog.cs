using UnityEngine;
using Verse;

namespace RimSkinner.Editor;

internal sealed class SkinTextDialog : Window
{
    private string value;
    private readonly Func<string, string?> submit;
    public override Vector2 InitialSize => new Vector2(420f, 140f);

    public SkinTextDialog(string title, string initial, Func<string, string?> submit)
    {
        optionalTitle = title;
        value = initial;
        this.submit = submit;
        forcePause = true;
        absorbInputAroundWindow = true;
        closeOnClickedOutside = false;
        closeOnAccept = false;
        doCloseX = true;
    }

    public override void DoWindowContents(Rect rect)
    {
        value = Widgets.TextField(new Rect(0f, 0f, rect.width, 30f), value);
        if (Widgets.ButtonText(new Rect(0f, rect.height - 30f, rect.width * 0.5f - 4f, 30f), "CancelButton".Translate()))
            Close();
        if (Widgets.ButtonText(new Rect(rect.width * 0.5f + 4f, rect.height - 30f, rect.width * 0.5f - 4f, 30f), "OK".Translate()))
        {
            string? error = submit(value.Trim());
            if (error == null) Close();
            else Messages.Message(error, RimWorld.MessageTypeDefOf.RejectInput, historical: false);
        }
    }
}