// Ignore Spelling: MVBP Wackys

using BepInEx.Bootstrap;
using HarmonyLib;
using MVBP.Helpers;
using System;
using System.Reflection;

namespace MVBP
{
    internal static class ModCompat
    {
        public const string ExtraSnapsGUID = "Searica.Valheim.ExtraSnapPointsMadeEasy";
        public const string PlanBuildGUID = "marcopogo.PlanBuild";
        public const string WackysDBGUID = "WackyMole.WackysDatabase";

        private static bool? _ExtraSnapsInstalled;
        private static bool? _PlanBuildInstalled;
        private static bool? _WackysDBInstalled;

        private static MethodInfo ReInitExtraSnapPoints;

        private static Type PlanDBType;
        private static object PlanDBInstance;
        private static MethodInfo PlanBuildScanTables;

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

            if (ReInitExtraSnapPoints == null)
            {
                try
                {
                    ReInitExtraSnapPoints = ReflectionUtils.GetMethod(plugin.GetType(), "ReInitExtraSnapPoints", Type.EmptyTypes);
                }
                catch (Exception e)
                {
                    Log.LogWarning(e);
                }
            }
            try
            {
                ReInitExtraSnapPoints?.Invoke(plugin, Array.Empty<object>());
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
            if (!IsPlanBuildInstalled())
            {
                return false;
            }

            var plugin = Chainloader.PluginInfos[PlanBuildGUID].Instance;
            if (plugin == null)
            {
                return false;
            }

            // trigger rescanning of piece tables via reflection          
            Log.LogInfo("PlanBuild is installed", LogLevel.Medium);
            try
            {
                var method = GetPlanBuildUpdateScanPieces();
                Log.LogInfo("Triggering PlanBuild.UpdateScanPieces", LogLevel.Medium);
                method?.Invoke(plugin, Array.Empty<object>()); // Invoke if not null
                return true;
            }
            catch (Exception e)
            {
                Log.LogWarning($"Failed to update PlanBuild:\n {e}");
            }

            return false;
        }

        private static MethodInfo GetPlanBuildUpdateScanPieces()
        {
            if (PlanBuildScanTables == null)
            {
                PlanBuildScanTables = AccessTools.Method("PlanBuild.PlanBuildPlugin:UpdateScanPieces", Type.EmptyTypes);
            }
            return PlanBuildScanTables;
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