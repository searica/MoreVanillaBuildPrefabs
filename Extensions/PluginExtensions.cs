using Jotunn.Configs;
using Jotunn.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace MoreVanillaBuildPrefabs
{

    public static class TypeExtensions
    {
        public static List<T> GetAllPublicConstantValues<T>(this Type type)
        {
            return type
                .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                .Where(fi => fi.IsLiteral && !fi.IsInitOnly && fi.FieldType == typeof(T))
                .Select(x => (T)x.GetRawConstantValue())
                .ToList();
        }

        public static List<T> GetAllPublicStaticValues<T>(this Type type)
        {
            return type
                .GetFields(BindingFlags.Public | BindingFlags.Static)
                .Where(fi => fi.FieldType == typeof(T))
                .Select(x => (T)x.GetValue(null))
                .ToList();
        }
    }

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

        public static Piece SetCategory(this Piece piece, string pieceCategoryName)
        {
            var pieceCategory = PieceManager.Instance.GetPieceCategory(pieceCategoryName);
            piece.m_category = (Piece.PieceCategory)pieceCategory; 
            return piece;
        }


        public static Piece SetCategory(this Piece piece, Piece.PieceCategory pieceCategory)
        {
            piece.m_category = pieceCategory;
            return piece;
        }

        public static Piece SetCraftingStation(this Piece piece, string humanReadableName)
        {
            CraftingStation station;
            if (humanReadableName == "None")
            {
                station = null;
            }
            else
            {
                var internalName = CraftingStations.GetNames()[humanReadableName];
                station = ZNetScene.instance.GetPrefab(internalName).GetComponent<CraftingStation>();
            }
            piece.m_craftingStation = station;
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

        public static Piece SetAllowedInDungeons(this Piece piece, bool value)
        {
            piece.m_allowedInDungeons = value;
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
#if DEBUG
            Log.LogInfo($"Added Piece {piece.m_name} to PieceTable {pieceTable.name}");
#endif
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
