using HarmonyLib;
using UnityEngine.SceneManagement;


namespace MoreVanillaBuildPrefabs
{

    [HarmonyPatch(typeof(ZNetScene))]
    internal class ZNetScenePatch
    {
        // Hook just before Jotunn registers the Pieces
        [HarmonyPrefix]
        [HarmonyPatch(nameof(ZNetScene.Shutdown))]
        static void ZNetSceneShutDown()
        {
#if DEBUG
            Log.LogInfo("ZNetScene.ShutDown()");
#endif
            if (PluginConfig.IsModEnabled.Value)
            {
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
