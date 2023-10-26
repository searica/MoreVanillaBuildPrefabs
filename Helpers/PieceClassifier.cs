using System;
using System.Collections.Generic;
using UnityEngine;

using MoreVanillaBuildPrefabs.Configs;
using MoreVanillaBuildPrefabs.Logging;

namespace MoreVanillaBuildPrefabs.Helpers
{
    internal class PieceClassifier
    {
        private static readonly Dictionary<string, PieceGroup> Cache = new();

        internal Dictionary<PieceGroup, List<GameObject>> PieceGroupDict;

        internal static PieceGroup GetPieceGroup(PieceDB pieceDB)
        {
            if (Cache.ContainsKey(pieceDB.name))
            {
                return Cache[pieceDB.name];
            }
            if (pieceDB.pieceGroup != PieceGroup.None)
            {
                Cache[pieceDB.name] = pieceDB.pieceGroup;
                return pieceDB.pieceGroup;
            }
            var result = _GetPieceGroup(pieceDB.Prefab);
            Cache[pieceDB.name] = result;
            return result;
        }

        internal static PieceGroup GetPieceGroup(GameObject prefab)
        {
            if (Cache.ContainsKey(prefab.name))
            {
                return Cache[prefab.name];
            }
            var result = _GetPieceGroup(prefab);
            Cache[prefab.name] = result;
            return result;
        }

        private static PieceGroup _GetPieceGroup(GameObject prefab)
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

            if (prefab.HasComponent<Bed>() || prefabName.Contains("bed"))
            {
                return PieceGroup.Bed;
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

            if (prefabName.ContainsAny("dvergr", "dverger"))
            {
                return PieceGroup.Dvergr;
            }

            if (prefab.HasAnyComponent(typeof(WearNTear), typeof(Door)))
            {
                if (prefabName.Contains("darkwood")) return PieceGroup.Darkwood;
                if (prefabName.ContainsAny("wood", "turf")) return PieceGroup.Wood;
                if (prefabName.Contains("stone")) return PieceGroup.Stone;
            }

            if (
                prefab.HasComponent<Chair>()
                || prefabName.ContainsAny("chair", "throne", "bench", "stool")
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
                piece?.m_comfortGroup == Piece.ComfortGroup.Carpet
                || prefabName.ContainsAny("rug", "carpet")
            )
            {
                return PieceGroup.Rug;
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

            if (prefabName.Contains("ice"))
            {
                return PieceGroup.Ice;
            }

            if (prefabName.ContainsAny("rock", "cliff"))
            {
                return PieceGroup.Rock;
            }

            if (prefabName.Contains("blackmarble"))
            {
                return PieceGroup.BlackMarble;
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