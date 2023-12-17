using System.Collections.Generic;

namespace MVBP.Extensions {
    internal static class DropOnDestroyedExtensions {
        /// <summary>
        ///     Extension method to compute the average drops when destroying all hit areas.
        /// </summary>
        /// <param name="mineRock5"></param>
        /// <returns></returns>
        internal static List<DropTableExtensions.AvgItemDrop> GetAvgDrops(
            this DropOnDestroyed dropOnDestroyed
        ) {
            return dropOnDestroyed.m_dropWhenDestroyed.GetAvgDrops();
        }
    }
}
