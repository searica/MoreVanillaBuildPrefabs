// Ignore Spelling: MVBP

using MVBP.Extensions;
using MVBP.PrefabManagement;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MVBP.PieceManagement;

/// <summary>
///     IEnumerable that has a list of prefabs for each PieceGroup
///     and returns the PieceGroups in sorted order when iterated over.
/// </summary>
internal class SortedPieceGroups : IEnumerable
{
    private readonly Dictionary<PieceClassification, List<GameObject>> pieceGroupLists;
    private static readonly List<PieceClassification> _pieceGroupOrder = new();
    private static List<PieceClassification> PieceGroupOrder => GetPieceGroupOrder();

    private static List<PieceClassification> GetPieceGroupOrder()
    {
        if (_pieceGroupOrder.Count > 0)
        {
            return _pieceGroupOrder;
        }

        foreach (PieceClassification pieceGroup in Enum.GetValues(typeof(PieceClassification)))
        {
            if (pieceGroup == PieceClassification.None) { continue; }
            _pieceGroupOrder.Add(pieceGroup);
        }
        return _pieceGroupOrder;
    }

    /// <summary>
    ///     IEnumerable that has a list of prefabs for each PieceGroup
    ///     and returns the PieceGroups in sorted order when iterated over.
    /// </summary>
    public SortedPieceGroups()
    {
        pieceGroupLists = new Dictionary<PieceClassification, List<GameObject>>();
        foreach (PieceClassification group in PieceGroupOrder)
        {
            pieceGroupLists[group] = new List<GameObject>();
        }
    }

    public void Add(PrefabConfig prefabConfig)
    {
        PieceClassification key = PieceClassifier.GetPieceGroup(prefabConfig);
        pieceGroupLists[key].Add(prefabConfig.Prefab);
    }

    public IEnumerator GetEnumerator()
    {
        return new SortedPieceGroupsEnumerator(pieceGroupLists);
    }

    private class SortedPieceGroupsEnumerator : IEnumerator
    {
        private int position = -1;
        private readonly Dictionary<PieceClassification, List<GameObject>> pieceGroupLists;

        public SortedPieceGroupsEnumerator(Dictionary<PieceClassification, List<GameObject>> pieceGroupLists)
        {
            this.pieceGroupLists = pieceGroupLists;
        }

        public bool MoveNext()
        {
            position++;
            return position < PieceGroupOrder.Count;
        }

        public void Reset()
        {
            position = -1;
        }

        public object Current
        {
            get
            {
                try
                {
                    return pieceGroupLists[PieceGroupOrder[position]];
                }
                catch (IndexOutOfRangeException)
                {
                    throw new InvalidOperationException();
                }
            }
        }
    }
}

internal class PieceClassifier
{
    private static readonly Dictionary<string, PieceClassification> Cache = new();

    internal static PieceClassification GetPieceGroup(PrefabConfig prefabConfig)
    {
        if (Cache.ContainsKey(prefabConfig.Name))
        {
            return Cache[prefabConfig.Name];
        }
        if (prefabConfig.PieceGroup != PieceClassification.None)
        {
            Cache[prefabConfig.Name] = prefabConfig.PieceGroup;
            return prefabConfig.PieceGroup;
        }
        PieceClassification result = DetectPieceGroup(prefabConfig.Prefab);
        Cache[prefabConfig.Name] = result;
        return result;
    }

    internal static PieceClassification GetPieceGroup(GameObject prefab)
    {
        if (Cache.ContainsKey(prefab.name))
        {
            return Cache[prefab.name];
        }
        PieceClassification result = DetectPieceGroup(prefab);
        Cache[prefab.name] = result;
        return result;
    }

    private static PieceClassification DetectPieceGroup(GameObject prefab)
    {
        string prefabName = prefab.name.ToLower();
        Piece piece = prefab.GetComponent<Piece>();
        Destructible destructible = prefab.GetComponent<Destructible>();

        if (prefab.GetComponent<PrivateArea>())
        {
            return PieceClassification.Ward;
        }

        if (prefab.GetComponent<Ship>())
        {
            return PieceClassification.Ship;
        }

        if (prefab.GetComponent<Vagon>())
        {
            return PieceClassification.Cart;
        }

        if (prefab.GetComponent<TeleportWorld>())
        {
            return PieceClassification.Portal;
        }

        if (prefab.GetComponent<Bed>() || prefabName.Contains("bed"))
        {
            return PieceClassification.Bed;
        }

        if (
            prefab.HasAnyComponent(
            "CreatureSpawner",
            "SpawnArea",
            "TriggerSpawner")
        )
        {
            return PieceClassification.Spawner;
        }

        if (prefab.HasAnyComponent(
                "CraftingStation",
                "StationExtension",
                "CookingStation",
                "Smelter", // includes windmills
                "WispSpawner"
            )
        )
        {
            return PieceClassification.Crafting;
        }

        if (prefabName.Contains("chest") && prefab.GetComponent<Container>())
        {
            return PieceClassification.Chest;
        }

        if (
            piece?.m_comfortGroup == Piece.ComfortGroup.Fire
            || prefab.transform.FindDeepChild("FireWarmth") != null
            || prefab.GetComponentInChildren<Demister>(true)
        )
        {
            if (prefabName.Contains("brazier"))
            {
                return PieceClassification.Brazier;
            }

            if (prefabName.ContainsAny("torch", "demister"))
            {
                return PieceClassification.Torch;
            }

            if (prefabName.Contains("fire") || prefab.GetComponent<Fireplace>())
            {
                return PieceClassification.Fire;
            }
        }

        if (prefabName.Contains("armorstand")
            || prefab.GetComponent<ArmorStand>())
        {
            return PieceClassification.ArmorStand;
        }

        if (prefabName.EndsWith("pile") || prefabName.EndsWith("stack"))
        {
            return PieceClassification.Stack;
        }

        if (prefabName.ContainsAny("iron", "rusty")
            && !prefab.GetComponent<CookingStation>())
        {
            return PieceClassification.Iron;
        }

        if (prefabName.ContainsAny("dvergr", "dverger"))
        {
            return PieceClassification.Dvergr;
        }

        if (prefab.HasAnyComponent(typeof(WearNTear), typeof(Door)))
        {
            if (prefabName.Contains("darkwood"))
            {
                return PieceClassification.Darkwood;
            }

            if (prefabName.Contains("ashwood"))
            {
                return PieceClassification.Ashwood;
            }

            if (prefabName.ContainsAny("wood", "turf"))
            {
                return PieceClassification.Wood;
            }

            if (prefabName.Contains("stone"))
            {
                return PieceClassification.Stone;
            }
        }

        if (
            prefab.GetComponent<Chair>()
            || prefabName.ContainsAny("chair", "throne", "bench", "stool")
            || piece.m_comfortGroup == Piece.ComfortGroup.Chair
        )
        {
            if (!prefab.HasAnyComponent("CraftingStation", "StationExtension", "Barber"))
            {
                return PieceClassification.Chair;
            }
        }

        if (
            piece?.m_comfortGroup == Piece.ComfortGroup.Banner
            || prefabName.ContainsAny("banner", "curtain", "drape", "cloth_hanging")
        )
        {
            return PieceClassification.Banner;
        }

        if (
            prefabName.ContainsAny("minerock")
            || prefab.GetComponent<MineRock>()
        )
        {
            return PieceClassification.Ore;
        }

        if (
            piece?.m_comfortGroup == Piece.ComfortGroup.Table ||
            (prefabName.Contains("table")
            && !prefab.HasAnyComponent("CraftingStation", "StationExtension", "MapTable"))
        )
        {
            return PieceClassification.Table;
        }

        if (
                prefabName.ContainsAny(
                    "onion",
                    "carrot",
                    "turnip",
                    "mushroom",
                    "barley",
                    "flax",
                    "berry",
                    "thistle",
                    "dandelion"
                )
                && prefab.GetComponent<Pickable>()
            )
        {
            return PieceClassification.Plant;
        }

        if (
            piece?.m_comfortGroup == Piece.ComfortGroup.Carpet
            || prefabName.ContainsAny("rug", "carpet")
        )
        {
            return PieceClassification.Rug;
        }

        if (
            prefabName.ContainsAny(
                "bush", "root", "shrub", "stubbe", "vines", "tree"
            )
            || prefab.HasAnyComponent("TreeBase", "TreeLog")
            || destructible?.m_destructibleType == DestructibleType.Tree)
        {
            return PieceClassification.Flora;
        }

        if (prefabName.Contains("ice"))
        {
            return PieceClassification.Ice;
        }

        if (prefabName.ContainsAny("rock", "cliff"))
        {
            return PieceClassification.Rock;
        }

        if (prefabName.Contains("blackmarble"))
        {
            return PieceClassification.BlackMarble;
        }

        if (prefabName.Contains("goblin"))
        {
            return PieceClassification.Goblin;
        }

        if (prefabName.Contains("statue"))
        {
            return PieceClassification.Statue;
        }

        return PieceClassification.Misc;
    }
}
