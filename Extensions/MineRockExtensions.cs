using System.Collections.Generic;
using UnityEngine;

namespace MVBP.Extensions
{
    internal static class MineRockExtensions
    {
        /// <summary>
        ///     Extension method to destroy a MineRock immediately.
        ///     If the MineRock has a piece component, then dropping
        ///     build resources is handled by MineRockPatch and PiecePatch.
        /// </summary>
        /// <param name="mineRock5"></param>
        internal static void DestroyMineRockPiece(this MineRock mineRock)
        {
            if (mineRock == null || mineRock.m_nview == null || !mineRock.m_nview.IsValid() || !mineRock.m_nview.IsOwner())
            {
                return;
            }

            for (int i = 0; i < mineRock.m_hitAreas.Length; i++)
            {
                if (mineRock != null && mineRock.m_nview != null && mineRock.m_nview.IsValid() && mineRock.m_nview.IsOwner())
                {
                    var hitArea = mineRock.m_hitAreas[i];
                    var hitData = new HitData();
                    hitData.m_damage.m_damage = mineRock.m_health;
                    hitData.m_point = hitArea.bounds.center;
                    hitData.m_toolTier = 100;
                    hitData.m_hitType = HitData.HitType.Structural;
                    mineRock.m_nview.InvokeRPC("Hit", hitData, i);
                }
            }
        }

        /// <summary>
        ///     Extension method to compute the average drops when destroying all hit areas.
        /// </summary>
        /// <param name="mineRock"></param>
        /// <returns></returns>
        internal static List<DropTableExtensions.AvgItemDrop> GetAvgDrops(this MineRock mineRock)
        {
            var avgDrops = mineRock?.m_dropItems.GetAvgDrops();
            var hitAreasCount = mineRock.gameObject.GetComponentsInChildren<Collider>().Length;
            foreach (var drop in avgDrops)
            {
                drop.amount *= hitAreasCount;
            }
            return avgDrops;
        }
    }
}