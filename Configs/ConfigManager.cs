// Ignore Spelling: MVBP

using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Configuration;
using MVBP.Extensions;
using System;
using System.IO;
using System.Reflection;
using UnityEngine;
using UnityEngine.Rendering;

namespace MVBP.Configs {
    internal static class ConfigManager {
        private static string ConfigFileName;
        private static string ConfigFileFullPath;

        private static ConfigFile configFile;
        private static BaseUnityPlugin ConfigurationManager;
        private const string ConfigManagerGUID = "com.bepis.bepinex.configurationmanager";

        private static FileSystemWatcher watcher;
        #region Events

        /// <summary>
        ///     Event triggered after a the in-game configuration manager is closed.
        /// </summary>
        internal static event Action OnConfigWindowClosed;

        /// <summary>
        ///     Safely invoke the <see cref="OnConfigWindowClosed"/> event
        /// </summary>
        private static void InvokeOnConfigWindowClosed() {
            OnConfigWindowClosed?.SafeInvoke();
        }

        /// <summary>
        ///     Event triggered after the file watcher reloads the configuration file.
        /// </summary>
        internal static event Action OnConfigFileReloaded;

        /// <summary>
        ///     Safely invoke the <see cref="OnConfigFileReloaded"/> event
        /// </summary>
        private static void InvokeOnConfigFileReloaded() {
            OnConfigFileReloaded?.SafeInvoke();
        }

        #endregion Events

        #region BindConfig

        internal static ConfigEntry<T> BindConfig<T>(
            string section,
            string name,
            T value,
            string description,
            AcceptableValueBase acceptVals = null,
            bool synced = true
        ) {
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

        private static readonly ConfigurationManagerAttributes AdminConfig = new() { IsAdminOnly = true };
        private static readonly ConfigurationManagerAttributes ClientConfig = new() { IsAdminOnly = false };
        private const char ZWS = '\u200B';

        /// <summary>
        ///     Prepends Zero-Width-Space to set ordering of configuration sections
        /// </summary>
        /// <param name="sectionName">Section name</param>
        /// <param name="priority">Number of ZWS chars to prepend</param>
        /// <returns></returns>
        internal static string SetStringPriority(string sectionName, int priority) {
            if (priority == 0) { return sectionName; }
            return new string(ZWS, priority) + sectionName;
        }

        internal static string GetExtendedDescription(string description, bool synchronizedSetting) {
            return description + (synchronizedSetting ? " [Synced with Server]" : " [Not Synced with Server]");
        }

        #endregion BindConfig

        internal static void Init(string GUID, ConfigFile config, bool saveOnConfigSet = false) {
            configFile = config;
            configFile.SaveOnConfigSet = saveOnConfigSet;
            ConfigFileName = GUID + ".cfg";
            ConfigFileFullPath = Path.Combine(Paths.ConfigPath, ConfigFileName);
        }

        #region Saving

        /// <summary>
        ///     Sets SaveOnConfigSet to false and returns
        ///     the Value prior to calling this method.
        /// </summary>
        /// <returns></returns>
        internal static bool DisableSaveOnConfigSet() {
            var val = configFile.SaveOnConfigSet;
            configFile.SaveOnConfigSet = false;
            return val;
        }

        /// <summary>
        ///     Set the Value for the SaveOnConfigSet field.
        /// </summary>
        /// <param name="value"></param>
        internal static void SaveOnConfigSet(bool value) {
            configFile.SaveOnConfigSet = value;
        }

        /// <summary>
        ///     Save config file to disk.
        /// </summary>
        internal static void Save() {
            configFile.Save();
        }

        #endregion Saving

        #region FileWatcher

        internal static void SetupWatcher() {
            watcher = new FileSystemWatcher(Paths.ConfigPath, ConfigFileName);
            watcher.Changed += ReloadConfigFile;
            watcher.Created += ReloadConfigFile;
            watcher.Renamed += ReloadConfigFile;
            watcher.IncludeSubdirectories = true;
            watcher.SynchronizingObject = ThreadingHelper.SynchronizingObject;
            watcher.EnableRaisingEvents = true;
        }

        private static void ReloadConfigFile(object sender, FileSystemEventArgs e) {
            if (!File.Exists(ConfigFileFullPath)) {
                return;
            }

            try {
                Log.LogInfo("Reloading config file");
                var saveOnConfigSet = DisableSaveOnConfigSet(); // turn off saving on config entry set
                configFile.Reload();
                SaveOnConfigSet(saveOnConfigSet); // reset config saving state

                InvokeOnConfigFileReloaded(); // fire event

            }
            catch {
                Log.LogError($"There was an issue loading your {ConfigFileName}");
                Log.LogError("Please check your config entries for spelling and format!");
            }
        }

        #endregion FileWatcher

        #region ConfigWindow

        /// <summary>
        ///     Checks for in-game configuration manager and
        ///     sets Up OnConfigWindowClosed event if it is present
        /// </summary>
        internal static void CheckForConfigManager() {
            if (SystemInfo.graphicsDeviceType == GraphicsDeviceType.Null) {
                return;
            }

            if (Chainloader.PluginInfos.TryGetValue(ConfigManagerGUID, out PluginInfo configManagerInfo) && configManagerInfo.Instance) {
                ConfigurationManager = configManagerInfo.Instance;
                Log.LogDebug("Configuration manager found, hooking DisplayingWindowChanged");

                EventInfo eventinfo = ConfigurationManager.GetType().GetEvent("DisplayingWindowChanged");

                if (eventinfo != null) {
                    Action<object, object> local = new(OnConfigManagerDisplayingWindowChanged);
                    Delegate converted = Delegate.CreateDelegate(
                        eventinfo.EventHandlerType,
                        local.Target,
                        local.Method
                    );
                    eventinfo.AddEventHandler(ConfigurationManager, converted);
                }
            }
        }

        private static void OnConfigManagerDisplayingWindowChanged(object sender, object e) {
            PropertyInfo pi = ConfigurationManager.GetType().GetProperty("DisplayingWindow");
            bool ConfigurationManagerWindowShown = (bool)pi.GetValue(ConfigurationManager, null);

            if (!ConfigurationManagerWindowShown) {
                InvokeOnConfigWindowClosed();
            }
        }

        #endregion ConfigWindow
    }
}