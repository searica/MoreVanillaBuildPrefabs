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
        static bool OnDestroyedPrefix(DropOnDestroyed __instance)
        {
            if (MoreVanillaBuildPrefabs.DisableDestructionDrops)
            {
                MoreVanillaBuildPrefabs.DisableDestructionDrops = false;
                return false;
            }
            return true;
        }
    }
}