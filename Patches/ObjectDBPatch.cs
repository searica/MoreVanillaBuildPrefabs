using UnityEngine.SceneManagement;
using HarmonyLib;

using MoreVanillaBuildPrefabs.Helpers;
using MoreVanillaBuildPrefabs.Logging;



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
#if DEBUG
            Log.LogInfo("ObjectDBAwakePostfix()");
#endif
            if (SceneManager.GetActiveScene() == null)
            {
                return;
            }

            // If loading into game world and prefabs have not been added
            if (SceneManager.GetActiveScene().name == "main")
            {
                Log.LogInfo("Performing mod initialization");
                CreatorShopHelper.AddCreatorShopPieceCategory();
                MoreVanillaBuildPrefabs.InitPrefabRefs();
                MoreVanillaBuildPrefabs.InitPieceRefs();
                MoreVanillaBuildPrefabs.InitPieces();
                MoreVanillaBuildPrefabs.InitHammer();
            }
        }
    }
}
