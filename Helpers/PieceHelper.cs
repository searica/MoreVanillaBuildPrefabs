using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Jotunn.Managers;
using Jotunn.Configs;
using MoreVanillaBuildPrefabs.Configs;
using MoreVanillaBuildPrefabs.Logging;


namespace MoreVanillaBuildPrefabs.Helpers
{
    internal class PieceHelper
    {
        internal static HashSet<string> AddedPrefabs = new();

        /// <summary>
        ///     Returns a bool indicating if the piece was added by this mod.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static bool IsAddedByMod(string name)
        {
            return AddedPrefabs.Contains(name);
        }

        internal static CraftingStation GetCraftingStation(string name)
        {
            var internalName = CraftingStations.GetInternalName(name);
            var station = ZNetScene.instance?.GetPrefab(internalName)?.GetComponent<CraftingStation>();
            return station;
        }

        /// <summary>
        ///     Get existing objects of type PieceTable.
        /// </summary>
        /// <returns></returns>
        internal static IEnumerable<PieceTable> GetPieceTables()
        {
            return Resources.FindObjectsOfTypeAll<PieceTable>();
        }

        /// <summary>
        ///     Method to get existing piece table.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        internal static PieceTable GetPieceTable(string name)
        {
            var pieceTables = GetPieceTables();
            if (pieceTables != null)
            {
                foreach (var pieceTable in pieceTables)
                {
                    if (pieceTable.name == name) return pieceTable;
                }
            }
            return null;
        }


        /// <summary>
        ///     Get HashSet of all customPiece name attached to existing PieceTable objects.
        /// </summary>
        /// <returns></returns>
        internal static HashSet<string> GetExistingPieceNames()
        {
            var result = GetPieceTables()
                .SelectMany(pieceTable => pieceTable.m_pieces)
                .Select(piece => piece.name);
            return new HashSet<string>(result);

        }

        /// <summary>
        ///     Prevents creation of duplicate ZNetViews
        /// </summary>
        /// <param name="prefab"></param>
        /// <returns></returns>
        internal static bool EnsureNoDuplicateZNetView(GameObject prefab)
        {
            var views = prefab?.GetComponents<ZNetView>();

            if (views == null) return true;

            for (int i = 1; i < views.Length; ++i)
            {
                GameObject.DestroyImmediate(views[i]);
            }

            return views.Length <= 1;
        }

        /// <summary>
        ///     Create and initalize piece component if needed.
        /// </summary>
        /// <param name="prefab"></param>
        internal static Piece InitPieceComponent(GameObject prefab)
        {
            var piece = prefab?.GetComponent<Piece>();
            if (piece == null)
            {
                piece = prefab.AddComponent<Piece>();
                if (piece != null)
                {
                    piece.m_name = prefab.name;
                    piece.m_description = prefab.name;
                    piece.m_groundOnly = false;
                    piece.m_groundPiece = false;
                    piece.m_cultivatedGroundOnly = false;
                    piece.m_waterPiece = false;
                    piece.m_noInWater = false;
                    piece.m_notOnWood = false;
                    piece.m_notOnTiltingSurface = false;
                    piece.m_inCeilingOnly = false;
                    piece.m_notOnFloor = false;
                    piece.m_onlyInTeleportArea = false;
                    piece.m_allowedInDungeons = false;
                    piece.m_clipEverything = PiecePlacement.CanClipEverything(prefab.name);
                    piece.m_clipGround = PiecePlacement.CanClipGround(prefab.name);
                    piece.m_allowRotatedOverlap = true;
                    piece.m_repairPiece = false; // setting to true prevents placement
                    piece.m_canBeRemoved = true;
                    piece.m_onlyInBiome = Heightmap.Biome.None;

                    // to prevent deconstruction of pieces that are not enabled by the mod
                    piece.m_targetNonPlayerBuilt = false;

                    if (PluginConfig.IsVerbose)
                    {
                        Log.LogInfo($"Created Piece component for: {prefab.name}");
                    }
                }
            }
            return piece;
        }

        /// <summary>
        ///     Method to configure Piece fields based on cfg file data.
        /// </summary>
        /// <param name="piece"></param>
        /// <returns></returns>
        internal static Piece ConfigurePiece(
            Piece piece,
            string name,
            string description,
            bool allowedInDungeons,
            string category,
            string craftingStation,
            string requirements
        )
        {
            var pieceCategory = (Piece.PieceCategory)PieceManager.Instance.GetPieceCategory(category);
            var reqs = PluginConfig.CreateRequirementsArray(requirements);
            var station = GetCraftingStation(craftingStation);
            return ConfigurePiece(
                piece,
                name,
                description,
                allowedInDungeons,
                pieceCategory,
                station,
                reqs
            );
        }

        /// <summary>
        ///     Method to configure Piece fields.
        /// </summary>
        /// <param name="piece"></param>
        /// <returns></returns>
        internal static Piece ConfigurePiece(
            Piece piece,
            string name,
            string description,
            bool allowedInDungeons,
            Piece.PieceCategory category,
            CraftingStation craftingStation,
            Piece.Requirement[] requirements
        )
        {
            piece.m_name = name;
            piece.m_description = description;
            piece.m_allowedInDungeons = allowedInDungeons;
            piece.m_category = category;
            piece.m_craftingStation = craftingStation;
            piece.m_resources = requirements;
            return piece;
        }


        /// <summary>
        ///     Method to add a iece to a piece table.
        /// </summary>
        /// <param name="piece"></param>
        /// <param name="pieceTable"></param>
        /// <returns> bool indicating if customPiece was added. </returns>
        internal static void AddPiecesListToPieceTable(IEnumerable<Piece> pieces, string pieceTableName)
        {
            var pieceTable = GetPieceTable(pieceTableName);
            foreach (var piece in pieces)
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
            var piece = prefab.GetComponent<Piece>() ?? throw new Exception($"Prefab {prefab.name} has no Piece component.");
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

            var prefab = piece.gameObject;
            var name = prefab.name;
            var hash = name.GetStableHashCode();

            if (ZNetScene.instance != null && !ZNetScene.instance.m_namedPrefabs.ContainsKey(hash))
            {
                RegisterToZNetScene(prefab);
            }

            pieceTable.m_pieces.Add(prefab);

            if (PluginConfig.IsVerbose)
            {
                Log.LogInfo($"Added Piece {piece.m_name} to PieceTable {pieceTable.name}");
            }

            AddedPrefabs.Add(prefab.name);

            return true;
        }

        /// <summary>
        ///     Register a single prefab to the current <see cref="ZNetScene"/>.<br />
        ///     Checks for existence of the object via GetStableHashCode() and adds the prefab if it is not already added.
        /// </summary>
        /// <param name="gameObject"></param>
        internal static void RegisterToZNetScene(GameObject gameObject)
        {
            var znet = ZNetScene.instance;

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
            Log.LogInfo("RemoveAllCustomPiecesFromPieceTable()");

            int numCustomPieces = AddedPrefabs.Count();
            var prefabsToRemove = AddedPrefabs.ToList();
            var pieceTable = GetPieceTable(pieceTableName);

            if (pieceTable == null)
            {
                Log.LogError($"Could not find piece table: {pieceTableName}");
            }

            foreach (var name in prefabsToRemove)
            {
                RemovePieceFromPieceTable(name, pieceTable);
            }
            Log.LogInfo($"Removed {numCustomPieces - AddedPrefabs.Count} custom pieces");
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
                var prefab = ZNetScene.instance.GetPrefab(name);
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
#if DEBUG
                Log.LogInfo($"{name}: {e}");
#endif
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
#if DEBUG
                Log.LogInfo($"{prefab.name}: {e}");
#endif
                return false;
            }
        }
    }
}


// Jotunn based code

/// <summary>
///     Method to configure and return CustomPiece.
/// </summary>
/// <param name = "piece" ></ param >
/// < returns ></ returns >
//internal static CustomPiece ConfigureCustomPiece(
//    Piece piece,
//    string name,
//    string description,
//    bool allowedInDungeons,
//    string category,
//    string pieceTable,
//    string craftingStation,
//    string requirements
//)
//{
//    PieceConfig pieceConfig = new()
//    {
//        name = name,
//        Description = description,
//        allowedInDungeons = allowedInDungeons,
//        category = category,
//        PieceTable = pieceTable,
//        craftingStation = craftingStation,
//        requirements = PluginConfig.CreateRequirementConfigsArray(requirements)
//    };
//    CustomPiece customPiece = new(piece.gameObject, true, pieceConfig);
//    return customPiece;
//}

//internal static bool AddCustomPiece(CustomPiece customPiece)
//{
//    customPiece.PiecePrefab.FixReferences(true);
//    var flag = PieceManager.Instance.AddPiece(customPiece);
//    PieceManager.Instance.RegisterPieceInPieceTable(customPiece.PiecePrefab, customPiece.PieceTable, customPiece.category);
//    AddedPrefabs.Add(customPiece.PiecePrefab.name);
//    return flag;
//}

/// <summary>
///     Remove all custom pieces added by this mod.
/// </summary>
//internal static void RemoveCustomPieces()
//{
//    Log.LogInfo("RemoveCustomPieces()");

//    int numCustomPieces = AddedPrefabs.Count();
//    var prefabsToRemove = AddedPrefabs.ToList();
//    foreach (var name in prefabsToRemove)
//    {
//        RemoveCustomPiece(name);
//    }
//    Log.LogInfo($"Removed {numCustomPieces - AddedPrefabs.Count} custom pieces");
//}

//        internal static bool RemoveCustomPiece(string name)
//        {
//            try // Remove customPiece from PieceTable
//            {
//                var customPiece = PieceManager.Instance.GetPiece(name);
//                var pieceTable = PieceManager.Instance.GetPieceTable(customPiece.PieceTable);

//                pieceTable.m_pieces.Remove(customPiece.PiecePrefab);
//                PieceManager.Instance.RemovePiece(name);

//                AddedPrefabs.Remove(customPiece.PiecePrefab.name);
//                return true;
//            }
//            catch (Exception e)
//            {
//#if DEBUG
//                Log.LogInfo($"{name}: {e}");
//#endif
//                return false;
//            }
//        }