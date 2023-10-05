using HarmonyLib;
using UnityEngine.SceneManagement;


namespace MoreVanillaBuildPrefabs
{

    [HarmonyPatch(typeof(ObjectDB))]
    internal class ObjectDBPatch
    {
        // Hook just before Jotunn registers the Pieces
        [HarmonyPrefix]
        [HarmonyPatch(nameof(ObjectDB.Awake))]
        static void ObjectDBAwake()
        {
#if DEBUG
            Log.LogInfo("ObjectDBAwake()");
#endif
            if (PluginConfig.IsModEnabled.Value)
            {
                if (SceneManager.GetActiveScene().name == "start" && PrefabHelper.AddedPrefabs.Count != 0)
                {
                    PrefabHelper.RemoveCustomPieces();
                    Hammer.RemoveHammerCategories();
                }
            }
        }
    }
}
