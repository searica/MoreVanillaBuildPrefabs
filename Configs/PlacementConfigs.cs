using System.Collections.Generic;

namespace MoreVanillaBuildPrefabs.Configs
{
    internal class PlacementConfigs
    {
        /// <summary>
        ///     Get a bool indicating if the prefab should be
        ///     allowed to clip through everything.
        /// </summary>
        /// <param name="PrefabName"></param>
        /// <returns></returns>
        internal static bool CanClipEverything(string prefabName)
        {
            return !_RestrictClipping.Contains(prefabName);
        }

        /// <summary>
        ///     Get a bool indicating if the prefab should be
        ///     allowed to clip into the ground..
        /// </summary>
        /// <param name="PrefabName"></param>
        /// <returns></returns>
        internal static bool CanClipGround(string prefabName)
        {
            return _CanClipGround.Contains(prefabName);
        }

        /// <summary>
        ///     Get a bool indicating if the prefab is in the
        ///     HashSet of prefabs that need a collision patch.
        /// </summary>
        /// <param name="PrefabName"></param>
        /// <returns></returns>
        internal static bool NeedsCollisionPatchForGhost(string prefabName)
        {
            return _NeedsCollisionPatchForGhost.Contains(prefabName);
        }

        internal static readonly HashSet<string> _NeedsCollisionPatchForGhost = new()
        {
            "blackmarble_stair_corner",
            "blackmarble_stair_corner_left",
            "Ice_floor",
        };

        // EligiblePrefabs that should not be set to allow clipping everything
        private static readonly HashSet<string> _RestrictClipping = new()
        {
            "blackmarble_post01",
            "dverger_demister",
            "dverger_demister_large",
            "dvergrprops_hooknchain",
            "barrell",
            "MountainKit_brazier_blue",
            "MountainKit_brazier",
            "stoneblock_fracture",
            "piece_dvergr_pole",
            "dvergrprops_wood_pole",
            "dvergrtown_wood_pole",
            "dvergrprops_wood_wall",
            "TreasureChest_dvergr_loose_stone",
            "Ice_floor",
            "CastleKit_braided_box01",
        };

        private static readonly HashSet<string> _CanClipGround = new()
        {
            "stoneblock_fracture",
            "blackmarble_post01",
        };
    }
}