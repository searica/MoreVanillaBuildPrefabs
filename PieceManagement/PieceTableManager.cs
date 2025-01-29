// Ignore Spelling: MVBP

using Jotunn.Configs;
using Jotunn.Managers;

using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MVBP.PieceManagement;

/// <summary>
///     Manages Piece configuration and updating Hammer piece table.
/// </summary>
internal static class PieceTableManager
{
    internal static readonly HashSet<string> AddedPrefabs = new();


    internal static CraftingStation GetCraftingStation(string name)
    {
        string internalName = CraftingStations.GetInternalName(name);
        CraftingStation station = ZNetScene.instance?.GetPrefab(internalName)?.GetComponent<CraftingStation>();
        return station;
    }

    /// <summary>
    ///     Method to configure Piece fields based on a PieceDB instance.
    /// </summary>
    /// <param name="pieceDB"></param>
    /// <returns></returns>
    internal static Piece ConfigurePiece(PieceDB pieceDB)
    {
        var piece = pieceDB.piece;
        string name = NameMaker.FormatPieceName(pieceDB);
        string description = NameMaker.GetPieceDescription(pieceDB);
        Piece.PieceCategory pieceCategory = GetPieceCategory(pieceDB.category);

        if (AddedPieceComponent.Contains(pieceDB.name))
        {
            // set component enabled/disabled for components added by MVBP
            piece.enabled = pieceDB.enabled || MorePrefabs.IsForceAllPrefabs;
            piece.m_enabled = pieceDB.enabled; // set piece visible in PieceTable based on MVBP config
            pieceDB.piece.m_canBeRemoved = pieceDB.enabled; // set if removeable
        }

        piece.m_name = name;
        piece.m_description = description;
        piece.m_allowedInDungeons = pieceDB.allowedInDungeons;
        piece.m_category = pieceCategory;
        piece.m_craftingStation = GetCraftingStation(pieceDB.craftingStation);
        piece.m_resources = ConfigurePieceRequirements(pieceDB);
        piece.m_clipEverything = pieceDB.clipEverything;
        piece.m_clipGround = pieceDB.clipGround;

        // Prevent CreativeMode pieces and any clones of them
        // from being removable.
        // (Player.RemovePiece patch allows removing player-built instances).
        // Mimic Vanilla, make ships/carts non-removable.
        if (PieceCategoryHelper.IsCreativeModePiece(pieceDB.piece) ||
            pieceDB.Prefab.GetComponent<Ship>() ||
            pieceDB.Prefab.GetComponent<Vagon>())
        {
            pieceDB.piece.m_canBeRemoved = false;
        }


        return piece;
    }


    /// <summary>
    ///     Safely search for piece category and fall back to "Misc" if
    ///     it cannot be found.
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    private static Piece.PieceCategory GetPieceCategory(string name)
    {
        try
        {
            return (Piece.PieceCategory)PieceManager.Instance.GetPieceCategory(name);
        }
        catch (InvalidOperationException)
        {
            Log.LogWarning($"Could not find value for Piece Category: {name}");
            return Piece.PieceCategory.Misc;
        }
    }

    /// <summary>
    ///     Create piece requirements array from pieceDB and modify it to prevent
    ///     exploits if the piece has a pickable component.
    /// </summary>
    /// <param name="pieceDB"></param>
    /// <returns></returns>
    private static Piece.Requirement[] ConfigurePieceRequirements(PieceDB pieceDB)
    {
        var reqs = RequirementsEntry.CreateRequirementsArray(pieceDB.requirements);

        if (pieceDB.piece.TryGetComponent(out MineRock mineRock))
        {
            reqs = RequirementsHelper.AddMineRockDropsToRequirements(reqs, mineRock);
        }

        if (pieceDB.piece.TryGetComponent(out MineRock5 mineRock5))
        {
            reqs = RequirementsHelper.AddMineRock5DropsToRequirements(reqs, mineRock5);
        }

        if (pieceDB.piece.TryGetComponent(out Pickable pickable))
        {
            reqs = RequirementsHelper.AddPickableToRequirements(reqs, pickable);
        }

        return reqs;
    }


    /// <summary>
    ///     Method to add a piece to a piece table.
    /// </summary>
    /// <param name="pieces"></param>
    /// <param name="pieceTableName"></param>
    /// <returns> bool indicating if customPiece was added. </returns>
    internal static void AddPiecesListToPieceTable(IEnumerable<Piece> pieces, string pieceTableName)
    {
        PieceTable pieceTable = PieceManager.Instance.GetPieceTable(pieceTableName);

        foreach (Piece piece in pieces)
        {
            AddPieceToPieceTable(piece, pieceTable);
        }

        Log.LogInfo($"Added {AddedPrefabs.Count} custom pieces");
    }

    /// <summary>
    ///     Method to add a prefab to a piece table.
    /// </summary>
    /// <param name="piece"></param>
    /// <param name="pieceTable"></param>
    /// <returns> bool indicating if customPiece was added. </returns>
    internal static bool AddPieceToPieceTable(GameObject prefab, PieceTable pieceTable)
    {
        Piece piece = prefab.GetComponent<Piece>() ?? throw new Exception($"Prefab {prefab.name} has no Piece component.");

        return AddPieceToPieceTable(piece, pieceTable);
    }

    /// <summary>
    ///     Method to add a piece to a piece table.
    /// </summary>
    /// <param name="piece"></param>
    /// <param name="pieceTable"></param>
    /// <returns> bool indicating if customPiece was added. </returns>
    internal static bool AddPieceToPieceTable(Piece piece, PieceTable pieceTable)
    {
        if (!piece || !pieceTable || pieceTable.m_pieces == null || pieceTable.m_pieces.Contains(piece.gameObject))
        {
            return false;
        }

        GameObject prefab = piece.gameObject;
        string name = prefab.name;
        int hash = name.GetStableHashCode();

        if (ZNetScene.instance && !ZNetScene.instance.m_namedPrefabs.ContainsKey(hash))
        {
            RegisterToZNetScene(prefab);
        }

        pieceTable.m_pieces.Add(prefab);
        AddedPrefabs.Add(prefab.name);
        Log.LogInfo($"Added Piece {piece.m_name} to PieceTable {pieceTable.name}", Log.InfoLevel.High);
        return true;
    }

    /// <summary>
    ///     Register a single prefab to the current <see cref="ZNetScene"/>.<br />
    ///     Checks for existence of the object via GetStableHashCode() and adds the prefab if it is not already added.
    /// </summary>
    /// <param name="gameObject"></param>
    internal static void RegisterToZNetScene(GameObject gameObject)
    {
        ZNetScene znet = ZNetScene.instance;

        if (znet)
        {
            string name = gameObject.name;
            int hash = name.GetStableHashCode();

            if (znet.m_namedPrefabs.ContainsKey(hash))
            {
                Log.LogDebug($"Prefab {name} already in ZNetScene");
            }
            else
            {
                if (gameObject.GetComponent<ZNetView>() != null)
                {
                    znet.m_prefabs.Add(gameObject);
                }
                else
                {
                    znet.m_nonNetViewPrefabs.Add(gameObject);
                }
                znet.m_namedPrefabs.Add(hash, gameObject);
                Log.LogDebug($"Added prefab {name}");
            }
        }
    }

    /// <summary>
    ///     Removes all pieces added by the mod from the piece table.
    /// </summary>
    /// <param name="pieceTableName"></param>
    internal static void RemoveAllCustomPiecesFromPieceTable(string pieceTableName)
    {
        Log.LogInfo("RemoveAllCustomPiecesFromPieceTable()", Log.InfoLevel.Medium);

        int numCustomPieces = AddedPrefabs.Count;
        var prefabsToRemove = AddedPrefabs.ToList();
        PieceTable pieceTable = PieceManager.Instance.GetPieceTable(pieceTableName);

        if (pieceTable == null)
        {
            Log.LogError($"Could not find piece table: {pieceTableName}");
        }

        foreach (string name in prefabsToRemove)
        {
            RemovePieceFromPieceTable(name, pieceTable);
        }

        Log.LogInfo($"Removed {numCustomPieces - AddedPrefabs.Count} custom pieces", Log.InfoLevel.Medium);
    }

    /// <summary>
    ///     Remove piece from PieceTable
    /// </summary>
    /// <param name="name"></param>
    /// <param name="pieceTable"></param>
    /// <returns></returns>
    internal static bool RemovePieceFromPieceTable(string name, PieceTable pieceTable)
    {
        try
        {
            GameObject prefab = ZNetScene.instance.GetPrefab(name);
            if (pieceTable.m_pieces.Contains(prefab))
            {
                pieceTable.m_pieces.Remove(prefab);
                AddedPrefabs.Remove(prefab.name);
                return true;
            }
            return false;
        }
        catch (Exception e)
        {
            Log.LogInfo($"{name}: {e}");
            return false;
        }
    }

    /// <summary>
    ///     Remove piece from PieceTable
    /// </summary>
    /// <param name="name"></param>
    /// <param name="pieceTable"></param>
    /// <returns></returns>
    internal static bool RemovePieceFromPieceTable(GameObject prefab, PieceTable pieceTable)
    {
        try
        {
            if (pieceTable.m_pieces.Contains(prefab))
            {
                pieceTable.m_pieces.Remove(prefab);
                AddedPrefabs.Remove(prefab.name);
                return true;
            }
            return false;
        }
        catch (Exception e)
        {
            Log.LogInfo($"{prefab.name}: {e}", Log.InfoLevel.Medium);
            return false;
        }
    }
}
