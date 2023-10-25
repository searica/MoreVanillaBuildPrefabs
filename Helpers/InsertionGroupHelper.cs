using System;
using System.Collections.Generic;
using UnityEngine;

using MoreVanillaBuildPrefabs.Configs;

namespace MoreVanillaBuildPrefabs.Helpers
{
    internal class InsertionGroupHelper
    {
        internal Dictionary<PieceGroup, List<GameObject>> PieceGroupDict;

        public InsertionGroupHelper()
        {
            PieceGroupDict = new();
            foreach (PieceGroup group in Enum.GetValues(typeof(PieceGroup)))
            {
                if (group == PieceGroup.None) { continue; }
                PieceGroupDict[group] = new List<GameObject>();
            }
        }

        internal void AddPieces(PieceTable pieceTable)
        {
            // Loop through groups in based on the ordering in PieceGroup Enum
            foreach (PieceGroup group in Enum.GetValues(typeof(PieceGroup)))
            {
                if (group == PieceGroup.None) { continue; }
                var pieceGroup = PieceGroupDict[group];
                foreach (var prefab in pieceGroup)
                {
                    PieceHelper.AddPieceToPieceTable(prefab, pieceTable);
                }
            }
            // need to figure out an insertion point for each Enum group
        }

        internal void Add(PieceDB pieceDB)
        {
            var key = GetPieceGroup(pieceDB);
            PieceGroupDict[key].Add(pieceDB.Prefab);
        }

        internal static PieceGroup GetPieceGroup(PieceDB pieceDB)
        {
            if (pieceDB.pieceGroup != PieceGroup.None) return pieceDB.pieceGroup;

            var prefabName = pieceDB.name.ToLower();
            if (prefabName.Contains("blackmarble"))
            {
                return PieceGroup.BlackMarble;
            }
            else if (pieceDB.Prefab.GetComponent<Ship>() != null)
            {
                return PieceGroup.Ship;
            }
            else if (prefabName.Contains("chest"))
            {
                return PieceGroup.Chest;
            }
            else if (prefabName.Contains("brazier"))
            {
                return PieceGroup.Brazier;
            }
            else if (prefabName.Contains("fire"))
            {
                return PieceGroup.Fire;
            }
            else if (prefabName.Contains("dvergr") || prefabName.Contains("dverger"))
            {
                return PieceGroup.Dvergr;
            }
            else if (prefabName.Contains("armorstand"))
            {
                return PieceGroup.ArmorStand;
            }
            else if (prefabName.Contains("wood"))
            {
                return PieceGroup.Wood;
            }
            else if (prefabName.Contains("stone"))
            {
                return PieceGroup.Stone;
            }
            else if (prefabName.Contains("torch"))
            {
                return PieceGroup.Torch;
            }
            else if (prefabName.Contains("statue"))
            {
                return PieceGroup.Statue;
            }
            else if (prefabName.Contains("chair")
                || prefabName.Contains("throne")
                || prefabName.Contains("bench"))
            {
                return PieceGroup.Chair;
            }
            else if (prefabName.Contains("banner")
                || prefabName.Contains("curtain")
                || prefabName.Contains("drape")
                || prefabName.Contains("cloth"))
            {
                return PieceGroup.Banner;
            }
            else if (prefabName.Contains("ore"))
            {
                return PieceGroup.Ore;
            }
            else if (prefabName.Contains("table"))
            {
                return PieceGroup.Table;
            }
            else if (prefabName.Contains("tree"))
            {
                return PieceGroup.Tree;
            }
            else if (prefabName.Contains("rock") || prefabName.Contains("cliff"))
            {
                return PieceGroup.Rock;
            }

            return PieceGroup.Misc;
        }
    }
}