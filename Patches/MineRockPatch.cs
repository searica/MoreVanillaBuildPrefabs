// Ignore Spelling: MVBP

using HarmonyLib;

namespace MVBP.Patches
{
    [HarmonyPatch(typeof(MineRock))]
    internal static class MineRockPatch
    {
        /// <summary>
        ///     Prevent MineRock from updating visibility until
        ///     after it acquires a ZNetView (such as by restarting
        ///     the game or hitting it with something).
        /// </summary>
        /// <param name="__instance"></param>
        /// <returns></returns>
        [HarmonyPrefix]
        [HarmonyPatch(nameof(MineRock.UpdateVisability))]
        private static bool UpdateVisabilityPrefix(MineRock __instance)
        {
            Log.LogInfo("MineRock.UpdateVisability patch applied", LogLevel.High);
            // Try to avoid accessing value from null object?
            return __instance && __instance.m_nview != null;
        }
    }
}
