using HarmonyLib;
using UnityEngine.SceneManagement;


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
                if (SceneManager.GetActiveScene().name == "main" && PrefabHelper.AddedPrefabs.Count != 0)
                {
                    PrefabHelper.RemoveCustomPieces();
                    HammerCategories.RemoveCustomCategories();
                }
            }
        }
    }
}
