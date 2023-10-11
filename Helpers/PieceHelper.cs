using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Jotunn.Managers;
using MoreVanillaBuildPrefabs.Configs;
using MoreVanillaBuildPrefabs.Logging;
using Jotunn.Configs;
using Jotunn.Entities;
using Jotunn;

namespace MoreVanillaBuildPrefabs.Helpers
{
    internal class PieceHelper
    {
        public static HashSet<string> AddedPrefabs = new();

        /// <summary>
        ///     Get HashSet of all customPiece name attached to existing PieceTable objects.
        /// </summary>
        /// <returns></returns>
        internal static HashSet<string> GetExistingPieceNames()
        {
            var result = PieceManager.Instance.GetPieceTables()
                .SelectMany(pieceTable => pieceTable.m_pieces)
                .Select(piece => piece.name);
            return new HashSet<string>(result);

        }

        /// <summary>
        ///     Create and initalize piece component if needed.
        /// </summary>
        /// <param name="prefab"></param>
        internal static void InitPieceComponent(GameObject prefab)
        {
            if (prefab?.GetComponent<Piece>() == null)
            {
                var piece = prefab.AddComponent<Piece>();
                if (piece != null)
                {
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
                    piece.m_clipEverything = !DefaultConfigs.RestrictClipping.Contains(prefab.name);
                    piece.m_clipGround = DefaultConfigs.CanClipGround.Contains(prefab.name);
                    piece.m_allowRotatedOverlap = true;
                    piece.m_repairPiece = false; // setting to true prevents placement
                    piece.m_canBeRemoved = true;
                    piece.m_onlyInBiome = Heightmap.Biome.None;
                    if (PluginConfig.IsVerbose())
                    {
                        Log.LogInfo($"Creating Piece for: {prefab.name}");
                    }
                }
            }
        }

        /// <summary>
        ///     Method to configure and return CustomPiece.
        /// </summary>
        /// <param name="piece"></param>
        /// <returns></returns>
        internal static CustomPiece ConfigureCustomPiece(
            Piece piece,
            string name,
            string description,
            bool allowedInDungeons,
            string category,
            string pieceTable,
            string craftingStation,
            string requirements
        )
        {
            PieceConfig pieceConfig = new()
            {
                Name = name,
                Description = description,
                AllowedInDungeons = allowedInDungeons,
                Category = category,
                PieceTable = pieceTable,
                CraftingStation = craftingStation,
                Requirements = PluginConfig.CreateRequirementConfigsArray(requirements)
            };
            CustomPiece customPiece = new(piece.gameObject, true, pieceConfig);
            return customPiece;
        }

        internal static bool AddCustomPiece(CustomPiece customPiece)
        {
            customPiece.PiecePrefab.FixReferences(true);
            var flag = PieceManager.Instance.AddPiece(customPiece);
            PieceManager.Instance.RegisterPieceInPieceTable(customPiece.PiecePrefab, customPiece.PieceTable, customPiece.Category);
            AddedPrefabs.Add(customPiece.PiecePrefab.name);
            return flag;
        }

        /// <summary>
        ///     Remove all custom pieces added by this mod.
        /// </summary>
        internal static void RemoveCustomPieces()
        {
            Log.LogInfo("RemoveCustomPieces()");

            int numCustomPieces = PieceHelper.AddedPrefabs.Count();
            var prefabsToRemove = PieceHelper.AddedPrefabs.ToList();
            foreach (var name in prefabsToRemove)
            {
                RemoveCustomPiece(name);
            }
            Log.LogInfo($"Removed {numCustomPieces - PieceHelper.AddedPrefabs.Count} custom pieces");
        }

        internal static bool RemoveCustomPiece(string name)
        {
            try // Remove customPiece from PieceTable
            {
                var customPiece = PieceManager.Instance.GetPiece(name);
                var pieceTable = PieceManager.Instance.GetPieceTable(customPiece.PieceTable);

                pieceTable.m_pieces.Remove(customPiece.PiecePrefab);
                PieceManager.Instance.RemovePiece(name);

                AddedPrefabs.Remove(customPiece.PiecePrefab.name);
                return true;
            }
            catch (Exception e)
            {
#if DEBUG
                Log.LogInfo($"{name}: {e}");
#endif
                return false;
            }
        }



        // Obsolete code

        ///// <summary>
        /////     Get existing objects of type PieceTable.
        ///// </summary>
        ///// <returns></returns>
        //internal static IEnumerable<PieceTable> GetPieceTables()
        //{
        //    return Resources.FindObjectsOfTypeAll<PieceTable>();
        //}

        ///// <summary>
        /////     Method to get existing customPiece table.
        ///// </summary>
        ///// <param name="name"></param>
        ///// <returns></returns>
        //internal static PieceTable GetPieceTable(string name)
        //{
        //    var pieceTables = GetPieceTables();
        //    if (pieceTables != null)
        //    {
        //        foreach (var pieceTable in pieceTables)
        //        {
        //            if (pieceTable.name == name) return pieceTable;
        //        }
        //    }
        //    return null;
        //}

        //        /// <summary>
        //        ///     Method to configure customPiece fields based on cfg file data.
        //        /// </summary>
        //        /// <param name="piece"></param>
        //        /// <returns></returns>
        //        internal static Piece ConfigurePiece(
        //            Piece piece,
        //            string name,
        //            string description,
        //            bool allowedInDungeons,
        //            string category,
        //            string craftingStation,
        //            string requirements
        //        )
        //        {
        //            var pieceCategory = (Piece.PieceCategory)PieceManager.Instance.GetPieceCategory(category);
        //            var reqs = PluginConfig.CreateRequirementsArray(requirements);
        //            var station = CraftingStations.GetCraftingStation(craftingStation);
        //            return ConfigurePiece(
        //                piece,
        //                name,
        //                description,
        //                allowedInDungeons,
        //                pieceCategory,
        //                station,
        //                reqs
        //            );
        //        }

        //        /// <summary>
        //        ///     Method to configure customPiece fields.
        //        /// </summary>
        //        /// <param name="piece"></param>
        //        /// <returns></returns>
        //        internal static Piece ConfigurePiece(
        //            Piece piece,
        //            string name,
        //            string description,
        //            bool allowedInDungeons,
        //            Piece.PieceCategory category,
        //            CraftingStation craftingStation,
        //            Piece.Requirement[] requirements
        //        )
        //        {
        //            piece.m_name = name;
        //            piece.m_description = description;
        //            piece.m_allowedInDungeons = allowedInDungeons;
        //            piece.m_category = category;
        //            piece.m_craftingStation = craftingStation;
        //            piece.m_resources = requirements;
        //            return piece;
        //        }

        //        /// <summary>
        //        ///     Method to add a prefab to a customPiece table.
        //        /// </summary>
        //        /// <param name="piece"></param>
        //        /// <param name="pieceTable"></param>
        //        /// <returns> bool indicating if customPiece was added. </returns>
        //        internal static bool AddPieceToPieceTable(GameObject prefab, PieceTable pieceTable)
        //        {
        //            var piece = prefab.GetComponent<Piece>() ?? throw new Exception($"Prefab {prefab.name} has no Piece component attached");
        //            return AddPieceToPieceTable(piece, pieceTable);
        //        }

        //        /// <summary>
        //        ///     Method to add a customPiece to a customPiece table.
        //        /// </summary>
        //        /// <param name="piece"></param>
        //        /// <param name="pieceTable"></param>
        //        /// <returns> bool indicating if customPiece was added. </returns>
        //        internal static bool AddPieceToPieceTable(Piece piece, PieceTable pieceTable)
        //        {

        //            if (!piece || !pieceTable || pieceTable.m_pieces == null || pieceTable.m_pieces.Contains(piece.gameObject))
        //            {
        //                return false;
        //            }

        //            var prefab = piece.gameObject;
        //            var name = prefab.name;
        //            var hash = name.GetStableHashCode();

        //            if (ZNetScene.instance != null && !ZNetScene.instance.m_namedPrefabs.ContainsKey(hash))
        //            {
        //                RegisterToZNetScene(prefab);
        //            }

        //            pieceTable.m_pieces.Add(prefab);

        //            if (PluginConfig.IsVerbose())
        //            {
        //                Log.LogInfo($"Added Piece {piece.m_name} to PieceTable {pieceTable.name}");
        //            }
        //            AddedPrefabs.Add(prefab.name);

        //            return true;
        //        }

        //        /// <summary>
        //        ///     Register a single prefab to the current <see cref="ZNetScene"/>.<br />
        //        ///     Checks for existence of the object via GetStableHashCode() and adds the prefab if it is not already added.
        //        /// </summary>
        //        /// <param name="gameObject"></param>
        //        public static void RegisterToZNetScene(GameObject gameObject)
        //        {
        //            var znet = ZNetScene.instance;

        //            if (znet)
        //            {
        //                string name = gameObject.name;
        //                int hash = name.GetStableHashCode();

        //                if (znet.m_namedPrefabs.ContainsKey(hash))
        //                {
        //                    Log.LogDebug($"Prefab {name} already in ZNetScene");
        //                }
        //                else
        //                {
        //                    if (gameObject.GetComponent<ZNetView>() != null)
        //                    {
        //                        znet.m_prefabs.Add(gameObject);
        //                    }
        //                    else
        //                    {
        //                        znet.m_nonNetViewPrefabs.Add(gameObject);
        //                    }
        //                    znet.m_namedPrefabs.Add(hash, gameObject);
        //                    Log.LogDebug($"Added prefab {name}");
        //                }
        //            }
        //        }

        //        internal static bool RemovePieceFromPieceTable(string name, PieceTable pieceTable)
        //        {
        //            try // Remove customPiece from PieceTable
        //            {
        //                var prefab = ZNetScene.instance.GetPrefab(name);
        //                pieceTable.m_pieces.Remove(prefab);
        //                AddedPrefabs.Remove(prefab.name);
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

        //        /// <summary>
        //        ///     Helper to get existing crafting station names
        //        /// </summary>
        //        public static class CraftingStations
        //        {
        //            /// <summary>
        //            ///     No crafting station
        //            /// </summary>
        //            internal static string None => string.Empty;

        //            /// <summary>
        //            ///    Workbench crafting station
        //            /// </summary>
        //            internal static string Workbench => "piece_workbench";

        //            /// <summary>
        //            ///    Forge crafting station
        //            /// </summary>
        //            internal static string Forge => "forge";

        //            /// <summary>
        //            ///     Stonecutter crafting station
        //            /// </summary>
        //            internal static string Stonecutter => "piece_stonecutter";

        //            /// <summary>
        //            ///     Cauldron crafting station
        //            /// </summary>
        //            internal static string Cauldron => "piece_cauldron";

        //            /// <summary>
        //            ///     Artisan table crafting station
        //            /// </summary>
        //            internal static string ArtisanTable => "piece_artisanstation";

        //            /// <summary>
        //            ///     Black forge crafting station
        //            /// </summary>
        //            internal static string BlackForge => "blackforge";

        //            /// <summary>
        //            ///     Galdr table crafting station
        //            /// </summary>
        //            internal static string GaldrTable => "piece_magetable";


        //            private static readonly Dictionary<string, string> NamesMap = new()
        //            {
        //                { nameof(None), None },
        //                { nameof(Workbench), Workbench },
        //                { nameof(Forge), Forge },
        //                { nameof(Stonecutter), Stonecutter },
        //                { nameof(Cauldron), Cauldron },
        //                { nameof(ArtisanTable), ArtisanTable },
        //                { nameof(BlackForge), BlackForge },
        //                { nameof(GaldrTable), GaldrTable },
        //            };

        //            private static readonly AcceptableValueList<string> AcceptableValues = new(NamesMap.Keys.ToArray());

        //            /// <summary>
        //            ///     Gets the human readable name to internal names map
        //            /// </summary>
        //            /// <returns></returns>
        //            internal static Dictionary<string, string> GetNames()
        //            {
        //                return NamesMap;
        //            }

        //            /// <summary>
        //            ///     Get a <see cref="AcceptableValueList{T}"/> of all crafting station names.
        //            ///     This can be used to create a <see cref="ConfigEntry{T}"/> where only valid crafting stations can be selected.<br/><br/>
        //            ///     Example:
        //            ///     <code>
        //            ///         var stationConfig = Config.Bind("Section", "Key", nameof(CraftingStations.Workbench), new ConfigDescription("Description", CraftingStations.GetAcceptableValueList()));
        //            ///     </code>
        //            /// </summary>
        //            /// <returns></returns>
        //            internal static AcceptableValueList<string> GetAcceptableValueList()
        //            {
        //                return AcceptableValues;
        //            }

        //            /// <summary>
        //            ///     Get the internal name for a crafting station from its human readable name.
        //            /// </summary>
        //            /// <param name="craftingStation"></param>
        //            /// <returns>
        //            ///     The matched internal name.
        //            ///     If the craftingStation parameter is null or empty, <see cref="None"/> is returned.
        //            ///     Otherwise the unchanged craftingStation parameter is returned.
        //            /// </returns>
        //            /// 
        //            internal static string GetInternalName(string craftingStation)
        //            {
        //                if (string.IsNullOrEmpty(craftingStation))
        //                {
        //                    return None;
        //                }

        //                if (NamesMap.TryGetValue(craftingStation, out string internalName))
        //                {
        //                    return internalName;
        //                }

        //                return craftingStation;
        //            }

        //            /// <summary>
        //            ///     Get CraftingStation object from either the human readable or internal name.
        //            /// </summary>
        //            /// <param name="craftingStation"></param>
        //            /// <returns></returns>
        //            internal static CraftingStation GetCraftingStation(string name)
        //            {
        //                var internalName = GetInternalName(name);
        //                var station = ZNetScene.instance?.GetPrefab(internalName)?.GetComponent<CraftingStation>();
        //                return station;
        //            }
        //        } // End CraftingStations
    }
}
