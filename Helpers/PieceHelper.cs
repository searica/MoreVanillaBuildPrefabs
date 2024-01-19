// Ignore Spelling: MVBP

using Jotunn.Configs;
using Jotunn.Managers;
using MVBP.Configs;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MVBP.Helpers {
    internal static class PieceHelper {
        internal static readonly HashSet<string> AddedPrefabs = new();
        private static readonly HashSet<string> AddedPieceComponent = new();

        internal static CraftingStation GetCraftingStation(string name) {
            var internalName = CraftingStations.GetInternalName(name);
            var station = ZNetScene.instance?.GetPrefab(internalName)?.GetComponent<CraftingStation>();
            return station;
        }

        /// <summary>
        ///     Get existing objects of type PieceTable.
        /// </summary>
        /// <returns></returns>
        internal static IEnumerable<PieceTable> GetPieceTables() {
            return Resources.FindObjectsOfTypeAll<PieceTable>();
        }

        /// <summary>
        ///     Get HashSet of all customPiece name attached to existing PieceTable objects.
        /// </summary>
        /// <returns></returns>
        internal static HashSet<string> GetExistingPieceNames() {
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
        internal static bool EnsureNoDuplicateZNetView(GameObject prefab) {
            if (!prefab) {
                return false;
            }

            var views = prefab.GetComponents<ZNetView>();

            if (views == null) {
                return true;
            }

            for (int i = 1; i < views.Length; ++i) {
                GameObject.DestroyImmediate(views[i]);
            }

            return views.Length <= 1;
        }

        /// <summary>
        ///     Create and initialize piece component if needed.
        ///     Sets m_canBeRemoved to false by default when adding
        ///     piece components prefabs that are missing them.
        /// </summary>
        /// <param name="prefab"></param>
        internal static Piece InitPieceComponent(GameObject prefab) {
            var piece = prefab.GetComponent<Piece>();

            if (!piece) {
                piece = prefab.AddComponent<Piece>();

                piece.enabled = false; // disable the component unless enabled in config

                piece.m_enabled = false;
                piece.m_name = prefab.name;
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
                piece.m_clipEverything = false;
                piece.m_clipGround = false;
                piece.m_allowRotatedOverlap = true;
                piece.m_repairPiece = false; // setting to true prevents placement
                piece.m_onlyInBiome = Heightmap.Biome.None;

                // if it doesn't normally have a piece component then mobs shouldn't randomly target it
                piece.m_randomTarget = false;
                //piece.m_targetNonPlayerBuilt = false;

                // I could change this value for player-built pieces in piece.Awake and piece.SetCreator

                // to prevent deconstruction of pieces that are not enabled by the mod
                piece.m_canBeRemoved = false;

                AddedPieceComponent.Add(prefab.name);
                Log.LogInfo($"Created Piece component for: {prefab.name}", LogLevel.High);

            }

            return piece;
        }

        /// <summary>
        ///     Method to configure Piece fields based on a PieceDB instance.
        /// </summary>
        /// <param name="pieceDB"></param>
        /// <returns></returns>
        internal static Piece ConfigurePiece(PieceDB pieceDB) {
            var piece = pieceDB.piece;

            var name = NameMaker.FormatPrefabName(pieceDB);
            var description = NameMaker.GetPrefabDescription(pieceDB);
            var pieceCategory = (Piece.PieceCategory)PieceManager.Instance.GetPieceCategory(pieceDB.category);

            if (AddedPieceComponent.Contains(pieceDB.name)) {
                piece.enabled = pieceDB.enabled; // set component enabled/disabled for components added by MVBP
                piece.m_enabled = pieceDB.enabled; // set piece visible in PieceTable based on MVBP config
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
            if (!PieceCategoryHelper.IsCreativeModePiece(pieceDB.piece) &&
                !pieceDB.Prefab.GetComponent<Ship>() &&
                !pieceDB.Prefab.GetComponent<Vagon>()) {
                pieceDB.piece.m_canBeRemoved = pieceDB.enabled;
            }
            else {
                pieceDB.piece.m_canBeRemoved = false;
            }

            return piece;
        }

        /// <summary>
        ///     Create piece requirements array from pieceDB and modify it to prevent
        ///     exploits if the piece has a pickable component.
        /// </summary>
        /// <param name="pieceDB"></param>
        /// <returns></returns>
        private static Piece.Requirement[] ConfigurePieceRequirements(PieceDB pieceDB) {
            var reqs = RequirementsHelper.CreateRequirementsArray(pieceDB.requirements);

            if (pieceDB.piece.TryGetComponent(out MineRock mineRock)) {
                reqs = RequirementsHelper.AddMineRockDropsToRequirements(reqs, mineRock);
            }

            if (pieceDB.piece.TryGetComponent(out MineRock5 mineRock5)) {
                reqs = RequirementsHelper.AddMineRock5DropsToRequirements(reqs, mineRock5);
            }

            if (pieceDB.piece.TryGetComponent(out Pickable pickable)) {
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
        internal static void AddPiecesListToPieceTable(IEnumerable<Piece> pieces, string pieceTableName) {
            var pieceTable = PieceManager.Instance.GetPieceTable(pieceTableName);

            foreach (var piece in pieces) {
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
        internal static bool AddPieceToPieceTable(GameObject prefab, PieceTable pieceTable) {
            var piece = prefab.GetComponent<Piece>() ?? throw new Exception($"Prefab {prefab.name} has no Piece component.");

            return AddPieceToPieceTable(piece, pieceTable);
        }

        /// <summary>
        ///     Method to add a piece to a piece table.
        /// </summary>
        /// <param name="piece"></param>
        /// <param name="pieceTable"></param>
        /// <returns> bool indicating if customPiece was added. </returns>
        internal static bool AddPieceToPieceTable(Piece piece, PieceTable pieceTable) {
            if (!piece || !pieceTable || pieceTable.m_pieces == null || pieceTable.m_pieces.Contains(piece.gameObject)) {
                return false;
            }

            var prefab = piece.gameObject;
            var name = prefab.name;
            var hash = name.GetStableHashCode();

            if (ZNetScene.instance && !ZNetScene.instance.m_namedPrefabs.ContainsKey(hash)) {
                RegisterToZNetScene(prefab);
            }

            pieceTable.m_pieces.Add(prefab);
            AddedPrefabs.Add(prefab.name);
            Log.LogInfo($"Added Piece {piece.m_name} to PieceTable {pieceTable.name}", LogLevel.High);
            return true;
        }

        /// <summary>
        ///     Register a single prefab to the current <see cref="ZNetScene"/>.<br />
        ///     Checks for existence of the object via GetStableHashCode() and adds the prefab if it is not already added.
        /// </summary>
        /// <param name="gameObject"></param>
        internal static void RegisterToZNetScene(GameObject gameObject) {
            var znet = ZNetScene.instance;

            if (znet) {
                string name = gameObject.name;
                int hash = name.GetStableHashCode();

                if (znet.m_namedPrefabs.ContainsKey(hash)) {
                    Log.LogDebug($"Prefab {name} already in ZNetScene");
                }
                else {
                    if (gameObject.GetComponent<ZNetView>() != null) {
                        znet.m_prefabs.Add(gameObject);
                    }
                    else {
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
        internal static void RemoveAllCustomPiecesFromPieceTable(string pieceTableName) {
            Log.LogInfo("RemoveAllCustomPiecesFromPieceTable()", LogLevel.Medium);

            int numCustomPieces = AddedPrefabs.Count;
            var prefabsToRemove = AddedPrefabs.ToList();
            var pieceTable = PieceManager.Instance.GetPieceTable(pieceTableName);

            if (pieceTable == null) {
                Log.LogError($"Could not find piece table: {pieceTableName}");
            }

            foreach (var name in prefabsToRemove) {
                RemovePieceFromPieceTable(name, pieceTable);
            }

            Log.LogInfo($"Removed {numCustomPieces - AddedPrefabs.Count} custom pieces", LogLevel.Medium);
        }

        /// <summary>
        ///     Remove piece from PieceTable
        /// </summary>
        /// <param name="name"></param>
        /// <param name="pieceTable"></param>
        /// <returns></returns>
        internal static bool RemovePieceFromPieceTable(string name, PieceTable pieceTable) {
            try {
                var prefab = ZNetScene.instance.GetPrefab(name);
                if (pieceTable.m_pieces.Contains(prefab)) {
                    pieceTable.m_pieces.Remove(prefab);
                    AddedPrefabs.Remove(prefab.name);
                    return true;
                }
                return false;
            }
            catch (Exception e) {
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
        internal static bool RemovePieceFromPieceTable(GameObject prefab, PieceTable pieceTable) {
            try {
                if (pieceTable.m_pieces.Contains(prefab)) {
                    pieceTable.m_pieces.Remove(prefab);
                    AddedPrefabs.Remove(prefab.name);
                    return true;
                }
                return false;
            }
            catch (Exception e) {
                Log.LogInfo($"{prefab.name}: {e}", LogLevel.Medium);
                return false;
            }
        }
    }
}