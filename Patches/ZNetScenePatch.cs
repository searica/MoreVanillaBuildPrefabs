//using HarmonyLib;

//using MoreVanillaBuildPrefabs.Logging;
//using UnityEngine.SceneManagement;
//using static MoreVanillaBuildPrefabs.MoreVanillaBuildPrefabs;

//namespace MoreVanillaBuildPrefabs
//{

//    [HarmonyPatch(typeof(ZNetScene))]
//    internal class ZNetScenePatch
//    {
//        [HarmonyPostfix]
//        [HarmonyPatch(nameof(ZNetScene.Awake))]
//        public static void ZNetSceneAwakePostfix(ZNetScene __instance)
//        {
//            Log.LogInfo("ZNetSceneAwake");
//            Log.LogInfo("Performing final mod initialization");
//            FinalInit();
//            //if (SceneManager.GetActiveScene() == null)
//            //{
//            //    return;
//            //}

//            //// If loading into game world and prefabs have not been added
//            //if (SceneManager.GetActiveScene().name == "main")
//            //{
//            //    FinalInit();
//            //}
//        }
//    }
//}
