using HarmonyLib;

namespace MoreVanillaBuildPrefabs
{
    // Disables desctruction drops for player built pieces.
    // Only affects pieces added by this mod (see PiecePatch.cs).
    // Prevents things like player built dvergerprops_crate dropping
    // dvergr extractors when extractors were not used to build it.
    [HarmonyPatch(typeof(DropOnDestroyed))]
    internal static class DropOnDestroyedPatch
    {
        [HarmonyPrefix]
        [HarmonyPatch(nameof(DropOnDestroyed.OnDestroyed))]
        private static bool OnDestroyedPrefix(DropOnDestroyed __instance)
        {
            if (MoreVanillaBuildPrefabs.DisableDropOnDestroyed)
            {
                MoreVanillaBuildPrefabs.DisableDropOnDestroyed = false;
                return false;
            }
            return true;
        }
    }
}