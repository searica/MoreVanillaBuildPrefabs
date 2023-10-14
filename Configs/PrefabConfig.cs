using MoreVanillaBuildPrefabs.Helpers;

namespace MoreVanillaBuildPrefabs.Configs
{
    internal struct PrefabConfig
    {
        public bool Enabled;
        public bool AllowedInDungeons;
        public string Category;
        public string CraftingStation;
        public string Requirements;
        public bool PlacementPatch;

        public PrefabConfig(
            bool Enabled = false,
            bool AllowedInDungeons = false,
            string Category = HammerCategories.CreatorShop,
            string CraftingStation = nameof(CraftingStations.None),
            string Requirements = "",
            bool PlacementPatch = false
        )
        {
            this.Enabled = Enabled;
            this.AllowedInDungeons = AllowedInDungeons;
            this.Category = Category;
            this.CraftingStation = CraftingStation;
            this.Requirements = Requirements;
            this.PlacementPatch = PlacementPatch;
        }
    }
}
