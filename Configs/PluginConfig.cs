using System;
using System.IO;
using System.Collections.Generic;
using BepInEx;
using BepInEx.Configuration;
using UnityEngine;
using Jotunn.Configs;

using static MoreVanillaBuildPrefabs.MoreVanillaBuildPrefabs;
using MoreVanillaBuildPrefabs.Logging;


namespace MoreVanillaBuildPrefabs.Configs
{
    internal class PluginConfig
    {
        private static readonly string ConfigFileName = PluginGuid + ".cfg";
        private static readonly string ConfigFileFullPath = string.Concat(
            Paths.ConfigPath,
            Path.DirectorySeparatorChar,
            ConfigFileName
        );

        private static ConfigFile configFile;

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

        private const string MainSectionName = "\u200BGlobal";
        internal static ConfigEntry<bool> IsModEnabled { get; private set; }
        internal static ConfigEntry<bool> LockConfiguration { get; private set; }
        internal static ConfigEntry<bool> CreatorShopAdminOnly { get; private set; }
        internal static ConfigEntry<bool> CreatorShopDeconstructAdminOnly { get; private set; }
        internal static ConfigEntry<bool> ForceAllPrefabs { get; private set; }
        internal static ConfigEntry<bool> VerboseMode { get; private set; }


        internal class PieceConfigEntries
        {
            internal ConfigEntry<bool> enabled;
            internal ConfigEntry<bool> allowedInDungeons;
            internal ConfigEntry<string> category;
            internal ConfigEntry<string> craftingStation;
            internal ConfigEntry<string> requirements;
            internal ConfigEntry<bool> placementPatch;
        }

        internal static Dictionary<string, PieceConfigEntries> PieceConfigEntriesMap { get; private set; }

        private static readonly AcceptableValueList<bool> AcceptableBoolValuesList = new(new bool[] { false, true });

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

        internal static bool IsVerbose => VerboseMode.Value;
        internal static bool IsForceAllPrefabs => ForceAllPrefabs.Value;
        internal static bool IsCreatorShopAdminOnly => CreatorShopAdminOnly.Value;
        internal static bool IsCreatorShopDeconstructAdminOnly => CreatorShopDeconstructAdminOnly.Value;

        internal static void SetUpConfig()
        {
            CreatorShopAdminOnly = BindConfig(
                MainSectionName,
                "CreatorShopAdminOnly",
                false,
                "Set to true to restrict placement and deconstruction of CreatorShop pieces to players with Admin status.",
                AcceptableBoolValuesList
            );

            CreatorShopDeconstructAdminOnly = BindConfig(
                MainSectionName,
                "CreatorShopDeconstructAdminOnly",
                true,
                "Set to true to allow admin players to deconstruct any CreatorShop pieces built by players." +
                " Intended to prevent griefing via placement of indestructible objects.",
                AcceptableBoolValuesList
            );

            ForceAllPrefabs = BindConfig(
                MainSectionName,
                "ForceAllPrefabs",
                false,
                "If enabled, adds all prefabs from the configuration file to the hammer based on their current configuration (requirements).",
                AcceptableBoolValuesList
            );

            VerboseMode = BindConfig(
                MainSectionName,
                "VerboseMode",
                false,
                "If enable, print debug informations in console.",
                AcceptableBoolValuesList
            );

            CreatorShopAdminOnly.SettingChanged += PieceSettingChanged;
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
                "If true then add the prefab as a buildable piece. Note: this setting is ignored if ForceAllPrefabs is true.",
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

            // if the prefab is not already included in the list of prefabs that need a 
            // collision patch then add a config option to enable the placement collision patch.
            if (!PiecePlacement.NeedsCollisionPatchForGhost(prefab.name))
            {
                pieceConfigEntries.placementPatch = BindConfig(
                    sectionName,
                    "PlacementPatch",
                    false,
                    "Set to true to enable collision patching during placement of the piece.\n" +
                    "Reccomended to try this if the piece is not appearing when you go to place it.\n\n" +
                    " If enabling the placement patch via this setting fixes the issue please open an issue on Github" +
                    " letting me know so I can make sure the collision patch is always applied to this piece.",
                    AcceptableBoolValuesList
                );
                pieceConfigEntries.placementPatch.SettingChanged += PlacementSettingChanged;
                defaultPieceDB.placementPatch = pieceConfigEntries.placementPatch.Value;

                if (defaultPieceDB.placementPatch)
                {
                    // add prefab to list of prefabs needing a collision patch if setting is true
                    PiecePlacement._NeedsCollisionPatchForGhost.Add(prefab.name);
                }
            }

            // keep a reference to the config entries
            // to make sure the events fire as intended
            PieceConfigEntriesMap[prefab.name] = pieceConfigEntries;
            return defaultPieceDB;
        }

        internal static void SetupWatcher()
        {
            FileSystemWatcher watcher = new(Paths.ConfigPath, ConfigFileName);
            watcher.Changed += ReadConfigValues;
            watcher.Created += ReadConfigValues;
            watcher.Renamed += ReadConfigValues;
            watcher.IncludeSubdirectories = true;
            watcher.SynchronizingObject = ThreadingHelper.SynchronizingObject;
            watcher.EnableRaisingEvents = true;
        }

        private static void ReadConfigValues(object sender, FileSystemEventArgs e)
        {
            if (!File.Exists(ConfigFileFullPath)) return;
            try
            {
                Log.LogInfo("ReadConfigValues called");
                configFile.Reload();
            }
            catch
            {
                Log.LogError($"There was an issue loading your {ConfigFileName}");
                Log.LogError("Please check your config entries for spelling and format!");
            }
        }

        /// <summary>
        ///     Convert requirements string from cfg file to Piece.Requirement Array
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        internal static Piece.Requirement[] CreateRequirementsArray(string data)
        {
            if (string.IsNullOrEmpty(data.Trim())) return new Piece.Requirement[0];

            // If not empty
            List<Piece.Requirement> requirements = new();

            foreach (var entry in data.Split(';'))
            {
                string[] values = entry.Split(',');
                var itm = ObjectDB.instance.GetItemPrefab(values[0].Trim())?.GetComponent<ItemDrop>();
                if (itm == null)
                {
                    Log.LogWarning($"Unable to find requirement ID: {values[0].Trim()}");
                    continue;
                }
                Piece.Requirement req = new()
                {
                    m_resItem = itm,
                    m_amount = int.Parse(values[1].Trim()),
                    m_recover = true
                };
                requirements.Add(req);
            }
            return requirements.ToArray();
        }
    }
}


// Jotunn based code

/// <summary>
///     Create array of Requirement Configs for use with Jotunn
/// </summary>
/// <param name="data"></param>
/// <returns></returns>
//internal static RequirementConfig[] CreateRequirementConfigsArray(string data)
//{
//    if (string.IsNullOrEmpty(data.Trim())) return Array.Empty<RequirementConfig>();

//    // If not empty
//    List<RequirementConfig> requirements = new();

//    foreach (var entry in data.Split(';'))
//    {
//        string[] values = entry.Split(',');
//        RequirementConfig reqConfig = new()
//        {
//            Item = values[0].Trim(),
//            Amount = int.Parse(values[1].Trim()),
//            Recover = true
//        };
//        requirements.Add(reqConfig);
//    }
//    return requirements.ToArray();
//}