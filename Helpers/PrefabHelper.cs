using System;
using UnityEngine;
using MoreVanillaBuildPrefabs.Configs;

namespace MoreVanillaBuildPrefabs.Helpers
{
    public class PrefabHelper
    {
        

        /// <summary>
        ///     Prevents creation of duplicate ZNetViews
        /// </summary>
        /// <param name="prefab"></param>
        /// <returns></returns>
        internal static bool EnsureNoDuplicateZNetView(GameObject prefab)
        {
            var views = prefab?.GetComponents<ZNetView>();

            if (views == null) return true;

            for (int i = 1; i < views.Length; ++i)
            {
                GameObject.DestroyImmediate(views[i]);
            }

            return views.Length <= 1;
        }
    }
}
