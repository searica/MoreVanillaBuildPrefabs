//// Ignore Spelling: MVBP

//using HarmonyLib;
//using Jotunn;
//using Jotunn.Managers;
//using MVBP.Configs;

//namespace MVBP.Patches
//{
//    [HarmonyPatch(typeof(SynchronizationManager))]
//    internal class JotunnPatch
//    {
//        [HarmonyPostfix]
//        [HarmonyPatch("ConfigRPC_OnServerReceive")]
//        private static void ConfigRPC_OnServerReceivePostfix(long sender)
//        {
//            if (ZNet.instance != null && ZNet.instance.IsAdmin(sender) && ZNet.instance.IsServer())
//            {
//                Log.LogInfo("ConfigRPC_OnServerReceive Postfix!");
//                Config.Save();
//                //if (Config.SaveIfChanged())
//                //{
//                //    Log.LogInfo("Saved config");
//                //}
//                //else
//                //{
//                //    Log.LogInfo("Did not save config");
//                //}
//            }
//        }
//    }
//}

//// MVBP, Version=0.4.0.0, Culture=neutral, PublicKeyToken=null
//// MVBP.Patches.JotunnPatch
//using HarmonyLib;
//using Jotunn.Managers;
//using MVBP;

//[HarmonyPatch(typeof(SynchronizationManager))]
//internal class JotunnPatch
//{
//    [HarmonyPrefix]
//    [HarmonyPriority(200)]
//    [HarmonyPatch("ConfigRPC_OnClientReceive")]
//    private static void ConfigRPC_OnClientReceive_Prefix()
//    {
//        global::MVBP.MVBP.DisableIndividualConfigEvents = true;
//    }
//}