using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Configuration;
using Jotunn.Configs;
using Jotunn.Managers;
using MoreVanillaBuildPrefabs.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
using static MoreVanillaBuildPrefabs.MoreVanillaBuildPrefabs;

namespace MoreVanillaBuildPrefabs.Configs
{
    internal class PluginConfig
    {
        private static BaseUnityPlugin configurationManager;

        private static readonly string ConfigFileName = PluginGUID + ".cfg";

        private static readonly string ConfigFileFullPath = string.Concat(
            Paths.ConfigPath,
            Path.DirectorySeparatorChar,
            ConfigFileName
        );

        private static ConfigFile configFile;

        private static readonly ConfigurationManagerAttributes AdminConfig = new() { IsAdminOnly = true };
        private static readonly ConfigurationManagerAttributes ClientConfig = new() { IsAdminOnly = false };

        internal enum LoggerLevel
        {
            Low = 0,
            Medium = 1,
            High = 2,
        }

        private const string MainSectionName = "\u200BGlobal";

        internal static ConfigEntry<bool> CreativeMode { get; private set; }
        internal static ConfigEntry<bool> CreatorShopAdminOnly { get; private set; }
        internal static ConfigEntry<bool> AdminDeconstructOtherPlayers { get; private set; }
        internal static ConfigEntry<bool> ForceAllPrefabs { get; private set; }
        internal static ConfigEntry<bool> EnableHammerCrops { get; private set; }
        internal static ConfigEntry<LoggerLevel> Verbosity { get; private set; }

        internal class PieceConfigEntries
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

        internal static readonly Dictionary<string, PieceConfigEntries> PieceConfigEntriesMap = new();

        private static readonly AcceptableValueList<bool> AcceptableBoolValuesList = new(new bool[] { false, true });

        internal static readonly HashSet<string> _NeedsCollisionPatch = new();

        internal static readonly HashSet<string> _ClipEverything = new();

        internal static readonly HashSet<string> _ClipGround = new()
        {
            "stoneblock_fracture",
            "blackmarble_post01",
            "dungeon_sunkencrypt_irongate_rusty",
        };

        /// <summary>
        ///     Get a bool indicating if the prefab should be
        ///     allowed to clip into the ground..
        /// </summary>
        /// <param name="PrefabName"></param>
        /// <returns></returns>
        internal static bool CanClipEverything(string prefabName)
        {
            return _ClipEverything.Contains(prefabName);
        }

        /// <summary>
        ///     Get a bool indicating if the prefab should be
        ///     allowed to clip into the ground.
        /// </summary>
        /// <param name="PrefabName"></param>
        /// <returns></returns>
        internal static bool CanClipGround(string prefabName)
        {
            return _ClipGround.Contains(prefabName);
        }

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

        internal static void Init(ConfigFile config)
        {
            configFile = config;
            configFile.SaveOnConfigSet = false;
        }

        internal static void Save()
        {
            configFile.Save();
        }

        internal static void SaveOnConfigSet(bool value)
        {
            configFile.SaveOnConfigSet = value;
        }

        internal static LoggerLevel VerbosityLevel => Verbosity.Value;

        internal static bool IsEnableHammerCrops => EnableHammerCrops.Value;
        internal static bool IsCreativeMode => CreativeMode.Value;
        internal static bool IsVerbosityLow => Verbosity.Value >= LoggerLevel.Low;
        internal static bool IsVerbosityMedium => Verbosity.Value >= LoggerLevel.Medium;
        internal static bool IsVerbosityHigh => Verbosity.Value >= LoggerLevel.High;
        internal static bool IsForceAllPrefabs => ForceAllPrefabs.Value;
        internal static bool IsCreatorShopAdminOnly => CreatorShopAdminOnly.Value;
        internal static bool IsAdminDeconstructOtherPlayers => AdminDeconstructOtherPlayers.Value;

        internal static void SetUpConfig()
        {
            CreativeMode = BindConfig(
                MainSectionName,
                "CreativeMode",
                false,
                "Setting to enable pieces set to the CreatorShop or Nature piece categories. " +
                "By default, the pieces set to those categories are not standard build pieces.",
                AcceptableBoolValuesList
            );

            CreatorShopAdminOnly = BindConfig(
                MainSectionName,
                "CreatorShopAdminOnly",
                false,
                "Set to true to restrict placement and deconstruction of CreatorShop pieces " +
                "to players with Admin status.",
                AcceptableBoolValuesList
            );

            AdminDeconstructOtherPlayers = BindConfig(
                MainSectionName,
                "AdminDeconstructOtherPlayers",
                true,
                "Set to true to allow admin players to deconstruct any pieces built by other players, " +
                "even if doing so would normally be prevented (such as for CreatorShop or Nature pieces). " +
                "Intended to prevent griefing via placement of indestructible objects.",
                AcceptableBoolValuesList
            );

            EnableHammerCrops = BindConfig(
                MainSectionName,
                "EnableHammerCrops",
                false,
                "Setting to enable prefabs for crops that can already be planted  " +
                "in the Vanilla game. Unless this setting is true Vanilla crops " +
                "will not be available for placing with the hammer.",
                AcceptableBoolValuesList
            );

            ForceAllPrefabs = BindConfig(
                MainSectionName,
                "ForceAllPrefabs",
                false,
                "If enabled, adds all prefabs to the hammer for building. Unless CreativeMode is " +
                "also enabled it will not add pieces set to the CreatorShop or Nature category though.",
                AcceptableBoolValuesList
            );

            Verbosity = BindConfig(
                MainSectionName,
                "Verbosity",
                LoggerLevel.Low,
                "Low will log basic information about the mod. Medium will log information that " +
                "is useful for troubleshooting. High will log a lot of information, do not set " +
                "it to this without good reason as it will slow down your game."
            );

            EnableHammerCrops.SettingChanged += PieceSettingChanged;
            CreatorShopAdminOnly.SettingChanged += PieceSettingChanged;
            CreativeMode.SettingChanged += PieceSettingChanged;
            ForceAllPrefabs.SettingChanged += PieceSettingChanged;
            Save();
        }

        internal static PrefabDB LoadPrefabDB(GameObject prefab)
        {
            string sectionName = prefab.name;

            // get predefined configs or generic settings if no predefined config
            PrefabDB defaultPieceDB = DefaultConfigs.GetDefaultPieceDB(prefab.name);
            PieceConfigEntries pieceConfigEntries = new();

            pieceConfigEntries.enabled = BindConfig(
                sectionName,
                "\u200BEnabled",
                defaultPieceDB.enabled,
                "If true then allow this prefab to be built and deconstructed. " +
                "Note: this setting is ignored if ForceAllPrefabs is true. " +
                "It is also ignored if the piece category is CreatorShop or Nature " +
                "and CreativeMode is false.",
                AcceptableBoolValuesList
            );
            pieceConfigEntries.enabled.SettingChanged += PieceSettingChanged;
            defaultPieceDB.enabled = pieceConfigEntries.enabled.Value;

            pieceConfigEntries.allowedInDungeons = BindConfig(
                sectionName,
                "AllowedInDungeons",
                defaultPieceDB.allowedInDungeons,
                "If true then this prefab can be built inside dungeon zones.",
                AcceptableBoolValuesList
            );
            pieceConfigEntries.allowedInDungeons.SettingChanged += PieceSettingChanged;
            defaultPieceDB.allowedInDungeons = pieceConfigEntries.allowedInDungeons.Value;

            pieceConfigEntries.category = BindConfig(
                sectionName,
                "Category",
                defaultPieceDB.category,
                "A string defining the tab the prefab shows up on in the hammer build table.",
                HammerCategories.GetAcceptableValueList()
            );
            pieceConfigEntries.category.SettingChanged += PieceSettingChanged;
            defaultPieceDB.category = pieceConfigEntries.category.Value;

            pieceConfigEntries.craftingStation = BindConfig(
                sectionName,
                "CraftingStation",
                defaultPieceDB.craftingStation,
                "A string defining the crafting station required to built the prefab.",
                CraftingStations.GetAcceptableValueList()
            );
            pieceConfigEntries.craftingStation.SettingChanged += PieceSettingChanged;
            defaultPieceDB.craftingStation = pieceConfigEntries.craftingStation.Value;

            pieceConfigEntries.requirements = BindConfig(
                sectionName,
                "Requirements",
                defaultPieceDB.requirements,
                "Resources required to build the prefab. Formatted as: itemID,amount;itemID,amount where itemID is the in-game identifier for the resource and amount is an integer. "
            );
            pieceConfigEntries.requirements.SettingChanged += PieceSettingChanged;
            defaultPieceDB.requirements = pieceConfigEntries.requirements.Value;

            // if the prefab is not already set to use the placement patch by default
            // then add a config option to enable the placement collision patch.
            if (!defaultPieceDB.placementPatch)
            {
                pieceConfigEntries.placementPatch = BindConfig(
                    sectionName,
                    "PlacementPatch",
                    false,
                    "Set to true to enable collision patching during placement of the piece.\n" +
                    "Recommended to try this if the piece is not appearing when you go to place it.\n\n" +
                    " If enabling the placement patch via this setting fixes the issue please open an issue on Github" +
                    " letting me know so I can make sure the collision patch is always applied to this piece.",
                    AcceptableBoolValuesList
                );
                pieceConfigEntries.placementPatch.SettingChanged += PlacementSettingChanged;
                defaultPieceDB.placementPatch = pieceConfigEntries.placementPatch.Value;
            }
            if (defaultPieceDB.placementPatch) { _NeedsCollisionPatch.Add(prefab.name); }

            if (!defaultPieceDB.clipEverything)
            {
                pieceConfigEntries.clipEverything = BindConfig(
                    sectionName,
                    "ClipEverything",
                    false,
                    "Set to true to allow piece to clip through everything during placement. Recommended to try this if the piece is not appearing when you go to place it.\n" +
                    "If this setting fixes the issue please open an issue on Github letting me know so I can make sure the collision patch is always applied to this piece.",
                    AcceptableBoolValuesList
                );
                pieceConfigEntries.clipEverything.SettingChanged += PieceSettingChanged;
                defaultPieceDB.clipEverything = pieceConfigEntries.clipEverything.Value;
            }
            if (defaultPieceDB.clipEverything) { _ClipEverything.Add(prefab.name); }

            if (!defaultPieceDB.clipGround)
            {
                pieceConfigEntries.clipGround = BindConfig(
                sectionName,
                "ClipGround",
                false,
                "Set to true to allow piece to clip through ground during placement.Recommended to try this if the piece is not floating when you try to place it.\n" +
                "(If this setting fixes the issue please open an issue on Github letting me know so I can make sure the piece can always applied clip the ground.)",
                    AcceptableBoolValuesList
                );
                pieceConfigEntries.clipGround.SettingChanged += PieceSettingChanged;
                defaultPieceDB.clipGround = pieceConfigEntries.clipGround.Value;
            }
            if (defaultPieceDB.clipGround) { _ClipGround.Add(prefab.name); }

            // keep a reference to the config entries
            // to make sure the events fire as intended
            PieceConfigEntriesMap[prefab.name] = pieceConfigEntries;
            return defaultPieceDB;
        }

        internal static void SetupWatcher()
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
                var saveOnConfig = configFile.SaveOnConfigSet;
                configFile.SaveOnConfigSet = false;
                configFile.Reload();
                configFile.SaveOnConfigSet = saveOnConfig;
            }
            catch
            {
                Log.LogError($"There was an issue loading your {ConfigFileName}");
                Log.LogError("Please check your config entries for spelling and format!");
            }
            // run a single re-initialization to deal with all changed data
            ReInitPlugin("Config settings changed, re-initializing");
        }

        internal static void CheckForConfigManager()
        {
            if (GUIManager.IsHeadless())
            {
                return;
            }

            if (
                Chainloader.PluginInfos.TryGetValue(
                    "com.bepis.bepinex.configurationmanager",
                    out PluginInfo configManagerInfo
                )
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
                PluginConfig.Save();
                //ReInitPlugin("Config settings changed via in-game manager, re-initializing");
            }
        }
    }
}