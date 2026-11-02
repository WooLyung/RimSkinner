using HarmonyLib;
using RimSkinner.Core;
using RimSkinner.Editor;
using RimSkinner.WorldComponents;
using System.Reflection;
using UnityEngine;
using Verse;

namespace RimSkinner.Patch
{
    [HarmonyPatch(typeof(Graphic), nameof(Graphic.Print))]
    public static class Graphic_Patch_Print
    {
        private static bool Prefix(Graphic __instance, SectionLayer layer, Thing thing, float extraRotation)
        {
            var replacement = Graphic_Patcher.Resolve(__instance, thing);
            if (replacement == null) 
                return true;

            replacement.Print(layer, thing, extraRotation);
            return false;
        }
    }

    [HarmonyPatch(typeof(Graphic), nameof(Graphic.DrawWorker))]
    public static class Graphic_Patch_DrawWorker
    {
        private static bool Prefix(Graphic __instance, Vector3 loc, Rot4 rot, ThingDef thingDef, Thing thing, float extraRotation)
        {
            var replacement = Graphic_Patcher.Resolve(__instance, thing);
            if (replacement == null) 
                return true;

            replacement.DrawWorker(loc, rot, thingDef, thing, extraRotation);
            return false;
        }
    }

    [HarmonyPatch]
    public static class Graphic_Patch_Assignments
    {
        private static IEnumerable<MethodBase> TargetMethods()
        {
            yield return AccessTools.Method(typeof(WorldComponent_SkinAssignments), nameof(WorldComponent_SkinAssignments.AssignSkin));
            yield return AccessTools.Method(typeof(WorldComponent_SkinAssignments), nameof(WorldComponent_SkinAssignments.UnassignSkin));
        }
        private static void Postfix(Thing building) => Graphic_Patcher.Dirty(building);
    }

    [HarmonyPatch(typeof(SkinEditorContent), nameof(SkinEditorContent.Save))]
    public static class Graphic_Patch_Save
    {
        private static void Postfix()
        {
            if (Find.World == null) return;
            foreach (var thing in WorldComponent_SkinAssignments.Instance.Assignments.Keys)
                Graphic_Patcher.Dirty(thing);
        }
    }
}