// Ignore Spelling: MVBP

using BepInEx.Configuration;
using Jotunn.Managers;
using Logging;
using MVBP.Extensions;
using MVBP.PrefabManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MVBP.PieceManagement;

internal static class PieceCategoryManager
{

    /// <summary>
    ///     Custom category name for pieces that aren't other categorized
    /// </summary>
    public const string CreatorShop = "CreatorShop";

    /// <summary>
    ///     Custom category name for pieces that are part of nature
    /// </summary>
    public const string Nature = "Nature";

    /// <summary>
    ///     Custom category name for pieces that are pickable plants.
    /// </summary>
    public const string Plants = "Plants";

    /// <summary>
    ///     Custom category name for pieces that make up the flora.
    /// </summary>
    public const string Flora = "Flora";

    /// <summary>
    ///     Custom category name for pieces that are rocks.
    /// </summary>
    public const string Rocks = "Rocks";

    /// <summary>
    ///     Custom category name for pieces that are ores.
    /// </summary>
    public const string Ore = "Ore";

    /// <summary>
    ///     Custom category name for pieces that are made of ice.
    /// </summary>
    public const string Ice = "Ice";

    /// <summary>
    ///     Custom category name for pieces that spawn creatures.
    /// </summary>
    public const string Spawner = "Spawner";

    /// <summary>
    ///     Default category for misc
    /// </summary>
    public const string Misc = "Misc";

    /// <summary>
    ///     Default category for crafting pieces
    /// </summary>
    public const string Crafting = "Crafting";

    /// <summary>
    ///     Building Category
    /// </summary>
    public const string Building = "BuildingWorkbench";

    /// <summary>
    ///     Heavy build category.
    /// </summary>
    public const string Stonecutter = "BuildingStonecutter";

    /// <summary>
    ///     Furniture category.
    /// </summary>
    public const string Furniture = "Furniture";

    /// <summary>
    ///     Deep north pieces?
    /// </summary>
    public const string DeepNorth = "DeepNorth";

    public const string Feasts = "Feasts";
    public const string Food = "Food";
    public const string Meads = "Meads";

    internal static AcceptableValueList<string> GetAcceptableValueList()
    {
        return new AcceptableValueList<string>(typeof(PieceCategoryManager).GetAllPublicConstantValues<string>().ToArray());
    }


    private static readonly Dictionary<string, Piece.PieceCategory> CustomCategories = new() {
        {CreatorShop, 0},
        {Nature, 0},
        {Plants, 0},
        {Flora, 0},
        {Rocks, 0},
        {Ore, 0},
        {Ice, 0},
        {Spawner, 0},
    };

    public static Piece.PieceCategory CreatorShopCategory => CustomCategories[CreatorShop];
    public static Piece.PieceCategory NatureCategory => CustomCategories[Nature];
    public static Piece.PieceCategory PlantsCategory => CustomCategories[Plants];
    public static Piece.PieceCategory FloraCategory => CustomCategories[Flora];
    public static Piece.PieceCategory RockCategory => CustomCategories[Rocks];
    public static Piece.PieceCategory OreCategory => CustomCategories[Ore];
    public static Piece.PieceCategory IceCategory => CustomCategories[Ice];
    public static Piece.PieceCategory SpawnerCategory => CustomCategories[Spawner];

    private static HashSet<Piece.PieceCategory> CreativeModeCategories = new();


    internal static void AddCustomPieceCategories()
    { 
        foreach (string name in PieceCategoryManager.CustomCategories.Keys.ToArray())
        {
            if (PieceManager.Instance.GetPieceCategory(name) == null)
            {
                Log.LogInfo($"Adding custom piece category {name}", Log.InfoLevel.Medium);
                Piece.PieceCategory customCategory = PieceManager.Instance.AddPieceCategory(name);
                CustomCategories[name] = customCategory;
                CreativeModeCategories.Add(customCategory);
            }
        }
    }

    internal static bool IsCreativeModePiece(Piece piece)
    {
        return MVPrefabManager.IsPatchedByMVBP(piece) && (CreativeModeCategories.Contains(piece.m_category));
    }

    internal static bool IsCreatorShopPiece(Piece piece)
    {
        return MVPrefabManager.IsPatchedByMVBP(piece) && piece.m_category == CreatorShopCategory;
    }

    internal static bool IsNaturePiece(Piece piece)
    {
        return MVPrefabManager.IsPatchedByMVBP(piece) && piece.m_category == NatureCategory;
    }

    /// <summary>
    ///     Safely search for piece category and fall back to "Misc" if
    ///     it cannot be found.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    internal static Piece.PieceCategory GetPieceCategory(string name)
    {
        try
        {
            return (Piece.PieceCategory)PieceManager.Instance.GetPieceCategory(name);
        }
        catch (InvalidOperationException)
        {
            Log.LogWarning($"Could not find value for Piece Category: {name}");
            return Piece.PieceCategory.Misc;
        }
    }
}
