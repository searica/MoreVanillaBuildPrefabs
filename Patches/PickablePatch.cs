// Ignore Spelling: MVBP

using HarmonyLib;
using MVBP.Helpers;

namespace MVBP.Patches
{
    [HarmonyPatch(typeof(Pickable))]
    internal static class PickablePatch
    {
        private static readonly DropTable emptyDrops = new();

        [HarmonyPrefix]
        [HarmonyPatch(nameof(Pickable.RPC_Pick))]
        private static void RPC_PickPrefix(Pickable __instance, out DropTable __state)
        {
            var piece = __instance.GetComponent<Piece>();
            if (InitManager.IsPatchedByMod(__instance)
                && piece != null && piece.IsPlacedByPlayer())
            {
                __state = __instance.m_extraDrops;
                __instance.m_extraDrops = emptyDrops;
            }
            else
            {
                __state = null;
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(nameof(Pickable.RPC_Pick))]
        private static void RPC_PickPostfix(Pickable __instance, DropTable __state)
        {
            if (__state != null)
            {
                __instance.m_extraDrops = __state;
            }
        }
    }
}