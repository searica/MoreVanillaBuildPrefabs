using System.Collections.Generic;
using UnityEngine;

namespace MVBP.Extensions {
    internal static class MineRock5Extensions {
        /// <summary>
        ///     Extension method to destroy a MineRock5 immediately.
        ///     If the MineRock5 has a piece component, then dropping
        ///     build resources is handled by MineRock5Patch and PiecePatch.
        /// </summary>
        /// <param name="mineRock5"></param>
        internal static void DestroyMineRock5Piece(this MineRock5 mineRock5) {
            if (!mineRock5 ||
                mineRock5.m_nview == null ||
                !mineRock5.m_nview.IsValid() ||
                !mineRock5.m_nview.IsOwner()) {
                return;
            }

            for (int i = 0; i < mineRock5.m_hitAreas.Count; i++) {
                MineRock5.HitArea hitArea = mineRock5.m_hitAreas[i];
                if (hitArea.m_health > 0f) {
                    var hitData = new HitData();
                    hitData.m_damage.m_damage = mineRock5.m_health;
                    hitData.m_point = hitArea.m_collider.bounds.center;
                    hitData.m_toolTier = 100;
                    hitData.m_hitType = HitData.HitType.Structural;
                    mineRock5.DamageArea(i, hitData);
                }
            }
        }

        /// <summary>
        ///     Extension method to compute the average drops when destroying all hit areas.
        /// </summary>
        /// <param name="mineRock5"></param>
        /// <returns></returns>
        internal static List<DropTableExtensions.AvgItemDrop> GetAvgDrops(this MineRock5 mineRock5) {

            var avgDrops = mineRock5.m_dropItems.GetAvgDrops();
            var hitAreasCount = mineRock5.gameObject.GetComponentsInChildren<Collider>().Length;
            foreach (var drop in avgDrops) {
                drop.amount *= hitAreasCount;
            }
            return avgDrops;
        }
    }
}
