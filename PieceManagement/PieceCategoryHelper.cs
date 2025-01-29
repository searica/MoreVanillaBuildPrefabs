// Ignore Spelling: MVBP

using Jotunn.Configs;
using Jotunn.Managers;
using MVBP.Models;

namespace MVBP.SnapPoints;

internal static class PieceCategoryHelper
{
    private static Piece.PieceCategory Nature;
    private static Piece.PieceCategory CreatorShop;

    [System.Obsolete]
    internal static void AddCreatorShopPieceCategory()
    {
        if (PieceManager.Instance.GetPieceCategory(HammerCategories.Nature) == null
            || PieceManager.Instance.GetPieceCategory(HammerCategories.CreatorShop) == null)
        {
            Log.LogInfo("Adding custom piece categories", Log.InfoLevel.Medium);
            Nature = PieceManager.Instance.AddPieceCategory(PieceTables.Hammer, HammerCategories.Nature);
            CreatorShop = PieceManager.Instance.AddPieceCategory(PieceTables.Hammer, HammerCategories.CreatorShop);
        }
    }

    [System.Obsolete]
    internal static void RemoveCreatorShopPieceCategory()
    {
        Log.LogInfo("Removing custom piece categories", Log.InfoLevel.Medium);
        PieceManager.Instance.RemovePieceCategory(PieceTables.Hammer, HammerCategories.CreatorShop);
    }

    internal static bool IsCreativeModePiece(Piece piece)
    {
        return IsCreatorShopPiece(piece) || IsNaturePiece(piece);
    }

    internal static bool IsCreatorShopPiece(Piece piece)
    {
        if (UpdateController.IsPatchedByMod(piece) && piece.m_category == CreatorShop)
        {
            return true;
        }

        return false;
    }

    internal static bool IsNaturePiece(Piece piece)
    {
        if (UpdateController.IsPatchedByMod(piece) && piece.m_category == Nature)
        {
            return true;
        }

        return false;
    }
}
