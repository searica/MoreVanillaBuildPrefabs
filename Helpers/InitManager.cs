// Ignore Spelling: MVBP

using Jotunn.Configs;
using Jotunn.Managers;
using MVBP.Configs;
using MVBP.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MVBP.Helpers
{
    internal static class InitManager
    {
        internal static readonly Dictionary<string, GameObject> PrefabRefs = new();
        internal static readonly Dictionary<string, Piece> DefaultPieceClones = new();
        private static Dictionary<string, PieceDB> PieceRefs = new();
        private static Dictionary<string, string> PieceToPrefabMap = new();

        internal static readonly Dictionary<string, GameObject> SeasonalPieceRefs = new()
        {
            {"piece_maypole", null },
            {"piece_jackoturnip", null },
            {"piece_gift1", null },
            {"piece_gift2", null },
            {"piece_gift3", null },
            {"piece_mistletoe",null },
            {"piece_xmascrown",null },
            {"piece_xmasgarland",null },
            {"piece_xmastree",null },
        };

        // These lack a ZNetView component making them non-persistent and unsuitable for this mod.
        //private static readonly List<string> SpecificPrefabNames = new()
        //{
        //    "MountainKit_int_wall_4x2",
        //    "SunkenKit_int_arch",
        //    "SunkenKit_int_floor_2x2",
        //    "SunkenKit_int_wall_1x4",
        //    "SunkenKit_int_wall_2x4",
        //    "SunkenKit_int_wall_4x4",
        //    "SunkenKit_slope1x2",
        //    "SunkenKit_int_stair",
        //    "SunkenKit_stair_corner_left",
        //    "TraderChest_static",
        //    "TraderRune",
        //    "TraderTent",
        //    "ForceField_0",
        //};

        internal static bool HasInitializedPlugin => PrefabRefs.Count > 0;

        internal static bool TryGetDefaultPieceClone(GameObject gameObject, out Piece pieceClone)
        {
            var prefabName = GetPrefabName(gameObject);
            if (DefaultPieceClones.ContainsKey(prefabName))
            {
                pieceClone = DefaultPieceClones[prefabName];
                return true;
            }
            pieceClone = null;
            return false;
        }

        internal static bool TryGetPieceDB(string name, out PieceDB pieceDB)
        {
            if (!string.IsNullOrEmpty(name) && PieceRefs.TryGetValue(name, out pieceDB))
            {
                return true;
            }
            pieceDB = null;
            return false;
        }

        internal static bool TryGetPieceDB(Piece piece, out PieceDB pieceDB)
        {
            var prefabName = GetPrefabName(piece);
            if (string.IsNullOrEmpty(prefabName) && PieceRefs.TryGetValue(prefabName, out pieceDB))
            {
                return true;
            }
            pieceDB = null;
            return false;
        }

        /// <summary>
        ///     Returns a bool indicating if the prefab has been changed by mod.
        /// </summary>
        /// <returns></returns>
        internal static bool IsPatchedByMod(GameObject gameObject)
        {
            var prefabName = GetPrefabName(gameObject);
            return PrefabRefs.ContainsKey(prefabName);
        }

        /// <summary>
        ///     Returns a bool indicating if the prefab has been changed by mod.
        /// </summary>
        /// <returns></returns>
        internal static bool IsPatchedByMod(Component compo)
        {
            var prefabName = GetPrefabName(compo);
            return PrefabRefs.ContainsKey(prefabName);
        }

        /// <summary>
        ///     Returns a bool indicating if the prefab has been changed by mod.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        internal static bool IsPatchedByMod(string name)
        {
            return PrefabRefs.ContainsKey(name);
        }

        /// <summary>
        ///     Returns a bool indicating if the prefab is patched
        ///     by this mod and is set to be enabled.
        /// </summary>
        /// <param name="gameObject"></param>
        /// <returns></returns>
        internal static bool IsPrefabEnabled(GameObject gameObject)
        {
            var prefabName = GetPrefabName(gameObject);
            if (IsPatchedByMod(prefabName))
            {
                return MorePrefabs.IsPrefabConfigEnabled(prefabName) || MorePrefabs.IsForceAllPrefabs;
            }
            return false;
        }

        /// <summary>
        ///     Get the root prefab name.
        /// </summary>
        /// <param name="compo"></param>
        /// <returns></returns>
        internal static string GetPrefabName(Component compo)
        {
            return GetPrefabName(compo?.gameObject);
        }

        /// <summary>
        ///     Get the root prefab name.
        /// </summary>
        /// <param name="gameObject"></param>
        /// <returns></returns>
        internal static string GetPrefabName(GameObject gameObject)
        {
            if (gameObject == null) { return string.Empty; }

            var prefabName = gameObject.name.RemoveSuffix("(Clone)");
            if (PrefabRefs.ContainsKey(prefabName)) { return prefabName; }

            if (gameObject.TryGetComponent(out Piece piece) && PieceToPrefabMap.ContainsKey(piece.m_name))
            {
                return PieceToPrefabMap[piece.m_name];
            }

            return prefabName;
        }

        private static void InitPrefabRefs()
        {
            if (PrefabRefs.Count > 0)
            {
                return;
            }

            Log.LogInfo("Initializing prefabs");

            InitSeasonalPieceRefs();

            // Find eligible prefabs for adding
            var PieceNameCache = PieceHelper.GetExistingPieceNames();
            var EligiblePrefabs = new Dictionary<string, GameObject>();
            foreach (var prefab in ZNetScene.instance.m_prefabs)
            {
                if (prefab.transform.parent == null && !PieceNameCache.Contains(prefab.name))
                {
                    if (PrefabFilter.GetEligiblePrefab(prefab, out GameObject result))
                    {
                        if (!EligiblePrefabs.ContainsKey(result.name))
                        {
                            EligiblePrefabs.Add(result.name, result);
                        }
                    }
                }
            }

            foreach (var prefab in EligiblePrefabs.Values)
            {
                if (!PieceHelper.EnsureNoDuplicateZNetView(prefab))
                {
                    continue;
                }

                if (Log.IsVerbosityHigh) { Log.LogGameObject(prefab); }

                // Always patching means it only runs once and
                // prevents trailership being unusable if disabled.
                try
                {
                    PrefabPatcher.PatchPrefabIfNeeded(prefab);
                }
                catch (Exception ex)
                {
                    Log.LogWarning($"Failed to patch prefab {prefab.name}: {ex}");
                }

                PrefabRefs.Add(prefab.name, prefab);
            }

            Log.LogInfo($"Found {PrefabRefs.Count} prefabs");

            Log.LogInfo("Initializing default pieces");
            // Get a default icon to use if pieceClone doesn't have an icon.
            // Need this to prevent NRE's if other code references the pieceClone
            // before the coroutine that is rendering the icons finishes. (Such as PlanBuild)
            var defaultIcon = PrefabManager.Cache.GetPrefab<Sprite>("mapicon_hildir1");
            //var defaultIcon = Resources.FindObjectsOfTypeAll<Sprite>().Where(spr => spr.name == "mapicon_hildir1").First();

            foreach (var prefab in PrefabRefs.Values)
            {
                var defaultPiece = PieceHelper.InitPieceComponent(prefab);
                if (defaultPiece.m_icon == null)
                {
                    defaultPiece.m_icon = defaultIcon;
                }
            }

            Log.LogInfo("Initializing default icons", LogLevel.Medium);

            IconHelper.Instance.GeneratePrefabIcons(PrefabRefs.Values);
        }

        /// <summary>
        ///     Get refs to seasonal pieces that are disabled.
        /// </summary>
        private static void InitSeasonalPieceRefs()
        {
            var pieceNames = SeasonalPieceRefs.Keys.ToList();
            var nullKeys = new List<string>();
            foreach (var name in pieceNames)
            {
                var prefab = PrefabManager.Instance.GetPrefab(name);
                if (prefab != null && prefab.TryGetComponent(out Piece piece))
                {
                    // Only add pieces that are currently disabled
                    if (!piece.m_enabled)
                    {
                        SeasonalPieceRefs[name] = prefab;
                    }
                    else
                    {
                        nullKeys.Add(name);
                    }
                }
            }

            foreach (var key in nullKeys)
            {
                SeasonalPieceRefs.Remove(key);
            }
        }

        /// <summary>
        ///     Initializes references to pieces and their configuration settings then applies 
        ///     the configuration settings from the PieceDB for each piece in PieceRefs.
        /// </summary>
        private static void InitPieces()
        {
            Log.LogInfo("Initializing piece refs");

            if (PieceRefs.Count > 0)
            {
                PieceTable hammerTable = PieceHelper.GetPieceTable(PieceTables.Hammer);

                foreach (PieceDB pdb in PieceRefs.Values)
                {
                    // remove pieces from hammer build table and sheath hammer if piece table is open
                    PieceHelper.RemovePieceFromPieceTable(pdb.Prefab, hammerTable);
                    ForceUnequipHammer();
                }
                PieceRefs.Clear();
            }

            PieceRefs = GeneratePieceRefs();

            Log.LogInfo("Initializing pieces");
            foreach (var pieceDB in PieceRefs.Values)
            {
                var piece = PieceHelper.ConfigurePiece(pieceDB);
                SfxHelper.FixPlacementSfx(piece);

                // Need to map names of pieces to source prefab for MineRock5 prefabs
                PieceToPrefabMap[piece.m_name] = pieceDB.name;
            }
        }

        /// <summary>
        ///     Forces hammer to be unequipped if it is currently equipped.
        /// </summary>
        private static void ForceUnequipHammer()
        {
            var player = Player.m_localPlayer;

            if (player && Player.m_localPlayer.GetRightItem()?.m_shared.m_name == "$item_hammer")
            {
                Log.LogWarning("Hammer updated through config change, unequipping hammer");
                Player.m_localPlayer.HideHandItems();
            }
        }

        /// <summary>
        ///     Create a set of pieceClone refs with each prefab's
        ///     pieceClone reset to the default state and the PieceDB
        ///     containing the configuration settings to apply.
        /// </summary>
        /// <returns></returns>
        private static Dictionary<string, PieceDB> GeneratePieceRefs()
        {
            Dictionary<string, PieceDB> newPieceRefs = new();
            foreach (var name in PrefabRefs.Keys)
            {
                if (PrefabRefs.TryGetValue(name, out GameObject prefab))
                {
                    if (!prefab)
                    {
                        Log.LogWarning($"Prefab: {name} has been destroyed");
                        continue;
                    }

                    // reset piece component to match the default pieceClone clone
                    if (prefab.TryGetComponent(out Piece piece))
                    {
                        newPieceRefs.Add(prefab.name, new PieceDB(MorePrefabs.GetPrefabDB(prefab), piece));
                    }
                    else
                    {
                        Log.LogWarning($"Prefab: {name} is missing piece component");
                    }
                }
                else
                {
                    Log.LogWarning($"Could not find Prefab: {name}");
                }
            }

            return newPieceRefs;
        }

        /// <summary>
        ///     Add all pieces that are enabled in the cfg file to the hammer build
        ///     table according to CreatorShop related cfg settings. Allow sets
        ///     all pieces added to the hammer permit deconstruction by players.
        /// </summary>
        private static void InitHammer()
        {
            Log.LogInfo("Initializing hammer");

            var pieceGroups = new SortedPieceGroups();

            foreach (var pieceDB in PieceRefs.Values)
            {
                // Check if pieceClone is enabled by the mod
                if (!pieceDB.enabled && !MorePrefabs.IsForceAllPrefabs)
                {
                    continue;
                }

                // Prevent adding creative mode pieces if not in CreativeMode
                if (!MorePrefabs.IsCreativeMode && PieceCategoryHelper.IsCreativeModePiece(pieceDB.piece))
                {
                    continue;
                }

                // Only add vanilla crops if enabled
                if (!MorePrefabs.IsEnableHammerCrops && pieceDB.pieceGroup == PieceGroup.VanillaCrop)
                {
                    continue;
                }

                // Restrict placement of CreatorShop pieces to Admins only
                if (MorePrefabs.IsCreatorShopAdminOnly &&
                    PieceCategoryHelper.IsCreatorShopPiece(pieceDB.piece) &&
                    !SynchronizationManager.Instance.PlayerIsAdmin)
                {
                    continue;
                }

                pieceGroups.Add(pieceDB);
            }

            PieceTable hammerTable = PieceHelper.GetPieceTable(PieceTables.Hammer);
            foreach (List<GameObject> pieceGroup in pieceGroups)
            {
                foreach (var prefab in pieceGroup)
                {
                    PieceHelper.AddPieceToPieceTable(prefab, hammerTable);
                }
            }
        }

        /// <summary>
        ///     Enables/disables seasonal pieces based on config settings.
        ///     Has no effect on seasonal pieces that are already enabled in Vanilla.
        /// </summary>
        private static void InitSeasonalPieces()
        {
            if (!HasInitializedPlugin) return;

            foreach (var name in SeasonalPieceRefs.Keys)
            {
                if (SeasonalPieceRefs.TryGetValue(name, out GameObject prefab) && prefab != null)
                {
                    if (prefab.TryGetComponent(out Piece piece))
                    {
                        piece.m_enabled = MorePrefabs.IsEnableSeasonalPieces;
                    }
                }
                else
                {
                    Log.LogWarning($"Seasonal piece: {name} could not be found");
                }
            }
        }

        /// <summary>
        ///     Initialize plugin for the first time.
        /// </summary>
        internal static void InitPlugin()
        {
            if (HasInitializedPlugin) { return; }

            PieceCategoryHelper.AddCreatorShopPieceCategory();
            SfxHelper.Init();
            InitPrefabRefs();
            InitSeasonalPieces();
            InitPieces();
            InitHammer();
        }

        /// <summary>
        ///     Reinitialize pieces and the hammer build table.
        /// </summary>
        internal static void UpdatePieces()
        {
            if (!HasInitializedPlugin) { return; }

            InitPieces();
            InitHammer();
        }

        /// <summary>
        ///     Method to re-initialize the plugin when the configuration
        ///     has been updated based on whether the pieceClone or placement
        ///     settings have been changed for any of the config entries.
        /// </summary>
        /// <param name="msg"></param>
        internal static void UpdatePlugin(string msg, bool saveConfig = true)
        {
            if (!HasInitializedPlugin)
            {
                return;
            }

            // Don't update unless settings have actually changed
            if (!MorePrefabs.UpdatePieceSettings && !MorePrefabs.UpdatePlacementSettings)
            {
                return;
            }

            var watch = new System.Diagnostics.Stopwatch();
            if (Log.IsVerbosityMedium) { watch.Start(); }
            Log.LogInfo(msg);

            if (MorePrefabs.UpdatePieceSettings)
            {
                UpdatePieces();
            }

            if (MorePrefabs.UpdateSeasonalSettings)
            {
                InitSeasonalPieces();
            }

            if (MorePrefabs.UpdatePlacementSettings)
            {
                ForceUnequipHammer(); // reset placement ghost set up to apply patch
            }

            if (Log.IsVerbosityMedium)
            {
                watch.Stop();
                Log.LogInfo($"Time to re-initialize: {watch.ElapsedMilliseconds} ms");
            }
            else
            {
                Log.LogInfo("Re-initializing complete");
            }

            if (MorePrefabs.UpdatePieceSettings)
            {
                ModCompat.UpdateExtraSnaps();
                ModCompat.UpdatePlanBuild();
            }

            MorePrefabs.UpdatePieceSettings = false;
            MorePrefabs.UpdatePlacementSettings = false;
            MorePrefabs.UpdateSeasonalSettings = false;
            if (saveConfig) { ConfigManager.Save(); }
        }
    }
}