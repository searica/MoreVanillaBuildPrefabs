using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

using MoreVanillaBuildPrefabs.Configs;
using MoreVanillaBuildPrefabs.Logging;
using MoreVanillaBuildPrefabs.Helpers;
using Jotunn.Entities;
using Jotunn.Managers;
using Jotunn.Configs;

namespace MoreVanillaBuildPrefabs.Patches
{
    //[HarmonyPatch(typeof(Game))]
    [HarmonyPatch(typeof(ObjectDB))]
    internal class GamePatch
    {

        // Hook here to add pieces after ServerSync recieves data
        [HarmonyPostfix]
        [HarmonyPriority(Priority.High)] // High priority for compatiability with WackyDB
        //[HarmonyPatch(nameof(Game._RequestRespawn))]
        [HarmonyPatch(nameof(ObjectDB.Awake))]
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
            List<CustomPiece> customPieces = new();
            foreach (var prefab in EligiblePrefabs)
            {
                // Check to fix rare incompatability with other mods.
                if (prefab == null)
                {
                    Log.LogWarning("Null prefab found in EligiblePrefabs");
                    continue;
                }
                var customPiece = CreatePrefabCustomPiece(prefab);
                if (customPiece != null)
                {
                    customPieces.Add(customPiece);
                }
            }

            // Create icons
            IconHelper.Instance.GeneratePrefabIcons(customPieces);

            // Add pieces
            foreach (var customPiece in customPieces)
            {
                PieceHelper.AddCustomPiece(customPiece);
            }

            Log.LogInfo($"Added {PieceHelper.AddedPrefabs.Count} custom pieces");
#if DEBUG
            watch.Stop();
            Log.LogInfo($"Creation Time: {watch.ElapsedMilliseconds} ms");
#endif
            PluginConfig.Save();
        }

        /// <summary>
        ///     Create and add custom pieces based on cfg file.
        /// </summary>
        /// <param name="prefab"></param>
        /// <param name="pieceTable"></param>
        internal static CustomPiece CreatePrefabCustomPiece(GameObject prefab)
        {
            if (!PrefabHelper.EnsureNoDuplicateZNetView(prefab))
            {
                // Just dont, as it will fuck over vanilla (non-mod) users
                if (PluginConfig.IsVerbose())
                {
                    Log.LogInfo($"Prevent duplicate ZNetView for: {prefab.name}");
                }
                return null;
            }

            // load config data and create piece config
            PluginConfig.PrefabConfig prefabConfig = PluginConfig.LoadPrefabConfig(prefab);

            if (!prefabConfig.Enabled && !PluginConfig.IsForceAllPrefabs()) // prefab denied by config
            {
                return null;
            }

            if (PluginConfig.IsVerbose())
            {
                Log.LogInfo("Initialize '" + prefab.name + "'");
                foreach (Component compo in prefab.GetComponents<Component>())
                {
                    Log.LogInfo("  - " + compo.GetType().Name);
                }
            }

            DefaultConfigs.SaveDefaultResources(prefab);
            PieceHelper.InitPieceComponent(prefab);
            PrefabPatcher.PatchPrefabIfNeeded(prefab);

            var piece = prefab.GetComponent<Piece>();
            CustomPiece customPiece = PieceHelper.ConfigureCustomPiece(
                piece,
                NameHelper.FormatPrefabName(prefab.name),
                NameHelper.GetPrefabDescription(prefab),
                prefabConfig.AllowedInDungeons,
                prefabConfig.Category,
                PieceTables.Hammer,
                prefabConfig.CraftingStation,
                prefabConfig.Requirements
            );

            // Fix missing hover text if needed.
            var hover = prefab.GetComponent<HoverText>() ?? prefab.AddComponent<HoverText>();
            if (string.IsNullOrEmpty(hover.m_text))
            {
                hover.enabled = true;
                hover.m_text = prefab.GetComponent<Piece>().m_name;
            }

            // Restrict CreatorShop pieces to Admins only
            if (HammerCategories.IsCreatorShopPiece(piece)
                && PluginConfig.AdminOnlyCreatorShop.Value
                && !SynchronizationManager.Instance.PlayerIsAdmin)
            {
                return null;
            }

            return customPiece;
        }
    }
}
