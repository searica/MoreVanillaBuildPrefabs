using HarmonyLib;

namespace MoreVanillaBuildPrefabs.Patches
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
        private static void DestroyPrefix(Destructible __instance)
        {
            var prefabName = __instance?.gameObject?.name?.RemoveSuffix("(Clone)");
            if (MoreVanillaBuildPrefabs.IsPatchedByMod(prefabName))
            {
                var piece = __instance?.gameObject?.GetComponent<Piece>();
                if (piece != null && piece.IsPlacedByPlayer())
                {
                    piece.DropResources();
                }
            }
        }
    }
}