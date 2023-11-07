// Ignore Spelling: MVBP

using HarmonyLib;

using MVBP.Configs;
using MVBP.Helpers;
using UnityEngine.SceneManagement;

namespace MVBP.Patches
{
    [HarmonyPatch(typeof(ZoneSystem))]
    internal class ZoneSystemPatch
    {
        /// <summary>
        ///     Hook to initialize the mod. This is after both PlantEverything
        ///     and PotteryBarn add pieces but before PlanBuild scans for them.
        /// </summary>
        /// <param name="__instance"></param>
        [HarmonyPrefix]
        [HarmonyPriority(Priority.High)]
        [HarmonyPatch(nameof(ZoneSystem.Start))]
        public static void ZoneSystemStartPrefix()
        {
            if (Config.IsVerbosityMedium)
            {
                Log.LogInfo("ZoneSystemStartPrefix()");
            }

            // If loading into game world and prefabs have not been added
            if (SceneManager.GetActiveScene() == null
                || SceneManager.GetActiveScene().name != "main")
            {
                return;
            }

            Log.LogInfo("Performing mod initialization");

            var watch = new System.Diagnostics.Stopwatch();
            if (Config.IsVerbosityMedium)
            {
                watch.Start();
            }

            InitManager.InitPlugin();

            if (Config.IsVerbosityMedium)
            {
                watch.Stop();
                Log.LogInfo($"Time to initialize: {watch.ElapsedMilliseconds} ms");
            }
        }
    }
}