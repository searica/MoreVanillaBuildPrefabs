using UnityEngine;
using Jotunn.Configs;

namespace MoreVanillaBuildPrefabs.Configs
{
    internal class PrefabDB
    {
        public string name;
        public bool enabled;
        public bool allowedInDungeons;
        public string category;
        public string craftingStation;
        public string requirements;
        public bool placementPatch;
        internal GameObject Prefab
        {
            get { return MoreVanillaBuildPrefabs.PrefabRefs[name]; }
        }

        // for inheiritance
        public PrefabDB() { }

        public PrefabDB(
            string name,
            bool enabled = false,
            bool allowedInDungeons = false,
            string category = HammerCategories.CreatorShop,
            string craftingStation = nameof(CraftingStations.None),
            string requirements = null,
            bool placementPatch = false
        )
        {
            this.name = name;
            this.enabled = enabled;
            this.allowedInDungeons = allowedInDungeons;
            this.category = category;
            this.craftingStation = craftingStation;
            this.requirements = requirements;
            this.placementPatch = placementPatch;
        }
    }
}
