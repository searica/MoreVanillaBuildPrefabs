// Ignore Spelling: MVBP

using HarmonyLib;
using MVBP.Extensions;
using MVBP.Helpers;
using UnityEngine.SceneManagement;

namespace MVBP.Patches {
    [HarmonyPatch(typeof(ZoneSystem))]
    internal static class ZoneSystemPatch {
        /// <summary>
        ///     Hook to initialize the mod. This is after both PlantEverything
        ///     and PotteryBarn add pieces but before PlanBuild scans for them.
        /// </summary>
        /// <param name="__instance"></param>
        [HarmonyPrefix]
        [HarmonyPriority(Priority.High)]
        [HarmonyPatch(nameof(ZoneSystem.Start))]
        public static void ZoneSystemStartPrefix() {
            Log.LogInfo("ZoneSystemStartPrefix()", LogLevel.Medium);

            // If loading into game world and prefabs have not been added
            if (SceneManager.GetActiveScene().name != "main") {
                return;
            }

            Log.LogInfo("Performing mod initialization");

            var watch = new System.Diagnostics.Stopwatch();
            if (Log.IsVerbosityMedium) { watch.Start(); }

            InitManager.InitPlugin();

            if (Log.IsVerbosityMedium) {
                watch.Stop();
                Log.LogInfo($"Time to initialize: {watch.ElapsedMilliseconds} ms");
            }

            //Log.LogInfo("Getting average drops");

            //foreach (var prefab in InitManager.PrefabRefs.Values) {
            //    if (prefab.TryGetComponent(out DropOnDestroyed dropOnDestroyed)) {
            //        Log.LogInfo($"{prefab.name}:");
            //        foreach (var drop in dropOnDestroyed.GetAvgDrops()) {
            //            Log.LogInfo($"\t-{drop.item.name}: {drop.amount}");
            //        }
            //    }
            //}
        }
    }
}