using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MoreVanillaBuildPrefabs.Configs
{
    internal class Pickables
    {
        /// <summary>
        ///     Get a bool indicating if the prefab has a pickable
        ///     component that should be removed upon placement by
        ///     a player.
        /// </summary>
        /// <param name="PrefabName"></param>
        /// <returns></returns>
        internal static bool ShouldRemovePickableOnPlacment(string prefabName)
        {
            return _RemovePickableOnPlacment.Contains(prefabName);
        }

        private static readonly HashSet<string> _RemovePickableOnPlacment = new()
        {
            "Pickable_SurtlingCoreStand",
            "Pickable_BlackCoreStand",
            // "Pickable_Tar"
        };
    }
}
