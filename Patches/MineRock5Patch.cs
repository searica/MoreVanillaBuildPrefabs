using HarmonyLib;

namespace MVBP.Patches
{
    [HarmonyPatch(typeof(MineRock5))]
    internal static class MineRock5Patch
    {
        /// <summary>
        ///     Drop piece resources (if they exist) when MineRock5 is
        ///     completely destroyed to ensure a full refund even if
        ///     the MineRock5 was not removed with the hammer.
        ///     The patch applied to Piece.DropResources handles the rest.
        /// </summary>
        /// <param name="__instance"></param>
        /// <param name="__result"></param>
        [HarmonyPostfix]
        [HarmonyPatch(nameof(MineRock5.AllDestroyed))]
        private static void AllDestroyedPostfix(MineRock5 __instance, ref bool __result)
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