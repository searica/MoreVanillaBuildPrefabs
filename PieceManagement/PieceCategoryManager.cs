// Ignore Spelling: MVBP

using Jotunn.Managers;
using Logging;
using MVBP.PrefabManagement;
using System;

namespace MVBP.PieceManagement;

internal static class PieceCategoryManager
{
    private static Piece.PieceCategory Nature;
    private static Piece.PieceCategory CreatorShop;

    internal static void AddCustomPieceCategories()
    {
        foreach (string name in PieceCategoryNames.GetCustomCategoryNames())
        {
            if (PieceManager.Instance.GetPieceCategory(name) == null)
            {
                Log.LogInfo($"Adding custom piece category {name}", Log.InfoLevel.Medium);
                Piece.PieceCategory customCategory = PieceManager.Instance.AddPieceCategory(name);
                switch (name)
                {
                    case "Nature":
                        Nature = customCategory;
                        break;
                    case "CreatorShop":
                        CreatorShop = customCategory;
                        break;
                    default:
                        break;
                }
            }
        }
    }

    internal static void RemoveCreatorShopPieceCategory()
    {
        Log.LogInfo("Removing custom piece categories", Log.InfoLevel.Medium);
        PieceManager.Instance.RemovePieceCategory(PieceCategoryNames.CreatorShop);
    }

    internal static bool IsCreativeModePiece(Piece piece)
    {
        return ZNetPrefabManager.IsPatchedByMVBP(piece) && (piece.m_category == CreatorShop || piece.m_category == Nature);
    }

    internal static bool IsCreatorShopPiece(Piece piece)
    {
        return ZNetPrefabManager.IsPatchedByMVBP(piece) && piece.m_category == CreatorShop;
    }

    internal static bool IsNaturePiece(Piece piece)
    {
        return ZNetPrefabManager.IsPatchedByMVBP(piece) && piece.m_category == Nature;
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
