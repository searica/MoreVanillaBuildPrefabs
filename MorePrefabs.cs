// Ignore Spelling: Plugin MVBP

using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using HarmonyLib;
using Jotunn.Configs;
using Jotunn.Managers;
using Jotunn.Utils;
using MVBP.Configs;
using MVBP.Helpers;
using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace MVBP
{
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    [BepInDependency(Jotunn.Main.ModGuid, Jotunn.Main.Version)]
    [BepInDependency(ModCompat.ExtraSnapsGUID, BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency(ModCompat.PlanBuildGUID, BepInDependency.DependencyFlags.SoftDependency)]
    [NetworkCompatibility(CompatibilityLevel.VersionCheckOnly, VersionStrictness.Patch)]
    public class MorePrefabs : BaseUnityPlugin
    {
        public const string PluginName = "MoreVanillaBuildPrefabs";
        internal const string Author = "Searica";
        public const string PluginGUID = $"{Author}.Valheim.{PluginName}";
        public const string PluginVersion = "1.0.1";

        #region Global Settings

        private static readonly string MainSection = ConfigManager.SetStringPriority("Global", 5);

        private static ConfigEntry<bool> CreativeMode { get; set; }
        private static ConfigEntry<bool> ForceAllPrefabs { get; set; }
        internal static bool IsCreativeMode => CreativeMode.Value;
        internal static bool IsForceAllPrefabs => ForceAllPrefabs.Value;

        #endregion Global Settings

        #region Admin Settings

        private static readonly string AdminSection = ConfigManager.SetStringPriority("Admin", 4);
        private static ConfigEntry<bool> CreatorShopAdminOnly { get; set; }
        private static ConfigEntry<bool> AdminDeconstructOtherPlayers { get; set; }
        internal static bool IsCreatorShopAdminOnly => CreatorShopAdminOnly.Value;
        internal static bool IsAdminDeconstructOtherPlayers => AdminDeconstructOtherPlayers.Value;

        #endregion Admin Settings

        #region Customization Settings

        private static readonly string CustomizationSection = ConfigManager.SetStringPriority("Customization", 3);
        private static ConfigEntry<bool> EnableHammerCrops { get; set; }
        private static ConfigEntry<bool> EnableDoorPatches { get; set; }
        private static ConfigEntry<bool> EnableComfortPatches { get; set; }
        private static ConfigEntry<bool> EnableSeasonalPieces { get; set; }
        private static ConfigEntry<bool> EnablePlayerBasePatches { get; set; }
        private static ConfigEntry<bool> EnablePortalPatch { get; set; }
        internal static bool IsEnableHammerCrops => EnableHammerCrops.Value;
        internal static bool IsEnableDoorPatches => EnableDoorPatches.Value;
        internal static bool IsEnableComfortPatches => EnableComfortPatches.Value;
        internal static bool IsEnableSeasonalPieces => EnableSeasonalPieces.Value;
        internal static bool IsEnablePlayerBasePatches => EnablePlayerBasePatches.Value;
        internal static bool IsEnablePortalPatch => EnablePortalPatch.Value;

        #endregion Customization Settings

        #region Texture Patches

        private static readonly string TextureSection = ConfigManager.SetStringPriority("Textures", 2);

        private static ConfigEntry<bool> PortalTexture;
        private static ConfigEntry<bool> DvergrWoodTexture;
        internal static bool PatchPortalTexture => PortalTexture.Value;
        internal static bool PatchDvergrWoodTexture => DvergrWoodTexture.Value;

        #endregion Texture Patches

        #region Unsafe Patches

        private static readonly string UnsafeSection = ConfigManager.SetStringPriority("Unsafe Patches", 1);
        private static ConfigEntry<bool> EnableBedPatches { get; set; }
        private static ConfigEntry<bool> EnableFermenterPatches { get; set; }
        internal static bool IsEnableBedPatches => EnableBedPatches.Value;
        internal static bool IsEnableFermenterPatches => EnableFermenterPatches.Value;

        #endregion Unsafe Patches

        #region Prefab Settings

        internal class PrefabDBConfig
        {
            internal ConfigEntry<bool> enabled;
            internal ConfigEntry<bool> allowedInDungeons;
            internal ConfigEntry<string> category;
            internal ConfigEntry<string> craftingStation;
            internal ConfigEntry<string> requirements;
            internal ConfigEntry<bool> placementPatch;
            internal ConfigEntry<bool> clipEverything;
            internal ConfigEntry<bool> clipGround;
        }

        private static readonly Dictionary<string, PrefabDBConfig> PrefabDBConfigsMap = new();

        internal static bool IsPrefabConfigEnabled(string prefabName) => PrefabDBConfigsMap[prefabName].enabled.Value;

        #endregion Prefab Settings

        #region Update Flags & Checks

        internal static bool UpdatePieceSettings { get; set; } = false;
        internal static bool UpdatePlacementSettings { get; set; } = false;
        internal static bool UpdateModSettings { get; set; } = false;
        internal static bool UpdateSeasonalSettings { get; set; } = false;

        internal static readonly HashSet<string> _NeedsCollisionPatch = new();

        /// <summary>
        ///     Event hook to set whether a config entry
        ///     for a piece setting has been changed.
        /// </summary>
        internal static void PieceSettingChanged(object obj, EventArgs args)
        {
            if (!UpdatePieceSettings) UpdatePieceSettings = true;
        }

        /// <summary>
        ///     Event hook to set whether a config entry
        ///     for placement patches has been changed.
        /// </summary>
        internal static void PlacementSettingChanged(object obj, EventArgs args)
        {
            if (!UpdatePlacementSettings) UpdatePlacementSettings = true;
        }

        /// <summary>
        ///     Event hook to set whether a config entry
        ///     for general mod settings has been changed.
        /// </summary>
        internal static void ModSettingChanged(object obj, EventArgs args)
        {
            if (!UpdateModSettings) UpdateModSettings = true;
        }

        internal static void SeasonalSettingChanged(object obj, EventArgs args)
        {
            if (!UpdateSeasonalSettings) UpdateSeasonalSettings = true;
        }

        /// <summary>
        ///     Get a bool indicating if the prefab is configured to require a placement patch.
        /// </summary>
        /// <param name="PrefabName"></param>
        /// <returns></returns>
        internal static bool NeedsCollisionPatchForGhost(string prefabName)
        {
            if (PrefabDBConfigsMap.TryGetValue(prefabName, out PrefabDBConfig prefabDBConfig))
            {
                // If there is no configuration option then always apply the placement patch
                if (prefabDBConfig.placementPatch == null)
                {
                    return true;
                }
                return prefabDBConfig.placementPatch.Value;
            }
            return false;
        }

        #endregion Update Flags & Checks

        public void Awake()
        {
            Log.Init(Logger);
            ConfigManager.Init(PluginGUID, Config, false);
            Initialize();
            ConfigManager.Save();

            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), harmonyInstanceId: PluginGUID);

            Game.isModded = true;

            ConfigManager.SetupWatcher();
            ConfigManager.CheckForConfigManager();

            // Re-initialization after reloading config and don't save since file was just reloaded
            ConfigManager.OnConfigFileReloaded += () =>
            {
                InitManager.UpdatePlugin("Configuration file changed, re-initializing", saveConfig: false);
            };

            // Re-initialize after changing config data in-game and trigger a save to disk.
            ConfigManager.OnConfigWindowClosed += () => InitManager.UpdatePlugin("Configuration changed in-game, re-initializing");

            // Re-initialize after getting updated config data and trigger a save to disk.
            SynchronizationManager.OnConfigurationSynchronized += (obj, attr) =>
            {
                InitManager.UpdatePlugin("Configuration synced, re-initializing");
            };
        }

        public void OnDestroy()
        {
            ConfigManager.Save();
        }

        internal static void Initialize()
        {
            CreativeMode = ConfigManager.BindConfig(
                MainSection,
                "CreativeMode",
                false,
                "Set to true/enabled to enable pieces from the CreatorShop or Nature piece categories. " +
                "By default, pieces set to those categories are not standard build pieces."
            );

            ForceAllPrefabs = ConfigManager.BindConfig(
                MainSection,
                "ForceAllPrefabs",
                false,
                "If true/enabled, adds all prefabs to the hammer for building. Unless CreativeMode is " +
                "also enabled it will not add pieces set to the CreatorShop or Nature category though."
            );

            Log.Verbosity = ConfigManager.BindConfig(
                MainSection,
                "Verbosity",
                LogLevel.Low,
                "Low will log basic information about the mod. Medium will log information that " +
                "is useful for troubleshooting. High will log a lot of information, do not set " +
                "it to this without good reason as it will slow down your game.",
                synced: false
            );

            CreatorShopAdminOnly = ConfigManager.BindConfig(
                AdminSection,
                "CreatorShopAdminOnly",
                false,
                "Set to true/enabled to restrict placement and deconstruction of CreatorShop pieces " +
                "to players with Admin status."
            );

            AdminDeconstructOtherPlayers = ConfigManager.BindConfig(
                AdminSection,
                "AdminDeconstructOtherPlayers",
                true,
                "Set to true/enabled to allow admin players to deconstruct any pieces built by other players," +
                " even if doing so would normally be prevented (such as for CreatorShop or Nature pieces)." +
                " Intended to prevent griefing via placement of indestructible objects."
            );

            // Customization settings
            EnableHammerCrops = ConfigManager.BindConfig(
                CustomizationSection,
                "HammerCrops",
                false,
                "Set to true/enabled to enable placing vanilla crops with the hammer." +
                " Unless this setting is true Vanilla crops will not be available for placing with the hammer."
            );

            EnableComfortPatches = ConfigManager.BindConfig(
                CustomizationSection,
                "ComfortPatches (Requires Restart)",
                true,
                "Set to true/enabled to patch new pieces to have comfort values like their vanilla counterparts."
            );

            EnableDoorPatches = ConfigManager.BindConfig(
                CustomizationSection,
                "DoorPatches (Requires Restart)",
                true,
                "Set to true/enabled to patch player-built instances of new doors " +
                "(that do not require keys) to allow closing them even if that is normally prevented."
            );

            EnablePlayerBasePatches = ConfigManager.BindConfig(
                CustomizationSection,
                "PlayerBasePatches (Requires Restart)",
                true,
                "Set to true/enabled to patch player-built instances of new torches, fires, " +
                "and beds so they suppress monster spawning just like their vanilla counterparts."
            );

            EnableSeasonalPieces = ConfigManager.BindConfig(
                CustomizationSection,
                "SeasonalPieces",
                true,
                "Set to true/enabled to add all currently disabled seasonal pieces to the hammer build table."
            );

            EnablePortalPatch = ConfigManager.BindConfig(
                CustomizationSection,
                "PortalPatch",
                true,
                "Set to true/enabled to have the new portal allow unrestricted teleporting. " +
                "Set to false/disabled to have the new portal work the same as the vanilla portal."
            );

            // Texture Section
            PortalTexture = ConfigManager.BindConfig(
                TextureSection,
                "PortalTexturePatch (Requires Restart)",
                false,
                "Set to true/enabled to change the texture of the new portal to appear " +
                "as if it was created by those who dwell in the Mistlands. " +
                "\nNote: change in appearance will not work for users without this mod."
            );

            DvergrWoodTexture = ConfigManager.BindConfig(
                TextureSection,
                "DvergrWoodPatch (Requires Restart)",
                false,
                "Set to true/enabled to change the texture of the player built instances of " +
                "of Dvergr wood floors and stairs to appear as if they were brand new. " +
                "\nNote: change in appearance will not work for users without this mod."
            );

            // Unsafe Section
            EnableBedPatches = ConfigManager.BindConfig(
                UnsafeSection,
                "BedPatches (Requires Restart, Unsafe)",
                false,
                "Set to true/enabled to patch player-built instances of new beds so you can sleep in them." +
                "\nWARNING: enabling this setting can result in you losing your spawn point" +
                " if had set your spawn using a patched bed and log in without this mod."
            );

            EnableFermenterPatches = ConfigManager.BindConfig(
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

        /// <summary>
        ///     Gets a PrefanDB instance based on the configuration settings.
        /// </summary>
        /// <param name="prefab"></param>
        /// <returns></returns>
        internal static PrefabDB GetPrefabDB(GameObject prefab)
        {
            string sectionName = prefab.name;

            // get predefined configs or generic settings if no predefined config
            PrefabDB defaultPrefabDB = PrefabConfigs.GetDefaultPrefabDB(prefab.name);
            PrefabDBConfig prefabDBConfig;

            if (PrefabDBConfigsMap.ContainsKey(sectionName)) // already loaded
            {
                // configure PrefabDB based on existing ConfigEntries
                prefabDBConfig = PrefabDBConfigsMap[sectionName];
                defaultPrefabDB.enabled = prefabDBConfig.enabled.Value;
                defaultPrefabDB.allowedInDungeons = prefabDBConfig.allowedInDungeons.Value;
                defaultPrefabDB.category = prefabDBConfig.category.Value;
                defaultPrefabDB.craftingStation = prefabDBConfig.craftingStation.Value;
                defaultPrefabDB.requirements = prefabDBConfig.requirements.Value;
                if (prefabDBConfig.clipEverything != null)
                {
                    defaultPrefabDB.clipEverything = prefabDBConfig.clipEverything.Value;
                }
                if (prefabDBConfig.clipGround != null)
                {
                    defaultPrefabDB.clipGround = prefabDBConfig.clipGround.Value;
                }
                return defaultPrefabDB;
            }

            var saveSetting = ConfigManager.DisableSaveOnConfigSet();
            prefabDBConfig = new();

            prefabDBConfig.enabled = ConfigManager.BindConfig(
                sectionName,
                ConfigManager.SetStringPriority("Enabled", 1),
                defaultPrefabDB.enabled,
                "If true then allow this prefab to be built and deconstructed. " +
                "Note: this setting is ignored if ForceAllPrefabs is true. " +
                "It is also ignored if the piece category is CreatorShop or Nature " +
                "and CreativeMode is false."
            );
            prefabDBConfig.enabled.SettingChanged += PieceSettingChanged;
            defaultPrefabDB.enabled = prefabDBConfig.enabled.Value;

            prefabDBConfig.allowedInDungeons = ConfigManager.BindConfig(
                sectionName,
                "AllowedInDungeons",
                defaultPrefabDB.allowedInDungeons,
                "If true then this prefab can be built inside dungeon zones."
            );
            prefabDBConfig.allowedInDungeons.SettingChanged += PieceSettingChanged;
            defaultPrefabDB.allowedInDungeons = prefabDBConfig.allowedInDungeons.Value;

            prefabDBConfig.category = ConfigManager.BindConfig(
                sectionName,
                "Category",
                defaultPrefabDB.category,
                "A string defining the tab the prefab shows up on in the hammer build table.",
                HammerCategories.GetAcceptableValueList()
            );
            prefabDBConfig.category.SettingChanged += PieceSettingChanged;
            defaultPrefabDB.category = prefabDBConfig.category.Value;

            prefabDBConfig.craftingStation = ConfigManager.BindConfig(
                sectionName,
                "CraftingStation",
                defaultPrefabDB.craftingStation,
                "A string defining the crafting station required to built the prefab.",
                CraftingStations.GetAcceptableValueList()
            );
            prefabDBConfig.craftingStation.SettingChanged += PieceSettingChanged;
            defaultPrefabDB.craftingStation = prefabDBConfig.craftingStation.Value;

            prefabDBConfig.requirements = ConfigManager.BindConfig(
                sectionName,
                "Requirements",
                defaultPrefabDB.requirements,
                "Resources required to build the prefab. Formatted as: itemID,amount;itemID,amount where itemID is the in-game identifier for the resource and amount is an integer. "
            );
            prefabDBConfig.requirements.SettingChanged += PieceSettingChanged;
            defaultPrefabDB.requirements = prefabDBConfig.requirements.Value;

            // if the prefab is not already set to use the placement patch by default
            // then add a config option to enable the placement collision patch.
            if (!defaultPrefabDB.placementPatch)
            {
                prefabDBConfig.placementPatch = ConfigManager.BindConfig(
                    sectionName,
                    "PlacementPatch",
                    false,
                    "Set to true to enable collision patching during placement of the piece. " +
                    "Recommended to try this if the piece is not appearing when you go to place it.\n" +
                    "(If this setting fixes the issue please let me know via Github or Discord so I can change the default settings.)"
                );
                prefabDBConfig.placementPatch.SettingChanged += PlacementSettingChanged;
                defaultPrefabDB.placementPatch = prefabDBConfig.placementPatch.Value;
            }

            if (!defaultPrefabDB.clipEverything)
            {
                prefabDBConfig.clipEverything = ConfigManager.BindConfig(
                    sectionName,
                    "ClipEverything",
                    false,
                    "Set to true to allow piece to clip through everything during placement. Recommended to try this if the piece is not appearing when you go to place it.\n" +
                    "(If this setting fixes the issue please let me know via Github or Discord so I can change the default settings.)"
                );
                prefabDBConfig.clipEverything.SettingChanged += PieceSettingChanged;
                defaultPrefabDB.clipEverything = prefabDBConfig.clipEverything.Value;
            }

            if (!defaultPrefabDB.clipGround)
            {
                prefabDBConfig.clipGround = ConfigManager.BindConfig(
                    sectionName,
                    "ClipGround",
                    false,
                    "Set to true to allow piece to clip through ground during placement.Recommended to try this if the piece is not floating when you try to place it.\n" +
                    "(If this setting fixes the issue please let me know via Github or Discord so I can change the default settings.)"
                );
                prefabDBConfig.clipGround.SettingChanged += PieceSettingChanged;
                defaultPrefabDB.clipGround = prefabDBConfig.clipGround.Value;
            }

            ConfigManager.SaveOnConfigSet(saveSetting);

            // keep a reference to the config entries for later use and making sure events are fired correctly
            PrefabDBConfigsMap[prefab.name] = prefabDBConfig;
            return defaultPrefabDB;
        }
    }

    /// <summary>
    ///     Log level to control output to BepInEx log
    /// </summary>
    internal enum LogLevel
    {
        Low = 0,
        Medium = 1,
        High = 2,
    }

    /// <summary>
    ///     Helper class for properly logging from static contexts.
    /// </summary>
    internal static class Log
    {
        #region Verbosity

        internal static ConfigEntry<LogLevel> Verbosity { get; set; }
        internal static LogLevel VerbosityLevel => Verbosity.Value;
        internal static bool IsVerbosityLow => Verbosity.Value >= LogLevel.Low;
        internal static bool IsVerbosityMedium => Verbosity.Value >= LogLevel.Medium;
        internal static bool IsVerbosityHigh => Verbosity.Value >= LogLevel.High;

        #endregion Verbosity

        private static ManualLogSource logSource;

        internal static void Init(ManualLogSource logSource)
        {
            Log.logSource = logSource;
        }

        internal static void LogDebug(object data) => logSource.LogDebug(data);

        internal static void LogError(object data) => logSource.LogError(data);

        internal static void LogFatal(object data) => logSource.LogFatal(data);

        internal static void LogMessage(object data) => logSource.LogMessage(data);

        internal static void LogWarning(object data) => logSource.LogWarning(data);

        internal static void LogInfo(object data, LogLevel level = LogLevel.Low)
        {
            if (Verbosity is null || VerbosityLevel >= level)
            {
                logSource.LogInfo(data);
            }
        }

        internal static void LogGameObject(GameObject prefab, bool includeChildren = false)
        {
            LogInfo("***** " + prefab.name + " *****");
            foreach (Component compo in prefab.GetComponents<Component>())
            {
                LogComponent(compo);
            }

            if (!includeChildren) { return; }

            LogInfo("***** " + prefab.name + " (children) *****");
            foreach (Transform child in prefab.transform)
            {
                LogInfo($" - {child.gameObject.name}");
                foreach (Component compo in child.gameObject.GetComponents<Component>())
                {
                    LogComponent(compo);
                }
            }
        }

        internal static void LogComponent(Component compo)
        {
            LogInfo($"--- {compo.GetType().Name}: {compo.name} ---");

            PropertyInfo[] properties = compo.GetType().GetProperties(ReflectionUtils.AllBindings);
            foreach (var property in properties)
            {
                LogInfo($" - {property.Name} = {property.GetValue(compo)}");
            }

            FieldInfo[] fields = compo.GetType().GetFields(ReflectionUtils.AllBindings);
            foreach (var field in fields)
            {
                LogInfo($" - {field.Name} = {field.GetValue(compo)}");
            }
        }
    }
}