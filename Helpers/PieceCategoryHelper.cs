// Ignore Spelling: MVBP

using Jotunn.Configs;
using Jotunn.Managers;
using MVBP.Configs;

namespace MVBP.Helpers
{
    internal static class PieceCategoryHelper
    {
        private static Piece.PieceCategory Nature;
        private static Piece.PieceCategory CreatorShop;

        internal static void AddCreatorShopPieceCategory()
        {
            if (PieceManager.Instance.GetPieceCategory(HammerCategories.Nature) == null
                || PieceManager.Instance.GetPieceCategory(HammerCategories.CreatorShop) == null)
            {
                Log.LogInfo("Adding custom piece categories", LogLevel.Medium);
                Nature = PieceManager.Instance.AddPieceCategory(PieceTables.Hammer, HammerCategories.Nature);
                CreatorShop = PieceManager.Instance.AddPieceCategory(PieceTables.Hammer, HammerCategories.CreatorShop);
            }
        }

        internal static void RemoveCreatorShopPieceCategory()
        {
            Log.LogInfo("Removing custom piece categories", LogLevel.Medium);
            PieceManager.Instance.RemovePieceCategory(PieceTables.Hammer, HammerCategories.CreatorShop);
        }

        internal static bool IsCreativeModePiece(Piece piece)
        {
            return IsCreatorShopPiece(piece) || IsNaturePiece(piece);
        }

        internal static bool IsCreatorShopPiece(Piece piece)
        {
            if (InitManager.IsPatchedByMod(piece) && piece.m_category == CreatorShop)
            {
                return true;
            }

            return false;
        }

        internal static bool IsNaturePiece(Piece piece)
        {
            if (InitManager.IsPatchedByMod(piece) && piece.m_category == Nature)
            {
                return true;
            }

            return false;
        }
    }
}