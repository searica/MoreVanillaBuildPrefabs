//using HarmonyLib;
//using MoreVanillaBuildPrefabs.Logging;
//using MoreVanillaBuildPrefabs.Configs;
//using MoreVanillaBuildPrefabs.Helpers;

//namespace MoreVanillaBuildPrefabs.Patches
//{
//    [HarmonyPatch(typeof(Container))]
//    internal class ContainerPatch
//    {
//        [HarmonyPrefix]
//        [HarmonyPatch(nameof(Container.AddDefaultItems))]
//        internal static void AddDefaultItemsPrefix(Container __instance, out DropTable __state)
//        {
//            Log.LogInfo("AddDefaultItemsPrefix()");
//            Log.LogInfo(__instance.gameObject.name);
//            Log.LogInfo(__instance.name);
//            __state = null;
//            if (PluginConfig.IsModEnabled.Value)
//            {
//                var piece = __instance?.gameObject?.GetComponent<Piece>();
//                if (piece == null)
//                {
//                    Log.LogInfo("Piece is null");
//                    return;
//                }

//                var prefabName = NameHelper.GetPrefabName(piece);
//                Log.LogInfo($"AddedByMod {PieceHelper.IsAddedByMod(prefabName)}");
//                Log.LogInfo($"RemoveTresure {DefaultConfigs.RemoveTreasure.Contains(prefabName)}");
//                Log.LogInfo($"IsPlacedByPlayer {piece.IsPlacedByPlayer()}");
//                if (PieceHelper.IsAddedByMod(prefabName)
//                    && DefaultConfigs.RemoveTreasure.Contains(prefabName))
//                {
//                    Log.LogInfo("Change drop table");
//                    __state = __instance.m_defaultItems;
//                    __instance.m_defaultItems = new DropTable();
//                }
//            }
//        }

//        [HarmonyPostfix]
//        [HarmonyPatch(nameof(Container.AddDefaultItems))]
//        internal static void AddDefaultItemsPostfix(Container __instance, DropTable __state)
//        {
//            Log.LogInfo("AddDefaultItemsPostfix()");
//            if (__state != null)
//            {
//                Log.LogInfo("Undo change drop table");
//                __instance.m_defaultItems = __state;
//            }
//        }
//    }
//}
