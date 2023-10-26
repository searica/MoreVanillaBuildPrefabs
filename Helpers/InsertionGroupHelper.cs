using System;
using System.Collections.Generic;
using UnityEngine;

using MoreVanillaBuildPrefabs.Configs;
using MoreVanillaBuildPrefabs.Logging;

namespace MoreVanillaBuildPrefabs.Helpers
{
    internal class InsertionGroupHelper
    {
        internal Dictionary<PieceGroup, List<GameObject>> PieceGroupDict;

        public InsertionGroupHelper()
        {
            PieceGroupDict = new();
            foreach (PieceGroup group in Enum.GetValues(typeof(PieceGroup)))
            {
                if (group == PieceGroup.None) { continue; }
                PieceGroupDict[group] = new List<GameObject>();
            }
        }

        internal void AddPieces(PieceTable pieceTable)
        {
            // Loop through groups in based on the ordering in PieceGroup Enum
            foreach (PieceGroup group in Enum.GetValues(typeof(PieceGroup)))
            {
                if (group == PieceGroup.None) { continue; }
                var pieceGroup = PieceGroupDict[group];
                foreach (var prefab in pieceGroup)
                {
                    PieceHelper.AddPieceToPieceTable(prefab, pieceTable);
                }
            }
            // need to figure out an insertion point for each Enum group
        }

        internal void Add(PieceDB pieceDB)
        {
            var key = GetPieceGroup(pieceDB);
            Log.LogInfo($"{pieceDB.name}: {key}");
            key = GetPieceGroup(pieceDB.Prefab);
            Log.LogInfo($"{pieceDB.name}: {key}");
            PieceGroupDict[key].Add(pieceDB.Prefab);
        }

        internal static PieceGroup GetPieceGroup(PieceDB pieceDB)
        {
            if (pieceDB.pieceGroup != PieceGroup.None) return pieceDB.pieceGroup;
            return GetPieceGroup(pieceDB.Prefab);
        }

        internal static PieceGroup GetPieceGroup(GameObject prefab)
        {
            var prefabName = prefab.name.ToLower();
            var piece = prefab.GetComponent<Piece>();
            var destructible = prefab.GetComponent<Destructible>();

            if (prefab.HasComponent<PrivateArea>())
            {
                return PieceGroup.Ward;
            }

            if (prefab.HasComponent<Ship>())
            {
                return PieceGroup.Ship;
            }

            if (prefab.HasComponent<Vagon>())
            {
                return PieceGroup.Cart;
            }

            if (prefab.HasComponent<TeleportWorld>())
            {
                return PieceGroup.Portal;
            }

            if (
                prefab.HasAnyComponent(
                "CreatureSpawner",
                "SpawnArea",
                "TriggerSpawner")
            )
            {
                return PieceGroup.Spawner;
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
                return PieceGroup.Crafting;
            }

            if (prefabName.Contains("chest") && prefab.HasComponent<Container>())
            {
                return PieceGroup.Chest;
            }

            if (
                piece?.m_comfortGroup == Piece.ComfortGroup.Fire
                || prefab.transform.FindDeepChild("FireWarmth") != null
                || prefab.HasComponentInChildren<Demister>(true)
            )
            {
                if (prefabName.Contains("brazier")) return PieceGroup.Brazier;
                if (prefabName.ContainsAny("torch", "demister")) return PieceGroup.Torch;
                if (prefabName.Contains("fire") || prefab.HasComponent<Fireplace>()) return PieceGroup.Fire;
            }

            if (prefabName.Contains("armorstand")
                || prefab.HasComponent<ArmorStand>())
            {
                return PieceGroup.ArmorStand;
            }

            if (prefabName.EndsWith("pile") || prefabName.EndsWith("stack"))
            {
                return PieceGroup.Stack;
            }

            if (prefabName.ContainsAny("iron", "rusty")
                && !prefab.HasComponent<CookingStation>())
            {
                return PieceGroup.Iron;
            }

            if (prefab.HasComponent<WearNTear>())
            {
                if (prefabName.Contains("darkwood")) return PieceGroup.Darkwood;
                if (prefabName.Contains("wood")) return PieceGroup.Wood;
                if (prefabName.Contains("stone")) return PieceGroup.Stone;
            }

            if (
                prefab.HasComponent<Chair>()
                || prefabName.ContainsAny("chair", "throne", "bench")
                || piece.m_comfortGroup == Piece.ComfortGroup.Chair
            )
            {
                if (!prefab.HasAnyComponent("CraftingStation", "StationExtension", "Barber"))
                {
                    return PieceGroup.Chair;
                }
            }

            if (
                piece?.m_comfortGroup == Piece.ComfortGroup.Banner
                || prefabName.ContainsAny("banner", "curtain", "drape", "cloth_hanging")
            )
            {
                return PieceGroup.Banner;
            }

            if (
                prefabName.ContainsAny("minerock")
                || prefab.HasComponent<MineRock>()
            )
            {
                return PieceGroup.Ore;
            }

            if (
                piece?.m_comfortGroup == Piece.ComfortGroup.Table ||
                (prefabName.Contains("table")
                && !prefab.HasAnyComponent("CraftingStation", "StationExtension", "MapTable"))
            )
            {
                return PieceGroup.Table;
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
                    && prefab.HasComponent<Pickable>()
                )
            {
                return PieceGroup.Plant;
            }

            if (
                prefabName.ContainsAny(
                    "bush", "root", "shrub", "stubbe", "vines", "tree"
                )
                || prefab.HasAnyComponent("TreeBase", "TreeLog")
                || destructible?.m_destructibleType == DestructibleType.Tree)
            {
                return PieceGroup.Flora;
            }

            if (prefabName.ContainsAny("rock", "cliff"))
            {
                return PieceGroup.Rock;
            }

            if (
                piece?.m_comfortGroup == Piece.ComfortGroup.Carpet
                || prefabName.ContainsAny("rug", "carpet")
            )
            {
                return PieceGroup.Rug;
            }

            if (prefabName.Contains("blackmarble"))
            {
                return PieceGroup.BlackMarble;
            }

            if (prefabName.ContainsAny("dvergr", "dverger"))
            {
                return PieceGroup.Dvergr;
            }

            if (prefabName.Contains("goblin"))
            {
                return PieceGroup.Goblin;
            }

            if (prefabName.Contains("statue"))
            {
                return PieceGroup.Statue;
            }

            return PieceGroup.Misc;
        }
    }
}

//private static string GetPrefabCategory(GameObject prefab)
//{
//    Destructible destructible = prefab.GetComponent<Destructible>();

//    string category = "Extended";
//    if (prefab.HasAnyComponent("Pickable", "PickableItem"))
//    {
//        category = "Pickable";
//    }
//    else if (prefab.HasAnyComponent("Humanoid", "Character", "Leviathan", "RandomFlyingBird", "Fish", "Trader", "Odin", "Valkyrie", "Player"))
//    {
//        category = "NPCs";
//    }
//    else if (prefab.HasAnyComponent("CreatureSpawner", "SpawnArea", "TriggerSpawner"))
//    {
//        category = "Spawners";
//    }
//    else if (prefab.name.ContainsAny("Bush", "Root", "root", "shrub", "stubbe", "vines", "SwampTree")
//      || prefab.HasAnyComponent("TreeBase", "TreeLog") || destructible?.m_destructibleType == DestructibleType.Tree)
//    {
//        category = "Vegetation";
//    }
//    else if (prefab.HasAnyComponent("ArmorStand", "Container", "Fireplace") ||
//      prefab.name.ContainsAny("groundtorch", "brazier", "cloth_hanging", "banner", "table", "chair", "sign", "bed"))
//    {
//        category = "Furniture";
//    }
//    else if (prefab.HasAnyComponent("WearNTear", "Door"))
//    {
//        category = "Building";
//    }
//    else if (prefab.HasAnyComponent("Destructible", "MineRock"))
//    {
//        category = "Destructible";
//    }
//    else if (prefab.GetComponent("ZNetView"))
//    {
//        category = "Other";
//    }

//    int pageNum = itemCounts[category] / NumPiecesPerPage + 1;
//    itemCounts[category]++;
//    if (pageNum > 1)
//    {
//        return $"{category} {pageNum}";
//    }
//    return category;
//}