using BepInEx.Configuration;


namespace MoreVanillaBuildPrefabs.Configs
{
    /// <summary>
    ///     Helper class to get the names of hammer piece categories for this mod.
    /// </summary>
    internal static class HammerCategories
    {
        public const string CreatorShop = "CreatorShop";
        public const string Misc = "Misc";
        public const string Crafting = "Crafting";
        public const string Building = "Building";
        public const string Furniture = "Furniture";

        internal static AcceptableValueList<string> GetAcceptableValueList()
        {
            return new AcceptableValueList<string>(typeof(HammerCategories).GetAllPublicConstantValues<string>().ToArray());
        }
    }
}
