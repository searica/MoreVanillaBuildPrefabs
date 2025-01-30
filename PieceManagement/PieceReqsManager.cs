// Ignore Spelling: MVBP

using Jotunn.Configs;
using Jotunn.Managers;
using Logging;
using MVBP.Extensions;
using MVBP.PrefabManagement;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MVBP.PieceManagement;


/// <summary>
///     Handles modifying piece requirements.
/// </summary>
internal static class PieceReqsManager
{

    /// <summary>
    ///     Create piece requirements array from pieceDB and modify it to prevent
    ///     exploits if the piece has a pickable component.
    /// </summary>
    /// <param name="pieceDB"></param>
    /// <returns></returns>
    public static Piece.Requirement[] ConfigurePieceRequirements(PrefabConfig prefabConfig)
    {
        var reqs = CreateRequirementsArray(prefabConfig.ReadRequirements());

        if (prefabConfig.Piece.TryGetComponent(out MineRock mineRock))
        {
            reqs = AddMineRockDropsToRequirements(reqs, mineRock);
        }
       
        if (prefabConfig.Piece.TryGetComponent(out MineRock5 mineRock5))
        {
            reqs = AddMineRock5DropsToRequirements(reqs, mineRock5);
        }

        if (prefabConfig.Piece.TryGetComponent(out Pickable pickable))
        {
            reqs = AddPickableToRequirements(reqs, pickable);
        }
        return reqs;
    }

    /// <summary>
    ///     Convert List<RequirementConfig> from deserialized requirement config string to Piece.Requirement Array
    /// </summary>
    /// <param name="data"></param>
    /// <returns></returns>
    internal static Piece.Requirement[] CreateRequirementsArray(List<RequirementConfig> reqConfigs)
    {
        var requirements = new List<Piece.Requirement>();
        foreach (RequirementConfig reqConfig in reqConfigs)
        {
            if (reqConfig.Item == " ")
            {
                continue;
            }

            ItemDrop itm = ObjectDB.instance.GetItemPrefab(reqConfig.Item)?.GetComponent<ItemDrop>();
            if (!itm)
            {
                Log.LogWarning($"Unable to find requirement ID: {reqConfig.Item}");
                continue;
            }
            Piece.Requirement pieceReq = new()
            {
                m_resItem = itm,
                m_amount = reqConfig.Amount,
                m_recover = true
            };
            requirements.Add(pieceReq);
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
        if (requirements == null ||
            !pickable ||
            !pickable.m_itemPrefab ||
            !pickable.m_itemPrefab.TryGetComponent(out ItemDrop pickableItem) ||
            !pickableItem ||
            pickableItem.m_itemData == null
        )
        {
            return requirements;
        }

        // Get dropAmount returned on picking based on world modifiers
        int pickedAmount = pickable.GetScaledDropAmount();

        // Check if pickable is included in piece build requirements
        foreach (Piece.Requirement req in requirements)
        {
            if (req.m_resItem.m_itemData.m_shared.m_name == pickableItem.m_itemData.m_shared.m_name)
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
            m_resItem = pickableItem,
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
        // If not pickable or has not been picked them do no alter drops
        if (requirements == null || !pickable || !pickable.m_picked)
        {
            return requirements;
        }

        // If drops not drop a pickable item then do not alter drops
        if (!pickable.m_itemPrefab || !pickable.m_itemPrefab.TryGetComponent(out ItemDrop pickableItem))
        {
            return requirements;
        }

        // Check for valid pickable drop
        ItemDrop.ItemData pickableDrop = pickableItem.m_itemData;
        if (pickableDrop == null)
        {
            return requirements;
        }

        // Check if pickable is included in piece build requirements
        for (int i = 0; i < requirements.Length; i++)
        {
            Piece.Requirement req = requirements[i];
            if (req.m_resItem.m_itemData.m_shared.m_name == pickableDrop.m_shared.m_name)
            {
                // Make a copy before altering drops
                var pickedRequirements = new Piece.Requirement[requirements.Length];
                requirements.CopyTo(pickedRequirements, 0);

                // Get dropAmount returned on picking based on world modifiers
                int pickedAmount = pickable.GetScaledDropAmount();

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

        List<DropTableExtensions.AvgItemDrop> avgDrops = mineRock.GetAvgDrops();
        var reqList = requirements.ToList();
        foreach (DropTableExtensions.AvgItemDrop drop in avgDrops)
        {
            string dropName = drop.item.m_itemData.m_shared.m_name;
            int dropAmount = (int)Mathf.Round(drop.amount);
            bool inReqs = false;
            foreach (Piece.Requirement req in reqList)
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

        List<DropTableExtensions.AvgItemDrop> avgDrops = mineRock.GetAvgDrops();
        // Make a copy before altering drops
        var newRequirements = new Piece.Requirement[requirements.Length];
        requirements.CopyTo(newRequirements, 0);
        foreach (DropTableExtensions.AvgItemDrop drop in avgDrops)
        {
            string dropName = drop.item.m_itemData.m_shared.m_name;
            int dropAmount = (int)Mathf.Round(drop.amount);
            foreach (Piece.Requirement req in newRequirements)
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

        List<DropTableExtensions.AvgItemDrop> avgDrops = mineRock5.GetAvgDrops();
        var reqList = requirements.ToList();
        foreach (DropTableExtensions.AvgItemDrop drop in avgDrops)
        {
            string dropName = drop.item.m_itemData.m_shared.m_name;
            int dropAmount = (int)Mathf.Round(drop.amount);
            bool inReqs = false;
            foreach (Piece.Requirement req in reqList)
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

        List<DropTableExtensions.AvgItemDrop> avgDrops = mineRock5.GetAvgDrops();
        // Make a copy before altering drops
        var newRequirements = new Piece.Requirement[requirements.Length];
        requirements.CopyTo(newRequirements, 0);
        foreach (DropTableExtensions.AvgItemDrop drop in avgDrops)
        {
            string dropName = drop.item.m_itemData.m_shared.m_name;
            int dropAmount = (int)Mathf.Round(drop.amount);
            foreach (Piece.Requirement req in newRequirements)
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
