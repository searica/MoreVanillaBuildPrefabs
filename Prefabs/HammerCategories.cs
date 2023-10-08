using BepInEx.Configuration;
using Jotunn.Managers;


namespace MoreVanillaBuildPrefabs
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
            CreatorShop = PieceManager.Instance.AddPieceCategory("_HammerPieceTable", Names.CreatorShop);
        }

        public static void RemoveCustomCategories()
        {
            Log.LogInfo("RemoveCustomCategories()");
            PieceManager.Instance.RemovePieceCategory("_HammerPieceTable", Names.CreatorShop);
        }

        public static bool IsCreatorShopPiece(Piece piece)
        {
            if (PrefabHelper.AddedPrefabs.Contains(piece.gameObject.name))
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
