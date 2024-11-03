// Ignore Spelling: MVBP

using BepInEx;
using BepInEx.Bootstrap;
using BepInEx.Configuration;
using Jotunn.Configs;
using MVBP.Extensions;
using System;
using System.Collections.Generic;
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

        private static DateTime lastRead = DateTime.MinValue;

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

        private static readonly ConfigurationManagerAttributes AdminConfig = new() { IsAdminOnly = true };
        private static readonly ConfigurationManagerAttributes ClientConfig = new() { IsAdminOnly = false };
   
        private const char ZWS = '\u200B';


        internal static void Init(string GUID, ConfigFile config, bool saveOnConfigSet = false)
        {
            configFile = config;
            configFile.SaveOnConfigSet = saveOnConfigSet;
            ConfigFileName = GUID + ".cfg";
            ConfigFileFullPath = Path.Combine(Paths.ConfigPath, ConfigFileName);
        }


        /// <summary>
        ///     Sets SaveOnConfigSet to false and returns
        ///     the Value prior to calling this method.
        /// </summary>
        /// <returns></returns>
        internal static bool DisableSaveOnConfigSet()
        {
            var val = configFile.SaveOnConfigSet;
            configFile.SaveOnConfigSet = false;
            return val;
        }

        /// <summary>
        ///     Set the Value for the SaveOnConfigSet field.
        /// </summary>
        /// <param name="value"></param>
        internal static void SaveOnConfigSet(bool value)
        {
            configFile.SaveOnConfigSet = value;
        }

        /// <summary>
        ///     Save config file to disk.
        /// </summary>
        internal static void Save()
        {
            configFile.Save();
        }

        internal static ConfigEntry<T> BindConfig<T>(
            string section,
            string name,
            T value,
            string description,
            AcceptableValueBase acceptVals = null,
            bool synced = true,
            bool drawer = false,
            int order = 0
        ) {
            string extendedDescription = GetExtendedDescription(description, synced);

            var configAttr = new ConfigurationManagerAttributes() { Order = order };
            if (drawer) {
                configAttr.CustomDrawer = SharedDrawers.DrawReqConfigTable();
            }

            ConfigEntry<T> configEntry = configFile.Bind(
                section,
                name,
                value,
                new ConfigDescription(
                    extendedDescription,
                    acceptVals,
                    synced ? AdminConfig : ClientConfig,
                    configAttr
                )
            );
            return configEntry;
        }

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

        internal static void SetupWatcher() {
            var watcher = new FileSystemWatcher(Paths.ConfigPath, ConfigFileName);
            watcher.Changed += ReloadConfigFile;
            watcher.Created += ReloadConfigFile;
            watcher.Renamed += ReloadConfigFile;
            watcher.IncludeSubdirectories = true;
            watcher.SynchronizingObject = ThreadingHelper.SynchronizingObject;
            watcher.EnableRaisingEvents = true;
        }

        private static void ReloadConfigFile(object sender, FileSystemEventArgs eventArgs) {
            if (!File.Exists(ConfigFileFullPath)) {
                return;
            }

            try {
                var lastWriteTime = File.GetLastWriteTime(eventArgs.FullPath);
                if (lastRead != lastWriteTime) {
                    Log.LogInfo("Reloading config file");
                    var saveOnConfigSet = DisableSaveOnConfigSet(); // turn off saving on config entry set
                    configFile.Reload();
                    SaveOnConfigSet(saveOnConfigSet); // reset config saving state

                    InvokeOnConfigFileReloaded(); // fire event
                    lastRead = lastWriteTime;
                }
            }
            catch {
                Log.LogError($"There was an issue loading your {ConfigFileName}");
                Log.LogError("Please check your config entries for spelling and format!");
            }
        }

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
            PropertyInfo propInfo = ConfigurationManager.GetType().GetProperty("DisplayingWindow");
            bool ConfigurationManagerWindowShown = (bool)propInfo.GetValue(ConfigurationManager, null);

            if (!ConfigurationManagerWindowShown) {
                InvokeOnConfigWindowClosed();
            }
        }
    }

    internal class AcceptableValueConfigNote : AcceptableValueBase
    {
        public virtual string Note { get; }

        public AcceptableValueConfigNote(string note) : base(typeof(string))
        {
            if (string.IsNullOrEmpty(note))
            {
                throw new ArgumentException("A string with atleast 1 character is needed", "Note");
            }
            this.Note = note;
        }

        // passthrough overrides
        public override object Clamp(object value) { return value; }
        public override bool IsValid(object value) { return !string.IsNullOrEmpty(value as string); }

        public override string ToDescriptionString()
        {
            return "# Note: " + Note;
        }
    }

    internal static class SharedDrawers
    {
        private static BaseUnityPlugin configManager = null;
        private static BaseUnityPlugin GetConfigManager()
        {
            if (SharedDrawers.configManager == null)
            {
                PluginInfo configManagerInfo;
                if (Chainloader.PluginInfos.TryGetValue("com.bepis.bepinex.configurationmanager", out configManagerInfo) && configManagerInfo.Instance)
                {
                    SharedDrawers.configManager = configManagerInfo.Instance;
                }
            }

            return SharedDrawers.configManager;
        }

        public static int GetRightColumnWidth()
        {
            int result = 130;
            BaseUnityPlugin configManager = GetConfigManager();
            if (configManager != null)
            {
                PropertyInfo pi = configManager?.GetType().GetProperty("RightColumnWidth", BindingFlags.Instance | BindingFlags.NonPublic);
                if (pi != null)
                {
                    result = (int)pi.GetValue(configManager);
                }
            }

            return result;
        }

        public static void ReloadConfigDisplay()
        {
            BaseUnityPlugin configManager = GetConfigManager();
            if (configManager != null && configManager.GetType()?.GetProperty("DisplayingWindow")?.GetValue(configManager) is true)
            {
                configManager.GetType().GetMethod("BuildSettingList").Invoke(configManager, Array.Empty<object>());
            }
        }

        public static Action<ConfigEntryBase> DrawReqConfigTable(bool hasUpgrades = false)
        {
            return cfg =>
            {
                List<RequirementConfig> newReqs = new List<RequirementConfig>();
                bool wasUpdated = false;

                int RightColumnWidth = GetRightColumnWidth();

                GUILayout.BeginVertical();

                List<RequirementConfig> reqs = RequirementsEntry.Deserialize((string)cfg.BoxedValue);

                foreach (var req in reqs)
                {
                    GUILayout.BeginHorizontal();

                    string newItem = GUILayout.TextField(req.Item, new GUIStyle(GUI.skin.textField) { fixedWidth = RightColumnWidth - 33 - (hasUpgrades ? 37 : 0) - 21 - 21 - 12 });
                    string prefabName = string.IsNullOrEmpty(newItem) ? req.Item : newItem;
                    wasUpdated = wasUpdated || prefabName != req.Item;


                    int amount = req.Amount;
                    if (int.TryParse(GUILayout.TextField(amount.ToString(), new GUIStyle(GUI.skin.textField) { fixedWidth = 33 }), out int newAmount) && newAmount != amount)
                    {
                        amount = newAmount;
                        wasUpdated = true;
                    }

                    if (GUILayout.Button("x", new GUIStyle(GUI.skin.button) { fixedWidth = 21 }))
                    {
                        wasUpdated = true;
                    }
                    else
                    {
                        newReqs.Add(new RequirementConfig { Item = prefabName, Amount = amount });
                    }

                    if (GUILayout.Button("+", new GUIStyle(GUI.skin.button) { fixedWidth = 21 }))
                    {
                        wasUpdated = true;
                        newReqs.Add(new RequirementConfig { Item = "<Prefab Name>", Amount = 1 });
                    }

                    GUILayout.EndHorizontal();
                }

                GUILayout.EndVertical();

                if (wasUpdated)
                {
                    cfg.BoxedValue = RequirementsEntry.Serialize(newReqs.ToArray());
                }
            };
        }
    }
}