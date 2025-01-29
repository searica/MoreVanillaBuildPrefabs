using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using HarmonyLib;
using Logging;
using Jotunn.Managers;
using System.Linq;
using System;
using MVBP.Extensions;
using MVBP.PieceManagement;


namespace MVBP.PrefabManagement;


/// <summary>
///     Manages detecting the eligible ZNetScene prefabs and patching them.
/// </summary>
[HarmonyPatch]
internal static class ZNetPrefabManager
{
    internal static readonly Dictionary<string, Piece.Requirement[]> VanillaPieceResources = [];

    private static readonly HashSet<string> AddedPieceComponent = [];

    internal static readonly Dictionary<string, GameObject> SeasonalPiecePrefabMap = new()
    {
        {"piece_maypole", null },
        {"piece_jackoturnip", null },
        {"piece_gift1", null },
        {"piece_gift2", null },
        {"piece_gift3", null },
        {"piece_mistletoe",null },
        {"piece_xmascrown",null },
        {"piece_xmasgarland",null },
        {"piece_xmastree",null },
    };

    internal static readonly Dictionary<string, GameObject> EligiblePrefabMap = [];

    private static readonly HashSet<string> IgnorePlantEverything =
    [
        "Pickable_Branch",
        "Pickable_Flint",
        "Pickable_Stone"
    ];

    private static readonly HashSet<string> IgnoredPrefabs = [
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
        "odin",
        "dvergrprops_wood_stake",
        "Flies",
        "Pickable_DvergerThing",
        "demister_ball", // Placement is glitchy
        "CargoCrate", // Deletes itself on placement because it's empty
        "SunkenKit_int_towerwall_LOD", // is not an actual prefab for building
        "fuling_turret", // Duplicate of vanilla ballista
        "dragoneggcup", // It's invisible and I don't want to patch it
        "FishingRodFloat", // placement is broken
        "Pickable_RandomFood", // not sure what this is meant to be
        "horizontal_web", // has no mesh?
        "tolroko_flyer", // It's a little space ship! Instantiating it throws errors though.
        "turf_roof_wall", // Duplicate of "wood_roof_wall"
        "Pickable_Item", // buggy and weird
        "Pickable_Barley_Wild",
        "Pickable_Flax_Wild",
        "Pickable_DolmenTreasure", // random items

        // Causes errors when destroying (even if spawned in Vanilla game)
        "fenrirhide_hanging_door",

        // Duplicates of wood chest
        "TreasureChest_blackforest",
        "TreasureChest_heath",
        "TreasureChest_heath_hildir",
        "TreasureChest_heath_hildir",
        "TreasureChest_meadows",
        "TreasureChest_meadows_buried",
        "TreasureChest_mountains",
        "TreasureChest_swamp",
        "shipwreck_karve_chest",
        "loot_chest_wood",

        // Duplicate of fcrypt
        "TreasureChest_forestcrypt",
        "stonechest", // Inventory name is weird

        // Duplicate of loot_chest_stone
        "TreasureChest_plains_stone",

        // Duplicate of "TreasureChest_dvergrtown"
        "TreasureChest_mountaincave_hildir",
        "TreasureChest_plainsfortress_hildir",
        "TreasureChest_forestcrypt_hildir",

        // Duplicate of mountainkit_brazier
        "CastleKit_brazier",

        // Not useful or different
        "CastleKit_groundtorch_unlit",
        "CastleKit_metal_groundtorch_unlit",
        "dvergrprops_lantern",

        // Duplicate of Dvergr Crate
        "dvergrprops_crate_ashlands",
        "Charredfortress_LOD"
    ];

    /// <summary>
    ///     Hook to initialize the mod. This is after both PlantEverything
    ///     and PotteryBarn add pieces but before PlanBuild scans for them.
    /// </summary>
    /// <param name="__instance"></param>
    [HarmonyPrefix]
    [HarmonyPriority(Priority.High)]
    [HarmonyPatch(typeof(ZoneSystem), nameof(ZoneSystem.Start))]
    private static void InitializeZNetPrefabManager()
    {
        // If loading into game world and prefabs have not been added
        if (SceneManager.GetActiveScene().name != "main")
        {
            return;
        }

        Log.LogInfo("Performing mod initialization");

        System.Diagnostics.Stopwatch watch = new System.Diagnostics.Stopwatch();
        if (Log.IsVerbosityMedium) { watch.Start(); }

        Initialize();

        if (Log.IsVerbosityMedium)
        {
            watch.Stop();
            Log.LogInfo($"Time to initialize: {watch.ElapsedMilliseconds} ms");
        }
    }

    /// <summary>
    ///     Try to get default Piece Resources for the vanilla version of the gameobject.
    /// </summary>
    /// <param name="gameObject"></param>
    /// <param name="defaultResources"></param>
    /// <returns></returns>
    internal static bool TryGetVanillaPieceResources(GameObject gameObject, out Piece.Requirement[] defaultResources)
    {
        if (VanillaPieceResources.TryGetValue(gameObject.GetPrefabName(), out defaultResources)) 
        {
            return true;
        }

        defaultResources = null;
        return false;
    }

    public static bool IsPatchedByMVBP(GameObject gameObject)
    {
        return EligiblePrefabMap.ContainsKey(gameObject.GetPrefabName());
    }

    public static bool IsPatchedByMVBP(string name)
    {
        return EligiblePrefabMap.ContainsKey(name);
    }

    public static bool TryGetEligiblePrefab(string name, out GameObject prefab)
    {
        if (EligiblePrefabMap.TryGetValue(name, out prefab))
        {
            return true;  
        }
        return false;
    }

    private static void Initialize()
    {
        if (EligiblePrefabMap.Count > 0)
        {
            return;
        }
        Log.LogInfo("Initializing prefabs");
        InitSeasonalPiecePrefabs();
        InitEligiblePrefabs();

        Log.LogInfo("Initializing default pieces");
        // Get a default icon to use if defaultResources doesn't have an icon.
        // Need this to prevent NRE's if other code references the defaultResources
        // before the coroutine that is rendering the icons finishes. (Such as PlanBuild)
        Sprite defaultIcon = PrefabManager.Cache.GetPrefab<Sprite>("mapicon_hildir1");

        foreach (GameObject prefab in EligiblePrefabMap.Values)
        {
            Piece piece = InitPieceComponent(prefab, defaultIcon);
            PrefabConfigManager.BindPrefabConfig(prefab, piece);
        }

        Log.LogInfo("Initializing default icons", Log.InfoLevel.Medium);
        IconManager.Instance.GeneratePrefabIcons(EligiblePrefabMap.Values);
    }

    /// <summary>
    ///     Get HashSet of all prefab names for existing pieces in all PieceTables.
    /// </summary>
    /// <returns></returns>
    private static HashSet<string> GetExistingBuildablePrefabs()
    {
        return Resources.FindObjectsOfTypeAll<PieceTable>()
            .SelectMany(pieceTable => pieceTable.m_pieces)
            .Select(piece => piece.name)
            .ToHashSet();
    }
    
    /// <summary>
    ///     Get refs to seasonal pieces that are disabled.
    /// </summary>
    private static void InitSeasonalPiecePrefabs()
    {
        List<string> pieceNames = SeasonalPiecePrefabMap.Keys.ToList();
        List<string> nullKeys = [];

        foreach (string name in pieceNames)
        {
            GameObject prefab = PrefabManager.Instance.GetPrefab(name);
            if (prefab && prefab.TryGetComponent(out Piece piece))
            {
                // Only add pieces that are currently disabled
                if (!piece.m_enabled)
                {
                    SeasonalPiecePrefabMap[name] = prefab;
                }
                else
                {
                    Log.LogInfo($"Seasonal Piece: {name} already enabled", Log.InfoLevel.Medium);
                    nullKeys.Add(name);
                }
            }
            else
            {
                Log.LogWarning($"Seasonal piece: {name} could not be found");
            }
        }

        foreach (string key in nullKeys)
        {
            SeasonalPiecePrefabMap.Remove(key);
        }
    }

    /// <summary>
    ///     Get map prefabs that are eligible for MVBP.
    /// </summary>
    private static void InitEligiblePrefabs()
    {
        // Find eligible prefabs for adding
        HashSet<string> ExistingBuildablePrefabs = GetExistingBuildablePrefabs();
        foreach (GameObject prefab in ZNetScene.instance.m_prefabs)
        {
            if (!prefab.transform.parent)
            {
                continue; // not root prefab
            }

            if (!ExistingBuildablePrefabs.Contains(prefab.name))
            {
                continue; // already buildable
            }

            if (!GetEligiblePrefab(prefab, out GameObject result) || EligiblePrefabMap.ContainsKey(result.name))
            {
                continue;  // not eligible or already added
            }

            if (!EnsureNoDuplicateZNetView(prefab))
            {
                continue;
            }

            try
            {
                // Always patching means it only runs once and
                // prevents trailership being unusable if disabled.
                PrefabPatcher.PatchPrefabIfNeeded(prefab);
            }
            catch (Exception ex)
            {
                Log.LogWarning($"Failed to patch prefab {prefab.name}: {ex}");
            }

            EligiblePrefabMap.Add(prefab.name, prefab);
            UpdateVanillaResources(prefab);
           
        }
        Log.LogInfo($"Found {EligiblePrefabMap.Count} prefabs");
    }

    /// <summary>
    ///     Prevents creation of duplicate ZNetViews
    /// </summary>
    /// <param name="prefab"></param>
    /// <returns></returns>
    private static bool EnsureNoDuplicateZNetView(GameObject prefab)
    {
        if (!prefab)
        {
            return false;
        }

        ZNetView[] views = prefab.GetComponents<ZNetView>();

        if (views == null)
        {
            return true;
        }

        for (int i = 1; i < views.Length; ++i)
        {
            GameObject.DestroyImmediate(views[i]);
        }

        return views.Length <= 1;
    }


    /// <summary>
    ///     Track the default Vanilla Piece resources for this prefab.
    /// </summary>
    /// <param name="prefab"></param>
    private static void UpdateVanillaResources(GameObject prefab)
    {
        if (prefab.TryGetComponent(out Piece piece) && piece.m_resources != null)
        {
            VanillaPieceResources.Add(prefab.name, piece.m_resources);
        }
        else
        {
            VanillaPieceResources.Add(prefab.name, Array.Empty<Piece.Requirement>());
        }
    }

    /// <summary>
    ///     Create and initialize piece component if needed.
    ///     Sets m_canBeRemoved to false by default when adding
    ///     piece components prefabs that are missing them.
    /// </summary>
    /// <param name="prefab"></param>
    internal static Piece InitPieceComponent(GameObject prefab, Sprite defaultIcon)
    {
        if (!prefab.TryGetComponent(out Piece piece))
        {
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
            Log.LogInfo($"Created Piece component for: {prefab.name}", Log.InfoLevel.Medium);
        }

        if (piece.m_icon == null)
        {
            piece.m_icon = defaultIcon;
        }
        return piece;
    }

    /// <summary>
    ///     Checks if a prefab is eligible for adding. If the prefab
    ///     spawns a MineRock5 component when destroyed then result
    ///     will point to that instead.
    /// </summary>
    /// <param name="prefab"></param>
    /// <param name="result"></param>
    /// <returns></returns>
    private static bool GetEligiblePrefab(GameObject prefab, out GameObject result)
    {
        if (ShouldIgnorePrefab(prefab))
        {
            result = null;
            return false;
        }

        if (ModCompat.IsPlantEverythingInstalled() && IgnorePlantEverything.Contains(prefab.name))
        {
            result = null;
            return false;
        }

        // Is it a destructible thing that spawns something else?
        if (prefab.TryGetComponent(out Destructible destructible))
        {
            // Is it set up to be patched to spawn something other than the vanilla spawn prefab.
            // If yes then skip checking for MineRock5 prefab.
            if (PrefabConfigManager.TryGetPrefabConfig(prefab.name, out PrefabConfig prefabConfig, checkIfBound: false) &&
                !string.IsNullOrEmpty(prefabConfig.SpawnOnDestroyed))
            {
                result = prefab;
                return true;
            }

            // If it spawns a MineRock5 when damaged then just return the MineRock5 variant
            if (destructible.m_spawnWhenDestroyed &&
                destructible.m_spawnWhenDestroyed.transform.parent == null &&
                destructible.m_spawnWhenDestroyed.GetComponent<MineRock5>())
            {
                result = destructible.m_spawnWhenDestroyed;
                return true;
            }
        }

        // Return the original prefab
        result = prefab;
        return true;
    }

    /// <summary>
    ///     Checks prefab to see if it is eligible for making a custom piece.
    /// </summary>
    /// <param name="prefab"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    private static bool ShouldIgnorePrefab(GameObject prefab)
    {
        // Ignore specific prefab names
        if (IgnoredPrefabs.Contains(prefab.name))
        {
            return true;
        }

        // Customs filters
        if (prefab.GetComponent<Projectile>() ||
            prefab.GetComponent<Humanoid>() ||
            prefab.GetComponent<AnimalAI>() ||
            prefab.GetComponent<Character>() ||
            prefab.GetComponent<CreatureSpawner>() ||
            prefab.GetComponent<SpawnArea>() ||
            prefab.GetComponent<Fish>() ||
            prefab.GetComponent<RandomFlyingBird>() ||
            prefab.GetComponent<MusicLocation>() ||
            prefab.GetComponent<Aoe>() ||
            prefab.GetComponent<ItemDrop>() ||
            prefab.GetComponent<DungeonGenerator>() ||
            prefab.GetComponent<TerrainModifier>() ||
            prefab.GetComponent<EventZone>() ||
            prefab.GetComponent<LocationProxy>() ||
            prefab.GetComponent<LootSpawner>() ||
            prefab.GetComponent<Mister>() ||
            prefab.GetComponent<Ragdoll>() ||
            prefab.GetComponent<MineRock5>() ||
            prefab.GetComponent<TombStone>() ||
            prefab.GetComponent<LiquidVolume>() ||
            prefab.GetComponent<Gibber>() ||
            prefab.GetComponent<ShipConstructor>() ||
            prefab.GetComponent<TriggerSpawner>() ||
            prefab.GetComponent<TeleportAbility>() ||
            prefab.GetComponent<Trader>() ||

            prefab.name.StartsWith("_") ||
            prefab.name.StartsWith("OLD_") ||
            prefab.name.EndsWith("OLD") ||
            prefab.name.EndsWith("_old") ||
            prefab.name.StartsWith("vfx_") ||
            prefab.name.StartsWith("sfx_") ||
            prefab.name.StartsWith("fx_") ||
            prefab.name.Contains("Random") ||
            prefab.name.Contains("random") ||
            prefab.name.EndsWith("_test")
        )
        {
            return true;
        }

        // Ignore pieces added by other mods
        if (prefab.name.StartsWith("BBH_") || // Azumat's BowsBeforeHoes mod
            prefab.name.StartsWith("rrr_") || // RRR prefabs
            prefab.name.StartsWith("CLLC_")) // Creature level and loot control
        {
            return true;
        }
        return false;
    }
}
