using Jotunn.Configs;
using UnityEngine;

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
        public bool clipEverything;
        public bool clipGround;
        public bool placementPatch;
        public string pieceName;
        public string pieceDesc;
        public PieceGroup pieceGroup;

        internal GameObject Prefab
        {
            get { return MoreVanillaBuildPrefabs.PrefabRefs[name]; }
        }

        // for inheiritance
        public PrefabDB()
        { }

        public PrefabDB(
            string name,
            bool enabled = false,
            bool allowedInDungeons = false,
            string category = HammerCategories.CreatorShop,
            string craftingStation = nameof(CraftingStations.None),
            string requirements = null,
            bool placementPatch = false,
            bool clipEverything = false,
            bool clipGround = false,
            string pieceName = null,
            string pieceDesc = null,
            PieceGroup pieceGroup = default
        )
        {
            this.name = name;
            this.enabled = enabled;
            this.allowedInDungeons = allowedInDungeons;
            this.category = category;
            this.craftingStation = craftingStation;
            this.requirements = requirements;
            this.placementPatch = placementPatch;
            this.clipEverything = clipEverything;
            this.clipGround = clipGround;
            this.pieceName = pieceName;
            this.pieceDesc = pieceDesc;
            this.pieceGroup = pieceGroup;
        }
    }
}