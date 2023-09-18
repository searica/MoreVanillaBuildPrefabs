using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MoreVanillaBuildPrefabs
{

    public static class PieceExtensions
    {
        public static Piece AddPlantComponent(this Piece piece)
        {
            piece.GetComponent<Plant>();
            return piece;
        }

        public static Piece SetCanBeRemoved(this Piece piece, bool canBeRemoved)
        {
            piece.m_canBeRemoved = canBeRemoved;
            return piece;
        }

        public static Piece SetCategory(this Piece piece, Piece.PieceCategory pieceCategory)
        {
            piece.m_category = pieceCategory;
            return piece;
        }

        public static Piece SetCraftingStation(this Piece piece, CraftingStation craftingStation)
        {
            piece.m_craftingStation = craftingStation;
            return piece;
        }

        public static Piece SetGroundOnly(this Piece piece, bool groundOnly)
        {
            piece.m_groundOnly = groundOnly;
            return piece;
        }

        public static Piece SetName(this Piece piece, string name)
        {
            piece.m_name = name;
            return piece;
        }

        public static Piece SetDescription(this Piece piece, string description)
        {
            piece.m_description = description;
            return piece;
        }

        public static Piece SetIcon(this Piece piece, Sprite icon)
        {
            piece.m_icon = icon;
            return piece;
        }

        public static Piece SetResources(this Piece piece, params Piece.Requirement[] requirements)
        {
            piece.m_resources = requirements;
            return piece;
        }

        public static Piece SetTargetNonPlayerBuilt(this Piece piece, bool canTarget)
        {
            piece.m_targetNonPlayerBuilt = canTarget;
            return piece;
        }
    }
    public static class PieceTableExtensions
    {
        public static bool AddPiece(this PieceTable pieceTable, Piece piece)
        {
            if (!piece || !pieceTable || pieceTable.m_pieces == null || pieceTable.m_pieces.Contains(piece.gameObject))
            {
                return false;
            }

            pieceTable.m_pieces.Add(piece.gameObject);
            ZLog.Log($"Added Piece {piece.m_name} to PieceTable {pieceTable.name}");

            return true;
        }
    }

    public static class PluginExtensions
    {
        public static T Ref<T>(this T o) where T : UnityEngine.Object
        {
            return o ? o : null;
        }

        public static void Dispose(this IEnumerable<IDisposable> collection)
        {
            foreach (IDisposable item in collection)
            {
                if (item != null)
                {
                    try
                    {
                        item.Dispose();
                    }
                    catch (Exception)
                    {
                        Log.LogWarning("Could not dispose of item");
                    }
                }
            }
        }
    }
}
