using System.Collections.Generic;
using UnityEngine;
using Logging;

namespace MVBP.Extensions;

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
        if (!mineRock || mineRock.m_nview == null || !mineRock.m_nview.IsValid() || !mineRock.m_nview.IsOwner())
        {
            return;
        }

        var hitData = new HitData();
        hitData.m_damage.m_damage = mineRock.m_health;
        hitData.m_toolTier = 100;
        hitData.m_hitType = HitData.HitType.Structural;
        //hitData.m_attacker = ZDOID.None; // ideally use this
        hitData.m_attacker = Player.m_localPlayer.GetZDOID(); // have to use this because of the silly hit.GetAttacker().GetZDOID()

        for (int i = 0; i < mineRock.m_hitAreas.Length; i++)
        {
            if (mineRock && mineRock.m_nview != null && mineRock.m_nview.IsValid() && mineRock.m_nview.IsOwner())
            {
                Collider hitArea = mineRock.m_hitAreas[i];
                if (!hitArea)
                {
                    Log.LogWarning($"Missing hit area {i}");
                    continue;
                }
                hitData.m_point = hitArea.bounds.center;
                Log.LogInfo($"Hitting area {i}");
                mineRock.m_nview.InvokeRPC("Hit", hitData, i);
            }
        }

        if (mineRock && mineRock.m_nview != null && mineRock.m_nview.IsValid() && mineRock.m_removeWhenDestroyed && mineRock.AllDestroyed())
        {
            mineRock.m_nview.Destroy();
        }
    }

    /// <summary>
    ///     Extension method to compute the average drops when destroying all hit areas.
    /// </summary>
    /// <param name="mineRock"></param>
    /// <returns></returns>
    internal static List<DropTableExtensions.AvgItemDrop> GetAvgDrops(this MineRock mineRock)
    {
        if (!mineRock)
        {
            return new List<DropTableExtensions.AvgItemDrop>();
        }

        List<DropTableExtensions.AvgItemDrop> avgDrops = mineRock.m_dropItems.GetAvgDrops();
        int hitAreasCount = mineRock.gameObject.GetComponentsInChildren<Collider>().Length;
        foreach (DropTableExtensions.AvgItemDrop drop in avgDrops)
        {
            drop.amount *= hitAreasCount;
        }

        return avgDrops;
    }
}
