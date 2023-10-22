using HarmonyLib;
using MoreVanillaBuildPrefabs.Configs;
using MoreVanillaBuildPrefabs.Logging;
using MoreVanillaBuildPrefabs.Helpers;

namespace MoreVanillaBuildPrefabs
{
    /// <summary>
    ///     Disables desctruction drops for player built pieces.
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
            var prefabName = NameHelper.RemoveSuffix(__instance.name, "(Clone)");
            if (MoreVanillaBuildPrefabs.IsChangedByMod(prefabName))
            {
                var piece = __instance.gameObject.GetComponent<Piece>();
                if (piece != null && piece.IsPlacedByPlayer())
                {
                    if (PluginConfig.IsVerbosityMedium)
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