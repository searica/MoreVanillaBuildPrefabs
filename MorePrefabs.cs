// Ignore Spelling: Plugin MVBP
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using Jotunn.Managers;
using Jotunn.Utils;
using Jotunn.Extensions;
using Logging;
using Configs;
using MVBP.SnapPoints;
using MVBP.Models;

namespace MVBP;

[BepInPlugin(PluginGUID, PluginName, PluginVersion)]
[BepInDependency(Jotunn.Main.ModGuid, Jotunn.Main.Version)]
[BepInDependency(ModCompat.ExtraSnapsGUID, BepInDependency.DependencyFlags.SoftDependency)]
[BepInDependency(ModCompat.PlanBuildGUID, BepInDependency.DependencyFlags.SoftDependency)]
[NetworkCompatibility(CompatibilityLevel.VersionCheckOnly, VersionStrictness.Patch)]
[SynchronizationMode(AdminOnlyStrictness.IfOnServer)]
public class MorePrefabs : BaseUnityPlugin
{
    public const string PluginName = "MoreVanillaBuildPrefabs";
    internal const string Author = "Searica";
    public const string PluginGUID = $"{Author}.Valheim.{PluginName}";
    public const string PluginVersion = "1.3.5";

    public static MorePrefabs Instance { get; private set; }
    private ConfigFileWatcher configFileWatcher;

    #region Global Settings
    private const string MainSection = "Global";
    private static ConfigEntry<bool> CreativeMode { get; set; }
    private static ConfigEntry<bool> ForceAllPrefabs { get; set; }
    internal static bool IsCreativeMode => CreativeMode.Value;
    internal static bool IsForceAllPrefabs => ForceAllPrefabs.Value;
    #endregion Global Settings

    #region Admin Settings
    private const string AdminSection = "Admin";
    private static ConfigEntry<bool> CreatorShopAdminOnly { get; set; }
    private static ConfigEntry<bool> AdminDeconstructOtherPlayers { get; set; }
    internal static bool IsCreatorShopAdminOnly => CreatorShopAdminOnly.Value;
    internal static bool IsAdminDeconstructOtherPlayers => AdminDeconstructOtherPlayers.Value;
    #endregion Admin Settings

    #region Customization Settings
    private const string CustomizationSection = "Customization";
    private static ConfigEntry<bool> EnableHammerCrops { get; set; }
    private static ConfigEntry<bool> EnableComfortPatches { get; set; }
    private static ConfigEntry<bool> EnableSeasonalPieces { get; set; }
    private static ConfigEntry<bool> EnablePlayerBasePatches { get; set; }
    private static ConfigEntry<bool> EnablePortalPatch { get; set; }
    internal static bool IsEnableHammerCrops => EnableHammerCrops.Value;
    internal static bool IsEnableComfortPatches => EnableComfortPatches.Value;
    internal static bool IsEnableSeasonalPieces => EnableSeasonalPieces.Value;
    internal static bool IsEnablePlayerBasePatches => EnablePlayerBasePatches.Value;
    internal static bool IsEnablePortalPatch => EnablePortalPatch.Value;
    #endregion Customization Settings

    #region Texture Patches
    private const string TextureSection = "Textures";
    private static ConfigEntry<bool> PortalTexture;
    private static ConfigEntry<bool> DvergrWoodTexture;
    internal static bool PatchPortalTexture => PortalTexture.Value;
    internal static bool PatchDvergrWoodTexture => DvergrWoodTexture.Value;
    #endregion Texture Patches

    #region Unsafe Patches
    private const string UnsafeSection = "Unsafe Patches";
    private static ConfigEntry<bool> EnableBedPatches { get; set; }
    private static ConfigEntry<bool> EnableFermenterPatches { get; set; }
    internal static bool IsEnableBedPatches => EnableBedPatches.Value;
    internal static bool IsEnableFermenterPatches => EnableFermenterPatches.Value;
    #endregion Unsafe Patches

    #region Prefab Settings
    private static readonly Dictionary<string, PrefabDBConfigEntries> PrefabDBConfigsMap = new();

    internal static bool IsPrefabConfigEnabled(string prefabName)
    {
        if (PrefabDBConfigsMap.ContainsKey(prefabName) && PrefabDBConfigsMap[prefabName].enabled != null)
        {
            return PrefabDBConfigsMap[prefabName].enabled.Value;
        }
        return false;
    }
    #endregion Prefab Settings

    #region Update Flags & Checks
    internal static bool UpdatePieceSettings { get; set; } = false;
    internal static bool UpdatePlacementSettings { get; set; } = false;
    internal static bool UpdateModSettings { get; set; } = false;
    internal static bool UpdateSeasonalSettings { get; set; } = false;

    /// <summary>
    ///     Event hook to set whether a config entry
    ///     for a piece setting has been changed.
    /// </summary>
    internal static void PieceSettingChanged(object obj, EventArgs args)
    {
        if (!UpdatePieceSettings)
        {
            UpdatePieceSettings = true;
        }
    }

    /// <summary>
    ///     Event hook to set whether a config entry
    ///     for placement patches has been changed.
    /// </summary>
    internal static void PlacementSettingChanged(object obj, EventArgs args)
    {
        if (!UpdatePlacementSettings)
        {
            UpdatePlacementSettings = true;
        }
    }

    /// <summary>
    ///     Event hook to set whether a config entry
    ///     for general mod settings has been changed.
    /// </summary>
    internal static void ModSettingChanged(object obj, EventArgs args)
    {
        if (!UpdateModSettings)
        {
            UpdateModSettings = true;
        }
    }

    internal static void SeasonalSettingChanged(object obj, EventArgs args)
    {
        if (!UpdateSeasonalSettings)
        {
            UpdateSeasonalSettings = true;
        }
    }

    /// <summary>
    ///     Get a bool indicating if the prefab is configured to require a placement patch.
    /// </summary>
    /// <param name="PrefabName"></param>
    /// <returns></returns>
    internal static bool NeedsCollisionPatchForGhost(string prefabName)
    {
        if (PrefabDBConfigsMap.TryGetValue(prefabName, out PrefabDBConfigEntries prefabDBConfig))
        {
            return prefabDBConfig.ApplyPlacementPatch;
        }

        return false;
    }
    #endregion Update Flags & Checks

    public void Awake()
    {
        Instance = this;
        Log.Init(Logger);

        Config.SaveOnConfigSet = false;
        SetUpConfigEntries();
        Config.Save();

        Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), harmonyInstanceId: PluginGUID);

        Game.isModded = true;

        // Re-initialization after reloading config and don't save since file was just reloaded
        configFileWatcher = new(Config);
        configFileWatcher.OnConfigFileReloaded += () =>
        {
            UpdateController.UpdatePlugin("Configuration file changed, re-initializing", saveConfig: false);
        };

        // Re-initialize after changing config data in-game and trigger a save to disk.
        SynchronizationManager.OnConfigurationWindowClosed += () =>
        {
            UpdateController.UpdatePlugin("Configuration changed in-game, re-initializing");
        };

        // Re-initialize after getting updated config data and trigger a save to disk.
        SynchronizationManager.OnConfigurationSynchronized += (obj, attr) =>
        {
            UpdateController.UpdatePlugin("Configuration synced, re-initializing");
        };
    }

    public void OnDestroy()
    {
        Config.Save();
    }

    private void SetUpConfigEntries()
    {
        CreativeMode = Config.BindConfigInOrder(
            MainSection,
            "CreativeMode",
            false,
            "Set to true/enabled to enable pieces from the CreatorShop or Nature piece categories. " +
            "By default, pieces set to those categories are not standard build pieces."
        );

        ForceAllPrefabs = Config.BindConfigInOrder(
            MainSection,
            "ForceAllPrefabs",
            false,
            "If true/enabled, adds all prefabs to the hammer for building. Unless CreativeMode is " +
            "also enabled it will not add pieces set to the CreatorShop or Nature category though."
        );

        Log.Verbosity = Config.BindConfigInOrder(
            MainSection,
            "Verbosity",
            Log.InfoLevel.Low,
            "Low will log basic information about the mod. Medium will log information that " +
            "is useful for troubleshooting. High will log a lot of information, do not set " +
            "it to this without good reason as it will slow down your game.",
            synced: false
        );

        CreatorShopAdminOnly = Config.BindConfigInOrder(
            AdminSection,
            "CreatorShopAdminOnly",
            false,
            "Set to true/enabled to restrict placement and deconstruction of CreatorShop pieces " +
            "to players with Admin status."
        );

        AdminDeconstructOtherPlayers = Config.BindConfigInOrder(
            AdminSection,
            "AdminDeconstructOtherPlayers",
            true,
            "Set to true/enabled to allow admin players to deconstruct any pieces built by other players," +
            " even if doing so would normally be prevented (such as for CreatorShop or Nature pieces)." +
            " Intended to prevent griefing via placement of indestructible objects."
        );

        // Customization settings
        EnableHammerCrops = Config.BindConfigInOrder(
            CustomizationSection,
            "HammerCrops",
            false,
            "Set to true/enabled to enable placing vanilla crops with the hammer." +
            " Unless this setting is true Vanilla crops will not be available for placing with the hammer."
        );

        EnableComfortPatches = Config.BindConfigInOrder(
            CustomizationSection,
            "ComfortPatches (Requires Restart)",
            true,
            "Set to true/enabled to patch new pieces to have comfort values like their vanilla counterparts."
        );

        EnablePlayerBasePatches = Config.BindConfigInOrder(
            CustomizationSection,
            "PlayerBasePatches (Requires Restart)",
            true,
            "Set to true/enabled to patch player-built instances of new torches, fires, " +
            "and beds so they suppress monster spawning just like their vanilla counterparts."
        );

        EnableSeasonalPieces = Config.BindConfigInOrder(
            CustomizationSection,
            "SeasonalPieces",
            true,
            "Set to true/enabled to add all currently disabled seasonal pieces to the hammer build table."
        );

        EnablePortalPatch = Config.BindConfigInOrder(
            CustomizationSection,
            "PortalPatch",
            true,
            "Set to true/enabled to have the new portal allow unrestricted teleporting. " +
            "Set to false/disabled to have the new portal work the same as the vanilla portal."
        );

        // Texture Section
        PortalTexture = Config.BindConfigInOrder(
            TextureSection,
            "PortalTexturePatch (Requires Restart)",
            false,
            "Set to true/enabled to change the texture of the new portal to appear " +
            "as if it was created by those who dwell in the Mistlands. " +
            "\nNote: change in appearance will not work for users without this mod."
        );

        DvergrWoodTexture = Config.BindConfigInOrder(
            TextureSection,
            "DvergrWoodPatch (Requires Restart)",
            false,
            "Set to true/enabled to change the texture of the player built instances of " +
            "of Dvergr wood floors and stairs to appear as if they were brand new. " +
            "\nNote: change in appearance will not work for users without this mod."
        );

        // Unsafe Section
        EnableBedPatches = Config.BindConfigInOrder(
            UnsafeSection,
            "BedPatches (Requires Restart, Unsafe)",
            false,
            "Set to true/enabled to patch player-built instances of new beds so you can sleep in them." +
            "\nWARNING: enabling this setting can result in you losing your spawn point" +
            " if had set your spawn using a patched bed and log in without this mod."
        );

        EnableFermenterPatches = Config.BindConfigInOrder(
            UnsafeSection,
            "FermenterPatches (Requires Restart, Unsafe)",
            false,
            "Set to true/enabled to patch player-built instances of fermenting barrels " +
            "to function as a fermenter that are 30% faster than the vanilla fermenter." +
            "\nWARNING: enabling this setting can result in you losing the mead base that " +
            "is fermenting if you load the area without this mod."
        );

        // Set up event hooks
        CreativeMode.SettingChanged += PieceSettingChanged;
        ForceAllPrefabs.SettingChanged += PieceSettingChanged;
        CreatorShopAdminOnly.SettingChanged += PieceSettingChanged;
        EnableHammerCrops.SettingChanged += PieceSettingChanged;

        AdminDeconstructOtherPlayers.SettingChanged += ModSettingChanged;
        Log.Verbosity.SettingChanged += ModSettingChanged;

        EnableSeasonalPieces.SettingChanged += SeasonalSettingChanged;
    }



    // Public API Section

    /// <summary>
    ///     Checks if the root prefab of the GameObject has had a 
    ///     Piece component added to it by MVBP. So this method can also
    ///     be used on any clones of the root prefab.
    /// </summary>
    /// <param name="prefab">GameObject to check.</param>
    /// <param name="piece">Optional piece component to prevent duplicate GetComponent calls.</param>
    /// <returns>True if MVBP has added a Piece component, False otherwise.</returns>
    public bool IsPieceAddedByMVBP(GameObject prefab, Piece piece = null)
    {
        return PieceHelper.IsPieceAddedByMVBP(prefab, piece);
    }


}
