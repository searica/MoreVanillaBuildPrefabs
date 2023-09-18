using HarmonyLib;
using UnityEngine.SceneManagement;


namespace MoreVanillaBuildPrefabs
{

    [HarmonyPatch]
    internal class ObjectDBPatch
    {
        // Hook just before Jotunn registers the Pieces
        [HarmonyPatch(typeof(ObjectDB), "Awake"), HarmonyPrefix]
        static void ObjectDBAwakePrefix()
        {
            if (PluginConfig.IsModEnabled.Value)
            {
                if (SceneManager.GetActiveScene().name == "main")
                {
                    Plugin.AddHammerCategories();
                    PrefabAdder.FindAndRegisterPrefabs();
                }
            }
        }
    }
}
