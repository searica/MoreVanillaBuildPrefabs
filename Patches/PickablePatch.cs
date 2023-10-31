// Ignore Spelling: MVBP

using HarmonyLib;
using MoreVanillaBuildPrefabs.Helpers;

namespace MVBP.Patches
{
    [HarmonyPatch(typeof(Pickable))]
    internal class PickablePatch
    {
        private static readonly DropTable emptyDrops = new();

        [HarmonyPrefix]
        [HarmonyPatch(nameof(Pickable.RPC_Pick))]
        private static void RPCPickPrefix(Pickable __instance, out DropTable __state)
        {
            var piece = __instance.GetComponent<Piece>();
            var prefabName = __instance.gameObject.name.RemoveSuffix("(Clone)");
            if (InitManager.IsPatchedByMod(prefabName)
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
        private static void RPCPickPostfix(Pickable __instance, DropTable __state)
        {
            if (__state != null)
            {
                __instance.m_extraDrops = __state;
            }
        }
    }
}