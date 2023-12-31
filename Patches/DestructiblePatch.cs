﻿// Ignore Spelling: MVBP

using HarmonyLib;
using MVBP.Helpers;

namespace MVBP.Patches
{
    [HarmonyPatch(typeof(Destructible))]
    internal static class DestructiblePatch
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
                        Log.LogInfo("Piece nview was destroyed during DropResources()", LogLevel.Medium);
                        return false;
                    }
                }
            }
            return true;
        }
    }
}