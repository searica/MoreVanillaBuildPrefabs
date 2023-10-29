using Jotunn.Configs;
using Jotunn.Managers;
using MVBP.Configs;

namespace MVBP.Helpers
{
    internal class PieceCategoryHelper
    {
        internal static Piece.PieceCategory Nature;
        internal static Piece.PieceCategory CreatorShop;

        internal static void AddCreatorShopPieceCategory()
        {
            if (PieceManager.Instance.GetPieceCategory(HammerCategories.Nature) == null
                || PieceManager.Instance.GetPieceCategory(HammerCategories.CreatorShop) == null)
            {
                if (Config.IsVerbosityMedium)
                {
                    Log.LogInfo("Adding custom piece categories");
                }
                Nature = PieceManager.Instance.AddPieceCategory(PieceTables.Hammer, HammerCategories.Nature);
                CreatorShop = PieceManager.Instance.AddPieceCategory(PieceTables.Hammer, HammerCategories.CreatorShop);
            }
        }

        internal static void RemoveCreatorShopPieceCategory()
        {
            if (Config.IsVerbosityMedium)
            {
                Log.LogInfo("Removing custom piece categories");
            }
            PieceManager.Instance.RemovePieceCategory(PieceTables.Hammer, HammerCategories.CreatorShop);
        }

        internal static bool IsCreativeModePiece(Piece piece)
        {
            return IsCreatorShopPiece(piece) || IsNaturePiece(piece);
        }

        internal static bool IsCreatorShopPiece(Piece piece)
        {
            var pieceName = NameHelper.GetRootPrefabName(piece);
            if (InitManager.IsPatchedByMod(pieceName))
            {
                if (piece.m_category == CreatorShop)
                {
                    return true;
                }
            }
            return false;
        }

        internal static bool IsNaturePiece(Piece piece)
        {
            var pieceName = NameHelper.GetRootPrefabName(piece);
            if (InitManager.IsPatchedByMod(pieceName))
            {
                if (piece.m_category == Nature)
                {
                    return true;
                }
            }
            return false;
        }
    }
}