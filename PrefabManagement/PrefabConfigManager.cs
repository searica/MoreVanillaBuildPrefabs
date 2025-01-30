// Ignore Spelling: MVBP
using System;
using System.Collections.Generic;
using Jotunn.Configs;
using UnityEngine;
using Logging;
using MVBP.Extensions;
using MVBP.PieceManagement;
using System.Linq;


namespace MVBP.PrefabManagement;

internal static class PrefabConfigManager
{
    private static readonly HashSet<string> DoNotCacheIcon =
    [
        "portal",
        "dvergrprops_wood_floor",
        "dvergrprops_wood_stair",
    ];

    internal static bool ShouldCacheIcon(string name) => !DoNotCacheIcon.Contains(name);

    internal static readonly HashSet<string> DvergrWoodPieces =
    [
        "dvergrprops_wood_floor",
        "dvergrprops_wood_stair",
    ];

    public static List<PrefabConfig> GetPrefabConfigs(bool checkIfBound = true)
    {
        if (!checkIfBound)
        {
            return PrefabConfigMap.Values.ToList();
        }
        return PrefabConfigMap.Values.Where(x => x.IsBound).ToList();
    }

    /// <summary>
    ///     Get a bool indicating if the prefab is configured to require a placement patch.
    /// </summary>
    /// <param name="PrefabName"></param>
    /// <returns></returns>
    internal static bool NeedsCollisionPatchForGhost(string prefabName)
    {
        if (TryGetPrefabConfig(prefabName, out var prefabConfig, checkIfBound: true))
        {
            return prefabConfig.PlacementPatch.Value;
        }
        return false;
    }


    internal static bool IsPrefabEnabled(GameObject gameObject)
    {
        if (TryGetPrefabConfig(gameObject.GetPrefabName(), out var prefabConfig, checkIfBound: true))
        {
            return prefabConfig.Enabled.Value || MorePrefabs.IsForceAllPrefabs;
        }
        return false;
    }

    /// <summary>
    ///     Tries to get the PrefabConfig for the root prefab version of the game object.
    /// </summary>
    /// <param name="prefab"></param>
    /// <param name="prefabConfig"></param>
    /// <param name="checkIfBound">Whether to only return true if PrefabConfig IsBound.</param>
    /// <returns></returns>
    public static bool TryGetPrefabConfig(GameObject prefab, out PrefabConfig prefabConfig, bool checkIfBound = false)
    {
        return TryGetPrefabConfig(prefab.GetPrefabName(), out prefabConfig, checkIfBound);
    }

    /// <summary>
    ///     Tries to get the PrefabConfig for the prefab name.
    /// </summary>
    /// <param name="prefabName"></param>
    /// <param name="prefabConfig"></param>
    /// <param name="checkIfBound">Whether to only return true if PrefabConfig IsBound.</param>
    /// <returns></returns>
    public static bool TryGetPrefabConfig(string prefabName, out PrefabConfig prefabConfig, bool checkIfBound = false)
    {
        if (PrefabConfigMap.TryGetValue(prefabName, out prefabConfig))
        {
            return !checkIfBound || prefabConfig.IsBound;
        }
        return false;
    }

    /// <summary>
    ///     Gets PrefabConfig for the prefab if it exists or binds a new PrefabConfig if needed.
    /// </summary>
    /// <param name="prefab"></param>
    /// <returns></returns>
    public static PrefabConfig BindPrefabConfig(GameObject prefab, Piece piece)
    {
        string prefabName = prefab.GetPrefabName();      
        if (!PrefabConfigMap.TryGetValue(prefabName, out PrefabConfig prefabConfig))
        {
            // if no existing PrefabConfig then make one.
            prefabConfig = new(prefabName);
            PrefabConfigMap[prefabName] = prefabConfig;
        }

        if (!prefabConfig.IsBound)
        {
            Internal_BindPrefabConfig(prefabConfig, prefab, piece);
        }
        return prefabConfig;
    }

    /// <summary>
    ///     Gets PrefabConfig for the prefab if it exists or binds a new PrefabConfig if needed.
    /// </summary>
    /// <param name="prefab"></param>
    /// <returns></returns>
    public static PrefabConfig BindPrefabConfig(string prefabName)
    {
        if (!PrefabConfigMap.TryGetValue(prefabName, out PrefabConfig prefabConfig))
        {
            // if no existing PrefabConfig then make one.
            prefabConfig = new(prefabName);
            PrefabConfigMap[prefabName] = prefabConfig;
        }

        if (!prefabConfig.IsBound)
        {
            if (!ZNetPrefabManager.TryGetEligiblePrefab(prefabName, out GameObject prefab))
            {
                string msg = $"{prefabName} is not an eligible prefab for MVBP! Cannot bind config.";
                Log.LogWarning(msg);
                throw new ArgumentException(msg);
            }
            if (!prefab.TryGetComponent(out Piece piece))
            {
                string msg = $"{prefabName} is missing a piece component! Cannot bind config.";
                Log.LogWarning(msg);
                throw new ArgumentException(msg);
            }
            Internal_BindPrefabConfig(prefabConfig, prefab, piece);
        }
        return prefabConfig;
    }

    private static void Internal_BindPrefabConfig(PrefabConfig prefabConfig, GameObject prefab, Piece piece)
    {
        prefabConfig.BindToConfig(MorePrefabs.Instance.Config, prefab, piece);
        prefabConfig.Enabled.SettingChanged += UpdateController.PieceSettingChanged;
        prefabConfig.AllowedInDungeons.SettingChanged += UpdateController.PieceSettingChanged;
        prefabConfig.Category.SettingChanged += UpdateController.PieceSettingChanged;
        prefabConfig.CraftingStation.SettingChanged += UpdateController.PieceSettingChanged;
        prefabConfig.Requirements.SettingChanged += UpdateController.PieceSettingChanged;
        prefabConfig.ClipEverything.SettingChanged += UpdateController.PieceSettingChanged;
        prefabConfig.ClipGround.SettingChanged += UpdateController.PieceSettingChanged;

        prefabConfig.PlacementPatch.SettingChanged += UpdateController.PlacementSettingChanged;
    }

    private static readonly Dictionary<string, PrefabConfig> PrefabConfigMap = new()
    {
        {
            "ArmorStand_Female",
            new PrefabConfig(
                name: "ArmorStand_Female",
                enabled: true,
                allowedInDungeons: false,
                category: HammerCategories.Furniture,
                craftingStation: nameof(CraftingStations.Workbench),
                requirements: "FineWood,8;BronzeNails,2;Tar,4",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceDesc: "",
                pieceGroup: PieceClassification.ArmorStand
            )
        },
        {
            "ArmorStand_Male",
            new PrefabConfig(
                name: "ArmorStand_Male",
                enabled: true,
                allowedInDungeons: false,
                category: HammerCategories.Furniture,
                craftingStation: nameof(CraftingStations.Workbench),
                requirements: "FineWood,8;BronzeNails,2;Tar,4",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceDesc: "",
                pieceGroup: PieceClassification.ArmorStand
            )
        },
        {
            "Ashland_Stair",
            new PrefabConfig(
                name: "Ashland_Stair",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.Stonecutter),
                requirements: "Grausten,32",
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "Ashland_Steepstair",
            new PrefabConfig(
                name: "Ashland_Steepstair",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: false,
                placementPatch: true
            )
        },
        {
            "AshlandsBranch1",
            new PrefabConfig(
                name: "AshlandsBranch1",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: false,
                placementPatch: true
            )
        },
        {
            "AshlandsBranch2",
            new PrefabConfig(
                name: "AshlandsBranch2",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "AshlandsBranch3",
            new PrefabConfig(
                name: "AshlandsBranch3",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: false,
                placementPatch: true
            )
        },
        {
            "AshlandsBush1",
            new PrefabConfig(
                name: "AshlandsBush1",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: false,
                placementPatch: true
            )
        },
        {
            "AshlandsBush2",
            new PrefabConfig(
                name: "AshlandsBush2",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: false,
                placementPatch: true
            )
        },
        {
            "AshlandsTree1",
            new PrefabConfig(
                name: "AshlandsTree1",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: true,
                clipGround: false,
                placementPatch: false,
                placementOffset: new Vector3(0f, 1f, 0f)
            )
        },
        {
            "AshlandsTree3",
            new PrefabConfig(
                name: "AshlandsTree3",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: true,
                clipGround: false,
                placementPatch: false,
                placementOffset: new Vector3(0f, 1f, 0f)
            )
        },
        {
            "AshlandsTree4",
            new PrefabConfig(
                name: "AshlandsTree4",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: true,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "AshlandsTree5",
            new PrefabConfig(
                name: "AshlandsTree5",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: true,
                clipGround: false,
                placementPatch: false,
                placementOffset: new Vector3(0f, 0.5f, 0f)
            )
        },
        {
            "AshlandsTree6",
            new PrefabConfig(
                name: "AshlandsTree6",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: true,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "AshlandsTree6_big",
            new PrefabConfig(
                name: "AshlandsTree6_big",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: true,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Flora
            )
        },
        {
            "AshlandsTreeLog1",
            new PrefabConfig(
                name: "AshlandsTreeLog1",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Flora
            )
        },
        {
            "AshlandsTreeLog2",
            new PrefabConfig(
                name: "AshlandsTreeLog2",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Flora
            )
        },
        {
            "AshlandsTreeLogHalf1",
            new PrefabConfig(
                name: "AshlandsTreeLogHalf1",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Flora
            )
        },
        {
            "AshlandsTreeLogHalf2",
            new PrefabConfig(
                name: "AshlandsTreeLogHalf2",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Flora
            )
        },
        {
            "AshlandsTreeStump1",
            new PrefabConfig(
                name: "AshlandsTreeStump1",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Flora
            )
        },
        {
            "AshlandsTreeStump2",
            new PrefabConfig(
                name: "AshlandsTreeStump2",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Flora
            )
        },
        {
            "AshlandsTreeStump3",
            new PrefabConfig(
                name: "AshlandsTreeStump3",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: true,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Flora
            )
        },
        {
            "Ashlands_Altar",
            new PrefabConfig(
                name: "Ashlands_Altar",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "Ashlands_Arch1",
            new PrefabConfig(
                name: "Ashlands_Arch1",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.Stonecutter),
                requirements: "Grausten,8",
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "Ashlands_Arch2",
            new PrefabConfig(
                name: "Ashlands_Arch2",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.Stonecutter),
                requirements: "Grausten,10",
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "Ashlands_Arch2_Broken1",
            new PrefabConfig(
                name: "Ashlands_Arch2_Broken1",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "Ashlands_Arch2_Broken2",
            new PrefabConfig(
                name: "Ashlands_Arch2_Broken2",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "Ashlands_ArchRoof",
            new PrefabConfig(
                name: "Ashlands_ArchRoof",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: false,
                placementPatch: true
            )
        },
        {
            "Ashlands_ArchRoofDamaged",
            new PrefabConfig(
                name: "Ashlands_ArchRoofDamaged",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "Ashlands_ArchRoofDamaged_half1",
            new PrefabConfig(
                name: "Ashlands_ArchRoofDamaged_half1",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "Ashlands_ArchRoofDamaged_half2",
            new PrefabConfig(
                name: "Ashlands_ArchRoofDamaged_half2",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "Ashlands_ArchRoofLong_Damaged",
            new PrefabConfig(
                name: "Ashlands_ArchRoofLong_Damaged",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "Ashlands_Boss_Pillar",
            new PrefabConfig(
                name: "Ashlands_Boss_Pillar",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: false,
                placementPatch: true
            )
        },
        {
            "Ashlands_Boss_Pillar_Twist_broken1",
            new PrefabConfig(
                name: "Ashlands_Boss_Pillar_Twist_broken1",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "Ashlands_Boss_Pillar_Twist_broken2",
            new PrefabConfig(
                name: "Ashlands_Boss_Pillar_Twist_broken2",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "Ashlands_Boss_Pillar_Twist_broken3",
            new PrefabConfig(
                name: "Ashlands_Boss_Pillar_Twist_broken3",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "Ashlands_Floor",
            new PrefabConfig(
                name: "Ashlands_Floor",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.Stonecutter),
                requirements: "Grausten,4",
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "Ashlands_Fortress_Floor",
            new PrefabConfig(
                name: "Ashlands_Fortress_Floor",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.Stonecutter),
                requirements: "Grausten,6",
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "Ashlands_Fortress_Gate",
            new PrefabConfig(
                name: "Ashlands_Fortress_Gate",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.Stonecutter),
                requirements: "Grausten,32",
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "Ashlands_Fortress_Gate_Door",
            new PrefabConfig(
                name: "Ashlands_Fortress_Gate_Door",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.BlackForge),
                requirements: "Copper,35",
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "Ashlands_Fortress_Wall_PillarTopStone_frac",
            new PrefabConfig(
                name: "Ashlands_Fortress_Wall_PillarTopStone_frac",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: false,
                placementPatch: true,
                placementOffset: new Vector3(0f, -0.1f, 0f)
            )
        },
        {
            "Ashlands_Fortress_Wall_PillarTop_frac",
            new PrefabConfig(
                name: "Ashlands_Fortress_Wall_PillarTop_frac",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.Stonecutter),
                requirements: "",
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "Ashlands_Fortress_Wall_Pillar_frac",
            new PrefabConfig(
                name: "Ashlands_Fortress_Wall_Pillar_frac",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.Stonecutter),
                requirements: "",
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "Ashlands_Fortress_Wall_Spikes",
            new PrefabConfig(
                name: "Ashlands_Fortress_Wall_Spikes",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.BlackForge),
                requirements: "Copper,6",
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "Ashlands_Pillar4",
            new PrefabConfig(
                name: "Ashlands_Pillar4",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.Stonecutter),
                requirements: "Grausten,10",
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "Ashlands_Pillar4_tip",
            new PrefabConfig(
                name: "Ashlands_Pillar4_tip",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.Stonecutter),
                requirements: "Grausten,10",
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "Ashlands_Pillar4_tip2",
            new PrefabConfig(
                name: "Ashlands_Pillar4_tip2",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.Stonecutter),
                requirements: "Grausten,10",
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "Ashlands_Pillar4_tip2_broken1",
            new PrefabConfig(
                name: "Ashlands_Pillar4_tip2_broken1",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "Ashlands_Pillar4_tip2_broken2",
            new PrefabConfig(
                name: "Ashlands_Pillar4_tip2_broken2",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "Ashlands_Pillar4_tip3",
            new PrefabConfig(
                name: "Ashlands_Pillar4_tip3",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "Ashlands_Pillar4_tip3_broken1",
            new PrefabConfig(
                name: "Ashlands_Pillar4_tip3_broken1",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "Ashlands_Pillar4_tip3_broken2",
            new PrefabConfig(
                name: "Ashlands_Pillar4_tip3_broken2",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "Ashlands_Pillar4_tip3_broken3",
            new PrefabConfig(
                name: "Ashlands_Pillar4_tip3_broken3",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "Ashlands_Pillar4_tip_broken1",
            new PrefabConfig(
                name: "Ashlands_Pillar4_tip_broken1",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "Ashlands_Pillar4_tip_broken2",
            new PrefabConfig(
                name: "Ashlands_Pillar4_tip_broken2",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "Ashlands_PillarBase3_double",
            new PrefabConfig(
                name: "Ashlands_PillarBase3_double",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.Stonecutter),
                requirements: "Grausten,3",
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "Ashlands_Ramp",
            new PrefabConfig(
                name: "Ashlands_Ramp",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.Stonecutter),
                requirements: "Grausten,10",
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "Ashlands_Ruins_Floor_1point5x1point5",
            new PrefabConfig(
                name: "Ashlands_Ruins_Floor_1point5x1point5",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "Ashlands_Ruins_Floor_1point5x1point5_broken",
            new PrefabConfig(
                name: "Ashlands_Ruins_Floor_1point5x1point5_broken",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "Ashlands_Ruins_Floor_3x3",
            new PrefabConfig(
                name: "Ashlands_Ruins_Floor_3x3",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "Ashlands_Ruins_Floor_3x3_broken1",
            new PrefabConfig(
                name: "Ashlands_Ruins_Floor_3x3_broken1",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "Ashlands_Ruins_Floor_3x3_broken2",
            new PrefabConfig(
                name: "Ashlands_Ruins_Floor_3x3_broken2",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "Ashlands_Ruins_Floor_3x3_broken3",
            new PrefabConfig(
                name: "Ashlands_Ruins_Floor_3x3_broken3",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "Ashlands_Ruins_Floor_6x6",
            new PrefabConfig(
                name: "Ashlands_Ruins_Floor_6x6",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "Ashlands_Ruins_Floor_6x6_broken1",
            new PrefabConfig(
                name: "Ashlands_Ruins_Floor_6x6_broken1",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "Ashlands_Ruins_Floor_6x6_broken2",
            new PrefabConfig(
                name: "Ashlands_Ruins_Floor_6x6_broken2",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: false,
                placementPatch: true
            )
        },
        {
            "Ashlands_Ruins_Ramp",
            new PrefabConfig(
                name: "Ashlands_Ruins_Ramp",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.Stonecutter),
                requirements: "Grausten,10",
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "Ashlands_Ruins_Ramp_Upsidedown",
            new PrefabConfig(
                name: "Ashlands_Ruins_Ramp_Upsidedown",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.Stonecutter),
                requirements: "Grausten,10",
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "Ashlands_Ruins_TopStone",
            new PrefabConfig(
                name: "Ashlands_Ruins_TopStone",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "Ashlands_Ruins_Wall_4x6",
            new PrefabConfig(
                name: "Ashlands_Ruins_Wall_4x6",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "Ashlands_Ruins_Wall_Broken3_4x6",
            new PrefabConfig(
                name: "Ashlands_Ruins_Wall_Broken3_4x6",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "Ashlands_Ruins_Wall_Broken4_4x6",
            new PrefabConfig(
                name: "Ashlands_Ruins_Wall_Broken4_4x6",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "Ashlands_Ruins_Wall_Broken5_4x6",
            new PrefabConfig(
                name: "Ashlands_Ruins_Wall_Broken5_4x6",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "Ashlands_Ruins_Wall_Top_wHole",
            new PrefabConfig(
                name: "Ashlands_Ruins_Wall_Top_wHole",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.Stonecutter),
                requirements: "Grausten,16",
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "Ashlands_Ruins_Wall_Window_4x6_broken2",
            new PrefabConfig(
                name: "Ashlands_Ruins_Wall_Window_4x6_broken2",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "Ashlands_Ruins_Wall_Window_4x6_broken3",
            new PrefabConfig(
                name: "Ashlands_Ruins_Wall_Window_4x6_broken3",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "Ashlands_Ruins_Wall_Window_4x6_broken4",
            new PrefabConfig(
                name: "Ashlands_Ruins_Wall_Window_4x6_broken4",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "Ashlands_Ruins_Wall_Window_4x6_broken5",
            new PrefabConfig(
                name: "Ashlands_Ruins_Wall_Window_4x6_broken5",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "Ashlands_Ruins_Wall_Window_4x6_broken6",
            new PrefabConfig(
                name: "Ashlands_Ruins_Wall_Window_4x6_broken6",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "Ashlands_Ruins_Wall_Windows_Broken_4x6",
            new PrefabConfig(
                name: "Ashlands_Ruins_Wall_Windows_Broken_4x6",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: false,
                placementPatch: true
            )
        },
        {
            "Ashlands_Ruins_twist_ArchBig",
            new PrefabConfig(
                name: "Ashlands_Ruins_twist_ArchBig",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.Stonecutter),
                requirements: "Grausten,8",
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "Ashlands_Ruins_twist_PillarBase",
            new PrefabConfig(
                name: "Ashlands_Ruins_twist_PillarBase",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.Stonecutter),
                requirements: "Grausten,8",
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "Ashlands_Ruins_twist_PillarBaseSmall",
            new PrefabConfig(
                name: "Ashlands_Ruins_twist_PillarBaseSmall",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.Stonecutter),
                requirements: "Grausten,3",
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "Ashlands_StairsBroad",
            new PrefabConfig(
                name: "Ashlands_StairsBroad",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.Stonecutter),
                requirements: "Grausten,24",
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "Ashlands_WallBlock",
            // This prefab acts as if it does not have correct WearNTear data, so it may have issues.
            new PrefabConfig(
                name: "Ashlands_WallBlock",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "Ashlands_WallBlock_1x2x2",
            new PrefabConfig(
                name: "Ashlands_WallBlock_1x2x2",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.Stonecutter),
                requirements: "Grausten,2",
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "Ashlands_WallBlock_base",
            new PrefabConfig(
                name: "Ashlands_WallBlock_base",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.Stonecutter),
                requirements: "Grausten,3",
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "Ashlands_Wall_2x2",
            new PrefabConfig(
                name: "Ashlands_Wall_2x2",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.Stonecutter),
                requirements: "Grausten,6",
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "Ashlands_Wall_2x2_cornerL",
            new PrefabConfig(
                name: "Ashlands_Wall_2x2_cornerL",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.Stonecutter),
                requirements: "Grausten,8",
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "Ashlands_Wall_2x2_cornerL_top",
            new PrefabConfig(
                name: "Ashlands_Wall_2x2_cornerL_top",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.Stonecutter),
                requirements: "Grausten,8",
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "Ashlands_Wall_2x2_cornerR",
            new PrefabConfig(
                name: "Ashlands_Wall_2x2_cornerR",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.Stonecutter),
                requirements: "Grausten,8",
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "Ashlands_Wall_2x2_cornerR_top",
            new PrefabConfig(
                name: "Ashlands_Wall_2x2_cornerR_top",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.Stonecutter),
                requirements: "Grausten,8",
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "Ashlands_Wall_2x2_edge",
            new PrefabConfig(
                name: "Ashlands_Wall_2x2_edge",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.Stonecutter),
                requirements: "Grausten,6",
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "Ashlands_Wall_2x2_edge2",
            new PrefabConfig(
                name: "Ashlands_Wall_2x2_edge2",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.Stonecutter),
                requirements: "Grausten,6",
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "Ashlands_Wall_2x2_edge2_top",
            new PrefabConfig(
                name: "Ashlands_Wall_2x2_edge2_top",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "Ashlands_Wall_2x2_edge_top",
            new PrefabConfig(
                name: "Ashlands_Wall_2x2_edge_top",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.Stonecutter),
                requirements: "Grausten,6",
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "Ashlands_Wall_2x2_top",
            new PrefabConfig(
                name: "Ashlands_Wall_2x2_top",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.Stonecutter),
                requirements: "Grausten,6",
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "Ashlands_floor_large_fractured",
            new PrefabConfig(
                name: "Ashlands_floor_large_fractured",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "Beech1",
            new PrefabConfig(
                name: "Beech1",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.Nature,
                craftingStation: nameof(CraftingStations.None),
                requirements: "Wood,16",
                clipEverything: true,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Flora
            )
        },
        {
            "Beech_Stub",
            new PrefabConfig(
                name: "Beech_Stub",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.Nature,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: true,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Flora
            )
        },
        {
            "Beech_small1",
            new PrefabConfig(
                name: "Beech_small1",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.Nature,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Flora
            )
        },
        {
            "Beech_small2",
            new PrefabConfig(
                name: "Beech_small2",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.Nature,
                craftingStation: nameof(CraftingStations.None),
                requirements: "Wood,1",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Flora
            )
        },
        {
            "Birch1",
            new PrefabConfig(
                name: "Birch1",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.Nature,
                craftingStation: nameof(CraftingStations.None),
                requirements: "BirchSeeds,1;FineWood,2",
                clipEverything: true,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Flora
            )
        },
        {
            "Birch1_aut",
            new PrefabConfig(
                name: "Birch1_aut",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.Nature,
                craftingStation: nameof(CraftingStations.None),
                requirements: "BirchSeeds,1;FineWood,2",
                clipEverything: true,
                clipGround: false,
                placementPatch: false,
                pieceName: "Birch1 (autumn)",
                pieceGroup: PieceClassification.Flora
            )
        },
        {
            "Birch2",
            new PrefabConfig(
                name: "Birch2",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.Nature,
                craftingStation: nameof(CraftingStations.None),
                requirements: "BirchSeeds,1;FineWood,2",
                clipEverything: true,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Flora
            )
        },
        {
            "Birch2_aut",
            new PrefabConfig(
                name: "Birch2_aut",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.Nature,
                craftingStation: nameof(CraftingStations.None),
                requirements: "BirchSeeds,1;FineWood,2",
                clipEverything: true,
                clipGround: false,
                placementPatch: false,
                pieceName: "Birch (autumn)",
                pieceGroup: PieceClassification.Flora
            )
        },
        {
            "BirchStub",
            new PrefabConfig(
                name: "BirchStub",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.Nature,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: true,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Flora
            )
        },
        {
            "Birch_log",
            new PrefabConfig(
                name: "Birch_log",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.Nature,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Flora
            )
        },
        {
            "Birch_log_half",
            new PrefabConfig(
                name: "Birch_log_half",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.Nature,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Flora
            )
        },
        {
            "BlueberryBush",
            new PrefabConfig(
                name: "BlueberryBush",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.Nature,
                craftingStation: nameof(CraftingStations.None),
                requirements: "Blueberries,5",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Plant
            )
        },
        {
            "BogWitch_Fire_Pit",
            new PrefabConfig(
                name: "BogWitch_Fire_Pit",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "BossStone_Bonemass",
            new PrefabConfig(
                name: "BossStone_Bonemass",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: false,
                placementPatch: true,
                pieceGroup: PieceClassification.Misc
            )
        },
        {
            "BossStone_DragonQueen",
            new PrefabConfig(
                name: "BossStone_DragonQueen",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: false,
                placementPatch: true,
                pieceGroup: PieceClassification.Misc
            )
        },
        {
            "BossStone_Eikthyr",
            new PrefabConfig(
                name: "BossStone_Eikthyr",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: false,
                placementPatch: true,
                pieceGroup: PieceClassification.Misc
            )
        },
        {
            "BossStone_Fader",
            new PrefabConfig(
                name: "BossStone_Fader",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: false,
                placementPatch: true
            )
        },
        {
            "BossStone_TheElder",
            new PrefabConfig(
                name: "BossStone_TheElder",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: false,
                placementPatch: true,
                pieceGroup: PieceClassification.Misc
            )
        },
        {
            "BossStone_TheQueen",
            new PrefabConfig(
                name: "BossStone_TheQueen",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: false,
                placementPatch: true,
                pieceGroup: PieceClassification.Misc
            )
        },
        {
            "BossStone_Yagluth",
            new PrefabConfig(
                name: "BossStone_Yagluth",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: false,
                placementPatch: true,
                pieceGroup: PieceClassification.Misc
            )
        },
        {
            "Bush01",
            new PrefabConfig(
                name: "Bush01",
                enabled: true,
                allowedInDungeons: false,
                category: HammerCategories.Nature,
                craftingStation: nameof(CraftingStations.None),
                requirements: "Wood,2",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Flora
            )
        },
        {
            "Bush01_heath",
            new PrefabConfig(
                name: "Bush01_heath",
                enabled: true,
                allowedInDungeons: false,
                category: HammerCategories.Nature,
                craftingStation: nameof(CraftingStations.None),
                requirements: "Wood,2",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Flora
            )
        },
        {
            "Bush02_en",
            new PrefabConfig(
                name: "Bush02_en",
                enabled: true,
                allowedInDungeons: false,
                category: HammerCategories.Nature,
                craftingStation: nameof(CraftingStations.None),
                requirements: "Wood,2",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Flora
            )
        },
        {
            "Candle_resin_bogwitch",
            new PrefabConfig(
                name: "Candle_resin_bogwitch",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: false,
                placementPatch: true
            )
        },
        {
            "CastleKit_braided_box01",
            new PrefabConfig(
                name: "CastleKit_braided_box01",
                enabled: true,
                allowedInDungeons: false,
                category: HammerCategories.Misc,
                craftingStation: nameof(CraftingStations.Workbench),
                requirements: "Wood,2",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceName: "Wood box",
                pieceGroup: PieceClassification.Misc
            )
        },
        {
            "CastleKit_groundtorch",
            new PrefabConfig(
                name: "CastleKit_groundtorch",
                enabled: true,
                allowedInDungeons: false,
                category: HammerCategories.Furniture,
                craftingStation: nameof(CraftingStations.Forge),
                requirements: "Iron,2;Resin,2;SurtlingCore,1",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceName: "Standing iron torch (everburning)",
                pieceDesc: "Burns eternally without fuel.",
                pieceGroup: PieceClassification.Torch,
                playerBasePatch: true
            )
        },
        {
            "CastleKit_groundtorch_blue",
            new PrefabConfig(
                name: "CastleKit_groundtorch_blue",
                enabled: true,
                allowedInDungeons: false,
                category: HammerCategories.Furniture,
                craftingStation: nameof(CraftingStations.Forge),
                requirements: "Iron,2;GreydwarfEye,2;SurtlingCore,1",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceName: "Standing blue-burning iron torch (everburning)",
                pieceDesc: "Burns eternally without fuel.",
                pieceGroup: PieceClassification.Torch,
                playerBasePatch: true
            )
        },
        {
            "CastleKit_groundtorch_green",
            new PrefabConfig(
                name: "CastleKit_groundtorch_green",
                enabled: true,
                allowedInDungeons: false,
                category: HammerCategories.Furniture,
                craftingStation: nameof(CraftingStations.Forge),
                requirements: "Iron,2;Guck,2;SurtlingCore,1",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceName: "Standing green-burning iron torch (everburning)",
                pieceDesc: "Burns eternally without fuel.",
                pieceGroup: PieceClassification.Torch,
                playerBasePatch: true
            )
        },
        {
            "CharredBanner1",
            new PrefabConfig(
                name: "CharredBanner1",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "CharredBanner2",
            new PrefabConfig(
                name: "CharredBanner2",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "CharredBanner3",
            new PrefabConfig(
                name: "CharredBanner3",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "Charred_altar_bellfragment",
            new PrefabConfig(
                name: "Charred_altar_bellfragment",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "Charredfortress_LOD",
            new PrefabConfig(
                name: "Charredfortress_LOD",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: true,
                clipGround: false,
                placementPatch: true
            )
        },
        {
            "Chest",
            new PrefabConfig(
                name: "Chest",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.Furniture,
                craftingStation: nameof(CraftingStations.None),
                requirements: "Wood,10;Iron,1",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Chest
            )
        },
        {
            "Cinder",
            new PrefabConfig(
                name: "Cinder",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: false,
                placementPatch: true
            )
        },
        {
            "CinderSky",
            new PrefabConfig(
                name: "CinderSky",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "CinderStorm",
            new PrefabConfig(
                name: "CinderStorm",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "Cinder_campfire",
            new PrefabConfig(
                name: "Cinder_campfire",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "CloudberryBush",
            new PrefabConfig(
                name: "CloudberryBush",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.Nature,
                craftingStation: nameof(CraftingStations.None),
                requirements: "Cloudberry,5",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Plant
            )
        },
        {
            "CreepProp_egg_hanging01",
            new PrefabConfig(
                name: "CreepProp_egg_hanging01",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.Nature,
                craftingStation: nameof(CraftingStations.None),
                requirements: "YggdrasilWood,2",
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "CreepProp_egg_hanging02",
            new PrefabConfig(
                name: "CreepProp_egg_hanging02",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.Nature,
                craftingStation: nameof(CraftingStations.None),
                requirements: "YggdrasilWood,2",
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "CreepProp_entrance1",
            new PrefabConfig(
                name: "CreepProp_entrance1",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.Nature,
                craftingStation: nameof(CraftingStations.None),
                requirements: "YggdrasilWood,2",
                clipEverything: true,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "CreepProp_entrance2",
            new PrefabConfig(
                name: "CreepProp_entrance2",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.Nature,
                craftingStation: nameof(CraftingStations.None),
                requirements: "YggdrasilWood,2",
                clipEverything: true,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "CreepProp_hanging01",
            new PrefabConfig(
                name: "CreepProp_hanging01",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.Nature,
                craftingStation: nameof(CraftingStations.None),
                requirements: "YggdrasilWood,2",
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "CreepProp_wall01",
            new PrefabConfig(
                name: "CreepProp_wall01",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.Nature,
                craftingStation: nameof(CraftingStations.None),
                requirements: "YggdrasilWood,2",
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "FernAshlands",
            new PrefabConfig(
                name: "FernAshlands",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "FernFiddleHeadAshlands",
            new PrefabConfig(
                name: "FernFiddleHeadAshlands",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: false,
                placementPatch: true
            )
        },
        {
            "FirTree",
            new PrefabConfig(
                name: "FirTree",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.Nature,
                craftingStation: nameof(CraftingStations.None),
                requirements: "Wood,16",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Flora
            )
        },
        {
            "FirTree_Stub",
            new PrefabConfig(
                name: "FirTree_Stub",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.Nature,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: true,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Flora
            )
        },
        {
            "FirTree_log",
            new PrefabConfig(
                name: "FirTree_log",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.Nature,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Flora
            )
        },
        {
            "FirTree_log_half",
            new PrefabConfig(
                name: "FirTree_log_half",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.Nature,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Flora
            )
        },
        {
            "FirTree_oldLog",
            new PrefabConfig(
                name: "FirTree_oldLog",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.Nature,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Flora
            )
        },
        {
            "FirTree_small",
            new PrefabConfig(
                name: "FirTree_small",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.Nature,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Flora
            )
        },
        {
            "FirTree_small_dead",
            new PrefabConfig(
                name: "FirTree_small_dead",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.Nature,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Flora
            )
        },
        {
            "Fire",
            new PrefabConfig(
                name: "Fire",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "FireFlies",
            new PrefabConfig(
                name: "FireFlies",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "FlametalRockstand_frac",
            new PrefabConfig(
                name: "FlametalRockstand_frac",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "GlowingMushroom",
            new PrefabConfig(
                name: "GlowingMushroom",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.Nature,
                craftingStation: nameof(CraftingStations.None),
                requirements: "Ooze,1",
                clipEverything: true,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Flora
            )
        },
        {
            "GraveStone_Broken_CharredTwitcherNest",
            new PrefabConfig(
                name: "GraveStone_Broken_CharredTwitcherNest",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "GraveStone_Broken_World",
            new PrefabConfig(
                name: "GraveStone_Broken_World",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "GraveStone_CharredFaderLocation",
            new PrefabConfig(
                name: "GraveStone_CharredFaderLocation",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "GraveStone_CharredTwitcherNest",
            new PrefabConfig(
                name: "GraveStone_CharredTwitcherNest",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "GraveStone_Elite_Broken_CharredTwitcherNest",
            new PrefabConfig(
                name: "GraveStone_Elite_Broken_CharredTwitcherNest",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "GraveStone_Elite_CharredTwitcherNest",
            new PrefabConfig(
                name: "GraveStone_Elite_CharredTwitcherNest",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "Greydwarf_Root",
            new PrefabConfig(
                name: "Greydwarf_Root",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.Nature,
                craftingStation: nameof(CraftingStations.None),
                requirements: "Wood,2",
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "GuckSack",
            new PrefabConfig(
                name: "GuckSack",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "Guck,12",
                clipEverything: true,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "GuckSack_small",
            new PrefabConfig(
                name: "GuckSack_small",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "Guck,6",
                clipEverything: true,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "HouseFire",
            new PrefabConfig(
                name: "HouseFire",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "HugeRoot1",
            new PrefabConfig(
                name: "HugeRoot1",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.Nature,
                craftingStation: nameof(CraftingStations.None),
                requirements: "ElderBark,32",
                clipEverything: true,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Flora
            )
        },
        {
            "IceBlocker",
            new PrefabConfig(
                name: "IceBlocker",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "Ice_floor",
            new PrefabConfig(
                name: "Ice_floor",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "Crystal,16",
                clipEverything: false,
                clipGround: true,
                placementPatch: false
            )
        },
        {
            "LavaRock",
            new PrefabConfig(
                name: "LavaRock",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: true,
                clipGround: false,
                placementPatch: true
            )
        },
        {
            "Leviathan",
            new PrefabConfig(
                name: "Leviathan",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "Chitin,50",
                clipEverything: true,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "LeviathanLava",
            new PrefabConfig(
                name: "LeviathanLava",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: true,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "LuredWisp",
            new PrefabConfig(
                name: "LuredWisp",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "Wisp,1",
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "MineRock_Copper",
            new PrefabConfig(
                name: "MineRock_Copper",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.Nature,
                craftingStation: nameof(CraftingStations.None),
                requirements: "Stone,10;CopperOre,10",
                clipEverything: true,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Ore
            )
        },
        {
            "MineRock_Iron",
            new PrefabConfig(
                name: "MineRock_Iron",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.Nature,
                craftingStation: nameof(CraftingStations.None),
                requirements: "Stone,10;IronScrap,10",
                clipEverything: true,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Ore
            )
        },
        {
            "MineRock_Meteorite",
            new PrefabConfig(
                name: "MineRock_Meteorite",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.Nature,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: true,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Ore
            )
        },
        {
            "MineRock_Obsidian",
            new PrefabConfig(
                name: "MineRock_Obsidian",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.Nature,
                craftingStation: nameof(CraftingStations.None),
                requirements: "Obsidian,7",
                clipEverything: true,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Ore
            )
        },
        {
            "MineRock_Stone",
            new PrefabConfig(
                name: "MineRock_Stone",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.Nature,
                craftingStation: nameof(CraftingStations.None),
                requirements: "Stone,10",
                clipEverything: true,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Ore
            )
        },
        {
            "MineRock_Tin",
            new PrefabConfig(
                name: "MineRock_Tin",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.Nature,
                craftingStation: nameof(CraftingStations.None),
                requirements: "TinOre,4",
                clipEverything: true,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Ore
            )
        },
        {
            "MountainGraveStone01",
            new PrefabConfig(
                name: "MountainGraveStone01",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "Stone,5",
                clipEverything: true,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "MountainKit_brazier",
            new PrefabConfig(
                name: "MountainKit_brazier",
                enabled: true,
                allowedInDungeons: false,
                category: HammerCategories.Furniture,
                craftingStation: nameof(CraftingStations.Forge),
                requirements: "Bronze,5;Coal,2;BlackCore,1;WolfClaw,3",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceName: "Standing brazier (everburning)",
                pieceDesc: "Burns eternally without fuel.",
                pieceGroup: PieceClassification.Brazier,
                playerBasePatch: true
            )
        },
        {
            "MountainKit_brazier_blue",
            new PrefabConfig(
                name: "MountainKit_brazier_blue",
                enabled: true,
                allowedInDungeons: false,
                category: HammerCategories.Furniture,
                craftingStation: nameof(CraftingStations.Forge),
                requirements: "Bronze,5;GreydwarfEye,2;BlackCore,1;WolfClaw,3",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceName: "Blue standing brazier (everburning)",
                pieceDesc: "Burns eternally without fuel.",
                pieceGroup: PieceClassification.Brazier,
                playerBasePatch: true
            )
        },
        {
            "MountainKit_brazier_purple",
            new PrefabConfig(
                name: "MountainKit_brazier_purple",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "MountainKit_wood_gate",
            new PrefabConfig(
                name: "MountainKit_wood_gate",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.Building,
                craftingStation: nameof(CraftingStations.Forge),
                requirements: "Wood,20;Iron,4",
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "Oak1",
            new PrefabConfig(
                name: "Oak1",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.Nature,
                craftingStation: nameof(CraftingStations.None),
                requirements: "Wood,20;FineWood,20",
                clipEverything: true,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Flora
            )
        },
        {
            "OakStub",
            new PrefabConfig(
                name: "OakStub",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.Nature,
                craftingStation: nameof(CraftingStations.None),
                requirements: "Wood,5;FineWood,5",
                clipEverything: true,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Flora
            )
        },
        {
            "Oak_log",
            new PrefabConfig(
                name: "Oak_log",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.Nature,
                craftingStation: nameof(CraftingStations.None),
                requirements: "Wood,10;FineWood,10",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Flora
            )
        },
        {
            "Oak_log_half",
            new PrefabConfig(
                name: "Oak_log_half",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.Nature,
                craftingStation: nameof(CraftingStations.None),
                requirements: "Wood,5;FineWood,5",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Flora
            )
        },
        {
            "Pickable_Ashstone",
            new PrefabConfig(
                name: "Pickable_Ashstone",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "Pickable_Barley",
            new PrefabConfig(
                name: "Pickable_Barley",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.Nature,
                craftingStation: nameof(CraftingStations.None),
                requirements: "Barley,1",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.VanillaCrop
            )
        },
        {
            "Pickable_BlackCoreStand",
            new PrefabConfig(
                name: "Pickable_BlackCoreStand",
                enabled: true,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.BlackForge),
                requirements: "Iron,2;BlackCore,1",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Treasure
            )
        },
        {
            "Pickable_BogIronOre",
            new PrefabConfig(
                name: "Pickable_BogIronOre",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "IronScrap,1",
                clipEverything: true,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Ore
            )
        },
        {
            "Pickable_Branch",
            new PrefabConfig(
                name: "Pickable_Branch",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.Nature,
                craftingStation: nameof(CraftingStations.None),
                requirements: "Wood,5",
                clipEverything: true,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Flora
            )
        },
        {
            "Pickable_Carrot",
            new PrefabConfig(
                name: "Pickable_Carrot",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.Nature,
                craftingStation: nameof(CraftingStations.None),
                requirements: "Carrot,1",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.VanillaCrop
            )
        },
        {
            "Pickable_Charredskull",
            new PrefabConfig(
                name: "Pickable_Charredskull",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "Pickable_Dandelion",
            new PrefabConfig(
                name: "Pickable_Dandelion",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.Nature,
                craftingStation: nameof(CraftingStations.None),
                requirements: "Dandelion,5",
                clipEverything: true,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Plant
            )
        },
        {
            "Pickable_DolmenTreasure",
            new PrefabConfig(
                name: "Pickable_DolmenTreasure",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "Coins,5",
                pieceGroup: PieceClassification.Treasure
            )
        },
        {
            "Pickable_DragonEgg",
            new PrefabConfig(
                name: "Pickable_DragonEgg",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "DragonEgg,1",
                clipEverything: true,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "Pickable_DvergrLantern",
            new PrefabConfig(
                name: "Pickable_DvergrLantern",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: true,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "Pickable_DvergrMineTreasure",
            new PrefabConfig(
                name: "Pickable_DvergrMineTreasure",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: true,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Treasure
            )
        },
        {
            "Pickable_DvergrStein",
            new PrefabConfig(
                name: "Pickable_DvergrStein",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: true,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Treasure
            )
        },
        {
            "Pickable_Fiddlehead",
            new PrefabConfig(
                name: "Pickable_Fiddlehead",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "Pickable_Fishingrod",
            new PrefabConfig(
                name: "Pickable_Fishingrod",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: true,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Treasure
            )
        },
        {
            "Pickable_Flax",
            new PrefabConfig(
                name: "Pickable_Flax",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.Nature,
                craftingStation: nameof(CraftingStations.None),
                requirements: "Flax,1",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.VanillaCrop
            )
        },
        {
            "Pickable_Flint",
            new PrefabConfig(
                name: "Pickable_Flint",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.Nature,
                craftingStation: nameof(CraftingStations.None),
                requirements: "Flint,5",
                clipEverything: true,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Flora
            )
        },
        {
            "Pickable_ForestCryptRemains01",
            new PrefabConfig(
                name: "Pickable_ForestCryptRemains01",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: true,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Treasure
            )
        },
        {
            "Pickable_ForestCryptRemains02",
            new PrefabConfig(
                name: "Pickable_ForestCryptRemains02",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: true,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Treasure
            )
        },
        {
            "Pickable_ForestCryptRemains03",
            new PrefabConfig(
                name: "Pickable_ForestCryptRemains03",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: true,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Treasure
            )
        },
        {
            "Pickable_ForestCryptRemains04",
            new PrefabConfig(
                name: "Pickable_ForestCryptRemains04",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: true,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Treasure
            )
        },
        {
            "Pickable_Hairstrands01",
            new PrefabConfig(
                name: "Pickable_Hairstrands01",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Treasure
            )
        },
        {
            "Pickable_Hairstrands02",
            new PrefabConfig(
                name: "Pickable_Hairstrands02",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Treasure
            )
        },
        {
            "Pickable_Item",
            new PrefabConfig(
                name: "Pickable_Item",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                pieceName: "Coins (pickable)",
                pieceGroup: PieceClassification.Treasure
            )
        },
        {
            "Pickable_MeatPile",
            new PrefabConfig(
                name: "Pickable_MeatPile",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: true,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Treasure
            )
        },
        {
            "Pickable_Meteorite",
            new PrefabConfig(
                name: "Pickable_Meteorite",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.Nature,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: true,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Ore
            )
        },
        {
            "Pickable_MoltenCoreStand",
            new PrefabConfig(
                name: "Pickable_MoltenCoreStand",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "Pickable_MountainCaveCrystal",
            new PrefabConfig(
                name: "Pickable_MountainCaveCrystal",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.Nature,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Ore
            )
        },
        {
            "Pickable_MountainCaveObsidian",
            new PrefabConfig(
                name: "Pickable_MountainCaveObsidian",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.Nature,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Ore
            )
        },
        {
            "Pickable_MountainRemains01_buried",
            new PrefabConfig(
                name: "Pickable_MountainRemains01_buried",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: true,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Treasure
            )
        },
        {
            "Pickable_Mushroom",
            new PrefabConfig(
                name: "Pickable_Mushroom",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.Nature,
                craftingStation: nameof(CraftingStations.None),
                requirements: "Mushroom,5",
                clipEverything: true,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Plant
            )
        },
        {
            "Pickable_Mushroom_JotunPuffs",
            new PrefabConfig(
                name: "Pickable_Mushroom_JotunPuffs",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.Nature,
                craftingStation: nameof(CraftingStations.None),
                requirements: "MushroomJotunPuffs,1",
                clipEverything: true,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.VanillaCrop
            )
        },
        {
            "Pickable_Mushroom_Magecap",
            new PrefabConfig(
                name: "Pickable_Mushroom_Magecap",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.Nature,
                craftingStation: nameof(CraftingStations.None),
                requirements: "MushroomMagecap,1",
                clipEverything: true,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.VanillaCrop
            )
        },
        {
            "Pickable_Mushroom_blue",
            new PrefabConfig(
                name: "Pickable_Mushroom_blue",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.Nature,
                craftingStation: nameof(CraftingStations.None),
                requirements: "MushroomBlue,5",
                clipEverything: true,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Plant
            )
        },
        {
            "Pickable_Mushroom_yellow",
            new PrefabConfig(
                name: "Pickable_Mushroom_yellow",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.Nature,
                craftingStation: nameof(CraftingStations.None),
                requirements: "MushroomYellow,5",
                clipEverything: true,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Plant
            )
        },
        {
            "Pickable_Obsidian",
            new PrefabConfig(
                name: "Pickable_Obsidian",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.Nature,
                craftingStation: nameof(CraftingStations.None),
                requirements: "Obsidian,1",
                clipEverything: true,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Ore
            )
        },
        {
            "Pickable_Onion",
            new PrefabConfig(
                name: "Pickable_Onion",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.Nature,
                craftingStation: nameof(CraftingStations.None),
                requirements: "Obsidian,1",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.VanillaCrop
            )
        },
        {
            "Pickable_Pot_Shard",
            new PrefabConfig(
                name: "Pickable_Pot_Shard",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "Pickable_RoyalJelly",
            new PrefabConfig(
                name: "Pickable_RoyalJelly",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "Pickable_SeedCarrot",
            new PrefabConfig(
                name: "Pickable_SeedCarrot",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.Nature,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: true,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.VanillaCrop
            )
        },
        {
            "Pickable_SeedOnion",
            new PrefabConfig(
                name: "Pickable_SeedOnion",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.Nature,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: true,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.VanillaCrop
            )
        },
        {
            "Pickable_SeedTurnip",
            new PrefabConfig(
                name: "Pickable_SeedTurnip",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.Nature,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: true,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.VanillaCrop
            )
        },
        {
            "Pickable_SmokePuff",
            new PrefabConfig(
                name: "Pickable_SmokePuff",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "Pickable_Stone",
            new PrefabConfig(
                name: "Pickable_Stone",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.Nature,
                craftingStation: nameof(CraftingStations.None),
                requirements: "Stone,5",
                clipEverything: true,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Flora
            )
        },
        {
            "Pickable_SulfurRock",
            new PrefabConfig(
                name: "Pickable_SulfurRock",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "Pickable_SurtlingCoreStand",
            new PrefabConfig(
                name: "Pickable_SurtlingCoreStand",
                enabled: true,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.Forge),
                requirements: "Iron,2;SurtlingCore,1",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Treasure
            )
        },
        {
            "Pickable_Swordpiece1",
            new PrefabConfig(
                name: "Pickable_Swordpiece1",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "Pickable_Swordpiece2",
            new PrefabConfig(
                name: "Pickable_Swordpiece2",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "Pickable_Swordpiece3",
            new PrefabConfig(
                name: "Pickable_Swordpiece3",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "Pickable_Tar",
            new PrefabConfig(
                name: "Pickable_Tar",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.Nature,
                craftingStation: nameof(CraftingStations.None),
                requirements: "Tar,1",
                clipEverything: true,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Flora
            )
        },
        {
            "Pickable_TarBig",
            new PrefabConfig(
                name: "Pickable_TarBig",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.Nature,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: true,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Flora
            )
        },
        {
            "Pickable_Thistle",
            new PrefabConfig(
                name: "Pickable_Thistle",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.Nature,
                craftingStation: nameof(CraftingStations.None),
                requirements: "Thistle,5",
                clipEverything: true,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Plant
            )
        },
        {
            "Pickable_Tin",
            new PrefabConfig(
                name: "Pickable_Tin",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.Nature,
                craftingStation: nameof(CraftingStations.None),
                requirements: "TinOre,1",
                clipEverything: true,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Ore
            )
        },
        {
            "Pickable_Turnip",
            new PrefabConfig(
                name: "Pickable_Turnip",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.Nature,
                craftingStation: nameof(CraftingStations.None),
                requirements: "Turnip,1",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.VanillaCrop
            )
        },
        {
            "Pickable_VoltureEgg",
            new PrefabConfig(
                name: "Pickable_VoltureEgg",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "PineTree",
            new PrefabConfig(
                name: "PineTree",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.Nature,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: true,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Flora
            )
        },
        {
            "PineTree_log",
            new PrefabConfig(
                name: "PineTree_log",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.Nature,
                craftingStation: nameof(CraftingStations.None),
                requirements: "Wood,10;RoundLog,10",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Flora
            )
        },
        {
            "PineTree_log_half",
            new PrefabConfig(
                name: "PineTree_log_half",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.Nature,
                craftingStation: nameof(CraftingStations.None),
                requirements: "Wood,5;RoundLog,5",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Flora
            )
        },
        {
            "Pinetree_01",
            new PrefabConfig(
                name: "Pinetree_01",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.Nature,
                craftingStation: nameof(CraftingStations.None),
                requirements: "Wood,10;RoundLog,10",
                clipEverything: true,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Flora
            )
        },
        {
            "Pinetree_01_Stub",
            new PrefabConfig(
                name: "Pinetree_01_Stub",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.Nature,
                craftingStation: nameof(CraftingStations.None),
                requirements: "Wood,5;RoundLog,5",
                clipEverything: true,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Flora
            )
        },
        {
            "RaspberryBush",
            new PrefabConfig(
                name: "RaspberryBush",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.Nature,
                craftingStation: nameof(CraftingStations.None),
                requirements: "Raspberry,5",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Plant
            )
        },
        {
            "RockDolmen_1",
            new PrefabConfig(
                name: "RockDolmen_1",
                enabled: true,
                allowedInDungeons: false,
                category: HammerCategories.Nature,
                craftingStation: nameof(CraftingStations.None),
                requirements: "Stone,75",
                clipEverything: true,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Rock
            )
        },
        {
            "RockDolmen_2",
            new PrefabConfig(
                name: "RockDolmen_2",
                enabled: true,
                allowedInDungeons: false,
                category: HammerCategories.Nature,
                craftingStation: nameof(CraftingStations.None),
                requirements: "Stone,50",
                clipEverything: true,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Rock
            )
        },
        {
            "RockDolmen_3",
            new PrefabConfig(
                name: "RockDolmen_3",
                enabled: true,
                allowedInDungeons: false,
                category: HammerCategories.Nature,
                craftingStation: nameof(CraftingStations.None),
                requirements: "Stone,75",
                clipEverything: true,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Rock
            )
        },
        {
            "RockFinger",
            new PrefabConfig(
                name: "RockFinger",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "Stone,350",
                clipEverything: true
            )
        },
        {
            "RockFingerBroken",
            new PrefabConfig(
                name: "RockFingerBroken",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "Stone,175",
                clipEverything: true
            )
        },
        {
            "RockFingerBroken_frac",
            new PrefabConfig(
                name: "RockFingerBroken_frac",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "Stone,175",
                clipEverything: true,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "RockFinger_frac",
            new PrefabConfig(
                name: "RockFinger_frac",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "Stone,350",
                clipEverything: true,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "RockThumb",
            new PrefabConfig(
                name: "RockThumb",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "Stone,200",
                clipEverything: true
            )
        },
        {
            "RockThumb_frac",
            new PrefabConfig(
                name: "RockThumb_frac",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "Stone,200",
                clipEverything: true,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "Rock_3",
            new PrefabConfig(
                name: "Rock_3",
                enabled: true,
                allowedInDungeons: false,
                category: HammerCategories.Nature,
                craftingStation: nameof(CraftingStations.None),
                requirements: "Stone,24",
                clipEverything: true,
                pieceGroup: PieceClassification.Rock
            )
        },
        {
            "Rock_3_frac",
            new PrefabConfig(
                name: "Rock_3_frac",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "Stone,24",
                clipEverything: true,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Rock
            )
        },
        {
            "Rock_4",
            new PrefabConfig(
                name: "Rock_4",
                enabled: true,
                allowedInDungeons: false,
                category: HammerCategories.Nature,
                craftingStation: nameof(CraftingStations.None),
                requirements: "Stone,16",
                clipEverything: true,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Rock
            )
        },
        {
            "Rock_4_plains",
            new PrefabConfig(
                name: "Rock_4_plains",
                enabled: true,
                allowedInDungeons: false,
                category: HammerCategories.Nature,
                craftingStation: nameof(CraftingStations.None),
                requirements: "Stone,24",
                clipEverything: true,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Rock
            )
        },
        {
            "Rock_7",
            new PrefabConfig(
                name: "Rock_7",
                enabled: true,
                allowedInDungeons: false,
                category: HammerCategories.Nature,
                craftingStation: nameof(CraftingStations.None),
                requirements: "Stone,32",
                clipEverything: true,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Rock
            )
        },
        {
            "Rock_destructible",
            new PrefabConfig(
                name: "Rock_destructible",
                enabled: true,
                allowedInDungeons: false,
                category: HammerCategories.Nature,
                craftingStation: nameof(CraftingStations.None),
                requirements: "Stone,24",
                clipEverything: true,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Rock
            )
        },
        {
            "Rock_destructible_test",
            new PrefabConfig(
                name: "Rock_destructible_test",
                enabled: true,
                allowedInDungeons: false,
                category: HammerCategories.Nature,
                craftingStation: nameof(CraftingStations.None),
                requirements: "Stone,24",
                clipEverything: true,
                clipGround: false,
                pieceGroup: PieceClassification.Rock
            )
        },
        {
            "SeekerEgg",
            new PrefabConfig(
                name: "SeekerEgg",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "SeekerEgg_alwayshatch",
            new PrefabConfig(
                name: "SeekerEgg_alwayshatch",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "SeekerQueen_spithit",
            new PrefabConfig(
                name: "SeekerQueen_spithit",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "ShootStump",
            new PrefabConfig(
                name: "ShootStump",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.Nature,
                craftingStation: nameof(CraftingStations.None),
                requirements: "YggdrasilWood,10",
                clipEverything: true,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Flora
            )
        },
        {
            "Skull1",
            new PrefabConfig(
                name: "Skull1",
                enabled: true,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "TrophySkeleton,1",
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "Skull2",
            new PrefabConfig(
                name: "Skull2",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "StatueCorgi",
            new PrefabConfig(
                name: "StatueCorgi",
                enabled: true,
                allowedInDungeons: false,
                category: HammerCategories.Furniture,
                craftingStation: nameof(CraftingStations.Stonecutter),
                requirements: "Stone,24",
                clipEverything: true,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Statue
            )
        },
        {
            "StatueDeer",
            new PrefabConfig(
                name: "StatueDeer",
                enabled: true,
                allowedInDungeons: false,
                category: HammerCategories.Furniture,
                craftingStation: nameof(CraftingStations.Stonecutter),
                requirements: "Stone,24",
                clipEverything: true,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Statue
            )
        },
        {
            "StatueEvil",
            new PrefabConfig(
                name: "StatueEvil",
                enabled: true,
                allowedInDungeons: false,
                category: HammerCategories.Furniture,
                craftingStation: nameof(CraftingStations.Stonecutter),
                requirements: "Stone,32",
                clipEverything: true,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Statue
            )
        },
        {
            "StatueHare",
            new PrefabConfig(
                name: "StatueHare",
                enabled: true,
                allowedInDungeons: false,
                category: HammerCategories.Furniture,
                craftingStation: nameof(CraftingStations.Stonecutter),
                requirements: "Stone,24",
                clipEverything: true,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Statue
            )
        },
        {
            "StatueSeed",
            new PrefabConfig(
                name: "StatueSeed",
                enabled: true,
                allowedInDungeons: false,
                category: HammerCategories.Furniture,
                craftingStation: nameof(CraftingStations.Stonecutter),
                requirements: "Stone,16",
                clipEverything: true,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Statue
            )
        },
        {
            "SwampTree1",
            new PrefabConfig(
                name: "SwampTree1",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.Nature,
                craftingStation: nameof(CraftingStations.None),
                requirements: "ElderBark,10",
                clipEverything: true,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Flora
            )
        },
        {
            "SwampTree1_Stub",
            new PrefabConfig(
                name: "SwampTree1_Stub",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.Nature,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: true,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Flora
            )
        },
        {
            "SwampTree1_log",
            new PrefabConfig(
                name: "SwampTree1_log",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.Nature,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: true,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Flora
            )
        },
        {
            "SwampTree2",
            new PrefabConfig(
                name: "SwampTree2",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.Nature,
                craftingStation: nameof(CraftingStations.None),
                requirements: "ElderBark,32",
                clipEverything: true,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Flora
            )
        },
        {
            "SwampTree2_darkland",
            new PrefabConfig(
                name: "SwampTree2_darkland",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.Nature,
                craftingStation: nameof(CraftingStations.None),
                requirements: "ElderBark,32",
                clipEverything: true,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Flora
            )
        },
        {
            "SwampTree2_log",
            new PrefabConfig(
                name: "SwampTree2_log",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.Nature,
                craftingStation: nameof(CraftingStations.None),
                requirements: "ElderBark,32",
                clipEverything: true,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Flora
            )
        },
        {
            "TESTTOWER",
            new PrefabConfig(
                name: "TESTTOWER",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: true,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "Trailership",
            new PrefabConfig(
                name: "Trailership",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.Misc,
                craftingStation: nameof(CraftingStations.Workbench),
                requirements: "IronNails,100;DeerHide,10;FineWood,40;ElderBark,40",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceName: "Trader ship",
                pieceGroup: PieceClassification.Ship
                //invWidth: 6,
                //invHeight: 4
            )
        },
        {
            "TreasureChest_ashland_stone",
            new PrefabConfig(
                name: "TreasureChest_ashland_stone",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Chest,
                invWidth: 5,
                invHeight: 2
            )
        },
        {
            "TreasureChest_charredfortress",
            new PrefabConfig(
                name: "TreasureChest_charredfortress",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Chest,
                invWidth: 8,
                invHeight: 4
            )
        },
        {
            "TreasureChest_dvergr_loose_stone",
            new PrefabConfig(
                name: "TreasureChest_dvergr_loose_stone",
                enabled: true,
                allowedInDungeons: false,
                category: HammerCategories.Furniture,
                craftingStation: nameof(CraftingStations.Stonecutter),
                requirements: "BlackMarble,10",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceName: "Black marble chest",
                pieceGroup: PieceClassification.Chest,
                invWidth: 8,
                invHeight: 4
            )
        },
        {
            "TreasureChest_dvergrtower",
            new PrefabConfig(
                name: "TreasureChest_dvergrtower",
                enabled: true,
                allowedInDungeons: false,
                category: HammerCategories.Furniture,
                craftingStation: nameof(CraftingStations.BlackForge),
                requirements: "YggdrasilWood,10;Copper,2",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceName: "Dvergr chest",
                pieceGroup: PieceClassification.Chest,
                invWidth: 7,
                invHeight: 4
            )
        },
        {
            "TreasureChest_dvergrtown",
            new PrefabConfig(
                name: "TreasureChest_dvergrtown",
                enabled: true,
                allowedInDungeons: false,
                category: HammerCategories.Furniture,
                craftingStation: nameof(CraftingStations.BlackForge),
                requirements: "YggdrasilWood,10;Copper,6",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceName: "Dvergr chest (large)",
                pieceGroup: PieceClassification.Chest,
                invWidth: 8,
                invHeight: 4
            )
        },
        {
            "TreasureChest_fCrypt",
            new PrefabConfig(
                name: "TreasureChest_fCrypt",
                enabled: true,
                allowedInDungeons: false,
                category: HammerCategories.Furniture,
                craftingStation: nameof(CraftingStations.Stonecutter),
                requirements: "Stone,10;RoundLog,1",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceName: "Stone chest (mossy)",
                pieceGroup: PieceClassification.Chest,
                invWidth: 5,
                invHeight: 2
            )
        },
        {
            "TreasureChest_mountaincave",
            new PrefabConfig(
                name: "TreasureChest_mountaincave",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.Furniture,
                craftingStation: nameof(CraftingStations.Stonecutter),
                requirements: "Stone,10;Crystal,1",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceName: "Stone chest (snow)",
                pieceGroup: PieceClassification.Chest,
                invWidth: 6,
                invHeight: 3
            )
        },
        {
            "TreasureChest_sunkencrypt",
            new PrefabConfig(
                name: "TreasureChest_sunkencrypt",
                enabled: true,
                allowedInDungeons: false,
                category: HammerCategories.Furniture,
                craftingStation: nameof(CraftingStations.Stonecutter),
                requirements: "Stone,10;ElderBark,1",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceName: "Stone chest (dark moss)",
                pieceGroup: PieceClassification.Chest,
                invWidth: 5,
                invHeight: 2
            )
        },
        {
            "TreasureChest_trollcave",
            new PrefabConfig(
                name: "TreasureChest_trollcave",
                enabled: true,
                allowedInDungeons: false,
                category: HammerCategories.Furniture,
                craftingStation: nameof(CraftingStations.Stonecutter),
                requirements: "Stone,10;RoundLog,2",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceName: "Stone chest (mossy, big)",
                pieceGroup: PieceClassification.Chest,
                invWidth: 6,
                invHeight: 3
            )
        },
        {
            "UnstableLavaRock",
            new PrefabConfig(
                name: "UnstableLavaRock",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "VineAsh",
            new PrefabConfig(
                name: "VineAsh",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "VineGreen",
            new PrefabConfig(
                name: "VineGreen",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "YggaShoot1",
            new PrefabConfig(
                name: "YggaShoot1",
                enabled: true,
                allowedInDungeons: false,
                category: HammerCategories.Nature,
                craftingStation: nameof(CraftingStations.None),
                requirements: "YggdrasilWood,16",
                clipEverything: true,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Flora
            )
        },
        {
            "YggaShoot2",
            new PrefabConfig(
                name: "YggaShoot2",
                enabled: true,
                allowedInDungeons: false,
                category: HammerCategories.Nature,
                craftingStation: nameof(CraftingStations.None),
                requirements: "YggdrasilWood,16",
                clipEverything: true,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Flora
            )
        },
        {
            "YggaShoot3",
            new PrefabConfig(
                name: "YggaShoot3",
                enabled: true,
                allowedInDungeons: false,
                category: HammerCategories.Nature,
                craftingStation: nameof(CraftingStations.None),
                requirements: "YggdrasilWood,16",
                clipEverything: true,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Flora
            )
        },
        {
            "YggaShoot_small1",
            new PrefabConfig(
                name: "YggaShoot_small1",
                enabled: true,
                allowedInDungeons: false,
                category: HammerCategories.Nature,
                craftingStation: nameof(CraftingStations.None),
                requirements: "YggdrasilWood,10",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Flora
            )
        },
        {
            "YggdrasilRoot",
            new PrefabConfig(
                name: "YggdrasilRoot",
                enabled: true,
                allowedInDungeons: false,
                category: HammerCategories.Nature,
                craftingStation: nameof(CraftingStations.None),
                requirements: "YggdrasilWood,64;Sap,10",
                clipEverything: true,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Flora
            )
        },
        {
            "ancient_skull",
            new PrefabConfig(
                name: "ancient_skull",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "BlackMarble,100",
                clipEverything: false,
                clipGround: false,
                placementPatch: true,
                spawnOnDestroyed: "sfx_rock_destroyed"
            )
        },
        {
            "ashland_pot1_green",
            new PrefabConfig(
                name: "ashland_pot1_green",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "ashland_pot1_red",
            new PrefabConfig(
                name: "ashland_pot1_red",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "ashland_pot2_green",
            new PrefabConfig(
                name: "ashland_pot2_green",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "ashland_pot2_red",
            new PrefabConfig(
                name: "ashland_pot2_red",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "ashland_pot3_green",
            new PrefabConfig(
                name: "ashland_pot3_green",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "ashland_pot3_red",
            new PrefabConfig(
                name: "ashland_pot3_red",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "ashwood_arch_bottom",
            new PrefabConfig(
                name: "ashwood_arch_bottom",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.Workbench),
                requirements: "Blackwood,1",
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "ashwood_arch_top",
            new PrefabConfig(
                name: "ashwood_arch_top",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.Workbench),
                requirements: "Blackwood,1",
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "ashwood_wall_beam_26_alt",
            new PrefabConfig(
                name: "ashwood_wall_beam_26_alt",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "ashwood_wall_beam_45_alt",
            new PrefabConfig(
                name: "ashwood_wall_beam_45_alt",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "ashwood_wall_cross_26_alt",
            new PrefabConfig(
                name: "ashwood_wall_cross_26_alt",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "ashwood_wall_cross_45_alt",
            new PrefabConfig(
                name: "ashwood_wall_cross_45_alt",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "asksvin_carrion",
            new PrefabConfig(
                name: "asksvin_carrion",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "asksvin_carrion2",
            new PrefabConfig(
                name: "asksvin_carrion2",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "barrell",
            new PrefabConfig(
                name: "barrell",
                enabled: true,
                allowedInDungeons: false,
                category: HammerCategories.Misc,
                craftingStation: nameof(CraftingStations.Workbench),
                requirements: "FineWood,2",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceName: "Barrel",
                pieceDesc: "",
                pieceGroup: PieceClassification.Misc
            )
        },
        {
            "beech_log",
            new PrefabConfig(
                name: "beech_log",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.Nature,
                craftingStation: nameof(CraftingStations.None),
                requirements: "BeechSeeds,1,Wood,20",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Flora
            )
        },
        {
            "beech_log_half",
            new PrefabConfig(
                name: "beech_log_half",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.Nature,
                craftingStation: nameof(CraftingStations.None),
                requirements: "BeechSeeds,1,Wood,10",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Flora
            )
        },
        {
            "blackmarble_2x2_enforced",
            new PrefabConfig(
                name: "blackmarble_2x2_enforced",
                enabled: true,
                allowedInDungeons: false,
                category: HammerCategories.Stonecutter,
                craftingStation: nameof(CraftingStations.BlackForge),
                requirements: "BlackMarble,8;Copper,2",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.BlackMarble
            )
        },
        {
            "blackmarble_2x2x1",
            new PrefabConfig(
                name: "blackmarble_2x2x1",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.Stonecutter,
                craftingStation: nameof(CraftingStations.Stonecutter),
                requirements: "BlackMarble,4",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.BlackMarble
            )
        },
        {
            "blackmarble_altar_crystal",
            new PrefabConfig(
                name: "blackmarble_altar_crystal",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "blackmarble_altar_crystal_broken",
            new PrefabConfig(
                name: "blackmarble_altar_crystal_broken",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "blackmarble_base_2",
            new PrefabConfig(
                name: "blackmarble_base_2",
                enabled: true,
                allowedInDungeons: false,
                category: HammerCategories.Stonecutter,
                craftingStation: nameof(CraftingStations.Stonecutter),
                requirements: "BlackMarble,6",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceName: "Black marble plinth (wide)",
                pieceGroup: PieceClassification.BlackMarble
            )
        },
        {
            "blackmarble_column_3",
            new PrefabConfig(
                name: "blackmarble_column_3",
                enabled: true,
                allowedInDungeons: false,
                category: HammerCategories.Stonecutter,
                craftingStation: nameof(CraftingStations.Stonecutter),
                requirements: "BlackMarble,16",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.BlackMarble
            )
        },
        {
            "blackmarble_creep_4x1x1",
            new PrefabConfig(
                name: "blackmarble_creep_4x1x1",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.Stonecutter,
                craftingStation: nameof(CraftingStations.Stonecutter),
                requirements: "BlackMarble,4",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.BlackMarble
            )
        },
        {
            "blackmarble_creep_4x2x1",
            new PrefabConfig(
                name: "blackmarble_creep_4x2x1",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.Stonecutter,
                craftingStation: nameof(CraftingStations.Stonecutter),
                requirements: "BlackMarble,8",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.BlackMarble
            )
        },
        {
            "blackmarble_creep_slope_inverted_1x1x2",
            new PrefabConfig(
                name: "blackmarble_creep_slope_inverted_1x1x2",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.Stonecutter,
                craftingStation: nameof(CraftingStations.Stonecutter),
                requirements: "BlackMarble,2",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.BlackMarble
            )
        },
        {
            "blackmarble_creep_slope_inverted_2x2x1",
            new PrefabConfig(
                name: "blackmarble_creep_slope_inverted_2x2x1",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.Stonecutter,
                craftingStation: nameof(CraftingStations.Stonecutter),
                requirements: "BlackMarble,4",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.BlackMarble
            )
        },
        {
            "blackmarble_creep_stair",
            new PrefabConfig(
                name: "blackmarble_creep_stair",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.Stonecutter,
                craftingStation: nameof(CraftingStations.Stonecutter),
                requirements: "BlackMarble,8",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.BlackMarble
            )
        },
        {
            "blackmarble_floor_large",
            new PrefabConfig(
                name: "blackmarble_floor_large",
                enabled: true,
                allowedInDungeons: false,
                category: HammerCategories.Stonecutter,
                craftingStation: nameof(CraftingStations.Stonecutter),
                requirements: "BlackMarble,32",
                clipEverything: false,
                clipGround: true,
                placementPatch: false,
                pieceName: "Black marble floor 8x8",
                pieceGroup: PieceClassification.BlackMarble
            )
        },
        {
            "blackmarble_head01",
            new PrefabConfig(
                name: "blackmarble_head01",
                enabled: true,
                allowedInDungeons: false,
                category: HammerCategories.Stonecutter,
                craftingStation: nameof(CraftingStations.Stonecutter),
                requirements: "BlackMarble,2",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.BlackMarble
            )
        },
        {
            "blackmarble_head02",
            new PrefabConfig(
                name: "blackmarble_head02",
                enabled: true,
                allowedInDungeons: false,
                category: HammerCategories.Stonecutter,
                craftingStation: nameof(CraftingStations.Stonecutter),
                requirements: "BlackMarble,2",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.BlackMarble
            )
        },
        {
            "blackmarble_head_big01",
            new PrefabConfig(
                name: "blackmarble_head_big01",
                enabled: true,
                allowedInDungeons: false,
                category: HammerCategories.Stonecutter,
                craftingStation: nameof(CraftingStations.Stonecutter),
                requirements: "BlackMarble,6",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.BlackMarble
            )
        },
        {
            "blackmarble_head_big02",
            new PrefabConfig(
                name: "blackmarble_head_big02",
                enabled: true,
                allowedInDungeons: false,
                category: HammerCategories.Stonecutter,
                craftingStation: nameof(CraftingStations.Stonecutter),
                requirements: "BlackMarble,6",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.BlackMarble
            )
        },
        {
            "blackmarble_out_2",
            new PrefabConfig(
                name: "blackmarble_out_2",
                enabled: true,
                allowedInDungeons: false,
                category: HammerCategories.Stonecutter,
                craftingStation: nameof(CraftingStations.Stonecutter),
                requirements: "BlackMarble,6",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceName: "Black marble cornice (wide)",
                pieceGroup: PieceClassification.BlackMarble
            )
        },
        {
            "blackmarble_post01",
            new PrefabConfig(
                name: "blackmarble_post01",
                enabled: true,
                allowedInDungeons: false,
                category: HammerCategories.Stonecutter,
                craftingStation: nameof(CraftingStations.Stonecutter),
                requirements: "BlackMarble,8",
                clipEverything: false,
                clipGround: true,
                placementPatch: false,
                pieceGroup: PieceClassification.BlackMarble
            )
        },
        {
            "blackmarble_slope_1x2",
            new PrefabConfig(
                name: "blackmarble_slope_1x2",
                enabled: true,
                allowedInDungeons: false,
                category: HammerCategories.Stonecutter,
                craftingStation: nameof(CraftingStations.Stonecutter),
                requirements: "BlackMarble,2",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.BlackMarble
            )
        },
        {
            "blackmarble_slope_inverted_1x2",
            new PrefabConfig(
                name: "blackmarble_slope_inverted_1x2",
                enabled: true,
                allowedInDungeons: false,
                category: HammerCategories.Stonecutter,
                craftingStation: nameof(CraftingStations.Stonecutter),
                requirements: "BlackMarble,2",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.BlackMarble
            )
        },
        {
            "blackmarble_stair_corner",
            new PrefabConfig(
                name: "blackmarble_stair_corner",
                enabled: true,
                allowedInDungeons: false,
                category: HammerCategories.Stonecutter,
                craftingStation: nameof(CraftingStations.Stonecutter),
                requirements: "BlackMarble,8",
                clipEverything: false,
                clipGround: false,
                placementPatch: true,
                pieceGroup: PieceClassification.BlackMarble
            )
        },
        {
            "blackmarble_stair_corner_left",
            new PrefabConfig(
                name: "blackmarble_stair_corner_left",
                enabled: true,
                allowedInDungeons: false,
                category: HammerCategories.Stonecutter,
                craftingStation: nameof(CraftingStations.Stonecutter),
                requirements: "BlackMarble,8",
                clipEverything: false,
                clipGround: false,
                placementPatch: true,
                pieceGroup: PieceClassification.BlackMarble
            )
        },
        {
            "blackmarble_tile_floor_1x1",
            new PrefabConfig(
                name: "blackmarble_tile_floor_1x1",
                enabled: true,
                allowedInDungeons: false,
                category: HammerCategories.Stonecutter,
                craftingStation: nameof(CraftingStations.Stonecutter),
                requirements: "BlackMarble,2",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.BlackMarble
            )
        },
        {
            "blackmarble_tile_floor_2x2",
            new PrefabConfig(
                name: "blackmarble_tile_floor_2x2",
                enabled: true,
                allowedInDungeons: false,
                category: HammerCategories.Stonecutter,
                craftingStation: nameof(CraftingStations.Stonecutter),
                requirements: "BlackMarble,2",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.BlackMarble
            )
        },
        {
            "blackmarble_tile_wall_1x1",
            new PrefabConfig(
                name: "blackmarble_tile_wall_1x1",
                enabled: true,
                allowedInDungeons: false,
                category: HammerCategories.Stonecutter,
                craftingStation: nameof(CraftingStations.Stonecutter),
                requirements: "BlackMarble,1",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.BlackMarble
            )
        },
        {
            "blackmarble_tile_wall_2x2",
            new PrefabConfig(
                name: "blackmarble_tile_wall_2x2",
                enabled: true,
                allowedInDungeons: false,
                category: HammerCategories.Stonecutter,
                craftingStation: nameof(CraftingStations.Stonecutter),
                requirements: "BlackMarble,2",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.BlackMarble
            )
        },
        {
            "blackmarble_tile_wall_2x4",
            new PrefabConfig(
                name: "blackmarble_tile_wall_2x4",
                enabled: true,
                allowedInDungeons: false,
                category: HammerCategories.Stonecutter,
                craftingStation: nameof(CraftingStations.Stonecutter),
                requirements: "BlackMarble,4",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.BlackMarble
            )
        },
        {
            "bucket",
            new PrefabConfig(
                name: "bucket",
                enabled: true,
                allowedInDungeons: false,
                category: HammerCategories.Misc,
                craftingStation: nameof(CraftingStations.Workbench),
                requirements: "Wood,2",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Misc
            )
        },
        {
            "caverock_ice_pillar_wall",
            new PrefabConfig(
                name: "caverock_ice_pillar_wall",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.Nature,
                craftingStation: nameof(CraftingStations.None),
                requirements: "Crystal,10",
                clipEverything: true,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Ice
            )
        },
        {
            "caverock_ice_stalagmite",
            new PrefabConfig(
                name: "caverock_ice_stalagmite",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.Nature,
                craftingStation: nameof(CraftingStations.None),
                requirements: "Crystal,2",
                clipEverything: true,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Ice
            )
        },
        {
            "caverock_ice_stalagmite_broken",
            new PrefabConfig(
                name: "caverock_ice_stalagmite_broken",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.Nature,
                craftingStation: nameof(CraftingStations.None),
                requirements: "Crystal,4",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Ice
            )
        },
        {
            "caverock_ice_stalagtite",
            new PrefabConfig(
                name: "caverock_ice_stalagtite",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.Nature,
                craftingStation: nameof(CraftingStations.None),
                requirements: "Crystal,2",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Ice
            )
        },
        {
            "caverock_ice_stalagtite_falling",
            new PrefabConfig(
                name: "caverock_ice_stalagtite_falling",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.Nature,
                craftingStation: nameof(CraftingStations.None),
                requirements: "Crystal,2",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Ice
            )
        },
        {
            "charred_shieldgenerator",
            new PrefabConfig(
                name: "charred_shieldgenerator",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "cliff_ashlands1_frac",
            new PrefabConfig(
                name: "cliff_ashlands1_frac",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: true,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "cliff_ashlands2_frac",
            new PrefabConfig(
                name: "cliff_ashlands2_frac",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: false,
                placementPatch: true
            )
        },
        {
            "cliff_ashlands4_frac",
            new PrefabConfig(
                name: "cliff_ashlands4_frac",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: true,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "cliff_ashlands6_frac",
            new PrefabConfig(
                name: "cliff_ashlands6_frac",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: true,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "cliff_ashlands7_HalfArch_frac",
            new PrefabConfig(
                name: "cliff_ashlands7_HalfArch_frac",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: true,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "cliff_ashlands_Arch_frac",
            new PrefabConfig(
                name: "cliff_ashlands_Arch_frac",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: true,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "cliff_ashlandsflowrock_frac",
            new PrefabConfig(
                name: "cliff_ashlandsflowrock_frac",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: true,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "cliff_mistlands1",
            new PrefabConfig(
                name: "cliff_mistlands1",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.Nature,
                craftingStation: nameof(CraftingStations.None),
                requirements: "Stone,350",
                clipEverything: true,
                pieceGroup: PieceClassification.Rock
            )
        },
        {
            "cliff_mistlands1_creep",
            new PrefabConfig(
                name: "cliff_mistlands1_creep",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.Nature,
                craftingStation: nameof(CraftingStations.None),
                requirements: "Stone,350",
                clipEverything: true,
                pieceGroup: PieceClassification.Rock
            )
        },
        {
            "cliff_mistlands1_creep_frac",
            new PrefabConfig(
                name: "cliff_mistlands1_creep_frac",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "Stone,350",
                clipEverything: true,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Rock
            )
        },
        {
            "cliff_mistlands1_frac",
            new PrefabConfig(
                name: "cliff_mistlands1_frac",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "Stone,350",
                clipEverything: true,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Rock
            )
        },
        {
            "cliff_mistlands2",
            new PrefabConfig(
                name: "cliff_mistlands2",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.Nature,
                craftingStation: nameof(CraftingStations.None),
                requirements: "Stone,175",
                clipEverything: true,
                pieceGroup: PieceClassification.Rock
            )
        },
        {
            "cliff_mistlands2_frac",
            new PrefabConfig(
                name: "cliff_mistlands2_frac",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "Stone,175",
                clipEverything: true,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Rock
            )
        },
        {
            "cloth_hanging_door",
            new PrefabConfig(
                name: "cloth_hanging_door",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.Furniture,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Banner
            )
        },
        {
            "cloth_hanging_door_double",
            new PrefabConfig(
                name: "cloth_hanging_door_double",
                enabled: true,
                allowedInDungeons: false,
                category: HammerCategories.Furniture,
                craftingStation: nameof(CraftingStations.Workbench),
                requirements: "JuteRed,4",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Banner
            )
        },
        {
            "cloth_hanging_long",
            new PrefabConfig(
                name: "cloth_hanging_long",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.Furniture,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Banner
            )
        },
        {
            "crypt_skeleton_chest",
            new PrefabConfig(
                name: "crypt_skeleton_chest",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "dead_deer",
            new PrefabConfig(
                name: "dead_deer",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: true,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "dungeon_forestcrypt_door",
            new PrefabConfig(
                name: "dungeon_forestcrypt_door",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "dungeon_queen_door",
            new PrefabConfig(
                name: "dungeon_queen_door",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "BlackMarble,40;DvergrKeyFragment,4;Iron,12",
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "dungeon_sunkencrypt_irongate",
            new PrefabConfig(
                name: "dungeon_sunkencrypt_irongate",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.Building,
                craftingStation: nameof(CraftingStations.None),
                requirements: "Iron,4",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Iron
            )
        },
        {
            "dungeon_sunkencrypt_irongate_rusty",
            new PrefabConfig(
                name: "dungeon_sunkencrypt_irongate_rusty",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.Building,
                craftingStation: nameof(CraftingStations.Forge),
                requirements: "Iron,4",
                clipEverything: false,
                clipGround: true,
                placementPatch: false,
                pieceGroup: PieceClassification.Iron
            )
        },
        {
            "dverger_demister",
            new PrefabConfig(
                name: "dverger_demister",
                enabled: true,
                allowedInDungeons: false,
                category: HammerCategories.Furniture,
                craftingStation: nameof(CraftingStations.BlackForge),
                requirements: "Iron,1;Wisp,1",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Torch
            )
        },
        {
            "dverger_demister_broken",
            new PrefabConfig(
                name: "dverger_demister_broken",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Misc
            )
        },
        {
            "dverger_demister_large",
            new PrefabConfig(
                name: "dverger_demister_large",
                enabled: true,
                allowedInDungeons: false,
                category: HammerCategories.Furniture,
                craftingStation: nameof(CraftingStations.BlackForge),
                requirements: "Iron,1;Wisp,2",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Torch
            )
        },
        {
            "dverger_demister_ruins",
            new PrefabConfig(
                name: "dverger_demister_ruins",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Misc
            )
        },
        {
            "dverger_guardstone",
            new PrefabConfig(
                name: "dverger_guardstone",
                enabled: true,
                allowedInDungeons: false,
                category: HammerCategories.Misc,
                craftingStation: nameof(CraftingStations.BlackForge),
                requirements: "YggdrasilWood,5;BlackMarble,5;BlackCore,1",
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "dvergrprops_banner",
            new PrefabConfig(
                name: "dvergrprops_banner",
                enabled: true,
                allowedInDungeons: false,
                category: HammerCategories.Furniture,
                craftingStation: nameof(CraftingStations.Workbench),
                requirements: "JuteBlue,6;FineWood,2",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Banner
            )
        },
        {
            "dvergrprops_barrel",
            new PrefabConfig(
                name: "dvergrprops_barrel",
                enabled: true,
                allowedInDungeons: false,
                category: HammerCategories.Furniture,
                craftingStation: nameof(CraftingStations.Workbench),
                requirements: "YggdrasilWood,20;Bronze,5;Resin,10",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Dvergr
            )
        },
        {
            "dvergrprops_bed",
            new PrefabConfig(
                name: "dvergrprops_bed",
                enabled: true,
                allowedInDungeons: false,
                category: HammerCategories.Furniture,
                craftingStation: nameof(CraftingStations.BlackForge),
                requirements: "YggdrasilWood,8;ScaleHide,2;IronNails,5",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Bed,
                playerBasePatch: true
            )
        },
        {
            "dvergrprops_chair",
            new PrefabConfig(
                name: "dvergrprops_chair",
                enabled: true,
                allowedInDungeons: false,
                category: HammerCategories.Furniture,
                craftingStation: nameof(CraftingStations.BlackForge),
                requirements: "YggdrasilWood,4",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Chair
            )
        },
        {
            "dvergrprops_crate",
            new PrefabConfig(
                name: "dvergrprops_crate",
                enabled: true,
                allowedInDungeons: false,
                category: HammerCategories.Misc,
                craftingStation: nameof(CraftingStations.BlackForge),
                requirements: "YggdrasilWood,4",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Misc
            )
        },
        {
            "dvergrprops_crate_ashlands",
            new PrefabConfig(
                name: "dvergrprops_crate_ashlands",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "dvergrprops_crate_long",
            new PrefabConfig(
                name: "dvergrprops_crate_long",
                enabled: true,
                allowedInDungeons: false,
                category: HammerCategories.Misc,
                craftingStation: nameof(CraftingStations.BlackForge),
                requirements: "YggdrasilWood,4",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceName: "Dvergr component crate",
                pieceDesc: "",
                pieceGroup: PieceClassification.Misc
            )
        },
        {
            "dvergrprops_curtain",
            new PrefabConfig(
                name: "dvergrprops_curtain",
                enabled: true,
                allowedInDungeons: false,
                category: HammerCategories.Furniture,
                craftingStation: nameof(CraftingStations.Workbench),
                requirements: "JuteBlue,6;FineWood,2",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Banner
            )
        },
        {
            "dvergrprops_hooknchain",
            new PrefabConfig(
                name: "dvergrprops_hooknchain",
                enabled: true,
                allowedInDungeons: false,
                category: HammerCategories.Stonecutter,
                craftingStation: nameof(CraftingStations.BlackForge),
                requirements: "Chain,2;Iron,2",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceName: "Dvergr hook & chain",
                pieceGroup: PieceClassification.Misc
            )
        },
        {
            "dvergrprops_lantern_standing",
            new PrefabConfig(
                name: "dvergrprops_lantern_standing",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Dvergr
            )
        },
        {
            "dvergrprops_pickaxe",
            new PrefabConfig(
                name: "dvergrprops_pickaxe",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: true,
                clipGround: false,
                placementPatch: false,
                placementOffset: new Vector3(-1f, 0f, 0f),
                pieceGroup: PieceClassification.Dvergr
            )
        },
        {
            "dvergrprops_shelf",
            new PrefabConfig(
                name: "dvergrprops_shelf",
                enabled: true,
                allowedInDungeons: false,
                category: HammerCategories.Furniture,
                craftingStation: nameof(CraftingStations.BlackForge),
                requirements: "YggdrasilWood,4;IronNails,5",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Dvergr
            )
        },
        {
            "dvergrprops_stool",
            new PrefabConfig(
                name: "dvergrprops_stool",
                enabled: true,
                allowedInDungeons: false,
                category: HammerCategories.Furniture,
                craftingStation: nameof(CraftingStations.BlackForge),
                requirements: "YggdrasilWood,2",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Chair
            )
        },
        {
            "dvergrprops_table",
            new PrefabConfig(
                name: "dvergrprops_table",
                enabled: true,
                allowedInDungeons: false,
                category: HammerCategories.Furniture,
                craftingStation: nameof(CraftingStations.BlackForge),
                requirements: "YggdrasilWood,6;IronNails,10",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Table
            )
        },
        {
            "dvergrprops_wood_beam",
            new PrefabConfig(
                name: "dvergrprops_wood_beam",
                enabled: true,
                allowedInDungeons: false,
                category: HammerCategories.Building,
                craftingStation: nameof(CraftingStations.BlackForge),
                requirements: "YggdrasilWood,4",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Dvergr
            )
        },
        {
            "dvergrprops_wood_floor",
            new PrefabConfig(
                name: "dvergrprops_wood_floor",
                enabled: true,
                allowedInDungeons: false,
                category: HammerCategories.Building,
                craftingStation: nameof(CraftingStations.BlackForge),
                requirements: "YggdrasilWood,2",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Dvergr
            )
        },
        {
            "dvergrprops_wood_pole",
            new PrefabConfig(
                name: "dvergrprops_wood_pole",
                enabled: true,
                allowedInDungeons: false,
                category: HammerCategories.Building,
                craftingStation: nameof(CraftingStations.BlackForge),
                requirements: "YggdrasilWood,2",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceName: "Dvergr wood pole (large)",
                pieceGroup: PieceClassification.Dvergr
            )
        },
        {
            "dvergrprops_wood_stair",
            new PrefabConfig(
                name: "dvergrprops_wood_stair",
                enabled: true,
                allowedInDungeons: false,
                category: HammerCategories.Building,
                craftingStation: nameof(CraftingStations.BlackForge),
                requirements: "YggdrasilWood,2",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Dvergr
            )
        },
        {
            "dvergrprops_wood_stakewall",
            new PrefabConfig(
                name: "dvergrprops_wood_stakewall",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.Building,
                craftingStation: nameof(CraftingStations.None),
                requirements: "YggdrasilWood,4",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Dvergr
            )
        },
        {
            "dvergrprops_wood_wall",
            new PrefabConfig(
                name: "dvergrprops_wood_wall",
                enabled: true,
                allowedInDungeons: false,
                category: HammerCategories.Building,
                craftingStation: nameof(CraftingStations.BlackForge),
                requirements: "YggdrasilWood,10",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceName: "Dvergr wood wall 4x4",
                pieceGroup: PieceClassification.Dvergr
            )
        },
        {
            "dvergrtown_arch",
            new PrefabConfig(
                name: "dvergrtown_arch",
                enabled: true,
                allowedInDungeons: false,
                category: HammerCategories.Stonecutter,
                craftingStation: nameof(CraftingStations.Stonecutter),
                requirements: "BlackMarble,8",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Dvergr
            )
        },
        {
            "dvergrtown_creep_door",
            new PrefabConfig(
                name: "dvergrtown_creep_door",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.Nature,
                craftingStation: nameof(CraftingStations.None),
                requirements: "YggdrasilWood,4",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceName: "Door hanging (creep)",
                pieceDesc: "",
                pieceGroup: PieceClassification.Misc
            )
        },
        {
            "dvergrtown_secretdoor",
            new PrefabConfig(
                name: "dvergrtown_secretdoor",
                enabled: true,
                allowedInDungeons: false,
                category: HammerCategories.Building,
                craftingStation: nameof(CraftingStations.BlackForge),
                requirements: "BlackMarble,12;Eitr,2",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceName: "Dvergr secret door",
                pieceGroup: PieceClassification.Dvergr
            )
        },
        {
            "dvergrtown_slidingdoor",
            new PrefabConfig(
                name: "dvergrtown_slidingdoor",
                enabled: true,
                allowedInDungeons: false,
                category: HammerCategories.Building,
                craftingStation: nameof(CraftingStations.BlackForge),
                requirements: "BlackMarble,32;Copper,8",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceName: "Dvergr sliding door",
                pieceGroup: PieceClassification.Dvergr
            )
        },
        {
            "dvergrtown_stair_corner_wood_left",
            new PrefabConfig(
                name: "dvergrtown_stair_corner_wood_left",
                enabled: true,
                allowedInDungeons: false,
                category: HammerCategories.Building,
                craftingStation: nameof(CraftingStations.BlackForge),
                requirements: "YggdrasilWood,5;CopperScrap,2",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Dvergr
            )
        },
        {
            "dvergrtown_wood_beam",
            new PrefabConfig(
                name: "dvergrtown_wood_beam",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.Building,
                craftingStation: nameof(CraftingStations.BlackForge),
                requirements: "YggdrasilWood,4",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceName: "Dvergr wood beam (creep)",
                pieceGroup: PieceClassification.Dvergr
            )
        },
        {
            "dvergrtown_wood_crane",
            new PrefabConfig(
                name: "dvergrtown_wood_crane",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.Building,
                craftingStation: nameof(CraftingStations.BlackForge),
                requirements: "YggdrasilWood,8;Chain,2;Iron,2",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Dvergr
            )
        },
        {
            "dvergrtown_wood_pole",
            new PrefabConfig(
                name: "dvergrtown_wood_pole",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceName: "Dvergr wood pole (creep)",
                pieceGroup: PieceClassification.Dvergr
            )
        },
        {
            "dvergrtown_wood_stake",
            new PrefabConfig(
                name: "dvergrtown_wood_stake",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.Building,
                craftingStation: nameof(CraftingStations.BlackForge),
                requirements: "YggdrasilWood,1",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Dvergr
            )
        },
        {
            "dvergrtown_wood_stakewall",
            new PrefabConfig(
                name: "dvergrtown_wood_stakewall",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.Building,
                craftingStation: nameof(CraftingStations.BlackForge),
                requirements: "YggdrasilWood,4",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Dvergr
            )
        },
        {
            "dvergrtown_wood_support",
            new PrefabConfig(
                name: "dvergrtown_wood_support",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.Building,
                craftingStation: nameof(CraftingStations.BlackForge),
                requirements: "YggdrasilWood,10",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Dvergr
            )
        },
        {
            "dvergrtown_wood_wall01",
            new PrefabConfig(
                name: "dvergrtown_wood_wall01",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.Building,
                craftingStation: nameof(CraftingStations.BlackForge),
                requirements: "YggdrasilWood,20",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Dvergr
            )
        },
        {
            "dvergrtown_wood_wall02",
            new PrefabConfig(
                name: "dvergrtown_wood_wall02",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.Building,
                craftingStation: nameof(CraftingStations.BlackForge),
                requirements: "YggdrasilWood,12",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Dvergr
            )
        },
        {
            "dvergrtown_wood_wall03",
            new PrefabConfig(
                name: "dvergrtown_wood_wall03",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.Building,
                craftingStation: nameof(CraftingStations.BlackForge),
                requirements: "YggdrasilWood,10;IronNails,6",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Dvergr
            )
        },
        {
            "fader_bellholder",
            new PrefabConfig(
                name: "fader_bellholder",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "fenrirhide_hanging",
            new PrefabConfig(
                name: "fenrirhide_hanging",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "WolfHairBundle,2",
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "fenrirhide_hanging_door",
            new PrefabConfig(
                name: "fenrirhide_hanging_door",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "WolfHairBundle,2"
            )
        },
        {
            "fire_pit_haldor",
            new PrefabConfig(
                name: "fire_pit_haldor",
                enabled: true,
                allowedInDungeons: false,
                category: HammerCategories.Misc,
                craftingStation: nameof(CraftingStations.None),
                requirements: "Stone,5;Wood,2;BlackCore,1",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceName: "Campfire (everburning)",
                pieceDesc: "Burns eternally without fuel.",
                pieceGroup: PieceClassification.Fire
            )
        },
        {
            "fire_pit_hildir",
            new PrefabConfig(
                name: "fire_pit_hildir",
                enabled: true,
                allowedInDungeons: false,
                category: HammerCategories.Misc,
                craftingStation: nameof(CraftingStations.Forge),
                requirements: "Ironpit,1;Wood,1;BlackCore,1",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceName: "Firepit iron (everburning)",
                pieceDesc: "Burns eternally without fuel.",
                pieceGroup: PieceClassification.Fire
            )
        },
        {
            "flying_core",
            new PrefabConfig(
                name: "flying_core",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "Crystal,24",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                spawnOnDestroyed: "fx_crystal_destruction"
            )
        },
        {
            "fuling_trap",
            new PrefabConfig(
                name: "fuling_trap",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "giant_arm",
            new PrefabConfig(
                name: "giant_arm",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "BlackMarble,32",
                clipEverything: true
            )
        },
        {
            "giant_brain",
            new PrefabConfig(
                name: "giant_brain",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "Softtissue,64",
                clipEverything: true
            )
        },
        {
            "giant_brain_frac",
            new PrefabConfig(
                name: "giant_brain_frac",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "Softtissue,64",
                clipEverything: true,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "giant_helmet1",
            new PrefabConfig(
                name: "giant_helmet1",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "Iron,32",
                clipEverything: true
            )
        },
        {
            "giant_helmet1_destruction",
            new PrefabConfig(
                name: "giant_helmet1_destruction",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "Iron,24",
                clipEverything: true,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "giant_helmet2",
            new PrefabConfig(
                name: "giant_helmet2",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "Iron,32",
                clipEverything: true
            )
        },
        {
            "giant_helmet2_destruction",
            new PrefabConfig(
                name: "giant_helmet2_destruction",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "Iron,24",
                clipEverything: true,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "giant_ribs",
            new PrefabConfig(
                name: "giant_ribs",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "BlackMarble,64",
                clipEverything: true
            )
        },
        {
            "giant_ribs_frac",
            new PrefabConfig(
                name: "giant_ribs_frac",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "BlackMarble,64",
                clipEverything: true,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "giant_skull",
            new PrefabConfig(
                name: "giant_skull",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "BlackMarble,32",
                clipEverything: true
            )
        },
        {
            "giant_skull_frac",
            new PrefabConfig(
                name: "giant_skull_frac",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "BlackMarble,32",
                clipEverything: true,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "giant_sword1",
            new PrefabConfig(
                name: "giant_sword1",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "Iron,16",
                clipEverything: true
            )
        },
        {
            "giant_sword1_destruction",
            new PrefabConfig(
                name: "giant_sword1_destruction",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "Iron,16",
                clipEverything: true,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "giant_sword2",
            new PrefabConfig(
                name: "giant_sword2",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "Iron,16",
                clipEverything: true
            )
        },
        {
            "giant_sword2_destruction",
            new PrefabConfig(
                name: "giant_sword2_destruction",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "Iron,16",
                clipEverything: true,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "goblin_banner",
            new PrefabConfig(
                name: "goblin_banner",
                enabled: true,
                allowedInDungeons: false,
                category: HammerCategories.Furniture,
                craftingStation: nameof(CraftingStations.Workbench),
                requirements: "Wood,1;DeerHide,1;Tar,1",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Banner
            )
        },
        {
            "goblin_bed",
            new PrefabConfig(
                name: "goblin_bed",
                enabled: true,
                allowedInDungeons: false,
                category: HammerCategories.Furniture,
                craftingStation: nameof(CraftingStations.Workbench),
                requirements: "Wood,8;Tar,1",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Bed,
                playerBasePatch: true
            )
        },
        {
            "goblin_fence",
            new PrefabConfig(
                name: "goblin_fence",
                enabled: true,
                allowedInDungeons: false,
                category: HammerCategories.Building,
                craftingStation: nameof(CraftingStations.Workbench),
                requirements: "Wood,2;Tar,1",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Goblin
            )
        },
        {
            "goblin_pole",
            new PrefabConfig(
                name: "goblin_pole",
                enabled: true,
                allowedInDungeons: false,
                category: HammerCategories.Building,
                craftingStation: nameof(CraftingStations.Workbench),
                requirements: "Wood,1;Tar,1",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Goblin
            )
        },
        {
            "goblin_pole_small",
            new PrefabConfig(
                name: "goblin_pole_small",
                enabled: true,
                allowedInDungeons: false,
                category: HammerCategories.Building,
                craftingStation: nameof(CraftingStations.Workbench),
                requirements: "Wood,1;Tar,1",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Goblin
            )
        },
        {
            "goblin_roof_45d",
            new PrefabConfig(
                name: "goblin_roof_45d",
                enabled: true,
                allowedInDungeons: false,
                category: HammerCategories.Building,
                craftingStation: nameof(CraftingStations.Workbench),
                requirements: "Wood,1;DeerHide,1;Tar,1",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Goblin
            )
        },
        {
            "goblin_roof_45d_corner",
            new PrefabConfig(
                name: "goblin_roof_45d_corner",
                enabled: true,
                allowedInDungeons: false,
                category: HammerCategories.Building,
                craftingStation: nameof(CraftingStations.Workbench),
                requirements: "Wood,1;DeerHide,1;Tar,1",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Goblin
            )
        },
        {
            "goblin_roof_cap",
            new PrefabConfig(
                name: "goblin_roof_cap",
                enabled: true,
                allowedInDungeons: false,
                category: HammerCategories.Building,
                craftingStation: nameof(CraftingStations.Workbench),
                requirements: "Wood,4;DeerHide,4;Tar,1",
                clipEverything: true,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Goblin
            )
        },
        {
            "goblin_stairs",
            new PrefabConfig(
                name: "goblin_stairs",
                enabled: true,
                allowedInDungeons: false,
                category: HammerCategories.Building,
                craftingStation: nameof(CraftingStations.Workbench),
                requirements: "Wood,2;Tar,1",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Goblin
            )
        },
        {
            "goblin_stepladder",
            new PrefabConfig(
                name: "goblin_stepladder",
                enabled: true,
                allowedInDungeons: false,
                category: HammerCategories.Building,
                craftingStation: nameof(CraftingStations.Workbench),
                requirements: "Wood,2;Tar,1",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Goblin
            )
        },
        {
            "goblin_strawpile",
            new PrefabConfig(
                name: "goblin_strawpile",
                enabled: true,
                allowedInDungeons: false,
                category: HammerCategories.Furniture,
                craftingStation: nameof(CraftingStations.None),
                requirements: "Wood,4",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceName: "Rug straw (large)",
                pieceDesc: "",
                pieceGroup: PieceClassification.Rug
            )
        },
        {
            "goblin_totempole",
            new PrefabConfig(
                name: "goblin_totempole",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.Workbench),
                requirements: "Wood,2;GoblinTotem,1",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Goblin
            )
        },
        {
            "goblin_trashpile",
            new PrefabConfig(
                name: "goblin_trashpile",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Goblin
            )
        },
        {
            "goblin_woodwall_1m",
            new PrefabConfig(
                name: "goblin_woodwall_1m",
                enabled: true,
                allowedInDungeons: false,
                category: HammerCategories.Building,
                craftingStation: nameof(CraftingStations.Workbench),
                requirements: "Wood,1;Tar,1",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Goblin
            )
        },
        {
            "goblin_woodwall_2m",
            new PrefabConfig(
                name: "goblin_woodwall_2m",
                enabled: true,
                allowedInDungeons: false,
                category: HammerCategories.Building,
                craftingStation: nameof(CraftingStations.Workbench),
                requirements: "Wood,2;Tar,1",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Goblin
            )
        },
        {
            "goblin_woodwall_2m_ribs",
            new PrefabConfig(
                name: "goblin_woodwall_2m_ribs",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "BoneFragments,6;Tar,1",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Goblin
            )
        },
        {
            "goblinking_totemholder",
            new PrefabConfig(
                name: "goblinking_totemholder",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.Stonecutter),
                requirements: "Stone,1;Tar,1",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Goblin
            )
        },
        {
            "hanging_hairstrands",
            new PrefabConfig(
                name: "hanging_hairstrands",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "highstone",
            new PrefabConfig(
                name: "highstone",
                enabled: true,
                allowedInDungeons: false,
                category: HammerCategories.Nature,
                craftingStation: nameof(CraftingStations.None),
                requirements: "Stone,32",
                clipEverything: true,
                pieceGroup: PieceClassification.Rock
            )
        },
        {
            "highstone_frac",
            new PrefabConfig(
                name: "highstone_frac",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "Stone,32",
                clipEverything: true,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Rock
            )
        },
        {
            "ice1",
            new PrefabConfig(
                name: "ice1",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "ice_rock1",
            new PrefabConfig(
                name: "ice_rock1",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "Crystal,32",
                clipEverything: true
            )
        },
        {
            "ice_rock1_frac",
            new PrefabConfig(
                name: "ice_rock1_frac",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "Crystal,32",
                clipEverything: true,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "iron_floor_1x1",
            new PrefabConfig(
                name: "iron_floor_1x1",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "iron_wall_1x1_rusty",
            new PrefabConfig(
                name: "iron_wall_1x1_rusty",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.Building,
                craftingStation: nameof(CraftingStations.Forge),
                requirements: "Iron,1",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Iron
            )
        },
        {
            "lavabomb_rock1",
            new PrefabConfig(
                name: "lavabomb_rock1",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: true,
                clipGround: false,
                placementPatch: true
            )
        },
        {
            "lightningAOE",
            new PrefabConfig(
                name: "lightningAOE",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "loot_chest_stone",
            new PrefabConfig(
                name: "loot_chest_stone",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.Furniture,
                craftingStation: nameof(CraftingStations.Stonecutter),
                requirements: "Stone,10",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceName: "Stone chest (light moss)",
                pieceGroup: PieceClassification.Chest,
                invWidth: 5,
                invHeight: 2
            )
        },
        {
            "lox_ribs",
            new PrefabConfig(
                name: "lox_ribs",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "BoneFragments,30",
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "marker01",
            new PrefabConfig(
                name: "marker01",
                enabled: true,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "Stone,4",
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "marker02",
            new PrefabConfig(
                name: "marker02",
                enabled: true,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "Stone,4",
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "metalbar_1x2",
            new PrefabConfig(
                name: "metalbar_1x2",
                enabled: true,
                allowedInDungeons: false,
                category: HammerCategories.Building,
                craftingStation: nameof(CraftingStations.BlackForge),
                requirements: "BlackMarble,2;Copper,1",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceName: "Black marble 1x2 enforced",
                pieceDesc: "",
                pieceGroup: PieceClassification.BlackMarble
            )
        },
        {
            "mistvolume",
            new PrefabConfig(
                name: "mistvolume",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "Eitr,10;Wisp,4",
                clipEverything: true,
                clipGround: false,
                placementPatch: false,
                pieceName: "Mist volume",
                pieceDesc: "Warning: requires devcommands to remove."
            )
        },
        {
            "morgenhole_pile",
            new PrefabConfig(
                name: "morgenhole_pile",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "mountainkit_chair",
            new PrefabConfig(
                name: "mountainkit_chair",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.Furniture,
                craftingStation: nameof(CraftingStations.Workbench),
                requirements: "FineWood,4",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Chair
            )
        },
        {
            "mountainkit_table",
            new PrefabConfig(
                name: "mountainkit_table",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.Furniture,
                craftingStation: nameof(CraftingStations.Workbench),
                requirements: "FineWood,10;IronNails,20",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Table
            )
        },
        {
            "mudpile",
            new PrefabConfig(
                name: "mudpile",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "IronScrap,10"
            )
        },
        {
            "mudpile2",
            new PrefabConfig(
                name: "mudpile2",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "IronScrap,10",
                clipEverything: true
            )
        },
        {
            "mudpile2_frac",
            new PrefabConfig(
                name: "mudpile2_frac",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "IronScrap,10",
                clipEverything: true,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "mudpile_beacon",
            new PrefabConfig(
                name: "mudpile_beacon",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: ""
            )
        },
        {
            "mudpile_frac",
            new PrefabConfig(
                name: "mudpile_frac",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "IronScrap,10",
                clipEverything: true,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "mudpile_old",
            new PrefabConfig(
                name: "mudpile_old",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "IronScrap,10"
            )
        },
        {
            "piece_Charred_Balista",
            new PrefabConfig(
                name: "piece_Charred_Balista",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "piece_blackwood_bench",
            new PrefabConfig(
                name: "piece_blackwood_bench",
                enabled: true,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.Workbench),
                requirements: "Blackwood,6",
                clipEverything: false,
                clipGround: false,
                placementPatch: true
            )
        },
        {
            "piece_dvergr_pole",
            new PrefabConfig(
                name: "piece_dvergr_pole",
                enabled: true,
                allowedInDungeons: false,
                category: HammerCategories.Building,
                craftingStation: nameof(CraftingStations.BlackForge),
                requirements: "YggdrasilWood,1",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceName: "Dvergr wood pole",
                pieceGroup: PieceClassification.Dvergr
            )
        },
        {
            "piece_dvergr_wood_door",
            new PrefabConfig(
                name: "piece_dvergr_wood_door",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.Building,
                craftingStation: nameof(CraftingStations.BlackForge),
                requirements: "YggdrasilWood,16;IronNails,24",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Dvergr
            )
        },
        {
            "piece_dvergr_wood_wall",
            new PrefabConfig(
                name: "piece_dvergr_wood_wall",
                enabled: true,
                allowedInDungeons: false,
                category: HammerCategories.Building,
                craftingStation: nameof(CraftingStations.BlackForge),
                requirements: "YggdrasilWood,5",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Dvergr
            )
        },
        {
            "piece_pot1_cracked",
            new PrefabConfig(
                name: "piece_pot1_cracked",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "piece_pot1_red",
            new PrefabConfig(
                name: "piece_pot1_red",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "piece_pot2_cracked",
            new PrefabConfig(
                name: "piece_pot2_cracked",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "piece_pot2_red",
            new PrefabConfig(
                name: "piece_pot2_red",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "piece_pot3_cracked",
            new PrefabConfig(
                name: "piece_pot3_cracked",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "piece_pot3_red",
            new PrefabConfig(
                name: "piece_pot3_red",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "portal",
            new PrefabConfig(
                name: "portal",
                enabled: true,
                allowedInDungeons: false,
                category: HammerCategories.Misc,
                craftingStation: nameof(CraftingStations.GaldrTable),
                requirements: "GreydwarfEye,10;Stone,20;BlackCore,2",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceName: "Dvergr portal",
                pieceDesc: "Connects another portal with equal or no tag.",
                pieceGroup: PieceClassification.Portal
            )
        },
        {
            "rock1_mistlands",
            new PrefabConfig(
                name: "rock1_mistlands",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.Nature,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: true,
                pieceName: "Rock (large boulder)",
                pieceGroup: PieceClassification.Rock
            )
        },
        {
            "rock1_mountain",
            new PrefabConfig(
                name: "rock1_mountain",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.Nature,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: true,
                pieceGroup: PieceClassification.Rock
            )
        },
        {
            "rock1_mountain_frac",
            new PrefabConfig(
                name: "rock1_mountain_frac",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "Stone,350",
                clipEverything: true,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Rock
            )
        },
        {
            "rock2_heath",
            new PrefabConfig(
                name: "rock2_heath",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.Nature,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: true,
                pieceGroup: PieceClassification.Rock
            )
        },
        {
            "rock2_heath_frac",
            new PrefabConfig(
                name: "rock2_heath_frac",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "Stone,32",
                clipEverything: true,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Rock
            )
        },
        {
            "rock2_mountain",
            new PrefabConfig(
                name: "rock2_mountain",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.Nature,
                craftingStation: nameof(CraftingStations.None),
                requirements: "Stone,64",
                clipEverything: true,
                pieceGroup: PieceClassification.Rock
            )
        },
        {
            "rock2_mountain_frac",
            new PrefabConfig(
                name: "rock2_mountain_frac",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "Stone,64",
                clipEverything: true,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Rock
            )
        },
        {
            "rock3_ice",
            new PrefabConfig(
                name: "rock3_ice",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.Nature,
                craftingStation: nameof(CraftingStations.None),
                requirements: "Crystal,32",
                clipEverything: true,
                pieceGroup: PieceClassification.Rock
            )
        },
        {
            "rock3_ice_frac",
            new PrefabConfig(
                name: "rock3_ice_frac",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "Crystal,32",
                clipEverything: true,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Rock
            )
        },
        {
            "rock3_mountain",
            new PrefabConfig(
                name: "rock3_mountain",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.Nature,
                craftingStation: nameof(CraftingStations.None),
                requirements: "Stone,100",
                clipEverything: true,
                pieceGroup: PieceClassification.Rock
            )
        },
        {
            "rock3_mountain_frac",
            new PrefabConfig(
                name: "rock3_mountain_frac",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "Stone,100",
                clipEverything: true,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Rock
            )
        },
        {
            "rock3_silver",
            new PrefabConfig(
                name: "rock3_silver",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.Nature,
                craftingStation: nameof(CraftingStations.None),
                requirements: "Stone,64;SilverOre,32",
                clipEverything: true,
                pieceGroup: PieceClassification.Ore
            )
        },
        {
            "rock3_silver_frac",
            new PrefabConfig(
                name: "rock3_silver_frac",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "Stone,64;SilverOre,32",
                clipEverything: true,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Ore
            )
        },
        {
            "rock4_ashlands_frac",
            new PrefabConfig(
                name: "rock4_ashlands_frac",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: true,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "rock4_coast",
            new PrefabConfig(
                name: "rock4_coast",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.Nature,
                craftingStation: nameof(CraftingStations.None),
                requirements: "Stone,64",
                clipEverything: true,
                pieceGroup: PieceClassification.Rock
            )
        },
        {
            "rock4_coast_frac",
            new PrefabConfig(
                name: "rock4_coast_frac",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "Stone,64",
                clipEverything: true,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Rock
            )
        },
        {
            "rock4_copper",
            new PrefabConfig(
                name: "rock4_copper",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.Nature,
                craftingStation: nameof(CraftingStations.None),
                requirements: "Stone,64;CopperOre,32",
                clipEverything: true,
                pieceGroup: PieceClassification.Ore
            )
        },
        {
            "rock4_copper_frac",
            new PrefabConfig(
                name: "rock4_copper_frac",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.Nature,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: true,
                clipGround: true,
                placementPatch: false,
                pieceGroup: PieceClassification.Ore
            )
        },
        {
            "rock4_forest",
            new PrefabConfig(
                name: "rock4_forest",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.Nature,
                craftingStation: nameof(CraftingStations.None),
                requirements: "Stone,64",
                clipEverything: true,
                pieceGroup: PieceClassification.Rock
            )
        },
        {
            "rock4_forest_frac",
            new PrefabConfig(
                name: "rock4_forest_frac",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "Stone,64",
                clipEverything: true,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Rock
            )
        },
        {
            "rock4_heath",
            new PrefabConfig(
                name: "rock4_heath",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.Nature,
                craftingStation: nameof(CraftingStations.None),
                requirements: "Stone,32",
                clipEverything: true,
                pieceGroup: PieceClassification.Rock
            )
        },
        {
            "rock4_heath_frac",
            new PrefabConfig(
                name: "rock4_heath_frac",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "Stone,32",
                clipEverything: true,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Rock
            )
        },
        {
            "rock_mistlands1",
            new PrefabConfig(
                name: "rock_mistlands1",
                enabled: true,
                allowedInDungeons: false,
                category: HammerCategories.Nature,
                craftingStation: nameof(CraftingStations.None),
                requirements: "Stone,48",
                clipEverything: true,
                pieceName: "Rock (black)",
                pieceGroup: PieceClassification.Rock
            )
        },
        {
            "rock_mistlands1_frac",
            new PrefabConfig(
                name: "rock_mistlands1_frac",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "Stone,48",
                clipEverything: true,
                clipGround: false,
                placementPatch: false,
                pieceName: "Rock (black)",
                pieceGroup: PieceClassification.Rock
            )
        },
        {
            "rock_mistlands2",
            new PrefabConfig(
                name: "rock_mistlands2",
                enabled: true,
                allowedInDungeons: false,
                category: HammerCategories.Nature,
                craftingStation: nameof(CraftingStations.None),
                requirements: "Stone,48",
                clipEverything: true,
                clipGround: false,
                placementPatch: false,
                pieceName: "Rock (black, alt)",
                pieceGroup: PieceClassification.Rock,
                spawnOnDestroyed: "sfx_rock_destroyed"
            )
        },
        {
            "rockformation1",
            new PrefabConfig(
                name: "rockformation1",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.Nature,
                craftingStation: nameof(CraftingStations.None),
                requirements: "Stone,350",
                clipEverything: true,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Rock
            )
        },
        {
            "root07",
            new PrefabConfig(
                name: "root07",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.Nature,
                craftingStation: nameof(CraftingStations.None),
                requirements: "ElderBark,2",
                clipEverything: true,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Flora
            )
        },
        {
            "root08",
            new PrefabConfig(
                name: "root08",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.Nature,
                craftingStation: nameof(CraftingStations.None),
                requirements: "ElderBark,4",
                clipEverything: true,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Flora
            )
        },
        {
            "root11",
            new PrefabConfig(
                name: "root11",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.Nature,
                craftingStation: nameof(CraftingStations.None),
                requirements: "ElderBark,4",
                clipEverything: true,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Flora
            )
        },
        {
            "root12",
            new PrefabConfig(
                name: "root12",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.Nature,
                craftingStation: nameof(CraftingStations.None),
                requirements: "ElderBark,4",
                clipEverything: true,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Flora
            )
        },
        {
            "rug_bogwitch_deer",
            new PrefabConfig(
                name: "rug_bogwitch_deer",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "rug_bogwitch_fur",
            new PrefabConfig(
                name: "rug_bogwitch_fur",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "rug_bogwitch_wolf",
            new PrefabConfig(
                name: "rug_bogwitch_wolf",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "rug_straw",
            new PrefabConfig(
                name: "rug_straw",
                enabled: true,
                allowedInDungeons: false,
                category: HammerCategories.Furniture,
                craftingStation: nameof(CraftingStations.None),
                requirements: "Wood,2",
                clipEverything: false,
                clipGround: false,
                pieceGroup: PieceClassification.Rug
            )
        },
        {
            "shieldgenerator_attack",
            new PrefabConfig(
                name: "shieldgenerator_attack",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "shipwreck_karve_bottomboards",
            new PrefabConfig(
                name: "shipwreck_karve_bottomboards",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "FineWood,8",
                clipEverything: true,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "shipwreck_karve_bow",
            new PrefabConfig(
                name: "shipwreck_karve_bow",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "FineWood,16",
                clipEverything: true,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "shipwreck_karve_dragonhead",
            new PrefabConfig(
                name: "shipwreck_karve_dragonhead",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "FineWood,8",
                clipEverything: true,
                clipGround: false,
                placementPatch: false,
                placementOffset: new Vector3(0f, -1.5f, 6f)
            )
        },
        {
            "shipwreck_karve_stern",
            new PrefabConfig(
                name: "shipwreck_karve_stern",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "FineWood,16",
                clipEverything: true,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "shipwreck_karve_sternpost",
            new PrefabConfig(
                name: "shipwreck_karve_sternpost",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "FineWood,10",
                clipEverything: true,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "shrub_2",
            new PrefabConfig(
                name: "shrub_2",
                enabled: true,
                allowedInDungeons: false,
                category: HammerCategories.Nature,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Flora
            )
        },
        {
            "shrub_2_heath",
            new PrefabConfig(
                name: "shrub_2_heath",
                enabled: true,
                allowedInDungeons: false,
                category: HammerCategories.Nature,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Flora
            )
        },
        {
            "siege_wall_1x1",
            new PrefabConfig(
                name: "siege_wall_1x1",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.Stonecutter),
                requirements: "Grausten,2",
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "sign_notext",
            new PrefabConfig(
                name: "sign_notext",
                enabled: true,
                allowedInDungeons: false,
                category: HammerCategories.Building,
                craftingStation: nameof(CraftingStations.Workbench),
                requirements: "Wood,1",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceName: "Wood plank",
                pieceDesc: "",
                pieceGroup: PieceClassification.Wood
            )
        },
        {
            "silvervein",
            new PrefabConfig(
                name: "silvervein",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.Nature,
                craftingStation: nameof(CraftingStations.None),
                requirements: "Stone,50;SilverOre,50",
                clipEverything: true,
                pieceGroup: PieceClassification.Ore
            )
        },
        {
            "silvervein_frac",
            new PrefabConfig(
                name: "silvervein_frac",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "Stone,50;SilverOre,50",
                clipEverything: true,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Ore
            )
        },
        {
            "smokebomb_explosion",
            new PrefabConfig(
                name: "smokebomb_explosion",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "stone_floor",
            new PrefabConfig(
                name: "stone_floor",
                enabled: true,
                allowedInDungeons: false,
                category: HammerCategories.Stonecutter,
                craftingStation: nameof(CraftingStations.Stonecutter),
                requirements: "Stone,16",
                clipEverything: false,
                clipGround: true,
                placementPatch: false,
                pieceName: "Stone floor 4x4",
                pieceGroup: PieceClassification.Stone
            )
        },
        {
            "stoneblock_fracture",
            new PrefabConfig(
                name: "stoneblock_fracture",
                enabled: true,
                allowedInDungeons: false,
                category: HammerCategories.Stonecutter,
                craftingStation: nameof(CraftingStations.Stonecutter),
                requirements: "Stone,16",
                clipEverything: false,
                clipGround: true,
                placementPatch: false,
                pieceName: "Stone floor 4x4 (2)",
                pieceGroup: PieceClassification.Stone
            )
        },
        {
            "stubbe",
            new PrefabConfig(
                name: "stubbe",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.Nature,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: true,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Flora
            )
        },
        {
            "stubbe_spawner",
            new PrefabConfig(
                name: "stubbe_spawner",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: true,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "sunken_crypt_gate",
            new PrefabConfig(
                name: "sunken_crypt_gate",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.Building,
                craftingStation: nameof(CraftingStations.Forge),
                requirements: "Iron,4",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Iron
            )
        },
        {
            "tarlump1",
            new PrefabConfig(
                name: "tarlump1",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "Tar,50",
                clipEverything: true,
                clipGround: false,
                pieceName: "Crystallized tar",
                pieceDesc: "Warning: requires devcommands to remove"
            )
        },
        {
            "tarlump1_frac",
            new PrefabConfig(
                name: "tarlump1_frac",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "Tar,50",
                clipEverything: true,
                clipGround: false,
                placementPatch: false,
                pieceName: "Crystallized tar"
            )
        },
        {
            "trader_wagon_destructable",
            new PrefabConfig(
                name: "trader_wagon_destructable",
                enabled: true,
                allowedInDungeons: false,
                category: HammerCategories.Furniture,
                craftingStation: nameof(CraftingStations.BlackForge),
                requirements: "FineWood,32",
                clipEverything: true,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "tunnel_web",
            new PrefabConfig(
                name: "tunnel_web",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "turf_roof",
            new PrefabConfig(
                name: "turf_roof",
                enabled: true,
                allowedInDungeons: false,
                category: HammerCategories.Building,
                craftingStation: nameof(CraftingStations.Workbench),
                requirements: "Wood,2",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Wood
            )
        },
        {
            "turf_roof_top",
            new PrefabConfig(
                name: "turf_roof_top",
                enabled: true,
                allowedInDungeons: false,
                category: HammerCategories.Building,
                craftingStation: nameof(CraftingStations.Workbench),
                requirements: "Wood,2",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Wood
            )
        },
        {
            "veg_skull_Ashlands",
            new PrefabConfig(
                name: "veg_skull_Ashlands",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: false,
                placementPatch: true
            )
        },
        {
            "vertical_web",
            new PrefabConfig(
                name: "vertical_web",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "vines",
            new PrefabConfig(
                name: "vines",
                enabled: true,
                allowedInDungeons: false,
                category: HammerCategories.Nature,
                craftingStation: nameof(CraftingStations.None),
                requirements: "Wood,2",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Flora
            )
        },
        {
            "volture_strawpile",
            new PrefabConfig(
                name: "volture_strawpile",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "widestone",
            new PrefabConfig(
                name: "widestone",
                enabled: true,
                allowedInDungeons: false,
                category: HammerCategories.Nature,
                craftingStation: nameof(CraftingStations.None),
                requirements: "Stone,32",
                clipEverything: true,
                pieceGroup: PieceClassification.Rock
            )
        },
        {
            "widestone_frac",
            new PrefabConfig(
                name: "widestone_frac",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "Stone,32",
                clipEverything: true,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Rock
            )
        },
        {
            "wood_ledge",
            new PrefabConfig(
                name: "wood_ledge",
                enabled: true,
                allowedInDungeons: false,
                category: HammerCategories.Building,
                craftingStation: nameof(CraftingStations.Workbench),
                requirements: "Wood,1",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Wood
            )
        },
        {
            "yggashoot_log",
            new PrefabConfig(
                name: "yggashoot_log",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.Nature,
                craftingStation: nameof(CraftingStations.None),
                requirements: "YggdrasilWood,10",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Flora
            )
        },
        {
            "yggashoot_log_half",
            new PrefabConfig(
                name: "yggashoot_log_half",
                enabled: false,
                allowedInDungeons: false,
                category: HammerCategories.Nature,
                craftingStation: nameof(CraftingStations.None),
                requirements: "YggdrasilWood,10",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Flora
            )
        },
    };
}
