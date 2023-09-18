using BepInEx.Configuration;
using UnityEngine;


namespace MoreVanillaBuildPrefabs
{
    internal class PluginConfig
    {
        private static ConfigFile configFile;

        private static string MainSectionName = "_Global";
        public static ConfigEntry<bool> IsModEnabled { get; private set; }
        private static ConfigEntry<bool> forceAllPrefabs;
        private static ConfigEntry<bool> verboseMode;

        public static void Init(ConfigFile config)
        {
            configFile = config;

            IsModEnabled = configFile.Bind(
                MainSectionName,
                "EnableMod",
                true,
                "Globally enable or disable this mod (restart required)."
             );

            forceAllPrefabs = configFile.Bind(
                MainSectionName,
                "ForceAllPrefabs",
                false,
                new ConfigDescription("If enabled, adds all prefabs from the configuration file to the hammer based on their current configuration (requirements).")
            );

            verboseMode = configFile.Bind(
                MainSectionName,
                "VerboseMode",
                false,
                new ConfigDescription("If enable, print debug informations in console.")
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

        public static PrefabConfigs.PrefabConfig LoadPrefabConfig(GameObject prefab)
        {
            string sectionName = prefab.name;
         
            // get predefined configs or generic settings if no predefined config
            PrefabConfigs.PrefabConfig default_config = PrefabConfigs.GetDefaultPrefabConfigValues(prefab.name);
            default_config.Enabled = configFile.Bind(sectionName, "Enabled", default_config.Enabled).Value;
            default_config.AllowedInDungeons = configFile.Bind(sectionName, "AllowedInDungeons", default_config.AllowedInDungeons).Value;
            default_config.Category = configFile.Bind(sectionName, "Category", default_config.Category).Value;
            default_config.CraftingStation = configFile.Bind(sectionName, "CraftingStation", default_config.CraftingStation).Value;
            default_config.Requirements = configFile.Bind(sectionName, "Requirements", default_config.Requirements).Value;

            return default_config;
        }
    }
}
