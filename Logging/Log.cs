using BepInEx.Logging;
using System.Reflection;
using UnityEngine;

namespace MoreVanillaBuildPrefabs.Logging
{
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

        internal static void LogPrefab(GameObject prefab)
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
    }
}
