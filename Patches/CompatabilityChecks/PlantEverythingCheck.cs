//using HarmonyLib;
//using MoreVanillaBuildPrefabs.Logging;

//namespace MoreVanillaBuildPrefabs.Patches
//{

//    [HarmonyPatch(typeof(ZNetScene))]
//    internal class ZNetScenePatch
//    {
//        [HarmonyPostfix]
//        [HarmonyPatch(nameof(ZNetScene.Awake))]
//        public static void Postfix(ZNetScene __instance)
//        {
//            Log.LogInfo("ZNetSceneAwakePostFix (PlantEverything hooks here)");
//        }
//    }
//}

