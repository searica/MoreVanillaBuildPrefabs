// Ignore Spelling: MVBP

using HarmonyLib;

namespace MVBP.Patches
{
    [HarmonyPatch(typeof(FejdStartup))]
    internal static class FejdStartupPatch
    {
        /// <summary>
        ///     Patch to check if world modifiers for resources are active
        ///     and re-initialize the mod if they are so pickables have the
        ///     correct build requirement costs.
        /// </summary>
        [HarmonyPrefix]
        [HarmonyPatch(nameof(FejdStartup.Awake))]
        public static void FejdStartup_Awakex_Prefix()
        {
            Log.LogInfo("FejdStartup.Awake");
        }
    }
}