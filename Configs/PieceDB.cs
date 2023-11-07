// Ignore Spelling: MVBP

namespace MVBP.Configs
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
            this.clipEverything = prefabDB.clipEverything;
            this.clipGround = prefabDB.clipGround;
            this.pieceName = prefabDB.pieceName;
            this.pieceDesc = prefabDB.pieceDesc;
            this.pieceGroup = prefabDB.pieceGroup;
            this.playerBasePatch = prefabDB.playerBasePatch;
        }
    }
}