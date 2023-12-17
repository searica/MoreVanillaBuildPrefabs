// Ignore Spelling: MVBP
using HarmonyLib;
using MVBP.Helpers;

namespace MVBP {
    /// <summary>
    ///     Disables destruction drops for player built pieces.
    ///     Prevents things like player built dvergerprops_crate dropping
    ///     dvergr extractors when extractors were not used to build it.
    /// </summary>
    [HarmonyPatch(typeof(DropOnDestroyed))]
    internal static class DropOnDestroyedPatch {
        [HarmonyPrefix]
        [HarmonyPatch(nameof(DropOnDestroyed.OnDestroyed))]
        private static bool OnDestroyedPrefix(DropOnDestroyed __instance) {
            if (__instance &&
                InitManager.IsPatchedByMod(__instance) &&
                __instance.TryGetComponent(out Piece piece)
                && piece.IsPlacedByPlayer()) {
                Log.LogInfo("Disabling on destroyed drops for player-built object", LogLevel.Medium);
                return false;
            }

            return true;
        }
    }
}