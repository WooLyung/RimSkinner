using HarmonyLib;
using RimWorld;
using System.Reflection;
using Verse;

namespace RimSkinner.Core
{
    [StaticConstructorOnStartup]
    public class RimSkinnerMod
    {
        public static Harmony Harmony { get; private set; }

        static RimSkinnerMod()
        {
            Harmony = new Harmony("ng.lyu.rimskinner");
            Harmony.PatchAll(Assembly.GetExecutingAssembly());
            foreach (string defName in new[] { "Bed", "MechGestator" })
            {
                ThingDef def = DefDatabase<ThingDef>.GetNamedSilentFail(defName);
                if (def == null)
                {
                    Log.Message($"[RimSkinner] Skipping graphics extraction: {defName} is not loaded.");
                    continue;
                }

                for (int pass = 1; pass <= 2; pass++)
                {
                    var suit = RimSkinner.Management.ThingDefGraphics.GetGraphicSuit(def);
                    foreach (string path in suit.Slots.Keys)
                        Log.Message($"[RimSkinner] {defName} graphics pass {pass}: {path}");
                }
            }
        }
    }
}
