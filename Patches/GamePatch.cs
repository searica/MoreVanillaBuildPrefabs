using HarmonyLib;
using UnityEngine.SceneManagement;

namespace MoreVanillaBuildPrefabs.Patches
{
    [HarmonyPatch(typeof(Game))]
    internal class GamePatch
    {

        // Hook here to add pieces after ServerSync recieves data
        [HarmonyPostfix]
        [HarmonyPriority(Priority.High)] // High priority for compatiability with WackyDB
        [HarmonyPatch(nameof(Game._RequestRespawn))]
        static void Game_RequestRespawnPostFix()
        {
#if DEBUG
            Log.LogInfo("Game._RequestRespawn.Postfix()");
#endif
            if (PluginConfig.IsModEnabled.Value)
            {
                if (SceneManager.GetActiveScene() == null)
                {
                    return;
                }

                // If loading into game world and prefabs have not been added
                if (SceneManager.GetActiveScene().name == "main" && PrefabHelper.AddedPrefabs.Count == 0)
                {
                    HammerCategories.AddCustomCategories();
                    PrefabHelper.FindAndAddPrefabs();
                }
            }
        }
    }
}
