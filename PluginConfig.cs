using BepInEx.Configuration;
using Jotunn.Configs;
using UnityEngine;
using ServerSync;


namespace MoreVanillaBuildPrefabs
{
    internal class PluginConfig
    {
        private static ConfigFile configFile;

        private static readonly ConfigSync configSync = new(Plugin.PluginGuid)
        {
            DisplayName = Plugin.PluginName,
            CurrentVersion = Plugin.PluginVersion,
            MinimumRequiredVersion = Plugin.PluginVersion
        };

        internal static ConfigEntry<T> BindConfig<T>(string group, string name, T value, ConfigDescription description, bool synchronizedSetting = true)
        {
            ConfigEntry<T> configEntry = configFile.Bind(group, name, value, description);

            SyncedConfigEntry<T> syncedConfigEntry = configSync.AddConfigEntry(configEntry);
            syncedConfigEntry.SynchronizedConfig = synchronizedSetting;

            return configEntry;
        }

        internal static ConfigEntry<bool> BindLockingConfig(string group, string name, bool value, ConfigDescription description, bool synchronizedSetting = true)
        {
            ConfigEntry<bool> configEntry = configFile.Bind(group, name, value, description);

            SyncedConfigEntry<bool> syncedConfigEntry = configSync.AddLockingConfigEntry(configEntry);
            syncedConfigEntry.SynchronizedConfig = synchronizedSetting;

            return configEntry;
        }

        internal static ConfigEntry<T> BindConfig<T>(string group, string name, T value, string description, bool synchronizedSetting = true) => BindConfig(group, name, value, new ConfigDescription(description), synchronizedSetting);


        private static readonly string MainSectionName = "\u200BGlobal";
        public static ConfigEntry<bool> IsModEnabled { get; private set; }
        public static ConfigEntry<bool> LockConfiguration { get; private set; }
        internal static ConfigEntry<bool> AdminDeconstructCreatorShop;
        private static ConfigEntry<bool> ForceAllPrefabs;
        private static ConfigEntry<bool> VerboseMode;
        private static readonly AcceptableValueList<bool> AcceptableToggleValuesList = new(new bool[]{false, true});

        public static void Init(ConfigFile config)
        {
            configFile = config;
        }

        public static void SetUpConfig()
        {
            IsModEnabled = BindConfig(
                MainSectionName,
                "EnableMod",
                true,
                new ConfigDescription(
                    "Globally enable or disable this mod (restart required).",
                    AcceptableToggleValuesList
                )
             );

            LockConfiguration = BindLockingConfig(
                MainSectionName,
                "LockConfiguration",
                true,
                new ConfigDescription(
                    "If true, the configuration is locked and can be changed by server admins only.",
                    AcceptableToggleValuesList
                )
            );

            AdminDeconstructCreatorShop = BindConfig(
                MainSectionName,
                "AdminDeconstructCreatorShop",
                true,
                new ConfigDescription(
                    "Set to true to allow admin players to deconstruct any CreatorShop pieces built by players." +
                    " Intended to prevent griefing via placement of indestructible objects.",
                    AcceptableToggleValuesList
                )
            );

            ForceAllPrefabs = BindConfig(
                MainSectionName,
                "ForceAllPrefabs",
                false,
                new ConfigDescription(
                    "If enabled, adds all prefabs from the configuration file to the hammer based on their current configuration (requirements).",
                    AcceptableToggleValuesList
                )
            );

            VerboseMode = BindConfig(
                MainSectionName,
                "VerboseMode",
                false,
                new ConfigDescription(
                    "If enable, print debug informations in console.",
                    AcceptableToggleValuesList
                )
            );
        }

        public static void Save()
        {
            configFile.Save();
        }

        public static bool IsVerbose()
        {
            return VerboseMode.Value;
        }

        public static bool IsForceAllPrefabs()
        {
            return ForceAllPrefabs.Value;
        }

        public static PrefabDefaults.PrefabConfig LoadPrefabConfig(GameObject prefab)
        {
            string sectionName = prefab.name;
         
            // get predefined configs or generic settings if no predefined config
            PrefabDefaults.PrefabConfig default_config = PrefabDefaults.GetDefaultPrefabConfigValues(prefab.name);
            default_config.Enabled = BindConfig(
                sectionName, 
                "Enabled", 
                default_config.Enabled,
                new ConfigDescription(
                    "If true then add the prefab as a buildable piece. Note: this setting is ignored if ForceAllPrefabs is true.",
                    AcceptableToggleValuesList
                )
            ).Value;
            default_config.AllowedInDungeons = BindConfig(
                sectionName, 
                "AllowedInDungeons", 
                default_config.AllowedInDungeons,
                new ConfigDescription(
                    "If true then this prefab can be built inside dungeon zones.", 
                    AcceptableToggleValuesList
                )
            ).Value;
            default_config.Category = BindConfig(
                sectionName, 
                "Category", 
                default_config.Category,
                new ConfigDescription(
                    "A string defining the tab the prefab shows up on in the hammer build table.", 
                    Plugin.HammerCategoryNames.GetAcceptableValueList()
                )
            ).Value;
            default_config.CraftingStation = BindConfig(
                sectionName, 
                "CraftingStation", 
                default_config.CraftingStation,
                new ConfigDescription(
                    "A string defining the crafting station required to built the prefab.", 
                    CraftingStations.GetAcceptableValueList()
                )
            ).Value;
            default_config.Requirements = BindConfig(
                sectionName, 
                "Requirements", 
                default_config.Requirements,
                "Resources required to build the prefab. Formatted as: itemID,amount;itemID,amount where itemID is the in-game identifier for the resource and amount is an integer. "
            ).Value;

            // if the prefab is not already included in the list of prefabs that need a 
            // collision patch then add a config option to enable the placement collision patch.
            if (!PrefabDefaults.NeedsCollisionPatchForGhost.Contains(prefab.name))
            {
                default_config.PlacementPatch = BindConfig(
                    sectionName, 
                    "PlacementPatch", 
                    false, 
                    new ConfigDescription(
                        "Set to true to enable collision patching during placement of the piece.\n" +
                        "Reccomended to try this if the piece is not appearing when you go to place it.\n\n" +
                        " If enabling the placement patch via this setting fixes the issue please open an issue on Github" +
                        " letting me know so I can make sure the collision patch is always applied to this piece.", 
                        AcceptableToggleValuesList
                    )
                ).Value;
                if (default_config.PlacementPatch)
                {
                    // add prefab to list of prefabs needing a collision patch if setting is true
                    PrefabDefaults.NeedsCollisionPatchForGhost.Add(prefab.name);
                }
            }
            return default_config;
        }
    }
}
