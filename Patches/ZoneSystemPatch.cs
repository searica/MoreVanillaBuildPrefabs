using HarmonyLib;
using MoreVanillaBuildPrefabs.Configs;
using MoreVanillaBuildPrefabs.Helpers;
using MoreVanillaBuildPrefabs.Logging;
using UnityEngine.SceneManagement;

namespace MoreVanillaBuildPrefabs.Patches
{

    [HarmonyPatch(typeof(ZoneSystem))]
    internal class ZoneSystemPatch
    {
        /// <summary>
        ///     Hook to initialize the mod. This is after both PlantEverything
        ///     and PotteryBarn add pieces but before PlanBuild scans for them.
        /// </summary>
        /// <param name="__instance"></param>
        [HarmonyPostfix]
        [HarmonyPatch(nameof(ZoneSystem.Start))]
        public static void ZoneSystemStartPostfix()
        {
            if (PluginConfig.IsVerbosityMedium)
            {
                Log.LogInfo("ZoneSystemStartPostfix()");
            }

            if (SceneManager.GetActiveScene() == null)
            {
                return;
            }

            // If loading into game world and prefabs have not been added
            if (SceneManager.GetActiveScene().name == "main")
            {

                Log.LogInfo("Performing mod initialization");

                var watch = new System.Diagnostics.Stopwatch();
                if (PluginConfig.IsVerbosityMedium)
                {
                    watch.Start();
                }

                CreatorShopHelper.AddCreatorShopPieceCategory();
                MoreVanillaBuildPrefabs.InitPrefabRefs();
                MoreVanillaBuildPrefabs.InitPieceRefs();
                MoreVanillaBuildPrefabs.InitPieces();
                MoreVanillaBuildPrefabs.InitHammer();

                if (PluginConfig.IsVerbosityMedium)
                {
                    watch.Stop();
                    Log.LogInfo($"Time to initialize: {watch.ElapsedMilliseconds} ms");
                }
            }
        }
    }
}

