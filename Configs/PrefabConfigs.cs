// Ignore Spelling: MVBP

using Jotunn.Configs;
using System.Collections.Generic;

namespace MVBP.Configs
{
    /* Want to try finding these prefabs:
     * MountainKit_int_wall_4x2
     * SunkenKit_int_arch
     * SunkenKit_int_floor_2x2
     * SunkenKit_int_floor_2x2
     * SunkenKit_int_wall_1x4
     * SunkenKit_int_wall_2x4
     * SunkenKit_int_wall_4x4
     * SunkenKit_slope1x2
     * SunkenKit_int_stair
     * SunkenKit_stair_corner_left
     */

    internal class PrefabConfigs
    {
        internal static PrefabDB GetDefaultPieceDB(string prefab_name)
        {
            if (DefaultConfigValues.ContainsKey(prefab_name))
            {
                return DefaultConfigValues[prefab_name];
            }
            return new PrefabDB(prefab_name);
        }

        internal static readonly Dictionary<string, PrefabDB> DefaultConfigValues = new()
        {
            {
                "ancient_skull",
                new PrefabDB(
                    "ancient_skull",
                    false,
                    false,
                    HammerCategories.CreatorShop,
                    nameof(CraftingStations.None),
                    "BlackMarble,100"

                )
            },
            {
                "ArmorStand_Female",
                new PrefabDB(
                    "ArmorStand_Female",
                    true,
                    false,
                    HammerCategories.Furniture,
                    nameof(CraftingStations.Workbench),
                    "FineWood,8;BronzeNails,2;Tar,4",
                    pieceDesc: "",
                    pieceGroup: PieceGroup.ArmorStand

                )
            },
            {
                "ArmorStand_Male",
                new PrefabDB(
                    "ArmorStand_Male",
                    true,
                    false,
                    HammerCategories.Furniture,
                    nameof(CraftingStations.Workbench),
                    "FineWood,8;BronzeNails,2;Tar,4",
                    pieceDesc: "",
                    pieceGroup: PieceGroup.ArmorStand
                )
            },
            {
                "barrell",
                new PrefabDB(
                    "barrell",
                    true,
                    false,
                    HammerCategories.Misc,
                    nameof(CraftingStations.Workbench),
                    "FineWood,2",
                    pieceName: "Barrel",
                    pieceDesc: "",
                    pieceGroup: PieceGroup.Misc
                )
            },
            {
                "beech_log",
                new PrefabDB(
                    "beech_log",
                    false,
                    false,
                    HammerCategories.Nature,
                    nameof(CraftingStations.None),
                    "BeechSeeds,1,Wood,20",
                    pieceGroup: PieceGroup.Flora
                )
            },
            {
                "beech_log_half",
                new PrefabDB(
                    "beech_log_half",
                    false,
                    false,
                    HammerCategories.Nature,
                    nameof(CraftingStations.None),
                    "BeechSeeds,1,Wood,10",
                    pieceGroup: PieceGroup.Flora
                )
            },
            {
                "Beech_small1",
                new PrefabDB(
                    "Beech_small1",
                    false,
                    false,
                    HammerCategories.Nature,
                    nameof(CraftingStations.None),
                    "",
                    pieceGroup: PieceGroup.Flora
                )
            },
            {
                "Beech_small2",
                new PrefabDB(
                    "Beech_small2",
                    false,
                    false,
                    HammerCategories.Nature,
                    nameof(CraftingStations.None),
                    "Wood,1",
                    pieceGroup: PieceGroup.Flora
                )
            },
            {
                "Beech_Stub",
                new PrefabDB(
                    "Beech_Stub",
                    false,
                    false,
                    HammerCategories.Nature,
                    nameof(CraftingStations.None),
                    "",
                    clipEverything: true,
                    pieceGroup: PieceGroup.Flora
                )
            },
            {
                "Beech1",
                new PrefabDB(
                    "Beech1",
                    false,
                    false,
                    HammerCategories.Nature,
                    nameof(CraftingStations.None),
                    "Wood,16",
                    clipEverything: true,
                    pieceGroup: PieceGroup.Flora
                )
            },
            {
                "Birch_log",
                new PrefabDB(
                    "Birch_log",
                    false,
                    false,
                    HammerCategories.Nature,
                    nameof(CraftingStations.None),
                    "",
                    pieceGroup: PieceGroup.Flora
                )
            },
            {
                "Birch_log_half",
                new PrefabDB(
                    "Birch_log_half",
                    false,
                    false,
                    HammerCategories.Nature,
                    nameof(CraftingStations.None),
                    "",
                    pieceGroup: PieceGroup.Flora
                )
            },
            {
                "Birch1",
                new PrefabDB(
                    "Birch1",
                    false,
                    false,
                    HammerCategories.Nature,
                    nameof(CraftingStations.None),
                    "BirchSeeds,1;FineWood,2",
                    clipEverything: true,
                    pieceGroup: PieceGroup.Flora
                )
            },
            {
                "Birch1_aut",
                new PrefabDB(
                    "Birch1_aut",
                    true,
                    false,
                    HammerCategories.Nature,
                    nameof(CraftingStations.None),
                    "BirchSeeds,1;FineWood,2",
                    pieceName: "Birch1 (autumn)",
                    clipEverything: true,
                    pieceGroup: PieceGroup.Flora
                )
            },
            {
                "Birch2",
                new PrefabDB(
                    "Birch2",
                    false,
                    false,
                    HammerCategories.Nature,
                    nameof(CraftingStations.None),
                    "BirchSeeds,1;FineWood,2",
                    clipEverything: true,
                    pieceGroup: PieceGroup.Flora
                )
            },
            {
                "Birch2_aut",
                new PrefabDB(
                    "Birch2_aut",
                    true,
                    false,
                    HammerCategories.Nature,
                    nameof(CraftingStations.None),
                    "BirchSeeds,1;FineWood,2",
                    pieceName: "Birch (autumn)",
                    clipEverything: true,
                    pieceGroup: PieceGroup.Flora
                )
            },
            {
                "BirchStub",
                new PrefabDB(
                    "BirchStub",
                    false,
                    false,
                    HammerCategories.Nature,
                    nameof(CraftingStations.None),
                    "",
                    clipEverything: true,
                    pieceGroup: PieceGroup.Flora
                )
            },
            {
                "blackmarble_2x2_enforced",
                new PrefabDB(
                    "blackmarble_2x2_enforced",
                    true,
                    false,
                    HammerCategories.Building,
                    nameof(CraftingStations.BlackForge),
                    "BlackMarble,8;Copper,2",
                    pieceGroup: PieceGroup.BlackMarble
                )
            },
            {
                "blackmarble_2x2x1",
                new PrefabDB(
                    "blackmarble_2x2x1",
                    false,
                    false,
                    HammerCategories.Building,
                    nameof(CraftingStations.Stonecutter),
                    "BlackMarble,4",
                    pieceGroup: PieceGroup.BlackMarble
                )
            },
            {
                "blackmarble_altar_crystal",
                new PrefabDB(
                    "blackmarble_altar_crystal",
                    false,
                    false,
                    HammerCategories.CreatorShop,
                    nameof(CraftingStations.None),
                    ""
                )
            },
            {
                "blackmarble_altar_crystal_broken",
                new PrefabDB(
                    "blackmarble_altar_crystal_broken",
                    false,
                    false,
                    HammerCategories.CreatorShop,
                    nameof(CraftingStations.None),
                    ""
                )
            },
            {
                "blackmarble_base_2",
                new PrefabDB(
                    "blackmarble_base_2",
                    true,
                    false,
                    HammerCategories.Building,
                    nameof(CraftingStations.Stonecutter),
                    "BlackMarble,6",
                    pieceName: "Black marble plinth (wide)",
                    pieceGroup: PieceGroup.BlackMarble
                )
            },
            {
                "blackmarble_column_3",
                new PrefabDB(
                    "blackmarble_column_3",
                    true,
                    false,
                    HammerCategories.Building,
                    nameof(CraftingStations.Stonecutter),
                    "BlackMarble,16",
                    pieceGroup: PieceGroup.BlackMarble
                )
            },
            {
                "blackmarble_creep_4x1x1",
                new PrefabDB(
                    "blackmarble_creep_4x1x1",
                    false,
                    false,
                    HammerCategories.Building,
                    nameof(CraftingStations.Stonecutter),
                    "BlackMarble,4",
                    pieceGroup: PieceGroup.BlackMarble
                )
            },
            {
                "blackmarble_creep_4x2x1",
                new PrefabDB(
                    "blackmarble_creep_4x2x1",
                    false,
                    false,
                    HammerCategories.Building,
                    nameof(CraftingStations.Stonecutter),
                    "BlackMarble,8",
                    pieceGroup: PieceGroup.BlackMarble
                )
            },
            {
                "blackmarble_creep_slope_inverted_1x1x2",
                new PrefabDB(
                    "blackmarble_creep_slope_inverted_1x1x2",
                    false,
                    false,
                    HammerCategories.Building,
                    nameof(CraftingStations.Stonecutter),
                    "BlackMarble,2",
                    pieceGroup: PieceGroup.BlackMarble
                )
            },
            {
                "blackmarble_creep_slope_inverted_2x2x1",
                new PrefabDB(
                    "blackmarble_creep_slope_inverted_2x2x1",
                    false,
                    false,
                    HammerCategories.Building,
                    nameof(CraftingStations.Stonecutter),
                    "BlackMarble,4",
                    pieceGroup: PieceGroup.BlackMarble
                )
            },
            {
                "blackmarble_creep_stair",
                new PrefabDB(
                    "blackmarble_creep_stair",
                    false,
                    false,
                    HammerCategories.Building,
                    nameof(CraftingStations.Stonecutter),
                    "BlackMarble,8",
                    pieceGroup: PieceGroup.BlackMarble
                )
            },
            {
                "blackmarble_floor_large",
                new PrefabDB(
                    "blackmarble_floor_large",
                    true,
                    false,
                    HammerCategories.Building,
                    nameof(CraftingStations.Stonecutter),
                    "BlackMarble,32",
                    pieceName: "Black marble floor 8x8",
                    clipGround: true,
                    pieceGroup: PieceGroup.BlackMarble
                )
            },
            {
                "blackmarble_head_big01",
                new PrefabDB(
                    "blackmarble_head_big01",
                    true,
                    false,
                    HammerCategories.Building,
                    nameof(CraftingStations.Stonecutter),
                    "BlackMarble,6",
                    pieceGroup: PieceGroup.BlackMarble
                )
            },
            {
                "blackmarble_head_big02",
                new PrefabDB(
                    "blackmarble_head_big02",
                    true,
                    false,
                    HammerCategories.Building,
                    nameof(CraftingStations.Stonecutter),
                    "BlackMarble,6",
                    pieceGroup: PieceGroup.BlackMarble
                )
            },
            {
                "blackmarble_head01",
                new PrefabDB(
                    "blackmarble_head01",
                    true,
                    false,
                    HammerCategories.Building,
                    nameof(CraftingStations.Stonecutter),
                    "BlackMarble,2",
                    pieceGroup: PieceGroup.BlackMarble
                )
            },
            {
                "blackmarble_head02",
                new PrefabDB(
                    "blackmarble_head02",
                    true,
                    false,
                    HammerCategories.Building,
                    nameof(CraftingStations.Stonecutter),
                    "BlackMarble,2",
                    pieceGroup: PieceGroup.BlackMarble
                )
            },
            {
                "blackmarble_out_2",
                new PrefabDB(
                    "blackmarble_out_2",
                    true,
                    false,
                    HammerCategories.Building,
                    nameof(CraftingStations.Stonecutter),
                    "BlackMarble,6",
                    pieceName: "Black marble cornice (wide)",
                    pieceGroup: PieceGroup.BlackMarble
                )
            },
            {
                "blackmarble_post01",
                new PrefabDB(
                    "blackmarble_post01",
                    true,
                    false,
                    HammerCategories.Building,
                    nameof(CraftingStations.Stonecutter),
                    "BlackMarble,8",
                    clipGround: true,
                    pieceGroup: PieceGroup.BlackMarble
                )
            },
            {
                "blackmarble_slope_1x2",
                new PrefabDB(
                    "blackmarble_slope_1x2",
                    true,
                    false,
                    HammerCategories.Building,
                    nameof(CraftingStations.Stonecutter),
                    "BlackMarble,2",
                    pieceGroup: PieceGroup.BlackMarble
                )
            },
            {
                "blackmarble_slope_inverted_1x2",
                new PrefabDB(
                    "blackmarble_slope_inverted_1x2",
                    true,
                    false,
                    HammerCategories.Building,
                    nameof(CraftingStations.Stonecutter),
                    "BlackMarble,2",
                    pieceGroup: PieceGroup.BlackMarble
                )
            },
            {
                "blackmarble_stair_corner",
                new PrefabDB(
                    "blackmarble_stair_corner",
                    true,
                    false,
                    HammerCategories.Building,
                    nameof(CraftingStations.Stonecutter),
                    "BlackMarble,8",
                    placementPatch: true,
                    pieceGroup: PieceGroup.BlackMarble
                )
            },
            {
                "blackmarble_stair_corner_left",
                new PrefabDB(
                    "blackmarble_stair_corner_left",
                    true,
                    false,
                    HammerCategories.Building,
                    nameof(CraftingStations.Stonecutter),
                    "BlackMarble,8",
                    placementPatch: true,
                    pieceGroup: PieceGroup.BlackMarble
                )
            },
            {
                "blackmarble_tile_floor_1x1",
                new PrefabDB(
                    "blackmarble_tile_floor_1x1",
                    true,
                    false,
                    HammerCategories.Building,
                    nameof(CraftingStations.Stonecutter),
                    "BlackMarble,2",
                    pieceGroup: PieceGroup.BlackMarble
                )
            },
            {
                "blackmarble_tile_floor_2x2",
                new PrefabDB(
                    "blackmarble_tile_floor_2x2",
                    true,
                    false,
                    HammerCategories.Building,
                    nameof(CraftingStations.Stonecutter),
                    "BlackMarble,2",
                    pieceGroup: PieceGroup.BlackMarble
                )
            },
            {
                "blackmarble_tile_wall_1x1",
                new PrefabDB(
                    "blackmarble_tile_wall_1x1",
                    true,
                    false,
                    HammerCategories.Building,
                    nameof(CraftingStations.Stonecutter),
                    "BlackMarble,1",
                    pieceGroup: PieceGroup.BlackMarble
                )
            },
            {
                "blackmarble_tile_wall_2x2",
                new PrefabDB(
                    "blackmarble_tile_wall_2x2",
                    true,
                    false,
                    HammerCategories.Building,
                    nameof(CraftingStations.Stonecutter),
                    "BlackMarble,2",
                    pieceGroup: PieceGroup.BlackMarble
                )
            },
            {
                "blackmarble_tile_wall_2x4",
                new PrefabDB(
                    "blackmarble_tile_wall_2x4",
                    true,
                    false,
                    HammerCategories.Building,
                    nameof(CraftingStations.Stonecutter),
                    "BlackMarble,4",
                    pieceGroup: PieceGroup.BlackMarble
                )
            },
            {
                "BlueberryBush",
                new PrefabDB(
                    "BlueberryBush",
                    false,
                    false,
                    HammerCategories.Nature,
                    nameof(CraftingStations.None),
                    "Blueberries,5",
                    pieceGroup: PieceGroup.Plant
                )
            },
            {
                "BossStone_Bonemass",
                new PrefabDB(
                    "BossStone_Bonemass",
                    false,
                    false,
                    HammerCategories.CreatorShop,
                    nameof(CraftingStations.None),
                    "",
                    pieceGroup: PieceGroup.Misc
                )
            },
            {
                "BossStone_DragonQueen",
                new PrefabDB(
                    "BossStone_DragonQueen",
                    false,
                    false,
                    HammerCategories.CreatorShop,
                    nameof(CraftingStations.None),
                    "",
                    pieceGroup: PieceGroup.Misc
                )
            },
            {
                "BossStone_Eikthyr",
                new PrefabDB(
                    "BossStone_Eikthyr",
                    false,
                    false,
                    HammerCategories.CreatorShop,
                    nameof(CraftingStations.None),
                    "",
                    pieceGroup: PieceGroup.Misc
                )
            },
            {
                "BossStone_TheElder",
                new PrefabDB(
                    "BossStone_TheElder",
                    false,
                    false,
                    HammerCategories.CreatorShop,
                    nameof(CraftingStations.None),
                    "",
                    pieceGroup: PieceGroup.Misc
                )
            },
            {
                "BossStone_TheQueen",
                new PrefabDB(
                    "BossStone_TheQueen",
                    false,
                    false,
                    HammerCategories.CreatorShop,
                    nameof(CraftingStations.None),
                    "",
                    pieceGroup: PieceGroup.Misc
                )
            },
            {
                "BossStone_Yagluth",
                new PrefabDB(
                    "BossStone_Yagluth",
                    false,
                    false,
                    HammerCategories.CreatorShop,
                    nameof(CraftingStations.None),
                    "",
                    pieceGroup: PieceGroup.Misc
                )
            },
            {
                "bucket",
                new PrefabDB(
                    "bucket",
                    true,
                    false,
                    HammerCategories.Misc,
                    nameof(CraftingStations.Workbench),
                    "Wood,2",
                    pieceGroup: PieceGroup.Misc
                )
            },
            {
                "Bush01",
                new PrefabDB(
                    "Bush01",
                    true,
                    false,
                    HammerCategories.Nature,
                    nameof(CraftingStations.None),
                    "Wood,2",
                    pieceGroup: PieceGroup.Flora
                )
            },
            {
                "Bush01_heath",
                new PrefabDB(
                    "Bush01_heath",
                    true,
                    false,
                    HammerCategories.Nature,
                    nameof(CraftingStations.None),
                    "Wood,2",
                    pieceGroup: PieceGroup.Flora
                )
            },
            {
                "Bush02_en",
                new PrefabDB(
                    "Bush02_en",
                    true,
                    false,
                    HammerCategories.Nature,
                    nameof(CraftingStations.None),
                    "Wood,2",
                    pieceGroup: PieceGroup.Flora
                )
            },
            {
                "CastleKit_braided_box01",
                new PrefabDB(
                    "CastleKit_braided_box01",
                    true,
                    false,
                    HammerCategories.Misc,
                    nameof(CraftingStations.Workbench),
                    "Wood,2",
                    pieceName: "Wood box",
                    pieceGroup: PieceGroup.Misc
                )
            },
            {
                "CastleKit_groundtorch",
                new PrefabDB(
                    "CastleKit_groundtorch",
                    true,
                    false,
                    HammerCategories.Furniture,
                    nameof(CraftingStations.Forge),
                    "Iron,2;Resin,2;BlackCore,1",
                    pieceName: "Standing iron torch (everburning)",
                    pieceDesc: "Burns eternally without fuel, but does not prevent spawning of monsters.",
                    pieceGroup: PieceGroup.Torch
                )
            },
            {
                "CastleKit_groundtorch_blue",
                new PrefabDB(
                    "CastleKit_groundtorch_blue",
                    true,
                    false,
                    HammerCategories.Furniture,
                    nameof(CraftingStations.Forge),
                    "Iron,2;GreydwarfEye,2;BlackCore,1",
                    pieceName: "Standing blue-burning iron torch (everburning)",
                    pieceDesc: "Burns eternally without fuel, but does not prevent spawning of monsters.",
                    pieceGroup: PieceGroup.Torch
                )
            },
            {
                "CastleKit_groundtorch_green",
                new PrefabDB(
                    "CastleKit_groundtorch_green",
                    true,
                    false,
                    HammerCategories.Furniture,
                    nameof(CraftingStations.Forge),
                    "Iron,2;Guck,2;BlackCore,1",
                    pieceName: "Standing green-burning iron torch (everburning)",
                    pieceDesc: "Burns eternally without fuel, but does not prevent spawning of monsters.",
                    pieceGroup: PieceGroup.Torch
                )
            },
            {
                "caverock_ice_pillar_wall",
                new PrefabDB(
                    "caverock_ice_pillar_wall",
                    false,
                    false,
                    HammerCategories.Nature,
                    nameof(CraftingStations.None),
                    "Crystal,10",
                    placementPatch: true,
                    pieceGroup: PieceGroup.Ice
                )
            },
            {
                "caverock_ice_stalagmite",
                new PrefabDB(
                    "caverock_ice_stalagmite",
                    false,
                    false,
                    HammerCategories.Nature,
                    nameof(CraftingStations.None),
                    "Crystal,2",
                    placementPatch: true,
                    pieceGroup: PieceGroup.Ice
                )
            },
            {
                "caverock_ice_stalagmite_broken",
                new PrefabDB(
                    "caverock_ice_stalagmite_broken",
                    false,
                    false,
                    HammerCategories.Nature,
                    nameof(CraftingStations.None),
                    "Crystal,4",
                    placementPatch: true,
                    pieceGroup: PieceGroup.Ice
                )
            },
            {
                "caverock_ice_stalagtite",
                new PrefabDB(
                    "caverock_ice_stalagtite",
                    false,
                    false,
                    HammerCategories.Nature,
                    nameof(CraftingStations.None),
                    "Crystal,2",
                    placementPatch: true,
                    pieceGroup: PieceGroup.Ice
                )
            },
            {
                "caverock_ice_stalagtite_falling",
                new PrefabDB(
                    "caverock_ice_stalagtite_falling",
                    false,
                    false,
                    HammerCategories.Nature,
                    nameof(CraftingStations.None),
                    "Crystal,2",
                    placementPatch: true,
                    pieceGroup: PieceGroup.Ice
                )
            },
            {
                "Chest",
                new PrefabDB(
                    "Chest",
                    false,
                    false,
                    HammerCategories.Furniture,
                    nameof(CraftingStations.None),
                    "Wood,10;Iron,1",
                    pieceGroup: PieceGroup.Chest
                )
            },
            {
                "cliff_mistlands1",
                new PrefabDB(
                    "cliff_mistlands1",
                    false,
                    false,
                    HammerCategories.Nature,
                    nameof(CraftingStations.None),
                    "Stone,350",
                    clipEverything: true,
                    pieceGroup: PieceGroup.Rock
                )
            },
            {
                "cliff_mistlands1_creep",
                new PrefabDB(
                    "cliff_mistlands1_creep",
                    false,
                    false,
                    HammerCategories.Nature,
                    nameof(CraftingStations.None),
                    "Stone,350",
                    clipEverything: true,
                    pieceGroup: PieceGroup.Rock
                )
            },
            {
                "cliff_mistlands2",
                new PrefabDB(
                    "cliff_mistlands2",
                    false,
                    false,
                    HammerCategories.Nature,
                    nameof(CraftingStations.None),
                    "Stone,175",
                    clipEverything: true,
                    pieceGroup: PieceGroup.Rock
                )
            },
            {
                "cloth_hanging_door",
                new PrefabDB(
                    "cloth_hanging_door",
                    false,
                    false,
                    HammerCategories.Furniture,
                    nameof(CraftingStations.None),
                    "",
                    pieceGroup: PieceGroup.Banner
                )
            },
            {
                "cloth_hanging_door_double",
                new PrefabDB(
                    "cloth_hanging_door_double",
                    true,
                    false,
                    HammerCategories.Furniture,
                    nameof(CraftingStations.Workbench),
                    "JuteRed,4",
                    pieceGroup: PieceGroup.Banner
                )
            },
            {
                "cloth_hanging_long",
                new PrefabDB(
                    "cloth_hanging_long",
                    false,
                    false,
                    HammerCategories.Furniture,
                    nameof(CraftingStations.None),
                    "",
                    pieceGroup: PieceGroup.Banner
                )
            },
            {
                "CloudberryBush",
                new PrefabDB(
                    "CloudberryBush",
                    false,
                    false,
                    HammerCategories.Nature,
                    nameof(CraftingStations.None),
                    "Cloudberry,5",
                    pieceGroup: PieceGroup.Plant
                )
            },
            {
                "CreepProp_egg_hanging01",
                new PrefabDB(
                    "CreepProp_egg_hanging01",
                    false,
                    false,
                    HammerCategories.Nature,
                    nameof(CraftingStations.None),
                    "YggdrasilWood,2"
                )
            },
            {
                "CreepProp_egg_hanging02",
                new PrefabDB(
                    "CreepProp_egg_hanging02",
                    false,
                    false,
                    HammerCategories.Nature,
                    nameof(CraftingStations.None),
                    "YggdrasilWood,2"
                )
            },
            {
                "CreepProp_entrance1",
                new PrefabDB(
                    "CreepProp_entrance1",
                    false,
                    false,
                    HammerCategories.Nature,
                    nameof(CraftingStations.None),
                    "YggdrasilWood,2",
                    placementPatch: true
                )
            },
            {
                "CreepProp_entrance2",
                new PrefabDB(
                    "CreepProp_entrance2",
                    false,
                    false,
                    HammerCategories.Nature,
                    nameof(CraftingStations.None),
                    "YggdrasilWood,2",
                    placementPatch: true
                )
            },
            {
                "CreepProp_hanging01",
                new PrefabDB(
                    "CreepProp_hanging01",
                    false,
                    false,
                    HammerCategories.Nature,
                    nameof(CraftingStations.None),
                    "YggdrasilWood,2"
                )
            },
            {
                "CreepProp_wall01",
                new PrefabDB(
                    "CreepProp_wall01",
                    false,
                    false,
                    HammerCategories.Nature,
                    nameof(CraftingStations.None),
                    "YggdrasilWood,2"
                )
            },
            {
                "crypt_skeleton_chest",
                new PrefabDB(
                    "crypt_skeleton_chest",
                    false,
                    false,
                    HammerCategories.CreatorShop,
                    nameof(CraftingStations.None),
                    ""
                )
            },
            {
                "dungeon_forestcrypt_door",
                new PrefabDB(
                    "dungeon_forestcrypt_door",
                    false,
                    false,
                    HammerCategories.CreatorShop,
                    nameof(CraftingStations.None),
                    ""
                )
            },
            {
                "dungeon_queen_door",
                new PrefabDB(
                    "dungeon_queen_door",
                    false,
                    false,
                    HammerCategories.CreatorShop,
                    nameof(CraftingStations.None),
                    "BlackMarble,40;DvergrKeyFragment,4;Iron,12"
                )
            },
            {
                "dungeon_sunkencrypt_irongate",
                new PrefabDB(
                    "dungeon_sunkencrypt_irongate",
                    false,
                    false,
                    HammerCategories.Building,
                    nameof(CraftingStations.None),
                    "Iron,4",
                    pieceGroup: PieceGroup.Iron
                )
            },
            {
                "dungeon_sunkencrypt_irongate_rusty",
                new PrefabDB(
                    "dungeon_sunkencrypt_irongate_rusty",
                    false,
                    false,
                    HammerCategories.Building,
                    nameof(CraftingStations.Forge),
                    "Iron,4",
                    clipGround: true,
                    pieceGroup: PieceGroup.Iron
                )
            },
            {
                "dverger_demister",
                new PrefabDB(
                    "dverger_demister",
                    true,
                    false,
                    HammerCategories.Furniture,
                    nameof(CraftingStations.BlackForge),
                    "Iron,1;Wisp,1",
                    pieceGroup: PieceGroup.Torch
                )
            },
            {
                "dverger_demister_broken",
                new PrefabDB(
                    "dverger_demister_broken",
                    false,
                    false,
                    HammerCategories.CreatorShop,
                    nameof(CraftingStations.None),
                    "",
                    pieceGroup: PieceGroup.Misc
                )
            },
            {
                "dverger_demister_large",
                new PrefabDB(
                    "dverger_demister_large",
                    true,
                    false,
                    HammerCategories.Furniture,
                    nameof(CraftingStations.BlackForge),
                    "Iron,1;Wisp,2",
                    pieceGroup: PieceGroup.Torch
                )
            },
            {
                "dverger_demister_ruins",
                new PrefabDB(
                    "dverger_demister_ruins",
                    false,
                    false,
                    HammerCategories.CreatorShop,
                    nameof(CraftingStations.None),
                    "",
                    pieceGroup: PieceGroup.Misc
                )
            },
            {
                "dverger_guardstone",
                new PrefabDB(
                    "dverger_guardstone",
                    true,
                    false,
                    HammerCategories.Misc,
                    nameof(CraftingStations.BlackForge),
                    "YggdrasilWood,5;BlackMarble,5;BlackCore,1"
                )
            },
            {
                "dvergrprops_banner",
                new PrefabDB(
                    "dvergrprops_banner",
                    true,
                    false,
                    HammerCategories.Furniture,
                    nameof(CraftingStations.Workbench),
                    "JuteBlue,6;FineWood,2",
                    pieceGroup: PieceGroup.Banner
                )
            },
            {
                "dvergrprops_barrel",
                new PrefabDB(
                    "dvergrprops_barrel",
                    true,
                    false,
                    HammerCategories.Furniture,
                    nameof(CraftingStations.Workbench),
                    "YggdrasilWood,20;Bronze,5;Resin,10",
                    pieceGroup: PieceGroup.Dvergr
                )
            },
            {
                "dvergrprops_bed",
                new PrefabDB(
                    "dvergrprops_bed",
                    true,
                    false,
                    HammerCategories.Furniture,
                    nameof(CraftingStations.BlackForge),
                    "YggdrasilWood,8;ScaleHide,2;IronNails,5",
                    pieceGroup: PieceGroup.Bed
                )
            },
            {
                "dvergrprops_chair",
                new PrefabDB(
                    "dvergrprops_chair",
                    true,
                    false,
                    HammerCategories.Furniture,
                    nameof(CraftingStations.BlackForge),
                    "YggdrasilWood,4",
                    pieceGroup: PieceGroup.Chair
                )
            },
            {
                "dvergrprops_crate",
                new PrefabDB(
                    "dvergrprops_crate",
                    true,
                    false,
                    HammerCategories.Misc,
                    nameof(CraftingStations.BlackForge),
                    "YggdrasilWood,4",
                    pieceGroup: PieceGroup.Misc
                )
            },
            {
                "dvergrprops_crate_long",
                new PrefabDB(
                    "dvergrprops_crate_long",
                    true,
                    false,
                    HammerCategories.Misc,
                    nameof(CraftingStations.BlackForge),
                    "YggdrasilWood,4",
                    pieceName: "Dvergr component crate",
                    pieceDesc: "",
                    pieceGroup: PieceGroup.Misc
                )
            },
            {
                "dvergrprops_curtain",
                new PrefabDB(
                    "dvergrprops_curtain",
                    true,
                    false,
                    HammerCategories.Furniture,
                    nameof(CraftingStations.Workbench),
                    "JuteBlue,6;FineWood,2",
                    pieceGroup: PieceGroup.Banner
                )
            },
            {
                "dvergrprops_hooknchain",
                new PrefabDB(
                    "dvergrprops_hooknchain",
                    true,
                    false,
                    HammerCategories.Misc,
                    nameof(CraftingStations.BlackForge),
                    "Chain,2;Iron,2",
                    pieceName: "Dvergr hook & chain",
                    pieceGroup: PieceGroup.Misc
                )
            },
            {
                "dvergrprops_lantern_standing",
                new PrefabDB(
                    "dvergrprops_lantern_standing",
                    false,
                    false,
                    HammerCategories.CreatorShop,
                    nameof(CraftingStations.None),
                    "",
                    pieceGroup: PieceGroup.Dvergr
                )
            },
            {
                "dvergrprops_pickaxe",
                new PrefabDB(
                    "dvergrprops_pickaxe",
                    false,
                    false,
                    HammerCategories.CreatorShop,
                    nameof(CraftingStations.None),
                    "",
                    pieceGroup: PieceGroup.Dvergr
                )
            },
            {
                "dvergrprops_shelf",
                new PrefabDB(
                    "dvergrprops_shelf",
                    true,
                    false,
                    HammerCategories.Furniture,
                    nameof(CraftingStations.BlackForge),
                    "YggdrasilWood,4;IronNails,5",
                    pieceGroup: PieceGroup.Dvergr
                )
            },
            {
                "dvergrprops_stool",
                new PrefabDB(
                    "dvergrprops_stool",
                    true,
                    false,
                    HammerCategories.Furniture,
                    nameof(CraftingStations.BlackForge),
                    "YggdrasilWood,2",
                    pieceGroup: PieceGroup.Chair
                )
            },
            {
                "dvergrprops_table",
                new PrefabDB(
                    "dvergrprops_table",
                    true,
                    false,
                    HammerCategories.Furniture,
                    nameof(CraftingStations.BlackForge),
                    "YggdrasilWood,6;IronNails,10",
                    pieceGroup: PieceGroup.Table
                )
            },
            {
                "dvergrprops_wood_beam",
                new PrefabDB(
                    "dvergrprops_wood_beam",
                    true,
                    false,
                    HammerCategories.Building,
                    nameof(CraftingStations.BlackForge),
                    "YggdrasilWood,4",
                    pieceGroup: PieceGroup.Dvergr
                )
            },
            {
                "dvergrprops_wood_floor",
                new PrefabDB(
                    "dvergrprops_wood_floor",
                    true,
                    false,
                    HammerCategories.Building,
                    nameof(CraftingStations.BlackForge),
                    "YggdrasilWood,2",
                    pieceGroup: PieceGroup.Dvergr
                )
            },
            {
                "dvergrprops_wood_pole",
                new PrefabDB(
                    "dvergrprops_wood_pole",
                    true,
                    false,
                    HammerCategories.Building,
                    nameof(CraftingStations.BlackForge),
                    "YggdrasilWood,2",
                    pieceGroup: PieceGroup.Dvergr
                )
            },
            {
                "dvergrprops_wood_stair",
                new PrefabDB(
                    "dvergrprops_wood_stair",
                    true,
                    false,
                    HammerCategories.Building,
                    nameof(CraftingStations.BlackForge),
                    "YggdrasilWood,2",
                    pieceGroup: PieceGroup.Dvergr
                )
            },
            {
                "dvergrprops_wood_stakewall",
                new PrefabDB(
                    "dvergrprops_wood_stakewall",
                    false,
                    false,
                    HammerCategories.Building,
                    nameof(CraftingStations.None),
                    "YggdrasilWood,4",
                    pieceGroup: PieceGroup.Dvergr
                )
            },
            {
                "dvergrprops_wood_wall",
                new PrefabDB(
                    "dvergrprops_wood_wall",
                    true,
                    false,
                    HammerCategories.Building,
                    nameof(CraftingStations.BlackForge),
                    "YggdrasilWood,10",
                    pieceName: "Dvergr wood wall 4x4",
                    pieceGroup: PieceGroup.Dvergr
                )
            },
            {
                "dvergrtown_arch",
                new PrefabDB(
                    "dvergrtown_arch",
                    true,
                    false,
                    HammerCategories.Building,
                    nameof(CraftingStations.Stonecutter),
                    "BlackMarble,8",
                    pieceGroup: PieceGroup.Dvergr
                )
            },
            {
                "dvergrtown_creep_door",
                new PrefabDB(
                    "dvergrtown_creep_door",
                    false,
                    false,
                    HammerCategories.Nature,
                    nameof(CraftingStations.None),
                    "YggdrasilWood,4",
                    pieceName: "Door hanging (creep)",
                    pieceDesc: "",
                    pieceGroup: PieceGroup.Misc
                )
            },
            {
                "dvergrtown_secretdoor",
                new PrefabDB(
                    "dvergrtown_secretdoor",
                    false,
                    false,
                    HammerCategories.CreatorShop,
                    nameof(CraftingStations.None),
                    "BlackMarble,12",
                    pieceName: "Dvergr secret door",
                    pieceGroup: PieceGroup.Dvergr
                )
            },
            {
                "dvergrtown_slidingdoor",
                new PrefabDB(
                    "dvergrtown_slidingdoor",
                    false,
                    false,
                    HammerCategories.CreatorShop,
                    nameof(CraftingStations.None),
                    "BlackMarble,6",
                    pieceName: "Dvergr sliding door",
                    pieceGroup: PieceGroup.Dvergr
                )
            },
            {
                "dvergrtown_stair_corner_wood_left",
                new PrefabDB(
                    "dvergrtown_stair_corner_wood_left",
                    true,
                    false,
                    HammerCategories.Building,
                    nameof(CraftingStations.BlackForge),
                    "YggdrasilWood,5;CopperScrap,2",
                    pieceGroup: PieceGroup.Dvergr
                )
            },
            {
                "dvergrtown_wood_beam",
                new PrefabDB(
                    "dvergrtown_wood_beam",
                    false,
                    false,
                    HammerCategories.Building,
                    nameof(CraftingStations.BlackForge),
                    "YggdrasilWood,4",
                    pieceName: "Dvergr wood beam (creep)",
                    pieceGroup: PieceGroup.Dvergr
                )
            },
            {
                "dvergrtown_wood_crane",
                new PrefabDB(
                    "dvergrtown_wood_crane",
                    false,
                    false,
                    HammerCategories.Building,
                    nameof(CraftingStations.BlackForge),
                    "YggdrasilWood,8;Chain,2;Iron,2",
                    pieceGroup: PieceGroup.Dvergr
                )
            },
            {
                "dvergrtown_wood_pole",
                new PrefabDB(
                    "dvergrtown_wood_pole",
                    false,
                    false,
                    HammerCategories.CreatorShop,
                    nameof(CraftingStations.None),
                    "",
                    pieceName: "Dvergr wood pole (creep)",
                    pieceGroup: PieceGroup.Dvergr
                )
            },
            {
                "dvergrtown_wood_stake",
                new PrefabDB(
                    "dvergrtown_wood_stake",
                    false,
                    false,
                    HammerCategories.Building,
                    nameof(CraftingStations.BlackForge),
                    "YggdrasilWood,1",
                    pieceGroup: PieceGroup.Dvergr
                )
            },
            {
                "dvergrtown_wood_stakewall",
                new PrefabDB(
                    "dvergrtown_wood_stakewall",
                    false,
                    false,
                    HammerCategories.Building,
                    nameof(CraftingStations.BlackForge),
                    "YggdrasilWood,4",
                    pieceGroup: PieceGroup.Dvergr
                )
            },
            {
                "dvergrtown_wood_support",
                new PrefabDB(
                    "dvergrtown_wood_support",
                    false,
                    false,
                    HammerCategories.Building,
                    nameof(CraftingStations.BlackForge),
                    "YggdrasilWood,10",
                    pieceGroup: PieceGroup.Dvergr
                )
            },
            {
                "dvergrtown_wood_wall01",
                new PrefabDB(
                    "dvergrtown_wood_wall01",
                    false,
                    false,
                    HammerCategories.Building,
                    nameof(CraftingStations.BlackForge),
                    "YggdrasilWood,20",
                    pieceGroup: PieceGroup.Dvergr
                )
            },
            {
                "dvergrtown_wood_wall02",
                new PrefabDB(
                    "dvergrtown_wood_wall02",
                    false,
                    false,
                    HammerCategories.Building,
                    nameof(CraftingStations.BlackForge),
                    "YggdrasilWood,12",
                    pieceGroup: PieceGroup.Dvergr
                )
            },
            {
                "dvergrtown_wood_wall03",
                new PrefabDB(
                    "dvergrtown_wood_wall03",
                    false,
                    false,
                    HammerCategories.Building,
                    nameof(CraftingStations.BlackForge),
                    "YggdrasilWood,10;IronNails,6",
                    pieceGroup: PieceGroup.Dvergr
                )
            },
            {
                "fenrirhide_hanging",
                new PrefabDB(
                    "fenrirhide_hanging",
                    false,
                    false,
                    HammerCategories.CreatorShop,
                    nameof(CraftingStations.None),
                    "WolfHairBundle,2"
                )
            },
            {
                "fenrirhide_hanging_door",
                new PrefabDB(
                    "fenrirhide_hanging_door",
                    false,
                    false,
                    HammerCategories.CreatorShop,
                    nameof(CraftingStations.None),
                    "WolfHairBundle,2"
                )
            },
            {
                "fire_pit_haldor",
                new PrefabDB(
                    "fire_pit_haldor",
                    true,
                    false,
                    HammerCategories.Misc,
                    nameof(CraftingStations.None),
                    "Stone,5;Wood,2;BlackCore,1",
                    pieceName: "Campfire (everburning)",
                    pieceDesc: "Burns eternally without fuel.",
                    pieceGroup: PieceGroup.Fire
                )
            },
            {
                "fire_pit_hildir",
                new PrefabDB(
                    "fire_pit_hildir",
                    true,
                    false,
                    HammerCategories.Misc,
                    nameof(CraftingStations.Forge),
                    "Ironpit,1;Wood,1;BlackCore,1",
                    pieceName: "Firepit iron (everburning)",
                    pieceDesc: "Burns eternally without fuel.",
                    pieceGroup: PieceGroup.Fire
                )
            },
            {
                "FirTree",
                new PrefabDB(
                    "FirTree",
                    false,
                    false,
                    HammerCategories.Nature,
                    nameof(CraftingStations.None),
                    "Wood,16",
                    pieceGroup: PieceGroup.Flora
                )
            },
            {
                "FirTree_log",
                new PrefabDB(
                    "FirTree_log",
                    false,
                    false,
                    HammerCategories.Nature,
                    nameof(CraftingStations.None),
                    "",
                    pieceGroup: PieceGroup.Flora
                )
            },
            {
                "FirTree_log_half",
                new PrefabDB(
                    "FirTree_log_half",
                    false,
                    false,
                    HammerCategories.Nature,
                    nameof(CraftingStations.None),
                    "",
                    pieceGroup: PieceGroup.Flora
                )
            },
            {
                "FirTree_oldLog",
                new PrefabDB(
                    "FirTree_oldLog",
                    false,
                    false,
                    HammerCategories.Nature,
                    nameof(CraftingStations.None),
                    "",
                    pieceGroup: PieceGroup.Flora
                )
            },
            {
                "FirTree_small",
                new PrefabDB(
                    "FirTree_small",
                    false,
                    false,
                    HammerCategories.Nature,
                    nameof(CraftingStations.None),
                    "",
                    pieceGroup: PieceGroup.Flora
                )
            },
            {
                "FirTree_small_dead",
                new PrefabDB(
                    "FirTree_small_dead",
                    false,
                    false,
                    HammerCategories.Nature,
                    nameof(CraftingStations.None),
                    "",
                    pieceGroup: PieceGroup.Flora
                )
            },
            {
                "FirTree_Stub",
                new PrefabDB(
                    "FirTree_Stub",
                    false,
                    false,
                    HammerCategories.Nature,
                    nameof(CraftingStations.None),
                    "",
                    clipEverything: true,
                    pieceGroup: PieceGroup.Flora
                )
            },
            {
                "flying_core",
                new PrefabDB(
                    "flying_core",
                    false,
                    false,
                    HammerCategories.CreatorShop,
                    nameof(CraftingStations.None),
                    "Crystal,24"
                )
            },
            {
                "fuling_trap",
                new PrefabDB(
                    "fuling_trap",
                    false,
                    false,
                    HammerCategories.CreatorShop,
                    nameof(CraftingStations.None),
                    ""
                )
            },
            {
                "giant_arm",
                new PrefabDB(
                    "giant_arm",
                    false,
                    false,
                    HammerCategories.CreatorShop,
                    nameof(CraftingStations.None),
                    "BlackMarble,32",
                    clipEverything: true
                )
            },
            {
                "giant_brain",
                new PrefabDB(
                    "giant_brain",
                    false,
                    false,
                    HammerCategories.CreatorShop,
                    nameof(CraftingStations.None),
                    "SoftTissue,64",
                    clipEverything: true
                )
            },
            {
                "giant_helmet1",
                new PrefabDB(
                    "giant_helmet1",
                    false,
                    false,
                    HammerCategories.CreatorShop,
                    nameof(CraftingStations.None),
                    "Iron,32",
                    clipEverything: true
                )
            },
            {
                "giant_helmet2",
                new PrefabDB(
                    "giant_helmet2",
                    false,
                    false,
                    HammerCategories.CreatorShop,
                    nameof(CraftingStations.None),
                    "Iron,8",
                    clipEverything: true
                )
            },
            {
                "giant_ribs",
                new PrefabDB(
                    "giant_ribs",
                    false,
                    false,
                    HammerCategories.CreatorShop,
                    nameof(CraftingStations.None),
                    "BlackMarble,64",
                    clipEverything: true
                )
            },
            {
                "giant_skull",
                new PrefabDB(
                    "giant_skull",
                    false,
                    false,
                    HammerCategories.CreatorShop,
                    nameof(CraftingStations.None),
                    "BlackMarble,32",
                    clipEverything: true
                )
            },
            {
                "giant_sword1",
                new PrefabDB(
                    "giant_sword1",
                    false,
                    false,
                    HammerCategories.CreatorShop,
                    nameof(CraftingStations.None),
                    "Iron,16",
                    clipEverything: true
                )
            },
            {
                "giant_sword2",
                new PrefabDB(
                    "giant_sword2",
                    false,
                    false,
                    HammerCategories.CreatorShop,
                    nameof(CraftingStations.None),
                    "Iron,16",
                    clipEverything: true
                )
            },
            {
                "GlowingMushroom",
                new PrefabDB(
                    "GlowingMushroom",
                    false,
                    false,
                    HammerCategories.Nature,
                    nameof(CraftingStations.None),
                    "Ooze,1",
                    placementPatch: true,
                    pieceGroup: PieceGroup.Flora
                )
            },
            {
                "goblin_banner",
                new PrefabDB(
                    "goblin_banner",
                    true,
                    false,
                    HammerCategories.Furniture,
                    nameof(CraftingStations.Workbench),
                    "Wood,1;DeerHide,1;Tar,1",
                    pieceGroup: PieceGroup.Banner
                )
            },
            {
                "goblin_bed",
                new PrefabDB(
                    "goblin_bed",
                    true,
                    false,
                    HammerCategories.Furniture,
                    nameof(CraftingStations.Workbench),
                    "Wood,8;Tar,1",
                    pieceGroup: PieceGroup.Bed
                )
            },
            {
                "goblin_fence",
                new PrefabDB(
                    "goblin_fence",
                    true,
                    false,
                    HammerCategories.Building,
                    nameof(CraftingStations.Workbench),
                    "Wood,2;Tar,1",
                    pieceGroup: PieceGroup.Goblin
                )
            },
            {
                "goblin_pole",
                new PrefabDB(
                    "goblin_pole",
                    true,
                    false,
                    HammerCategories.Building,
                    nameof(CraftingStations.Workbench),
                    "Wood,1;Tar,1",
                    pieceGroup: PieceGroup.Goblin
                )
            },
            {
                "goblin_pole_small",
                new PrefabDB(
                    "goblin_pole_small",
                    true,
                    false,
                    HammerCategories.Building,
                    nameof(CraftingStations.Workbench),
                    "Wood,1;Tar,1",
                    pieceGroup: PieceGroup.Goblin
                )
            },
            {
                "goblin_roof_45d",
                new PrefabDB(
                    "goblin_roof_45d",
                    true,
                    false,
                    HammerCategories.Building,
                    nameof(CraftingStations.Workbench),
                    "Wood,1;DeerHide,1;Tar,1",
                    placementPatch: true,
                    pieceGroup: PieceGroup.Goblin
                )
            },
            {
                "goblin_roof_45d_corner",
                new PrefabDB(
                    "goblin_roof_45d_corner",
                    true,
                    false,
                    HammerCategories.Building,
                    nameof(CraftingStations.Workbench),
                    "Wood,1;DeerHide,1;Tar,1",
                    pieceGroup: PieceGroup.Goblin
                )
            },
            {
                "goblin_roof_cap", // TODO: needs custom placement patching
                new PrefabDB(
                    "goblin_roof_cap",
                    true,
                    false,
                    HammerCategories.Building,
                    nameof(CraftingStations.Workbench),
                    "Wood,4;DeerHide,4;Tar,1",
                    clipEverything: true,
                    pieceGroup: PieceGroup.Goblin
                )
            },
            {
                "goblin_stairs",
                new PrefabDB(
                    "goblin_stairs",
                    true,
                    false,
                    HammerCategories.Building,
                    nameof(CraftingStations.Workbench),
                    "Wood,2;Tar,1",
                    pieceGroup: PieceGroup.Goblin
                )
            },
            {
                "goblin_stepladder",
                new PrefabDB(
                    "goblin_stepladder",
                    true,
                    false,
                    HammerCategories.Building,
                    nameof(CraftingStations.Workbench),
                    "Wood,2;Tar,1",
                    pieceGroup: PieceGroup.Goblin
                )
            },
            {
                "goblin_strawpile",
                new PrefabDB(
                    "goblin_strawpile",
                    true,
                    false,
                    HammerCategories.Furniture,
                    nameof(CraftingStations.None),
                    "Wood,4",
                    pieceName: "Rug straw (large)",
                    pieceDesc: "",
                    pieceGroup: PieceGroup.Rug
                )
            },
            {
                "goblin_totempole",
                new PrefabDB(
                    "goblin_totempole",
                    false,
                    false,
                    HammerCategories.CreatorShop,
                    nameof(CraftingStations.Workbench),
                    "Wood,2;GoblinTotem,1",
                    pieceGroup: PieceGroup.Goblin
                )
            },
            {
                "goblin_trashpile",
                new PrefabDB(
                    "goblin_trashpile",
                    false,
                    false,
                    HammerCategories.CreatorShop,
                    nameof(CraftingStations.None),
                    "",
                    pieceGroup: PieceGroup.Goblin
                )
            },
            {
                "goblin_woodwall_1m",
                new PrefabDB(
                    "goblin_woodwall_1m",
                    true,
                    false,
                    HammerCategories.Building,
                    nameof(CraftingStations.Workbench),
                    "Wood,1;Tar,1",
                    pieceGroup: PieceGroup.Goblin
                )
            },
            {
                "goblin_woodwall_2m",
                new PrefabDB(
                    "goblin_woodwall_2m",
                    true,
                    false,
                    HammerCategories.Building,
                    nameof(CraftingStations.Workbench),
                    "Wood,2;Tar,1",
                    pieceGroup: PieceGroup.Goblin
                )
            },
            {
                "goblin_woodwall_2m_ribs",
                new PrefabDB(
                    "goblin_woodwall_2m_ribs",
                    false,
                    false,
                    HammerCategories.CreatorShop,
                    nameof(CraftingStations.None),
                    "BoneFragments,6;Tar,1",
                    pieceGroup: PieceGroup.Goblin
                )
            },
            {
                "goblinking_totemholder",
                new PrefabDB(
                    "goblinking_totemholder",
                    false,
                    false,
                    HammerCategories.CreatorShop,
                    nameof(CraftingStations.Stonecutter),
                    "Stone,1;Tar,1",
                    pieceGroup: PieceGroup.Goblin
                )
            },
            {
                "Greydwarf_Root",
                new PrefabDB(
                    "Greydwarf_Root",
                    false,
                    false,
                    HammerCategories.Nature,
                    nameof(CraftingStations.None),
                    "Wood,2"
                )
            },
            {
                "GuckSack",
                new PrefabDB(
                    "GuckSack",
                    false,
                    false,
                    HammerCategories.CreatorShop,
                    nameof(CraftingStations.None),
                    "Guck,12",
                    clipEverything: true

                )
            },
            {
                "GuckSack_small",
                new PrefabDB(
                    "GuckSack_small",
                    false,
                    false,
                    HammerCategories.CreatorShop,
                    nameof(CraftingStations.None),
                    "Guck,6",
                    clipEverything:true
                )
            },
            {
                "hanging_hairstrands",
                new PrefabDB(
                    "hanging_hairstrands",
                    false,
                    false,
                    HammerCategories.CreatorShop,
                    nameof(CraftingStations.None),
                    ""
                )
            },
            {
                "highstone",
                new PrefabDB(
                    "highstone",
                    true,
                    false,
                    HammerCategories.Nature,
                    nameof(CraftingStations.None),
                    "Stone,32",
                    clipEverything: true,
                    pieceGroup: PieceGroup.Rock
                )
            },
            {
                "HugeRoot1",
                new PrefabDB(
                    "HugeRoot1",
                    false,
                    false,
                    HammerCategories.Nature,
                    nameof(CraftingStations.None),
                    "ElderBark,32",
                    clipEverything: true,
                    pieceGroup: PieceGroup.Flora
                )
            },
            {
                "Ice_floor",
                new PrefabDB(
                    "Ice_floor",
                    false,
                    false,
                    HammerCategories.CreatorShop,
                    nameof(CraftingStations.None),
                    "",
                    placementPatch: true
                )
            },
            {
                "ice_rock1",
                new PrefabDB(
                    "ice_rock1",
                    false,
                    false,
                    HammerCategories.CreatorShop,
                    nameof(CraftingStations.None),
                    "",
                    clipEverything: true
                )
            },
            {
                "ice1",
                new PrefabDB(
                    "ice1",
                    false,
                    false,
                    HammerCategories.CreatorShop,
                    nameof(CraftingStations.None),
                    ""
                )
            },
            {
                "iron_wall_1x1_rusty",
                new PrefabDB(
                    "iron_wall_1x1_rusty",
                    false,
                    false,
                    HammerCategories.Building,
                    nameof(CraftingStations.Forge),
                    "Iron,1",
                    pieceGroup: PieceGroup.Iron
                )
            },
            {
                "Leviathan",
                new PrefabDB(
                    "Leviathan",
                    false,
                    false,
                    HammerCategories.CreatorShop,
                    nameof(CraftingStations.None),
                    "Chitin,50",
                    clipEverything: true
                )
            },
            {
                "loot_chest_stone",
                new PrefabDB(
                    "loot_chest_stone",
                    false,
                    false,
                    HammerCategories.Furniture,
                    nameof(CraftingStations.Stonecutter),
                    "Stone,10",
                    pieceName: "Stone chest (light moss)",
                    pieceGroup: PieceGroup.Chest
                )
            },
            {
                "lox_ribs",
                new PrefabDB(
                    "lox_ribs",
                    false,
                    false,
                    HammerCategories.CreatorShop,
                    nameof(CraftingStations.None),
                    "BoneFragments,30"
                )
            },
            {
                "LuredWisp",
                new PrefabDB(
                    "LuredWisp",
                    false,
                    false,
                    HammerCategories.CreatorShop,
                    nameof(CraftingStations.None),
                    "Wisp,1"
                )
            },
            {
                "marker01",
                new PrefabDB(
                    "marker01",
                    true,
                    false,
                    HammerCategories.CreatorShop,
                    nameof(CraftingStations.None),
                    "Stone,4"
                )
            },
            {
                "marker02",
                new PrefabDB(
                    "marker02",
                    true,
                    false,
                    HammerCategories.CreatorShop,
                    nameof(CraftingStations.None),
                    "Stone,4"
                )
            },
            {
                "metalbar_1x2",
                new PrefabDB(
                    "metalbar_1x2",
                    true,
                    false,
                    HammerCategories.Building,
                    nameof(CraftingStations.BlackForge),
                    "BlackMarble,2;Copper,1",
                    pieceName: "Black marble 1x2 enforced",
                    pieceDesc: "",
                    pieceGroup: PieceGroup.BlackMarble
                )
            },
            {
                "MineRock_Copper",
                new PrefabDB(
                    "MineRock_Copper",
                    false,
                    false,
                    HammerCategories.Nature,
                    nameof(CraftingStations.None),
                    "Stone,10;CopperOre,10",
                    clipEverything: true,
                    pieceGroup: PieceGroup.Ore
                )
            },
            {
                "MineRock_Iron",
                new PrefabDB(
                    "MineRock_Iron",
                    false,
                    false,
                    HammerCategories.Nature,
                    nameof(CraftingStations.None),
                    "Stone,10;IronScrap,10",
                    clipEverything: true,
                    pieceGroup: PieceGroup.Ore
                )
            },
            {
                "MineRock_Meteorite",
                new PrefabDB(
                    "MineRock_Meteorite",
                    false,
                    false,
                    HammerCategories.Nature,
                    nameof(CraftingStations.None),
                    "",
                    clipEverything: true,
                    pieceGroup: PieceGroup.Ore
                )
            },
            {
                "MineRock_Obsidian",
                new PrefabDB(
                    "MineRock_Obsidian",
                    false,
                    false,
                    HammerCategories.Nature,
                    nameof(CraftingStations.None),
                    "Stone,10;Obsidian,10",
                    clipEverything: true,
                    pieceGroup: PieceGroup.Ore
                )
            },
            {
                "MineRock_Stone",
                new PrefabDB(
                    "MineRock_Stone",
                    false,
                    false,
                    HammerCategories.Nature,
                    nameof(CraftingStations.None),
                    "Stone,10",
                    clipEverything: true,
                    pieceGroup: PieceGroup.Ore
                )
            },
            {
                "MineRock_Tin",
                new PrefabDB(
                    "MineRock_Tin",
                    false,
                    false,
                    HammerCategories.Nature,
                    nameof(CraftingStations.None),
                    "Stone,10;TinOre,10",
                    clipEverything: true,
                    pieceGroup: PieceGroup.Ore
                )
            },
            {
                "mistvolume",
                new PrefabDB(
                    "mistvolume",
                    false,
                    false,
                    HammerCategories.CreatorShop,
                    nameof(CraftingStations.None),
                    "Eitr,10;Wisp,4",
                    clipEverything: true,
                    pieceName: "Mist volume",
                    pieceDesc: "Warning: requires devcommands to remove."
                )
            },
            {
                "MountainGraveStone01",
                new PrefabDB(
                    "MountainGraveStone01",
                    false,
                    false,
                    HammerCategories.CreatorShop,
                    nameof(CraftingStations.None),
                    "Stone,10",
                    clipEverything: true
                )
            },
            {
                "MountainKit_brazier",
                new PrefabDB(
                    "MountainKit_brazier",
                    true,
                    false,
                    HammerCategories.Furniture,
                    nameof(CraftingStations.Forge),
                    "Bronze,5;Coal,2;BlackCore,1;WolfClaw,3",
                    pieceName: "Standing brazier (everburning)",
                    pieceDesc: "Burns eternally without fuel, but does not prevent spawning of monsters.",
                    pieceGroup: PieceGroup.Brazier
                )
            },
            {
                "MountainKit_brazier_blue",
                new PrefabDB(
                    "MountainKit_brazier_blue",
                    true,
                    false,
                    HammerCategories.Furniture,
                    nameof(CraftingStations.Forge),
                    "Bronze,5;GreydwarfEye,2;BlackCore,1;WolfClaw,3",
                    pieceName: "Blue standing brazier (everburning)",
                    pieceDesc: "Burns eternally without fuel, but does not prevent spawning of monsters.",
                    pieceGroup: PieceGroup.Brazier
                )
            },
            {
                "mountainkit_chair",
                new PrefabDB(
                    "mountainkit_chair",
                    false,
                    false,
                    HammerCategories.Furniture,
                    nameof(CraftingStations.Workbench),
                    "FineWood,4",
                    pieceGroup: PieceGroup.Chair
                )
            },
            {
                "mountainkit_table",
                new PrefabDB(
                    "mountainkit_table",
                    false,
                    false,
                    HammerCategories.Furniture,
                    nameof(CraftingStations.Workbench),
                    "FineWood,10;IronNails,20",
                    pieceGroup: PieceGroup.Table
                )
            },
            {
                "MountainKit_wood_gate",
                new PrefabDB(
                    "MountainKit_wood_gate",
                    false,
                    false,
                    HammerCategories.Building,
                    nameof(CraftingStations.Forge),
                    "Wood,20;Iron,4"
                )
            },
            {
                "mudpile",
                new PrefabDB(
                    "mudpile",
                    false,
                    false,
                    HammerCategories.CreatorShop,
                    nameof(CraftingStations.None),
                    "IronScrap,10"
                )
            },
            {
                "mudpile_beacon",
                new PrefabDB(
                    "mudpile_beacon",
                    false,
                    false,
                    HammerCategories.CreatorShop,
                    nameof(CraftingStations.None),
                    ""
                )
            },
            {
                "mudpile_old",
                new PrefabDB(
                    "mudpile_old",
                    false,
                    false,
                    HammerCategories.CreatorShop,
                    nameof(CraftingStations.None),
                    "IronScrap,10"
                )
            },
            {
                "mudpile2",
                new PrefabDB(
                    "mudpile2",
                    false,
                    false,
                    HammerCategories.CreatorShop,
                    nameof(CraftingStations.None),
                    "IronScrap,10",
                    clipEverything: true
                )
            },
            {
                "Oak_log",
                new PrefabDB(
                    "Oak_log",
                    false,
                    false,
                    HammerCategories.Nature,
                    nameof(CraftingStations.None),
                    "Wood,10;FineWood,10",
                    pieceGroup: PieceGroup.Flora
                )
            },
            {
                "Oak_log_half",
                new PrefabDB(
                    "Oak_log_half",
                    false,
                    false,
                    HammerCategories.Nature,
                    nameof(CraftingStations.None),
                    "Wood,5;FineWood,5",
                    pieceGroup: PieceGroup.Flora
                )
            },
            {
                "Oak1",
                new PrefabDB(
                    "Oak1",
                    false,
                    false,
                    HammerCategories.Nature,
                    nameof(CraftingStations.None),
                    "Wood,20;FineWood,20",
                    clipEverything: true,
                    pieceGroup: PieceGroup.Flora
                )
            },
            {
                "OakStub",
                new PrefabDB(
                    "OakStub",
                    false,
                    false,
                    HammerCategories.Nature,
                    nameof(CraftingStations.None),
                    "Wood,5;FineWood,5",
                    clipEverything: true,
                    pieceGroup: PieceGroup.Flora
                )
            },
            {
                "Pickable_Barley",
                new PrefabDB(
                    "Pickable_Barley",
                    false,
                    false,
                    HammerCategories.Nature,
                    nameof(CraftingStations.None),
                    "Barley,1",
                    pieceGroup: PieceGroup.VanillaCrop
                )
            },
            {
                "Pickable_BlackCoreStand",
                new PrefabDB(
                    "Pickable_BlackCoreStand",
                    true,
                    false,
                    HammerCategories.CreatorShop,
                    nameof(CraftingStations.BlackForge),
                    "Iron,2;BlackCore,1",
                    pieceGroup: PieceGroup.Treasure
                )
            },
            {
                "Pickable_BogIronOre",
                new PrefabDB(
                    "Pickable_BogIronOre",
                    false,
                    false,
                    HammerCategories.CreatorShop,
                    nameof(CraftingStations.None),
                    "IronScrap,1",
                    clipEverything: true,
                    pieceGroup: PieceGroup.Ore
                )
            },
            {
                "Pickable_Branch",
                new PrefabDB(
                    "Pickable_Branch",
                    false,
                    false,
                    HammerCategories.Nature,
                    nameof(CraftingStations.None),
                    "Wood,5",
                    clipEverything: true,
                    pieceGroup: PieceGroup.Flora
                )
            },
            {
                "Pickable_Carrot",
                new PrefabDB(
                    "Pickable_Carrot",
                    false,
                    false,
                    HammerCategories.Nature,
                    nameof(CraftingStations.None),
                    "Carrot,1",
                    pieceGroup: PieceGroup.VanillaCrop
                )
            },
            {
                "Pickable_Dandelion",
                new PrefabDB(
                    "Pickable_Dandelion",
                    false,
                    false,
                    HammerCategories.Nature,
                    nameof(CraftingStations.None),
                    "Dandelion,5",
                    clipEverything: true,
                    pieceGroup: PieceGroup.Plant
                )
            },
            {
                "Pickable_DolmenTreasure",
                new PrefabDB(
                    "Pickable_DolmenTreasure",
                    false,
                    false,
                    HammerCategories.CreatorShop,
                    nameof(CraftingStations.None),
                    "Coins,5",
                    pieceGroup: PieceGroup.Treasure
                )
            },
            {
                "Pickable_DragonEgg",
                new PrefabDB(
                    "Pickable_DragonEgg",
                    false,
                    false,
                    HammerCategories.CreatorShop,
                    nameof(CraftingStations.None),
                    "DragonEgg,1",
                    clipEverything: true
                )
            },
            {
                "Pickable_DvergrLantern",
                new PrefabDB(
                    "Pickable_DvergrLantern",
                    false,
                    false,
                    HammerCategories.CreatorShop,
                    nameof(CraftingStations.None),
                    "",
                    clipEverything: true
                )
            },
            {
                "Pickable_DvergrMineTreasure",
                new PrefabDB(
                    "Pickable_DvergrMineTreasure",
                    false,
                    false,
                    HammerCategories.CreatorShop,
                    nameof(CraftingStations.None),
                    "",
                    clipEverything: true,
                    pieceGroup: PieceGroup.Treasure
                )
            },
            {
                "Pickable_DvergrStein",
                new PrefabDB(
                    "Pickable_DvergrStein",
                    false,
                    false,
                    HammerCategories.CreatorShop,
                    nameof(CraftingStations.None),
                    "",
                    clipEverything: true,
                    pieceGroup: PieceGroup.Treasure
                )
            },
            {
                "Pickable_Fishingrod",
                new PrefabDB(
                    "Pickable_Fishingrod",
                    false,
                    false,
                    HammerCategories.CreatorShop,
                    nameof(CraftingStations.None),
                    "",
                    clipEverything: true,
                    pieceGroup: PieceGroup.Treasure
                )
            },
            {
                "Pickable_Flax",
                new PrefabDB(
                    "Pickable_Flax",
                    false,
                    false,
                    HammerCategories.Nature,
                    nameof(CraftingStations.None),
                    "Flax,1",
                    pieceGroup: PieceGroup.VanillaCrop
                )
            },
            {
                "Pickable_Flint",
                new PrefabDB(
                    "Pickable_Flint",
                    false,
                    false,
                    HammerCategories.Nature,
                    nameof(CraftingStations.None),
                    "Flint,5",
                    clipEverything: true,
                    pieceGroup: PieceGroup.Flora
                )
            },
            {
                "Pickable_ForestCryptRemains01",
                new PrefabDB(
                    "Pickable_ForestCryptRemains01",
                    false,
                    false,
                    HammerCategories.CreatorShop,
                    nameof(CraftingStations.None),
                    "",
                    clipEverything: true,
                    pieceGroup: PieceGroup.Treasure
                )
            },
            {
                "Pickable_ForestCryptRemains02",
                new PrefabDB(
                    "Pickable_ForestCryptRemains02",
                    false,
                    false,
                    HammerCategories.CreatorShop,
                    nameof(CraftingStations.None),
                    "",
                    clipEverything: true,
                    pieceGroup: PieceGroup.Treasure
                )
            },
            {
                "Pickable_ForestCryptRemains03",
                new PrefabDB(
                    "Pickable_ForestCryptRemains03",
                    false,
                    false,
                    HammerCategories.CreatorShop,
                    nameof(CraftingStations.None),
                    "",
                    clipEverything: true,
                    pieceGroup: PieceGroup.Treasure
                )
            },
            {
                "Pickable_ForestCryptRemains04",
                new PrefabDB(
                    "Pickable_ForestCryptRemains04",
                    false,
                    false,
                    HammerCategories.CreatorShop,
                    nameof(CraftingStations.None),
                    "",
                    clipEverything: true,
                    pieceGroup: PieceGroup.Treasure
                )
            },
            {
                "Pickable_Hairstrands01",
                new PrefabDB(
                    "Pickable_Hairstrands01",
                    false,
                    false,
                    HammerCategories.CreatorShop,
                    nameof(CraftingStations.None),
                    "",
                    pieceGroup: PieceGroup.Treasure
                )
            },
            {
                "Pickable_Hairstrands02",
                new PrefabDB(
                    "Pickable_Hairstrands02",
                    false,
                    false,
                    HammerCategories.CreatorShop,
                    nameof(CraftingStations.None),
                    "",
                    pieceGroup: PieceGroup.Treasure
                )
            },
            {
                "Pickable_Item",
                new PrefabDB(
                    "Pickable_Item",
                    false,
                    false,
                    HammerCategories.CreatorShop,
                    nameof(CraftingStations.None),
                    "",
                    pieceName: "Coins (pickable)",
                    pieceGroup: PieceGroup.Treasure
                )
            },
            {
                "Pickable_MeatPile",
                new PrefabDB(
                    "Pickable_MeatPile",
                    false,
                    false,
                    HammerCategories.CreatorShop,
                    nameof(CraftingStations.None),
                    "",
                    clipEverything: true,
                    pieceGroup: PieceGroup.Treasure
                )
            },
            {
                "Pickable_Meteorite",
                new PrefabDB(
                    "Pickable_Meteorite",
                    false,
                    false,
                    HammerCategories.Nature,
                    nameof(CraftingStations.None),
                    "",
                    clipEverything: true,
                    pieceGroup: PieceGroup.Ore
                )
            },
            {
                "Pickable_MountainCaveCrystal",
                new PrefabDB(
                    "Pickable_MountainCaveCrystal",
                    false,
                    false,
                    HammerCategories.Nature,
                    nameof(CraftingStations.None),
                    "",
                    pieceGroup: PieceGroup.Ore
                )
            },
            {
                "Pickable_MountainCaveObsidian",
                new PrefabDB(
                    "Pickable_MountainCaveObsidian",
                    false,
                    false,
                    HammerCategories.Nature,
                    nameof(CraftingStations.None),
                    "",
                    pieceGroup: PieceGroup.Ore
                )
            },
            {
                "Pickable_MountainRemains01_buried",
                new PrefabDB(
                    "Pickable_MountainRemains01_buried",
                    false,
                    false,
                    HammerCategories.CreatorShop,
                    nameof(CraftingStations.None),
                    "",
                    clipEverything: true,
                    pieceGroup: PieceGroup.Treasure
                )
            },
            {
                "Pickable_Mushroom",
                new PrefabDB(
                    "Pickable_Mushroom",
                    false,
                    false,
                    HammerCategories.Nature,
                    nameof(CraftingStations.None),
                    "Mushroom,5",
                    clipEverything: true,
                    pieceGroup: PieceGroup.Plant
                )
            },
            {
                "Pickable_Mushroom_blue",
                new PrefabDB(
                    "Pickable_Mushroom_blue",
                    false,
                    false,
                    HammerCategories.Nature,
                    nameof(CraftingStations.None),
                    "MushroomBlue,5",
                    clipEverything: true,
                    pieceGroup: PieceGroup.Plant
                )
            },
            {
                "Pickable_Mushroom_JotunPuffs",
                new PrefabDB(
                    "Pickable_Mushroom_JotunPuffs",
                    false,
                    false,
                    HammerCategories.Nature,
                    nameof(CraftingStations.None),
                    "MushroomJotunPuffs,1",
                    clipEverything: true,
                    pieceGroup: PieceGroup.VanillaCrop
                )
            },
            {
                "Pickable_Mushroom_Magecap",
                new PrefabDB(
                    "Pickable_Mushroom_Magecap",
                    false,
                    false,
                    HammerCategories.Nature,
                    nameof(CraftingStations.None),
                    "MushroomMagecap,1",
                    clipEverything: true,
                    pieceGroup: PieceGroup.VanillaCrop
                )
            },
            {
                "Pickable_Mushroom_yellow",
                new PrefabDB(
                    "Pickable_Mushroom_yellow",
                    false,
                    false,
                    HammerCategories.Nature,
                    nameof(CraftingStations.None),
                    "MushroomYellow,5",
                    clipEverything: true,
                    pieceGroup: PieceGroup.Plant
                )
            },
            {
                "Pickable_Obsidian",
                new PrefabDB(
                    "Pickable_Obsidian",
                    false,
                    false,
                    HammerCategories.Nature,
                    nameof(CraftingStations.None),
                    "Obsidian,1",
                    clipEverything: true,
                    pieceGroup: PieceGroup.Ore
                )
            },
            {
                "Pickable_Onion",
                new PrefabDB(
                    "Pickable_Onion",
                    false,
                    false,
                    HammerCategories.Nature,
                    nameof(CraftingStations.None),
                    "Obsidian,1",
                    pieceGroup: PieceGroup.VanillaCrop
                )
            },
            {
                "Pickable_RoyalJelly",
                new PrefabDB(
                    "Pickable_RoyalJelly",
                    false,
                    false,
                    HammerCategories.CreatorShop,
                    nameof(CraftingStations.None),
                    ""
                )
            },
            {
                "Pickable_SeedCarrot",
                new PrefabDB(
                    "Pickable_SeedCarrot",
                    false,
                    false,
                    HammerCategories.Nature,
                    nameof(CraftingStations.None),
                    "",
                    clipEverything: true,
                    pieceGroup: PieceGroup.VanillaCrop
                )
            },
            {
                "Pickable_SeedOnion",
                new PrefabDB(
                    "Pickable_SeedOnion",
                    false,
                    false,
                    HammerCategories.Nature,
                    nameof(CraftingStations.None),
                    "",
                    clipEverything: true,
                    pieceGroup: PieceGroup.VanillaCrop
                )
            },
            {
                "Pickable_SeedTurnip",
                new PrefabDB(
                    "Pickable_SeedTurnip",
                    false,
                    false,
                    HammerCategories.Nature,
                    nameof(CraftingStations.None),
                    "",
                    clipEverything: true,
                    pieceGroup: PieceGroup.VanillaCrop
                )
            },
            {
                "Pickable_Stone",
                new PrefabDB(
                    "Pickable_Stone",
                    false,
                    false,
                    HammerCategories.Nature,
                    nameof(CraftingStations.None),
                    "Stone,5",
                    clipEverything: true,
                    pieceGroup: PieceGroup.Flora
                )
            },
            {
                "Pickable_SurtlingCoreStand",
                new PrefabDB(
                    "Pickable_SurtlingCoreStand",
                    true,
                    false,
                    HammerCategories.CreatorShop,
                    nameof(CraftingStations.Forge),
                    "Iron,2;SurtlingCore,1",
                    pieceGroup: PieceGroup.Treasure
                )
            },
            {
                "Pickable_Tar",
                new PrefabDB(
                    "Pickable_Tar",
                    false,
                    false,
                    HammerCategories.Nature,
                    nameof(CraftingStations.None),
                    "Tar,1",
                    clipEverything: true,
                    pieceGroup: PieceGroup.Flora
                )
            },
            {
                "Pickable_TarBig",
                new PrefabDB(
                    "Pickable_TarBig",
                    false,
                    false,
                    HammerCategories.Nature,
                    nameof(CraftingStations.None),
                    "",
                    clipEverything: true,
                    pieceGroup: PieceGroup.Flora
                )
            },
            {
                "Pickable_Thistle",
                new PrefabDB(
                    "Pickable_Thistle",
                    false,
                    false,
                    HammerCategories.Nature,
                    nameof(CraftingStations.None),
                    "Thistle,5",
                    clipEverything: true,
                    pieceGroup: PieceGroup.Plant
                )
            },
            {
                "Pickable_Tin",
                new PrefabDB(
                    "Pickable_Tin",
                    false,
                    false,
                    HammerCategories.Nature,
                    nameof(CraftingStations.None),
                    "TinOre,1",
                    clipEverything: true,
                    pieceGroup: PieceGroup.Ore
                )
            },
            {
                "Pickable_Turnip",
                new PrefabDB(
                    "Pickable_Turnip",
                    false,
                    false,
                    HammerCategories.Nature,
                    nameof(CraftingStations.None),
                    "Turnip,1",
                    pieceGroup: PieceGroup.VanillaCrop
                )
            },
            {
                "piece_dvergr_pole",
                new PrefabDB(
                    "piece_dvergr_pole",
                    true,
                    false,
                    HammerCategories.Building,
                    nameof(CraftingStations.BlackForge),
                    "YggdrasilWood,1",
                    pieceGroup: PieceGroup.Dvergr
                )
            },
            {
                "piece_dvergr_wood_door",
                new PrefabDB(
                    "piece_dvergr_wood_door",
                    false,
                    false,
                    HammerCategories.Building,
                    nameof(CraftingStations.BlackForge),
                    "YggdrasilWood,16;IronNails,24",
                    pieceGroup: PieceGroup.Dvergr
                )
            },
            {
                "piece_dvergr_wood_wall",
                new PrefabDB(
                    "piece_dvergr_wood_wall",
                    true,
                    false,
                    HammerCategories.Building,
                    nameof(CraftingStations.BlackForge),
                    "YggdrasilWood,5",
                    pieceGroup: PieceGroup.Dvergr
                )
            },
            {
                "PineTree",
                new PrefabDB(
                    "PineTree",
                    false,
                    false,
                    HammerCategories.Nature,
                    nameof(CraftingStations.None),
                    "",
                    clipEverything: true,
                    pieceGroup: PieceGroup.Flora
                )
            },
            {
                "Pinetree_01",
                new PrefabDB(
                    "Pinetree_01",
                    false,
                    false,
                    HammerCategories.Nature,
                    nameof(CraftingStations.None),
                    "Wood,10;RoundLog,10",
                    clipEverything: true,
                    pieceGroup: PieceGroup.Flora
                )
            },
            {
                "Pinetree_01_Stub",
                new PrefabDB(
                    "Pinetree_01_Stub",
                    false,
                    false,
                    HammerCategories.Nature,
                    nameof(CraftingStations.None),
                    "Wood,5;RoundLog,5",
                    clipEverything: true,
                    pieceGroup: PieceGroup.Flora
                )
            },
            {
                "PineTree_log",
                new PrefabDB(
                    "PineTree_log",
                    false,
                    false,
                    HammerCategories.Nature,
                    nameof(CraftingStations.None),
                    "Wood,10;RoundLog,10",
                    pieceGroup: PieceGroup.Flora
                )
            },
            {
                "PineTree_log_half",
                new PrefabDB(
                    "PineTree_log_half",
                    false,
                    false,
                    HammerCategories.Nature,
                    nameof(CraftingStations.None),
                    "Wood,5;RoundLog,5",
                    pieceGroup: PieceGroup.Flora
                )
            },
            {
                "RaspberryBush",
                new PrefabDB(
                    "RaspberryBush",
                    false,
                    false,
                    HammerCategories.Nature,
                    nameof(CraftingStations.None),
                    "Raspberry,5",
                    pieceGroup: PieceGroup.Plant
                )
            },
            {
                "Rock_3",
                new PrefabDB(
                    "Rock_3",
                    true,
                    false,
                    HammerCategories.Nature,
                    nameof(CraftingStations.None),
                    "Stone,24",
                    clipEverything: true,
                    pieceGroup: PieceGroup.Rock
                )
            },
            {
                "Rock_4",
                new PrefabDB(
                    "Rock_4",
                    true,
                    false,
                    HammerCategories.Nature,
                    nameof(CraftingStations.None),
                    "Stone,16",
                    clipEverything: true,
                    pieceGroup: PieceGroup.Rock
                )
            },
            {
                "Rock_4_plains",
                new PrefabDB(
                    "Rock_4_plains",
                    true,
                    false,
                    HammerCategories.Nature,
                    nameof(CraftingStations.None),
                    "Stone,24",
                    clipEverything: true,
                    pieceGroup: PieceGroup.Rock
                )
            },
            {
                "Rock_7",
                new PrefabDB(
                    "Rock_7",
                    true,
                    false,
                    HammerCategories.Nature,
                    nameof(CraftingStations.None),
                    "Stone,32",
                    clipEverything: true,
                    pieceGroup: PieceGroup.Rock
                )
            },
            {
                "Rock_destructible",
                new PrefabDB(
                    "Rock_destructible",
                    true,
                    false,
                    HammerCategories.Nature,
                    nameof(CraftingStations.None),
                    "Stone,24",
                    clipEverything: true,
                    pieceGroup: PieceGroup.Rock
                )
            },
            {
                "Rock_destructible_test",
                new PrefabDB(
                    "Rock_destructible_test",
                    true,
                    false,
                    HammerCategories.Nature,
                    nameof(CraftingStations.None),
                    "Stone,32",
                    clipEverything: true,
                    pieceGroup: PieceGroup.Rock
                )
            },
            {
                "rock_mistlands1",
                new PrefabDB(
                    "rock_mistlands1",
                    true,
                    false,
                    HammerCategories.Nature,
                    nameof(CraftingStations.None),
                    "Stone,48",
                    clipEverything: true,
                    pieceName: "Rock (black)",
                    pieceGroup: PieceGroup.Rock
                )
            },
            {
                "rock1_mistlands",
                new PrefabDB(
                    "rock1_mistlands",
                    false,
                    false,
                    HammerCategories.Nature,
                    nameof(CraftingStations.None),
                    "",
                    clipEverything: true,
                    pieceName: "Rock (large boulder)",
                    pieceGroup: PieceGroup.Rock
                )
            },
            {
                "rock1_mountain",
                new PrefabDB(
                    "rock1_mountain",
                    false,
                    false,
                    HammerCategories.Nature,
                    nameof(CraftingStations.None),
                    "",
                    clipEverything: true,
                    pieceGroup: PieceGroup.Rock
                )
            },
            {
                "rock2_heath",
                new PrefabDB(
                    "rock2_heath",
                    false,
                    false,
                    HammerCategories.Nature,
                    nameof(CraftingStations.None),
                    "",
                    clipEverything: true,
                    pieceGroup: PieceGroup.Rock
                )
            },
            {
                "rock2_mountain",
                new PrefabDB(
                    "rock2_mountain",
                    false,
                    false,
                    HammerCategories.Nature,
                    nameof(CraftingStations.None),
                    "",
                    clipEverything: true,
                    pieceGroup: PieceGroup.Rock
                )
            },
            {
                "rock3_ice",
                new PrefabDB(
                    "rock3_ice",
                    false,
                    false,
                    HammerCategories.Nature,
                    nameof(CraftingStations.None),
                    "",
                    clipEverything: true,
                    pieceGroup: PieceGroup.Rock
                )
            },
            {
                "rock3_mountain",
                new PrefabDB(
                    "rock3_mountain",
                    false,
                    false,
                    HammerCategories.Nature,
                    nameof(CraftingStations.None),
                    "",
                    clipEverything: true,
                    pieceGroup: PieceGroup.Rock
                )
            },
            {
                "rock3_silver",
                new PrefabDB(
                    "rock3_silver",
                    false,
                    false,
                    HammerCategories.Nature,
                    nameof(CraftingStations.None),
                    "",
                    clipEverything: true,
                    pieceGroup: PieceGroup.Ore
                )
            },
            {
                "rock4_coast",
                new PrefabDB(
                    "rock4_coast",
                    false,
                    false,
                    HammerCategories.Nature,
                    nameof(CraftingStations.None),
                    "",
                    clipEverything: true,
                    pieceGroup: PieceGroup.Rock
                )
            },
            {
                "rock4_copper",
                new PrefabDB(
                    "rock4_copper",
                    false,
                    false,
                    HammerCategories.Nature,
                    nameof(CraftingStations.None),
                    "",
                    clipEverything: true,
                    pieceGroup: PieceGroup.Ore
                )
            },
            {
                "rock4_copper_frac",
                new PrefabDB(
                    "rock4_copper_frac",
                    false,
                    false,
                    HammerCategories.Nature,
                    nameof(CraftingStations.None),
                    "",
                    clipEverything: true,
                    pieceGroup: PieceGroup.Ore
                )
            },
            {
                "rock4_forest",
                new PrefabDB(
                    "rock4_forest",
                    false,
                    false,
                    HammerCategories.Nature,
                    nameof(CraftingStations.None),
                    "",
                    clipEverything: true,
                    pieceGroup: PieceGroup.Rock
                )
            },
            {
                "rock4_heath",
                new PrefabDB(
                    "rock4_heath",
                    false,
                    false,
                    HammerCategories.Nature,
                    nameof(CraftingStations.None),
                    "",
                    clipEverything: true,
                    pieceGroup: PieceGroup.Rock
                )
            },
            {
                "RockDolmen_1",
                new PrefabDB(
                    "RockDolmen_1",
                    true,
                    false,
                    HammerCategories.Nature,
                    nameof(CraftingStations.None),
                    "Stone,75",
                    clipEverything: true,
                    pieceGroup: PieceGroup.Rock
                )
            },
            {
                "RockDolmen_2",
                new PrefabDB(
                    "RockDolmen_2",
                    true,
                    false,
                    HammerCategories.Nature,
                    nameof(CraftingStations.None),
                    "Stone,50",
                    clipEverything: true,
                    pieceGroup: PieceGroup.Rock
                )
            },
            {
                "RockDolmen_3",
                new PrefabDB(
                    "RockDolmen_3",
                    true,
                    false,
                    HammerCategories.Nature,
                    nameof(CraftingStations.None),
                    "Stone,75",
                    clipEverything: true,
                    pieceGroup: PieceGroup.Rock
                )
            },
            {
                "RockFinger",
                new PrefabDB(
                    "RockFinger",
                    false,
                    false,
                    HammerCategories.CreatorShop,
                    nameof(CraftingStations.None),
                    "Stone,350",
                    clipEverything: true
                )
            },
            {
                "RockFingerBroken",
                new PrefabDB(
                    "RockFingerBroken",
                    false,
                    false,
                    HammerCategories.CreatorShop,
                    nameof(CraftingStations.None),
                    "Stone,175",
                    clipEverything: true
                )
            },
            {
                "rockformation1",
                new PrefabDB(
                    "rockformation1",
                    false,
                    false,
                    HammerCategories.Nature,
                    nameof(CraftingStations.None),
                    "Stone,350",
                    clipEverything: true,
                    pieceGroup: PieceGroup.Rock
                )
            },
            {
                "RockThumb",
                new PrefabDB(
                    "RockThumb",
                    false,
                    false,
                    HammerCategories.CreatorShop,
                    nameof(CraftingStations.None),
                    "Stone,200",
                    clipEverything: true
                )
            },
            {
                "root07",
                new PrefabDB(
                    "root07",
                    false,
                    false,
                    HammerCategories.Nature,
                    nameof(CraftingStations.None),
                    "ElderBark,2",
                    clipEverything: true,
                    pieceGroup: PieceGroup.Flora
                )
            },
            {
                "root08",
                new PrefabDB(
                    "root08",
                    false,
                    false,
                    HammerCategories.Nature,
                    nameof(CraftingStations.None),
                    "ElderBark,4",
                    clipEverything: true,
                    pieceGroup: PieceGroup.Flora
                )
            },
            {
                "root11",
                new PrefabDB(
                    "root11",
                    false,
                    false,
                    HammerCategories.Nature,
                    nameof(CraftingStations.None),
                    "ElderBark,4",
                    clipEverything: true,
                    pieceGroup: PieceGroup.Flora
                )
            },
            {
                "root12",
                new PrefabDB(
                    "root12",
                    false,
                    false,
                    HammerCategories.Nature,
                    nameof(CraftingStations.None),
                    "ElderBark,4",
                    clipEverything: true,
                    pieceGroup: PieceGroup.Flora
                )
            },
            {
                "rug_straw",
                new PrefabDB(
                    "rug_straw",
                    true,
                    false,
                    HammerCategories.Furniture,
                    nameof(CraftingStations.None),
                    "Wood,2",
                    pieceGroup: PieceGroup.Rug
                )
            },
            {
                "SeekerEgg",
                new PrefabDB(
                    "SeekerEgg",
                    false,
                    false,
                    HammerCategories.CreatorShop,
                    nameof(CraftingStations.None),
                    ""
                )
            },
            {
                "SeekerEgg_alwayshatch",
                new PrefabDB(
                    "SeekerEgg_alwayshatch",
                    false,
                    false,
                    HammerCategories.CreatorShop,
                    nameof(CraftingStations.None),
                    ""
                )
            },
            {
                "shipwreck_karve_bottomboards",
                new PrefabDB(
                    "shipwreck_karve_bottomboards",
                    false,
                    false,
                    HammerCategories.CreatorShop,
                    nameof(CraftingStations.None),
                    "FineWood,8",
                    clipEverything: true
                )
            },
            {
                "shipwreck_karve_bow",
                new PrefabDB(
                    "shipwreck_karve_bow",
                    false,
                    false,
                    HammerCategories.CreatorShop,
                    nameof(CraftingStations.None),
                    "FineWood,16",
                    clipEverything: true
                )
            },
            {
                "shipwreck_karve_dragonhead",
                new PrefabDB(
                    "shipwreck_karve_dragonhead",
                    false,
                    false,
                    HammerCategories.CreatorShop,
                    nameof(CraftingStations.None),
                    "FineWood,8",
                    clipEverything: true
                )
            },
            {
                "shipwreck_karve_stern",
                new PrefabDB(
                    "shipwreck_karve_stern",
                    false,
                    false,
                    HammerCategories.CreatorShop,
                    nameof(CraftingStations.None),
                    "FineWood,16",
                    clipEverything: true
                )
            },
            {
                "shipwreck_karve_sternpost",
                new PrefabDB(
                    "shipwreck_karve_sternpost",
                    false,
                    false,
                    HammerCategories.CreatorShop,
                    nameof(CraftingStations.None),
                    "FineWood,10",
                    clipEverything: true
                )
            },
            {
                "ShootStump",
                new PrefabDB(
                    "ShootStump",
                    false,
                    false,
                    HammerCategories.Nature,
                    nameof(CraftingStations.None),
                    "YggdrasilWood,10",
                    clipEverything: true,
                    pieceGroup: PieceGroup.Flora
                )
            },
            {
                "shrub_2",
                new PrefabDB(
                    "shrub_2",
                    true,
                    false,
                    HammerCategories.Nature,
                    nameof(CraftingStations.None),
                    "",
                    pieceGroup: PieceGroup.Flora
                )
            },
            {
                "shrub_2_heath",
                new PrefabDB(
                    "shrub_2_heath",
                    true,
                    false,
                    HammerCategories.Nature,
                    nameof(CraftingStations.None),
                    "",
                    pieceGroup: PieceGroup.Flora
                )
            },
            {
                "sign_notext",
                new PrefabDB(
                    "sign_notext",
                    true,
                    false,
                    HammerCategories.Building,
                    nameof(CraftingStations.Workbench),
                    "Wood,1",
                    pieceName: "Wood plank",
                    pieceDesc: "",
                    pieceGroup: PieceGroup.Wood
                )
            },
            {
                "silvervein",
                new PrefabDB(
                    "silvervein",
                    false,
                    false,
                    HammerCategories.Nature,
                    nameof(CraftingStations.None),
                    "Stone,50",
                    clipEverything: true,
                    pieceGroup: PieceGroup.Ore
                )
            },

            {
                "Skull1",
                new PrefabDB(
                    "Skull1",
                    true,
                    false,
                    HammerCategories.CreatorShop,
                    nameof(CraftingStations.None),
                    "TrophySkeleton,1"
                )
            },
            {
                "Skull2",
                new PrefabDB(
                    "Skull2",
                    false,
                    false,
                    HammerCategories.CreatorShop,
                    nameof(CraftingStations.None),
                    ""
                )
            },
            {
                "StatueCorgi",
                new PrefabDB(
                    "StatueCorgi",
                    true,
                    false,
                    HammerCategories.Furniture,
                    nameof(CraftingStations.Stonecutter),
                    "Stone,24",
                    clipEverything: true,
                    pieceGroup: PieceGroup.Statue
                )
            },
            {
                "StatueDeer",
                new PrefabDB(
                    "StatueDeer",
                    true,
                    false,
                    HammerCategories.Furniture,
                    nameof(CraftingStations.Stonecutter),
                    "Stone,24",
                    clipEverything: true,
                    pieceGroup: PieceGroup.Statue
                )
            },
            {
                "StatueEvil",
                new PrefabDB(
                    "StatueEvil",
                    true,
                    false,
                    HammerCategories.Furniture,
                    nameof(CraftingStations.Stonecutter),
                    "Stone,32",
                    clipEverything: true,
                    pieceGroup: PieceGroup.Statue
                )
            },
            {
                "StatueHare",
                new PrefabDB(
                    "StatueHare",
                    true,
                    false,
                    HammerCategories.Furniture,
                    nameof(CraftingStations.Stonecutter),
                    "Stone,24",
                    clipEverything: true,
                    pieceGroup: PieceGroup.Statue
                )
            },
            {
                "StatueSeed",
                new PrefabDB(
                    "StatueSeed",
                    true,
                    false,
                    HammerCategories.Furniture,
                    nameof(CraftingStations.Stonecutter),
                    "Stone,16",
                    clipEverything: true,
                    pieceGroup: PieceGroup.Statue
                )
            },
            {
                "stone_floor",
                new PrefabDB(
                    "stone_floor",
                    true,
                    false,
                    HammerCategories.Building,
                    nameof(CraftingStations.Stonecutter),
                    "Stone,16",
                    pieceName: "Stone floor 4x4",
                    clipGround: true,
                    pieceGroup: PieceGroup.Stone
                )
            },
            {
                "stoneblock_fracture",
                new PrefabDB(
                    "stoneblock_fracture",
                    true,
                    false,
                    HammerCategories.Building,
                    nameof(CraftingStations.Stonecutter),
                    "Stone,16",
                    pieceName: "Stone floor 4x4 (2)",
                    clipGround: true,
                    pieceGroup: PieceGroup.Stone
                )
            },
            {
                "stubbe",
                new PrefabDB(
                    "stubbe",
                    false,
                    false,
                    HammerCategories.Nature,
                    nameof(CraftingStations.None),
                    "",
                    clipEverything: true,
                    pieceGroup: PieceGroup.Flora
                )
            },
            {
                "stubbe_spawner",
                new PrefabDB(
                    "stubbe_spawner",
                    false,
                    false,
                    HammerCategories.CreatorShop,
                    nameof(CraftingStations.None),
                    "",
                    clipEverything: true
                )
            },
            {
                "sunken_crypt_gate",
                new PrefabDB(
                    "sunken_crypt_gate",
                    false,
                    false,
                    HammerCategories.Building,
                    nameof(CraftingStations.Forge),
                    "Iron,4",
                    pieceGroup: PieceGroup.Iron
                )
            },
            {
                "SwampTree1",
                new PrefabDB(
                    "SwampTree1",
                    false,
                    false,
                    HammerCategories.Nature,
                    nameof(CraftingStations.None),
                    "ElderBark,10",
                    clipEverything: true,
                    pieceGroup: PieceGroup.Flora
                )
            },
            {
                "SwampTree1_log",
                new PrefabDB(
                    "SwampTree1_log",
                    false,
                    false,
                    HammerCategories.Nature,
                    nameof(CraftingStations.None),
                    "",
                    clipEverything: true,
                    pieceGroup: PieceGroup.Flora
                )
            },
            {
                "SwampTree1_Stub",
                new PrefabDB(
                    "SwampTree1_Stub",
                    false,
                    false,
                    HammerCategories.Nature,
                    nameof(CraftingStations.None),
                    "",
                    clipEverything: true,
                    pieceGroup: PieceGroup.Flora
                )
            },
            {
                "SwampTree2",
                new PrefabDB(
                    "SwampTree2",
                    false,
                    false,
                    HammerCategories.Nature,
                    nameof(CraftingStations.None),
                    "ElderBark,32",
                    clipEverything: true,
                    pieceGroup: PieceGroup.Flora
                )
            },
            {
                "SwampTree2_darkland",
                new PrefabDB(
                    "SwampTree2_darkland",
                    false,
                    false,
                    HammerCategories.Nature,
                    nameof(CraftingStations.None),
                    "ElderBark,32",
                    clipEverything: true,
                    pieceGroup: PieceGroup.Flora
                )
            },
            {
                "SwampTree2_log",
                new PrefabDB(
                    "SwampTree2_log",
                    false,
                    false,
                    HammerCategories.Nature,
                    nameof(CraftingStations.None),
                    "ElderBark,32",
                    clipEverything: true,
                    pieceGroup: PieceGroup.Flora
                )
            },
            {
                "tarlump1",
                new PrefabDB(
                    "tarlump1",
                    false,
                    false,
                    HammerCategories.CreatorShop,
                    nameof(CraftingStations.None),
                    "Tar,50",
                    pieceName: "Tar volume",
                    pieceDesc: "Warning: requires devcommands to remove"
                )
            },
            {
                "trader_wagon_destructable",
                new PrefabDB(
                    "trader_wagon_destructable",
                    true,
                    false,
                    HammerCategories.Furniture,
                    nameof(CraftingStations.BlackForge),
                    "FineWood,32",
                    placementPatch: true
                )
            },
            {
                "Trailership",
                new PrefabDB(
                    "Trailership",
                    true,
                    false,
                    HammerCategories.Misc,
                    nameof(CraftingStations.Workbench),
                    "IronNails,100;DeerHide,10;FineWood,40;ElderBark,40",
                    pieceName: "Trader ship",
                    pieceGroup: PieceGroup.Ship
                )
            },
            {
                "TreasureChest_dvergr_loose_stone",
                new PrefabDB(
                    "TreasureChest_dvergr_loose_stone",
                    true,
                    false,
                    HammerCategories.Furniture,
                    nameof(CraftingStations.Stonecutter),
                    "BlackMarble,10",
                    pieceName: "Black marble chest",
                    pieceGroup: PieceGroup.Chest
                )
            },
            {
                "TreasureChest_dvergrtower",
                new PrefabDB(
                    "TreasureChest_dvergrtower",
                    true,
                    false,
                    HammerCategories.Furniture,
                    nameof(CraftingStations.BlackForge),
                    "YggdrasilWood,10;Copper,1",
                    pieceName: "Dvergr chest",
                    pieceGroup: PieceGroup.Chest
                )
            },
            {
                "TreasureChest_dvergrtown",
                new PrefabDB(
                    "TreasureChest_dvergrtown",
                    true,
                    false,
                    HammerCategories.Furniture,
                    nameof(CraftingStations.BlackForge),
                    "YggdrasilWood,10;Copper,1",
                    pieceName: "Dvergr chest (large)",
                    pieceGroup: PieceGroup.Chest
                )
            },
            {
                "TreasureChest_fCrypt",
                new PrefabDB(
                    "TreasureChest_fCrypt",
                    true,
                    false,
                    HammerCategories.Furniture,
                    nameof(CraftingStations.Stonecutter),
                    "Stone,10;RoundLog,1",
                    pieceName: "Stone chest (mossy)",
                    pieceGroup: PieceGroup.Chest
                )
            },
            {
                "TreasureChest_mountaincave",
                new PrefabDB(
                    "TreasureChest_mountaincave",
                    false,
                    false,
                    HammerCategories.Furniture,
                    nameof(CraftingStations.Stonecutter),
                    "Stone,10;Crystal,1",
                    pieceName: "Stone chest (snow)",
                    pieceGroup: PieceGroup.Chest
                )
            },
            {
                "TreasureChest_sunkencrypt",
                new PrefabDB(
                    "TreasureChest_sunkencrypt",
                    true,
                    false,
                    HammerCategories.Furniture,
                    nameof(CraftingStations.Stonecutter),
                    "Stone,10;ElderBark,1",
                    pieceName: "Stone chest (dark moss)",
                    pieceGroup: PieceGroup.Chest
                )
            },
            {
                "TreasureChest_trollcave",
                new PrefabDB(
                    "TreasureChest_trollcave",
                    true,
                    false,
                    HammerCategories.Furniture,
                    nameof(CraftingStations.Stonecutter),
                    "Stone,10;RoundLog,1",
                    pieceName: "Stone chest (mossy, big)",
                    pieceGroup: PieceGroup.Chest
                )
            },
            {
                "tunnel_web",
                new PrefabDB(
                    "tunnel_web",
                    false,
                    false,
                    HammerCategories.CreatorShop,
                    nameof(CraftingStations.None),
                    ""
                )
            },
            {
                "turf_roof",
                new PrefabDB(
                    "turf_roof",
                    true,
                    false,
                    HammerCategories.Building,
                    nameof(CraftingStations.Workbench),
                    "Wood,2",
                    pieceGroup: PieceGroup.Wood
                )
            },
            {
                "turf_roof_top",
                new PrefabDB(
                    "turf_roof_top",
                    true,
                    false,
                    HammerCategories.Building,
                    nameof(CraftingStations.Workbench),
                    "Wood,2",
                    pieceGroup: PieceGroup.Wood
                )
            },
            {
                "vertical_web",
                new PrefabDB(
                    "vertical_web",
                    false,
                    false,
                    HammerCategories.CreatorShop,
                    nameof(CraftingStations.None),
                    ""
                )
            },
            {
                "vines",
                new PrefabDB(
                    "vines",
                    true,
                    false,
                    HammerCategories.Nature,
                    nameof(CraftingStations.None),
                    "Wood,2",
                    pieceGroup: PieceGroup.Flora
                )
            },
            {
                "widestone",
                new PrefabDB(
                    "widestone",
                    true,
                    false,
                    HammerCategories.Nature,
                    nameof(CraftingStations.None),
                    "Stone,32",
                    clipEverything: true,
                    pieceGroup: PieceGroup.Rock
                )
            },
            {
                "wood_ledge",
                new PrefabDB(
                    "wood_ledge",
                    true,
                    false,
                    HammerCategories.Building,
                    nameof(CraftingStations.Workbench),
                    "Wood,1",
                    pieceGroup: PieceGroup.Wood
                )
            },
            {
                "yggashoot_log",
                new PrefabDB(
                    "yggashoot_log",
                    false,
                    false,
                    HammerCategories.Nature,
                    nameof(CraftingStations.None),
                    "YggdrasilWood,10",
                    pieceGroup: PieceGroup.Flora
                )
            },
            {
                "yggashoot_log_half",
                new PrefabDB(
                    "yggashoot_log_half",
                    false,
                    false,
                    HammerCategories.Nature,
                    nameof(CraftingStations.None),
                    "YggdrasilWood,10",
                    pieceGroup: PieceGroup.Flora
                )
            },
            {
                "YggaShoot_small1",
                new PrefabDB(
                    "YggaShoot_small1",
                    true,
                    false,
                    HammerCategories.Nature,
                    nameof(CraftingStations.None),
                    "YggdrasilWood,10",
                    pieceGroup: PieceGroup.Flora
                )
            },
            {
                "YggaShoot1",
                new PrefabDB(
                    "YggaShoot1",
                    true,
                    false,
                    HammerCategories.Nature,
                    nameof(CraftingStations.None),
                    "YggdrasilWood,16",
                    clipEverything: true,
                    pieceGroup: PieceGroup.Flora
                )
            },
            {
                "YggaShoot2",
                new PrefabDB(
                    "YggaShoot2",
                    true,
                    false,
                    HammerCategories.Nature,
                    nameof(CraftingStations.None),
                    "YggdrasilWood,16",
                    clipEverything: true,
                    pieceGroup: PieceGroup.Flora
                )
            },
            {
                "YggaShoot3",
                new PrefabDB(
                    "YggaShoot3",
                    true,
                    false,
                    HammerCategories.Nature,
                    nameof(CraftingStations.None),
                    "YggdrasilWood,16",
                    clipEverything: true,
                    pieceGroup: PieceGroup.Flora
                )
            },
            {
                "YggdrasilRoot",
                new PrefabDB(
                    "YggdrasilRoot",
                    true,
                    false,
                    HammerCategories.Nature,
                    nameof(CraftingStations.None),
                    "YggdrasilWood,64;Sap,10",
                    clipEverything: true,
                    pieceGroup: PieceGroup.Flora
                )
            },
        };
    }
}