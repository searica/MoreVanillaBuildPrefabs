using HarmonyLib;
using UnityEngine.SceneManagement;


namespace MoreVanillaBuildPrefabs
{
    // Switch to using Azumatt piece manager instead of Jotunn?

    [HarmonyPatch(typeof(ObjectDB))]
    internal class ObjectDBPatch
    {
        // Hook just before Jotunn registers the Pieces
        [HarmonyPrefix]
        [HarmonyPatch(nameof(ObjectDB.Awake))]
        static void ObjectDBAwake()
        {
            Log.LogInfo("ObjectDBAwake()");
            if (PluginConfig.IsModEnabled.Value)
            {
                if (SceneManager.GetActiveScene().name == "start" && PrefabHelper.AddedPieces.Count != 0)
                {
                    PrefabHelper.RemoveAddedPrefabs();
                    Plugin.RemoveHammerCategories();
                }
            }
        }
    }
}
