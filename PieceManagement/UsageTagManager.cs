// Ignore Spelling: MVBP

using MVBP.Extensions;
using MVBP.PrefabManagement;
using System;
using System.Collections.Generic;
using UnityEngine;


namespace MVBP.PieceManagement;

/// <summary>
///     IEnumerable that has a list of prefabs for each PieceGroup
///     and returns the PieceGroups in sorted order when iterated over.
/// </summary>
internal class UsageTagManager
{
    private static readonly Dictionary<string, Piece.UsageTagFlags> Cache = new();

    internal static Piece.UsageTagFlags GetPieceUsage(PrefabConfig prefabConfig)
    {
        if (Cache.ContainsKey(prefabConfig.Name))
        {
            return Cache[prefabConfig.Name];
        }
        Piece.UsageTagFlags result = DetectUsageTag(prefabConfig.Prefab);
        Cache[prefabConfig.Name] = result;
        return result;
    }

    internal static Piece.UsageTagFlags GetPieceUsage(GameObject prefab)
    {
        if (Cache.ContainsKey(prefab.name))
        {
            return Cache[prefab.name];
        }
        Piece.UsageTagFlags result = DetectUsageTag(prefab);
        Cache[prefab.name] = result;
        return result;
    }

    private static Piece.UsageTagFlags DetectUsageTag(GameObject prefab)
    {
        string prefabName = prefab.name.ToLower();

        Piece piece = prefab.GetComponent<Piece>();

        var flags = piece.m_category switch
        {
            Piece.PieceCategory.Misc => Piece.UsageTagFlags.Misc,
            Piece.PieceCategory.Crafting => Piece.UsageTagFlags.Crafting,
            Piece.PieceCategory.BuildingWorkbench => Piece.UsageTagFlags.Building,
            Piece.PieceCategory.BuildingStonecutter => Piece.UsageTagFlags.Building,
            Piece.PieceCategory.Furniture => Piece.UsageTagFlags.Furniture,
            Piece.PieceCategory.DeepNorth => (Piece.UsageTagFlags)0,
            Piece.PieceCategory.Feasts => Piece.UsageTagFlags.Feasts,
            Piece.PieceCategory.Food => Piece.UsageTagFlags.Food,
            Piece.PieceCategory.Meads => Piece.UsageTagFlags.Meads,
            _ => (Piece.UsageTagFlags)0
        };

        // Storage tag
        if ((prefabName.Contains("chest") || prefab.GetComponent<Container>()) || prefab.GetComponent<ItemStand>())
        {
            flags |= Piece.UsageTagFlags.Storage;
        }

        // Doors tag
        if (prefab.GetComponent<Door>())
        {
            flags |= Piece.UsageTagFlags.Doors;
        }

        // Lighting tag
        if (
            piece.m_comfortGroup == Piece.ComfortGroup.Fire
            || piece.GetComponent<Fireplace>()
            || prefabName.ContainsAny("torch", "lantern", "demister")
            || prefab.transform.FindDeepChild("FireWarmth")
            || prefab.GetComponentInChildren<Demister>(true)
        )
        {
            flags |= Piece.UsageTagFlags.Lighting;
        }

        // Crafting tag
        if (piece.m_category != Piece.PieceCategory.Crafting &&
            prefab.HasAnyComponent(
                "CraftingStation",
                "StationExtension",
                "CookingStation",
                "Smelter", // includes windmills
                "Fermenter",
                "Beehive",
                "WispSpawner",
                "SapCollector"
            )
        )
        {
            flags |= Piece.UsageTagFlags.Crafting;
        }

        // Transport tag
        if (prefab.GetComponent<Ship>() || prefab.GetComponent<Vagon>() || piece.GetComponent<TeleportWorld>())
        {
            flags |= Piece.UsageTagFlags.Transport;
        }

        // Decor tag
        if (prefabName.Contains("armorstand") || prefab.GetComponent<ArmorStand>())
        {
            flags |= Piece.UsageTagFlags.Decor;
        }
        else if (
            piece.m_comfortGroup == Piece.ComfortGroup.Banner || prefabName.ContainsAny("banner", "curtain", "drape", "cloth_hanging")
        )
        {
            flags |= Piece.UsageTagFlags.Decor;
        }
        else if (prefabName.Contains("statue"))
        {
            flags |= Piece.UsageTagFlags.Decor;
        }
        else if (piece.m_comfortGroup == Piece.ComfortGroup.Carpet || prefabName.ContainsAny("rug", "carpet"))
        {
            flags |= Piece.UsageTagFlags.Decor;
        }
        else if (piece.GetComponent<ItemStand>()  || piece.GetComponent<Sign>())
        {
            flags |= Piece.UsageTagFlags.Decor;
        }

        // Furniture tag
        if (piece.m_comfort > 0)
        {
            flags |= Piece.UsageTagFlags.Furniture;
        }
        else if (
            (prefab.GetComponent<Chair>() || prefabName.ContainsAny("chair", "throne", "bench", "stool") || piece.m_comfortGroup == Piece.ComfortGroup.Chair) &&
            !prefab.HasAnyComponent("CraftingStation", "StationExtension")
        )
        {
            flags |= Piece.UsageTagFlags.Furniture;
        }
        else if (
            piece.m_comfortGroup == Piece.ComfortGroup.Table ||
            (prefabName.Contains("table") && !prefab.HasAnyComponent("CraftingStation", "StationExtension", "MapTable"))
        )
        {
            flags |= Piece.UsageTagFlags.Furniture;
        }
        else if (prefab.GetComponent<Bed>() || prefabName.Contains("bed"))
        {
            flags |= Piece.UsageTagFlags.Furniture;
        }

        // Stairs tag
        if (prefabName.ContainsAny("stair", "ladder", "ramp"))
        {
            flags |= Piece.UsageTagFlags.Stairs;
        }

        // Wall tag
        if (prefabName.ContainsAny("wall", "window", "fence"))
        {
            flags |= Piece.UsageTagFlags.Wall;
        }

        // Stacks tag
        if (prefabName.EndsWith("pile") || prefabName.EndsWith("stack"))
        {
            flags |= Piece.UsageTagFlags.Stacks;
        }

        // Floor tag
        if (prefabName.ContainsAny("floor", "slap"))
        {
            flags |= Piece.UsageTagFlags.Floor;
        }

        // Roof tag
        // || (prefab.TryGetComponent(out WearNTear roof) && roof.m_noSupportWear)
        if (prefabName.Contains("roof"))
        {
            flags |= Piece.UsageTagFlags.Roof;
        }

        // Building tag
        // prefab.GetComponent<WearNTear>()
        if (prefabName.ContainsAny("beam", "pole", "pillar", "arch", "sharpstakes"))
        {
            flags |= Piece.UsageTagFlags.Building;
        }

        // Architecture tag
        if (prefabName.ContainsAny("pole", "beam", "cross", "pillar"))
        {
            flags |= Piece.UsageTagFlags.Architecture;
        }

        // Defense tag
        if (prefab.HasAnyComponent("PrivateArea", "ShieldGenerator", "Turret") || prefabName.ContainsAny("stakes", "spikes"))
        {
            flags |= Piece.UsageTagFlags.Defense;
        }

        return flags;
    }
}
