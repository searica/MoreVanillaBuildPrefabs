// Ignore Spelling: MVBP

namespace MVBP.Models
{
    internal class PieceDB : PrefabDB
    {
        internal Piece piece;

        public PieceDB(PrefabDB prefabDB, Piece piece)
        {
            name = prefabDB.name;
            this.piece = piece;
            enabled = prefabDB.enabled;
            allowedInDungeons = prefabDB.allowedInDungeons;
            category = prefabDB.category;
            craftingStation = prefabDB.craftingStation;
            requirements = prefabDB.requirements;
            clipEverything = prefabDB.clipEverything;
            clipGround = prefabDB.clipGround;
            placementPatch = prefabDB.placementPatch;
            placementOffset = prefabDB.placementOffset;
            pieceName = prefabDB.pieceName;
            pieceDesc = prefabDB.pieceDesc;
            pieceGroup = prefabDB.pieceGroup;
            playerBasePatch = prefabDB.playerBasePatch;
            invWidth = prefabDB.invWidth;
            invHeight = prefabDB.invHeight;
        }
    }
}