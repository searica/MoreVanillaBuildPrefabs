// Ignore Spelling: MVBP

using Jotunn.Managers;
using Jotunn.Configs;
using System;
using System.Collections.Generic;
using UnityEngine;
using Logging;
using MVBP.PrefabManagement;
using MVBP.Extensions;
using HarmonyLib;
using System.Linq;

namespace MVBP.PieceManagement;

/// <summary>
///     Manages Piece configuration and updating Hammer piece table.
/// </summary>
[HarmonyPatch]
internal static class PieceTableManager
{
    private sealed class PieceTableConfig
    {
        public bool HideAdvancedMenu;
        public bool CanRemovePieces;

        public PieceTableConfig(bool hideAdvancedMenu, bool canRemovePieces)
        {
            this.HideAdvancedMenu = hideAdvancedMenu;
            this.CanRemovePieces = canRemovePieces;
        }
    }
    private static readonly Dictionary<PieceTable, List<GameObject>> AddedPrefabs = [];
    private static readonly Dictionary<PieceTable, HashSet<int>> AddedPrefabsHashMap = [];
    private static readonly Dictionary<PieceTable, PieceTableConfig> DefaultPieceTableValuesMap = [];
    private static readonly HashSet<PieceTable> PieceTablesCache = [];


    /// <summary>
    ///     Need to cache default menu and removal values for piece tables before other
    ///     mods alter them to prevent compatibility issues and ensure the cached values
    ///     reflect the default vanilla settings.
    /// </summary>
    [HarmonyPostfix]
    [HarmonyPriority(Priority.First)]
    [HarmonyBefore("advize.PlantEverything")]
    [HarmonyPatch(typeof(ZNetScene), nameof(ZNetScene.Awake))]
    private static void InitPieceTableDefaults()
    {
        Initialize();
    }
        
    public static void Initialize()
    {
        Log.LogInfo("Initializing PieceTableManager");

        if (PieceTablesCache.Count == 0)
        {
            PieceTablesCache.UnionWith(Resources.FindObjectsOfTypeAll<PieceTable>());
        }
        else
        {
            PieceTablesCache.UnionWith(PieceManager.Instance.GetPieceTables());
        }

        foreach (PieceTable pieceTable in PieceTablesCache)
        {
            Log.LogInfo($"Found piece table: {pieceTable.name}", Log.InfoLevel.Medium);
            // cache the default menu and removal values for piece tables 
            if (!DefaultPieceTableValuesMap.ContainsKey(pieceTable))
            {
                Log.LogInfo($"Initializing default values for piece table: {pieceTable.name}", Log.InfoLevel.Medium);
                DefaultPieceTableValuesMap.Add(pieceTable, new PieceTableConfig(pieceTable.m_hideAdvancedMenu, pieceTable.m_canRemovePieces));
            }
        }
    }

    public static bool WasPieceAddedToPieceTable(PieceTable pieceTable, Piece piece)
    {
        if (AddedPrefabsHashMap.TryGetValue(pieceTable, out HashSet<int> pieceHashes))
        {
            return pieceHashes.Contains(piece.gameObject.GetPrefabName().GetStableHashCode());
        }
        return false;
    }

    /// <summary>
    ///     Checks if the piece table allows for removing the piece in question, first checks if MVBP has added the piece
    ///     to the piece table and then checks the cached default value of m_canRemovePieces for that piece table and falls
    ///     back to using the current value of m_canRemovePieces if needed. 
    /// </summary>
    /// <param name="pieceTable"></param>
    /// <param name="piece"></param>
    /// <returns></returns>
    public static bool CanToolRemovePiece(PieceTable pieceTable, Piece piece)
    {
        if (WasPieceAddedToPieceTable(pieceTable, piece))
        {
            return true;
        }
        
        if (pieceTable.m_canRemoveFeasts)  // ensure serving tray works properly
        {
            if (piece.GetComponent<Feast>() || (piece.TryGetComponent(out ItemDrop itemDrop) && itemDrop && itemDrop.IsPiece()))
            {
                return true;
            }
        }

        // fall back to relying on unmodified can remove piece value for piece table
        if (DefaultPieceTableValuesMap.TryGetValue(pieceTable, out PieceTableConfig pieceTableConfig))
        {
            return pieceTableConfig.CanRemovePieces;
        }

        return pieceTable.m_canRemovePieces;
    }

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

            // Check if this piece should be added to this pieceTable
            if (PieceTables.GetInternalName(prefabConfig.PieceTable.Value) != pieceTable.name)
            {
                continue;
            }

            // Prevent adding creative mode pieces if not in CreativeMode
            if (!MorePrefabs.IsCreativeMode && PieceCategoryManager.IsCreativeModePiece(prefabConfig.Piece))
            {
                continue;
            }

            // Only add vanilla crops if enabled
            if (!MorePrefabs.HammerCropsEnabled && prefabConfig.PieceGroup == PieceClassification.VanillaCrop)
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

        // respect default values for piece table set ups and only change them if needed
        if (!DefaultPieceTableValuesMap.ContainsKey(pieceTable))
        {
            Log.LogInfo($"Initializing default values for piece table: {pieceTable.name}", Log.InfoLevel.Medium);
            DefaultPieceTableValuesMap.Add(pieceTable, new PieceTableConfig(pieceTable.m_hideAdvancedMenu, pieceTable.m_canRemovePieces));
        }

        if (!pieceGroups.IsEmpty())
        {
            pieceTable.m_hideAdvancedMenu = false;
            pieceTable.m_canRemovePieces = true;
        }
        else if (ModCompat.IsPlantEverythingInstalled() && pieceTable.name == "_CultivatorPieceTable")
        {
            pieceTable.m_hideAdvancedMenu = DefaultPieceTableValuesMap[pieceTable].HideAdvancedMenu;
            pieceTable.m_canRemovePieces = true;
        }
        else 
        {
            pieceTable.m_hideAdvancedMenu = DefaultPieceTableValuesMap[pieceTable].HideAdvancedMenu;
            pieceTable.m_canRemovePieces = DefaultPieceTableValuesMap[pieceTable].CanRemovePieces;
        }

        // add prefabs to piece table
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
            AddedPrefabsHashMap.Add(pieceTable, new HashSet<int>());
        }
        AddedPrefabs[pieceTable].Add(prefab);
        AddedPrefabsHashMap[pieceTable].Add(hash);

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
        if (!AddedPrefabsHashMap.TryGetValue(pieceTable, out HashSet<int> pieceHashes))
        {
            return;
        }

        int numCustomPieces = AddedPieces.Count;
        if (numCustomPieces == 0)
        {
            return;
        }

        int hash;
        for (int i = numCustomPieces - 1; i > -1; i--)
        {
            hash = AddedPieces[i].name.GetStableHashCode();
            if (RemovePieceFromPieceTable(AddedPieces[i], pieceTable))
            {
                pieceHashes.Remove(hash);
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
