using HarmonyLib;
using UnityEngine;

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
        static void PieceDropResourcesPrefix(Piece __instance, out Piece.Requirement[] __state)
        {
            __state = null;
            if (!PluginConfig.IsModEnabled.Value) { return; }
#if DEBUG
            Log.LogInfo($"Custom drop resources for {__instance.gameObject.name}");
#endif
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
                else
                {
                    // set drops to defaults and store the current drops
                    __state = __instance.m_resources;
                    string prefab_name = RemoveFromEnd(__instance.gameObject.name, "(Clone)");
                    if (PrefabAdder.DefaultResources.ContainsKey(prefab_name))
                    {
                        Log.LogInfo("Resetting drop resources to defaults.");
                        __instance.m_resources = PrefabAdder.DefaultResources[prefab_name];
                    }
                    else
                    {
                        foreach (var resource in __instance.m_resources)
                        {
                            resource.m_resItem = null;
                        }
                    }
                }
            }
        }

        [HarmonyPostfix]
        [HarmonyPatch(typeof(Piece), nameof(Piece.DropResources))]
        static void PieceDropResourcesPostfix(Piece __instance, Piece.Requirement[] __state)
        {
            // restore original drops from before the prefix patch
            __instance.m_resources = __state;
        }

        public static string RemoveFromEnd(string s, string suffix)
        {
            if (s.EndsWith(suffix))
            {
                return s.Substring(0, s.Length - suffix.Length);
            }

            return s;
        }
    }
}
