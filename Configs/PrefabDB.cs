// Ignore Spelling: MVBP

using Jotunn.Configs;
using MVBP.Helpers;
using UnityEngine;

namespace MVBP.Configs
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
        public bool playerBasePatch;

        internal GameObject Prefab
        {
            get { return InitManager.PrefabRefs[name]; }
        }

        // for inheritance
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
            PieceGroup pieceGroup = default,
            bool playerBasePatch = false

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
            this.playerBasePatch = playerBasePatch;
        }
    }
}