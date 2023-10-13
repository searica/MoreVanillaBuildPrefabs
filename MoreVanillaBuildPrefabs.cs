using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using UnityEngine;
using BepInEx;
using HarmonyLib;
using Jotunn.Managers;
using Jotunn.Utils;
using Jotunn.Configs;

using MoreVanillaBuildPrefabs.Configs;
using MoreVanillaBuildPrefabs.Logging;
using MoreVanillaBuildPrefabs.Helpers;



namespace MoreVanillaBuildPrefabs
{
    [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
    [BepInDependency(Jotunn.Main.ModGuid, Jotunn.Main.Version)]
    [NetworkCompatibility(CompatibilityLevel.VersionCheckOnly, VersionStrictness.Patch)]
    public class MoreVanillaBuildPrefabs : BaseUnityPlugin
    {
        public const string PluginName = "MoreVanillaBuildPrefabs";
        internal const string Author = "Searica";
        public const string PluginGuid = $"{Author}.Valheim.{PluginName}";
        public const string PluginVersion = "0.3.7";

        Harmony _harmony;

        internal static readonly Dictionary<string, GameObject> PrefabRefs = new();
        internal static Dictionary<string, PieceDB> PieceRefs = new();
        internal static Dictionary<string, Piece.Requirement[]> DefaultResources = new();

        internal static bool DisableDropOnDestroyed { get; set; } = false;

        private static bool HasInitializedPrefabs => PrefabRefs.Count > 0;


        public void Awake()
        {
            Log.Init(Logger);

            PluginConfig.Init(Config);
            PluginConfig.SetUpConfig();

            _harmony = Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), harmonyInstanceId: PluginGuid);

            Game.isModded = true;

            PluginConfig.SetupWatcher();
        }

        public void OnDestroy()
        {
            PluginConfig.Save();
            _harmony?.UnpatchSelf();
        }

        /// <summary>
        ///     Returns a bool indicating if the prefab has been changed by mod.
        /// </summary>
        /// <param name="prefabName"></param>
        /// <returns></returns>
        internal static bool IsChangedByMod(string prefabName)
        {
            // Unsure if I should access PieceRefs or PrefabRefs here
            // both could work but PrefabRefs is unchanging after initial log in
            // Unchanging could be good but could also be a bad thing
            // That PieceRefs changes could maybe lead to errors or prevent me
            // trying to access something that isn't a thing anymore somehow?
            return PrefabRefs.ContainsKey(prefabName);
        }

        /// <summary>
        ///     Returns true if the piece is one the mod touches and it 
        ///     is currently enabled for building. Returns false if the
        ///     piece is not a custom piece or it is not enabled.
        /// </summary>
        /// <param name="prefabName"></param>
        /// <returns></returns>
        internal static bool IsCustomPieceEnabled(string prefabName)
        {
            if (PieceRefs.ContainsKey(prefabName))
            {
                return PieceRefs[prefabName].enabled;
            }
            return false;
        }

        internal static void InitPrefabRefs()
        {
            if (PrefabRefs.Count > 0)
            {
                return;
            }
            Log.LogInfo("Initializing prefabs");
            var PieceNameCache = PieceHelper.GetExistingPieceNames();

            var EligiblePrefabs = ZNetScene.instance.m_prefabs
            .Where(
                go => go.transform.parent == null
                && !PieceNameCache.Contains(go.name)
                && !IgnoredPrefabs.ShouldIgnorePrefab(go)
            ).ToList();

            foreach (var prefab in EligiblePrefabs)
            {
                SaveDefaultResources(prefab);
                // currently always applies patches to all prefabs
                // regardless of whether the pieces are enabled
                // only has to run once this way
                // also prevents trailership instances from
                // becoming unusable if you disable it as a build piece
                PrefabPatcher.PatchPrefabIfNeeded(prefab);
                PrefabRefs.Add(prefab.name, prefab);
            }
            Log.LogInfo($"Found {EligiblePrefabs.Count()} prefabs");
        }

        /// <summary>
        ///     If prefab has an existing piece with existing build requirements,
        ///     then add the default build requirements to DefaultResources dictionary 
        ///     if they have not already been added.
        /// </summary>
        /// <param PrefabName="prefab"></param>
        internal static void SaveDefaultResources(GameObject prefab)
        {
            // Stop errors on subsequent log ins
            if (!DefaultResources.ContainsKey(prefab.name))
            {
#if DEBUG
                if (PluginConfig.IsVerbose)
                {
                    Log.LogInfo($"Adding default resources for {prefab.name}");
                }
#endif
                var piece = prefab?.GetComponent<Piece>();

                if (piece != null)
                {
                    DefaultResources.Add(prefab.name, piece.m_resources);
                }
                else
                {
                    DefaultResources.Add(prefab.name, Array.Empty<Piece.Requirement>());
                }
            }
        }

        internal static void InitPieceRefs()
        {
            Log.LogInfo("Initializing piece refs");

            if (PieceRefs.Count > 0)
            {
                PieceTable hammerTable = PieceHelper.GetPieceTable(PieceTables.Hammer);

                foreach (PieceDB pdb in PieceRefs.Values)
                {
                    PieceHelper.RemovePieceFromPieceTable(pdb.Prefab, hammerTable);

                    if (Player.m_localPlayer?.GetRightItem()?.m_shared.m_name == "$item_hammer")
                    {
                        Log.LogWarning("Hammer updated through config change, unequipping hammer");
                        Player.m_localPlayer.HideHandItems();
                    }

                    DestroyImmediate(pdb.Prefab.GetComponent<Piece>());
                }
                PieceRefs.Clear();
            }
            PieceRefs = GeneratePieceRefs();
        }

        private static Dictionary<string, PieceDB> GeneratePieceRefs()
        {
            Dictionary<string, PieceDB> newPieceRefs = new();
            foreach (var prefab in PrefabRefs.Values)
            {
                PrefabDB prefabDB = PluginConfig.LoadPrefabDB(prefab);
                newPieceRefs.Add(
                    prefab.name,
                    new PieceDB(prefabDB, PieceHelper.InitPieceComponent(prefab))
                );
            }
            return newPieceRefs;
        }

        internal static void InitPieces()
        {
            Log.LogInfo("Initializing pieces");
            List<Piece> pieces = new();
            foreach (var pieceDB in PieceRefs.Values)
            {
                pieces.Add(CreatePiece(pieceDB));
            }
            IconHelper.GeneratePrefabIcons(pieces);
        }

        /// <summary>
        ///     Creates a piece if needed and configures it based on PrefabDB values.
        /// </summary>
        /// <param name="prefabDB"></param>
        /// <returns></returns>
        private static Piece CreatePiece(PieceDB pieceDB)
        {
            var prefab = pieceDB.Prefab;
            var piece = PieceHelper.ConfigurePiece(
                pieceDB.piece,
                NameHelper.FormatPrefabName(prefab.name),
                NameHelper.GetPrefabDescription(prefab),
                pieceDB.allowedInDungeons,
                pieceDB.category,
                pieceDB.craftingStation,
                pieceDB.requirements
            );

            // Fix missing hover text if needed.
            var hover = prefab.GetComponent<HoverText>() ?? prefab.AddComponent<HoverText>();
            if (string.IsNullOrEmpty(hover.m_text))
            {
                hover.enabled = true;
                hover.m_text = piece.m_name;
            }

            return piece;
        }

        internal static void InitHammer()
        {
            Log.LogInfo("Initializing hammer build table");
            PieceTable hammerTable = PieceHelper.GetPieceTable(PieceTables.Hammer);
            foreach (var pieceDB in PieceRefs.Values)
            {
                // check if piece is enabled by the mod
                if (!pieceDB.enabled && !PluginConfig.IsForceAllPrefabs)
                {
                    continue;
                }

                // to allow deconstruction of pieces enabled by the mod
                pieceDB.piece.m_canBeRemoved = true;


                // Restrict placement of CreatorShop pieces to Admins only
                if (PluginConfig.IsCreatorShopAdminOnly
                    && CreatorShopHelper.IsCreatorShopPiece(pieceDB.piece)
                    && !SynchronizationManager.Instance.PlayerIsAdmin)
                {
                    continue;
                }

                PieceHelper.AddPieceToPieceTable(pieceDB.Prefab, hammerTable);
            }
        }


        /// <summary>
        ///     Method update mod intialization when settings 
        ///     related to piece configuration are changed
        /// </summary>
        /// <param name="o"></param>
        /// <param name="e"></param>
        internal static void PieceSettingChanged(object o, EventArgs e)
        {
            if (HasInitializedPrefabs)
            {
                Log.LogInfo("Config setting changed, re-initializing");
                InitPieceRefs();
                InitPieces();
                InitHammer();
                Log.LogInfo("Re-initializing complete");
            }
        }

        /// <summary>
        ///     Method to update HashSet of prefabs
        ///     that require placement collision patch
        /// </summary>
        /// <param name="o"></param>
        /// <param name="e"></param>
        internal static void PlacementSettingChanged(object o, EventArgs e)
        {
            foreach (var prefabName in PrefabRefs.Keys)
            {
                if (PluginConfig.PieceConfigEntriesMap[prefabName].placementPatch.Value)
                {
                    // config is true so add it if not already in HashSet
                    if (!PiecePlacement.NeedsCollisionPatchForGhost(prefabName))
                    {
                        PiecePlacement._NeedsCollisionPatchForGhost.Add(prefabName);
                    }
                }
                else if (PiecePlacement.NeedsCollisionPatchForGhost(prefabName))
                {
                    // config is false so remove it from list if it's in HashSet
                    PiecePlacement._NeedsCollisionPatchForGhost.Remove(prefabName);
                }
            }
        }
    }
}