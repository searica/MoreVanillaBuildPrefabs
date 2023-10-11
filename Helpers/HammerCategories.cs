using BepInEx.Configuration;
using Jotunn.Managers;
using Jotunn.Configs;
using MoreVanillaBuildPrefabs.Logging;

namespace MoreVanillaBuildPrefabs.Helpers
{
    internal class HammerCategories
    {

        public static Piece.PieceCategory CreatorShop;

        public static class Names
        {
            public const string CreatorShop = "CreatorShop";
            public const string Misc = "Misc";
            public const string Crafting = "Crafting";
            public const string Building = "Building";
            public const string Furniture = "Furniture";

            public static AcceptableValueList<string> GetAcceptableValueList()
            {
                return new AcceptableValueList<string>(typeof(Names).GetAllPublicConstantValues<string>().ToArray());
            }
        }

        public static void AddCustomCategories()
        {
            Log.LogInfo("AddCustomCategories()");
            CreatorShop = PieceManager.Instance.AddPieceCategory(PieceTables.Hammer, Names.CreatorShop);
        }

        public static void RemoveCustomCategories()
        {
            Log.LogInfo("RemoveCustomCategories()");
            PieceManager.Instance.RemovePieceCategory(PieceTables.Hammer, Names.CreatorShop);
        }

        public static bool IsCreatorShopPiece(Piece piece)
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
