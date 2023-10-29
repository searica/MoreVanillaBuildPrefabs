// Ignore Spelling: MVBP

using HarmonyLib;
using MVBP.Configs;

namespace MVBP.Patches
{
    [HarmonyPatch(typeof(MineRock))]
    internal class MineRockPatch
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
            if (Config.IsVerbosityMedium)
            {
                Log.LogInfo("MineRock.UpdateVisability patch");
            }
            return __instance.m_nview != null;
        }
    }
}