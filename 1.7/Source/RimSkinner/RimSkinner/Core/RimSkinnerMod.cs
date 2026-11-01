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
        }
    }
}
