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

        public void Awake()
        {
            Log.Init(Logger);
            ConfigManager.Init(Config);
            ConfigManager.SetUpConfig();

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
    }

    /// <summary>
    /// Helper class for properly logging from static contexts.
    /// </summary>
    internal static class Log
    {
        internal static ManualLogSource _logSource;

        internal static void Init(ManualLogSource logSource)
        {
            _logSource = logSource;
        }

        internal static void LogDebug(object data) => _logSource.LogDebug(data);

        internal static void LogError(object data) => _logSource.LogError(data);

        internal static void LogFatal(object data) => _logSource.LogFatal(data);

        internal static void LogInfo(object data) => _logSource.LogInfo(data);

        internal static void LogMessage(object data) => _logSource.LogMessage(data);

        internal static void LogWarning(object data) => _logSource.LogWarning(data);

        internal static void LogPrefab(GameObject prefab, bool includeChildren = false)
        {
            LogInfo("***** " + prefab.name + " *****");
            foreach (Component compo in prefab.GetComponents<Component>())
            {
                LogInfo("-" + compo.GetType().Name);
                PropertyInfo[] properties = prefab.GetType().GetProperties();
                foreach (PropertyInfo property in properties)
                {
                    LogInfo("  -" + property.Name + " = " + property.GetValue(prefab));
                }
            }

            if (!includeChildren) { return; }

            LogInfo("***** " + prefab.name + " (childs) *****");
            foreach (Transform child in prefab.transform)
            {
                LogInfo("-" + child.gameObject.name);

                foreach (Component component in child.gameObject.GetComponents<Component>())
                {
                    LogInfo("  -" + component.GetType().Name);
                    PropertyInfo[] properties = component.GetType().GetProperties();
                    foreach (PropertyInfo property in properties)
                    {
                        LogInfo("    -" + property.Name + " = " + property.GetValue(component));
                    }
                }
            }
        }

        internal static void LogPiece(Piece piece)
        {
            LogInfo("***** " + piece.name + " *****");

            PropertyInfo[] properties = piece.GetType().GetProperties();
            foreach (PropertyInfo property in properties)
            {
                LogInfo("  -" + property.Name + " = " + property.GetValue(piece));
            }

            FieldInfo[] fields = piece.GetType().GetFields();
            foreach (var field in fields)
            {
                LogInfo("  -" + field.Name + " = " + field.GetValue(piece));
            }
        }

        internal static void LogComponent(Component comp)
        {
            LogInfo("***** " + comp.name + " *****");

            PropertyInfo[] properties = comp.GetType().GetProperties(ReflectionUtils.AllBindings);
            foreach (PropertyInfo property in properties)
            {
                LogInfo("  -" + property.Name + " = " + property.GetValue(comp));
            }

            FieldInfo[] fields = comp.GetType().GetFields(ReflectionUtils.AllBindings);
            foreach (var field in fields)
            {
                LogInfo("  -" + field.Name + " = " + field.GetValue(comp));
            }
        }
    }
}