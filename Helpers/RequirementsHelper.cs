using MVBP.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MVBP.Helpers
{
    internal class RequirementsHelper
    {
        /// <summary>
        ///     Convert requirements string from cfg file to Piece.Requirement Array
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        internal static Piece.Requirement[] CreateRequirementsArray(string data)
        {
            // avoid calling Trim() on null object
            if (data == null || string.IsNullOrEmpty(data.Trim()))
            {
                return Array.Empty<Piece.Requirement>();
            }

            // If not empty
            List<Piece.Requirement> requirements = new();

            foreach (var entry in data.Split(';'))
            {
                string[] values = entry.Split(',');
                var itm = ObjectDB.instance.GetItemPrefab(values[0].Trim())?.GetComponent<ItemDrop>();
                if (itm == null)
                {
                    Log.LogWarning($"Unable to find requirement ID: {values[0].Trim()}");
                    continue;
                }
                Piece.Requirement req = new()
                {
                    m_resItem = itm,
                    m_amount = int.Parse(values[1].Trim()),
                    m_recover = true
                };
                requirements.Add(req);
            }
            return requirements.ToArray();
        }

        /// <summary>
        ///     If the pickable is not null and drops an item, then modify the
        ///     requirements array to require the item dropped by the pickable to
        ///     build and cost a minimum dropAmount equal to the dropAmount dropped by the
        ///     pickable  (accounts for world modifiers to resource drops).
        /// </summary>
        /// <param name="requirements"></param>
        /// <param name="pickable"></param>
        /// <returns></returns>
        internal static Piece.Requirement[] AddPickableToRequirements(
           Piece.Requirement[] requirements,
           Pickable pickable
        )
        {
            // If the pickable does not exist or does not drop an item, return the requirements array unchanged.
            var pickableDrop = pickable?.m_itemPrefab?.GetComponent<ItemDrop>()?.m_itemData;
            if (requirements == null || pickable == null || pickableDrop == null)
            {
                return requirements;
            }

            // Get dropAmount returned on picking based on world modifiers
            var pickedAmount = pickable.GetScaledDropAmount();

            // Check if pickable is included in piece build requirements
            foreach (var req in requirements)
            {
                if (req.m_resItem.m_itemData.m_shared.m_name == pickableDrop.m_shared.m_name)
                {
                    // If build requirements for pickable item are less than
                    // the pickable drops increase them to equal it
                    if (req.m_amount < pickedAmount)
                    {
                        // this should change the value within the parent array?
                        req.m_amount = pickedAmount;
                        return requirements;
                    }
                    return requirements;
                }
            }

            var pickableReq = new Piece.Requirement()
            {
                m_resItem = pickable?.m_itemPrefab?.GetComponent<ItemDrop>(),
                m_amount = pickedAmount,
                m_recover = true
            };
            var reqList = requirements.ToList();
            reqList.Add(pickableReq);
            return reqList.ToArray();
        }

        /// <summary>
        ///     Used when deconstructing pickable pieces to prevent infinite item exploits.
        ///     Checks if the resources defined by the requirements array include
        ///     the item dropped by the pickable and reduces the dropAmount of the resources
        ///     based on if the pickable has been picked (accounts for world modifiers for resources).
        /// </summary>
        /// <param name="requirements"></param>
        /// <param name="pickable"></param>
        /// <returns></returns>
        internal static Piece.Requirement[] RemovePickableFromRequirements(
            Piece.Requirement[] requirements,
            Pickable pickable
        )
        {
            // If the pickable does not drop an item or has not been picked, return the
            // requirements array unchanged.
            var pickableDrop = pickable?.m_itemPrefab?.GetComponent<ItemDrop>()?.m_itemData;
            if (requirements == null || pickable == null || !pickable.m_picked || pickableDrop == null)
            {
                return requirements;
            }

            // Check if pickable is included in piece build requirements
            for (int i = 0; i < requirements.Length; i++)
            {
                var req = requirements[i];
                if (req.m_resItem.m_itemData.m_shared.m_name == pickableDrop.m_shared.m_name)
                {
                    // Make a copy before altering drops
                    var pickedRequirements = new Piece.Requirement[requirements.Length];
                    requirements.CopyTo(pickedRequirements, 0);

                    // Get dropAmount returned on picking based on world modifiers
                    var pickedAmount = pickable.GetScaledDropAmount();

                    // Reduce resource drops for the picked item by the dropAmount that picking the item gave.
                    // This is to prevent infinite resource exploits.
                    pickedRequirements[i].m_amount = Mathf.Clamp(req.m_amount - pickedAmount, 0, req.m_amount);
                    return pickedRequirements;
                }
            }

            // If pickable item is not present then return the requirements array unchanged.
            return requirements;
        }

        /// <summary>
        ///     Adjust the requirements to build so that it requires resources
        ///     equal to or greater than the average dropAmount dropped by mining
        ///     out the entire MineRock component.
        ///     (does not accounts for world modifiers to resource drops yet).
        /// </summary>
        /// <param name="requirements"></param>
        /// <param name="mineRock"></param>
        /// <returns></returns>
        internal static Piece.Requirement[] AddMineRockDropsToRequirements(
            Piece.Requirement[] requirements,
            MineRock mineRock
        )
        {
            if (requirements == null || mineRock == null)
            {
                return requirements;
            }

            var avgDrops = mineRock.GetAvgDrops();
            var reqList = requirements.ToList();
            foreach (var drop in avgDrops)
            {
                var dropName = drop.item.m_itemData.m_shared.m_name;
                var dropAmount = (int)Mathf.Round(drop.amount);
                bool inReqs = false;
                foreach (var req in reqList)
                {
                    // If drop is in piece requirements then adjust dropAmount as needed
                    if (req.m_resItem.m_itemData.m_shared.m_name == dropName)
                    {
                        inReqs = true;
                        if (req.m_amount < drop.amount)
                        {
                            req.m_amount = dropAmount;
                        }
                    }
                }
                // If drop not in requirements then add it
                if (!inReqs)
                {
                    var mineRockReq = new Piece.Requirement()
                    {
                        m_resItem = drop.item,
                        m_amount = dropAmount,
                        m_recover = true
                    };
                    reqList.Add(mineRockReq);
                }
            }
            return reqList.ToArray();
        }

        /// <summary>
        ///     Adjust the requirements to build so that dropped resources
        ///     are reduced by the average dropAmount dropped by mining
        ///     out the entire MineRock component.
        ///     (does not accounts for world modifiers to resource drops yet).
        /// </summary>
        /// <param name="requirements"></param>
        /// <param name="mineRock"></param>
        /// <returns></returns>
        internal static Piece.Requirement[] RemoveMineRockDropsFromRequirements(
            Piece.Requirement[] requirements,
            MineRock mineRock
        )
        {
            if (requirements == null || mineRock == null)
            {
                return requirements;
            }

            var avgDrops = mineRock.GetAvgDrops();
            // Make a copy before altering drops
            var newRequirements = new Piece.Requirement[requirements.Length];
            requirements.CopyTo(newRequirements, 0);
            foreach (var drop in avgDrops)
            {
                var dropName = drop.item.m_itemData.m_shared.m_name;
                var dropAmount = (int)Mathf.Round(drop.amount);
                foreach (var req in newRequirements)
                {
                    // If drop is in piece requirements then adjust dropAmount as needed
                    if (req.m_resItem.m_itemData.m_shared.m_name == dropName)
                    {
                        req.m_amount = Mathf.Clamp(req.m_amount - dropAmount, 0, req.m_amount);
                    }
                }
            }
            return newRequirements;
        }

        /// <summary>
        ///     Adjust the requirements to build so that it requires resources
        ///     equal to or greater than the average dropAmount dropped by mining
        ///     out the entire MineRock5 component.
        ///     (does not accounts for world modifiers to resource drops yet).
        /// </summary>
        /// <param name="requirements"></param>
        /// <param name="mineRock5"></param>
        /// <returns></returns>
        internal static Piece.Requirement[] AddMineRock5DropsToRequirements(
            Piece.Requirement[] requirements,
            MineRock5 mineRock5
        )
        {
            if (requirements == null || mineRock5 == null)
            {
                return requirements;
            }

            var avgDrops = mineRock5.GetAvgDrops();
            var reqList = requirements.ToList();
            foreach (var drop in avgDrops)
            {
                var dropName = drop.item.m_itemData.m_shared.m_name;
                var dropAmount = (int)Mathf.Round(drop.amount);
                bool inReqs = false;
                foreach (var req in reqList)
                {
                    // If drop is in piece requirements then adjust dropAmount as needed
                    if (req.m_resItem.m_itemData.m_shared.m_name == dropName)
                    {
                        inReqs = true;
                        if (req.m_amount < drop.amount)
                        {
                            req.m_amount = dropAmount;
                        }
                    }
                }
                // If drop not in requirements then add it
                if (!inReqs)
                {
                    var mineRockReq = new Piece.Requirement()
                    {
                        m_resItem = drop.item,
                        m_amount = dropAmount,
                        m_recover = true
                    };
                    reqList.Add(mineRockReq);
                }
            }
            return reqList.ToArray();
        }

        /// <summary>
        ///     Adjust the requirements to build so that dropped resources
        ///     are reduced by the average dropAmount dropped by mining
        ///     out the entire MineRock5 component.
        ///     (does not accounts for world modifiers to resource drops yet).
        /// </summary>
        /// <param name="requirements"></param>
        /// <param name="mineRock5"></param>
        /// <returns></returns>
        internal static Piece.Requirement[] RemoveMineRock5DropsFromRequirements(
            Piece.Requirement[] requirements,
            MineRock5 mineRock5
        )
        {
            if (requirements == null || mineRock5 == null)
            {
                return requirements;
            }

            var avgDrops = mineRock5.GetAvgDrops();
            // Make a copy before altering drops
            var newRequirements = new Piece.Requirement[requirements.Length];
            requirements.CopyTo(newRequirements, 0);
            foreach (var drop in avgDrops)
            {
                var dropName = drop.item.m_itemData.m_shared.m_name;
                var dropAmount = (int)Mathf.Round(drop.amount);
                foreach (var req in newRequirements)
                {
                    // If drop is in piece requirements then adjust dropAmount as needed
                    if (req.m_resItem.m_itemData.m_shared.m_name == dropName)
                    {
                        req.m_amount = Mathf.Clamp(req.m_amount - dropAmount, 0, req.m_amount);
                    }
                }
            }
            return newRequirements;
        }
    }
}