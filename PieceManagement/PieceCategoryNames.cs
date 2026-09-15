// Ignore Spelling: MVBP

using BepInEx.Configuration;
using MVBP.Extensions;
using System.Collections.Generic;


namespace MVBP.PieceManagement;

/// <summary>
///     Helper class to get the names of hammer piece categories for this mod.
/// </summary>
internal static class PieceCategoryNames
{
    /// <summary>
    ///     Custom category for pieces that aren't other categorized
    /// </summary>
    public const string CreatorShop = "CreatorShop";
    
    /// <summary>
    ///     Custom category for peices that are part of nature
    /// </summary>
    public const string Nature = "Nature";
    
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

    private readonly static string[] CustomCategoryNames = { CreatorShop, Nature };

    internal static AcceptableValueList<string> GetAcceptableValueList()
    {
        return new AcceptableValueList<string>(typeof(PieceCategoryNames).GetAllPublicConstantValues<string>().ToArray());
    }

    internal static string[] GetCustomCategoryNames()
    {
        return CustomCategoryNames;
    }
}
