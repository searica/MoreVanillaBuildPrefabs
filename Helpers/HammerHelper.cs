using Jotunn.Managers;
using Jotunn.Configs;

using MoreVanillaBuildPrefabs.Logging;
using MoreVanillaBuildPrefabs.Configs;

namespace MoreVanillaBuildPrefabs.Helpers
{
    internal class HammerHelper
    {

        internal static Piece.PieceCategory CreatorShop;

        internal static void AddCustomCategories()
        {
            Log.LogInfo("AddCustomCategories()");
            CreatorShop = PieceManager.Instance.AddPieceCategory(PieceTables.Hammer, HammerCategories.CreatorShop);
        }

        internal static void RemoveCustomCategories()
        {
            Log.LogInfo("RemoveCustomCategories()");
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
