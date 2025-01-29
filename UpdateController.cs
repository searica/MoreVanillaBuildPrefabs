// Ignore Spelling: MVBP

using Jotunn.Configs;
using Jotunn.Managers;
using MVBP.Extensions;
using MVBP.SnapPoints;
using MVBP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Logging;

namespace MVBP;

internal static class UpdateController
{

    /// <summary>
    ///     Returns a bool indicating if the prefab is patched
    ///     by this mod and is set to be enabled.
    /// </summary>
    /// <param name="gameObject"></param>
    /// <returns></returns>
    internal static bool IsPrefabEnabled(GameObject gameObject)
    {
        string prefabName = GetPrefabName(gameObject);
        if (IsPatchedByMod(prefabName))
        {
            return MorePrefabs.IsPrefabConfigEnabled(prefabName) || MorePrefabs.IsForceAllPrefabs;
        }

        return false;
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
        foreach (GameObject prefab in ZNetScene.instance.m_prefabs)
        {
            if (!prefab.transform.parent && !PieceNameCache.Contains(prefab.name) &&
                PrefabFilter.GetEligiblePrefab(prefab, out GameObject result) && !EligiblePrefabs.ContainsKey(result.name))
            {
                EligiblePrefabs.Add(result.name, result);
            }
        }

        foreach (GameObject prefab in EligiblePrefabs.Values)
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
            UpdateDefaultResources(prefab);
        }

        Log.LogInfo($"Found {PrefabRefs.Count} prefabs");

        Log.LogInfo("Initializing default pieces");
        // Get a default icon to use if defaultResources doesn't have an icon.
        // Need this to prevent NRE's if other code references the defaultResources
        // before the coroutine that is rendering the icons finishes. (Such as PlanBuild)
        Sprite defaultIcon = PrefabManager.Cache.GetPrefab<Sprite>("mapicon_hildir1");

        foreach (var prefab in PrefabRefs.Values)
        {
            Piece defaultPiece = PieceHelper.InitPieceComponent(prefab);
            if (defaultPiece.m_icon == null)
            {
                defaultPiece.m_icon = defaultIcon;
            }
        }

        Log.LogInfo("Initializing default icons", Log.InfoLevel.Medium);

        IconManager.Instance.GeneratePrefabIcons(PrefabRefs.Values);
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
            GameObject prefab = PrefabManager.Instance.GetPrefab(name);
            if (prefab && prefab.TryGetComponent(out Piece piece))
            {
                // Only add pieces that are currently disabled
                if (!piece.m_enabled)
                {
                    SeasonalPieceRefs[name] = prefab;
                }
                else
                {
                    Log.LogInfo($"Seasonal Piece: {name} already enabled", Log.InfoLevel.Medium);
                    nullKeys.Add(name);
                }
            }
            else
            {
                Log.LogWarning($"Seasonal piece: {name} could not be found");
            }
        }

        foreach (string key in nullKeys)
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
            PieceTable hammerTable = PieceManager.Instance.GetPieceTable(PieceTables.Hammer);

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
        foreach (PieceDB pieceDB in PieceRefs.Values)
        {
            Log.LogInfo($"Configuring: {pieceDB.name}", Log.InfoLevel.High);
            Piece piece = PieceHelper.ConfigurePiece(pieceDB);
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
        Player player = Player.m_localPlayer;

        if (player && Player.m_localPlayer.GetRightItem()?.m_shared.m_name == "$item_hammer")
        {
            Log.LogWarning("Hammer updated through config change, unequipping hammer");
            Player.m_localPlayer.HideHandItems();
        }
    }

    /// <summary>
    ///     Create a set of defaultResources refs with each prefab's
    ///     defaultResources reset to the default state and the PieceDB
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

                // reset piece component to match the default defaultResources clone
                if (prefab.TryGetComponent(out Piece piece))
                {
                    newPieceRefs.Add(prefab.name, new PieceDB(MorePrefabs.GetPrefabConfig(prefab), piece));
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

        foreach (PieceDB pieceDB in PieceRefs.Values)
        {
            // Check if defaultResources is enabled by the mod
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

        PieceTable hammerTable = PieceManager.Instance.GetPieceTable(PieceTables.Hammer);
        foreach (List<GameObject> pieceGroup in pieceGroups)
        {
            foreach (GameObject prefab in pieceGroup)
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
        if (!HasInitializedPlugin)
        {
            return;
        }

        foreach (var name in SeasonalPieceRefs.Keys)
        {
            if (SeasonalPieceRefs.TryGetValue(name, out GameObject prefab) && prefab)
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
        ConfigManager.Save(); // so that cfg file has everything in it right away
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
    ///     has been updated based on whether the defaultResources or placement
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
        if (!MorePrefabs.UpdatePieceSettings &&
            !MorePrefabs.UpdatePlacementSettings &&
            !MorePrefabs.UpdateSeasonalSettings &&
            !MorePrefabs.UpdateModSettings)
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

        if (MorePrefabs.UpdatePieceSettings || MorePrefabs.UpdateSeasonalSettings)
        {
            ModCompat.UpdateExtraSnaps();
            ModCompat.UpdatePlanBuild();
        }

        MorePrefabs.UpdatePieceSettings = false;
        MorePrefabs.UpdatePlacementSettings = false;
        MorePrefabs.UpdateSeasonalSettings = false;
        MorePrefabs.UpdateModSettings = false;

        if (saveConfig) { ConfigManager.Save(); }
    }
}
