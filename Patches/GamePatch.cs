using HarmonyLib;
using MoreVanillaBuildPrefabs.Configs;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MoreVanillaBuildPrefabs.Patches
{
    [HarmonyPatch(typeof(Game))]
    internal class GamePatch
    {

        // Hook here to add pieces after ServerSync recieves data
        [HarmonyPostfix]
        [HarmonyPriority(Priority.High)] // High priority for compatiability with WackyDB
        [HarmonyPatch(nameof(Game._RequestRespawn))]
        static void Game_RequestRespawnPostFix()
        {
#if DEBUG
            Log.LogInfo("Game._RequestRespawn.Postfix()");
#endif
            if (PluginConfig.IsModEnabled.Value)
            {
                if (SceneManager.GetActiveScene() == null)
                {
                    return;
                }

                // If loading into game world and prefabs have not been added
                if (SceneManager.GetActiveScene().name == "main" && PieceHelper.AddedPrefabs.Count == 0)
                {
                    HammerCategories.AddCustomCategories();
                    FindAndAddPrefabs();
                }
            }
        }

        private static void FindAndAddPrefabs()
        {
            Log.LogInfo("FindAndAddPrefabs()");
#if DEBUG
            var watch = new System.Diagnostics.Stopwatch();
            watch.Start();
#endif
            var PieceNameCache = PieceHelper.GetExistingPieceNames();

            var EligiblePrefabs = ZNetScene.instance.m_prefabs
            .Where(
                go => go.transform.parent == null
                && !PieceNameCache.Contains(go.name)
                && !PrefabHelper.ShouldIgnorePrefab(go)
            )
            .OrderBy(go => go.name)
            .ToList();
            Log.LogInfo($"Found {EligiblePrefabs.Count()} prefabs");
#if DEBUG
            watch.Stop();
            Log.LogInfo($"Search Time: {watch.ElapsedMilliseconds} ms");
            watch.Reset();
            watch.Start();
#endif
            // Create and configure pieces
            List<GameObject> customPrefabs = new();
            foreach (var prefab in EligiblePrefabs)
            {
                // Check to fix rare incompatability with other mods.
                if (prefab == null)
                {
                    Log.LogWarning("Null prefab found in EligiblePrefabs");
                    continue;
                }
                var prefabPiece = PrefabHelper.CreatePrefabPiece(prefab);
                if (prefabPiece != null)
                {
                    customPrefabs.Add(prefabPiece);
                }
            }

            // Create icons
            PrefabIcons.Instance.StartGeneratePrefabIcons(customPrefabs);

            // Add pieces to hammer piece table
            var pieceTable = PieceHelper.GetPieceTable("_HammerPieceTable");
            if (pieceTable == null) { Log.LogError("Could not find _HammerPieceTable"); }

            foreach (var prefab in customPrefabs)
            {
                PieceHelper.AddPieceToPieceTable(prefab.GetComponent<Piece>(), pieceTable);
            }

            Log.LogInfo($"Added {PieceHelper.AddedPrefabs.Count} custom pieces");
#if DEBUG
            watch.Stop();
            Log.LogInfo($"Creation Time: {watch.ElapsedMilliseconds} ms");
#endif
            PluginConfig.Save();
        }
    }
}
