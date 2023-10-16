//using HarmonyLib;
//using MoreVanillaBuildPrefabs.Logging;

//namespace MoreVanillaBuildPrefabs.Patches
//{
//    [HarmonyPatch(typeof(DungeonDB))]
//    internal class DungeonDBPatch
//    {
//        [HarmonyPostfix]
//        [HarmonyPatch(nameof(DungeonDB.Start))]
//        public static void DungeonDBStartPostfix()
//        {
//            Log.LogInfo("DungeonDB.Start Postfix (PlanBuild hooks around here)");
//        }
//    }
//}