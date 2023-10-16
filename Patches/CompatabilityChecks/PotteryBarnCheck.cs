//using Jotunn.Managers;
//using MoreVanillaBuildPrefabs.Logging;


//namespace MoreVanillaBuildPrefabs.Patches.CompatabilityChecks
//{
//    internal class PotteryBarnCheck
//    {
//        internal static void RunPotteryBarnCheck()
//        {
//            PieceManager.OnPiecesRegistered += PotteryBarnHookMsg;
//        }

//        private static void PotteryBarnHookMsg()
//        {
//            Log.LogInfo("Pottery barn hooks here");
//            PieceManager.OnPiecesRegistered -= PotteryBarnHookMsg;
//        }
//    }
//}
