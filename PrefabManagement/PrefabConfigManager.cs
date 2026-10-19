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
    public static List<PrefabConfig> GetPrefabConfigs(bool checkIfBound = true)
    {
        if (!checkIfBound)
        {
            return PrefabConfigMap.Values.ToList();
        }
        return PrefabConfigMap.Values.Where(x => x.IsBound).ToList();
    }

    /// <summary>
    ///     Get a bool indicating if the prefab is configured to require an offset patch.
    /// </summary>
    /// <param name="prefabName"></param>
    /// <returns></returns>
    internal static bool NeedsOffsetPatch(string prefabName)
    {
        if (TryGetPrefabConfig(prefabName, out PrefabConfig prefabConfig, checkIfBound: true))
        {
            return prefabConfig.OffsetPatch.Value;
        }
        return false;
    }

    /// <summary>
    ///     Get a bool indicating if the prefab is configured to require a placement patch.
    /// </summary>
    /// <param name="prefabName"></param>
    /// <returns></returns>
    internal static bool NeedsCollisionPatchForGhost(string prefabName)
    {
        if (TryGetPrefabConfig(prefabName, out PrefabConfig prefabConfig, checkIfBound: true))
        {
            return prefabConfig.PlacementPatch.Value;
        }
        return false;
    }


    internal static bool IsPrefabEnabled(GameObject gameObject)
    {
        if (TryGetPrefabConfig(gameObject.GetPrefabName(), out PrefabConfig prefabConfig, checkIfBound: true))
        {
            return prefabConfig.Enabled.Value || MorePrefabs.IsForceAllPrefabs;
        }
        return false;
    }

    internal static bool IsPrefabEnabled(string name)
    {
        if (TryGetPrefabConfig(name, out PrefabConfig prefabConfig, checkIfBound: true))
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
            // if no existing PrefabConfig then make one and add to map
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
            if (!MVPrefabManager.TryGetEligiblePrefab(prefabName, out GameObject prefab))
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
        prefabConfig.Enabled.SettingChanged += UpdateMananger.PieceSettingChanged;
        prefabConfig.AllowedInDungeons.SettingChanged += UpdateMananger.PieceSettingChanged;
        prefabConfig.Category.SettingChanged += UpdateMananger.PieceSettingChanged;
        prefabConfig.CraftingStation.SettingChanged += UpdateMananger.PieceSettingChanged;
        prefabConfig.Requirements.SettingChanged += UpdateMananger.PieceSettingChanged;
        prefabConfig.ClipEverything.SettingChanged += UpdateMananger.PieceSettingChanged;
        prefabConfig.ClipGround.SettingChanged += UpdateMananger.PieceSettingChanged;

        prefabConfig.OffsetPatch.SettingChanged += UpdateMananger.PlacementSettingChanged;
        prefabConfig.PlacementPatch.SettingChanged += UpdateMananger.PlacementSettingChanged;
        if (prefabConfig.PlacementOffset is not null)
        {
            prefabConfig.PlacementOffset.SettingChanged += UpdateMananger.ModSettingChanged;
        }
    }

    private static readonly Dictionary<string, PrefabConfig> PrefabConfigMap = new()
    {
        {
            "ArmorStand_Female",
            new PrefabConfig(
                name: "ArmorStand_Female",
                enabled: true,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.Furniture,
                craftingStation: nameof(CraftingStations.Workbench),
                requirements: "FineWood,8;BronzeNails,2;Tar,4",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceDesc: "",
                pieceGroup: PieceClassification.ArmorStand,
                comfortGroup: Piece.ComfortGroup.Display
            )
        },
        {
            "ArmorStand_Male",
            new PrefabConfig(
                name: "ArmorStand_Male",
                enabled: true,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.Furniture,
                craftingStation: nameof(CraftingStations.Workbench),
                requirements: "FineWood,8;BronzeNails,2;Tar,4",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceDesc: "",
                pieceGroup: PieceClassification.ArmorStand,
                comfortGroup: Piece.ComfortGroup.Display
            )
        },
        {
            "Ashland_Stair",
            new PrefabConfig(
                name: "Ashland_Stair",
                enabled: true,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.Stonecutter),
                requirements: "Grausten,24;FlametalNew,2",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceName: "Grausten Stairs 4x2 (Reinforced)",
                pieceDesc: "Nothing short of a siege should break this, however...",
                pieceGroup: PieceClassification.Fortress
            )
        },
        {
            "Ashland_Steepstair",
            new PrefabConfig(
                name: "Ashland_Steepstair",
                enabled: true,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.Stonecutter),
                requirements: "Grausten,6",
                clipEverything: false,
                clipGround: false,
                placementPatch: true,
                pieceName: "Grausten Steep Stairs",
                pieceGroup: PieceClassification.Grausten
            )
        },
        {
            "AshlandsBranch1",
            new PrefabConfig(
                name: "AshlandsBranch1",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Cultivator),
                category: PieceCategoryManager.Nature,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: false,
                placementPatch: true,
                pieceGroup: PieceClassification.Flora
            )
        },
        {
            "AshlandsBranch2",
            new PrefabConfig(
                name: "AshlandsBranch2",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Cultivator),
                category: PieceCategoryManager.Nature,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Flora
            )
        },
        {
            "AshlandsBranch3",
            new PrefabConfig(
                name: "AshlandsBranch3",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Cultivator),
                category: PieceCategoryManager.Nature,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: false,
                placementPatch: true,
                pieceGroup: PieceClassification.Flora
            )
        },
        {
            "AshlandsBush1",
            new PrefabConfig(
                name: "AshlandsBush1",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Cultivator),
                category: PieceCategoryManager.Nature,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: false,
                placementPatch: true,
                pieceGroup: PieceClassification.Flora
            )
        },
        {
            "AshlandsBush2",
            new PrefabConfig(
                name: "AshlandsBush2",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Cultivator),
                category: PieceCategoryManager.Nature,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: false,
                placementPatch: true,
                pieceGroup: PieceClassification.Flora
            )
        },
        {
            "AshlandsTree1",
            new PrefabConfig(
                name: "AshlandsTree1",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Cultivator),
                category: PieceCategoryManager.Nature,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: true,
                clipGround: false,
                placementPatch: false,
                placementOffset: new Vector3(0f, 1f, 0f),
                pieceGroup: PieceClassification.Flora
            )
        },
        {
            "AshlandsTree3",
            new PrefabConfig(
                name: "AshlandsTree3",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Cultivator),
                category: PieceCategoryManager.Nature,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: true,
                clipGround: false,
                placementPatch: false,
                placementOffset: new Vector3(0f, 1f, 0f),
                pieceGroup: PieceClassification.Flora
            )
        },
        {
            "AshlandsTree4",
            new PrefabConfig(
                name: "AshlandsTree4",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Cultivator),
                category: PieceCategoryManager.Nature,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: true,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Flora
            )
        },
        {
            "AshlandsTree5",
            new PrefabConfig(
                name: "AshlandsTree5",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Cultivator),
                category: PieceCategoryManager.Nature,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: true,
                clipGround: false,
                placementPatch: false,
                placementOffset: new Vector3(0f, 0.5f, 0f),
                pieceGroup: PieceClassification.Flora
            )
        },
        {
            "AshlandsTree6",
            new PrefabConfig(
                name: "AshlandsTree6",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Cultivator),
                category: PieceCategoryManager.Nature,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: true,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Flora
            )
        },
        {
            "AshlandsTree6_big",
            new PrefabConfig(
                name: "AshlandsTree6_big",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Cultivator),
                category: PieceCategoryManager.Nature,
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
                pieceTable: nameof(PieceTables.Cultivator),
                category: PieceCategoryManager.Nature,
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
                pieceTable: nameof(PieceTables.Cultivator),
                category: PieceCategoryManager.Nature,
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
                pieceTable: nameof(PieceTables.Cultivator),
                category: PieceCategoryManager.Nature,
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
                pieceTable: nameof(PieceTables.Cultivator),
                category: PieceCategoryManager.Nature,
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
                pieceTable: nameof(PieceTables.Cultivator),
                category: PieceCategoryManager.Nature,
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
                pieceTable: nameof(PieceTables.Cultivator),
                category: PieceCategoryManager.Nature,
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
                pieceTable: nameof(PieceTables.Cultivator),
                category: PieceCategoryManager.Nature,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: true,
                placementPatch: false,
                pieceGroup: PieceClassification.Flora
            )
        },
        {
            "Ashlands_Altar",
            new PrefabConfig(
                name: "Ashlands_Altar",
                enabled: true,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.Stonecutter),
                requirements: "Grausten,5;ProustitePowder,2",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceName: "Grausten Altar"
            )
        },
        {
            "Ashlands_Arch1",
            new PrefabConfig(
                name: "Ashlands_Arch1",
                enabled: true,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.Stonecutter),
                requirements: "Grausten,10",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceName: "Grausten Arch (Medium)",
                pieceGroup: PieceClassification.Grausten
            )
        },
        {
            "Ashlands_Arch2",
            new PrefabConfig(
                name: "Ashlands_Arch2",
                enabled: true,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.Stonecutter),
                requirements: "Grausten,16",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceName: "Grausten Large Arch",
                pieceGroup: PieceClassification.Grausten
            )
        },
        {
            "Ashlands_Arch2_Broken1",
            new PrefabConfig(
                name: "Ashlands_Arch2_Broken1",
                enabled: true,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.Stonecutter),
                requirements: "Grausten,10",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceName: "Grausten Large Arch (Base)",
                pieceGroup: PieceClassification.Grausten
            )
        },
        {
            "Ashlands_Arch2_Broken2",
            new PrefabConfig(
                name: "Ashlands_Arch2_Broken2",
                enabled: true,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.Stonecutter),
                requirements: "Grausten,6",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceName: "Grausten Large Arch (Top)",
                pieceGroup: PieceClassification.Grausten
            )
        },
        {
            "Ashlands_ArchRoof",
            new PrefabConfig(
                name: "Ashlands_ArchRoof",
                enabled: true,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.Stonecutter),
                requirements: "Grausten,24",
                clipEverything: false,
                clipGround: false,
                placementPatch: true,
                pieceName: "Grausten Arched Roof",
                pieceGroup: PieceClassification.Grausten
            )
        },
        {
            "Ashlands_ArchRoofDamaged",
            new PrefabConfig(
                name: "Ashlands_ArchRoofDamaged",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.Stonecutter),
                requirements: "Grausten,22",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceName: "Grausten Arched Roof (Damaged)",
                pieceGroup: PieceClassification.Grausten
            )
        },
        {
            "Ashlands_ArchRoofDamaged_half1",
            new PrefabConfig(
                name: "Ashlands_ArchRoofDamaged_half1",
                enabled: true,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.Stonecutter),
                requirements: "Grausten,16",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceName: "Grausten Arched Roof (Left)",
                pieceGroup: PieceClassification.Grausten
            )
        },
        {
            "Ashlands_ArchRoofDamaged_half2",
            new PrefabConfig(
                name: "Ashlands_ArchRoofDamaged_half2",
                enabled: true,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.Stonecutter),
                requirements: "Grausten,16",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceName: "Grausten Arched Roof (Right)",
                pieceGroup: PieceClassification.Grausten
            )
        },
        {
            "Ashlands_ArchRoofLong_Damaged",
            new PrefabConfig(
                name: "Ashlands_ArchRoofLong_Damaged",
                enabled: true,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.Stonecutter),
                requirements: "Grausten,16",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceName: "Grausten Arched Roof",
                pieceGroup: PieceClassification.Grausten
            )
        },
        {
            "Ashlands_Boss_Pillar",
            new PrefabConfig(
                name: "Ashlands_Boss_Pillar",
                enabled: true,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.Stonecutter),
                requirements: "Grausten,64",
                clipEverything: false,
                clipGround: false,
                placementPatch: true,
                pieceName: "Grausten Pillar (Giant)",
                pieceGroup: PieceClassification.Grausten
            )
        },
        {
            "Ashlands_Boss_Pillar_Twist_broken1",
            new PrefabConfig(
                name: "Ashlands_Boss_Pillar_Twist_broken1",
                enabled: true,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.Stonecutter),
                requirements: "Grausten,32",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceName: "Grausten Twisted Pillar (Base)",
                pieceGroup: PieceClassification.Grausten
            )
        },
        {
            "Ashlands_Boss_Pillar_Twist_broken2",
            new PrefabConfig(
                name: "Ashlands_Boss_Pillar_Twist_broken2",
                enabled: true,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.Stonecutter),
                requirements: "Grausten,32",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceName: "Grausten Twisted Pillar (Middle)",
                pieceGroup: PieceClassification.Grausten
            )
        },
        {
            "Ashlands_Boss_Pillar_Twist_broken3",
            new PrefabConfig(
                name: "Ashlands_Boss_Pillar_Twist_broken3",
                enabled: true,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.Stonecutter),
                requirements: "Grausten,32",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceName: "Grausten Twisted Pillar (Top)",
                pieceGroup: PieceClassification.Grausten
            )
        },
        {
            "Ashlands_Floor",
            new PrefabConfig(
                name: "Ashlands_Floor",
                enabled: true,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.Stonecutter),
                requirements: "Grausten,6",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceName: "Grausten Floor 2x2",
                pieceGroup: PieceClassification.Grausten
            )
        },
        {
            "Ashlands_Fortress_Floor",
            new PrefabConfig(
                name: "Ashlands_Fortress_Floor",
                enabled: true,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.Stonecutter),
                requirements: "Grausten,6;FlametalNew,1",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceName: "Grausten Floor 2x2 (Reinforced)",
                pieceDesc: "It would take a siege to break this.",
                pieceGroup: PieceClassification.Fortress
            )
        },
        {
            "Ashlands_Fortress_Gate",
            new PrefabConfig(
                name: "Ashlands_Fortress_Gate",
                enabled: true,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.Stonecutter),
                requirements: "Grausten,32;FlametalNew,4",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceName: "Grausten Gateway (Reinforced)",
                pieceDesc: "It would take a siege to break this.",
                pieceGroup: PieceClassification.Fortress
            )
        },
        {
            "Ashlands_Fortress_Gate_Door",
            new PrefabConfig(
                name: "Ashlands_Fortress_Gate_Door",
                enabled: true,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.BlackForge),
                requirements: "Copper,30;FlametalNew,5",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceName: "Grausten Gate Door (Reinforced)",
                pieceDesc: "It would take a siege to break this.",
                pieceGroup: PieceClassification.Fortress
            )
        },
        {
            "Ashlands_Fortress_Wall_PillarTopStone_frac",
            new PrefabConfig(
                name: "Ashlands_Fortress_Wall_PillarTopStone_frac",
                enabled: true,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.Stonecutter),
                requirements: "Grausten,12;Stone,30;FlametalNew,4",
                clipEverything: false,
                clipGround: false,
                placementPatch: true,
                placementOffset: new Vector3(0f, -0.1f, 0f),
                pieceName: "Grausten Pillar Top (Reinforced)",
                pieceDesc: "It would take a siege to break this.",
                pieceGroup: PieceClassification.Fortress
            )
        },
        {
            "Ashlands_Fortress_Wall_PillarTop_frac",
            new PrefabConfig(
                name: "Ashlands_Fortress_Wall_PillarTop_frac",
                enabled: true,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.Stonecutter),
                requirements: "Grausten,12;Stone,60;FlametalNew,4",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceName: "Grausten Pillar (Reinforced)",
                pieceDesc: "It would take a siege to break this.",
                pieceGroup: PieceClassification.Fortress
            )
        },
        {
            "Ashlands_Fortress_Wall_Pillar_frac",
            new PrefabConfig(
                name: "Ashlands_Fortress_Wall_Pillar_frac",
                enabled: true,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.Stonecutter),
                requirements: "Grausten,12;Stone,78;FlametalNew,4",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceName: "Grausten Buttress (Reinforced)",
                pieceDesc: "It would take a siege to break this.",
                pieceGroup: PieceClassification.Fortress
            )
        },
        {
            "Ashlands_Fortress_Wall_Spikes",
            new PrefabConfig(
                name: "Ashlands_Fortress_Wall_Spikes",
                enabled: true,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.BlackForge),
                requirements: "Copper,4;FlametalNew,2",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceName: "Reinforced Spikes",
                pieceDesc: "It would take a siege to break this.",
                pieceGroup: PieceClassification.Fortress
            )
        },
        {
            "Ashlands_Pillar4",
            new PrefabConfig(
                name: "Ashlands_Pillar4",
                enabled: true,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.Stonecutter),
                requirements: "Grausten,24",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceName: "Grausten Pillar 2x2x4",
                pieceGroup: PieceClassification.Grausten
            )
        },
        {
            "Ashlands_Pillar4_tip",
            new PrefabConfig(
                name: "Ashlands_Pillar4_tip",
                enabled: true,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.Stonecutter),
                requirements: "Grausten,24",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceName: "Grausten Spiked Pillar",
                pieceGroup: PieceClassification.Grausten
            )
        },
        {
            "Ashlands_Pillar4_tip2",
            new PrefabConfig(
                name: "Ashlands_Pillar4_tip2",
                enabled: true,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.Stonecutter),
                requirements: "Grausten,24",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceName: "Grausten Curved Spiked Pillar",
                pieceGroup: PieceClassification.Grausten
            )
        },
        {
            "Ashlands_Pillar4_tip2_broken1",
            new PrefabConfig(
                name: "Ashlands_Pillar4_tip2_broken1",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.Stonecutter),
                requirements: "Grausten,12",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceName: "Grausten Curved Spiked Pillar (Base)",
                pieceGroup: PieceClassification.Grausten
            )
        },
        {
            "Ashlands_Pillar4_tip2_broken2",
            new PrefabConfig(
                name: "Ashlands_Pillar4_tip2_broken2",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.Stonecutter),
                requirements: "Grausten,12",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceName: "Grausten Curved Spiked Pillar (Top)",
                pieceGroup: PieceClassification.Grausten
            )
        },
        {
            "Ashlands_Pillar4_tip3",
            new PrefabConfig(
                name: "Ashlands_Pillar4_tip3",
                enabled: true,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.Stonecutter),
                requirements: "Grausten,32",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceName: "Grausten Twisted Spiked Pillar (Large)",
                pieceGroup: PieceClassification.Grausten
            )
        },
        {
            "Ashlands_Pillar4_tip3_broken1",
            new PrefabConfig(
                name: "Ashlands_Pillar4_tip3_broken1",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.Stonecutter),
                requirements: "Grausten,10",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceName: "Grausten Twisted Spiked Pillar (Base)",
                pieceGroup: PieceClassification.Grausten
            )
        },
        {
            "Ashlands_Pillar4_tip3_broken2",
            new PrefabConfig(
                name: "Ashlands_Pillar4_tip3_broken2",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.Stonecutter),
                requirements: "Grausten,10",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceName: "Grausten Twisted Spiked Pillar (Middle)",
                pieceGroup: PieceClassification.Grausten
            )
        },
        {
            "Ashlands_Pillar4_tip3_broken3",
            new PrefabConfig(
                name: "Ashlands_Pillar4_tip3_broken3",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.Stonecutter),
                requirements: "Grausten,12",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceName: "Grausten Twisted Spiked Pillar (Top)",
                pieceGroup: PieceClassification.Grausten
            )
        },
        {
            "Ashlands_Pillar4_tip_broken1",
            new PrefabConfig(
                name: "Ashlands_Pillar4_tip_broken1",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.Stonecutter),
                requirements: "Grausten,12",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceName: "Grausten Spiked Pillar (Base)",
                pieceGroup: PieceClassification.Grausten
            )
        },
        {
            "Ashlands_Pillar4_tip_broken2",
            new PrefabConfig(
                name: "Ashlands_Pillar4_tip_broken2",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.Stonecutter),
                requirements: "Grausten,12",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceName: "Grausten Spiked Pillar (Top)",
                pieceGroup: PieceClassification.Grausten
            )
        },
        {
            "Ashlands_PillarBase3_double",
            new PrefabConfig(
                name: "Ashlands_PillarBase3_double",
                enabled: true,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.Stonecutter),
                requirements: "Grausten,5",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceName: "Grausten Pillar Base",
                pieceGroup: PieceClassification.Grausten
            )
        },
        {
            "Ashlands_Ramp",
            new PrefabConfig(
                name: "Ashlands_Ramp",
                enabled: true,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.Stonecutter),
                requirements: "Grausten,12",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceName: "Grausten Buttress",
                pieceGroup: PieceClassification.Grausten
            )
        },
        {
            "Ashlands_Ruins_Floor_1point5x1point5",
            new PrefabConfig(
                name: "Ashlands_Ruins_Floor_1point5x1point5",
                enabled: true,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.Stonecutter),
                requirements: "Grausten,3",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceName: "Grausten Floor 1.5x1.5",
                pieceGroup: PieceClassification.Grausten
            )
        },
        {
            "Ashlands_Ruins_Floor_1point5x1point5_broken",
            new PrefabConfig(
                name: "Ashlands_Ruins_Floor_1point5x1point5_broken",
                enabled: true,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.Stonecutter),
                requirements: "Grausten,3",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceName: "Grausten Floor 1.5x1.5 (Broken)",
                pieceGroup: PieceClassification.Grausten
            )
        },
        {
            "Ashlands_Ruins_Floor_3x3",
            new PrefabConfig(
                name: "Ashlands_Ruins_Floor_3x3",
                enabled: true,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.Stonecutter),
                requirements: "Grausten,6",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceName: "Grausten Floor 3x3",
                pieceGroup: PieceClassification.Grausten
            )
        },
        {
            "Ashlands_Ruins_Floor_3x3_broken1",
            new PrefabConfig(
                name: "Ashlands_Ruins_Floor_3x3_broken1",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.Stonecutter),
                requirements: "Grausten,5",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceName: "Grausten Floor 3x3 (Broken) (1)",
                pieceGroup: PieceClassification.Grausten
            )
        },
        {
            "Ashlands_Ruins_Floor_3x3_broken2",
            new PrefabConfig(
                name: "Ashlands_Ruins_Floor_3x3_broken2",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.Stonecutter),
                requirements: "Grausten,5",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceName: "Grausten Floor 3x3 (Broken) (2)",
                pieceGroup: PieceClassification.Grausten
            )
        },
        {
            "Ashlands_Ruins_Floor_3x3_broken3",
            new PrefabConfig(
                name: "Ashlands_Ruins_Floor_3x3_broken3",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.Stonecutter),
                requirements: "Grausten,5",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceName: "Grausten Floor 3x3 (Broken) (3)",
                pieceGroup: PieceClassification.Grausten
            )
        },
        {
            "Ashlands_Ruins_Floor_6x6",
            new PrefabConfig(
                name: "Ashlands_Ruins_Floor_6x6",
                enabled: true,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.Stonecutter),
                requirements: "Grausten,12",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceName: "Grausten Floor 6x6",
                pieceGroup: PieceClassification.Grausten
            )
        },
        {
            "Ashlands_Ruins_Floor_6x6_broken1",
            new PrefabConfig(
                name: "Ashlands_Ruins_Floor_6x6_broken1",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.Stonecutter),
                requirements: "Grausten,10",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceName: "Grausten Floor 6x6 (Broken) (1)",
                pieceGroup: PieceClassification.Grausten
            )
        },
        {
            "Ashlands_Ruins_Floor_6x6_broken2",
            new PrefabConfig(
                name: "Ashlands_Ruins_Floor_6x6_broken2",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.Stonecutter),
                requirements: "Grausten,10",
                clipEverything: false,
                clipGround: false,
                placementPatch: true,
                pieceName: "Grausten Floor 6x6 (Broken) (2)",
                pieceGroup: PieceClassification.Grausten
            )
        },
        {
            "Ashlands_Ruins_Ramp",
            new PrefabConfig(
                name: "Ashlands_Ruins_Ramp",
                enabled: true,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.Stonecutter),
                requirements: "Grausten,12",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceName: "Grausten Buttress (Weathered)",
                pieceGroup: PieceClassification.Grausten
            )
        },
        {
            "Ashlands_Ruins_Ramp_Upsidedown",
            new PrefabConfig(
                name: "Ashlands_Ruins_Ramp_Upsidedown",
                enabled: true,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.Stonecutter),
                requirements: "Grausten,12",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceName: "Grausten Buttress Inverted (Weathered)",
                pieceGroup: PieceClassification.Grausten
            )
        },
        {
            "Ashlands_Ruins_TopStone",
            new PrefabConfig(
                name: "Ashlands_Ruins_TopStone",
                enabled: true,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.Stonecutter),
                requirements: "Grausten,6",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceName: "Grausten Top Stone",
                pieceGroup: PieceClassification.Grausten
            )
        },
        {
            "Ashlands_Ruins_Wall_4x6",
            new PrefabConfig(
                name: "Ashlands_Ruins_Wall_4x6",
                enabled: true,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.Stonecutter),
                requirements: "Grausten,24",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceName: "Grausten Wall 4x6",
                pieceGroup: PieceClassification.Grausten
            )
        },
        {
            "Ashlands_Ruins_Wall_Broken3_4x6",
            new PrefabConfig(
                name: "Ashlands_Ruins_Wall_Broken3_4x6",
                enabled: true,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.Stonecutter),
                requirements: "Grausten,8",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceName: "Grausten Wall 4x6 (Left)",
                pieceGroup: PieceClassification.Grausten
            )
        },
        {
            "Ashlands_Ruins_Wall_Broken4_4x6",
            new PrefabConfig(
                name: "Ashlands_Ruins_Wall_Broken4_4x6",
                enabled: true,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.Stonecutter),
                requirements: "Grausten,8",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceName: "Grausten Wall 4x6 (Center)",
                pieceGroup: PieceClassification.Grausten
            )
        },
        {
            "Ashlands_Ruins_Wall_Broken5_4x6",
            new PrefabConfig(
                name: "Ashlands_Ruins_Wall_Broken5_4x6",
                enabled: true,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.Stonecutter),
                requirements: "Grausten,8",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceName: "Grausten Wall 4x6 (Right)",
                pieceGroup: PieceClassification.Grausten
            )
        },
        {
            "Ashlands_Ruins_Wall_Top_wHole",
            new PrefabConfig(
                name: "Ashlands_Ruins_Wall_Top_wHole",
                enabled: true,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.Stonecutter),
                requirements: "Grausten,24",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceName: "Grausten Arched Window",
                pieceGroup: PieceClassification.Grausten
            )
        },
        {
            "Ashlands_Ruins_Wall_Window_4x6_broken2",
            new PrefabConfig(
                name: "Ashlands_Ruins_Wall_Window_4x6_broken2",
                enabled: true,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.Stonecutter),
                requirements: "Grausten,10",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceName: "Grausten Window 4x6 (Left Half)",
                pieceGroup: PieceClassification.Grausten
            )
        },
        {
            "Ashlands_Ruins_Wall_Window_4x6_broken3",
            new PrefabConfig(
                name: "Ashlands_Ruins_Wall_Window_4x6_broken3",
                enabled: true,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.Stonecutter),
                requirements: "Grausten,10",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceName: "Grausten Window 4x6 (Right Half)",
                pieceGroup: PieceClassification.Grausten
            )
        },
        {
            "Ashlands_Ruins_Wall_Window_4x6_broken4",
            new PrefabConfig(
                name: "Ashlands_Ruins_Wall_Window_4x6_broken4",
                enabled: true,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.Stonecutter),
                requirements: "Grausten,8",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceName: "Grausten Window 4x6 (Right End)",
                pieceGroup: PieceClassification.Grausten
            )
        },
        {
            "Ashlands_Ruins_Wall_Window_4x6_broken5",
            new PrefabConfig(
                name: "Ashlands_Ruins_Wall_Window_4x6_broken5",
                enabled: true,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.Stonecutter),
                requirements: "Grausten,7",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceName: "Grausten Window 4x6 (Left End)",
                pieceGroup: PieceClassification.Grausten
            )
        },
        {
            "Ashlands_Ruins_Wall_Window_4x6_broken6",
            new PrefabConfig(
                name: "Ashlands_Ruins_Wall_Window_4x6_broken6",
                enabled: true,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.Stonecutter),
                requirements: "Grausten,7",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceName: "Grausten Window 4x6 (Center)",
                pieceGroup: PieceClassification.Grausten
            )
        },
        {
            "Ashlands_Ruins_Wall_Windows_Broken_4x6",
            new PrefabConfig(
                name: "Ashlands_Ruins_Wall_Windows_Broken_4x6",
                enabled: true,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.Stonecutter),
                requirements: "Grausten,20",
                clipEverything: false,
                clipGround: false,
                placementPatch: true,
                pieceName: "Grausten Window 4x6 (Broken)",
                pieceGroup: PieceClassification.Grausten
            )
        },
        {
            "Ashlands_Ruins_twist_ArchBig",
            new PrefabConfig(
                name: "Ashlands_Ruins_twist_ArchBig",
                enabled: true,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.Stonecutter),
                requirements: "Grausten,16",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceName: "Grausten Arch (Large)",
                pieceGroup: PieceClassification.Grausten
            )
        },
        {
            "Ashlands_Ruins_twist_PillarBase",
            new PrefabConfig(
                name: "Ashlands_Ruins_twist_PillarBase",
                enabled: true,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.Stonecutter),
                requirements: "Grausten,18",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceName: "Grausten Twised Pillar Base (Large)",
                pieceGroup: PieceClassification.Grausten
            )
        },
        {
            "Ashlands_Ruins_twist_PillarBaseSmall",
            new PrefabConfig(
                name: "Ashlands_Ruins_twist_PillarBaseSmall",
                enabled: true,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.Stonecutter),
                requirements: "Grausten,5",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceName: "Grausten Pillar",
                pieceGroup: PieceClassification.Grausten
            )
        },
        {
            "Ashlands_StairsBroad",
            new PrefabConfig(
                name: "Ashlands_StairsBroad",
                enabled: true,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.Stonecutter),
                requirements: "Grausten,24",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceName: "Grausten Dias Stairs",
                pieceGroup: PieceClassification.Grausten
            )
        },
        {
            "Ashlands_WallBlock",
            new PrefabConfig(
                name: "Ashlands_WallBlock",
                enabled: true,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.Stonecutter),
                requirements: "Grausten,6",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceName: "Grausten Wall 2x2",
                pieceGroup: PieceClassification.Grausten
            )
        },
        {
            "Ashlands_WallBlock_1x2x2",
            new PrefabConfig(
                name: "Ashlands_WallBlock_1x2x2",
                enabled: true,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.Stonecutter),
                requirements: "Grausten,5",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceName: "Grausten Wall 2x1",
                pieceGroup: PieceClassification.Grausten
            )
        },
        {
            "Ashlands_WallBlock_base",
            new PrefabConfig(
                name: "Ashlands_WallBlock_base",
                enabled: true,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.Stonecutter),
                requirements: "Grausten,5",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceName: "Grausten Plinth",
                pieceGroup: PieceClassification.Grausten
            )
        },
        {
            "Ashlands_Wall_2x2",
            new PrefabConfig(
                name: "Ashlands_Wall_2x2",
                enabled: true,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.Stonecutter),
                requirements: "Grausten,6;FlametalNew,1",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceName: "Grausten Wall 2x2 (Reinforced)",
                pieceDesc: "It would take a siege to break this.",
                pieceGroup: PieceClassification.Fortress
            )
        },
        {
            "Ashlands_Wall_2x2_cornerL",
            new PrefabConfig(
                name: "Ashlands_Wall_2x2_cornerL",
                enabled: true,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.Stonecutter),
                requirements: "Grausten,8;FlametalNew,1",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceName: "Grausten Left Corner Wall 2x2 (Reinforced)",
                pieceDesc: "Nothing short of a siege should break this, however...",
                pieceGroup: PieceClassification.Fortress
            )
        },
        {
            "Ashlands_Wall_2x2_cornerL_top",
            new PrefabConfig(
                name: "Ashlands_Wall_2x2_cornerL_top",
                enabled: true,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.Stonecutter),
                requirements: "Grausten,8;FlametalNew,1",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceName: "Grausten Left Worner Wall Top 2x2 (Reinforced)",
                pieceDesc: "Nothing short of a siege should break this, however...",
                pieceGroup: PieceClassification.Fortress
            )
        },
        {
            "Ashlands_Wall_2x2_cornerR",
            new PrefabConfig(
                name: "Ashlands_Wall_2x2_cornerR",
                enabled: true,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.Stonecutter),
                requirements: "Grausten,8;FlametalNew,1",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceName: "Grausten Right Corner Wall 2x2 (Reinforced)",
                pieceDesc: "Nothing short of a siege should break this, however...",
                pieceGroup: PieceClassification.Fortress
            )
        },
        {
            "Ashlands_Wall_2x2_cornerR_top",
            new PrefabConfig(
                name: "Ashlands_Wall_2x2_cornerR_top",
                enabled: true,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.Stonecutter),
                requirements: "Grausten,8;FlametalNew,1",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceName: "Grausten Right Corner Wall Top 2x2 (Reinforced)",
                pieceDesc: "Nothing short of a siege should break this, however...",
                pieceGroup: PieceClassification.Fortress
            )
        },
        {
            "Ashlands_Wall_2x2_edge",
            new PrefabConfig(
                name: "Ashlands_Wall_2x2_edge",
                enabled: true,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.Stonecutter),
                requirements: "Grausten,6;FlametalNew,1",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceName: "Grausten Wall Edge 2x2 (Reinforced) (1)",
                pieceDesc: "Nothing short of a siege should break this, however...",
                pieceGroup: PieceClassification.Fortress
            )
        },
        {
            "Ashlands_Wall_2x2_edge2",
            new PrefabConfig(
                name: "Ashlands_Wall_2x2_edge2",
                enabled: true,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.Stonecutter),
                requirements: "Grausten,6;FlametalNew,1",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceName: "Grausten Wall Edge 2x2 (Reinforced) (2)",
                pieceDesc: "Nothing short of a siege should break this, however...",
                pieceGroup: PieceClassification.Fortress
            )
        },
        {
            "Ashlands_Wall_2x2_edge2_top",
            new PrefabConfig(
                name: "Ashlands_Wall_2x2_edge2_top",
                enabled: true,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.Stonecutter),
                requirements: "Grausten,6;FlametalNew,1",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceName: "Grausten Wall Edge Top 2x2 (Reinforced) (2)",
                pieceDesc: "Nothing short of a siege should break this, however...",
                pieceGroup: PieceClassification.Fortress
            )
        },
        {
            "Ashlands_Wall_2x2_edge_top",
            new PrefabConfig(
                name: "Ashlands_Wall_2x2_edge_top",
                enabled: true,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.Stonecutter),
                requirements: "Grausten,6;FlametalNew,1",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceName: "Grausten Wall Edge Top 2x2 (Reinforced) (1)",
                pieceDesc: "Nothing short of a siege should break this, however...",
                pieceGroup: PieceClassification.Fortress
            )
        },
        {
            "Ashlands_Wall_2x2_top",
            new PrefabConfig(
                name: "Ashlands_Wall_2x2_top",
                enabled: true,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.Stonecutter),
                requirements: "Grausten,6;FlametalNew,1",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceName: "Grausten Wall Top 2x2 (Reinforced)",
                pieceDesc: "Nothing short of a siege should break this, however...",
                pieceGroup: PieceClassification.Fortress
            )
        },
        {
            "Ashlands_floor_large_fractured",
            new PrefabConfig(
                name: "Ashlands_floor_large_fractured",
                enabled: true,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.Stonecutter),
                requirements: "Grausten,125",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceName: "Grausten Floor 10x10",
                pieceGroup: PieceClassification.Grausten
            )
        },
        {
            "Beech1",
            new PrefabConfig(
                name: "Beech1",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Cultivator),
                category: PieceCategoryManager.Nature,
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
                pieceTable: nameof(PieceTables.Cultivator),
                category: PieceCategoryManager.Nature,
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
                pieceTable: nameof(PieceTables.Cultivator),
                category: PieceCategoryManager.Nature,
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
                pieceTable: nameof(PieceTables.Cultivator),
                category: PieceCategoryManager.Nature,
                craftingStation: nameof(CraftingStations.None),
                requirements: "Wood,1",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Flora
            )
        },
        {
            "BigBranch",
            new PrefabConfig(
                name: "BigBranch",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: true,  // not sure if this good
                placementPatch: false
            )
        },
        {
            "Birch1",
            new PrefabConfig(
                name: "Birch1",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Cultivator),
                category: PieceCategoryManager.Nature,
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
                pieceTable: nameof(PieceTables.Cultivator),
                category: PieceCategoryManager.Nature,
                craftingStation: nameof(CraftingStations.None),
                requirements: "BirchSeeds,1;FineWood,2",
                clipEverything: true,
                clipGround: false,
                placementPatch: false,
                pieceName: "Birch1 (Autumn)",
                pieceGroup: PieceClassification.Flora
            )
        },
        {
            "Birch2",
            new PrefabConfig(
                name: "Birch2",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Cultivator),
                category: PieceCategoryManager.Nature,
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
                pieceTable: nameof(PieceTables.Cultivator),
                category: PieceCategoryManager.Nature,
                craftingStation: nameof(CraftingStations.None),
                requirements: "BirchSeeds,1;FineWood,2",
                clipEverything: true,
                clipGround: false,
                placementPatch: false,
                pieceName: "Birch (Autumn)",
                pieceGroup: PieceClassification.Flora
            )
        },
        {
            "BirchStub",
            new PrefabConfig(
                name: "BirchStub",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Cultivator),
                category: PieceCategoryManager.Nature,
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
                pieceTable: nameof(PieceTables.Cultivator),
                category: PieceCategoryManager.Nature,
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
                pieceTable: nameof(PieceTables.Cultivator),
                category: PieceCategoryManager.Nature,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Flora
            )
        },
        {
            "BlackIceShard_01",
            new PrefabConfig(
                name: "BlackIceShard_01",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: true,
                placementPatch: false
            )
        },
        {
            "BlackIceShard_02",
            new PrefabConfig(
                name: "BlackIceShard_02",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: true,
                placementPatch: false
            )
        },
        {
            "BlackIce_Core",
            new PrefabConfig(
                name: "BlackIce_Core",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: false,
                placementPatch: true
            )
        },
        {
            "BlackIce_Core_outer",
            new PrefabConfig(
                name: "BlackIce_Core_outer",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: true,
                placementPatch: false
            )
        },
        {
            "BlackIce_Start",
            new PrefabConfig(
                name: "BlackIce_Start",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: false,
                placementPatch: true
            )
        },
        {
            "BlobMorkBig",
            new PrefabConfig(
                name: "BlobMorkBig",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "BlueberryBush",
            new PrefabConfig(
                name: "BlueberryBush",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Cultivator),
                category: PieceCategoryManager.Nature,
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
                enabled: true,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.Misc,
                craftingStation: nameof(CraftingStations.None),
                requirements: "PowderedDragonEgg,2;Stone,5",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                placementOffset: new Vector3(0f, -1f, 0)
            )
        },
        {
            "BonePileSpawner",
            new PrefabConfig(
                name: "BonePileSpawner",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "BonePileSpawner_swamp",
            new PrefabConfig(
                name: "BonePileSpawner_swamp",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
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
                pieceTable: nameof(PieceTables.Cultivator),
                category: PieceCategoryManager.Nature,
                craftingStation: nameof(CraftingStations.None),
                requirements: "Wood,2",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Flora
            )
        },
        {
            "Bush01_deepnorth",
            new PrefabConfig(
                name: "Bush01_deepnorth",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "Bush01_heath",
            new PrefabConfig(
                name: "Bush01_heath",
                enabled: true,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Cultivator),
                category: PieceCategoryManager.Nature,
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
                pieceTable: nameof(PieceTables.Cultivator),
                category: PieceCategoryManager.Nature,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.Misc,
                craftingStation: nameof(CraftingStations.Workbench),
                requirements: "Wood,2",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceName: "Wood Box",
                pieceGroup: PieceClassification.Misc
            )
        },
        {
            "CastleKit_groundtorch",
            new PrefabConfig(
                name: "CastleKit_groundtorch",
                enabled: true,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.Furniture,
                craftingStation: nameof(CraftingStations.Forge),
                requirements: "Iron,2;Resin,2;SurtlingCore,1",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceName: "Standing Iron Torch (Eternal)",
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.Furniture,
                craftingStation: nameof(CraftingStations.Forge),
                requirements: "Iron,2;GreydwarfEye,2;SurtlingCore,1",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceName: "Standing Blue-Burning Iron Torch (Eternal)",
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.Furniture,
                craftingStation: nameof(CraftingStations.Forge),
                requirements: "Iron,2;Guck,2;SurtlingCore,1",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceName: "Standing Green-Burning Iron Torch (Eternal)",
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
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
                category: PieceCategoryManager.CreatorShop,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.Furniture,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
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
                pieceTable: nameof(PieceTables.Cultivator),
                category: PieceCategoryManager.Nature,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.Nature,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.Nature,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.Nature,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.Nature,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.Nature,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.Nature,
                craftingStation: nameof(CraftingStations.None),
                requirements: "YggdrasilWood,2",
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "Enemy_Barka_Attack_BackSlam",
            new PrefabConfig(
                name: "Enemy_Barka_Attack_BackSlam",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "Enemy_Barka_Attack_HeavyImpact",
            new PrefabConfig(
                name: "Enemy_Barka_Attack_HeavyImpact",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "Enemy_Barka_Attack_HeavySwings",
            new PrefabConfig(
                name: "Enemy_Barka_Attack_HeavySwings",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "Enemy_Barka_Attack_SlamDrive",
            new PrefabConfig(
                name: "Enemy_Barka_Attack_SlamDrive",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "Enemy_Barka_Attack_WhipFlurry",
            new PrefabConfig(
                name: "Enemy_Barka_Attack_WhipFlurry",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "Enemy_Barka_Attack_WhipSlam",
            new PrefabConfig(
                name: "Enemy_Barka_Attack_WhipSlam",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "Enemy_Barka_Death",
            new PrefabConfig(
                name: "Enemy_Barka_Death",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "Enemy_Barka_Death_Debris",
            new PrefabConfig(
                name: "Enemy_Barka_Death_Debris",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "Enemy_Barka_Footstep",
            new PrefabConfig(
                name: "Enemy_Barka_Footstep",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "Enemy_Barka_Hurt",
            new PrefabConfig(
                name: "Enemy_Barka_Hurt",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "Enemy_Barka_Idle",
            new PrefabConfig(
                name: "Enemy_Barka_Idle",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "Enemy_Barka_Verse_Death",
            new PrefabConfig(
                name: "Enemy_Barka_Verse_Death",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "EvilHeart_Forest",
            new PrefabConfig(
                name: "EvilHeart_Forest",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "EvilHeart_Swamp",
            new PrefabConfig(
                name: "EvilHeart_Swamp",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: false,
                placementPatch: true
            )
        },
        {
            "FimbulvinterOrb",
            new PrefabConfig(
                name: "FimbulvinterOrb",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "FimbulvinterOrb_start",
            new PrefabConfig(
                name: "FimbulvinterOrb_start",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "FirTree",
            new PrefabConfig(
                name: "FirTree",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Cultivator),
                category: PieceCategoryManager.Nature,
                craftingStation: nameof(CraftingStations.None),
                requirements: "Wood,16",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Flora
            )
        },
        {
            "FirTree_Big_log",
            new PrefabConfig(
                name: "FirTree_Big_log",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "FirTree_Big_plantable_Stub",
            new PrefabConfig(
                name: "FirTree_Big_plantable_Stub",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "FirTree_Snow_Stub",
            new PrefabConfig(
                name: "FirTree_Snow_Stub",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "FirTree_Snow_log",
            new PrefabConfig(
                name: "FirTree_Snow_log",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "FirTree_Snow_log_half",
            new PrefabConfig(
                name: "FirTree_Snow_log_half",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "FirTree_Stub",
            new PrefabConfig(
                name: "FirTree_Stub",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Cultivator),
                category: PieceCategoryManager.Nature,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: true,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Flora
            )
        },
        {
            "FirTree_big",
            new PrefabConfig(
                name: "FirTree_big",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "FirTree_big_log_half",
            new PrefabConfig(
                name: "FirTree_big_log_half",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "FirTree_log",
            new PrefabConfig(
                name: "FirTree_log",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Cultivator),
                category: PieceCategoryManager.Nature,
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
                pieceTable: nameof(PieceTables.Cultivator),
                category: PieceCategoryManager.Nature,
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
                pieceTable: nameof(PieceTables.Cultivator),
                category: PieceCategoryManager.Nature,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Flora
            )
        },
        {
            "FirTree_oldLog_deepnorth",
            new PrefabConfig(
                name: "FirTree_oldLog_deepnorth",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "FirTree_small",
            new PrefabConfig(
                name: "FirTree_small",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Cultivator),
                category: PieceCategoryManager.Nature,
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
                pieceTable: nameof(PieceTables.Cultivator),
                category: PieceCategoryManager.Nature,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: false,
                placementPatch: false
                //placementOffset: new Vector3(1.166f, 0f, -0.575f)  // reverse offset in the prefab to center it
            )
        },
        {
            "FireFlies",
            new PrefabConfig(
                name: "FireFlies",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
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
                pieceTable: nameof(PieceTables.Hoe),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Rock
            )
        },
        {
            "FrostWisp_Storm",
            new PrefabConfig(
                name: "FrostWisp_Storm",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "FrozenGD",
            new PrefabConfig(
                name: "FrozenGD",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "FrozenKing_Summon",
            new PrefabConfig(
                name: "FrozenKing_Summon",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "FrozenSkeleton_Pose1",
            new PrefabConfig(
                name: "FrozenSkeleton_Pose1",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "FrozenSkeleton_Pose2",
            new PrefabConfig(
                name: "FrozenSkeleton_Pose2",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "GhostSkull",
            new PrefabConfig(
                name: "GhostSkull",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
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
                pieceTable: nameof(PieceTables.Cultivator),
                category: PieceCategoryManager.Nature,
                craftingStation: nameof(CraftingStations.None),
                requirements: "Ooze,1",
                clipEverything: true,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Flora
            )
        },
        {
            "GrapplingPoint",
            new PrefabConfig(
                name: "GrapplingPoint",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: false,
                placementPatch: true
            )
        },
        {
            "GrapplingPointSecondary",
            new PrefabConfig(
                name: "GrapplingPointSecondary",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: false,
                placementPatch: true
            )
        },
        {
            "GraveStone_Broken_CharredTwitcherNest",
            new PrefabConfig(
                name: "GraveStone_Broken_CharredTwitcherNest",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "GreydwarfSurprise",
            new PrefabConfig(
                name: "GreydwarfSurprise",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
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
                pieceTable: nameof(PieceTables.Cultivator),
                category: PieceCategoryManager.Nature,
                craftingStation: nameof(CraftingStations.None),
                requirements: "Wood,2",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Flora
            )
        },
        {
            "Greydwarf_Surprise",
            new PrefabConfig(
                name: "Greydwarf_Surprise",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
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
                pieceTable: nameof(PieceTables.Cultivator),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "Guck,12",
                clipEverything: true,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Flora
            )
        },
        {
            "GuckSack_small",
            new PrefabConfig(
                name: "GuckSack_small",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Cultivator),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "Guck,6",
                clipEverything: true,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Flora
            )
        },
        {
            "HoleRock_opening_znet",
            new PrefabConfig(
                name: "HoleRock_opening_znet",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: true,
                placementPatch: false
            )
        },
        {
            "HoleRock_root1",
            new PrefabConfig(
                name: "HoleRock_root1",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "HoleRock_rootBush1",
            new PrefabConfig(
                name: "HoleRock_rootBush1",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "HoleRock_rootFloor1",
            new PrefabConfig(
                name: "HoleRock_rootFloor1",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "HoleRock_rootWall1",
            new PrefabConfig(
                name: "HoleRock_rootWall1",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
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
                pieceTable: nameof(PieceTables.Cultivator),
                category: PieceCategoryManager.Nature,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "IcePond_rock_frac",
            new PrefabConfig(
                name: "IcePond_rock_frac",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: true,
                placementPatch: false
            )
        },
        {
            "IceShard_01",
            new PrefabConfig(
                name: "IceShard_01",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: true,
                placementPatch: false
            )
        },
        {
            "IceShard_02",
            new PrefabConfig(
                name: "IceShard_02",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: true,
                placementPatch: false
            )
        },
        {
            "IceShard_03",
            new PrefabConfig(
                name: "IceShard_03",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: true,
                placementPatch: false
            )
        },
        {
            "IceShard_04",
            new PrefabConfig(
                name: "IceShard_04",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: true,
                placementPatch: false
            )
        },
        {
            "IceShard_05",
            new PrefabConfig(
                name: "IceShard_05",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: true,
                placementPatch: false
            )
        },
        {
            "IceShard_06",
            new PrefabConfig(
                name: "IceShard_06",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: true,
                placementPatch: false
            )
        },
        {
            "IceShelf_01",
            new PrefabConfig(
                name: "IceShelf_01",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: true,
                placementPatch: false
            )
        },
        {
            "IceShelf_02",
            new PrefabConfig(
                name: "IceShelf_02",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: true,
                placementPatch: false
            )
        },
        {
            "IceShelf_03",
            new PrefabConfig(
                name: "IceShelf_03",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: true,
                placementPatch: false
            )
        },
        {
            "IceShelf_04",
            new PrefabConfig(
                name: "IceShelf_04",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: true,
                placementPatch: false
            )
        },
        {
            "IceShelf_05",
            new PrefabConfig(
                name: "IceShelf_05",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: true,
                placementPatch: false
            )
        },
        {
            "IceShelf_06",
            new PrefabConfig(
                name: "IceShelf_06",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: true,
                placementPatch: false
            )
        },
        {
            "IceShelf_07",
            new PrefabConfig(
                name: "IceShelf_07",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: true,
                placementPatch: false
            )
        },
        {
            "IceShelf_08",
            new PrefabConfig(
                name: "IceShelf_08",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: true,
                placementPatch: false
            )
        },
        {
            "IceShelf_09",
            new PrefabConfig(
                name: "IceShelf_09",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: true,
                placementPatch: false
            )
        },
        {
            "IceShelf_10",
            new PrefabConfig(
                name: "IceShelf_10",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: true,
                placementPatch: false
            )
        },
        {
            "IceShoreShard",
            new PrefabConfig(
                name: "IceShoreShard",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "IceShore_frac",
            new PrefabConfig(
                name: "IceShore_frac",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "IceWall",
            new PrefabConfig(
                name: "IceWall",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "Crystal,16",
                clipEverything: false,
                clipGround: true,
                placementPatch: false
            )
        },
        {
            "Ice_ship_1",
            new PrefabConfig(
                name: "Ice_ship_1",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: true,
                placementPatch: false
            )
        },
        {
            "Ice_ship_2",
            new PrefabConfig(
                name: "Ice_ship_2",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: true,
                placementPatch: false
            )
        },
        {
            "Ice_ship_3",
            new PrefabConfig(
                name: "Ice_ship_3",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: true,
                placementPatch: false
            )
        },
        {
            "Ice_ship_4",
            new PrefabConfig(
                name: "Ice_ship_4",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: true,
                placementPatch: false
            )
        },
        {
            "Ice_ship_5",
            new PrefabConfig(
                name: "Ice_ship_5",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: true,
                placementPatch: false
            )
        },
        {
            "Ice_ship_6",
            new PrefabConfig(
                name: "Ice_ship_6",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: true,
                placementPatch: false
            )
        },
        {
            "Ice_ship_7",
            new PrefabConfig(
                name: "Ice_ship_7",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: true,
                placementPatch: false
            )
        },
        {
            "LargeBone",
            new PrefabConfig(
                name: "LargeBone",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: true,
                placementPatch: false
            )
        },
        {
            "LargeBone_half01",
            new PrefabConfig(
                name: "LargeBone_half01",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "LargeBone_half02",
            new PrefabConfig(
                name: "LargeBone_half02",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "LavaRock",
            new PrefabConfig(
                name: "LavaRock",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hoe),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: true,
                clipGround: false,
                placementPatch: true,
                pieceGroup: PieceClassification.Rock
            )
        },
        {
            "Leviathan",
            new PrefabConfig(
                name: "Leviathan",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
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
                pieceTable: nameof(PieceTables.Hoe),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: true,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Ore
            )
        },
        {
            "LingonberryBush",
            new PrefabConfig(
                name: "LingonberryBush",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "LuredFaderEmber",
            new PrefabConfig(
                name: "LuredFaderEmber",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "Wisp,1",
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "MemorialStone_Large",
            new PrefabConfig(
                name: "MemorialStone_Large",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "MemorialStone_Medium",
            new PrefabConfig(
                name: "MemorialStone_Medium",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "MemorialStone_Small",
            new PrefabConfig(
                name: "MemorialStone_Small",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
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
                pieceTable: nameof(PieceTables.Hoe),
                category: PieceCategoryManager.Nature,
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
                pieceTable: nameof(PieceTables.Hoe),
                category: PieceCategoryManager.Nature,
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
                pieceTable: nameof(PieceTables.Hoe),
                category: PieceCategoryManager.Nature,
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
                pieceTable: nameof(PieceTables.Hoe),
                category: PieceCategoryManager.Nature,
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
                pieceTable: nameof(PieceTables.Hoe),
                category: PieceCategoryManager.Nature,
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
                pieceTable: nameof(PieceTables.Hoe),
                category: PieceCategoryManager.Nature,
                craftingStation: nameof(CraftingStations.None),
                requirements: "TinOre,4",
                clipEverything: true,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Ore
            )
        },
        {
            "Morkborg_gate",
            new PrefabConfig(
                name: "Morkborg_gate",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "Morkhalla_Banner1",
            new PrefabConfig(
                name: "Morkhalla_Banner1",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "Morkhalla_Banner2",
            new PrefabConfig(
                name: "Morkhalla_Banner2",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "Morkhalla_Bedroll1",
            new PrefabConfig(
                name: "Morkhalla_Bedroll1",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "Morkhalla_Bedroll2",
            new PrefabConfig(
                name: "Morkhalla_Bedroll2",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "Morkhalla_Bench",
            new PrefabConfig(
                name: "Morkhalla_Bench",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "Morkhalla_Block",
            new PrefabConfig(
                name: "Morkhalla_Block",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "Morkhalla_Chain",
            new PrefabConfig(
                name: "Morkhalla_Chain",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "Morkhalla_ChestAncient",
            new PrefabConfig(
                name: "Morkhalla_ChestAncient",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "Morkhalla_Drawbridge",
            new PrefabConfig(
                name: "Morkhalla_Drawbridge",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "Morkhalla_Eye1",
            new PrefabConfig(
                name: "Morkhalla_Eye1",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "Morkhalla_Eye2",
            new PrefabConfig(
                name: "Morkhalla_Eye2",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "Morkhalla_Eye3",
            new PrefabConfig(
                name: "Morkhalla_Eye3",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "Morkhalla_Eye4",
            new PrefabConfig(
                name: "Morkhalla_Eye4",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "Morkhalla_Eye5_gemstone",
            new PrefabConfig(
                name: "Morkhalla_Eye5_gemstone",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "Morkhalla_Eye6_gemstone",
            new PrefabConfig(
                name: "Morkhalla_Eye6_gemstone",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "Morkhalla_Eye7_gemstone",
            new PrefabConfig(
                name: "Morkhalla_Eye7_gemstone",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "Morkhalla_Floor_2x2",
            new PrefabConfig(
                name: "Morkhalla_Floor_2x2",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "Morkhalla_Floor_4x4",
            new PrefabConfig(
                name: "Morkhalla_Floor_4x4",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "Morkhalla_Floor_4x4_broken01",
            new PrefabConfig(
                name: "Morkhalla_Floor_4x4_broken01",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "Morkhalla_Floor_4x4_broken02",
            new PrefabConfig(
                name: "Morkhalla_Floor_4x4_broken02",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "Morkhalla_GateDoor",
            new PrefabConfig(
                name: "Morkhalla_GateDoor",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "Morkhalla_GateDoor02",
            new PrefabConfig(
                name: "Morkhalla_GateDoor02",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "Morkhalla_GateDoor03",
            new PrefabConfig(
                name: "Morkhalla_GateDoor03",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "Morkhalla_MetalBar",
            new PrefabConfig(
                name: "Morkhalla_MetalBar",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "Morkhalla_Rubble1",
            new PrefabConfig(
                name: "Morkhalla_Rubble1",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "Morkhalla_Rubble1_fall",
            new PrefabConfig(
                name: "Morkhalla_Rubble1_fall",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: true,
                placementPatch: false
            )
        },
        {
            "Morkhalla_Rubble2",
            new PrefabConfig(
                name: "Morkhalla_Rubble2",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "Morkhalla_Rubble2_fall",
            new PrefabConfig(
                name: "Morkhalla_Rubble2_fall",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: true,
                placementPatch: false
            )
        },
        {
            "Morkhalla_Rubble3",
            new PrefabConfig(
                name: "Morkhalla_Rubble3",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "Morkhalla_Rubble4",
            new PrefabConfig(
                name: "Morkhalla_Rubble4",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "Morkhalla_Rug_corner",
            new PrefabConfig(
                name: "Morkhalla_Rug_corner",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "Morkhalla_Rug_end1",
            new PrefabConfig(
                name: "Morkhalla_Rug_end1",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "Morkhalla_Rug_end2",
            new PrefabConfig(
                name: "Morkhalla_Rug_end2",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "Morkhalla_Rug_middle",
            new PrefabConfig(
                name: "Morkhalla_Rug_middle",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "Morkhalla_Rug_stair",
            new PrefabConfig(
                name: "Morkhalla_Rug_stair",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: false,
                placementPatch: true
            )
        },
        {
            "Morkhalla_Stairs_giant_railing",
            new PrefabConfig(
                name: "Morkhalla_Stairs_giant_railing",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "Morkhalla_Stairs_giant_short",
            new PrefabConfig(
                name: "Morkhalla_Stairs_giant_short",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: true,
                placementPatch: false
            )
        },
        {
            "Morkhalla_Stairs_giant_short_broken1",
            new PrefabConfig(
                name: "Morkhalla_Stairs_giant_short_broken1",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: true,
                placementPatch: false
            )
        },
        {
            "Morkhalla_Stairs_giant_short_broken2",
            new PrefabConfig(
                name: "Morkhalla_Stairs_giant_short_broken2",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: true,
                placementPatch: false
            )
        },
        {
            "Morkhalla_StatuePieceArmL",
            new PrefabConfig(
                name: "Morkhalla_StatuePieceArmL",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: true,
                placementPatch: false
                //placementOffset: new Vector3(0f, 0f, 0f)
            )
        },
        {
            "Morkhalla_StatuePieceArmL_big",
            new PrefabConfig(
                name: "Morkhalla_StatuePieceArmL_big",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: true,
                placementPatch: false
                //placementOffset: new Vector3(0f, 0f, 0f)
            )
        },
        {
            "Morkhalla_StatuePieceArmR",
            new PrefabConfig(
                name: "Morkhalla_StatuePieceArmR",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: true,
                placementPatch: false
                //placementOffset: new Vector3(0f, 0f, 0f)
            )
        },
        {
            "Morkhalla_StatuePieceArmR_big",
            new PrefabConfig(
                name: "Morkhalla_StatuePieceArmR_big",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: true,
                placementPatch: false
                //placementOffset: new Vector3(0f, 0f, 0f)
            )
        },
        {
            "Morkhalla_StatuePieceFace",
            new PrefabConfig(
                name: "Morkhalla_StatuePieceFace",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: true,
                placementPatch: false
                //placementOffset: new Vector3(0f, 0f, 0f)
            )
        },
        {
            "Morkhalla_StatuePieceFace_big",
            new PrefabConfig(
                name: "Morkhalla_StatuePieceFace_big",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: true,
                placementPatch: false
                //placementOffset: new Vector3(0f, 0f, 0f)
            )
        },
        {
            "Morkhalla_StatuePieceFeet",
            new PrefabConfig(
                name: "Morkhalla_StatuePieceFeet",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: true,
                placementPatch: false
                //placementOffset: new Vector3(0f, 0f, 0f)
            )
        },
        {
            "Morkhalla_StatuePieceFeet_big",
            new PrefabConfig(
                name: "Morkhalla_StatuePieceFeet_big",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: true,
                placementPatch: false
                //placementOffset: new Vector3(0f, 0f, 0f)
            )
        },
        {
            "Morkhalla_StatuePieceHorn",
            new PrefabConfig(
                name: "Morkhalla_StatuePieceHorn",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: true,
                placementPatch: false
                //placementOffset: new Vector3(-0.2f, -7f, -0.45f)
            )
        },
        {
            "Morkhalla_StatuePieceHorn2",
            new PrefabConfig(
                name: "Morkhalla_StatuePieceHorn2",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: true,
                placementPatch: false
                //placementOffset: new Vector3(0f, -5f, 0f)
            )
        },
        {
            "Morkhalla_StatuePieceHorn2_big",
            new PrefabConfig(
                name: "Morkhalla_StatuePieceHorn2_big",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: true,
                placementPatch: false
                //placementOffset: new Vector3(0f, -5f, 0f)
            )
        },
        {
            "Morkhalla_StatuePieceHorn_big",
            new PrefabConfig(
                name: "Morkhalla_StatuePieceHorn_big",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: true,
                placementPatch: false
                //placementOffset: new Vector3(0f, -5f, 0f)
            )
        },
        {
            "Morkhalla_StatuePieceLegs",
            new PrefabConfig(
                name: "Morkhalla_StatuePieceLegs",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: true,
                placementPatch: false
                //placementOffset: new Vector3(0f, -5f, 0f)
            )
        },
        {
            "Morkhalla_StatuePieceLegs_big",
            new PrefabConfig(
                name: "Morkhalla_StatuePieceLegs_big",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: true,
                placementPatch: false
                //placementOffset: new Vector3(0f, -5f, 0f)
            )
        },
        {
            "Morkhalla_StatuePieceSword",
            new PrefabConfig(
                name: "Morkhalla_StatuePieceSword",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: true,
                placementPatch: false
                //placementOffset: new Vector3(0f, -5f, 0f)
            )
        },
        {
            "Morkhalla_StatuePieceSword_big",
            new PrefabConfig(
                name: "Morkhalla_StatuePieceSword_big",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: true,
                placementPatch: false
                //placementOffset: new Vector3(0f, -5f, 0f)
            )
        },
        {
            "Morkhalla_StatuePieceTorso",
            new PrefabConfig(
                name: "Morkhalla_StatuePieceTorso",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: true,
                placementPatch: false
                //placementOffset: new Vector3(0f, -5f, 0f)
            )
        },
        {
            "Morkhalla_StatuePieceTorso_big",
            new PrefabConfig(
                name: "Morkhalla_StatuePieceTorso_big",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: true,
                placementPatch: false
                //placementOffset: new Vector3(0f, -5f, 0f)
            )
        },
        {
            "Morkhalla_StatueSword",
            new PrefabConfig(
                name: "Morkhalla_StatueSword",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: false,
                placementPatch: false
                //placementOffset: new Vector3(0f, 0f, 0f)
            )
        },
        {
            "Morkhalla_StatueSword_hanging",
            new PrefabConfig(
                name: "Morkhalla_StatueSword_hanging",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: false,
                placementPatch: false
                //placementOffset: new Vector3(0f, 0f, 0f)
            )
        },
        {
            "Morkhalla_Stonepile",
            new PrefabConfig(
                name: "Morkhalla_Stonepile",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: false,
                placementPatch: false
                //placementOffset: new Vector3(1.91f, -50f, 3.279f)
            )
        },
        {
            "Morkhalla_Stool",
            new PrefabConfig(
                name: "Morkhalla_Stool",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "Morkhalla_Table",
            new PrefabConfig(
                name: "Morkhalla_Table",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: false,
                placementPatch: true
            )
        },
        {
            "Morkhalla_Trainingdummy1",
            new PrefabConfig(
                name: "Morkhalla_Trainingdummy1",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: true,
                placementPatch: false
            )
        },
        {
            "Morkhalla_Trainingdummy2",
            new PrefabConfig(
                name: "Morkhalla_Trainingdummy2",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: true,
                placementPatch: false
            )
        },
        {
            "Morkhalla_WallChain1",
            new PrefabConfig(
                name: "Morkhalla_WallChain1",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                placementOffset: new Vector3(0f, -50f, 0f)
            )
        },
        {
            "Morkhalla_WeaponStand",
            new PrefabConfig(
                name: "Morkhalla_WeaponStand",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "Morkhalla_WoodBoards",
            new PrefabConfig(
                name: "Morkhalla_WoodBoards",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "Morkhalla_bridge_davinci",
            new PrefabConfig(
                name: "Morkhalla_bridge_davinci",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "Morkhalla_coal_pile_memorial",
            new PrefabConfig(
                name: "Morkhalla_coal_pile_memorial",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "Morkhalla_firepit",
            new PrefabConfig(
                name: "Morkhalla_firepit",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "Morkhalla_giant_railing",
            new PrefabConfig(
                name: "Morkhalla_giant_railing",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "Morkhalla_giant_railing_corner",
            new PrefabConfig(
                name: "Morkhalla_giant_railing_corner",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "Morkhalla_giant_railing_deco",
            new PrefabConfig(
                name: "Morkhalla_giant_railing_deco",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "Morkhalla_giant_railing_half",
            new PrefabConfig(
                name: "Morkhalla_giant_railing_half",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "Morkhalla_giant_railing_single",
            new PrefabConfig(
                name: "Morkhalla_giant_railing_single",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: true,
                placementPatch: false
            )
        },
        {
            "Morkhalla_giant_railing_torch",
            new PrefabConfig(
                name: "Morkhalla_giant_railing_torch",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "Morkhalla_giant_railing_torch_unlit",
            new PrefabConfig(
                name: "Morkhalla_giant_railing_torch_unlit",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "Morkhalla_jotun_gate",
            new PrefabConfig(
                name: "Morkhalla_jotun_gate",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "Morkhalla_rubble_trashpile",
            new PrefabConfig(
                name: "Morkhalla_rubble_trashpile",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "MountainGraveStone01",
            new PrefabConfig(
                name: "MountainGraveStone01",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.Furniture,
                craftingStation: nameof(CraftingStations.Forge),
                requirements: "Bronze,5;Coal,2;BlackCore,1;WolfClaw,3",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceName: "Standing Brazier (Eternal)",
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.Furniture,
                craftingStation: nameof(CraftingStations.Forge),
                requirements: "Bronze,5;GreydwarfEye,2;BlackCore,1;WolfClaw,3",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceName: "Blue Standing Brazier (Eternal)",
                pieceDesc: "Burns eternally without fuel.",
                pieceGroup: PieceClassification.Brazier,
                playerBasePatch: true
            )
        },
        {
            "MountainKit_brazier_purple",
            new PrefabConfig(
                name: "MountainKit_brazier_purple",
                enabled: true,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.Furniture,
                craftingStation: nameof(CraftingStations.Forge),
                requirements: "Bronze,5;CharcoalResin,2;BlackCore,1;WolfClaw,3",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceName: "Purple Standing Brazier (Eternal)",
                pieceDesc: "Burns eternally without fuel."
            )
        },
        {
            "MountainKit_wood_gate",
            new PrefabConfig(
                name: "MountainKit_wood_gate",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.Building,
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
                pieceTable: nameof(PieceTables.Cultivator),
                category: PieceCategoryManager.Nature,
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
                pieceTable: nameof(PieceTables.Cultivator),
                category: PieceCategoryManager.Nature,
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
                pieceTable: nameof(PieceTables.Cultivator),
                category: PieceCategoryManager.Nature,
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
                pieceTable: nameof(PieceTables.Cultivator),
                category: PieceCategoryManager.Nature,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.Nature,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
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
                pieceTable: nameof(PieceTables.Hoe),
                category: PieceCategoryManager.CreatorShop,
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
                pieceTable: nameof(PieceTables.Cultivator),
                category: PieceCategoryManager.Nature,
                craftingStation: nameof(CraftingStations.None),
                requirements: "Wood,5",
                clipEverything: true,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Flora
            )
        },
        {
            "Pickable_Branch_Snow",
            new PrefabConfig(
                name: "Pickable_Branch_Snow",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: true,
                placementPatch: false
            )
        },
        {
            "Pickable_Carrot",
            new PrefabConfig(
                name: "Pickable_Carrot",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.Nature,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: true,
                placementPatch: false
            )
        },
        {
            "Pickable_Dandelion",
            new PrefabConfig(
                name: "Pickable_Dandelion",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Cultivator),
                category: PieceCategoryManager.Nature,
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
                category: PieceCategoryManager.CreatorShop,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.Nature,
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
                pieceTable: nameof(PieceTables.Cultivator),
                category: PieceCategoryManager.Nature,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: true,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Treasure
            )
        },
        {
            "Pickable_FrostCoreHanger",
            new PrefabConfig(
                name: "Pickable_FrostCoreHanger",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "Pickable_GlowWorm",
            new PrefabConfig(
                name: "Pickable_GlowWorm",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: true,
                placementPatch: false
            )
        },
        {
            "Pickable_Hairstrands01",
            new PrefabConfig(
                name: "Pickable_Hairstrands01",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Treasure
            )
        },
        {
            "Pickable_HardRockOffspring",
            new PrefabConfig(
                name: "Pickable_HardRockOffspring",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: true,
                placementPatch: false
            )
        },
        {
            "Pickable_Item",
            new PrefabConfig(
                name: "Pickable_Item",
                enabled: false,
                allowedInDungeons: false,
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                pieceName: "Coins (Pickable)",
                pieceGroup: PieceClassification.Treasure
            )
        },
        {
            "Pickable_Kale",
            new PrefabConfig(
                name: "Pickable_Kale",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "Pickable_MeatPile",
            new PrefabConfig(
                name: "Pickable_MeatPile",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
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
                pieceTable: nameof(PieceTables.Hoe),
                category: PieceCategoryManager.Nature,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "Pickable_MorkHallaTreasure",
            new PrefabConfig(
                name: "Pickable_MorkHallaTreasure",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: true,
                placementPatch: false
            )
        },
        {
            "Pickable_MountainCaveCrystal",
            new PrefabConfig(
                name: "Pickable_MountainCaveCrystal",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hoe),
                category: PieceCategoryManager.Nature,
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
                pieceTable: nameof(PieceTables.Hoe),
                category: PieceCategoryManager.Nature,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
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
                pieceTable: nameof(PieceTables.Cultivator),
                category: PieceCategoryManager.Nature,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.Nature,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.Nature,
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
                pieceTable: nameof(PieceTables.Cultivator),
                category: PieceCategoryManager.Nature,
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
                pieceTable: nameof(PieceTables.Cultivator),
                category: PieceCategoryManager.Nature,
                craftingStation: nameof(CraftingStations.None),
                requirements: "MushroomYellow,5",
                clipEverything: true,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Plant
            )
        },
        {
            "Pickable_Oat",
            new PrefabConfig(
                name: "Pickable_Oat",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "Pickable_Obsidian",
            new PrefabConfig(
                name: "Pickable_Obsidian",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hoe),
                category: PieceCategoryManager.Nature,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.Nature,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: true,
                placementPatch: false
            )
        },
        {
            "Pickable_Poteitr",
            new PrefabConfig(
                name: "Pickable_Poteitr",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.Nature,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: true,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.VanillaCrop
            )
        },
        {
            "Pickable_SeedKale",
            new PrefabConfig(
                name: "Pickable_SeedKale",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "Pickable_SeedOnion",
            new PrefabConfig(
                name: "Pickable_SeedOnion",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.Nature,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.Nature,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "Pickable_Snowball",
            new PrefabConfig(
                name: "Pickable_Snowball",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: true,
                placementPatch: false
            )
        },
        {
            "Pickable_Stone",
            new PrefabConfig(
                name: "Pickable_Stone",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Cultivator),
                category: PieceCategoryManager.Nature,
                craftingStation: nameof(CraftingStations.None),
                requirements: "Stone,5",
                clipEverything: true,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Flora
            )
        },
        {
            "Pickable_StoneRock",
            new PrefabConfig(
                name: "Pickable_StoneRock",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: true,
                placementPatch: false
            )
        },
        {
            "Pickable_SulfurRock",
            new PrefabConfig(
                name: "Pickable_SulfurRock",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: true,
                placementPatch: false
            )
        },
        {
            "Pickable_Swordpiece2",
            new PrefabConfig(
                name: "Pickable_Swordpiece2",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: true,
                placementPatch: false
            )
        },
        {
            "Pickable_Swordpiece3",
            new PrefabConfig(
                name: "Pickable_Swordpiece3",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: true,
                placementPatch: false
            )
        },
        {
            "Pickable_Tar",
            new PrefabConfig(
                name: "Pickable_Tar",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Cultivator),
                category: PieceCategoryManager.Nature,
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
                pieceTable: nameof(PieceTables.Cultivator),
                category: PieceCategoryManager.Nature,
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
                pieceTable: nameof(PieceTables.Cultivator),
                category: PieceCategoryManager.Nature,
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
                pieceTable: nameof(PieceTables.Hoe),
                category: PieceCategoryManager.Nature,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.Nature,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: true,
                placementPatch: false
            )
        },
        {
            "PineTree",
            new PrefabConfig(
                name: "PineTree",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Cultivator),
                category: PieceCategoryManager.Nature,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: true,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Flora
            )
        },
        {
            "PineTree_Snow_log",
            new PrefabConfig(
                name: "PineTree_Snow_log",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "PineTree_Snow_log_XL",
            new PrefabConfig(
                name: "PineTree_Snow_log_XL",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "PineTree_Snow_log_XL_half",
            new PrefabConfig(
                name: "PineTree_Snow_log_XL_half",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "PineTree_Snow_log_half",
            new PrefabConfig(
                name: "PineTree_Snow_log_half",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "PineTree_Snow_log_half_frost_troll",
            new PrefabConfig(
                name: "PineTree_Snow_log_half_frost_troll",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "PineTree_log",
            new PrefabConfig(
                name: "PineTree_log",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Cultivator),
                category: PieceCategoryManager.Nature,
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
                pieceTable: nameof(PieceTables.Cultivator),
                category: PieceCategoryManager.Nature,
                craftingStation: nameof(CraftingStations.None),
                requirements: "Wood,5;RoundLog,5",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Flora
            )
        },
        {
            "PineTree_snowfall",
            new PrefabConfig(
                name: "PineTree_snowfall",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "Pinetree_01",
            new PrefabConfig(
                name: "Pinetree_01",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Cultivator),
                category: PieceCategoryManager.Nature,
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
                pieceTable: nameof(PieceTables.Cultivator),
                category: PieceCategoryManager.Nature,
                craftingStation: nameof(CraftingStations.None),
                requirements: "Wood,5;RoundLog,5",
                clipEverything: true,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Flora
            )
        },
        {
            "Pinetree_Snow",
            new PrefabConfig(
                name: "Pinetree_Snow",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "Pinetree_Snow_Stub",
            new PrefabConfig(
                name: "Pinetree_Snow_Stub",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "Pinetree_Snow_dead",
            new PrefabConfig(
                name: "Pinetree_Snow_dead",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "PropFeastDeepNorth",
            new PrefabConfig(
                name: "PropFeastDeepNorth",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "RaspberryBush",
            new PrefabConfig(
                name: "RaspberryBush",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Cultivator),
                category: PieceCategoryManager.Nature,
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
                pieceTable: nameof(PieceTables.Hoe),
                category: PieceCategoryManager.Nature,
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
                pieceTable: nameof(PieceTables.Hoe),
                category: PieceCategoryManager.Nature,
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
                pieceTable: nameof(PieceTables.Hoe),
                category: PieceCategoryManager.Nature,
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
                pieceTable: nameof(PieceTables.Hoe),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "Stone,350",
                clipEverything: true,
                pieceGroup: PieceClassification.Rock
            )
        },
        {
            "RockFingerBroken",
            new PrefabConfig(
                name: "RockFingerBroken",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hoe),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "Stone,175",
                clipEverything: true,
                pieceGroup: PieceClassification.Rock
            )
        },
        {
            "RockFingerBroken_frac",
            new PrefabConfig(
                name: "RockFingerBroken_frac",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hoe),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "Stone,175",
                clipEverything: true,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Rock
            )
        },
        {
            "RockFinger_frac",
            new PrefabConfig(
                name: "RockFinger_frac",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hoe),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "Stone,350",
                clipEverything: true,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Rock
            )
        },
        {
            "RockThumb",
            new PrefabConfig(
                name: "RockThumb",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hoe),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "Stone,200",
                clipEverything: true,
                pieceGroup: PieceClassification.Rock
            )
        },
        {
            "RockThumb_frac",
            new PrefabConfig(
                name: "RockThumb_frac",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hoe),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "Stone,200",
                clipEverything: true,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Rock
            )
        },
        {
            "Rock_3",
            new PrefabConfig(
                name: "Rock_3",
                enabled: true,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hoe),
                category: PieceCategoryManager.Nature,
                craftingStation: nameof(CraftingStations.None),
                requirements: "Stone,24",
                clipEverything: true,
                pieceGroup: PieceClassification.Rock
            )
        },
        {
            "Rock_3_deepnorth_frac",
            new PrefabConfig(
                name: "Rock_3_deepnorth_frac",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: true,
                placementPatch: false
            )
        },
        {
            "Rock_3_frac",
            new PrefabConfig(
                name: "Rock_3_frac",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hoe),
                category: PieceCategoryManager.CreatorShop,
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
                pieceTable: nameof(PieceTables.Hoe),
                category: PieceCategoryManager.Nature,
                craftingStation: nameof(CraftingStations.None),
                requirements: "Stone,16",
                clipEverything: true,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Rock
            )
        },
        {
            "Rock_4_deepnorth",
            new PrefabConfig(
                name: "Rock_4_deepnorth",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: true,
                placementPatch: false
            )
        },
        {
            "Rock_4_plains",
            new PrefabConfig(
                name: "Rock_4_plains",
                enabled: true,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hoe),
                category: PieceCategoryManager.Nature,
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
                pieceTable: nameof(PieceTables.Hoe),
                category: PieceCategoryManager.Nature,
                craftingStation: nameof(CraftingStations.None),
                requirements: "Stone,32",
                clipEverything: true,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Rock
            )
        },
        {
            "Rock_7_deepnorth",
            new PrefabConfig(
                name: "Rock_7_deepnorth",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: true,
                placementPatch: false
            )
        },
        {
            "Rock_7_meadows",
            new PrefabConfig(
                name: "Rock_7_meadows",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: true,
                placementPatch: false
            )
        },
        {
            "Rock_destructible",
            new PrefabConfig(
                name: "Rock_destructible",
                enabled: true,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hoe),
                category: PieceCategoryManager.Nature,
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
                pieceTable: nameof(PieceTables.Hoe),
                category: PieceCategoryManager.Nature,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: true,
                placementPatch: false
            )
        },
        {
            "ShimmeringSand_rock_frac",
            new PrefabConfig(
                name: "ShimmeringSand_rock_frac",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: true,
                placementPatch: false
            )
        },
        {
            "ShootStump",
            new PrefabConfig(
                name: "ShootStump",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Cultivator),
                category: PieceCategoryManager.Nature,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "Sled",
            new PrefabConfig(
                name: "Sled",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "SnowFirTree",
            new PrefabConfig(
                name: "SnowFirTree",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "SnowFirTree2_snowfall",
            new PrefabConfig(
                name: "SnowFirTree2_snowfall",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "SnowFirTreeSmall_snowfall",
            new PrefabConfig(
                name: "SnowFirTreeSmall_snowfall",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "SnowFirTree_small",
            new PrefabConfig(
                name: "SnowFirTree_small",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "SnowFirTree_snowfall",
            new PrefabConfig(
                name: "SnowFirTree_snowfall",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "SnowRoller",
            new PrefabConfig(
                name: "SnowRoller",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "Spawner_CharredCross",
            new PrefabConfig(
                name: "Spawner_CharredCross",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: true,
                placementPatch: false
            )
        },
        {
            "Spawner_CharredStone",
            new PrefabConfig(
                name: "Spawner_CharredStone",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "Spawner_CharredStone_Elite",
            new PrefabConfig(
                name: "Spawner_CharredStone_Elite",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "Spawner_CharredStone_event",
            new PrefabConfig(
                name: "Spawner_CharredStone_event",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "Spawner_DraugrPile",
            new PrefabConfig(
                name: "Spawner_DraugrPile",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "Spawner_GreydwarfNest",
            new PrefabConfig(
                name: "Spawner_GreydwarfNest",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: true,
                placementPatch: false
            )
        },
        {
            "Spawner_Hole",
            new PrefabConfig(
                name: "Spawner_Hole",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "Spawner_Hole_double",
            new PrefabConfig(
                name: "Spawner_Hole_double",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "Spawner_Kvastur",
            new PrefabConfig(
                name: "Spawner_Kvastur",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "StartPlatform",
            new PrefabConfig(
                name: "StartPlatform",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.Furniture,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.Furniture,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.Furniture,
                craftingStation: nameof(CraftingStations.Stonecutter),
                requirements: "Stone,32",
                clipEverything: true,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Statue
            )
        },
        {
            "StatueFreya",
            new PrefabConfig(
                name: "StatueFreya",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "StatueFreya_broken_left",
            new PrefabConfig(
                name: "StatueFreya_broken_left",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "StatueFreya_broken_right",
            new PrefabConfig(
                name: "StatueFreya_broken_right",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "StatueHare",
            new PrefabConfig(
                name: "StatueHare",
                enabled: true,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.Furniture,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.Furniture,
                craftingStation: nameof(CraftingStations.Stonecutter),
                requirements: "Stone,16",
                clipEverything: true,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Statue
            )
        },
        {
            "StatueThor",
            new PrefabConfig(
                name: "StatueThor",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "StatueThor_broken_bottom",
            new PrefabConfig(
                name: "StatueThor_broken_bottom",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "StatueThor_broken_top",
            new PrefabConfig(
                name: "StatueThor_broken_top",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "StumpHole",
            new PrefabConfig(
                name: "StumpHole",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: true,
                placementPatch: false
            )
        },
        {
            "StumpHut_frac",
            new PrefabConfig(
                name: "StumpHut_frac",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: true,
                placementPatch: false
            )
        },
        {
            "StumpLog",
            new PrefabConfig(
                name: "StumpLog",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: true,
                placementPatch: false
            )
        },
        {
            "SwampTree1",
            new PrefabConfig(
                name: "SwampTree1",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Cultivator),
                category: PieceCategoryManager.Nature,
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
                pieceTable: nameof(PieceTables.Cultivator),
                category: PieceCategoryManager.Nature,
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
                pieceTable: nameof(PieceTables.Cultivator),
                category: PieceCategoryManager.Nature,
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
                pieceTable: nameof(PieceTables.Cultivator),
                category: PieceCategoryManager.Nature,
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
                pieceTable: nameof(PieceTables.Cultivator),
                category: PieceCategoryManager.Nature,
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
                pieceTable: nameof(PieceTables.Cultivator),
                category: PieceCategoryManager.Nature,
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
                category: PieceCategoryManager.CreatorShop,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.Misc,
                craftingStation: nameof(CraftingStations.Workbench),
                requirements: "IronNails,100;DeerHide,10;FineWood,40;ElderBark,40",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceName: "Trader Ship",
                pieceGroup: PieceClassification.Ship,
                invWidth: 6,
                invHeight: 4
            )
        },
        {
            "TreasureChest_ashland_stone",
            new PrefabConfig(
                name: "TreasureChest_ashland_stone",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
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
                enabled: true,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.Misc,
                craftingStation: nameof(CraftingStations.Stonecutter),
                requirements: "Grausten,10;ProustitePowder,2;FlametalNew,6",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceName: "Charred Chest",
                pieceGroup: PieceClassification.Chest,
                invWidth: 8,
                invHeight: 6
            )
        },
        {
            "TreasureChest_deepnorth_village",
            new PrefabConfig(
                name: "TreasureChest_deepnorth_village",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "TreasureChest_dvergr_loose_stone",
            new PrefabConfig(
                name: "TreasureChest_dvergr_loose_stone",
                enabled: true,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.Furniture,
                craftingStation: nameof(CraftingStations.Stonecutter),
                requirements: "BlackMarble,10",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceName: "Black Marble Chest",
                pieceGroup: PieceClassification.Chest,
                invWidth: 8,
                invHeight: 5
            )
        },
        {
            "TreasureChest_dvergrtower",
            new PrefabConfig(
                name: "TreasureChest_dvergrtower",
                enabled: true,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.Furniture,
                craftingStation: nameof(CraftingStations.BlackForge),
                requirements: "YggdrasilWood,10;Copper,2",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceName: "Dvergr Chest",
                pieceGroup: PieceClassification.Chest,
                invWidth: 8,
                invHeight: 4
            )
        },
        {
            "TreasureChest_dvergrtown",
            new PrefabConfig(
                name: "TreasureChest_dvergrtown",
                enabled: true,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.Furniture,
                craftingStation: nameof(CraftingStations.BlackForge),
                requirements: "YggdrasilWood,10;Copper,6",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceName: "Dvergr Chest (Large)",
                pieceGroup: PieceClassification.Chest,
                invWidth: 8,
                invHeight: 5
            )
        },
        {
            "TreasureChest_fCrypt",
            new PrefabConfig(
                name: "TreasureChest_fCrypt",
                enabled: true,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.Furniture,
                craftingStation: nameof(CraftingStations.Stonecutter),
                requirements: "Stone,10;RoundLog,1",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceName: "Stone Chest (Mossy)",
                pieceGroup: PieceClassification.Chest,
                invWidth: 5,
                invHeight: 2
            )
        },
        {
            "TreasureChest_meadows_01",
            new PrefabConfig(
                name: "TreasureChest_meadows_01",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "TreasureChest_meadows_02",
            new PrefabConfig(
                name: "TreasureChest_meadows_02",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "TreasureChest_meadows_combat",
            new PrefabConfig(
                name: "TreasureChest_meadows_combat",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "TreasureChest_memorial_buried",
            new PrefabConfig(
                name: "TreasureChest_memorial_buried",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "TreasureChest_morkhalla",
            new PrefabConfig(
                name: "TreasureChest_morkhalla",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "TreasureChest_mountaincave",
            new PrefabConfig(
                name: "TreasureChest_mountaincave",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.Furniture,
                craftingStation: nameof(CraftingStations.Stonecutter),
                requirements: "Stone,10;Crystal,1",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceName: "Stone Chest (Snow)",
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.Furniture,
                craftingStation: nameof(CraftingStations.Stonecutter),
                requirements: "Stone,10;ElderBark,1",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceName: "Stone Chest (Dark Moss)",
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.Furniture,
                craftingStation: nameof(CraftingStations.Stonecutter),
                requirements: "Stone,10;RoundLog,2",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceName: "Stone Chest (Mossy, Big)",
                pieceGroup: PieceClassification.Chest,
                invWidth: 6,
                invHeight: 3
            )
        },
        {
            "TrollFrost_Frac",
            new PrefabConfig(
                name: "TrollFrost_Frac",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                placementOffset: new Vector3(0f, -0.5f, 0f)
            )
        },
        {
            "UnstableLavaRock",
            new PrefabConfig(
                name: "UnstableLavaRock",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hoe),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Rock
            )
        },
        {
            "UpgradeStation",
            new PrefabConfig(
                name: "UpgradeStation",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "Valkyrie_End",
            new PrefabConfig(
                name: "Valkyrie_End",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: true,
                placementPatch: false
            )
        },
        {
            "VineAsh",
            new PrefabConfig(
                name: "VineAsh",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Cultivator),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Plant
            )
        },
        {
            "VineGreen",
            new PrefabConfig(
                name: "VineGreen",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Cultivator),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Plant
            )
        },
        {
            "WallSnow1",
            new PrefabConfig(
                name: "WallSnow1",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: true,
                placementPatch: false
            )
        },
        {
            "WallSnow2",
            new PrefabConfig(
                name: "WallSnow2",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "WallSnow3",
            new PrefabConfig(
                name: "WallSnow3",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "WallSnow4",
            new PrefabConfig(
                name: "WallSnow4",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
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
                pieceTable: nameof(PieceTables.Cultivator),
                category: PieceCategoryManager.Nature,
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
                pieceTable: nameof(PieceTables.Cultivator),
                category: PieceCategoryManager.Nature,
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
                pieceTable: nameof(PieceTables.Cultivator),
                category: PieceCategoryManager.Nature,
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
                pieceTable: nameof(PieceTables.Cultivator),
                category: PieceCategoryManager.Nature,
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
                pieceTable: nameof(PieceTables.Cultivator),
                category: PieceCategoryManager.Nature,
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
                pieceTable: nameof(PieceTables.Hoe),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "BlackMarble,100",
                clipEverything: false,
                clipGround: false,
                placementPatch: true,
                pieceGroup: PieceClassification.Rock,
                spawnOnDestroyed: "sfx_rock_destroyed"
            )
        },
        {
            "ashland_pot1_green",
            new PrefabConfig(
                name: "ashland_pot1_green",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
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
                enabled: true,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
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
                enabled: true,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "bar_ancientmetal_stack",
            new PrefabConfig(
                name: "bar_ancientmetal_stack",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.Misc,
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
                pieceTable: nameof(PieceTables.Cultivator),
                category: PieceCategoryManager.Nature,
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
                pieceTable: nameof(PieceTables.Cultivator),
                category: PieceCategoryManager.Nature,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.Stonecutter,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.Stonecutter,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.Stonecutter,
                craftingStation: nameof(CraftingStations.Stonecutter),
                requirements: "BlackMarble,8",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceName: "Black Marble Plinth (Wide)",
                pieceGroup: PieceClassification.BlackMarble
            )
        },
        {
            "blackmarble_column_3",
            new PrefabConfig(
                name: "blackmarble_column_3",
                enabled: true,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.Stonecutter,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.Stonecutter,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.Stonecutter,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.Stonecutter,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.Stonecutter,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.Stonecutter,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.Stonecutter,
                craftingStation: nameof(CraftingStations.Stonecutter),
                requirements: "BlackMarble,32",
                clipEverything: false,
                clipGround: true,
                placementPatch: false,
                pieceName: "Black Marble Floor 8x8",
                pieceGroup: PieceClassification.BlackMarble
            )
        },
        {
            "blackmarble_head01",
            new PrefabConfig(
                name: "blackmarble_head01",
                enabled: true,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.Stonecutter,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.Stonecutter,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.Stonecutter,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.Stonecutter,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.Stonecutter,
                craftingStation: nameof(CraftingStations.Stonecutter),
                requirements: "BlackMarble,8",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceName: "Black Marble Cornice (Wide)",
                pieceGroup: PieceClassification.BlackMarble
            )
        },
        {
            "blackmarble_post01",
            new PrefabConfig(
                name: "blackmarble_post01",
                enabled: true,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.Stonecutter,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.Stonecutter,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.Stonecutter,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.Stonecutter,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.Stonecutter,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.Stonecutter,
                craftingStation: nameof(CraftingStations.Stonecutter),
                requirements: "BlackMarble,1",
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.Stonecutter,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.Stonecutter,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.Stonecutter,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.Stonecutter,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.Misc,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.Nature,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.Nature,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.Nature,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.Nature,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.Nature,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.BlackForge),
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
                pieceTable: nameof(PieceTables.Hoe),
                category: PieceCategoryManager.Rocks,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: true,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Rock
            )
        },
        {
            "cliff_ashlands2_frac",
            new PrefabConfig(
                name: "cliff_ashlands2_frac",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hoe),
                category: PieceCategoryManager.Rocks,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: true,
                placementPatch: true,
                pieceGroup: PieceClassification.Rock
            )
        },
        {
            "cliff_ashlands4_frac",
            new PrefabConfig(
                name: "cliff_ashlands4_frac",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hoe),
                category: PieceCategoryManager.Rocks,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: true,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Rock
            )
        },
        {
            "cliff_ashlands6_frac",
            new PrefabConfig(
                name: "cliff_ashlands6_frac",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hoe),
                category: PieceCategoryManager.Rocks,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: true,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Rock
            )
        },
        {
            "cliff_ashlands7_HalfArch_frac",
            new PrefabConfig(
                name: "cliff_ashlands7_HalfArch_frac",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hoe),
                category: PieceCategoryManager.Rocks,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: true,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Rock
            )
        },
        {
            "cliff_ashlands_Arch_frac",
            new PrefabConfig(
                name: "cliff_ashlands_Arch_frac",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hoe),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: true,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Rock
            )
        },
        {
            "cliff_ashlandsflowrock_frac",
            new PrefabConfig(
                name: "cliff_ashlandsflowrock_frac",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hoe),
                category: PieceCategoryManager.Rocks,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: true,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Rock
            )
        },
        {
            "cliff_mistlands1",
            new PrefabConfig(
                name: "cliff_mistlands1",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hoe),
                category: PieceCategoryManager.Nature,
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
                pieceTable: nameof(PieceTables.Hoe),
                category: PieceCategoryManager.Nature,
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
                pieceTable: nameof(PieceTables.Hoe),
                category: PieceCategoryManager.CreatorShop,
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
                pieceTable: nameof(PieceTables.Hoe),
                category: PieceCategoryManager.CreatorShop,
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
                pieceTable: nameof(PieceTables.Hoe),
                category: PieceCategoryManager.Nature,
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
                pieceTable: nameof(PieceTables.Hoe),
                category: PieceCategoryManager.CreatorShop,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.Furniture,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.Furniture,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.Furniture,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: true,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "deepnorth_lantern_standing",
            new PrefabConfig(
                name: "deepnorth_lantern_standing",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.Stonecutter,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.Stonecutter,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.Furniture,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.Furniture,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.Misc,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.Furniture,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.Furniture,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.Furniture,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.Furniture,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.Misc,
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
                category: PieceCategoryManager.CreatorShop,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.Misc,
                craftingStation: nameof(CraftingStations.BlackForge),
                requirements: "YggdrasilWood,4",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceName: "Dvergr Component Crate",
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.Furniture,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.Stonecutter,
                craftingStation: nameof(CraftingStations.BlackForge),
                requirements: "Chain,2;Iron,2",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceName: "Dvergr Hook & Chain",
                pieceGroup: PieceClassification.Misc
            )
        },
        {
            "dvergrprops_lantern_standing",
            new PrefabConfig(
                name: "dvergrprops_lantern_standing",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.Furniture,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.Furniture,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.Furniture,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.Building,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.Building,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.Building,
                craftingStation: nameof(CraftingStations.BlackForge),
                requirements: "YggdrasilWood,2",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceName: "Dvergr Wood Pole (Large)",
                pieceGroup: PieceClassification.Dvergr
            )
        },
        {
            "dvergrprops_wood_stair",
            new PrefabConfig(
                name: "dvergrprops_wood_stair",
                enabled: true,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.Building,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.Building,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.Building,
                craftingStation: nameof(CraftingStations.BlackForge),
                requirements: "YggdrasilWood,10",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceName: "Dvergr Wood Wall 4x4",
                pieceGroup: PieceClassification.Dvergr
            )
        },
        {
            "dvergrtown_arch",
            new PrefabConfig(
                name: "dvergrtown_arch",
                enabled: true,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.Stonecutter,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.Nature,
                craftingStation: nameof(CraftingStations.None),
                requirements: "YggdrasilWood,4",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceName: "Door Hanging (Creep)",
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.Stonecutter,
                craftingStation: nameof(CraftingStations.BlackForge),
                requirements: "BlackMarble,12;Eitr,2",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceName: "Dvergr Secret Door",
                pieceGroup: PieceClassification.Dvergr
            )
        },
        {
            "dvergrtown_slidingdoor",
            new PrefabConfig(
                name: "dvergrtown_slidingdoor",
                enabled: true,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.Stonecutter,
                craftingStation: nameof(CraftingStations.BlackForge),
                requirements: "BlackMarble,32;Copper,8",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceName: "Dvergr Sliding Door",
                pieceGroup: PieceClassification.Dvergr
            )
        },
        {
            "dvergrtown_stair_corner_wood_left",
            new PrefabConfig(
                name: "dvergrtown_stair_corner_wood_left",
                enabled: true,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.Building,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.Building,
                craftingStation: nameof(CraftingStations.BlackForge),
                requirements: "YggdrasilWood,4",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceName: "Dvergr Wood Beam (Creep)",
                pieceGroup: PieceClassification.Dvergr
            )
        },
        {
            "dvergrtown_wood_crane",
            new PrefabConfig(
                name: "dvergrtown_wood_crane",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.Building,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceName: "Dvergr Wood Pole (Creep)",
                pieceGroup: PieceClassification.Dvergr
            )
        },
        {
            "dvergrtown_wood_stake",
            new PrefabConfig(
                name: "dvergrtown_wood_stake",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.Building,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.Building,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.Building,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.Building,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.Building,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.Building,
                craftingStation: nameof(CraftingStations.BlackForge),
                requirements: "YggdrasilWood,10;IronNails,6",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Dvergr
            )
        },
        {
            "elaking_trashpile",
            new PrefabConfig(
                name: "elaking_trashpile",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "fader_bellholder",
            new PrefabConfig(
                name: "fader_bellholder",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
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
                category: PieceCategoryManager.CreatorShop,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.Misc,
                craftingStation: nameof(CraftingStations.None),
                requirements: "Stone,5;Wood,2;BlackCore,1",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceName: "Campfire (Eternal)",
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.Misc,
                craftingStation: nameof(CraftingStations.Forge),
                requirements: "Ironpit,1;Wood,1;BlackCore,1",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceName: "Firepit Iron (Eternal)",
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "Crystal,24",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                spawnOnDestroyed: "fx_crystal_destruction"
            )
        },
        {
            "frozenship",
            new PrefabConfig(
                name: "frozenship",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: true,
                placementPatch: false
            )
        },
        {
            "frozenship02",
            new PrefabConfig(
                name: "frozenship02",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "frozenship03",
            new PrefabConfig(
                name: "frozenship03",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "fuling_trap",
            new PrefabConfig(
                name: "fuling_trap",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
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
                pieceTable: nameof(PieceTables.Hoe),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "BlackMarble,32",
                clipEverything: true,
                pieceGroup: PieceClassification.Rock
            )
        },
        {
            "giant_brain",
            new PrefabConfig(
                name: "giant_brain",
                enabled: false,
                allowedInDungeons: false,
                category: PieceCategoryManager.CreatorShop,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
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
                category: PieceCategoryManager.CreatorShop,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
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
                category: PieceCategoryManager.CreatorShop,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
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
                pieceTable: nameof(PieceTables.Hoe),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "BlackMarble,64",
                clipEverything: true,
                pieceGroup: PieceClassification.Rock
            )
        },
        {
            "giant_ribs_frac",
            new PrefabConfig(
                name: "giant_ribs_frac",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hoe),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "BlackMarble,64",
                clipEverything: true,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Rock
            )
        },
        {
            "giant_skull",
            new PrefabConfig(
                name: "giant_skull",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hoe),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "BlackMarble,32",
                clipEverything: true,
                pieceGroup: PieceClassification.Rock
            )
        },
        {
            "giant_skull_frac",
            new PrefabConfig(
                name: "giant_skull_frac",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hoe),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "BlackMarble,32",
                clipEverything: true,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Rock
            )
        },
        {
            "giant_sword1",
            new PrefabConfig(
                name: "giant_sword1",
                enabled: false,
                allowedInDungeons: false,
                category: PieceCategoryManager.CreatorShop,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
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
                category: PieceCategoryManager.CreatorShop,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.Furniture,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.Furniture,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.Building,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.Building,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.Building,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.Building,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.Building,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.Building,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.Building,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.Building,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.Furniture,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.Building,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.Building,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.Stonecutter),
                requirements: "Stone,1;Tar,1",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Goblin
            )
        },
        {
            "goldvein_frac",
            new PrefabConfig(
                name: "goldvein_frac",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: true,
                placementPatch: false
            )
        },
        {
            "hanging_hairstrands",
            new PrefabConfig(
                name: "hanging_hairstrands",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
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
                pieceTable: nameof(PieceTables.Hoe),
                category: PieceCategoryManager.Nature,
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
                pieceTable: nameof(PieceTables.Hoe),
                category: PieceCategoryManager.CreatorShop,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
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
                category: PieceCategoryManager.CreatorShop,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.Stonecutter,
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
                pieceTable: nameof(PieceTables.Hoe),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: true,
                clipGround: false,
                placementPatch: true,
                pieceGroup: PieceClassification.Rock
            )
        },
        {
            "lightningAOE",
            new PrefabConfig(
                name: "lightningAOE",
                enabled: false,
                allowedInDungeons: false,
                category: PieceCategoryManager.CreatorShop,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.Furniture,
                craftingStation: nameof(CraftingStations.Stonecutter),
                requirements: "Stone,10",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceName: "Stone Chest (Light Moss)",
                pieceGroup: PieceClassification.Chest,
                invWidth: 5,
                invHeight: 2
            )
        },
        {
            "loot_deepNorth_Granary",
            new PrefabConfig(
                name: "loot_deepNorth_Granary",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "loot_deepNorth_TimberHall",
            new PrefabConfig(
                name: "loot_deepNorth_TimberHall",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "lox_ribs",
            new PrefabConfig(
                name: "lox_ribs",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.Building,
                craftingStation: nameof(CraftingStations.BlackForge),
                requirements: "BlackMarble,2;Copper,1",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceName: "Black Marble 1x2 (Reinforced)",
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "Eitr,10;Wisp,4",
                clipEverything: true,
                clipGround: false,
                placementPatch: false,
                pieceName: "Mist Volume",
                pieceDesc: "Warning: requires devcommands to remove."
            )
        },
        {
            "morgenhole_pile",
            new PrefabConfig(
                name: "morgenhole_pile",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "morkhalla_web_corner",
            new PrefabConfig(
                name: "morkhalla_web_corner",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "morkhalla_web_horisontal",
            new PrefabConfig(
                name: "morkhalla_web_horisontal",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "morkhalla_web_tunnel",
            new PrefabConfig(
                name: "morkhalla_web_tunnel",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.Furniture,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.Furniture,
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
                category: PieceCategoryManager.CreatorShop,
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
                category: PieceCategoryManager.CreatorShop,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
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
                category: PieceCategoryManager.CreatorShop,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
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
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "IronScrap,10"
            )
        },
        {
            "obsidian_pile",
            new PrefabConfig(
                name: "obsidian_pile",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "offeraltar_FrozenKing_bossroom",
            new PrefabConfig(
                name: "offeraltar_FrozenKing_bossroom",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: true,
                placementPatch: false
            )
        },
        {
            "piece_Charred_Balista",
            new PrefabConfig(
                name: "piece_Charred_Balista",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "piece_FaderEmbers",
            new PrefabConfig(
                name: "piece_FaderEmbers",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.Building,
                craftingStation: nameof(CraftingStations.BlackForge),
                requirements: "YggdrasilWood,1",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceName: "Dvergr Wood Pole",
                pieceGroup: PieceClassification.Dvergr
            )
        },
        {
            "piece_dvergr_wood_door",
            new PrefabConfig(
                name: "piece_dvergr_wood_door",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.Building,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.Building,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.Misc,
                craftingStation: nameof(CraftingStations.Stonecutter),
                requirements: "GreydwarfEye,10;BlackMarble,20;MoltenCore,2",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                placementOffset: new Vector3(0f, -0.35f, 0f),
                pieceName: "Dvergr Portal",
                pieceDesc: "Connects another portal with equal or no tag.",
                pieceGroup: PieceClassification.Portal
            )
        },
        {
            "prop_FeastAshlands",
            new PrefabConfig(
                name: "prop_FeastAshlands",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "prop_FeastMeadows",
            new PrefabConfig(
                name: "prop_FeastMeadows",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "prop_Tankard",
            new PrefabConfig(
                name: "prop_Tankard",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: true,
                placementPatch: false
            )
        },
        {
            "prop_ashwood_bed",
            new PrefabConfig(
                name: "prop_ashwood_bed",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "prop_bed02",
            new PrefabConfig(
                name: "prop_bed02",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "prop_bonfire",
            new PrefabConfig(
                name: "prop_bonfire",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "prop_cauldron_ext1_spice",
            new PrefabConfig(
                name: "prop_cauldron_ext1_spice",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "prop_cauldron_ext3_butchertable",
            new PrefabConfig(
                name: "prop_cauldron_ext3_butchertable",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "prop_cauldron_ext5_mortarandpestle",
            new PrefabConfig(
                name: "prop_cauldron_ext5_mortarandpestle",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "prop_cauldron_ext6_rollingpins",
            new PrefabConfig(
                name: "prop_cauldron_ext6_rollingpins",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "prop_chest_warderobe",
            new PrefabConfig(
                name: "prop_chest_warderobe",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "prop_forge_ext2",
            new PrefabConfig(
                name: "prop_forge_ext2",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "prop_forge_ext5",
            new PrefabConfig(
                name: "prop_forge_ext5",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "prop_hearth",
            new PrefabConfig(
                name: "prop_hearth",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "prop_itemstand",
            new PrefabConfig(
                name: "prop_itemstand",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "prop_itemstand_TrophyDraugrElite",
            new PrefabConfig(
                name: "prop_itemstand_TrophyDraugrElite",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "prop_itemstand_TrophyGoblinBrute",
            new PrefabConfig(
                name: "prop_itemstand_TrophyGoblinBrute",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "prop_itemstand_TrophyGoblinShaman",
            new PrefabConfig(
                name: "prop_itemstand_TrophyGoblinShaman",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "prop_itemstand_TrophyGreydwarf",
            new PrefabConfig(
                name: "prop_itemstand_TrophyGreydwarf",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "prop_itemstand_TrophyGreydwarfBrute",
            new PrefabConfig(
                name: "prop_itemstand_TrophyGreydwarfBrute",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "prop_itemstand_TrophySeekerBrute",
            new PrefabConfig(
                name: "prop_itemstand_TrophySeekerBrute",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "prop_piece_MeadCauldron",
            new PrefabConfig(
                name: "prop_piece_MeadCauldron",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: true,
                placementPatch: false
            )
        },
        {
            "prop_piece_bench_runed",
            new PrefabConfig(
                name: "prop_piece_bench_runed",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "prop_piece_brazierfloor01",
            new PrefabConfig(
                name: "prop_piece_brazierfloor01",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "prop_piece_cauldron",
            new PrefabConfig(
                name: "prop_piece_cauldron",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "prop_piece_chair03",
            new PrefabConfig(
                name: "prop_piece_chair03",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "prop_piece_cookingstation",
            new PrefabConfig(
                name: "prop_piece_cookingstation",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: true,
                placementPatch: false
            )
        },
        {
            "prop_piece_workbench_ext1",
            new PrefabConfig(
                name: "prop_piece_workbench_ext1",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "prop_piece_workbench_ext2",
            new PrefabConfig(
                name: "prop_piece_workbench_ext2",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "prop_piece_workbench_ext3",
            new PrefabConfig(
                name: "prop_piece_workbench_ext3",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "prop_piece_workbench_ext4",
            new PrefabConfig(
                name: "prop_piece_workbench_ext4",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "prop_preptable",
            new PrefabConfig(
                name: "prop_preptable",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "prop_wood_stack",
            new PrefabConfig(
                name: "prop_wood_stack",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "rock1_mistlands",
            new PrefabConfig(
                name: "rock1_mistlands",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hoe),
                category: PieceCategoryManager.Nature,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: true,
                pieceName: "Rock (Large Boulder)",
                pieceGroup: PieceClassification.Rock
            )
        },
        {
            "rock1_mountain",
            new PrefabConfig(
                name: "rock1_mountain",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hoe),
                category: PieceCategoryManager.Nature,
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
                pieceTable: nameof(PieceTables.Hoe),
                category: PieceCategoryManager.CreatorShop,
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
                pieceTable: nameof(PieceTables.Hoe),
                category: PieceCategoryManager.Nature,
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
                pieceTable: nameof(PieceTables.Hoe),
                category: PieceCategoryManager.CreatorShop,
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
                pieceTable: nameof(PieceTables.Hoe),
                category: PieceCategoryManager.Nature,
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
                pieceTable: nameof(PieceTables.Hoe),
                category: PieceCategoryManager.CreatorShop,
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
                pieceTable: nameof(PieceTables.Hoe),
                category: PieceCategoryManager.Nature,
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
                pieceTable: nameof(PieceTables.Hoe),
                category: PieceCategoryManager.CreatorShop,
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
                pieceTable: nameof(PieceTables.Hoe),
                category: PieceCategoryManager.Nature,
                craftingStation: nameof(CraftingStations.None),
                requirements: "Stone,100",
                clipEverything: true,
                pieceGroup: PieceClassification.Rock
            )
        },
        {
            "rock3_mountain_1_frac",
            new PrefabConfig(
                name: "rock3_mountain_1_frac",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: true,
                placementPatch: false
            )
        },
        {
            "rock3_mountain_frac",
            new PrefabConfig(
                name: "rock3_mountain_frac",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hoe),
                category: PieceCategoryManager.CreatorShop,
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
                pieceTable: nameof(PieceTables.Hoe),
                category: PieceCategoryManager.Nature,
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
                pieceTable: nameof(PieceTables.Hoe),
                category: PieceCategoryManager.CreatorShop,
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
                pieceTable: nameof(PieceTables.Hoe),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: true,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Rock
            )
        },
        {
            "rock4_bigrock_frac",
            new PrefabConfig(
                name: "rock4_bigrock_frac",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: true,
                placementPatch: false
            )
        },
        {
            "rock4_coast",
            new PrefabConfig(
                name: "rock4_coast",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hoe),
                category: PieceCategoryManager.Nature,
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
                pieceTable: nameof(PieceTables.Hoe),
                category: PieceCategoryManager.CreatorShop,
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
                pieceTable: nameof(PieceTables.Hoe),
                category: PieceCategoryManager.Nature,
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
                pieceTable: nameof(PieceTables.Hoe),
                category: PieceCategoryManager.Nature,
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
                pieceTable: nameof(PieceTables.Hoe),
                category: PieceCategoryManager.Nature,
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
                pieceTable: nameof(PieceTables.Hoe),
                category: PieceCategoryManager.CreatorShop,
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
                pieceTable: nameof(PieceTables.Hoe),
                category: PieceCategoryManager.Nature,
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
                pieceTable: nameof(PieceTables.Hoe),
                category: PieceCategoryManager.CreatorShop,
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
                pieceTable: nameof(PieceTables.Hoe),
                category: PieceCategoryManager.Nature,
                craftingStation: nameof(CraftingStations.None),
                requirements: "Stone,48",
                clipEverything: true,
                pieceName: "Rock (Black)",
                pieceGroup: PieceClassification.Rock
            )
        },
        {
            "rock_mistlands1_frac",
            new PrefabConfig(
                name: "rock_mistlands1_frac",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hoe),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "Stone,48",
                clipEverything: true,
                clipGround: false,
                placementPatch: false,
                pieceName: "Rock (Black)",
                pieceGroup: PieceClassification.Rock
            )
        },
        {
            "rock_mistlands2",
            new PrefabConfig(
                name: "rock_mistlands2",
                enabled: true,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hoe),
                category: PieceCategoryManager.Nature,
                craftingStation: nameof(CraftingStations.None),
                requirements: "Stone,48",
                clipEverything: true,
                clipGround: false,
                placementPatch: false,
                pieceName: "Rock (Black, Alt)",
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
                pieceTable: nameof(PieceTables.Hoe),
                category: PieceCategoryManager.Nature,
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
                pieceTable: nameof(PieceTables.Cultivator),
                category: PieceCategoryManager.Nature,
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
                pieceTable: nameof(PieceTables.Cultivator),
                category: PieceCategoryManager.Nature,
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
                pieceTable: nameof(PieceTables.Cultivator),
                category: PieceCategoryManager.Nature,
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
                pieceTable: nameof(PieceTables.Cultivator),
                category: PieceCategoryManager.Nature,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Rug
            )
        },
        {
            "rug_bogwitch_fur",
            new PrefabConfig(
                name: "rug_bogwitch_fur",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Rug
            )
        },
        {
            "rug_bogwitch_wolf",
            new PrefabConfig(
                name: "rug_bogwitch_wolf",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Rug
            )
        },
        {
            "rug_straw",
            new PrefabConfig(
                name: "rug_straw",
                enabled: true,
                allowedInDungeons: false,
                category: PieceCategoryManager.Furniture,
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
                category: PieceCategoryManager.CreatorShop,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "FineWood,10",
                clipEverything: true,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "shipwreck_vikingship_chest",
            new PrefabConfig(
                name: "shipwreck_vikingship_chest",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "shipwreck_vikingship_front",
            new PrefabConfig(
                name: "shipwreck_vikingship_front",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "shipwreck_vikingship_frontpiece",
            new PrefabConfig(
                name: "shipwreck_vikingship_frontpiece",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "shipwreck_vikingship_mast1",
            new PrefabConfig(
                name: "shipwreck_vikingship_mast1",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "shipwreck_vikingship_rear",
            new PrefabConfig(
                name: "shipwreck_vikingship_rear",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
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
                pieceTable: nameof(PieceTables.Cultivator),
                category: PieceCategoryManager.Nature,
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
                pieceTable: nameof(PieceTables.Cultivator),
                category: PieceCategoryManager.Nature,
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
                enabled: true,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.Stonecutter),
                requirements: "Grausten,2",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Fortress
            )
        },
        {
            "sign_notext",
            new PrefabConfig(
                name: "sign_notext",
                enabled: true,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.Building,
                craftingStation: nameof(CraftingStations.Workbench),
                requirements: "Wood,1",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceName: "Wood Plank",
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
                pieceTable: nameof(PieceTables.Hoe),
                category: PieceCategoryManager.Nature,
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
                pieceTable: nameof(PieceTables.Hoe),
                category: PieceCategoryManager.CreatorShop,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.Stonecutter,
                craftingStation: nameof(CraftingStations.Stonecutter),
                requirements: "Stone,16",
                clipEverything: false,
                clipGround: true,
                placementPatch: false,
                pieceName: "Stone Floor 4x4",
                pieceGroup: PieceClassification.Stone
            )
        },
        {
            "stone_wall_1x1_ruin",
            new PrefabConfig(
                name: "stone_wall_1x1_ruin",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "stone_wall_2x1_ruin",
            new PrefabConfig(
                name: "stone_wall_2x1_ruin",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "stone_wall_ruin",
            new PrefabConfig(
                name: "stone_wall_ruin",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "stone_wall_ruin_2",
            new PrefabConfig(
                name: "stone_wall_ruin_2",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "stoneblock_fracture",
            new PrefabConfig(
                name: "stoneblock_fracture",
                enabled: true,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.Stonecutter,
                craftingStation: nameof(CraftingStations.Stonecutter),
                requirements: "Stone,16",
                clipEverything: false,
                clipGround: true,
                placementPatch: false,
                pieceName: "Stone Floor 4x4 (2)",
                pieceGroup: PieceClassification.Stone
            )
        },
        {
            "stonewall_2",
            new PrefabConfig(
                name: "stonewall_2",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: true,
                placementPatch: false
            )
        },
        {
            "stonewall_3",
            new PrefabConfig(
                name: "stonewall_3",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: true,
                placementPatch: false
            )
        },
        {
            "stubbe",
            new PrefabConfig(
                name: "stubbe",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Cultivator),
                category: PieceCategoryManager.Nature,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: true,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Flora
            )
        },
        {
            "stubbe_deepnorth",
            new PrefabConfig(
                name: "stubbe_deepnorth",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: true,
                placementPatch: false
            )
        },
        {
            "stubbe_spawner",
            new PrefabConfig(
                name: "stubbe_spawner",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.Stonecutter,
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
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "Tar,50",
                clipEverything: true,
                clipGround: false,
                pieceName: "Crystallized Tar",
                pieceDesc: "Warning: requires devcommands to remove"
            )
        },
        {
            "tarlump1_frac",
            new PrefabConfig(
                name: "tarlump1_frac",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hoe),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "Tar,50",
                clipEverything: true,
                clipGround: false,
                placementPatch: false,
                pieceName: "Crystallized Tar",
                pieceGroup: PieceClassification.Ore
            )
        },
        {
            "trader_wagon_destructable",
            new PrefabConfig(
                name: "trader_wagon_destructable",
                enabled: true,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.Furniture,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: true,
                placementPatch: false
            )
        },
        {
            "turf_roof",
            new PrefabConfig(
                name: "turf_roof",
                enabled: true,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.Building,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.Building,
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
                enabled: true,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hoe),
                category: PieceCategoryManager.Flora,
                craftingStation: nameof(CraftingStations.None),
                requirements: "Charredskull,1",
                clipEverything: false,
                clipGround: false,
                placementPatch: true,
                pieceName: "Charred Skull"
            )
        },
        {
            "vertical_web",
            new PrefabConfig(
                name: "vertical_web",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                requirements: "",
                clipEverything: false,
                clipGround: true,
                placementPatch: false
            )
        },
        {
            "vines",
            new PrefabConfig(
                name: "vines",
                enabled: true,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Cultivator),
                category: PieceCategoryManager.Nature,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
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
                pieceTable: nameof(PieceTables.Hoe),
                category: PieceCategoryManager.Nature,
                craftingStation: nameof(CraftingStations.None),
                requirements: "Stone,32",
                clipEverything: true,
                pieceGroup: PieceClassification.Rock
            )
        },
        {
            "widestone_2_frac",
            new PrefabConfig(
                name: "widestone_2_frac",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: true,
                placementPatch: false
            )
        },
        {
            "widestone_frac",
            new PrefabConfig(
                name: "widestone_frac",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hoe),
                category: PieceCategoryManager.CreatorShop,
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
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.Building,
                craftingStation: nameof(CraftingStations.Workbench),
                requirements: "Wood,1",
                clipEverything: false,
                clipGround: false,
                placementPatch: false,
                pieceGroup: PieceClassification.Wood
            )
        },
        {
            "wood_pole_log_4_worn",
            new PrefabConfig(
                name: "wood_pole_log_4_worn",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "wood_wall_roof",
            new PrefabConfig(
                name: "wood_wall_roof",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "wooden_path",
            new PrefabConfig(
                name: "wooden_path",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Hammer),
                category: PieceCategoryManager.CreatorShop,
                craftingStation: nameof(CraftingStations.None),
                clipEverything: false,
                clipGround: false,
                placementPatch: false
            )
        },
        {
            "yggashoot_log",
            new PrefabConfig(
                name: "yggashoot_log",
                enabled: false,
                allowedInDungeons: false,
                pieceTable: nameof(PieceTables.Cultivator),
                category: PieceCategoryManager.Nature,
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
                pieceTable: nameof(PieceTables.Cultivator),
                category: PieceCategoryManager.Nature,
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
