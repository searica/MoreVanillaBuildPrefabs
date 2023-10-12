//using HarmonyLib;
//using System.Collections.Generic;
//using System.Linq;
//using UnityEngine;
//using UnityEngine.SceneManagement;

//using MoreVanillaBuildPrefabs.Configs;
//using MoreVanillaBuildPrefabs.Logging;
//using MoreVanillaBuildPrefabs.Helpers;
//using Jotunn.Managers;


//namespace MoreVanillaBuildPrefabs.Patches
//{
//    [HarmonyPatch(typeof(ObjectDB))]
//    internal class ObjectDBPatch
//    {
//        // Hook here to add pieces
//        [HarmonyPrefix]
//        // [HarmonyPriority(Priority.High)] // High priority for compatiability with WackyDB
//        [HarmonyPatch(nameof(ObjectDB.Awake))]
//        static void ObjectDBAwakePrefix()
//        {
//            Log.LogInfo("ObjectDB.Awake()");
//            //PieceAdder.AddPieces();
//        }
//    }
//}
