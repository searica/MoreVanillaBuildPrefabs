// Ignore Spelling: MVBP Wackys

using BepInEx.Bootstrap;
using HarmonyLib;
using MVBP.Helpers;
using System;
using System.Reflection;

namespace MVBP
{
    internal class ModCompat
    {
        public const string ExtraSnapsGUID = "Searica.Valheim.ExtraSnapPointsMadeEasy";
        public const string PlanBuildGUID = "marcopogo.PlanBuild";
        public const string WackysDBGUID = "WackyMole.WackysDatabase";

        private static bool? _PlanBuildInstalled;
        private static bool? _ExtraSnapsInstalled;
        private static bool? _WackysDBInstalled;

        private static Type PlanDBType;

        internal static bool IsWackysDBInstalled()
        {
            _WackysDBInstalled ??= Chainloader.PluginInfos.ContainsKey(WackysDBGUID);
            return _WackysDBInstalled.Value;
        }

        internal static bool IsPlanBuildInstalled()
        {
            _PlanBuildInstalled ??= Chainloader.PluginInfos.ContainsKey(PlanBuildGUID);
            return _PlanBuildInstalled.Value;
        }

        internal static bool IsExtraSnapsInstalled()
        {
            _ExtraSnapsInstalled ??= Chainloader.PluginInfos.ContainsKey(ExtraSnapsGUID);
            return _ExtraSnapsInstalled.Value;
        }

        /// <summary>
        ///     Triggers a re-initialization of ExtraSnapPointsMadeEasy if it is installed.
        /// </summary>
        /// <returns></returns>
        internal static bool UpdateExtraSnaps()
        {
            if (!IsExtraSnapsInstalled()) return false;

            var plugin = Chainloader.PluginInfos[ExtraSnapsGUID].Instance;
            if (plugin == null) return false;

            try
            {
                MethodInfo method = ReflectionUtils.GetMethod(
                                plugin.GetType(),
                                "ReInitExtraSnapPoints",
                                Type.EmptyTypes
                            );

                // Invoke if not null
                method?.Invoke(plugin, Array.Empty<object>());
            }
            catch
            {
                Log.LogWarning("Could not re-init ExtraSnapPointsMadeEasy");
                return false;
            }
            return true;
        }

        /// <summary>
        ///     Triggers rescanning of piece tables by PlanBuild if it is installed.
        /// </summary>
        /// <returns></returns>
        internal static bool UpdatePlanBuild()
        {
            // trigger rescanning of piece tables via reflection
            if (TryGetPlanDBType(out Type planDBType))
            {
                Log.LogInfo("PlanBuild is installed", LogLevel.Medium);

                try
                {
                    MethodInfo planDBGetter = AccessTools.PropertyGetter(planDBType, "Instance");
                    var planDBInstance = planDBGetter.Invoke(null, Array.Empty<object>());
                    MethodInfo method = AccessTools.Method("PlanBuild.Plans.PlanDB:ScanPieceTables", Type.EmptyTypes);
                    Log.LogInfo("Triggering PlanBuild ScanPieceTables", LogLevel.Medium);
                    // Invoke if not null
                    method?.Invoke(planDBInstance, Array.Empty<object>());
                }
                catch
                {
                    Log.LogWarning("Failed to trigger PlanBuild ScanPieceTables");
                    return false;
                }
            }

            return false;
        }

        private static bool TryGetPlanDBType(out Type planDBType)
        {
            planDBType = null;
            if (!IsPlanBuildInstalled()) { return false; }

            if (PlanDBType != null) // return cached value
            {
                planDBType = PlanDBType;
                return true;
            }

            var plugin = Chainloader.PluginInfos[PlanBuildGUID].Instance;
            if (plugin == null) return false;

            var assembly = Assembly.GetAssembly(plugin.GetType());
            foreach (var type in AccessTools.GetTypesFromAssembly(assembly))
            {
                if (type.FullName == "PlanBuild.Plans.PlanDB")
                {
                    PlanDBType = type;
                    planDBType = type;
                    return true;
                }
            }

            return false;
        }

        internal static bool IsWackyDBClone(string name)
        {
            if (!IsWackysDBInstalled()) return false;

            var plugin = Chainloader.PluginInfos[WackysDBGUID].Instance;
            if (plugin == null) return false;

            // get clone to source prefab map via reflection
            // check if the source prefab is one that MVBP patched
            return false;
        }
    }
}