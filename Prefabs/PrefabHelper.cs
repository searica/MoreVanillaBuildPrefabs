using System;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using Jotunn.Managers;
using MoreVanillaBuildPrefabs.Utils;

namespace MoreVanillaBuildPrefabs
{
    public class PrefabHelper
    {
        // keys are piece names and values are prefab names
        public static List<GameObject> EligiblePrefabs = new();

        public static HashSet<string> AddedPrefabs = new();

        public static Dictionary<string, Piece.Requirement[]> DefaultResources = new();

        private static HashSet<string> PieceNameCache = null;

        private static readonly HashSet<string> IgnoredPrefabs = new() {
            "Player",
            "Valkyrie",
            "HelmetOdin",
            "CapeOdin",
            "CastleKit_pot03",
            "Ravens",
            "TERRAIN_TEST",
            "PlaceMarker",
            "Circle_section",
            "guard_stone_test",
            "Haldor",
            "odin",
            "dvergrprops_wood_stake",
            "Hildir",
            //Placement is glitchy
            "demister_ball",
            "CargoCrate"
        };

        public static void FindPrefabs()
        {
            Log.LogInfo("FindPrefabs()");
#if DEBUG
            var watch = new System.Diagnostics.Stopwatch();
            watch.Start();
#endif
            PieceNameCache = GetExistingPieceNames();
            EligiblePrefabs = ZNetScene.instance.m_prefabs
            .Where(go => go.transform.parent == null && !ShouldIgnorePrefab(go))
            .OrderBy(go => go.name)
            .ToList();
            Log.LogInfo($"Found {EligiblePrefabs.Count()} prefabs");
#if DEBUG
            watch.Stop();
            Log.LogInfo($"Search Time: {watch.ElapsedMilliseconds} ms");
#endif
        }

        public static void AddCustomPieces()
        {
#if DEBUG
            var watch = new System.Diagnostics.Stopwatch();
            watch.Start();
#endif
            Log.LogInfo("AddCustomPieces()");
               
            var pieceTable = GetPieceTable("_HammerPieceTable");
            if ( pieceTable == null )
            {
                Log.LogError("Could not find _HammerPieceTable");
            }

            foreach (var prefab in EligiblePrefabs)
            {
                // Don't know how this happens but it definitely does
                // sometimes when other mods are used in addition to this one.
                if (prefab == null)
                {
                    Log.LogWarning("Null prefab found in EligiblePrefabs");
                    continue;
                }
                CreatePrefabPiece(prefab, pieceTable);
            }
           
            Log.LogInfo($"Created {AddedPrefabs.Count} custom pieces");
#if DEBUG
            watch.Stop();
            Log.LogInfo($"Creation Time: {watch.ElapsedMilliseconds} ms");
#endif
            PluginConfig.Save();
        }

        public static void RemoveCustomPieces()
        {
            Log.LogInfo("RemoveCustomPieces()");
            int removedCounter = 0;
            PieceTable pieceTable = GetPieceTable("_HammerPieceTable");

            foreach (var name in AddedPrefabs)
            {
                try
                {
                    // Remove piece from PieceTable 
                    var prefab = ZNetScene.instance.GetPrefab(name);
                    pieceTable.m_pieces.Remove(prefab);
                    removedCounter++;

                }
                catch (Exception e)
                {
#if DEBUG
                    Log.LogInfo($"{name}: {e}");
#endif
                }
            }
            AddedPrefabs.Clear();
            EligiblePrefabs.Clear();
            PieceNameCache = null;
            Log.LogInfo($"Removed {removedCounter} custom pieces");
        }

        private static PieceTable GetPieceTable(string name)
        {
            var pieceTables = Resources.FindObjectsOfTypeAll<PieceTable>();
            if (pieceTables != null)
            {
                foreach (var pieceTable in pieceTables)
                {
                    if (pieceTable.name == name) return pieceTable;
                }
            }
            return null;
        }

        private static bool ShouldIgnorePrefab(GameObject prefab)
        {
            // Ignore existing pieces
            if (PieceNameCache == null)
            {
                throw new Exception("PieceNameCache is null");
            }
            else if (PieceNameCache.Contains(prefab.name))
            {
                return true;
            }

            // Ignore specific prefab names
            if (IgnoredPrefabs.Contains(prefab.name))
            {
                return true;
            }

            // Ignore pieces added by Azumat's BowsBeforeHoes mod
            if (prefab.name.StartsWith("BBH_"))
            {
                return true;
            }

            // Customs filters
            if (prefab.GetComponent("Projectile") != null ||
                prefab.GetComponent("Humanoid") != null ||
                prefab.GetComponent("AnimalAI") != null ||
                prefab.GetComponent("Character") != null ||
                prefab.GetComponent("CreatureSpawner") != null ||
                prefab.GetComponent("SpawnArea") != null ||
                prefab.GetComponent("Fish") != null ||
                prefab.GetComponent("RandomFlyingBird") != null ||
                prefab.GetComponent("MusicLocation") != null ||
                prefab.GetComponent("Aoe") != null ||
                prefab.GetComponent("ItemDrop") != null ||
                prefab.GetComponent("DungeonGenerator") != null ||
                prefab.GetComponent("TerrainModifier") != null ||
                prefab.GetComponent("EventZone") != null ||
                prefab.GetComponent("LocationProxy") != null ||
                prefab.GetComponent("LootSpawner") != null ||
                prefab.GetComponent("Mister") != null ||
                prefab.GetComponent("Ragdoll") != null ||
                prefab.GetComponent("MineRock5") != null ||
                prefab.GetComponent("TombStone") != null ||
                prefab.GetComponent("LiquidVolume") != null ||
                prefab.GetComponent("Gibber") != null ||
                prefab.GetComponent("TimedDestruction") != null ||
                prefab.GetComponent("ShipConstructor") != null ||
                prefab.GetComponent("TriggerSpawner") != null ||
                prefab.GetComponent("TeleportAbility") != null ||
                prefab.GetComponent("TeleportWorld") != null ||

                prefab.name.StartsWith("_") ||
                prefab.name.StartsWith("OLD_") ||
                prefab.name.EndsWith("_OLD") ||
                prefab.name.StartsWith("vfx_") ||
                prefab.name.StartsWith("sfx_") ||
                prefab.name.StartsWith("fx_")
            )
            {
                return true;
            }
            return false;
        }

        // Refs:
        //  - PieceTable
        //  - PieceTable.m_pieces
        private static HashSet<string> GetExistingPieceNames()
        {
            if (PieceNameCache == null)
            {
                var result = Resources.FindObjectsOfTypeAll<PieceTable>()
                  .SelectMany(pieceTable => pieceTable.m_pieces)
                  .Select(piece => piece.name);

                PieceNameCache = new HashSet<string>(result);
            }
            return PieceNameCache;
        }

        private static void CreatePrefabPiece(GameObject prefab, PieceTable pieceTable)
        {
            if (!EnsureNoDuplicateZNetView(prefab))
            {
                // Just dont, as it will fuck over vanilla (non-mod) users
                if (PluginConfig.IsVerbose())
                {
                    Log.LogInfo($"Prevent duplicate ZNetView for: {prefab.name}");
                }
                return;
            }

            // load config data and create piece config
            PrefabDefaults.PrefabConfig prefabConfig = PluginConfig.LoadPrefabConfig(prefab);

            if (!prefabConfig.Enabled && !PluginConfig.IsForceAllPrefabs())
            {
                // prefab denied by config
                return;
            }

            if (PluginConfig.IsVerbose())
            {
                Log.LogInfo("Initialize '" + prefab.name + "'");
                foreach (Component compo in prefab.GetComponents<Component>())
                {
                    Log.LogInfo("  - " + compo.GetType().Name);
                }
            }

            InitPieceData(prefab);
            PatchPrefabIfNeeded(prefab);

            var piece = prefab.GetComponent<Piece>();
            piece.SetName(PrefabNames.FormatPrefabName(prefab.name))
                .SetDescription(PrefabNames.GetPrefabDescription(prefab))
                .SetAllowedInDungeons(prefabConfig.AllowedInDungeons)
                .SetCategory(prefabConfig.Category)
                .SetCraftingStation(prefabConfig.CraftingStation)
                .SetResources(CreateRequirements(prefabConfig.Requirements))
                .SetIcon(PrefabIcons.CreatePrefabIcon(prefab));

            // Restrict CreatorShop pieces to Admins only
            if (
                HammerCategories.IsCreatorShopPiece(piece)
                && PluginConfig.AdminOnlyCreatorShop.Value
                && !SynchronizationManager.Instance.PlayerIsAdmin
            )
            {
                return;
            }


            if (!pieceTable.m_pieces.Contains(prefab))
            {
                pieceTable.m_pieces.Add(prefab);
                AddedPrefabs.Add(prefab.name);
            }
        }

        private static bool EnsureNoDuplicateZNetView(GameObject prefab)
        {
            var views = prefab?.GetComponents<ZNetView>();

            if (views == null) return true;

            for (int i = 1; i < views.Length; ++i)
            {
                GameObject.DestroyImmediate(views[i]);
            }

            return views.Length <= 1;
        }

        static Piece.Requirement[] CreateRequirements(string data)
        {
            if (string.IsNullOrEmpty(data)) return new Piece.Requirement[0];

            // If not empty
            List<Piece.Requirement> requirements = new();

            foreach(var entry in data.Split(';'))
            {
                string[] values = entry.Split(',');

                var itm = PrefabManager.Cache.GetPrefab<GameObject>(values[0].Trim())?.GetComponent<ItemDrop>();
                if (itm == null)
                {
                    Log.LogWarning($"Invalid build requirement ID: {values[0].Trim()}");
                    continue;
                }
                Piece.Requirement req = new()
                {
                    m_resItem = PrefabManager.Cache.GetPrefab<GameObject>(values[0].Trim()).GetComponent<ItemDrop>(),
                    m_amount = int.Parse(values[1].Trim()),
                    m_recover = true
                };
                requirements.Add(req);
            }
            return requirements.ToArray();
        }

        private static void InitPieceData(GameObject prefab)
        {
            Piece piece = prefab.GetComponent<Piece>();
            if (piece == null)
            {
                piece = prefab.AddComponent<Piece>();
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
                    piece.m_clipEverything = !PrefabDefaults.RestrictClipping.Contains(prefab.name);
                    piece.m_allowRotatedOverlap = true;
                    piece.m_repairPiece = false; // setting this to true breaks a lot of pieces
                    piece.m_canBeRemoved = true;
                    piece.m_onlyInBiome = Heightmap.Biome.None;
                    if (PluginConfig.IsVerbose())
                    {
                        Log.LogInfo($"Creating Piece for: {prefab.name}");
                    }
                }
            }
            else
            {
                if (piece.m_resources !=  null)
                {
                    // stop errors on second log in
                    if (!DefaultResources.ContainsKey(prefab.name))
                    {
#if DEBUG
                        Log.LogDebug($"Adding default drops for {prefab.name}");
#endif
                        DefaultResources.Add(prefab.name, piece.m_resources);
                    }
                }
            }
        }

        /*
         * Fix collider and snap points on the prefab if necessary
         */
        private static void PatchPrefabIfNeeded(GameObject prefab)
        {
            switch (prefab.name)
            {
                case "blackmarble_column_3":
                    SnapPointHelper.AddSnapPoints(
                        prefab,
                        new Vector3[] {
                            new Vector3(-1, 4, -1),
                            new Vector3(1, 4, 1),
                            new Vector3(-1, 4, 1),
                            new Vector3(1, 4, -1),

                            new Vector3(-1, 2, -1),
                            new Vector3(1, 2, 1),
                            new Vector3(-1, 2, 1),
                            new Vector3(1, 2, -1),

                            new Vector3(-1, 0, -1),
                            new Vector3(1, 0, 1),
                            new Vector3(-1, 0, 1),
                            new Vector3(1, 0, -1),

                            new Vector3(-1, -2, -1),
                            new Vector3(1, -2, 1),
                            new Vector3(-1, -2, 1),
                            new Vector3(1, -2, -1),

                            new Vector3(-1, -4, -1),
                            new Vector3(1, -4, 1),
                            new Vector3(-1, -4, 1),
                            new Vector3(1, -4, -1),
                        }
                    );
                    break;
                case "blackmarble_creep_4x1x1":
                    SnapPointHelper.AddSnapPoints(
                        prefab,
                        new Vector3[] {
                            new Vector3(-0.5f, 2, -0.5f),
                            new Vector3(0.5f, 2, 0.5f),
                            new Vector3(-0.5f, 2, 0.5f),
                            new Vector3(0.5f, 2, -0.5f),

                            new Vector3(-0.5f, -2, -0.5f),
                            new Vector3(0.5f, -2, 0.5f),
                            new Vector3(-0.5f, -2, 0.5f),
                            new Vector3(0.5f, -2, -0.5f),

                            new Vector3(0, -2, 0),
                            new Vector3(0, 2, 0),
                        }
                    );

                    // ? Place the piece randomly in horizontal or vertical position ?
                    UnityEngine.Object.DestroyImmediate(prefab.transform.Find("new").gameObject.GetComponent<RandomPieceRotation>());

                    break;
                case "blackmarble_creep_4x2x1":
                    SnapPointHelper.AddSnapPoints(
                        prefab,
                        new Vector3[] {
                            new Vector3(1, 2, -0.5f),
                            new Vector3(1, 2, 0.5f),
                            new Vector3(-1, 2, -0.5f),
                            new Vector3(-1, 2, 0.5f),

                            new Vector3(1, -2, -0.5f),
                            new Vector3(1, -2, 0.5f),
                            new Vector3(-1, -2, -0.5f),
                            new Vector3(-1, -2, 0.5f),

                            new Vector3(0, -2, 0),
                            new Vector3(0, 2, 0),
                        }
                    );

                    // ? Place the piece randomly in horizontal or vertical position ?
                    UnityEngine.Object.DestroyImmediate(prefab.transform.Find("new").gameObject.GetComponent<RandomPieceRotation>());
                    break;
                case "blackmarble_creep_slope_inverted_1x1x2":
                    SnapPointHelper.AddSnapPoints(
                        prefab, 
                        new Vector3[] {
                            new Vector3(-0.5f, 1, -0.5f),
                            new Vector3(0.5f, 1, 0.5f),
                            new Vector3(-0.5f, 1, 0.5f),
                            new Vector3(0.5f, 1, -0.5f),

                            new Vector3(0.5f, -1, -0.5f),
                            new Vector3(-0.5f, -1, -0.5f),
                        }
                    );
                    break;
                case "blackmarble_creep_slope_inverted_2x2x1":
                    SnapPointHelper.AddSnapPoints(prefab, new Vector3[] {
                        new Vector3(-1, 0.5f, -1),
                        new Vector3(1, 0.5f,1),
                        new Vector3(-1, 0.5f, 1),
                        new Vector3(1, 0.5f, -1),

                        new Vector3(-1, -0.5f, -1),
                        new Vector3(1, -0.5f, -1),
                    });
                    break;
                case "blackmarble_creep_stair":
                    SnapPointHelper.AddSnapPoints(prefab, new Vector3[] {
                        new Vector3(-1, 1, -1),
                        new Vector3(1, 1, -1),
                        new Vector3(-1, 0, -1),
                        new Vector3(1, 0, -1),
                        new Vector3(-1, 0, 1),
                        new Vector3(1, 0, 1),
                    });
                    break;
                case "blackmarble_floor_large":
                    List<Vector3> points = new();
                    for (int y = -1; y <= 1; y += 2)
                    {
                        for (int x = -4; x <= 4; x += 2)
                        {
                            for (int z = -4; z <= 4; z += 2)
                            {
                                points.Add(new Vector3(x, y, z));
                            }
                        }
                    }
                    SnapPointHelper.AddSnapPoints(prefab, points);
                    break;
                case "blackmarble_head_big01":
                    SnapPointHelper.AddSnapPoints(prefab, new Vector3[] {
                        new Vector3(-1, 1, -1),
                        new Vector3(1, 1,1),
                        new Vector3(-1, 1, 1),
                        new Vector3(1, 1, -1),

                        new Vector3(1, -1, -1),
                        new Vector3(-1, -1, -1),
                    });
                    break;
                case "blackmarble_head_big02":
                    SnapPointHelper.AddSnapPoints(
                        prefab, 
                        new Vector3[] {
                            new Vector3(-1, 1, -1),
                            new Vector3(1, 1,1),
                            new Vector3(-1, 1, 1),
                            new Vector3(1, 1, -1),

                            new Vector3(1, -1, -1),
                            new Vector3(-1, -1, -1),
                        }
                    );
                    break;
                case "blackmarble_out_2":
                    // Fix piece colliders via layers
                    UnityEngine.Object.DestroyImmediate(prefab.GetComponent<MeshCollider>());
                    SnapPointHelper.FixPiece(prefab);
                    break;
                case "blackmarble_tile_floor_1x1":
                    prefab.transform.Find("_snappoint").gameObject.transform.localPosition = new Vector3(0.5f, 0.1f, 0.5f);
                    prefab.transform.Find("_snappoint (1)").gameObject.transform.localPosition = new Vector3(0.5f, 0.1f, -0.5f);
                    prefab.transform.Find("_snappoint (2)").gameObject.transform.localPosition = new Vector3(-0.5f, 0.1f, 0.5f);
                    prefab.transform.Find("_snappoint (3)").gameObject.transform.localPosition = new Vector3(-0.5f, 0.1f, -0.5f);
                    break;
                case "blackmarble_tile_floor_2x2":
                    prefab.transform.Find("_snappoint").gameObject.transform.localPosition = new Vector3(1, 0.1f, 1);
                    prefab.transform.Find("_snappoint (1)").gameObject.transform.localPosition = new Vector3(1, 0.1f, -1);
                    prefab.transform.Find("_snappoint (2)").gameObject.transform.localPosition = new Vector3(-1, 0.1f, 1);
                    prefab.transform.Find("_snappoint (3)").gameObject.transform.localPosition = new Vector3(-1, 0.1f, -1);
                    break;
                case "blackmarble_tile_wall_1x1":
                    prefab.transform.Find("_snappoint").gameObject.transform.localPosition = new Vector3(0.5f, 0, 0.1f);
                    prefab.transform.Find("_snappoint (1)").gameObject.transform.localPosition = new Vector3(0.5f, 1, 0.1f);
                    prefab.transform.Find("_snappoint (2)").gameObject.transform.localPosition = new Vector3(-0.5f, 0, 0.1f);
                    prefab.transform.Find("_snappoint (3)").gameObject.transform.localPosition = new Vector3(-0.5f, 1, 0.1f);
                    prefab.transform.Find("_snappoint (4)").gameObject.transform.localPosition = new Vector3(0.25f, 0, 0.1f);
                    prefab.transform.Find("_snappoint (5)").gameObject.transform.localPosition = new Vector3(0.25f, 0.5f, 0.1f);
                    prefab.transform.Find("_snappoint (6)").gameObject.transform.localPosition = new Vector3(-0.25f, 0, 0.1f);
                    prefab.transform.Find("_snappoint (7)").gameObject.transform.localPosition = new Vector3(-0.25f, 0.5f, 0.1f);
                    break;
                case "blackmarble_tile_wall_2x2":
                    prefab.transform.Find("_snappoint").gameObject.transform.localPosition = new Vector3(1, 0, 0.1f);
                    prefab.transform.Find("_snappoint (1)").gameObject.transform.localPosition = new Vector3(1, 2, 0.1f);
                    prefab.transform.Find("_snappoint (2)").gameObject.transform.localPosition = new Vector3(-1, 0, 0.1f);
                    prefab.transform.Find("_snappoint (3)").gameObject.transform.localPosition = new Vector3(-1, 2, 0.1f);
                    prefab.transform.Find("_snappoint (4)").gameObject.transform.localPosition = new Vector3(0.5f, 0, 0.1f);
                    prefab.transform.Find("_snappoint (5)").gameObject.transform.localPosition = new Vector3(0.5f, 1, 0.1f);
                    prefab.transform.Find("_snappoint (6)").gameObject.transform.localPosition = new Vector3(-0.5f, 0, 0.1f);
                    prefab.transform.Find("_snappoint (7)").gameObject.transform.localPosition = new Vector3(-0.5f, 1, 0.1f);
                    break;
                case "blackmarble_tile_wall_2x4":
                    prefab.transform.Find("_snappoint").gameObject.transform.localPosition = new Vector3(1, 0, 0.1f);
                    prefab.transform.Find("_snappoint (1)").gameObject.transform.localPosition = new Vector3(1, 4, 0.1f);
                    prefab.transform.Find("_snappoint (2)").gameObject.transform.localPosition = new Vector3(-1, 0, 0.1f);
                    prefab.transform.Find("_snappoint (3)").gameObject.transform.localPosition = new Vector3(-1, 4, 0.1f);
                    prefab.transform.Find("_snappoint (4)").gameObject.transform.localPosition = new Vector3(0.5f, 0, 0.1f);
                    prefab.transform.Find("_snappoint (5)").gameObject.transform.localPosition = new Vector3(0.5f, 2, 0.1f);
                    prefab.transform.Find("_snappoint (6)").gameObject.transform.localPosition = new Vector3(-0.5f, 0, 0.1f);
                    prefab.transform.Find("_snappoint (7)").gameObject.transform.localPosition = new Vector3(-0.5f, 2, 0.1f);
                    break;
                case "dungeon_queen_door":
                    SnapPointHelper.AddSnapPoints(
                        prefab, 
                        new Vector3[] {
                            new Vector3(2.5f, 0, 0),
                            new Vector3(-2.5f, 0, 0),
                        }
                    );
                    break;
                case "dungeon_sunkencrypt_irongate":
                    SnapPointHelper.AddSnapPoints(
                        prefab, 
                        new Vector3[] {
                            new Vector3(1, -0.4f, 0),
                            new Vector3(-1, -0.4f, 0),
                        }
                    );
                    break;
                case "sunken_crypt_gate":
                    SnapPointHelper.AddSnapPoints(
                        prefab, 
                        new Vector3[] {
                            new Vector3(1, 0, 0),
                            new Vector3(-1, 0, 0),
                        }
                    );
                    break;
                case "dvergrprops_wood_beam":
                    /*
                    SnapPointHelper.AddSnapPoints(
                        prefab, 
                        new Vector3[] {
                            new Vector3(3, 0.45f, 0.45f),
                            new Vector3(3, 0.45f, -0.42f),
                            new Vector3(3, -0.45f, -0.42f),
                            new Vector3(3, -0.45f, -0.45f),

                            new Vector3(-3, 0.45f, 0.45f),
                            new Vector3(-3, 0.45f, -0.42f),
                            new Vector3(-3, -0.45f, -0.42f),
                            new Vector3(-3, -0.45f, -0.45f),
                        }
                    );
                    */
                    SnapPointHelper.AddSnapPoints(
                        prefab, 
                        new Vector3[] {
                            new Vector3(3, 0, 0),
                            new Vector3(-3, 0, 0),
                        }
                    );
                    break;
                case "dvergrprops_wood_pole":
                    /*
                    SnapPointHelper.AddSnapPoints(
                        prefab, 
                        new Vector3[] {
                            new Vector3(0.45f, 2, 0.45f),
                            new Vector3(-0.45f, 2, 0.45f),
                            new Vector3(0.45f, 2, -0.45f),
                            new Vector3(-0.45f, 2, -0.45f),
                            new Vector3(0.45f, -2, 0.45f),
                            new Vector3(-0.45f, -2, 0.45f),
                            new Vector3(0.45f, -2, -0.45f),
                            new Vector3(-0.45f, -2, -0.45f),
                        }
                    );
                    */
                    SnapPointHelper.AddSnapPoints(
                        prefab, 
                        new Vector3[] {
                            new Vector3(0, 2, 0),
                            new Vector3(0, -2, 0),
                        }
                    );
                    break;
                case "dvergrprops_wood_wall":
                    // Patch only the floor
                    /*generateSnapPoints(prefab, new Vector3[] {
                        new Vector3(2.2f, -2, -0.45f),
                        new Vector3(2.2f, -2, 0.45f),
                        new Vector3(-2.2f, -2, -0.45f),
                        new Vector3(-2.2f, -2, 0.45f),
                    });*/
                    SnapPointHelper.AddSnapPoints(
                        prefab, 
                        new Vector3[] {
                            new Vector3(2.2f, 2, 0),
                            new Vector3(-2.2f, 2, 0),
                            new Vector3(2.2f, -2, 0),
                            new Vector3(-2.2f, -2, 0),
                        }
                    );
                    break;
                case "dvergrtown_arch":
                    /*
                    SnapPointHelper.AddSnapPoints(
                        prefab, 
                        new Vector3[] {
                            new Vector3(1, 0, -0.5f),
                            new Vector3(1, 0, 0.5f),
                            new Vector3(1, 1, -0.5f),
                            new Vector3(1, 1, 0.5f),

                            new Vector3(-1, 1, -0.5f),
                            new Vector3(-1, 1, 0.5f),

                            new Vector3(-1, -1, 0.5f),
                            new Vector3(-1, -1, -0.5f),
                            new Vector3(0, -1, 0.5f),
                            new Vector3(0, -1, -0.5f),
                        }
                    );
                    */
                    SnapPointHelper.AddSnapPoints(
                        prefab, 
                        new Vector3[] {
                            new Vector3(1, 0.5f, 0),
                        }
                    );
                    break;

                case "dvergrtown_secretdoor":
                    /*
                     SnapPointHelper.AddSnapPoints(
                        prefab, 
                        new Vector3[] {
                            new Vector3(2, 0, -0.35f),
                            new Vector3(2, 0, 0.4f),
                            new Vector3(-2, 0, -0.35f),
                            new Vector3(-2, 0, 0.4f),
                        
                            new Vector3(2, 4, -0.35f),
                            new Vector3(2, 4, 0.4f),
                            new Vector3(-2, 4, -0.35f),
                            new Vector3(-2, 4, 0.4f),
                        }
                    );
                    */
                    SnapPointHelper.AddSnapPoints(
                        prefab, 
                        new Vector3[] {
                            new Vector3(2, 0, 0),
                            new Vector3(-2, 0, 0),
                            new Vector3(2, 4, 0),
                            new Vector3(-2, 4, 0),
                        }
                    );
                    break;
                case "dvergrtown_slidingdoor":
                    /*
                    SnapPointHelper.AddSnapPoints(
                        prefab, 
                        new Vector3[] {
                            new Vector3(2, 0, 0),
                            new Vector3(2, 0, 0.2f),
                            new Vector3(-2, 0, -0.2f),
                            new Vector3(-2, 0, 0.2f),

                            new Vector3(2, 4, -0.2f),
                            new Vector3(2, 4, 0.2f),
                            new Vector3(-2, 4, -0.2f),
                            new Vector3(-2, 4, 0.2f),
                        }
                    );
                    */
                    SnapPointHelper.AddSnapPoints(
                        prefab, 
                        new Vector3[] {
                            new Vector3(2, 0, 0),
                            new Vector3(-2, 0, 0),
                            new Vector3(2, 4, 0),
                            new Vector3(-2, 4, 0),
                        }
                    );
                    break;
                case "dvergrtown_stair_corner_wood_left":
                    SnapPointHelper.AddSnapPoints(
                        prefab, 
                        new Vector3[] {
                            new Vector3(0.25f, 0, -0.25f),
                            new Vector3(0.25f, 0, 0.25f),
                            new Vector3(0.25f, 1.1f, -0.25f),
                            new Vector3(0.25f, 1.1f, 0.25f),

                            new Vector3(0.25f, 0, 2),
                            new Vector3(-0.25f, 1.1f, -0.25f),

                            new Vector3(-2, 1.1f, -0.25f)
                        }
                    );
                    break;
                case "dvergrtown_wood_beam":
                    /*
                     SnapPointHelper.AddSnapPoints(
                        prefab, 
                        new Vector3[] {
                            new Vector3(3, 0.45f, 0.45f),
                            new Vector3(3, 0.45f, -0.42f),
                            new Vector3(3, -0.45f, -0.42f),
                            new Vector3(3, -0.45f, -0.45f),

                            new Vector3(-3, 0.45f, 0.45f),
                            new Vector3(-3, 0.45f, -0.42f),
                            new Vector3(-3, -0.45f, -0.42f),
                            new Vector3(-3, -0.45f, -0.45f),
                        }
                    );
                    */
                    SnapPointHelper.AddSnapPoints(
                        prefab, 
                        new Vector3[] {
                            new Vector3(3, 0, 0),
                            new Vector3(-3, 0, 0),
                        }
                    );
                    break;
                case "dvergrtown_wood_pole":
                    /*
                    SnapPointHelper.AddSnapPoints(
                        prefab, 
                        new Vector3[] {
                            new Vector3(0.45f, 2, 0.45f),
                            new Vector3(-0.45f, 2, 0.45f),
                            new Vector3(0.45f, 2, -0.45f),
                            new Vector3(-0.45f, 2, -0.45f),
                            new Vector3(0.45f, -2, 0.45f),
                            new Vector3(-0.45f, -2, 0.45f),
                            new Vector3(0.45f, -2, -0.45f),
                            new Vector3(-0.45f, -2, -0.45f),
                        }
                    );
                    */
                    SnapPointHelper.AddSnapPoints(
                        prefab, 
                        new Vector3[] {
                            new Vector3(0, -2, 0),
                            new Vector3(0, 2, 0),
                        }
                    );
                    break;
                case "dvergrtown_wood_stake":
                    // Patch only the floor
                    /*
                    SnapPointHelper.AddSnapPoints(
                        prefab, 
                        new Vector3[] {
                            new Vector3(0.15f, 0, 0.15f),
                            new Vector3(-0.15f, 0, 0.15f),
                            new Vector3(0.15f, 0, -0.15f),
                            new Vector3(-0.15f, 0, -0.15f),
                        }
                    );
                    */
                    SnapPointHelper.AddSnapPoints(
                        prefab, 
                        new Vector3[] {
                            new Vector3(0, 0, 0),
                        }
                    );
                    break;
                case "dvergrtown_wood_crane":
                    /*
                    SnapPointHelper.AddSnapPoints(
                        prefab, 
                        new Vector3[] {
                            new Vector3(0.4f, -3, 0.4f),
                            new Vector3(-0.4f, -3, 0.4f),
                            new Vector3(0.4f, -3, -0.4f),
                            new Vector3(-0.4f, -3, -0.4f),
                        }
                    );
                    */
                    SnapPointHelper.AddSnapPoints(
                        prefab, 
                        new Vector3[] {
                            new Vector3(0, -3, 0),
                        }
                    );
                    break;
                case "dvergrtown_wood_support":
                    /*
                    SnapPointHelper.AddSnapPoints(
                        prefab, 
                        new Vector3[] {
                            new Vector3(-2.4f, 0, -0.4f),
                            new Vector3(-2.4f, 0, 0.4f),
                            new Vector3(-1.6f, 0, -0.4f),
                            new Vector3(-1.6f, 0, 0.4f),

                            new Vector3(2.4f, 0, -0.4f),
                            new Vector3(2.4f, 0, 0.4f),
                            new Vector3(1.6f, 0, -0.4f),
                            new Vector3(1.6f, 0, 0.4f),
                        }
                    );
                    */
                    SnapPointHelper.AddSnapPoints(
                        prefab, 
                        new Vector3[] {
                            new Vector3(-2, 0, 0),
                            new Vector3(2, 0, 0),
                        }
                    );
                    break;
                case "dvergrtown_wood_wall01":
                    // Fix collider y
                    prefab.transform.Find("wallcollider").transform.localPosition = new Vector3(0, 0, 0);
                    /*
                    SnapPointHelper.AddSnapPoints(
                        prefab, 
                        new Vector3[] {
                        new Vector3(3, -2.7f, -0.45f),
                        new Vector3(3, -2.7f, 0.45f),
                        new Vector3(-3, -2.7f, -0.45f),
                        new Vector3(-3, -2.7f, 0.45f),

                        new Vector3(3, 2.7f, -0.45f),
                        new Vector3(3, 2.7f, 0.45f),
                        new Vector3(-3, 2.7f, -0.45f),
                        new Vector3(-3, 2.7f, 0.45f),
                        }
                    );
                    */
                    SnapPointHelper.AddSnapPoints(
                        prefab, 
                        new Vector3[] {
                            new Vector3(-3, -2.7f, 0),
                            new Vector3(3, -2.7f, 0),
                            new Vector3(-3, 2.7f, 0),
                            new Vector3(3, 2.7f, 0),
                        }
                    );
                    break;
                case "dvergrtown_wood_wall02":
                    /*
                 
                    SnapPointHelper.AddSnapPoints(
                        prefab, 
                        new Vector3[] {
                            new Vector3(3, -0.6f, -0.25f),
                            new Vector3(3, -0.6f, 0.25f),
                            new Vector3(-3, -0.6f, -0.25f),
                            new Vector3(-3, -0.6f, 0.25f),

                            new Vector3(3, 4.6f, -0.25f),
                            new Vector3(3, 4.6f, 0.25f),
                            new Vector3(-3, 4.6f, -0.25f),
                            new Vector3(-3, 4.6f, 0.25f),
                        }
                    );
                    */

                    SnapPointHelper.AddSnapPoints(
                        prefab, 
                        new Vector3[] {
                            new Vector3(-3, -0.5f, 0),
                            new Vector3(3, -0.5f, 0),
                            new Vector3(-3, 4.5f, 0),
                            new Vector3(3, 4.5f, 0),
                        }
                    );
                    break;
                case "dvergrtown_wood_wall03":
                    SnapPointHelper.AddSnapPoints(
                        prefab, 
                        new Vector3[] {
                            new Vector3(1.1f, 0, 0),
                            new Vector3(-1.1f, 0, 0),

                            new Vector3(2, 2, 0),
                            new Vector3(-2, 2, 0),

                            new Vector3(1.1f, 4, 0),
                            new Vector3(-1.1f, 4, 0),
                        }
                    );
                    break;
                case "goblin_roof_45d":
                    SnapPointHelper.AddSnapPoints(
                        prefab, 
                        new Vector3[] {
                            new Vector3(1, 0, 1),
                            new Vector3(-1, 0, 1),
                            new Vector3(1, 2, -1),
                            new Vector3(-1, 2, -1),
                        }
                    );
                    break;
                case "goblin_roof_45d_corner":
                    SnapPointHelper.AddSnapPoints(
                        prefab, 
                        new Vector3[] {
                            new Vector3(-1, 0, -1),
                            new Vector3(1, 0, 1),
                        }
                    );
                    break;
                case "goblin_woodwall_1m":
                    SnapPointHelper.AddSnapPoints(
                        prefab, 
                        new Vector3[] {
                            new Vector3(-0.5f, 0, 0),
                            new Vector3(0.5f, 0, 0),
                            new Vector3(-0.5f, 2, 0),
                            new Vector3(0.5f, 2, 0),
                        }
                    );
                    break;
                case "goblin_woodwall_2m":
                    SnapPointHelper.AddSnapPoints(
                        prefab, 
                        new Vector3[] {
                            new Vector3(-1, 0, 0),
                            new Vector3(1, 0, 0),
                            new Vector3(-1, 2, 0),
                            new Vector3(1, 2, 0),
                        }
                    );
                    break;
                case "Ice_floor":
                    SnapPointHelper.AddSnapPoints(
                        prefab, 
                        new Vector3[] {
                            new Vector3(2, 1, 2),
                            new Vector3(-2, 1, -2),
                            new Vector3(2, 1, -2),
                            new Vector3(-2, 1, 2),

                            new Vector3(2, -1, 2),
                            new Vector3(-2, -1, -2),
                            new Vector3(2, -1, -2),
                            new Vector3(-2, -1, 2),
                        }
                    );
                    break;
                case "turf_roof": // 26 degree
                    SnapPointHelper.AddSnapPoints(
                        prefab,
                        new Vector3[]
                        {
                            new Vector3(0.0f, 0.5f, 0.0f), // center
                            new Vector3(0.0f, 0.0f, 1.0f), // Bottom mid edge
                            new Vector3(0.0f, 1.0f, -1.0f), // top mid edge
                            new Vector3(1.0f, 0.5f, 0.0f), // left mid edge
                            new Vector3(-1.0f, 0.5f, 0.0f), // right mid edge
                        }
                    );
                    break;
                case "turf_roof_top":
                    SnapPointHelper.AddSnapPoints(
                        prefab,
                        new Vector3[]
                        {
                            new Vector3(0.0f, 0.0f, 1.0f),  // side mid edge
                            new Vector3(0.0f, 0.0f, -1.0f),  // side mid edge
                            new Vector3(0.0f, 0.5f, 0.0f),  // top mid edge
                        }
                    );
                    break;
                case "ArmorStand_Female":
                case "ArmorStand_Male":
                    SnapPointHelper.AddCenterSnapPoint(prefab);
                    break;
                case "metalbar_1x2":
                    SnapPointHelper.AddSnapPoints(
                        prefab,
                        new Vector3[]
                        {
                            new Vector3(0.0f, 0.0f, 0.0f),
                            new Vector3(1.0f, -1.0f, 0.0f),
                            new Vector3(1.0f, 0.0f, 0.0f),
                            new Vector3(-1.0f, -1.0f, 0.0f),
                            new Vector3(-1.0f, 0.0f, 0.0f),
                        }
                    );
                    break;
                case "stone_floor":
                    List<Vector3> pts = new();
                    for (float y = -0.5f; y <= 0.5f; y += 1)
                    {
                        for (int x = -2; x <= 2; x += 1)
                        {
                            for (int z = -2; z <= 2; z += 1)
                            {
                                if (!(Math.Abs(x) == 2 && Mathf.Abs(z) == 2))  // skip corners that already have snap points
                                {
                                    pts.Add(new Vector3(x, y, z));
                                }
                            }
                        }
                    }
                    SnapPointHelper.AddSnapPoints(prefab, pts);
                    break;
                case "blackmarble_post01":
                    SnapPointHelper.AddSnapPoints(
                        prefab,
                        new Vector3[]
                        {
                            new Vector3(0.0f, 0.0f, 0.0f),
                            new Vector3(1.0f, -1.0f, 1.0f),
                            new Vector3(1.0f, -1.0f, -1.0f),
                            new Vector3(-1.0f, -1.0f, -1.0f),
                            new Vector3(-1.0f, -1.0f, 1.0f),
                            new Vector3(1.0f, 1.0f, 1.0f),
                            new Vector3(1.0f, 1.0f, -1.0f),
                            new Vector3(-1.0f, 1.0f, -1.0f),
                            new Vector3(-1.0f, 1.0f, 1.0f),
                            new Vector3(0.0f, 3.5f, 0.0f), // for torches
                        },
                        true // fix collision
                    );
                    break;
                case "wood_ledge":
                    SnapPointHelper.AddSnapPoints(
                        prefab,
                        new Vector3[]
                        {
                            new Vector3(0.5f, 0.0f, 0.25f),
                            new Vector3(0.0f, 0.0f, 0.25f),
                            new Vector3(-0.5f, 0.0f, 0.25f),
                            new Vector3(-0.5f, 0.0f, -0.25f),
                            new Vector3(0.5f, 0.0f, -0.25f),
                        }
                    );
                    break;
                case "dverger_demister":
                    CollisionHelper.RemoveColliders(prefab); //remove large box collider

                    // add thin collider along post
                    CollisionHelper.AddBoxCollider(prefab, new Vector3(0.0f, -0.2f, 0.0f), new Vector3(0.2f, 1.1f, 0.2f));

                    // add collider around the ring
                    CollisionHelper.AddBoxCollider(prefab, Vector3.zero, new Vector3(1.0f, 0.1f, 1.0f));

                    SnapPointHelper.AddSnapPoints(
                        prefab,
                        new Vector3[]
                        {
                            new Vector3(0.0f, 0.0f, 0.0f),
                            new Vector3(0.0f, -0.65f, 0.0f),
                        }
                    );
                    break;
                case "dverger_demister_large":
                    CollisionHelper.RemoveColliders(prefab); //remove large box collider

                    // add thin collider along post
                    CollisionHelper.AddBoxCollider(prefab, new Vector3(0.0f, -0.25f, 0.0f), new Vector3(0.2f, 1.3f, 0.2f));

                    // add collider around the ring
                    CollisionHelper.AddBoxCollider(prefab, new Vector3(0.0f, -0.25f, 0.0f), new Vector3(1.0f, 0.1f, 1.0f));

                    SnapPointHelper.AddSnapPoints(
                        prefab,
                        new Vector3[]
                        {
                            new Vector3(0.0f, 0.0f, 0.0f),
                            new Vector3(0.0f, -0.9f, 0.0f),
                        }
                    );
                    break;
                case "dvergrprops_hooknchain":
                    SnapPointHelper.AddSnapPoints(
                        prefab,
                        new Vector3[]
                        {
                            new Vector3(0.0f, 2.5f, 0.0f)
                        }
                    );
                    break;
                case "barrell":
                    SnapPointHelper.AddSnapPoints(
                        prefab,
                        new Vector3[]
                        {
                            new Vector3(0.0f, -1.0f, 0.0f)
                        }
                    );
                    break;
                default:
                    // Add SnapPoint to Local Center if there is not already one present there
                    Transform transform = prefab.GetComponent<Piece>().transform;
                    for (var index = 0; index < transform.childCount; ++index)
                    {
                        var child = transform.GetChild(index);
                        if (child.CompareTag("snappoint"))
                        {
                            if (child.localPosition.Equals(Vector3.zero))
                            {
                                return;
                            }
                        }
                    }
                    SnapPointHelper.AddCenterSnapPoint(prefab);
                    break;
            }
        }
    }
}
