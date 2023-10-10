using HarmonyLib;
using MoreVanillaBuildPrefabs.Configs;

namespace MoreVanillaBuildPrefabs
{
    // Disables desctruction drops for player built pieces.
    // Only affects pieces added by this mod (see PiecePatch.cs).
    // Prevents things like player built dvergerprops_crate dropping
    // dvergr extractors when extractors were not used to build it.
    [HarmonyPatch(typeof(DropOnDestroyed))]
    static class DropOnDestroyedPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch(nameof(DropOnDestroyed.OnDestroyed))]
#pragma warning disable IDE0060 // Remove unused parameter
        static bool OnDestroyedPrefix(DropOnDestroyed __instance)
#pragma warning restore IDE0060 // Remove unused parameter
        {
            if (PluginConfig.IsModEnabled.Value)
            {
                if (Plugin.DisableDestructionDrops)
                {
                    Plugin.DisableDestructionDrops = false;
                    return false;
                }
            }
            return true;
        }
    }
}