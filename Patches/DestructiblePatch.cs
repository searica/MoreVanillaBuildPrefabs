// Ignore Spelling: MVBP

using HarmonyLib;
using MVBP.Extensions;
using MVBP.Helpers;
using MVBP.Configs;

namespace MVBP.Patches
{
    [HarmonyPatch(typeof(Destructible))]
    internal class DestructiblePatch
    {
        /// <summary>
        ///     Back to make player-built instances of pieces
        ///     added by MVBP drop their build resources when destroyed.
        /// </summary>
        /// <param name="__instance"></param>
        [HarmonyPrefix]
        [HarmonyPatch(nameof(Destructible.Destroy))]
        private static bool DestroyPrefix(Destructible __instance)
        {
            if (InitManager.IsPatchedByMod(__instance.gameObject))
            {
                var piece = __instance?.gameObject?.GetComponent<Piece>();
                if (piece != null && piece.IsPlacedByPlayer())
                {
                    piece.DropResources();
                    // If it got picked during DropResources then it may no longer be valid.
                    var nview = __instance.m_nview;
                    if (!nview.IsValid() || !nview.IsOwner())
                    {
                        if (ConfigManager.IsVerbosityMedium) { Log.LogInfo("Piece nview was destroyed during DropResources()"); }
                        return false;
                    }
                }
            }
            return true;
        }
    }
}