using BepInEx.Configuration;
using Jotunn.Configs;
using UnityEngine;


namespace MoreVanillaBuildPrefabs
{
    internal class PluginConfig
    {
        private static ConfigFile configFile;

        private static readonly string MainSectionName = "_Global";
        public static ConfigEntry<bool> IsModEnabled { get; private set; }
        private static ConfigEntry<bool> forceAllPrefabs;
        private static ConfigEntry<bool> verboseMode;
        private static readonly AcceptableValueList<bool> AcceptableToggleValuesList = new(new bool[]{false, true});

        public static void Init(ConfigFile config)
        {
            configFile = config;

            IsModEnabled = configFile.Bind(
                MainSectionName,
                "EnableMod",
                true,
                new ConfigDescription(
                    "Globally enable or disable this mod (restart required).", 
                    AcceptableToggleValuesList
                )
             );

            forceAllPrefabs = configFile.Bind(
                MainSectionName,
                "ForceAllPrefabs",
                false,
                new ConfigDescription(
                    "If enabled, adds all prefabs from the configuration file to the hammer based on their current configuration (requirements).", 
                    AcceptableToggleValuesList
                )
            );

            verboseMode = configFile.Bind(
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
            return verboseMode.Value;
        }

        public static bool IsForceAllPrefabs()
        {
            return forceAllPrefabs.Value;
        }

        public static PrefabDefaults.PrefabConfig LoadPrefabConfig(GameObject prefab)
        {
            string sectionName = prefab.name;
         
            // get predefined configs or generic settings if no predefined config
            PrefabDefaults.PrefabConfig default_config = PrefabDefaults.GetDefaultPrefabConfigValues(prefab.name);
            default_config.Enabled = configFile.Bind(
                sectionName, 
                "Enabled", 
                default_config.Enabled,
                new ConfigDescription(
                    "If true then add the prefab as a buildable piece. Note: this setting is ignored if ForceAllPrefabs is true.",
                    AcceptableToggleValuesList
                )
            ).Value;
            default_config.AllowedInDungeons = configFile.Bind(
                sectionName, 
                "AllowedInDungeons", 
                default_config.AllowedInDungeons,
                new ConfigDescription(
                    "If true then this prefab can be built inside dungeon zones.", 
                    AcceptableToggleValuesList
                )
            ).Value;
            default_config.Category = configFile.Bind(
                sectionName, 
                "Category", 
                default_config.Category,
                new ConfigDescription(
                    "A string defining the tab the prefab shows up on in the hammer build table.", 
                    Plugin.HammerCategoryNames.GetAcceptableValueList()
                )
            ).Value;
#if DEBUG
            Log.LogInfo($"Default CraftingStation pre-bind: {default_config.CraftingStation}");
#endif
            default_config.CraftingStation = configFile.Bind(
                sectionName, 
                "CraftingStation", 
                default_config.CraftingStation,
                new ConfigDescription(
                    "A string defining the crafting station required to built the prefab.", 
                    CraftingStations.GetAcceptableValueList()
                )
            ).Value;
#if DEBUG
            Log.LogInfo($"Default CraftingStation post-bind: {default_config.CraftingStation}");
#endif
            default_config.Requirements = configFile.Bind(
                sectionName, 
                "Requirements", 
                default_config.Requirements
            ).Value;

            // if the prefab is not already included in the list of prefabs that need a 
            // collision patch then add a config option to enable the placement collision patch.
            if (!PrefabDefaults.NeedsCollisionPatchForGhost.Contains(prefab.name))
            {
                default_config.PlacementPatch = configFile.Bind(
                    sectionName, 
                    "PlacementPatch", 
                    false, 
                    new ConfigDescription(
                        "Set to true to enable collision patching during placement of the piece.\n" +
                        "Reccomended to try this if the piece is not appearing when you go to place it." +
                        " Also if this setting this to true fixes the issue please open an issue on Github " +
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
