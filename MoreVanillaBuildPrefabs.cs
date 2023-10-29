// Ignore Spelling: Plugin

using BepInEx;
using HarmonyLib;
using Jotunn.Managers;
using Jotunn.Utils;
using MoreVanillaBuildPrefabs.Configs;
using MoreVanillaBuildPrefabs.Logging;
using System.Reflection;

namespace MoreVanillaBuildPrefabs
{
    [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
    [BepInDependency(Jotunn.Main.ModGuid, Jotunn.Main.Version)]
    [BepInDependency(ModCompat.ExtraSnapPointsMadeEasyGUID, BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency(ModCompat.PlanBuildGUID, BepInDependency.DependencyFlags.SoftDependency)]
    [NetworkCompatibility(CompatibilityLevel.VersionCheckOnly, VersionStrictness.Patch)]
    public class MoreVanillaBuildPrefabs : BaseUnityPlugin
    {
        public const string PluginName = "MoreVanillaBuildPrefabs";
        internal const string Author = "Searica";
        public const string PluginGUID = $"{Author}.Valheim.{PluginName}";
        public const string PluginVersion = "0.4.7";

        public void Awake()
        {
            Log.Init(Logger);

            PluginConfig.Init(Config);
            PluginConfig.SetUpConfig();

            Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), harmonyInstanceId: PluginGUID);

            Game.isModded = true;

            PluginConfig.SetupWatcher();
            PluginConfig.CheckForConfigManager();

            SynchronizationManager.OnConfigurationSynchronized += (obj, attr) =>
            {
                // Re-initialize after syncing data with server
                PluginConfig.ReInitPlugin("Config settings synced with server, re-initializing");
            };
        }

        public void OnDestroy()
        {
            PluginConfig.Save();
        }
    }
}