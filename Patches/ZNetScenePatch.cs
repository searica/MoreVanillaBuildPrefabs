﻿using HarmonyLib;
using System.Linq;
using UnityEngine.SceneManagement;


namespace MoreVanillaBuildPrefabs
{

    [HarmonyPatch(typeof(ZNetScene))]
    internal class ZNetScenePatch
    {
        // Hook just ZNetScene as destroyed during log out
        [HarmonyPrefix]
        [HarmonyPatch(nameof(ZNetScene.Shutdown))]
        static void ZNetSceneShutDown()
        {
#if DEBUG
            Log.LogInfo("ZNetScene.ShutDown()");
#endif
            if (PluginConfig.IsModEnabled.Value)
            {
                PluginConfig.Save(); // save cfg file changes on logout

                if (SceneManager.GetActiveScene() == null)
                {
                    return;
                }
                if (SceneManager.GetActiveScene().name == "main" && PieceHelper.AddedPrefabs.Count != 0)
                {
                    RemoveCustomPieces();
                    HammerCategories.RemoveCustomCategories();
                }
            }
        }

        internal static void RemoveCustomPieces()
        {
            Log.LogInfo("RemoveCustomPieces()");
            PieceTable pieceTable = PieceHelper.GetPieceTable("_HammerPieceTable");

            int numCustomPieces = PieceHelper.AddedPrefabs.Count();
            var prefabsToRemove = PieceHelper.AddedPrefabs.ToList();
            foreach (var name in prefabsToRemove)
            {
                PieceHelper.RemovePieceFromPieceTable(name, pieceTable);
            }
            Log.LogInfo($"Removed {numCustomPieces - PieceHelper.AddedPrefabs.Count} custom pieces");
        }
    }
}
