using HarmonyLib;
using System.Linq;
using UnityEngine.SceneManagement;

using MoreVanillaBuildPrefabs.Configs;
using MoreVanillaBuildPrefabs.Helpers;
using MoreVanillaBuildPrefabs.Logging;

namespace MoreVanillaBuildPrefabs
{

    [HarmonyPatch(typeof(ZNetScene))]
    internal class ZNetScenePatch
    {
        // Hook just ZNetScene as destroyed during log out
        [HarmonyPrefix]
        [HarmonyPatch(nameof(ZNetScene.Shutdown))]
        static void ZNetSceneShutDown()
        {
#if DEBUG
            Log.LogInfo("ZNetScene.ShutDown()");
#endif
            if (PluginConfig.IsModEnabled.Value)
            {
                PluginConfig.Save(); // save cfg file changes on logout

                if (SceneManager.GetActiveScene() == null)
                {
                    return;
                }
                if (SceneManager.GetActiveScene().name == "main" && PieceHelper.AddedPrefabs.Count != 0)
                {
                    PieceHelper.RemoveCustomPieces();
                    HammerCategories.RemoveCustomCategories();
                }
            }
        }


    }
}
