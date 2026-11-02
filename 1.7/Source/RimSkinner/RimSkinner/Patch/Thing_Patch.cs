using HarmonyLib;
using RimSkinner.Editor;
using RimSkinner.Skin;
using RimSkinner.WorldComponents;
using RimWorld;
using UnityEngine;
using Verse;

namespace RimSkinner.Patch
{
    [HarmonyPatch(typeof(Thing), nameof(Thing.GetGizmos))]
    public static class Thing_Patch_GetGizmos
    {
        private static IEnumerable<Gizmo> Postfix(IEnumerable<Gizmo> __result, Thing __instance)
        {
            foreach (Gizmo gizmo in __result)
                yield return gizmo;

            if (__instance is not Building building)
                yield break;

            yield return new Command_Action
            {
                icon = ContentFinder<Texture2D>.Get("UI/Designators/rim_skinner"),
                defaultLabel = "RimSkinner.Gizmo.SkinSettings".Translate(),
                defaultDesc = "RimSkinner.Gizmo.SkinSettings.Tooltip".Translate(),
                action = () => OpenSkinMenu(building)
            };
        }

        private static void OpenSkinMenu(Building building)
        {
            if (building.Destroyed || Find.World == null)
                return;

            var options = new List<FloatMenuOption>()
            {
                new FloatMenuOption("RimSkinner.Gizmo.OpenEditor".Translate(), () => SkinEditor.Open(building.def))
                {
                    tooltip = new TipSignal("RimSkinner.Gizmo.OpenEditor.Tooltip".Translate())
                },
                new FloatMenuOption("RimSkinner.Gizmo.UnassignSkin".Translate(), () => {
                        WorldComponent_SkinAssignments.Instance.UnassignSkin(building);
                        Messages.Message(("RimSkinner.Gizmo.UnassignSkin.Completed").Translate(building.LabelNoParenthesis), MessageTypeDefOf.TaskCompletion, historical: false);
                    })
                {
                    tooltip = new TipSignal("RimSkinner.Gizmo.UnassignSkin.Tooltip".Translate())
                }
            };

            foreach (SkinData skin in SkinLibrary.GetSkins(building.def))
            {
                var selected = skin;
                options.Add(new FloatMenuOption($"{selected.Name} ({selected.Id})",
                    selected.IsCompatible ? () =>
                    {
                        if (!building.Destroyed && Find.World != null)
                        {
                            WorldComponent_SkinAssignments.Instance.AssignSkin(building, selected.Id);
                            Messages.Message(("RimSkinner.Gizmo.SelectSkin.Completed").Translate(building.LabelNoParenthesis, selected.Name), MessageTypeDefOf.TaskCompletion, historical: false);
                        }
                    } : null)
                {
                    tooltip = new TipSignal("RimSkinner.Gizmo.SelectSkin.Tooltip".Translate())
                });
            }

            Find.WindowStack.Add(new FloatMenu(options));
        }
    }

    [HarmonyPatch(typeof(Thing), nameof(Thing.Destroy))]
    public static class Thing_Patch_Destroy
    {
        private static void Postfix(Thing __instance)
        {
            if (__instance is Building && __instance.Destroyed && Find.World != null)
                WorldComponent_SkinAssignments.Instance.UnassignSkin(__instance);
        }
    }
}