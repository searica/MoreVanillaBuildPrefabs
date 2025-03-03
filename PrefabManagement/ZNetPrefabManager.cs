using System.Collections.Generic;
using UnityEngine;

using Logging;
using Jotunn.Managers;
using System.Linq;
using System;
using MVBP.Extensions;
using MVBP.PieceManagement;
using Jotunn.Configs;


namespace MVBP.PrefabManagement;


/// <summary>
///     Manages detecting the eligible ZNetScene prefabs and patching them.
/// </summary>
internal static class ZNetPrefabManager
{
    internal static readonly Dictionary<string, Piece.Requirement[]> VanillaPieceResources = [];
    private static readonly Dictionary<string, bool> DefaultRemoveSettings = [];

    private static readonly HashSet<string> AddedPieceComponent = [];

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
        "Charredfortress_LOD",
    ];

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

    /// <summary>
    ///     Try to get default Piece Resources for the vanilla version of the gameobject.
    /// </summary>
    /// <param name="gameObject"></param>
    /// <param name="defaultResources"></param>
    /// <returns></returns>
    internal static bool TryGetDefaultRemoveSettings(GameObject gameObject, out bool canRemove)
    {
        if (DefaultRemoveSettings.TryGetValue(gameObject.GetPrefabName(), out canRemove))
        {
            return true;
        }

        canRemove = false;
        return false;
    }

    public static bool IsPatchedByMVBP(Component component)
    {
        return EligiblePrefabMap.ContainsKey(component.gameObject.GetPrefabName());
    }

    public static bool IsPatchedByMVBP(GameObject gameObject)
    {
        return EligiblePrefabMap.ContainsKey(gameObject.GetPrefabName());
    }

    public static bool IsPatchedByMVBP(string name)
    {
        return EligiblePrefabMap.ContainsKey(name);
    }

    public static bool HasPieceAddedByMVBP(GameObject gameObject)
    {
        return AddedPieceComponent.Contains(gameObject.GetPrefabName());
    }


    public static bool HasPieceAddedByMVBP(string name)
    {
        return AddedPieceComponent.Contains(name);
    }

    public static bool HasPieceAddedByMVBP(Component component)
    {
        return AddedPieceComponent.Contains(component.gameObject.GetPrefabName());
    }

    public static bool TryGetEligiblePrefab(string name, out GameObject prefab)
    {
        if (EligiblePrefabMap.TryGetValue(name, out prefab))
        {
            return true;
        }
        return false;
    }

    public static void Initialize()
    {
        if (EligiblePrefabMap.Count > 0)
        {
            return;
        }

        
        InitEligiblePrefabs();

        Log.LogInfo("Initializing default pieces");
        // Get a default icon to use if defaultResources doesn't have an icon.
        // Need this to prevent NRE's if other code references the defaultResources
        // before the coroutine that is rendering the icons finishes. (Such as PlanBuild)
        Sprite defaultIcon = PrefabManager.Cache.GetPrefab<Sprite>("mapicon_hildir1");
           
        // Loop over eligible prefabs in sorted order by name 
        // Makes sure configs are bound in the right order.
        foreach (KeyValuePair<string, GameObject> kvp in EligiblePrefabMap.OrderBy(x => x.Key))
        {
            Piece piece = InitPieceComponent(kvp.Value, defaultIcon);
            PrefabConfigManager.BindPrefabConfig(kvp.Value, piece);
            UpdateDefaultRemoveSettings(piece);
        }

        Log.LogInfo("Initializing default icons", Log.InfoLevel.Medium);
        IconManager.Instance.GeneratePrefabIcons(EligiblePrefabMap.Values);
    }

    /// <summary>
    ///     Get map prefabs that are eligible for MVBP.
    /// </summary>
    private static void InitEligiblePrefabs()
    {
        Log.LogInfo("Initializing prefabs");

        // Find eligible prefabs for adding
        HashSet<string> ExistingBuildablePrefabs = GetExistingBuildablePrefabs();
        foreach (GameObject prefab in ZNetScene.instance.m_prefabs)
        {
            if (!prefab.IsRootPrefab())
            {
                continue; // not root prefab
            }

            if (!GetEligiblePrefab(prefab, out GameObject eligiblePrefab) || 
                EligiblePrefabMap.ContainsKey(eligiblePrefab.name) ||  // already added
                ExistingBuildablePrefabs.Contains(eligiblePrefab.name)  // already buildable
            )
            {
                continue;  // not eligible or already added
            }

            if (!EnsureNoDuplicateZNetView(eligiblePrefab))
            {
                continue;
            }

            EligiblePrefabMap.Add(eligiblePrefab.name, eligiblePrefab);
            UpdateVanillaResources(eligiblePrefab);

            try
            {
                // Always patching means it only runs once and
                // prevents trailership being unusable if disabled.
                PrefabPatcher.PatchPrefabIfNeeded(eligiblePrefab);
            }
            catch (Exception ex)
            {
                Log.LogWarning($"Failed to patch prefab {eligiblePrefab.name}: {ex}");
            }
        }


        Log.LogInfo($"Found {EligiblePrefabMap.Count} prefabs");
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
            VanillaPieceResources.Add(prefab.name, []);
        }
    }

    /// <summary>
    ///     Track the default Piece.m_removePiece settings for this prefab.
    /// </summary>
    /// <param name="prefab"></param>
    private static void UpdateDefaultRemoveSettings(Piece piece)
    {
        if (!DefaultRemoveSettings.ContainsKey(piece.name))
        {
            DefaultRemoveSettings.Add(piece.name, piece.m_removePiece);
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

            AddedPieceComponent.Add(prefab.GetPrefabName());
            Log.LogInfo($"Created Piece component for: {prefab.name}", Log.InfoLevel.High);
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

        // Is it set up to be patched to spawn something other than the vanilla spawn prefab.
        // If yes then skip checking for MineRock5 prefab.
        if (PrefabConfigManager.TryGetPrefabConfig(prefab.name, out PrefabConfig prefabConfig, checkIfBound: false) &&
            !string.IsNullOrWhiteSpace(prefabConfig.SpawnOnDestroyed))
        {
            result = prefab;
            return true;
        }

     
        // Is it a destructible thing that spawns something else?
        if (prefab.TryGetComponent(out Destructible destructible))
        {
            // If it spawns a something and that thing is a root prefab
            if (destructible.m_spawnWhenDestroyed && destructible.m_spawnWhenDestroyed.IsRootPrefab())
            {
                // if it spawns a MineRock5 only return the spawned prefab if it is valid,
                // otherwise disqualify this prefab.
                if (destructible.m_spawnWhenDestroyed.TryGetComponent(out MineRock5 mineRock5Spawn))
                {
                    if (!IsValidMineRock5(mineRock5Spawn))
                    {
                        result = null;
                        return false;
                    }
                    result = destructible.m_spawnWhenDestroyed;
                    return true;
                }
            }
        }

        // Disqualify prefab if it has a MineRock5 and it's colliders are not set up correctly 
        if (prefab.TryGetComponent(out MineRock5 mineRock5) && !IsValidMineRock5(mineRock5))
        {
            result = null;
            return false;
        }

        // Return the original prefab
        result = prefab;
        return true;
    }


    /// <summary>
    ///     Checks if the MineRock5 has colliders set up.
    /// </summary>
    /// <param name="mineRock5"></param>
    /// <returns></returns>
    private static bool IsValidMineRock5(MineRock5 mineRock5)
    {
        return mineRock5 && mineRock5.GetComponentsInChildren<Collider>().Length >= 1;
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
            prefab.GetComponent<Aoe>() ||  // these are AOE effects
            prefab.GetComponent<CamShaker>() ||  // These are magic AOE destruction effects
            

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

    /// <summary>
    ///     Apply settings from PrefabConfigs to EligiblePrefabs
    /// </summary>
    public static void ApplyPrefabConfigSettings()
    {
        foreach (KeyValuePair<string, GameObject> pair in EligiblePrefabMap)
        {
            if (!pair.Value)
            {
                Log.LogWarning($"Prefab {pair.Key} is null!");
                continue;
            }
            if (!PrefabConfigManager.TryGetPrefabConfig(pair.Key, out var prefabConfig, checkIfBound: true))
            {
                Log.LogWarning($"Prefab {pair.Key} does not have a valid config!");
                continue;
            }

            Piece piece = prefabConfig.Piece;
            // set piece visible in PieceTable based on MVBP config
            piece.m_enabled = prefabConfig.Enabled.Value || MorePrefabs.IsForceAllPrefabs;
            piece.m_name = PieceNameManager.FormatPieceName(prefabConfig);
            piece.m_description = PieceNameManager.GetPieceDescription(prefabConfig);
            SetCanBeRemoved(piece);
            piece.m_allowedInDungeons = prefabConfig.AllowedInDungeons.Value;
            piece.m_clipEverything = prefabConfig.ClipEverything.Value;
            piece.m_clipGround = prefabConfig.ClipGround.Value;
            piece.m_category = PieceCategoryManager.GetPieceCategory(prefabConfig.Category.Value);
            piece.m_craftingStation = GetCraftingStation(prefabConfig.CraftingStation.Value);
            piece.m_resources = PieceReqsManager.ConfigurePieceRequirements(prefabConfig);
            SfxManager.FixPlacementSfx(piece);
        }
    }

    /// <summary>
    ///     Set whether a piece can be removed based on the build hammer category and
    ///     if it has had a Piece component added by MVBP. 
    /// </summary>
    /// <param name="piece"></param>
    private static void SetCanBeRemoved(Piece piece)
    {
        // Prevent CreativeMode pieces and any clones of them from being removable.
        // (Player.RemovePiece patch allows removing player-built instances).
        // Mimic Vanilla, make ships/carts non-removable.
        if (HasPieceAddedByMVBP(piece) || IsNonRemovablePiece(piece))
        {
            piece.m_canBeRemoved = false;
        }
        else if (TryGetDefaultRemoveSettings(piece.gameObject, out bool canRemove))
        {
            piece.m_canBeRemoved = canRemove;
        }
        else
        {
            Log.LogWarning($"Could not find remove settings for {piece.name}");
        }
    }

    internal static bool IsNonRemovablePiece(Piece piece)
    {
        return (
            PieceCategoryManager.IsCreativeModePiece(piece) ||
            piece.GetComponent<Ship>() ||
            piece.GetComponent<Vagon>()
        );
    }

    private static CraftingStation GetCraftingStation(string name)
    {
        string internalName = CraftingStations.GetInternalName(name);
        CraftingStation station = ZNetScene.instance?.GetPrefab(internalName)?.GetComponent<CraftingStation>();
        return station;
    }
}
