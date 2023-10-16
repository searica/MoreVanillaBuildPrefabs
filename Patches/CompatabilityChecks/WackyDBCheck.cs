//using HarmonyLib;
//using MoreVanillaBuildPrefabs.Logging;

//namespace MoreVanillaBuildPrefabs.Patches
//{
//    [HarmonyPatch(typeof(Game))]
//    internal class GamePatch
//    {
//        [HarmonyPrefix]
//        [HarmonyPriority(Priority.High)]
//        [HarmonyPatch(nameof(Game._RequestRespawn))]
//        public static void _RequestRespawnPrefix()
//        {
//            Log.LogInfo("Game_RequestRespawnPrefix (WakcyDB hooks right after this)");
//        }
//    }
//}