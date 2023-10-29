﻿// Ignore Spelling: MVBP

using HarmonyLib;
using MVBP.Configs;

namespace MVBP
{
    /// <summary>
    ///     Disables destruction drops for player built pieces.
    ///     Prevents things like player built dvergerprops_crate dropping
    ///     dvergr extractors when extractors were not used to build it.
    /// </summary>
    [HarmonyPatch(typeof(DropOnDestroyed))]
    internal static class DropOnDestroyedPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch(nameof(DropOnDestroyed.OnDestroyed))]
        private static bool OnDestroyedPrefix(DropOnDestroyed __instance)
        {
            var prefabName = __instance.name.RemoveSuffix("(Clone)");
            if (InitManager.IsPatchedByMod(prefabName))
            {
                var piece = __instance.gameObject.GetComponent<Piece>();
                if (piece != null && piece.IsPlacedByPlayer())
                {
                    if (Config.IsVerbosityMedium)
                    {
                        Log.LogInfo("Disabling on destroyed drops for player-built object");
                    }
                    return false;
                }
            }
            return true;
        }
    }
}