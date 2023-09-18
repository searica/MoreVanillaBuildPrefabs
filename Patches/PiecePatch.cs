using HarmonyLib;

namespace MoreVanillaBuildPrefabs.Patches
{
    [HarmonyPatch]
    internal class PiecePatch
    {
        // Called when piece is just placed
        [HarmonyPatch(typeof(Piece), "SetCreator"), HarmonyPrefix]
        static void PieceSetCreatorPrefix(long uid, Piece __instance)
        {
            if (!PluginConfig.IsModEnabled.Value) { return; }
            
            // Synchronize the positions and rotations of otherwise non-persistent objects
            var view = __instance.GetComponent<ZNetView>();
            if (view && !view.m_persistent)
            {
                view.m_persistent = true;

                var sync = __instance.gameObject.GetComponent<ZSyncTransform>();
                if (sync == null)
                {
                    __instance.gameObject.AddComponent<ZSyncTransform>();
                }
                sync.m_syncPosition = true;
                sync.m_syncRotation = true;

            }            
        }

        [HarmonyPrefix]
        [HarmonyPatch(typeof(Piece), nameof(Piece.DropResources))]
        static void PieceDropResourcesPrefix(Piece __instance)
        {
            if (!PluginConfig.IsModEnabled.Value) { return; }

            // Only interact if it is a piece added by this mod
            if (PrefabAdder.AddedPieces.ContainsKey(__instance.m_name))
            {
                // disable desctruction drops for player built pieces
                // prevents things like player built dvergerprops_crate
                // dropping dvergr extractors even if they weren't used to build them
                if (__instance.IsPlacedByPlayer())
                {
                    Plugin.DisableDestructionDrops = true;
                }
                /* Could use a check to disable build resource drops 
                 * from world-generated pieces via returning false 
                 * in the harmony prefix patch, but this can prevent 
                 * them from dropping anything even when they would 
                 * normally drop something.
                 * 
                 * Instead just allow world-generated pieces affected 
                 * by this mod to drop the configured build resources
                 * along with their default resources as per vanilla 
                 * drop rules for non-player built pieces.
                 */
            }
        }
    }
}
