//using System.Collections.Generic;

//namespace MoreVanillaBuildPrefabs.Configs
//{
//    internal class TreasureChests
//    {
//        /// <summary>
//        ///     Get a bool indicating if the prefab is a treasure chest
//        ///     and the contents should be removed when placing it.
//        /// </summary>
//        /// <param name="PrefabName"></param>
//        /// <returns></returns>
//        internal static bool ShouldRemoveTreasure(string PrefabName)
//        {
//            return _RemoveTreasureOnPlacement.Contains(PrefabName) || PrefabName.StartsWith("TreasureChest");
//        }

//        private static readonly HashSet<string> _RemoveTreasureOnPlacement = new()
//        {
//            "TreasureChest_fCrypt",
//            "TreasureChest_mountaincave",
//            "TreasureChest_plains_stone",
//            "TreasureChest_sunkencrypt",
//            "TreasureChest_trollcave",
//            "TreasureChest_dvergr_loose_stone",
//            "TreasureChest_dvergrtown",
//            "TreasureChest_dvergrtower",
//        };
//    }
//}