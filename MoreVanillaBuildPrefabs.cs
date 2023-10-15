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
        public const string PluginVersion = "1.0.0";

        Harmony _harmony;

        internal static readonly Dictionary<string, GameObject> PrefabRefs = new();
        internal static readonly Dictionary<string, Piece> DefaultPieceClones = new();
        internal static Dictionary<string, PieceDB> PieceRefs = new();

        internal static bool DisableIndividualConfigEvents = false;

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

            SynchronizationManager.OnConfigurationSynchronized += (obj, attr) =>
            {
                // Re-initialize after syncing data with server
                // and re-enable individual config entry events
                ConfigDataSynced("Config settings synced with server, re-initializing");
                DisableIndividualConfigEvents = false;
            };
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
                // currently always applies patches to all prefabs
                // regardless of whether the pieces are enabled
                // only has to run once this way
                // also prevents trailership instances from
                // becoming unusable if you disable it as a build piece
                PrefabPatcher.PatchPrefabIfNeeded(prefab);
                PrefabRefs.Add(prefab.name, prefab);
            }

            Log.LogInfo($"Found {EligiblePrefabs.Count()} prefabs");

            InitDefaultPieceClones();
        }

        /// <summary>
        ///     Initializes all prefabs to have pieces and
        ///     stores a deep copy of the piece component
        ///     to provide a template to reset to upon
        ///     re-initialization.
        /// </summary>
        private static void InitDefaultPieceClones()
        {
            if (!HasInitializedPrefabs)
            {
                return; // can't run without PrefabRefs
            }

            if (DefaultPieceClones.Count > 0)
            {
                return; // should only ever run once
            }

            Log.LogInfo("Initializing default pieces");

            List<Piece> defaultPieces = new();
            foreach (var prefab in PrefabRefs.Values)
            {
                defaultPieces.Add(PieceHelper.InitPieceComponent(prefab));
            }

            if (PluginConfig.IsVerbosityMedium)
            {
                Log.LogInfo("Initializing default piece icons");
            }

            IconHelper.Instance.GeneratePrefabIcons(PrefabRefs.Values);

            foreach (var prefab in PrefabRefs.Values)
            {
                var clone = prefab.DeepCopy();
                DefaultPieceClones.Add(prefab.name, clone.GetComponent<Piece>());
            }
        }


        /// <summary>
        ///     Initializes references to pieces 
        ///     and their configuration settings
        /// </summary>
        internal static void InitPieceRefs()
        {
            Log.LogInfo("Initializing piece refs");

            if (PieceRefs.Count > 0)
            {
                PieceTable hammerTable = PieceHelper.GetPieceTable(PieceTables.Hammer);

                foreach (PieceDB pdb in PieceRefs.Values)
                {
                    // remove pieces from hammer build table
                    // and sheath hammer if table is open
                    PieceHelper.RemovePieceFromPieceTable(pdb.Prefab, hammerTable);

                    if (Player.m_localPlayer?.GetRightItem()?.m_shared.m_name == "$item_hammer")
                    {
                        Log.LogWarning("Hammer updated through config change, unequipping hammer");
                        Player.m_localPlayer.HideHandItems();
                    }
                }
                PieceRefs.Clear();
            }
            PieceRefs = GeneratePieceRefs();
        }

        /// <summary>
        ///     Create a set of piece refs with each prefab's
        ///     piece reset to the default state and the PieceDB
        ///     containing the configuration settings to apply.
        /// </summary>
        /// <returns></returns>
        private static Dictionary<string, PieceDB> GeneratePieceRefs()
        {
            Dictionary<string, PieceDB> newPieceRefs = new();
            foreach (var prefab in PrefabRefs.Values)
            {
                // reset piece component to match the default piece clone
                prefab.GetComponent<Piece>().CopyFields(DefaultPieceClones[prefab.name]);

                // create piece ref
                newPieceRefs.Add(
                    prefab.name,
                    new PieceDB(
                        PluginConfig.LoadPrefabDB(prefab),
                        prefab.GetComponent<Piece>()
                    )
                );
            }
            return newPieceRefs;
        }

        /// <summary>
        ///     Apply the configuration settings from the 
        ///     PieceDB for each piece in PieceRefs.
        /// </summary>
        internal static void InitPieces()
        {
            Log.LogInfo("Initializing pieces");
            List<Piece> pieces = new();
            foreach (var pieceDB in PieceRefs.Values)
            {
                pieces.Add(CreatePiece(pieceDB));
            }
        }

        /// <summary>
        ///     Creates a piece if needed and configures it based on PieceDB values.
        /// </summary>
        /// <param name="prefabDB"></param>
        /// <returns></returns>
        private static Piece CreatePiece(PieceDB pieceDB)
        {
            var piece = PieceHelper.ConfigurePiece(pieceDB);

            // Fix missing hover text if needed.
            var prefab = pieceDB.Prefab;
            var hover = prefab.GetComponent<HoverText>() ?? prefab.AddComponent<HoverText>();
            if (string.IsNullOrEmpty(hover.m_text))
            {
                hover.enabled = true;
                hover.m_text = piece.m_name;
            }

            return piece;
        }

        /// <summary>
        ///     Add all pieces that are enabled in the cfg file to the hammer build 
        ///     table according to CreatorShop related cfg settings. Allow sets
        ///     all pieces added to the hammer permit deconstruction by players.
        /// </summary>
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

                // Allows pieces added to the hammer to be deconstructed
                // unless they are a ship (to respect vanilla behaviour). 
                if (pieceDB.piece.gameObject.GetComponent<Ship>() == null)
                {
                    pieceDB.piece.m_canBeRemoved = true;
                }

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

        // TODO: Rework event handlers to only update the single piece/thing related to the setting that changed when the SettingChanged event fires, and make different handlers for when Config.Reload or Server Data synced fires. (https://github.com/BepInEx/BepInEx/blob/0d06996b52c0215a8327b8c69a747f425bbb0023/BepInEx/Configuration/ConfigEntryBase.cs#L146)

        /// <summary>
        ///     Method update mod intialization when settings 
        ///     related to piece configuration are changed
        /// </summary>
        /// <param name="o"></param>
        /// <param name="e"></param>
        internal static void PieceSettingChanged(object o, EventArgs e)
        {
            if (DisableIndividualConfigEvents)
            {
                return; // skip event
            }

            if (HasInitializedPrefabs)
            {

                var watch = new System.Diagnostics.Stopwatch();
                if (PluginConfig.IsVerbosityMedium)
                {
                    watch.Start();
                }

                Log.LogInfo("Config setting changed, re-initializing");
                InitPieceRefs();
                InitPieces();
                InitHammer();
                Log.LogInfo("Re-initializing complete");

                if (PluginConfig.IsVerbosityMedium)
                {
                    watch.Stop();
                    Log.LogInfo($"Time to re-initialize: {watch.ElapsedMilliseconds} ms");
                }
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
            if (DisableIndividualConfigEvents)
            {
                return; // skip event
            }
            UpdateNeedsCollisionPatchForGhost();
        }

        /// <summary>
        ///     Method allow both the PlacementSettingChanged 
        ///     and ConfigDataSynced methods to update collision 
        ///     patches when events fire
        /// </summary>
        private static void UpdateNeedsCollisionPatchForGhost()
        {
            if (!HasInitializedPrefabs)
            {
                return;
            }

            if (PluginConfig.IsVerbosityMedium)
            {
                Log.LogInfo("Initializing collision patches");
            }

            foreach (var prefabName in PrefabRefs.Keys)
            {
                if (PluginConfig.PieceConfigEntriesMap[prefabName].placementPatch == null)
                {
                    // No placement patch config entry means that prefab is already
                    // placed in the NeedsCollisionPatchForGhost HashSet by default
                    continue;
                }

                if (PluginConfig.PieceConfigEntriesMap[prefabName].placementPatch.Value)
                {
                    // config is true so add it if not already in HashSet
                    if (!PlacementConfigs.NeedsCollisionPatchForGhost(prefabName))
                    {
                        PlacementConfigs._NeedsCollisionPatchForGhost.Add(prefabName);
                    }
                }
                else if (PlacementConfigs.NeedsCollisionPatchForGhost(prefabName))
                {
                    // config is false so remove it from list if it's in HashSet
                    PlacementConfigs._NeedsCollisionPatchForGhost.Remove(prefabName);
                }
            }
        }

        internal static void ConfigDataSynced(string msg)
        {
            if (HasInitializedPrefabs)
            {

                var watch = new System.Diagnostics.Stopwatch();
                if (PluginConfig.IsVerbosityMedium)
                {
                    watch.Start();
                }

                Log.LogInfo(msg);
                InitPieceRefs();
                InitPieces();
                InitHammer();
                UpdateNeedsCollisionPatchForGhost();
                Log.LogInfo("Re-initializing complete");

                if (PluginConfig.IsVerbosityMedium)
                {
                    watch.Stop();
                    Log.LogInfo($"Time to re-initialize: {watch.ElapsedMilliseconds} ms");
                }
            }
        }
    }
}