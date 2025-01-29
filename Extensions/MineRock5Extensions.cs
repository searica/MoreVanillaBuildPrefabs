using HarmonyLib;
using System.Collections.Generic;
using UnityEngine;

namespace MVBP.Extensions;

[HarmonyPatch]
internal static class MineRock5Extensions
{
    internal class MineRock5Tracker : MonoBehaviour
    {
        public string m_prefabName;
    }

    /// <summary>
    ///     Add component to track original name of prefab
    /// </summary>
    /// <param name="__instance"></param>
    [HarmonyPrefix]
    [HarmonyPatch(typeof(MineRock5), nameof(MineRock5.Awake))]
    private static void TrackMineRock5PrefabName(MineRock5 __instance)
    {
        if (!__instance.GetComponent<MineRock5Tracker>())
        {
            MineRock5Tracker tracker = __instance.gameObject.AddComponent<MineRock5Tracker>();
            tracker.m_prefabName = Utils.GetPrefabName(__instance.gameObject);
        }
    }

    /// <summary>
    ///     Extension method to destroy a MineRock5 immediately.
    ///     If the MineRock5 has a piece component, then dropping
    ///     build resources is handled by MineRock5Patch and PiecePatch.
    /// </summary>
    /// <param name="mineRock5"></param>
    internal static void DestroyMineRock5Piece(this MineRock5 mineRock5)
    {
        if (!mineRock5 ||
            mineRock5.m_nview == null ||
            !mineRock5.m_nview.IsValid() ||
            !mineRock5.m_nview.IsOwner())
        {
            return;
        }

        for (int i = 0; i < mineRock5.m_hitAreas.Count; i++)
        {
            MineRock5.HitArea hitArea = mineRock5.m_hitAreas[i];
            if (hitArea.m_health > 0f)
            {
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
    internal static List<DropTableExtensions.AvgItemDrop> GetAvgDrops(this MineRock5 mineRock5)
    {
        if (!mineRock5)
        {
            return new List<DropTableExtensions.AvgItemDrop>();
        }

        List<DropTableExtensions.AvgItemDrop> avgDrops = mineRock5.m_dropItems.GetAvgDrops();
        int hitAreasCount = mineRock5.gameObject.GetComponentsInChildren<Collider>().Length;
        foreach (DropTableExtensions.AvgItemDrop drop in avgDrops)
        {
            drop.amount *= hitAreasCount;
        }
        return avgDrops;
    }
}
