using BepInEx.Configuration;
using Jotunn.Managers;


namespace MoreVanillaBuildPrefabs
{
    internal class Hammer
    {
        public static class HammerCategories
        {
            public static Piece.PieceCategory Misc;
            public static Piece.PieceCategory Crafting;
            public static Piece.PieceCategory Building;
            public static Piece.PieceCategory Furniture;
            public static Piece.PieceCategory CreatorShop;
        }

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

        public static void AddHammerCategories()
        {
            Log.LogInfo("AddHammerCategories()");
            //HammerCategories.Misc = PieceManager.Instance.AddPieceCategory("_HammerPieceTable", HammerCategoryNames.Misc);
            //HammerCategories.Crafting = PieceManager.Instance.AddPieceCategory("_HammerPieceTable", HammerCategoryNames.Crafting);
            //HammerCategories.Furniture = PieceManager.Instance.AddPieceCategory("_HammerPieceTable", HammerCategoryNames.Furniture);
            //HammerCategories.Building = PieceManager.Instance.AddPieceCategory("_HammerPieceTable", HammerCategoryNames.Building);
            HammerCategories.CreatorShop = PieceManager.Instance.AddPieceCategory("_HammerPieceTable", HammerCategoryNames.CreatorShop);
        }

        public static void RemoveHammerCategories()
        {
            Log.LogInfo("RemoveHammerCategories()");
            PieceManager.Instance.RemovePieceCategory("_HammerPieceTable", HammerCategoryNames.CreatorShop);
        }
    }
}
