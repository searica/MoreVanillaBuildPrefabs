using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Jotunn.Managers;

namespace MoreVanillaBuildPrefabs
{
    public class PrefabHelper
    {
        public static HashSet<string> AddedPrefabs = new();

        public static Dictionary<string, Piece.Requirement[]> DefaultResources = new();

        private static HashSet<string> PieceNameCache = null;

        private static readonly HashSet<string> IgnoredPrefabs = new() {
            "Player",
            "Valkyrie",
            "HelmetOdin",
            "CapeOdin",
            "CastleKit_pot03",
            "Ravens",
            "TERRAIN_TEST",
            "PlaceMarker",
            "Circle_section",
            "guard_stone_test",
            "Haldor",
            "odin",
            "dvergrprops_wood_stake",
            "Hildir",
            //Placement is glitchy
            "demister_ball",
            "CargoCrate"
        };

        public static void FindAndAddPrefabs()
        {
            Log.LogInfo("FindAndAddPrefabs()");
#if DEBUG
            var watch = new System.Diagnostics.Stopwatch();
            watch.Start();
#endif

            if (PieceNameCache != null)
            {
                PieceNameCache = null;
                PieceNameCache = PieceHelper.GetExistingPieceNames();
            }

            var EligiblePrefabs = ZNetScene.instance.m_prefabs
            .Where(go => go.transform.parent == null && !ShouldIgnorePrefab(go))
            .OrderBy(go => go.name)
            .ToList();
            Log.LogInfo($"Found {EligiblePrefabs.Count()} prefabs");
#if DEBUG
            watch.Stop();
            Log.LogInfo($"Search Time: {watch.ElapsedMilliseconds} ms");
            watch.Reset();
            watch.Start();
#endif
            var pieceTable = PieceHelper.GetPieceTable("_HammerPieceTable");
            if (pieceTable == null)
            {
                Log.LogError("Could not find _HammerPieceTable");
            }

            foreach (var prefab in EligiblePrefabs)
            {
                // Don't know how this happens but it definitely does
                // sometimes when other mods are used in addition to this one.
                if (prefab == null)
                {
                    Log.LogWarning("Null prefab found in EligiblePrefabs");
                    continue;
                }
                CreatePrefabPiece(prefab, pieceTable);
            }

            Log.LogInfo($"Created {AddedPrefabs.Count} custom pieces");
#if DEBUG
            watch.Stop();
            Log.LogInfo($"Creation Time: {watch.ElapsedMilliseconds} ms");
#endif
            PluginConfig.Save();
        }

        public static void RemoveCustomPieces()
        {
            Log.LogInfo("RemoveCustomPieces()");
            int removedCounter = 0;
            PieceTable pieceTable = PieceHelper.GetPieceTable("_HammerPieceTable");

            foreach (var name in AddedPrefabs)
            {
                try // Remove piece from PieceTable
                {
                    var prefab = ZNetScene.instance.GetPrefab(name);
                    pieceTable.m_pieces.Remove(prefab);
                    removedCounter++;
                }
                catch (Exception e)
                {
#if DEBUG
                    Log.LogInfo($"{name}: {e}");
#endif
                }
            }
            AddedPrefabs.Clear();
            Log.LogInfo($"Removed {removedCounter} custom pieces");
        }

        /// <summary>
        ///     Checks prefab to see if it is eligble for making a custom piece.
        /// </summary>
        /// <param name="prefab"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private static bool ShouldIgnorePrefab(GameObject prefab)
        {
            // Ignore existing pieces
            if (PieceNameCache == null)
            {
                throw new Exception("PieceNameCache is null");
            }
            else if (PieceNameCache.Contains(prefab.name))
            {
                return true;
            }

            // Ignore specific prefab names
            if (IgnoredPrefabs.Contains(prefab.name))
            {
                return true;
            }

            // Ignore pieces added by Azumat's BowsBeforeHoes mod
            if (prefab.name.StartsWith("BBH_"))
            {
                return true;
            }

            // Customs filters
            if (prefab.GetComponent("Projectile") != null ||
                prefab.GetComponent("Humanoid") != null ||
                prefab.GetComponent("AnimalAI") != null ||
                prefab.GetComponent("Character") != null ||
                prefab.GetComponent("CreatureSpawner") != null ||
                prefab.GetComponent("SpawnArea") != null ||
                prefab.GetComponent("Fish") != null ||
                prefab.GetComponent("RandomFlyingBird") != null ||
                prefab.GetComponent("MusicLocation") != null ||
                prefab.GetComponent("Aoe") != null ||
                prefab.GetComponent("ItemDrop") != null ||
                prefab.GetComponent("DungeonGenerator") != null ||
                prefab.GetComponent("TerrainModifier") != null ||
                prefab.GetComponent("EventZone") != null ||
                prefab.GetComponent("LocationProxy") != null ||
                prefab.GetComponent("LootSpawner") != null ||
                prefab.GetComponent("Mister") != null ||
                prefab.GetComponent("Ragdoll") != null ||
                prefab.GetComponent("MineRock5") != null ||
                prefab.GetComponent("TombStone") != null ||
                prefab.GetComponent("LiquidVolume") != null ||
                prefab.GetComponent("Gibber") != null ||
                prefab.GetComponent("TimedDestruction") != null ||
                prefab.GetComponent("ShipConstructor") != null ||
                prefab.GetComponent("TriggerSpawner") != null ||
                prefab.GetComponent("TeleportAbility") != null ||
                prefab.GetComponent("TeleportWorld") != null ||

                prefab.name.StartsWith("_") ||
                prefab.name.StartsWith("OLD_") ||
                prefab.name.EndsWith("_OLD") ||
                prefab.name.StartsWith("vfx_") ||
                prefab.name.StartsWith("sfx_") ||
                prefab.name.StartsWith("fx_")
            )
            {
                return true;
            }
            return false;
        }

        /// <summary>
        ///     Create and add custom pieces based on cfg file.
        /// </summary>
        /// <param name="prefab"></param>
        /// <param name="pieceTable"></param>
        private static void CreatePrefabPiece(GameObject prefab, PieceTable pieceTable)
        {
            if (!EnsureNoDuplicateZNetView(prefab))
            {
                // Just dont, as it will fuck over vanilla (non-mod) users
                if (PluginConfig.IsVerbose())
                {
                    Log.LogInfo($"Prevent duplicate ZNetView for: {prefab.name}");
                }
                return;
            }

            // load config data and create piece config
            PluginConfig.PrefabConfig prefabConfig = PluginConfig.LoadPrefabConfig(prefab);

            if (!prefabConfig.Enabled && !PluginConfig.IsForceAllPrefabs()) // prefab denied by config
            {
                return;
            }

            if (PluginConfig.IsVerbose())
            {
                Log.LogInfo("Initialize '" + prefab.name + "'");
                foreach (Component compo in prefab.GetComponents<Component>())
                {
                    Log.LogInfo("  - " + compo.GetType().Name);
                }
            }

            SaveDefaultResources(prefab);
            PieceHelper.InitPieceComponent(prefab);
            PrefabPatcher.PatchPrefabIfNeeded(prefab);

            var piece = prefab.GetComponent<Piece>();
            piece = PieceHelper.ConfigurePiece(
                piece,
                PrefabNames.FormatPrefabName(prefab.name),
                PrefabNames.GetPrefabDescription(prefab),
                prefabConfig.AllowedInDungeons,
                prefabConfig.Category,
                prefabConfig.CraftingStation,
                prefabConfig.Requirements,
                PrefabIcons.CreatePrefabIcon(prefab)
            );

            // Restrict CreatorShop pieces to Admins only
            if (HammerCategories.IsCreatorShopPiece(piece)
                && PluginConfig.AdminOnlyCreatorShop.Value
                && !SynchronizationManager.Instance.PlayerIsAdmin)
            {
                return;
            }

            if (PieceHelper.AddPieceToPieceTable(piece, pieceTable))
            {
                AddedPrefabs.Add(prefab.name);
            }
        }

        /// <summary>
        ///     Prevents creation of duplicate ZNetViews
        /// </summary>
        /// <param name="prefab"></param>
        /// <returns></returns>
        private static bool EnsureNoDuplicateZNetView(GameObject prefab)
        {
            var views = prefab?.GetComponents<ZNetView>();

            if (views == null) return true;

            for (int i = 1; i < views.Length; ++i)
            {
                GameObject.DestroyImmediate(views[i]);
            }

            return views.Length <= 1;
        }

        private static void SaveDefaultResources(GameObject prefab)
        {
            var piece = prefab?.GetComponent<Piece>();
            if (piece?.m_resources != null)
            {
                // stop errors on second log in
                if (!DefaultResources.ContainsKey(prefab.name))
                {
#if DEBUG
                    Log.LogDebug($"Adding default drops for {prefab.name}");
#endif
                    DefaultResources.Add(prefab.name, piece.m_resources);
                }
            }
        }

        /// <summary>
        ///     Cache of existing game objects.
        /// </summary>
        internal static class Cache
        {
            private static readonly Dictionary<string, GameObject> dictionaryCache = new();


            public static GameObject GetPrefab(string name)
            {
                if (dictionaryCache.Count() == 0)
                {
                    InitCache();
                }
                if (dictionaryCache.TryGetValue(name, out GameObject prefab))
                {
                    return prefab;
                }
                return null;
            }

            /// <summary>
            ///     Initialize the internal cache of GameObject
            /// </summary>
            private static void InitCache()
            {
                foreach (var gameObject in Resources.FindObjectsOfTypeAll<GameObject>())
                {
                    string name = gameObject.name;
                    dictionaryCache[name] = FindBestAsset(gameObject, name);
                }
            }

            /// <summary>
            ///     Determines the best matching GameObject for a given name.
            ///     Only one asset can be associated with a name, this tries to find the 
            ///     best match if there is already a cached one present.
            /// </summary>
            /// <param name="map"></param>
            /// <param name="newGo"></param>
            /// <param name="name"></param>
            /// <returns></returns>
            private static GameObject FindBestAsset(GameObject newGo, string name)
            {
                if (!dictionaryCache.TryGetValue(name, out GameObject cachedGo))
                {
                    return newGo;
                }

                // if a ObjectDB parent exists in the main scene, prefer it over the prefab
                if (name == "_NetScene")
                {
                    if (!cachedGo.activeInHierarchy && newGo.activeInHierarchy)
                    {
                        return newGo;
                    }
                }

                bool cachedHasParent = GetParent(cachedGo);
                bool newHasParent = GetParent(newGo);

                if (!cachedHasParent && newHasParent)
                {
                    // as the cached object has no parent, it is more likely a real prefab and not a child GameObject
                    return cachedGo;
                }

                if (cachedHasParent && !newHasParent)
                {
                    // as the new object has no parent, it is more likely a real prefab and not a child GameObject
                    return newGo;
                }

                return newGo;
            }

            private static Transform GetParent(GameObject gameObject)
            {
                return gameObject.transform.parent;
            }
        }
    }
}
