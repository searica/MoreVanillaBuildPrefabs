// Ignore Spelling: Plugin MVBP

using BepInEx;
using BepInEx.Logging;
using HarmonyLib;
using Jotunn.Utils;
using Jotunn.Managers;
using System.Reflection;
using UnityEngine;
using MVBP.Configs;
using MVBP.Helpers;
using System.Drawing.Imaging;
using System.Drawing;
using System.IO;
using Jotunn.Configs;
using BepInEx.Configuration;
using System.Collections.Generic;
using System;

namespace MVBP
{
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    [BepInDependency(Jotunn.Main.ModGuid, Jotunn.Main.Version)]
    [BepInDependency(ModCompat.ExtraSnapsGUID, BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency(ModCompat.PlanBuildGUID, BepInDependency.DependencyFlags.SoftDependency)]
    [NetworkCompatibility(CompatibilityLevel.VersionCheckOnly, VersionStrictness.Patch)]
    public class MoreVanillaBuildPrefabs : BaseUnityPlugin
    {
        public const string PluginName = "MoreVanillaBuildPrefabs";
        internal const string Author = "Searica";
        public const string PluginGUID = $"{Author}.Valheim.{PluginName}";
        public const string PluginVersion = "0.6.1";

        #region Global Settings

        private static readonly string MainSection = ConfigManager.SetStringPriority("Global", 5);

        internal static ConfigEntry<bool> CreativeMode { get; private set; }
        internal static ConfigEntry<bool> ForceAllPrefabs { get; private set; }
        internal static bool IsCreativeMode => CreativeMode.Value;
        internal static bool IsForceAllPrefabs => ForceAllPrefabs.Value;

        #endregion Global Settings

        #region Admin Settings

        private static readonly string AdminSection = ConfigManager.SetStringPriority("Admin", 4);
        internal static ConfigEntry<bool> CreatorShopAdminOnly { get; private set; }
        internal static ConfigEntry<bool> AdminDeconstructOtherPlayers { get; private set; }
        internal static bool IsCreatorShopAdminOnly => CreatorShopAdminOnly.Value;
        internal static bool IsAdminDeconstructOtherPlayers => AdminDeconstructOtherPlayers.Value;

        #endregion Admin Settings

        #region Customization Settings

        private static readonly string CustomizationSection = ConfigManager.SetStringPriority("Customization", 3);
        internal static ConfigEntry<bool> EnableHammerCrops { get; private set; }
        internal static ConfigEntry<bool> EnableDoorPatches { get; private set; }
        internal static ConfigEntry<bool> EnableComfortPatches { get; private set; }
        internal static ConfigEntry<bool> EnableSeasonalPieces { get; private set; }
        internal static ConfigEntry<bool> EnablePlayerBasePatches { get; private set; }
        internal static ConfigEntry<bool> EnablePortalPatch { get; private set; }
        internal static bool IsEnableHammerCrops => EnableHammerCrops.Value;
        internal static bool IsEnableDoorPatches => EnableDoorPatches.Value;
        internal static bool IsEnableComfortPatches => EnableComfortPatches.Value;
        internal static bool IsEnableSeasonalPieces => EnableSeasonalPieces.Value;
        internal static bool IsEnablePlayerBasePatches => EnablePlayerBasePatches.Value;
        internal static bool IsEnablePortalPatch => EnablePortalPatch.Value;

        #endregion Customization Settings

        #region Texture Patches

        private static readonly string TextureSection = ConfigManager.SetStringPriority("Textures", 2);

        #endregion Texture Patches

        #region Unsafe Patches

        private static readonly string UnsafeSection = ConfigManager.SetStringPriority("Unsafe Patches", 1);
        internal static ConfigEntry<bool> EnableBedPatches { get; private set; }
        internal static ConfigEntry<bool> EnableFermenterPatches { get; private set; }
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

        internal static readonly Dictionary<string, PrefabDBConfig> PrefabDBConfigsMap = new();

        #endregion Prefab Settings

        #region Update Flags & Checks

        internal static bool UpdatePieceSettings { get; set; } = false;
        internal static bool UpdatePlacementSettings { get; set; } = false;
        internal static bool UpdateModSettings { get; set; } = false;
        internal static bool UpdateSeasonalSettings { get; set; } = false;

        internal static readonly HashSet<string> _NeedsCollisionPatch = new();

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

        internal static Texture2D LoadTextureFromResources(string fileName)
        {
            var extension = Path.GetExtension(fileName).ToLower();
            if (extension != ".png" && extension != ".jpg")
            {
                Log.LogWarning("LoadTextureFromResources can only load png or jpg textures");
                return null;
            }
            fileName = Path.GetFileNameWithoutExtension(fileName);

            var resource = Properties.Resources.ResourceManager.GetObject(fileName) as Bitmap;
            using (var mStream = new MemoryStream())
            {
                switch (extension)
                {
                    case ".jpg":
                        resource.Save(mStream, ImageFormat.Jpeg);
                        break;

                    case ".png":
                        resource.Save(mStream, ImageFormat.Png);
                        break;
                }

                var buffer = new byte[mStream.Length];
                mStream.Position = 0;
                mStream.Read(buffer, 0, buffer.Length);
                var texture = new Texture2D(0, 0);
                texture.LoadImage(buffer);
                return texture;
            }
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
                "Set to true/enabled to set the new portal to allow unrestricted teleporting. " +
                "Set to false/disabled to have the new portal work the same as the vanilla portal."
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

        #region Setting Changes

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

        #endregion Setting Changes

        internal static PrefabDB BindPrefabDB(GameObject prefab)
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
                    "Set to true to enable collision patching during placement of the piece.\n" +
                    "Recommended to try this if the piece is not appearing when you go to place it.\n\n" +
                    " If enabling the placement patch via this setting fixes the issue please open an issue on Github" +
                    " letting me know so I can make sure the collision patch is always applied to this piece."
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
                    "If this setting fixes the issue please open an issue on Github letting me know so I can make sure the collision patch is always applied to this piece."
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
                    "(If this setting fixes the issue please open an issue on Github letting me know so I can make sure the piece can always applied clip the ground.)"
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

        #endregion Verbosity

        internal static ManualLogSource _logSource;

        internal static void Init(ManualLogSource logSource)
        {
            _logSource = logSource;
        }

        internal static void LogDebug(object data) => _logSource.LogDebug(data);

        internal static void LogError(object data) => _logSource.LogError(data);

        internal static void LogFatal(object data) => _logSource.LogFatal(data);

        internal static void LogMessage(object data) => _logSource.LogMessage(data);

        internal static void LogWarning(object data) => _logSource.LogWarning(data);

        internal static void LogInfo(object data, LogLevel level = LogLevel.Low)
        {
            if (Verbosity is null || VerbosityLevel >= level)
            {
                _logSource.LogInfo(data);
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