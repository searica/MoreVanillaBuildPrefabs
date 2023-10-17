using Jotunn.Configs;
using System.Collections.Generic;

namespace MoreVanillaBuildPrefabs.Configs
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
     * SunkenKit_stair_corner_left
     */

    internal class DefaultConfigs
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
            // fire pits
            {
                "fire_pit_haldor",
                new PrefabDB(
                    "fire_pit_haldor",
                    true,
                    false,
                    HammerCategories.Misc,
                    nameof(CraftingStations.None),
                    "Stone,5;Wood,2;BlackCore,1"
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
                    "Ironpit,1;Wood,1;BlackCore,1"
                )
            },

            // black marble pieces
            {
                "blackmarble_head_big01",
                new PrefabDB(
                    "blackmarble_head_big01",
                    true,
                    false,
                    HammerCategories.Building,
                    nameof(CraftingStations.Stonecutter),
                    "BlackMarble,6"
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
                    "BlackMarble,6"
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
                    "BlackMarble,2"
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
                    "BlackMarble,2"
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
                    "BlackMarble,6"
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
                    "BlackMarble,8"
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
                    "BlackMarble,2"
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
                    "BlackMarble,2"
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
                    "BlackMarble,2"
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
                    "BlackMarble,8"
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
                    "BlackMarble,8"
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
                    "BlackMarble,2"
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
                    "BlackMarble,1"
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
                    "BlackMarble,2"
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
                    "BlackMarble,4"
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
                    "BlackMarble,6"
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
                    "BlackMarble,16"
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
                    "BlackMarble,32"
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
                    "BlackMarble,2;Copper,1"
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
                    "BlackMarble,8;Copper,2"
                )
            },

            // dvergr pieces
            {
                "piece_dvergr_pole",
                new PrefabDB(
                    "piece_dvergr_pole",
                    true,
                    false,
                    HammerCategories.Building,
                    nameof(CraftingStations.BlackForge),
                    "YggdrasilWood,1"
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
                    "YggdrasilWood,5"
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
                "dvergrprops_wood_beam",
                new PrefabDB(
                    "dvergrprops_wood_beam",
                    true,
                    false,
                    HammerCategories.Building,
                    nameof(CraftingStations.BlackForge),
                    "YggdrasilWood,4"
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
                    "YggdrasilWood,2"
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
                    "YggdrasilWood,2"
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
                    "YggdrasilWood,2"
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
                    "YggdrasilWood,10"
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
                    "YggdrasilWood,5;CopperScrap,2"
                )
            },

            // dvergr furniture
            {
                "dverger_demister",
                new PrefabDB(
                    "dverger_demister",
                    true,
                    false,
                    HammerCategories.Furniture,
                    nameof(CraftingStations.BlackForge),
                    "Iron,1;Wisp,1"
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
                    "Iron,1;Wisp,2"
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
                    "JuteBlue,6;FineWood,2"
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
                    "YggdrasilWood,20;Bronze,5;Resin,10"
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
                    "YggdrasilWood,8;ScaleHide,2;IronNails,5"
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
                    "YggdrasilWood,4"
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
                    "YggdrasilWood,4"
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
                    "YggdrasilWood,4"
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
                    "JuteBlue,6;FineWood,2"
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
                    "Chain,2;Iron,2"
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
                    "YggdrasilWood,4;IronNails,5"
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
                    "YggdrasilWood,2"
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
                    "YggdrasilWood,6;IronNails,10"
                )
            },
            {
                "Pickable_BlackCoreStand",
                new PrefabDB(
                    "Pickable_BlackCoreStand",
                    true,
                    false,
                    HammerCategories.Furniture,
                    nameof(CraftingStations.BlackForge),
                    "Iron,2;BlackCore,1"
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
                    "FineWood,32"
                )
            },

            // goblin pieces
            {
                "goblin_banner",
                new PrefabDB(
                    "goblin_banner",
                    true,
                    false,
                    HammerCategories.Furniture,
                    nameof(CraftingStations.Workbench),
                    "Wood,1;DeerHide,1;Tar,1"
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
                    "Wood,8;Tar,1"
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
                    "Wood,2;Tar,1"
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
                    "Wood,1;Tar,1"
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
                    "Wood,1;Tar,1"
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
                    "Wood,1;DeerHide,1;Tar,1"
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
                    "Wood,1;DeerHide,1;Tar,1"
                )
            },
            {
                "goblin_roof_cap",
                new PrefabDB(
                    "goblin_roof_cap",
                    true,
                    false,
                    HammerCategories.Building,
                    nameof(CraftingStations.Workbench),
                    "Wood,4;DeerHide,4;Tar,1"
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
                    "Wood,2;Tar,1"
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
                    "Wood,2;Tar,1"
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
                    "Wood,1;Tar,1"
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
                    "Wood,2;Tar,1"
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
                    "Wood,2;GoblinTotem,1"
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
                    "BoneFragments,6;Tar,1"
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
                    "Stone,1;Tar,1"
                )
            },

            // Furniture
            {
                "ArmorStand_Male",
                new PrefabDB(
                    "ArmorStand_Male",
                    true,
                    false,
                    HammerCategories.Furniture,
                    nameof(CraftingStations.Workbench),
                    "FineWood,8;BronzeNails,2;Tar,4"
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
                    "FineWood,8;BronzeNails,2;Tar,4"
                )
            },

            // misc pieces
            {
                "barrell",
                new PrefabDB(
                    "barrell",
                    true,
                    false,
                    HammerCategories.Misc,
                    nameof(CraftingStations.Workbench),
                    "Wood,2"
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
                    "Wood,2"
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
                    "Wood,2"
                )
            },
            {
                "turf_roof_wall", // it's just a wood 26 degree wall
                new PrefabDB(
                    "turf_roof_wall",
                    false,
                    false,
                    HammerCategories.Building,
                    nameof(CraftingStations.Workbench),
                    "Wood,2"
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
                    "Stone,16"
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
                    "Wood,2"
                )
            },
            {
                "CargoCrate",
                new PrefabDB(
                    "CargoCrate",
                    false,
                    false,
                    HammerCategories.Misc,
                    nameof(CraftingStations.Workbench),
                    "Wood,2"
                )
            },
            {
                "Pickable_SurtlingCoreStand",
                new PrefabDB(
                    "Pickable_SurtlingCoreStand",
                    true,
                    false,
                    HammerCategories.Furniture,
                    nameof(CraftingStations.Forge),
                    "Iron,2;SurtlingCore,1"
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
                    "JuteRed,4"
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
                    "Wood,2"
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
                    "Wood,1"
                )
            },

            // statues
            {
                "StatueCorgi",
                new PrefabDB(
                    "StatueCorgi",
                    true,
                    false,
                    HammerCategories.Furniture,
                    nameof(CraftingStations.Stonecutter),
                    "Stone,24"
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
                    "Stone,24"
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
                    "Stone,32"
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
                    "Stone,24"
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
                    "Stone,16"
                )
            },

            // CreatorShop
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

            // CreatorShop Rocks
            {
                "highstone",
                new PrefabDB(
                    "highstone",
                    true,
                    false,
                    HammerCategories.CreatorShop,
                    nameof(CraftingStations.None),
                    "Stone,32"
                )
            },
            {
                "widestone",
                new PrefabDB(
                    "widestone",
                    true,
                    false,
                    HammerCategories.CreatorShop,
                    nameof(CraftingStations.None),
                    "Stone,32"
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
                "Rock_3",
                new PrefabDB(
                    "Rock_3",
                    true,
                    false,
                    HammerCategories.CreatorShop,
                    nameof(CraftingStations.None),
                    "Stone,24"
                )
            },
            {
                "Rock_4",
                new PrefabDB(
                    "Rock_4",
                    true,
                    false,
                    HammerCategories.CreatorShop,
                    nameof(CraftingStations.None),
                    "Stone,16"
                )
            },
            {
                "Rock_4_plains",
                new PrefabDB(
                    "Rock_4_plains",
                    true,
                    false,
                    HammerCategories.CreatorShop,
                    nameof(CraftingStations.None),
                    "Stone,24"
                )
            },
            {
                "Rock_7",
                new PrefabDB(
                    "Rock_7",
                    true,
                    false,
                    HammerCategories.CreatorShop,
                    nameof(CraftingStations.None),
                    "Stone,32"
                )
            },
            {
                "Rock_destructible",
                new PrefabDB(
                    "Rock_destructible",
                    true,
                    false,
                    HammerCategories.CreatorShop,
                    nameof(CraftingStations.None),
                    "Stone,24"
                )
            },
            {
                "Rock_destructible_test",
                new PrefabDB(
                    "Rock_destructible_test",
                    true,
                    false,
                    HammerCategories.CreatorShop,
                    nameof(CraftingStations.None),
                    "Stone,32"
                )
            },
            {
                "rock_mistlands1",
                new PrefabDB(
                    "rock_mistlands1",
                    true,
                    false,
                    HammerCategories.CreatorShop,
                    nameof(CraftingStations.None),
                    "Stone,48"
                )
            },
            {
                "rock_mistlands2",
                new PrefabDB(
                    "rock_mistlands2",
                    true,
                    false,
                    HammerCategories.CreatorShop,
                    nameof(CraftingStations.None),
                    "Stone,32"
                )
            },
            {
                "RockDolmen_1",
                new PrefabDB(
                    "RockDolmen_1",
                    true,
                    false,
                    HammerCategories.CreatorShop,
                    nameof(CraftingStations.None),
                    "Stone,75"
                )
            },
            {
                "RockDolmen_2",
                new PrefabDB(
                    "RockDolmen_2",
                    true,
                    false,
                    HammerCategories.CreatorShop,
                    nameof(CraftingStations.None),
                    "Stone,50"
                )
            },
            {
                "RockDolmen_3",
                new PrefabDB(
                    "RockDolmen_3",
                    true,
                    false,
                    HammerCategories.CreatorShop,
                    nameof(CraftingStations.None),
                    "Stone,75"
                )
            },

            // CreatorShop Plants
            {
                "Birch1_aut",
                new PrefabDB(
                    "Birch1_aut",
                    true,
                    false,
                    HammerCategories.CreatorShop,
                    nameof(CraftingStations.None),
                    "BirchSeeds,1;FineWood,2"
                )
            },
            {
                "Birch2_aut",
                new PrefabDB(
                    "Birch2_aut",
                    true,
                    false,
                    HammerCategories.CreatorShop,
                    nameof(CraftingStations.None),
                    "BirchSeeds,1;FineWood,2"
                )
            },
            {
                "YggdrasilRoot",
                new PrefabDB(
                    "YggdrasilRoot",
                    true,
                    false,
                    HammerCategories.CreatorShop,
                    nameof(CraftingStations.None),
                    "YggdrasilWood,64;Sap,10"
                )
            },

            // Disabled
            {
                "ancient_skull",
                new PrefabDB(
                    "ancient_skull",
                    false,
                    false,
                    HammerCategories.CreatorShop,
                    nameof(CraftingStations.None),
                    "TrophySkeleton,32"
                )
            },
            {
                "Beech_small2",
                new PrefabDB(
                    "Beech_small2",
                    false,
                    false,
                    HammerCategories.CreatorShop,
                    nameof(CraftingStations.None),
                    "Wood,1"
                )
            },
            {
                "Beech1",
                new PrefabDB(
                    "Beech1",
                    false,
                    false,
                    HammerCategories.CreatorShop,
                    nameof(CraftingStations.None),
                    "Wood,16"
                )
            },
            {
                "Birch1",
                new PrefabDB(
                    "Birch1",
                    false,
                    false,
                    HammerCategories.CreatorShop,
                    nameof(CraftingStations.None),
                    "FineWood,16"
                )
            },
            {
                "Birch2",
                new PrefabDB(
                    "Birch2",
                    false,
                    false,
                    HammerCategories.CreatorShop,
                    nameof(CraftingStations.None),
                    "FineWood,16"
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
                    "BlackMarble,4"
                )
            },
            {
                "blackmarble_creep_4x1x1",
                new PrefabDB(
                    "blackmarble_creep_4x1x1",
                    false,
                    false,
                    HammerCategories.Building,
                    nameof(CraftingStations.None),
                    "BlackMarble,4"
                )
            },
            {
                "blackmarble_creep_4x2x1",
                new PrefabDB(
                    "blackmarble_creep_4x2x1",
                    false,
                    false,
                    HammerCategories.Building,
                    nameof(CraftingStations.None),
                    "BlackMarble,8"
                )
            },
            {
                "blackmarble_creep_slope_inverted_1x1x2",
                new PrefabDB(
                    "blackmarble_creep_slope_inverted_1x1x2",
                    false,
                    false,
                    HammerCategories.Building,
                    nameof(CraftingStations.None),
                    "BlackMarble,2"
                )
            },
            {
                "blackmarble_creep_slope_inverted_2x2x1",
                new PrefabDB(
                    "blackmarble_creep_slope_inverted_2x2x1",
                    false,
                    false,
                    HammerCategories.Building,
                    nameof(CraftingStations.None),
                    "BlackMarble,4"
                )
            },
            {
                "blackmarble_creep_stair",
                new PrefabDB(
                    "blackmarble_creep_stair",
                    false,
                    false,
                    HammerCategories.Building,
                    nameof(CraftingStations.None),
                    "BlackMarble,8"
                )
            },
            {
                "caverock_ice_stalagmite",
                new PrefabDB(
                    "caverock_ice_stalagmite",
                    false,
                    false,
                    HammerCategories.Building,
                    nameof(CraftingStations.None),
                    "Crystal,2"
                )
            },
            {
                "caverock_ice_stalagmite_broken",
                new PrefabDB(
                    "caverock_ice_stalagmite_broken",
                    false,
                    false,
                    HammerCategories.Building,
                    nameof(CraftingStations.None),
                    "Crystal,4"
                )
            },
            {
                "caverock_ice_stalagtite",
                new PrefabDB(
                    "caverock_ice_stalagtite",
                    false,
                    false,
                    HammerCategories.Building,
                    nameof(CraftingStations.None),
                    "Crystal,2"
                )
            },
            {
                "cliff_mistlands1",
                new PrefabDB(
                    "cliff_mistlands1",
                    false,
                    false,
                    HammerCategories.CreatorShop,
                    nameof(CraftingStations.None),
                    "Stone,350"
                )
            },
            {
                "cliff_mistlands1_creep",
                new PrefabDB(
                    "cliff_mistlands1_creep",
                    false,
                    false,
                    HammerCategories.CreatorShop,
                    nameof(CraftingStations.None),
                    "Stone,350"
                )
            },
            {
                "cliff_mistlands2",
                new PrefabDB(
                    "cliff_mistlands2",
                    false,
                    false,
                    HammerCategories.CreatorShop,
                    nameof(CraftingStations.None),
                    "Stone,175"
                )
            },
            {
                "CreepProp_egg_hanging01",
                new PrefabDB(
                    "CreepProp_egg_hanging01",
                    false,
                    false,
                    HammerCategories.Building,
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
                    HammerCategories.Building,
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
                    HammerCategories.Building,
                    nameof(CraftingStations.None),
                    "YggdrasilWood,2"
                )
            },
            {
                "CreepProp_entrance2",
                new PrefabDB(
                    "CreepProp_entrance2",
                    false,
                    false,
                    HammerCategories.Building,
                    nameof(CraftingStations.None),
                    "YggdrasilWood,2"
                )
            },
            {
                "CreepProp_hanging01",
                new PrefabDB(
                    "CreepProp_hanging01",
                    false,
                    false,
                    HammerCategories.Building,
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
                    HammerCategories.Building,
                    nameof(CraftingStations.None),
                    "YggdrasilWood,2"
                )
            },
            {
                "dungeon_queen_door",
                new PrefabDB(
                    "dungeon_queen_door",
                    false,
                    false,
                    HammerCategories.Building,
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
                    "Iron,4"
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
                    "YggdrasilWood,4"
                )
            },
            {
                "dvergrtown_creep_door",
                new PrefabDB(
                    "dvergrtown_creep_door",
                    false,
                    false,
                    HammerCategories.Building,
                    nameof(CraftingStations.None),
                    "YggdrasilWood,4"
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
                    "BlackMarble,12"
                )
            },
            {
                "dvergrtown_slidingdoor",
                new PrefabDB(
                    "dvergrtown_slidingdoor",
                    false,
                    false,
                    HammerCategories.Building,
                    nameof(CraftingStations.None),
                    "BlackMarble,6"
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
                    "YggdrasilWood,4"
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
                    "YggdrasilWood,8;Chain,2;Iron,2"
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
                    "YggdrasilWood,1"
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
                    "YggdrasilWood,4"
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
                    "YggdrasilWood,10"
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
                    "YggdrasilWood,20"
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
                    "YggdrasilWood,12"
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
                    "YggdrasilWood,10;IronNails,6"
                )
            },
            {
                "FirTree",
                new PrefabDB(
                    "FirTree",
                    false,
                    false,
                    HammerCategories.CreatorShop,
                    nameof(CraftingStations.None),
                    "Wood,16"
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
                    "BlackMarble,32"
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
                    "Iron,32"
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
                    "Iron,8"
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
                    "BlackMarble,64"
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
                    "BlackMarble,32"
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
                    "Iron,16"
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
                    "Iron,16"
                )
            },
            {
                "Greydwarf_Root",
                new PrefabDB(
                    "Greydwarf_Root",
                    false,
                    false,
                    HammerCategories.CreatorShop,
                    nameof(CraftingStations.None),
                    "Wood,2"
                )
            },
            {
                "HugeRoot1",
                new PrefabDB(
                    "HugeRoot1",
                    false,
                    false,
                    HammerCategories.CreatorShop,
                    nameof(CraftingStations.None),
                    "ElderBark,32"
                )
            },
            {
                "MountainKit_wood_gate",
                new PrefabDB(
                    "MountainKit_wood_gate",
                    false,
                    false,
                    HammerCategories.Building,
                    nameof(CraftingStations.None),
                    "Wood,20;Bronze,4"
                )
            },
            {
                "Oak1",
                new PrefabDB(
                    "Oak1",
                    false,
                    false,
                    HammerCategories.CreatorShop,
                    nameof(CraftingStations.None),
                    "Wood,20;FineWood,20"
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
                    "YggdrasilWood,16;IronNails,24"
                )
            },
            {
                "Pinetree_01",
                new PrefabDB(
                    "Pinetree_01",
                    false,
                    false,
                    HammerCategories.CreatorShop,
                    nameof(CraftingStations.None),
                    "Wood,16;RoundLog,16"
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
                    "Stone,350"
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
                    "Stone,175"
                )
            },
            {
                "rockformation1",
                new PrefabDB(
                    "rockformation1",
                    false,
                    false,
                    HammerCategories.CreatorShop,
                    nameof(CraftingStations.None),
                    "Stone,350"
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
                    "Stone,200"
                )
            },
            {
                "root07",
                new PrefabDB(
                    "root07",
                    false,
                    false,
                    HammerCategories.CreatorShop,
                    nameof(CraftingStations.None),
                    "ElderBark,2"
                )
            },
            {
                "root08",
                new PrefabDB(
                    "root08",
                    false,
                    false,
                    HammerCategories.CreatorShop,
                    nameof(CraftingStations.None),
                    "ElderBark,4"
                )
            },
            {
                "root11",
                new PrefabDB(
                    "root11",
                    false,
                    false,
                    HammerCategories.CreatorShop,
                    nameof(CraftingStations.None),
                    "ElderBark,4"
                )
            },
            {
                "root12",
                new PrefabDB(
                    "root12",
                    false,
                    false,
                    HammerCategories.CreatorShop,
                    nameof(CraftingStations.None),
                    "ElderBark,4"
                )
            },
            {
                "shipwreck_karve_bottomboards",
                new PrefabDB(
                    "shipwreck_karve_bottomboards",
                    false,
                    false,
                    HammerCategories.Furniture,
                    nameof(CraftingStations.None),
                    "FineWood,8"
                )
            },
            {
                "shipwreck_karve_bow",
                new PrefabDB(
                    "shipwreck_karve_bow",
                    false,
                    false,
                    HammerCategories.Furniture,
                    nameof(CraftingStations.None),
                    "FineWood,16"
                )
            },
            {
                "shipwreck_karve_stern",
                new PrefabDB(
                    "shipwreck_karve_stern",
                    false,
                    false,
                    HammerCategories.Furniture,
                    nameof(CraftingStations.None),
                    "FineWood,16"
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
                    "Stone,16"
                )
            },
            {
                "SwampTree1",
                new PrefabDB(
                    "SwampTree1",
                    false,
                    false,
                    HammerCategories.CreatorShop,
                    nameof(CraftingStations.None),
                    "ElderBark,16"
                )
            },
            {
                "SwampTree2",
                new PrefabDB(
                    "SwampTree2",
                    false,
                    false,
                    HammerCategories.CreatorShop,
                    nameof(CraftingStations.None),
                    "ElderBark,32"
                )
            },
            {
                "SwampTree2_darkland",
                new PrefabDB(
                    "SwampTree2_darkland",
                    false,
                    false,
                    HammerCategories.CreatorShop,
                    nameof(CraftingStations.None),
                    "ElderBark,32"
                )
            },
            {
                "SwampTree2_log",
                new PrefabDB(
                    "SwampTree2_log",
                    false,
                    false,
                    HammerCategories.CreatorShop,
                    nameof(CraftingStations.None),
                    "ElderBark,32"
                )
            },
            {
                "TreasureChest_mountaincave",
                new PrefabDB(
                    "TreasureChest_mountaincave",
                    false,
                    false,
                    HammerCategories.CreatorShop,
                    nameof(CraftingStations.None),
                    "Stone,10;Crystal,1"
                )
            },
            {
                "YggaShoot1",
                new PrefabDB(
                    "YggaShoot1",
                    false,
                    false,
                    HammerCategories.CreatorShop,
                    nameof(CraftingStations.None),
                    "YggdrasilWood,16"
                )
            },
            {
                "YggaShoot2",
                new PrefabDB(
                    "YggaShoot2",
                    false,
                    false,
                    HammerCategories.CreatorShop,
                    nameof(CraftingStations.None),
                    "YggdrasilWood,16"
                )
            },
            {
                "YggaShoot3",
                new PrefabDB(
                    "YggaShoot3",
                    false,
                    false,
                    HammerCategories.CreatorShop,
                    nameof(CraftingStations.None),
                    "YggdrasilWood,16"
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
                    "Wood,2"
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
                    "IronNails,100;DeerHide,10;FineWood,40;ElderBark,40"
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
                    "Bronze,5;Coal,2;BlackCore,1;WolfClaw,3"
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
                    "Bronze,5;GreydwarfEye,2;BlackCore,1;WolfClaw,3"
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
                    "Iron,2;Resin,2;BlackCore,1"
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
                    "Iron,2;GreydwarfEye,2;BlackCore,1"
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
                    "Iron,2;Guck,2;BlackCore,1"
                )
            },
        };
    }
}