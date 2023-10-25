using Jotunn.Configs;
using Jotunn.Managers;
using MoreVanillaBuildPrefabs.Configs;
using MoreVanillaBuildPrefabs.Logging;

namespace MoreVanillaBuildPrefabs.Helpers
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
                if (PluginConfig.IsVerbosityMedium)
                {
                    Log.LogInfo("Adding custom piece categories");
                }
                Nature = PieceManager.Instance.AddPieceCategory(PieceTables.Hammer, HammerCategories.Nature);
                CreatorShop = PieceManager.Instance.AddPieceCategory(PieceTables.Hammer, HammerCategories.CreatorShop);
            }
        }

        internal static void RemoveCreatorShopPieceCategory()
        {
            if (PluginConfig.IsVerbosityMedium)
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
            var pieceName = NameHelper.GetPrefabName(piece);
            if (MoreVanillaBuildPrefabs.IsPatchedByMod(pieceName))
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
            var pieceName = NameHelper.GetPrefabName(piece);
            if (MoreVanillaBuildPrefabs.IsPatchedByMod(pieceName))
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