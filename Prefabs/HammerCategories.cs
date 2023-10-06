using BepInEx.Configuration;
using Jotunn.Managers;


namespace MoreVanillaBuildPrefabs
{
    internal class HammerCategories
    {

        public static Piece.PieceCategory CreatorShop;

        public static class HammerCategoryNames
        {
            public const string CreatorShop = "CreatorShop";
            public const string Misc = "Misc";
            public const string Crafting = "Crafting";
            public const string Building = "Building";
            public const string Furniture = "Furniture";

            public static AcceptableValueList<string> GetAcceptableValueList()
            {
                return new AcceptableValueList<string>(typeof(HammerCategoryNames).GetAllPublicConstantValues<string>().ToArray());
            }
        }

        public static void AddCustomCategories()
        {
            Log.LogInfo("AddCustomCategories()");
            CreatorShop = PieceManager.Instance.AddPieceCategory("_HammerPieceTable", HammerCategoryNames.CreatorShop);
        }

        public static void RemoveCustomCategories()
        {
            Log.LogInfo("RemoveCustomCategories()");
            PieceManager.Instance.RemovePieceCategory("_HammerPieceTable", HammerCategoryNames.CreatorShop);
        }

        public static bool IsCreatorShopPiece(Piece piece)
        {
            //if (PrefabHelper.AddedPieces.Contains(piece.m_name))
            if (PrefabHelper.AddedPrefabs.Contains(piece.gameObject.name))
            {
                if (piece.m_category == HammerCategories.CreatorShop)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
