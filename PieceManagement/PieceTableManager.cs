// Ignore Spelling: MVBP

using Jotunn.Managers;
using System;
using System.Collections.Generic;
using UnityEngine;
using Logging;
using MVBP.PrefabManagement;

namespace MVBP.PieceManagement;

/// <summary>
///     Manages Piece configuration and updating Hammer piece table.
/// </summary>
internal static class PieceTableManager
{
    private static readonly Dictionary<PieceTable, List<GameObject>> AddedPrefabs = [];

    /// <summary>
    ///     Sorts and adds pieces based on PrefabConfigs and MorePrefabs config settings.
    /// </summary>
    /// <param name="pieceTable"></param>
    /// <param name="prefabConfigs"></param>
    /// <param name="clearPieces">Whether to remove all custom pieces from piece table or not.</param>
    public static void UpdatePieceTable(PieceTable pieceTable, List<PrefabConfig> prefabConfigs, bool clearPieces = true)
    {
        if (clearPieces)
        {
            RemoveAllCustomPiecesFromPieceTable(pieceTable);
        }

        SortedPieceGroups pieceGroups = [];
        foreach (PrefabConfig prefabConfig in prefabConfigs)
        {
            if (!prefabConfig.Prefab)
            {
                Log.LogWarning($"Prefab: {prefabConfig.Name} has been destroyed");
                continue;
            }

            // Check if defaultResources is enabled by the mod
            if (!prefabConfig.Enabled.Value && !MorePrefabs.IsForceAllPrefabs)
            {
                continue;
            }

            // Prevent adding creative mode pieces if not in CreativeMode
            if (!MorePrefabs.IsCreativeMode && PieceCategoryManager.IsCreativeModePiece(prefabConfig.Piece))
            {
                continue;
            }

            // Only add vanilla crops if enabled
            if (!MorePrefabs.IsEnableHammerCrops && prefabConfig.PieceGroup == PieceClassification.VanillaCrop)
            {
                continue;
            }

            // Restrict placement of CreatorShop pieces to Admins only
            if (MorePrefabs.IsCreatorShopAdminOnly &&
                PieceCategoryManager.IsCreatorShopPiece(prefabConfig.Piece) &&
                !SynchronizationManager.Instance.PlayerIsAdmin)
            {
                continue;
            }

            pieceGroups.Add(prefabConfig);
        }

        foreach (List<GameObject> pieceGroup in pieceGroups)
        {
            foreach (GameObject prefab in pieceGroup)
            {
                AddPieceToPieceTable(prefab, pieceTable);
            }
        }
    }

    /// <summary>
    ///     Method to add a prefab to a piece table.
    /// </summary>
    /// <param name="piece"></param>
    /// <param name="pieceTable"></param>
    /// <returns> bool indicating if customPiece was added. </returns>
    private static bool AddPieceToPieceTable(GameObject prefab, PieceTable pieceTable)
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
    private static bool AddPieceToPieceTable(Piece piece, PieceTable pieceTable)
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
        if (!AddedPrefabs.ContainsKey(pieceTable))
        {
            AddedPrefabs.Add(pieceTable, new List<GameObject>());
        }
        AddedPrefabs[pieceTable].Add(prefab);

        Log.LogInfo($"Added Piece {piece.m_name} to PieceTable {pieceTable.name}", Log.InfoLevel.High);
        return true;
    }

    /// <summary>
    ///     Register a single prefab to the current <see cref="ZNetScene"/>.<br />
    ///     Checks for existence of the object via GetStableHashCode() and adds the prefab if it is not already added.
    /// </summary>
    /// <param name="gameObject"></param>
    private static void RegisterToZNetScene(GameObject gameObject)
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
    private static void RemoveAllCustomPiecesFromPieceTable(PieceTable pieceTable)
    {
        if (pieceTable == null)
        {
            Log.LogError($"Piece table is null!");
        }
        if (!AddedPrefabs.TryGetValue(pieceTable, out List<GameObject> AddedPieces))
        {
            return;
        }

        int numCustomPieces = AddedPieces.Count;
        if (numCustomPieces == 0)
        {
            return;
        }

        for (int i = numCustomPieces - 1; i > -1; i--)
        {
            if (RemovePieceFromPieceTable(AddedPieces[i], pieceTable))
            {
                AddedPieces.RemoveAt(i);
            }
        }
        Log.LogInfo($"Removed {numCustomPieces - AddedPieces.Count} custom pieces", Log.InfoLevel.Medium);
    }

    /// <summary>
    ///     Remove piece from PieceTable
    /// </summary>
    /// <param name="name"></param>
    /// <param name="pieceTable"></param>
    /// <returns></returns>
    private static bool RemovePieceFromPieceTable(GameObject prefab, PieceTable pieceTable)
    {
        try
        {
            if (pieceTable.m_pieces.Contains(prefab))
            {
                pieceTable.m_pieces.Remove(prefab);
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
