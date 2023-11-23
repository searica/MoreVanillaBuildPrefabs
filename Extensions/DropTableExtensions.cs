// Ignore Spelling: MVBP

using System.Collections.Generic;
using System.Linq;

namespace MVBP.Extensions
{
    internal static class DropTableExtensions
    {
        internal struct Amount
        {
            public int min;
            public int max;
            public float chance;

            public Amount(int min, int max, float chance = 1f)
            {
                this.min = min;
                this.max = max;
                this.chance = chance;
            }

            public readonly float GetAvgAmount()
            {
                return (min + max) * chance / 2;
            }
        }

        internal class AvgItemDrop
        {
            public ItemDrop item;
            public float amount;

            public AvgItemDrop(ItemDrop item, float amount)
            {
                this.item = item;
                this.amount = amount;
            }
        }

        // scaling affects how many times it rolls for the item
        internal static List<AvgItemDrop> GetAvgDrops(this DropTable dropTable)
        {
            var tableAmount = new Amount(dropTable.m_dropMin, dropTable.m_dropMax, dropTable.m_dropChance).GetAvgAmount();
            float totalWeight = dropTable.m_drops.Sum(i => i.m_weight);
            var avgItemDrops = new List<AvgItemDrop>();
            foreach (DropTable.DropData drop in dropTable.m_drops)
            {
                float chance = totalWeight == 0 ? 1 : drop.m_weight / totalWeight;
                float dropAmount = new Amount(drop.m_stackMin, drop.m_stackMax).GetAvgAmount();
                float avgDropAmount = tableAmount * chance * dropAmount;
                if (drop.m_item.TryGetComponent(out ItemDrop itemDrop))
                {
                    avgItemDrops.Add(new AvgItemDrop(itemDrop, avgDropAmount));
                }
            }
            return avgItemDrops;
        }
    }
}