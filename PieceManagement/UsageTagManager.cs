// Ignore Spelling: MVBP

using MVBP.Extensions;
using MVBP.PrefabManagement;
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
        Destructible destructible = prefab.GetComponent<Destructible>();

        List<Piece.UsageTagFlags> tags = new();

        // Crafting tag
        if (prefab.HasAnyComponent(
                "CraftingStation",
                "StationExtension",
                "CookingStation",
                "Smelter", // includes windmills
                "WispSpawner"
            )
        )
        {
            tags.Add(Piece.UsageTagFlags.Crafting);
        }

        // Building tag
        if (prefab.GetComponent<WearNTear>())
        {
            tags.Add(Piece.UsageTagFlags.Building);
        }

        // Floor tag
        if (prefabName.Contains("floor"))
        {
            tags.Add(Piece.UsageTagFlags.Floor);
        }

        // Wall tag
        if (prefabName.Contains("wall"))
        {
            tags.Add(Piece.UsageTagFlags.Wall);
        }

        // Roof tag
        if (prefabName.Contains("roof") || (prefab.TryGetComponent(out WearNTear roof) && roof.m_noSupportWear))
        {
            tags.Add(Piece.UsageTagFlags.Roof);
        }

        // Architecture tag
        if (prefabName.ContainsAny("pole", "beam", "cross"))
        {
            tags.Add(Piece.UsageTagFlags.Architecture);
        }

        // Furniture tag
        if (
            (prefab.GetComponent<Chair>() ||
            prefabName.ContainsAny("chair", "throne", "bench", "stool") ||
            piece.m_comfortGroup == Piece.ComfortGroup.Chair) &&
            !prefab.HasAnyComponent("CraftingStation", "StationExtension", "Barber")
        )
        {
            tags.Add(Piece.UsageTagFlags.Furniture);
        }
        else if (
            piece?.m_comfortGroup == Piece.ComfortGroup.Table ||
            (prefabName.Contains("table") &&
            !prefab.HasAnyComponent("CraftingStation", "StationExtension", "MapTable"))
        )
        {
            tags.Add(Piece.UsageTagFlags.Furniture);
        }
        else if (prefab.GetComponent<Bed>() || prefabName.Contains("bed"))
        {
            tags.Add(Piece.UsageTagFlags.Furniture);
        }

        // Lighting tag
        if (
            piece?.m_comfortGroup == Piece.ComfortGroup.Fire
            || prefab.transform.FindDeepChild("FireWarmth") != null
            || prefab.GetComponentInChildren<Demister>(true)
        )
        {
            tags.Add(Piece.UsageTagFlags.Lighting);
        }

        // Decor tag
        if (prefabName.Contains("armorstand") || prefab.GetComponent<ArmorStand>())
        {
            tags.Add(Piece.UsageTagFlags.Decor);
        }
        else if (
            piece?.m_comfortGroup == Piece.ComfortGroup.Banner || prefabName.ContainsAny("banner", "curtain", "drape", "cloth_hanging")
        )
        {
            tags.Add(Piece.UsageTagFlags.Decor);
        }
        else if (prefabName.Contains("statue"))
        {
            tags.Add(Piece.UsageTagFlags.Decor);
        }
        else if (piece?.m_comfortGroup == Piece.ComfortGroup.Carpet || prefabName.ContainsAny("rug", "carpet"))
        {
            tags.Add(Piece.UsageTagFlags.Decor);
        }
        else if (piece.GetComponent<ItemStand>())
        {
            tags.Add(Piece.UsageTagFlags.Decor);
        }

        // Storage tag
        if ((prefabName.Contains("chest") && prefab.GetComponent<Container>()) || prefab.GetComponent<ItemStand>())
        {
            tags.Add(Piece.UsageTagFlags.Storage);
        }

        // Transport tag
        if (prefab.GetComponent<Ship>() || prefab.GetComponent<Vagon>())
        {
            tags.Add(Piece.UsageTagFlags.Transport);
        }

        // Food Tag

        // Mead tag

        // Feasts Tag

        // Defense tag
        if (prefab.HasAnyComponent("PrivateArea", "ShieldGenerator"))
        {
            tags.Add(Piece.UsageTagFlags.Defense);
        }

        // Stacks tag
        if (prefabName.EndsWith("pile") || prefabName.EndsWith("stack"))
        {
            tags.Add(Piece.UsageTagFlags.Stacks);
        }

        // Stairs tag
        if (prefabName.Contains("stair"))
        {
            tags.Add(Piece.UsageTagFlags.Stairs);
        }

        // Doors tag
        if (prefab.GetComponent<Door>())
        {
            tags.Add(Piece.UsageTagFlags.Doors);
        }

        // Seasonal tag


        //if (prefab.GetComponent<TeleportWorld>())
        //{
        //    return PieceClassification.Portal;
        //}

        if (tags.Count == 0)
        {
            return Piece.UsageTagFlags.Misc;
        }

        if (tags.Count == 1)
        {
            return tags[0];
        }

        Piece.UsageTagFlags result = tags[0];
        for (int i = 1; i < tags.Count; i++) 
        {
            result = result | tags[i];
        }
        return result;
    }
}
