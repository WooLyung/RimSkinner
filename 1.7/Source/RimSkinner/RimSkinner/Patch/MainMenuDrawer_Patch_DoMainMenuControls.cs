using System.Reflection.Emit;
using HarmonyLib;
using RimSkinner.Editor;
using RimWorld;
using UnityEngine;
using Verse;

namespace RimSkinner.Patch
{
    [HarmonyPatch(typeof(MainMenuDrawer), nameof(MainMenuDrawer.DoMainMenuControls))]
    public static class MainMenuDrawer_Patch_DoMainMenuControls
    {
        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var original = AccessTools.Method(typeof(OptionListingUtility), nameof(OptionListingUtility.DrawOptionListing));
            var replacement = AccessTools.Method(typeof(MainMenuDrawer_Patch_DoMainMenuControls), nameof(DrawOptions));

            foreach (var instruction in instructions)
            {
                if (instruction.Calls(original))
                {
                    instruction.opcode = OpCodes.Call;
                    instruction.operand = replacement;
                }

                yield return instruction;
            }
        }

        private static float DrawOptions(Rect rect, List<ListableOption> options)
        {
            if (Current.ProgramState == ProgramState.Entry)
            {
                string modsLabel = "Mods".Translate();
                int index = options.FindIndex(option => option.label == modsLabel);
                if (index >= 0)
                    options.Insert(index + 1, new ListableOption("RimSkinner".Translate(), OpenEditor));
            }

            return OptionListingUtility.DrawOptionListing(rect, options);
        }

        private static void OpenEditor()
        {
            SkinEditor.Open();
        }
    }
}
