﻿// Ignore Spelling: MVBP

using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Configuration;
using Jotunn.Configs;
using Jotunn.Managers;
using MVBP.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
using MVBP.Extensions;

namespace MVBP.Configs
{
    internal class Config
    {
        private static readonly string ConfigFileName = MoreVanillaBuildPrefabs.PluginGUID + ".cfg";

        private static readonly string ConfigFileFullPath = string.Concat(
            Paths.ConfigPath,
            Path.DirectorySeparatorChar,
            ConfigFileName
        );

        private static ConfigFile configFile;
        private static BaseUnityPlugin configurationManager;

        private static readonly AcceptableValueList<bool> AcceptableBoolValuesList = new(new bool[] { false, true });

        private const string MainSection = "\u200B\u200BGlobal";
        private const string AdminSection = "\u200BAdmin";
        private const string CustomizationSection = "\u200BCustomization";

        #region Events

        /// <summary>
        ///     Event triggered after a the in-game configuration manager is closed.
        /// </summary>
        internal static event Action OnConfigWindowClosed;

        /// <summary>
        ///     Safely invoke the <see cref="OnConfigWindowClosed"/> event
        /// </summary>
        private static void InvokeOnConfigWindowClosed()
        {
            OnConfigWindowClosed?.SafeInvoke();
        }

        /// <summary>
        ///     Event triggered after the file watcher reloads the configuration file.
        /// </summary>
        internal static event Action OnConfigFileReloaded;

        /// <summary>
        ///     Safely invoke the <see cref="OnConfigFileReloaded"/> event
        /// </summary>
        private static void InvokeOnConfigFileReloaded()
        {
            OnConfigFileReloaded?.SafeInvoke();
        }

        #endregion Events

        #region Global Settings

        internal enum LoggerLevel
        {
            Low = 0,
            Medium = 1,
            High = 2,
        }

        internal static ConfigEntry<bool> CreativeMode { get; private set; }
        internal static ConfigEntry<bool> ForceAllPrefabs { get; private set; }
        internal static ConfigEntry<LoggerLevel> Verbosity { get; private set; }
        internal static bool IsCreativeMode => CreativeMode.Value;
        internal static bool IsForceAllPrefabs => ForceAllPrefabs.Value;
        internal static bool IsVerbosityLow => Verbosity.Value >= LoggerLevel.Low;
        internal static bool IsVerbosityMedium => Verbosity.Value >= LoggerLevel.Medium;
        internal static bool IsVerbosityHigh => Verbosity.Value >= LoggerLevel.High;

        #endregion Global Settings

        #region Admin Settings

        internal static ConfigEntry<bool> CreatorShopAdminOnly { get; private set; }
        internal static ConfigEntry<bool> AdminDeconstructOtherPlayers { get; private set; }
        internal static bool IsCreatorShopAdminOnly => CreatorShopAdminOnly.Value;
        internal static bool IsAdminDeconstructOtherPlayers => AdminDeconstructOtherPlayers.Value;

        #endregion Admin Settings

        #region Customization Settings

        internal static ConfigEntry<bool> EnableHammerCrops { get; private set; }
        internal static ConfigEntry<bool> ApplyDoorPatches { get; private set; }
        internal static ConfigEntry<bool> ApplyComfortPatches { get; private set; }
        internal static ConfigEntry<bool> EnableSeasonalPieces { get; private set; }
        internal static bool IsEnableHammerCrops => EnableHammerCrops.Value;
        internal static bool IsApplyDoorPatches => ApplyDoorPatches.Value;
        internal static bool IsApplyComfortPatches => ApplyComfortPatches.Value;
        internal static bool IsEnableSeasonalPieces => EnableSeasonalPieces.Value;

        #endregion Customization Settings

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

        internal static readonly Dictionary<string, PrefabDBConfig> PrefabDBConfigsMap = new();

        #endregion Prefab Settings

        #region Update Flags & Checks

        internal static bool UpdatePieceSettings { get; set; } = false;
        internal static bool UpdatePlacementSettings { get; set; } = false;
        internal static bool UpdateModSettings { get; set; } = false;
        internal static bool UpdateSeasonalSettings { get; set; } = false;

        internal static readonly HashSet<string> _NeedsCollisionPatch = new();

        /// <summary>
        ///     Get a bool indicating if the prefab is in the
        ///     HashSet of prefabs that need a collision patch.
        /// </summary>
        /// <param name="PrefabName"></param>
        /// <returns></returns>
        internal static bool NeedsCollisionPatchForGhost(string prefabName)
        {
            return _NeedsCollisionPatch.Contains(prefabName);
        }

        #endregion Update Flags & Checks

        #region Config Binding

        private static readonly ConfigurationManagerAttributes AdminConfig = new() { IsAdminOnly = true };
        private static readonly ConfigurationManagerAttributes ClientConfig = new() { IsAdminOnly = false };

        internal static ConfigEntry<T> BindConfig<T>(
            string section,
            string name,
            T value,
            string description,
            AcceptableValueBase acceptVals = null,
            bool synced = true
        )
        {
            string extendedDescription = GetExtendedDescription(description, synced);
            ConfigEntry<T> configEntry = configFile.Bind(
                section,
                name,
                value,
                new ConfigDescription(
                    extendedDescription,
                    acceptVals,
                    synced ? AdminConfig : ClientConfig
                )
            );
            return configEntry;
        }

        internal static string GetExtendedDescription(string description, bool synchronizedSetting)
        {
            return description + (synchronizedSetting ? " [Synced with Server]" : " [Not Synced with Server]");
        }

        #endregion Config Binding

        internal static void Init(ConfigFile config)
        {
            configFile = config;
            configFile.SaveOnConfigSet = false;
        }

        #region Saving Config File

        /// <summary>
        ///     Save config file to disk.
        /// </summary>
        internal static void Save()
        {
            configFile.Save();
        }

        /// <summary>
        ///     Saves the config file if settings that need to be tracked have been changed.
        /// </summary>
        internal static bool SaveIfChanged()
        {
            if (UpdatePieceSettings || UpdatePlacementSettings || UpdateModSettings)
            {
                configFile.Save();
                if (UpdateModSettings) UpdateModSettings = false;
                return true;
            }
            return false;
        }

        /// <summary>
        ///     Set the value for the SaveOnConfigSet field.
        /// </summary>
        /// <param name="value"></param>
        internal static void SaveOnConfigSet(bool value)
        {
            configFile.SaveOnConfigSet = value;
        }

        /// <summary>
        ///     Sets SaveOnConfigSet to false and returns
        ///     the value prior to calling this method.
        /// </summary>
        /// <returns></returns>
        private static bool DisableSaveOnConfigSet()
        {
            var val = configFile.SaveOnConfigSet;
            configFile.SaveOnConfigSet = false;
            return val;
        }

        #endregion Saving Config File

        internal static void SetUpConfig()
        {
            var saveSetting = DisableSaveOnConfigSet();
            CreativeMode = BindConfig(
                MainSection,
                "CreativeMode",
                false,
                "Setting to enable pieces set to the CreatorShop or Nature piece categories. " +
                "By default, the pieces set to those categories are not standard build pieces.",
                AcceptableBoolValuesList
            );

            ForceAllPrefabs = BindConfig(
                MainSection,
                "ForceAllPrefabs",
                false,
                "If enabled, adds all prefabs to the hammer for building. Unless CreativeMode is " +
                "also enabled it will not add pieces set to the CreatorShop or Nature category though.",
                AcceptableBoolValuesList
            );

            Verbosity = BindConfig(
                MainSection,
                "Verbosity",
                LoggerLevel.Low,
                "Low will log basic information about the mod. Medium will log information that " +
                "is useful for troubleshooting. High will log a lot of information, do not set " +
                "it to this without good reason as it will slow down your game.",
                synced: false
            );

            CreatorShopAdminOnly = BindConfig(
                AdminSection,
                "CreatorShopAdminOnly",
                false,
                "Set to true to restrict placement and deconstruction of CreatorShop pieces " +
                "to players with Admin status.",
                AcceptableBoolValuesList
            );

            AdminDeconstructOtherPlayers = BindConfig(
                AdminSection,
                "AdminDeconstructOtherPlayers",
                true,
                "Set to true to allow admin players to deconstruct any pieces built by other players, " +
                "even if doing so would normally be prevented (such as for CreatorShop or Nature pieces). " +
                "Intended to prevent griefing via placement of indestructible objects.",
                AcceptableBoolValuesList
            );

            // Customization settings
            EnableHammerCrops = BindConfig(
                CustomizationSection,
                "EnableHammerCrops",
                false,
                "Setting to enable prefabs for crops that can already be planted  " +
                "in the Vanilla game. Unless this setting is true Vanilla crops " +
                "will not be available for placing with the hammer.",
                AcceptableBoolValuesList
            );

            ApplyComfortPatches = BindConfig(
                CustomizationSection,
                "ApplyComfortPatches (Requires Restart)",
                true,
                "Set to True to patch prefabs added by MVBP to have comfort values like corresponding Vanilla pieces.",
                AcceptableBoolValuesList
            );

            ApplyDoorPatches = BindConfig(
                CustomizationSection,
                "ApplyDoorPatches (Requires Restart)",
                true,
                "Set to True to patch player-built instances of doors so that they can be opened and closed." +
                " Currently only works for the sliding door piece.",
                AcceptableBoolValuesList
            );

            EnableSeasonalPieces = BindConfig(
                CustomizationSection,
                "EnableSeasonalPieces",
                true,
                "Set to True to enable seasonal pieces regardless of time of year." +
                " Has no effect on seasonal pieces that are already enabled in the Vanilla game.",
                AcceptableBoolValuesList
            );

            // Set up event hooks
            CreativeMode.SettingChanged += PieceSettingChanged;
            ForceAllPrefabs.SettingChanged += PieceSettingChanged;
            CreatorShopAdminOnly.SettingChanged += PieceSettingChanged;
            EnableHammerCrops.SettingChanged += PieceSettingChanged;

            AdminDeconstructOtherPlayers.SettingChanged += ModSettingChanged;
            Verbosity.SettingChanged += ModSettingChanged;

            EnableSeasonalPieces.SettingChanged += SeasonalSettingChanged;

            // trigger manual save and reset save settings
            Save();
            SaveOnConfigSet(saveSetting);
        }

        internal static PrefabDB LoadPrefabDB(GameObject prefab)
        {
            string sectionName = prefab.name;

            // get predefined configs or generic settings if no predefined config
            PrefabDB defaultPrefabDB = PrefabConfigs.GetDefaultPrefabDB(prefab.name);
            PrefabDBConfig prefabDBConfig;

            if (PrefabDBConfigsMap.ContainsKey(sectionName))
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

            var saveSetting = DisableSaveOnConfigSet();
            prefabDBConfig = new();

            prefabDBConfig.enabled = BindConfig(
                sectionName,
                "\u200BEnabled",
                defaultPrefabDB.enabled,
                "If true then allow this prefab to be built and deconstructed. " +
                "Note: this setting is ignored if ForceAllPrefabs is true. " +
                "It is also ignored if the piece category is CreatorShop or Nature " +
                "and CreativeMode is false.",
                AcceptableBoolValuesList
            );
            prefabDBConfig.enabled.SettingChanged += PieceSettingChanged;
            defaultPrefabDB.enabled = prefabDBConfig.enabled.Value;

            prefabDBConfig.allowedInDungeons = BindConfig(
                sectionName,
                "AllowedInDungeons",
                defaultPrefabDB.allowedInDungeons,
                "If true then this prefab can be built inside dungeon zones.",
                AcceptableBoolValuesList
            );
            prefabDBConfig.allowedInDungeons.SettingChanged += PieceSettingChanged;
            defaultPrefabDB.allowedInDungeons = prefabDBConfig.allowedInDungeons.Value;

            prefabDBConfig.category = BindConfig(
                sectionName,
                "Category",
                defaultPrefabDB.category,
                "A string defining the tab the prefab shows up on in the hammer build table.",
                HammerCategories.GetAcceptableValueList()
            );
            prefabDBConfig.category.SettingChanged += PieceSettingChanged;
            defaultPrefabDB.category = prefabDBConfig.category.Value;

            prefabDBConfig.craftingStation = BindConfig(
                sectionName,
                "CraftingStation",
                defaultPrefabDB.craftingStation,
                "A string defining the crafting station required to built the prefab.",
                CraftingStations.GetAcceptableValueList()
            );
            prefabDBConfig.craftingStation.SettingChanged += PieceSettingChanged;
            defaultPrefabDB.craftingStation = prefabDBConfig.craftingStation.Value;

            prefabDBConfig.requirements = BindConfig(
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
                prefabDBConfig.placementPatch = BindConfig(
                    sectionName,
                    "PlacementPatch",
                    false,
                    "Set to true to enable collision patching during placement of the piece.\n" +
                    "Recommended to try this if the piece is not appearing when you go to place it.\n\n" +
                    " If enabling the placement patch via this setting fixes the issue please open an issue on Github" +
                    " letting me know so I can make sure the collision patch is always applied to this piece.",
                    AcceptableBoolValuesList
                );
                prefabDBConfig.placementPatch.SettingChanged += PlacementSettingChanged;
                defaultPrefabDB.placementPatch = prefabDBConfig.placementPatch.Value;
            }
            if (defaultPrefabDB.placementPatch) { _NeedsCollisionPatch.Add(prefab.name); }

            if (!defaultPrefabDB.clipEverything)
            {
                prefabDBConfig.clipEverything = BindConfig(
                    sectionName,
                    "ClipEverything",
                    false,
                    "Set to true to allow piece to clip through everything during placement. Recommended to try this if the piece is not appearing when you go to place it.\n" +
                    "If this setting fixes the issue please open an issue on Github letting me know so I can make sure the collision patch is always applied to this piece.",
                    AcceptableBoolValuesList
                );
                prefabDBConfig.clipEverything.SettingChanged += PieceSettingChanged;
                defaultPrefabDB.clipEverything = prefabDBConfig.clipEverything.Value;
            }

            if (!defaultPrefabDB.clipGround)
            {
                prefabDBConfig.clipGround = BindConfig(
                sectionName,
                "ClipGround",
                false,
                "Set to true to allow piece to clip through ground during placement.Recommended to try this if the piece is not floating when you try to place it.\n" +
                "(If this setting fixes the issue please open an issue on Github letting me know so I can make sure the piece can always applied clip the ground.)",
                    AcceptableBoolValuesList
                );
                prefabDBConfig.clipGround.SettingChanged += PieceSettingChanged;
                defaultPrefabDB.clipGround = prefabDBConfig.clipGround.Value;
            }

            SaveOnConfigSet(saveSetting);
            // keep a reference to the config entries for later use
            // and making sure events are fired correctly
            PrefabDBConfigsMap[prefab.name] = prefabDBConfig;
            return defaultPrefabDB;
        }

        #region Config Syncing and File Watcher

        /// <summary>
        ///     Set up File-Watcher for configuration file and check for in-game configuration manager.
        /// </summary>
        internal static void SetUpSyncManagement()
        {
            SetupWatcher();
            CheckForConfigManager();

            // Re-initialization after reloading config and don't save since file was just reloaded
            OnConfigFileReloaded += () =>
            {
                InitManager.UpdatePlugin("Configuration file changed, re-initializing", saveConfig: false);
            };

            // Re-initialize after changing config data in-game and trigger a save to disk.
            OnConfigWindowClosed += () => InitManager.UpdatePlugin("Configuration changed in-game, re-initializing");

            // Re-initialize after getting updated config data and trigger a save to disk.
            SynchronizationManager.OnConfigurationSynchronized += (obj, attr) =>
            {
                InitManager.UpdatePlugin("Configuration synced, re-initializing");
            };
        }

        private static void SetupWatcher()
        {
            FileSystemWatcher watcher = new(Paths.ConfigPath, ConfigFileName);
            watcher.Changed += ReloadConfigFile;
            watcher.Created += ReloadConfigFile;
            watcher.Renamed += ReloadConfigFile;
            watcher.IncludeSubdirectories = true;
            watcher.SynchronizingObject = ThreadingHelper.SynchronizingObject;
            watcher.EnableRaisingEvents = true;
        }

        private static void ReloadConfigFile(object sender, FileSystemEventArgs e)
        {
            if (!File.Exists(ConfigFileFullPath)) return;
            try
            {
                Log.LogInfo("Reloading config file");
                var saveSetting = DisableSaveOnConfigSet();
                configFile.Reload();
                SaveOnConfigSet(saveSetting);
                InvokeOnConfigFileReloaded();
            }
            catch
            {
                Log.LogError($"There was an issue loading your {ConfigFileName}");
                Log.LogError("Please check your config entries for spelling and format!");
            }
        }

        private static void CheckForConfigManager()
        {
            if (GUIManager.IsHeadless())
            {
                return;
            }

            if (
                Chainloader.PluginInfos.TryGetValue("com.bepis.bepinex.configurationmanager", out PluginInfo configManagerInfo)
                && configManagerInfo.Instance
            )
            {
                configurationManager = configManagerInfo.Instance;
                Log.LogDebug("Configuration manager found, hooking DisplayingWindowChanged");

                EventInfo eventinfo = configurationManager.GetType()
                    .GetEvent("DisplayingWindowChanged");

                if (eventinfo != null)
                {
                    Action<object, object> local = new(OnConfigManagerDisplayingWindowChanged);
                    Delegate converted = Delegate.CreateDelegate(
                        eventinfo.EventHandlerType,
                        local.Target,
                        local.Method
                    );
                    eventinfo.AddEventHandler(configurationManager, converted);
                }
            }
        }

        private static void OnConfigManagerDisplayingWindowChanged(object sender, object e)
        {
            PropertyInfo pi = configurationManager.GetType().GetProperty("DisplayingWindow");
            bool cmActive = (bool)pi.GetValue(configurationManager, null);
            if (!cmActive)
            {
                InvokeOnConfigWindowClosed();
            }
        }

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

        #endregion Config Syncing and File Watcher
    }
}