// Ignore Spelling: MVBP

using UnityEngine.SceneManagement;
using System;
using Logging;
using HarmonyLib;
using MVBP.PrefabManagement;
using MVBP.PieceManagement;
using Jotunn.Managers;
using Jotunn.Configs;
using System.Collections.Generic;

namespace MVBP;

[HarmonyPatch]
internal static class UpdateMananger
{
    private static bool HasInit = false;
    private static bool PieceSettingsChanged { get; set; } = false;
    private static bool PlacementSettingsChanged { get; set; } = false;
    private static bool ModSettingsChanged { get; set; } = false;
    private static bool SeasonalSettingsChanged { get; set; } = false;

    private static bool IgnoreWearNTearPatchesChanged { get; set; } = false;

    /// <summary>
    ///     Event hook to set whether a config entry
    ///     for a piece setting has been changed.
    /// </summary>
    public static void PieceSettingChanged(object obj, EventArgs args)
    {
        if (!PieceSettingsChanged)
        {
            PieceSettingsChanged = true;
        }
    }

    /// <summary>
    ///     Event hook to set whether a config entry
    ///     for placement patches has been changed.
    /// </summary>
    public static void PlacementSettingChanged(object obj, EventArgs args)
    {
        if (!PlacementSettingsChanged)
        {
            PlacementSettingsChanged = true;
        }
    }

    /// <summary>
    ///     Event hook to set whether a config entry
    ///     for general mod settings has been changed.
    /// </summary>
    public static void ModSettingChanged(object obj, EventArgs args)
    {
        if (!ModSettingsChanged)
        {
            ModSettingsChanged = true;
        }
    }

    public static void SeasonalSettingChanged(object obj, EventArgs args)
    {
        if (!SeasonalSettingsChanged)
        {
            SeasonalSettingsChanged = true;
        }
    }

    public static void IgnoreWearNTearPatchesListChanged(object obj, EventArgs args)
    {
        if (!IgnoreWearNTearPatchesChanged)
        {
            IgnoreWearNTearPatchesChanged = true;
        }
    }

    /// <summary>
    ///     Hook to initialize the mod. This is after both PlantEverything
    ///     and PotteryBarn add pieces but before PlanBuild scans for them.
    /// </summary>
    /// <param name="__instance"></param>
    [HarmonyPrefix]
    [HarmonyPriority(Priority.High)]
    [HarmonyPatch(typeof(ZoneSystem), nameof(ZoneSystem.Start))]
    private static void InitializeMVPrefabManager()
    {
        // If loading into game world and prefabs have not been added
        if (SceneManager.GetActiveScene().name != "main")
        {
            return;
        }

        Log.LogInfo("Performing mod initialization");

        System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
        if (Log.IsVerbosityMedium) { watch.Start(); }

        Initialize();

        if (Log.IsVerbosityMedium)
        {
            watch.Stop();
            Log.LogInfo($"Time to initialize: {watch.ElapsedMilliseconds} ms");
        }
    }

    /// <summary>
    ///     Patch to check if world modifiers for resources are active
    ///     and re-initialize the mod if they are so pickables have the
    ///     correct build requirement costs.
    /// </summary>
    [HarmonyPostfix]
    [HarmonyPatch(typeof(ZNet), nameof(ZNet.Start))]
    private static void ApplyResourceRateToPieceResources()
    {
        Log.LogInfo("Checking world modifiers", Log.InfoLevel.Medium);

        // If loading into game world and prefabs have not been added
        if (SceneManager.GetActiveScene().name == "main")
        {
            // Resource rate modifiers are not active if == 1.0f
            if (Game.m_resourceRate == 1.0f) 
            {
                return; 
            }

            Log.LogInfo("World modifiers for resource rate are active, re-initializing");

            System.Diagnostics.Stopwatch watch = new();
            if (Log.IsVerbosityMedium) 
            { 
                watch.Start();
            }

            UpdatePieces();

            if (Log.IsVerbosityMedium)
            {
                watch.Stop();
                Log.LogInfo($"Time to re-initialize: {watch.ElapsedMilliseconds} ms");
            }
        }
    }

    /// <summary>
    ///     Initialize managers for the mod and update pieces for
    ///     the hammer piece table based on the config.
    /// </summary>
    private static void Initialize()
    {
        if (HasInit)
        {
            return;
        }
        HasInit = true;

        Log.LogInfo("Initializing managers.");
        PieceCategoryManager.AddCustomPieceCategories();
        SeasonalPieceMananger.Initialize();
        SfxManager.Initialize();
        MVPrefabManager.Initialize();
        UpdatePieces();
        SeasonalPieceMananger.UpdateSeasonalPieces();
        UpdateIgnoreWearNTearPatchesSet();
        MorePrefabs.Instance.Config.Save(); // save after binding prefab configs
    }

    /// <summary>
    ///     Method to update pieces added by MVBP when the configuration has been updated.
    /// </summary>
    /// <param name="msg">Text to log when triggering the update.</param>
    /// <param name="saveConfig">Whether to save the config after updating.</param>
    internal static void UpdatePlugin(string msg, bool saveConfig = true)
    {
        if (!HasInit)
        {
            return;
        }

        // Don't update unless settings have actually changed
        if (!PieceSettingsChanged && !PlacementSettingsChanged && !SeasonalSettingsChanged && 
            !IgnoreWearNTearPatchesChanged && !ModSettingsChanged )
        {
            return;
        }

        Log.LogInfo(msg);
        System.Diagnostics.Stopwatch watch = new();
        watch.Start();
        
        if (PieceSettingsChanged)
        {
            UpdatePieces();
        }

        if (SeasonalSettingsChanged)
        {
            SeasonalPieceMananger.UpdateSeasonalPieces();
        }

        if (PlacementSettingsChanged)
        {
            ForceUnequipBuildTools(); // reset placement ghost set up to apply patch
        }

        watch.Stop();
        Log.LogInfo($"Time to re-initialize: {watch.ElapsedMilliseconds} ms");
   

        if (PieceSettingsChanged || SeasonalSettingsChanged)
        {
            ModCompat.UpdateExtraSnaps();
            ModCompat.UpdatePlanBuild();
        }

        if (IgnoreWearNTearPatchesChanged)
        {
            UpdateIgnoreWearNTearPatchesSet();
        }

        ModSettingsChanged = false;
        PieceSettingsChanged = false;
        PlacementSettingsChanged = false;
        SeasonalSettingsChanged = false;
        IgnoreWearNTearPatchesChanged = false;  
 
        if (saveConfig) { MorePrefabs.Instance.Config.Save(); }
    }

    /// <summary>
    ///     Reinitialize pieces and the hammer build table.
    /// </summary>
    private static void UpdatePieces()
    {
        if (!HasInit) { return; }
        MVPrefabManager.ApplyPrefabConfigSettings();
        UpdatePieceTables();
    }

    /// <summary>
    ///     Remove all added pieces and insert the new pieces in sorted order.
    /// </summary>
    private static void UpdatePieceTables()
    {
        ForceUnequipBuildTools();
        List<PrefabConfig> boundPrefabConfigs = PrefabConfigManager.GetPrefabConfigs(checkIfBound: true);
     
        //PieceTable pieceTable = PieceManager.Instance.GetPieceTable(PieceTables.Hammer);
        //Log.LogInfo($"Updating {pieceTable.name}", Log.InfoLevel.Medium);
        //PieceTableManager.UpdatePieceTable(pieceTable, boundPrefabConfigs, clearPieces: true);

        foreach (PieceTable pieceTable in PieceManager.Instance.GetPieceTables())
        {
            Log.LogInfo($"Updating {pieceTable.name}", Log.InfoLevel.Medium);
            PieceTableManager.UpdatePieceTable(pieceTable, boundPrefabConfigs, clearPieces: true);
        }


    }

    /// <summary>
    ///     Forces hammer and other build tools to be unequipped if they are currently equipped.
    /// </summary>
    private static void ForceUnequipBuildTools()
    { 
        Player player = Player.m_localPlayer;

        //Player.m_localPlayer.GetRightItem()?.m_shared.m_name == "$item_hammer"
        if (player && Player.m_localPlayer.GetRightItem()?.m_shared.m_buildPieces)
        {
            Log.LogWarning("Hammer updated through config change, unequipping tool with piece table");
            Player.m_localPlayer.HideHandItems();
        }
    }

    private static void UpdateIgnoreWearNTearPatchesSet()
    {
        try
        {
            List<string> newItems = [];
            foreach (string item in MorePrefabs.IgnoreWearNTearPatches.Value.Split(',', ';'))
            {
                newItems.Add(item.Trim());
            }
            if (newItems.Count > 0)
            {
                MorePrefabs.IgnoreWearNTearPatchesSet.Clear();
                MorePrefabs.IgnoreWearNTearPatchesSet.UnionWith(newItems);
            }
        }
        catch (Exception ex)
        {
            Log.LogWarning($"Error parsing IgnoreWearNTearPatches: {ex}");
        }
    }

}
