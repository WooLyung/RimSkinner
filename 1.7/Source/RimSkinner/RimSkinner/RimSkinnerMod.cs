using HarmonyLib;
using System.Reflection;
using Verse;

namespace RimSkinner
{
    [StaticConstructorOnStartup]
    public class RimSkinnerMod
    {
        public static Harmony Harmony { get; private set; }

        static RimSkinnerMod()
        {
            Harmony = new Harmony("ng.lyu.rimskinner");
            Harmony.PatchAll(Assembly.GetExecutingAssembly());

            Log.Message("RimSkinner Activate");
        }
    }
}
