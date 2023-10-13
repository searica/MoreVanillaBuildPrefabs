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
        internal static List<PieceDB> PieceRefs = new();
        internal static Dictionary<string, Piece.Requirement[]> DefaultResources = new();

        internal static bool DisableDestructionDrops { get; set; } = false;

        public void Awake()
        {
            Log.Init(Logger);

            PluginConfig.Init(Config);
            PluginConfig.SetUpConfig();

            _harmony = Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), harmonyInstanceId: PluginGuid);

            Game.isModded = true;

            PluginConfig.SetupWatcher();

            SynchronizationManager.OnConfigurationSynchronized += (obj, attr) =>
            {
                if (attr.InitialSynchronization)
                {
                    Log.LogInfo("Initial Config sync event received");
                }
                else
                {
                    Log.LogInfo("Config sync event received");
                }
            };
        }

        public void OnDestroy()
        {
            PluginConfig.Save();
            _harmony?.UnpatchSelf();
        }

        internal static void InitPrefabRefs()
        {
            if (PrefabRefs.Count > 0)
            {
                return;
            }
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
            var piece = prefab?.GetComponent<Piece>();
            if (piece?.m_resources != null)
            {
                // Stop errors on subsequent log ins
                if (!DefaultResources.ContainsKey(prefab.name))
                {
#if DEBUG
                    Log.LogDebug($"Adding default resources for {prefab.name}");
#endif
                    DefaultResources.Add(prefab.name, piece.m_resources);
                }
            }
        }

        internal static void InitPieceRefs()
        {
            Log.LogInfo("InitPieceRefs");

            if (PieceRefs.Count > 0)
            {
                PieceTable hammerTable = PieceHelper.GetPieceTable(PieceTables.Hammer);
                foreach (PieceDB pdb in PieceRefs)
                {
                    PieceHelper.RemovePieceFromPieceTable(pdb.Prefab, hammerTable);
                    // Not sure if I have the right name for the hammer here
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

        private static List<PieceDB> GeneratePieceRefs()
        {
            List<PieceDB> newPieceRefs = new();
            foreach (var prefab in PrefabRefs.Values)
            {
                PrefabDB prefabDB = PluginConfig.LoadPrefabDB(prefab);

                newPieceRefs.Add(
                    new PieceDB(prefabDB, PieceHelper.InitPieceComponent(prefab))
                );
            }
            return newPieceRefs;
        }

        internal static void InitPieces()
        {
            List<Piece> pieces = new();
            foreach (var pieceDB in PieceRefs)
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
            PrefabPatcher.PatchPrefabIfNeeded(prefab);
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
            PieceTable hammerTable = PieceHelper.GetPieceTable(PieceTables.Hammer);
            foreach (var pieceDB in PieceRefs)
            {
                // check if piece is enabled by the mod
                if (!pieceDB.enabled && !PluginConfig.IsForceAllPrefabs)
                {
                    continue;
                }

                // to allow deconstruction of pieces enabled by the mod
                pieceDB.piece.m_targetNonPlayerBuilt = true;


                // Restrict placement of CreatorShop pieces to Admins only
                if (PluginConfig.IsCreatorShopAdminOnly
                    && HammerHelper.IsCreatorShopPiece(pieceDB.piece)
                    && !SynchronizationManager.Instance.PlayerIsAdmin)
                {
                    continue;
                }

                PieceHelper.AddPieceToPieceTable(pieceDB.Prefab, hammerTable);
            }
        }

        /// <summary>
        ///     Method to perform final intialization on start up
        /// </summary>
        internal static void FinalInit()
        {
            InitPieceRefs();
            InitPieces();
            InitHammer();
        }

        /// <summary>
        ///     Method update mod intialization when settings 
        ///     related to piece configuration are changed
        /// </summary>
        /// <param name="o"></param>
        /// <param name="e"></param>
        internal static void PieceSettingChanged(object o, EventArgs e)
        {
            Log.LogInfo("Config setting changed, re-initializing pieces");
            InitPieceRefs();
            InitPieces();
            InitHammer();
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