using System;
using HarmonyLib;

using MoreVanillaBuildPrefabs.Logging;
using MoreVanillaBuildPrefabs.Configs;
using MoreVanillaBuildPrefabs.Helpers;
using static MoreVanillaBuildPrefabs.MoreVanillaBuildPrefabs;


namespace MoreVanillaBuildPrefabs
{
    [HarmonyPatch(typeof(Piece))]
    internal class PiecePatch
    {
        /// <summary>
        ///     Called when just before pice is placed
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="__instance"></param>
        [HarmonyPrefix]
        [HarmonyPatch(nameof(Piece.SetCreator))]
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

        /// <summary>
        ///         Disable desctruction drops for player built pieces to prevent
        ///         things like player built dvergerprops_crate dropping dvergr 
        ///         extractors even if they're not a build requirement
        /// </summary>
        /// <param name="__instance"></param>
        /// <param name="__state"></param>
        [HarmonyPrefix]
        [HarmonyPatch(nameof(Piece.DropResources))]
        static void PieceDropResourcesPrefix(Piece __instance, out Piece.Requirement[] __state)
        {
            __state = null;
            if (!PluginConfig.IsModEnabled.Value) { return; }
#if DEBUG
            Log.LogInfo($"DropResourcesPrefix() for {__instance.gameObject.name}");
#endif
            // Only interact if it is a piece added by this mod or
            // the prefab has previously had it's resources altered by the mod
            string prefabName = NameHelper.GetPrefabName(__instance);
            if (IsChangedByMod(prefabName) || DefaultResources.ContainsKey(prefabName))
            {
                if (__instance.IsPlacedByPlayer())
                {
                    DisableDestructionDrops = true;
                }
                else
                {
                    // set drops to defaults and store the current drops
                    __state = __instance.m_resources;

                    if (DefaultResources.ContainsKey(prefabName))
                    {
                        Log.LogInfo("Resetting drop resources to defaults.");
                        __instance.m_resources = DefaultResources[prefabName];
                    }
                    else
                    {
                        __instance.m_resources = Array.Empty<Piece.Requirement>();
                    }
                }
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(nameof(Piece.DropResources))]
        static void PieceDropResourcesPostfix(Piece __instance, Piece.Requirement[] __state)
        {
            // restore original drops from before the prefix patch
            __instance.m_resources = __state;
        }
    }
}
