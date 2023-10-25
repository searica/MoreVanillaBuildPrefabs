using Jotunn.Configs;

namespace MoreVanillaBuildPrefabs.Configs
{
    internal class PieceDB : PrefabDB
    {
        internal Piece piece;

        public PieceDB(PrefabDB prefabDB, Piece piece)
        {
            this.name = prefabDB.name;
            this.piece = piece;
            this.enabled = prefabDB.enabled;
            this.allowedInDungeons = prefabDB.allowedInDungeons;
            this.category = prefabDB.category;
            this.craftingStation = prefabDB.craftingStation;
            this.requirements = prefabDB.requirements;
            this.placementPatch = prefabDB.placementPatch;
            this.pieceName = prefabDB.pieceName;
            this.pieceDesc = prefabDB.pieceDesc;
            this.pieceGroup = prefabDB.pieceGroup;
        }

        public PieceDB(
            string name,
            Piece piece,
            bool enabled = false,
            bool allowedInDungeons = false,
            string category = HammerCategories.CreatorShop,
            string craftingStation = nameof(CraftingStations.None),
            string requirements = null,
            bool placementPatch = false,
            string pieceName = null,
            string pieceDesc = null,
            PieceGroup pieceGroup = default
        )
        {
            this.name = name;
            this.piece = piece;
            this.enabled = enabled;
            this.allowedInDungeons = allowedInDungeons;
            this.category = category;
            this.craftingStation = craftingStation;
            this.requirements = requirements;
            this.placementPatch = placementPatch;
            this.pieceName = pieceName;
            this.pieceDesc = pieceDesc;
            this.pieceGroup = pieceGroup;
        }
    }
}