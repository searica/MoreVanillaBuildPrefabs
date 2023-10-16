using Jotunn.Configs;
using Jotunn.Managers;
using MoreVanillaBuildPrefabs.Configs;
using MoreVanillaBuildPrefabs.Logging;

namespace MoreVanillaBuildPrefabs.Helpers
{
    internal class CreatorShopHelper
    {
        internal static Piece.PieceCategory CreatorShop;

        internal static void AddCreatorShopPieceCategory()
        {
            if (PieceManager.Instance.GetPieceCategory(HammerCategories.CreatorShop) == null)
            {
                if (PluginConfig.IsVerbosityMedium)
                {
                    Log.LogInfo("AddCreatorShopPieceCategory()");
                }
                CreatorShop = PieceManager.Instance.AddPieceCategory(PieceTables.Hammer, HammerCategories.CreatorShop);
            }
        }

        internal static void RemoveCreatorShopPieceCategory()
        {
            if (PluginConfig.IsVerbosityMedium)
            {
                Log.LogInfo("RemoveCreatorShopPieceCategory()");
            }
            PieceManager.Instance.RemovePieceCategory(PieceTables.Hammer, HammerCategories.CreatorShop);
        }

        internal static bool IsCreatorShopPiece(Piece piece)
        {
            var pieceName = NameHelper.GetPrefabName(piece);
            if (PieceHelper.AddedPrefabs.Contains(pieceName))
            {
                if (piece.m_category == CreatorShop)
                {
                    return true;
                }
            }
            return false;
        }
    }
}