using UnityEngine.SceneManagement;
using HarmonyLib;

using MoreVanillaBuildPrefabs.Helpers;
using MoreVanillaBuildPrefabs.Logging;
using MoreVanillaBuildPrefabs.Configs;

namespace MoreVanillaBuildPrefabs.Patchess
{
    [HarmonyPatch(typeof(ObjectDB))]
    internal class ObjectDBPatch
    {
        // Hook here to add pieces
        [HarmonyPostfix]
        [HarmonyPriority(Priority.Low)]
        [HarmonyPatch(nameof(ObjectDB.Awake))]
        static void ObjectDBAwakePostfix()
        {
            if (PluginConfig.IsVerbosityMedium)
            {
                Log.LogInfo("ObjectDBAwakePostfix()");
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
