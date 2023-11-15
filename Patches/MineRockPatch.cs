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
            Log.LogInfo("MineRock.UpdateVisability patch applied", LogLevel.Medium);
            return __instance.m_nview != null;
        }

        /// <summary>
        ///     Patch to drop resources when MineRock is destroyed
        /// </summary>
        /// <param name="__instance"></param>
        /// <param name="__result"></param>
        [HarmonyPostfix]
        [HarmonyPatch(nameof(MineRock.AllDestroyed))]
        private static void AllDestroyed(MineRock __instance, ref bool __result)
        {
            if (__result && __instance?.gameObject != null)
            {
                if (__instance.gameObject.TryGetComponent(out Piece piece))
                {
                    piece.DropResources();
                }
            }
        }
    }
}