// Ignore Spelling: MVBP

using Jotunn.Configs;
using UnityEngine;

namespace MVBP.Models
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
        public Vector3? placementOffset;
        public string pieceName;
        public string pieceDesc;
        public PieceGroup pieceGroup;
        public bool playerBasePatch;
        public string spawnOnDestroyed;
        public uint? invWidth;
        public uint? invHeight;

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
            bool clipEverything = false,
            bool clipGround = false,
            bool placementPatch = false,
            Vector3? placementOffset = null,
            string pieceName = null,
            string pieceDesc = null,
            PieceGroup pieceGroup = default,
            bool playerBasePatch = false,
            string spawnOnDestroyed = null,
            uint? invWidth = null,
            uint? invHeight = null
        )
        {
            this.name = name;
            this.enabled = enabled;
            this.allowedInDungeons = allowedInDungeons;
            this.category = category;
            this.craftingStation = craftingStation;
            this.requirements = requirements;
            this.clipEverything = clipEverything;
            this.clipGround = clipGround;
            this.placementPatch = placementPatch;
            this.placementOffset = placementOffset;
            this.pieceName = pieceName;
            this.pieceDesc = pieceDesc;
            this.pieceGroup = pieceGroup;
            this.playerBasePatch = playerBasePatch;
            this.spawnOnDestroyed = spawnOnDestroyed;
            this.invWidth = invWidth;
            this.invHeight = invHeight;
        }

        /// <summary>
        ///     Update field data based on PrefabDBConfig data.
        /// </summary>
        /// <param name="DBConfig"></param>
        public void Update(PrefabDBConfigEntries DBConfig)
        {
            enabled = DBConfig.enabled.Value;
            allowedInDungeons = DBConfig.allowedInDungeons.Value;
            category = DBConfig.category.Value;
            craftingStation = DBConfig.craftingStation.Value;
            requirements = DBConfig.requirements.Value;

            // If a config settings is null then it does not show in the config file.
            // This is to avoid users changing it because it should always be set to true
            placementPatch = DBConfig.placementPatch == null || DBConfig.placementPatch.Value;
            clipEverything = DBConfig.clipEverything == null || DBConfig.clipEverything.Value;
            clipGround = DBConfig.clipGround == null || DBConfig.clipGround.Value;
        }
    }
}