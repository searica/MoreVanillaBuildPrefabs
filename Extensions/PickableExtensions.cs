// Ignore Spelling: MVBP

using UnityEngine;

namespace MVBP.Extensions
{
    internal static class PickableExtensions
    {
        /// <summary>
        ///     Get the amount of the resource that will be returned when it is picked,
        ///     based on current world modifier settings for scaling resources
        /// </summary>
        /// <param name="pickable"></param>
        /// <returns></returns>
        internal static int GetScaledDropAmount(this Pickable pickable)
        {
            if (Game.instance == null)
            {
                return pickable.m_amount;
            }
            return pickable.m_dontScale ? pickable.m_amount : Mathf.Max(pickable.m_minAmountScaled, Game.instance.ScaleDrops(pickable.m_itemPrefab, pickable.m_amount));
        }
    }
}