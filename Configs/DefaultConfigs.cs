using System.Collections.Generic;
using MoreVanillaBuildPrefabs.Helpers;
using static MoreVanillaBuildPrefabs.Configs.PluginConfig;
using static MoreVanillaBuildPrefabs.Helpers.PieceHelper;

namespace MoreVanillaBuildPrefabs.Configs
{
    internal class DefaultConfigs
    {
        internal static Dictionary<string, Piece.Requirement[]> DefaultResources = new();

        internal static readonly HashSet<string> IgnoredPrefabs = new() {
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
            "Flies",
            "turf_roof_wall",
            //Placement is glitchy
            "demister_ball",
            "CargoCrate"
        };

        internal static readonly HashSet<string> RemovePickable = new()
        {
            "Pickable_SurtlingCoreStand",
            "Pickable_BlackCoreStand",
            // "Pickable_Tar"
        };

        internal static PrefabConfig GetDefaultPrefabConfigValues(string prefab_name)
        {
            if (DefaultConfigValues.ContainsKey(prefab_name))
            {
                return DefaultConfigValues[prefab_name];
            }
            return new PrefabConfig();
        }

        internal static readonly HashSet<string> NeedsCollisionPatchForGhost = new()
        {
            "blackmarble_stair_corner",
            "blackmarble_stair_corner_left",
            //"stoneblock_fracture"
        };

        // EligiblePrefabs that should not be set to allow clipping everything
        internal static readonly HashSet<string> RestrictClipping = new()
        {
            "blackmarble_post01",
            "dverger_demister",
            "dverger_demister_large",
            "dvergrprops_hooknchain",
            "barrell",
            "MountainKit_brazier_blue",
            "MountainKit_brazier",
            //"stoneblock_fracture",
            "piece_dvergr_pole",
            "dvergrprops_wood_pole",
            "dvergrprops_wood_wall"
        };


        internal static readonly Dictionary<string, PrefabConfig> DefaultConfigValues = new()
        {
            // black marble pieces
            {
                "blackmarble_head_big01",
                new PrefabConfig(
                    true,
                    false,
                    HammerCategories.Names.Building,
                    nameof(CraftingStations.Stonecutter),
                    "BlackMarble,6"
                )
            },
            {
                "blackmarble_head_big02",
                new PrefabConfig(
                    true,
                    false,
                    HammerCategories.Names.Building,
                    nameof(CraftingStations.Stonecutter),
                    "BlackMarble,6"
                )
            },
            {
                "blackmarble_head01",
                new PrefabConfig(
                    true,
                    false,
                    HammerCategories.Names.Building,
                    nameof(CraftingStations.Stonecutter),
                    "BlackMarble,2"
                )
            },
            {
                "blackmarble_head02",
                new PrefabConfig(
                    true,
                    false,
                    HammerCategories.Names.Building,
                    nameof(CraftingStations.Stonecutter),
                    "BlackMarble,2"
                )
            },
            {
                "blackmarble_out_2",
                new PrefabConfig(
                    true,
                    false,
                    HammerCategories.Names.Building,
                    nameof(CraftingStations.Stonecutter),
                    "BlackMarble,6"
                )
            },
            {
                "blackmarble_post01",
                new PrefabConfig(
                    true,
                    false,
                    HammerCategories.Names.Building,
                    nameof(CraftingStations.Stonecutter),
                    "BlackMarble,8"
                )
            },
            {
                "blackmarble_slope_1x2",
                new PrefabConfig(
                    true,
                    false,
                    HammerCategories.Names.Building,
                    nameof(CraftingStations.Stonecutter),
                    "BlackMarble,2"
                )
            },
            {
                "blackmarble_tile_floor_1x1",
                new PrefabConfig(
                    true,
                    false,
                    HammerCategories.Names.Building,
                    nameof(CraftingStations.Stonecutter),
                    "BlackMarble,2"
                )
            },
            {
                "blackmarble_slope_inverted_1x2",
                new PrefabConfig(
                    true,
                    false,
                    HammerCategories.Names.Building,
                    nameof(CraftingStations.Stonecutter),
                    "BlackMarble,2"
                )
            },
            {
                "blackmarble_stair_corner_left",
                new PrefabConfig(
                    true,
                    false,
                    HammerCategories.Names.Building,
                    nameof(CraftingStations.Stonecutter),
                    "BlackMarble,8"
                )
            },
            {
                "blackmarble_stair_corner",
                new PrefabConfig(
                    true,
                    false,
                    HammerCategories.Names.Building,
                    nameof(CraftingStations.Stonecutter),
                    "BlackMarble,8"
                )
            },
            {
                "blackmarble_tile_floor_2x2",
                new PrefabConfig(
                    true,
                    false,
                    HammerCategories.Names.Building,
                    nameof(CraftingStations.Stonecutter),
                    "BlackMarble,2"
                )
            },
            {
                "blackmarble_tile_wall_1x1",
                new PrefabConfig(
                    true,
                    false,
                    HammerCategories.Names.Building,
                    nameof(CraftingStations.Stonecutter),
                    "BlackMarble,1"
                )
            },
            {
                "blackmarble_tile_wall_2x2",
                new PrefabConfig(
                    true,
                    false,
                    HammerCategories.Names.Building,
                    nameof(CraftingStations.Stonecutter),
                    "BlackMarble,2"
                )
            },
            {
                "blackmarble_tile_wall_2x4",
                new PrefabConfig(
                    true,
                    false,
                    HammerCategories.Names.Building,
                    nameof(CraftingStations.Stonecutter),
                    "BlackMarble,4"
                )
            },
            {
                "blackmarble_base_2",
                new PrefabConfig(
                    true,
                    false,
                    HammerCategories.Names.Building,
                    nameof(CraftingStations.Stonecutter),
                    "BlackMarble,6"
                )
            },
            {
                "blackmarble_column_3",
                new PrefabConfig(
                    true,
                    false,
                    HammerCategories.Names.Building,
                    nameof(CraftingStations.Stonecutter),
                    "BlackMarble,16"
                )
            },
            {
                "blackmarble_floor_large",
                new PrefabConfig(
                    true,
                    false,
                    HammerCategories.Names.Building,
                    nameof(CraftingStations.Stonecutter),
                    "BlackMarble,32"
                )
            },
            {
                "metalbar_1x2",
                new PrefabConfig(
                    true,
                    false,
                    HammerCategories.Names.Building,
                    nameof(CraftingStations.BlackForge),
                    "BlackMarble,2;Copper,1"
                )
            },
            {
                "blackmarble_2x2_enforced",
                new PrefabConfig(
                    true,
                    false,
                    HammerCategories.Names.Building,
                    nameof(CraftingStations.BlackForge),
                    "BlackMarble,8;Copper,2"
                )
            },

            // dvergr pieces
            {
                "piece_dvergr_pole",
                new PrefabConfig(
                    true,
                    false,
                    HammerCategories.Names.Building,
                    nameof(CraftingStations.BlackForge),
                    "YggdrasilWood,1"
                )
            },
            {
                "piece_dvergr_wood_wall",
                new PrefabConfig(
                    true,
                    false,
                    HammerCategories.Names.Building,
                    nameof(CraftingStations.BlackForge),
                    "YggdrasilWood,5"
                )
            },
            {
                "dverger_guardstone",
                new PrefabConfig(
                    true,
                    false,
                    HammerCategories.Names.Misc,
                    nameof(CraftingStations.BlackForge),
                    "YggdrasilWood,5;BlackMarble,5;BlackCore,1"
                )
            },
            {
                "dvergrprops_wood_beam",
                new PrefabConfig(
                    true,
                    false,
                    HammerCategories.Names.Building,
                    nameof(CraftingStations.BlackForge),
                    "YggdrasilWood,4"
                )
            },
            {
                "dvergrprops_wood_floor",
                new PrefabConfig(
                    true,
                    false,
                    HammerCategories.Names.Building,
                    nameof(CraftingStations.BlackForge),
                    "YggdrasilWood,2"
                )
            },
            {
                "dvergrprops_wood_pole",
                new PrefabConfig(
                    true,
                    false,
                    HammerCategories.Names.Building,
                    nameof(CraftingStations.BlackForge),
                    "YggdrasilWood,2"
                )
            },
            {
                "dvergrprops_wood_stair",
                new PrefabConfig(
                    true,
                    false,
                    HammerCategories.Names.Building,
                    nameof(CraftingStations.BlackForge),
                    "YggdrasilWood,2"
                )
            },
            {
                "dvergrprops_wood_wall",
                new PrefabConfig(
                    true,
                    false,
                    HammerCategories.Names.Building,
                    nameof(CraftingStations.BlackForge),
                    "YggdrasilWood,10"
                )
            },
            {
                "dvergrtown_stair_corner_wood_left",
                new PrefabConfig(
                    true,
                    false,
                    HammerCategories.Names.Building,
                    nameof(CraftingStations.BlackForge),
                    "YggdrasilWood,5;CopperScrap,2"
                )
            },

            // dvergr furniture
            {
                "dverger_demister",
                new PrefabConfig(
                    true,
                    false,
                    HammerCategories.Names.Furniture,
                    nameof(CraftingStations.BlackForge),
                    "Iron,1;Wisp,1"
                )
            },
            {
                "dverger_demister_large",
                new PrefabConfig(
                    true,
                    false,
                    HammerCategories.Names.Furniture,
                    nameof(CraftingStations.BlackForge),
                    "Iron,1;Wisp,2"
                )
            },
            {
                "dvergrprops_banner",
                new PrefabConfig(
                    true,
                    false,
                    HammerCategories.Names.Furniture,
                    nameof(CraftingStations.Workbench),
                    "JuteBlue,6;FineWood,2"
                )
            },
            {
                "dvergrprops_barrel",
                new PrefabConfig(
                    true,
                    false,
                    HammerCategories.Names.Furniture,
                    nameof(CraftingStations.Workbench),
                    "YggdrasilWood,25;Bronze,5"
                )
            },
            {
                "dvergrprops_bed",
                new PrefabConfig(
                    true,
                    false,
                    HammerCategories.Names.Furniture,
                    nameof(CraftingStations.BlackForge),
                    "YggdrasilWood,8;ScaleHide,2;IronNails,5"
                )
            },
            {
                "dvergrprops_chair",
                new PrefabConfig(
                    true,
                    false,
                    HammerCategories.Names.Furniture,
                    nameof(CraftingStations.BlackForge),
                    "YggdrasilWood,4"
                )
            },
            {
                "dvergrprops_crate",
                new PrefabConfig(
                    true,
                    false,
                    HammerCategories.Names.Misc,
                    nameof(CraftingStations.BlackForge),
                    "YggdrasilWood,4"
                )
            },
            {
                "dvergrprops_crate_long",
                new PrefabConfig(
                    true,
                    false,
                    HammerCategories.Names.Misc,
                    nameof(CraftingStations.BlackForge),
                    "YggdrasilWood,4"
                )
            },
            {
                "dvergrprops_curtain",
                new PrefabConfig(
                    true,
                    false,
                    HammerCategories.Names.Furniture,
                    nameof(CraftingStations.Workbench),
                    "JuteBlue,6;FineWood,2"
                )
            },
            {
                "dvergrprops_hooknchain",
                new PrefabConfig(
                    true,
                    false,
                    HammerCategories.Names.Misc,
                    nameof(CraftingStations.BlackForge),
                    "Chain,2;Iron,2"
                )
            },
            {
                "dvergrprops_shelf",
                new PrefabConfig(
                    true,
                    false,
                    HammerCategories.Names.Furniture,
                    nameof(CraftingStations.BlackForge),
                    "YggdrasilWood,4;IronNails,5"
                )
            },
            {
                "dvergrprops_stool",
                new PrefabConfig(
                    true,
                    false,
                    HammerCategories.Names.Furniture,
                    nameof(CraftingStations.BlackForge),
                    "YggdrasilWood,2"
                )
            },
            {
                "dvergrprops_table",
                new PrefabConfig(
                    true,
                    false,
                    HammerCategories.Names.Furniture,
                    nameof(CraftingStations.BlackForge),
                    "YggdrasilWood,6;IronNails,10"
                )
            },
            {
                "Pickable_BlackCoreStand",
                new PrefabConfig(
                    true,
                    false,
                    HammerCategories.Names.Furniture,
                    nameof(CraftingStations.BlackForge),
                    "Iron,2;BlackCore,1"
                )
            },
            {
                "trader_wagon_destructable",
                new PrefabConfig(
                    true,
                    false,
                    HammerCategories.Names.Furniture,
                    nameof(CraftingStations.BlackForge),
                    "FineWood,32"
                )
            },


            // goblin pieces
            {
                "goblin_banner",
                new PrefabConfig(
                    true,
                    false,
                    HammerCategories.Names.Furniture,
                    nameof(CraftingStations.Workbench),
                    "Wood,1;DeerHide,1;Tar,1"
                )
            },
            {
                "goblin_bed",
                new PrefabConfig(
                    true,
                    false,
                    HammerCategories.Names.Furniture,
                    nameof(CraftingStations.Workbench),
                    "Wood,8;Tar,1"
                )
            },
            {
                "goblin_fence",
                new PrefabConfig(
                    true,
                    false,
                    HammerCategories.Names.Building,
                    nameof(CraftingStations.Workbench),
                    "Wood,2;Tar,1"
                )
            },
            {
                "goblin_pole",
                new PrefabConfig(
                    true,
                    false,
                    HammerCategories.Names.Building,
                    nameof(CraftingStations.Workbench),
                    "Wood,1;Tar,1"
                )
            },
            {
                "goblin_pole_small",
                new PrefabConfig(
                    true,
                    false,
                    HammerCategories.Names.Building,
                    nameof(CraftingStations.Workbench),
                    "Wood,1;Tar,1"
                )
            },
            {
                "goblin_roof_45d",
                new PrefabConfig(
                    true,
                    false,
                    HammerCategories.Names.Building,
                    nameof(CraftingStations.Workbench),
                    "Wood,1;DeerHide,1;Tar,1"
                )
            },
            {
                "goblin_roof_45d_corner",
                new PrefabConfig(
                    true,
                    false,
                    HammerCategories.Names.Building,
                    nameof(CraftingStations.Workbench),
                    "Wood,1;DeerHide,1;Tar,1"
                )
            },
            {
                "goblin_roof_cap",
                new PrefabConfig(
                    true,
                    false,
                    HammerCategories.Names.Building,
                    nameof(CraftingStations.Workbench),
                    "Wood,4;DeerHide,4;Tar,1"
                )
            },
            {
                "goblin_stairs",
                new PrefabConfig(
                    true,
                    false,
                    HammerCategories.Names.Building,
                    nameof(CraftingStations.Workbench),
                    "Wood,2;Tar,1"
                )
            },
            {
                "goblin_stepladder",
                new PrefabConfig(
                    true,
                    false,
                    HammerCategories.Names.Building,
                    nameof(CraftingStations.Workbench),
                    "Wood,2;Tar,1"
                )
            },
            {
                "goblin_woodwall_1m",
                new PrefabConfig(
                    true,
                    false,
                    HammerCategories.Names.Building,
                    nameof(CraftingStations.Workbench),
                    "Wood,1;Tar,1"
                )
            },
            {
                "goblin_woodwall_2m",
                new PrefabConfig(
                    true,
                    false,
                    HammerCategories.Names.Building,
                    nameof(CraftingStations.Workbench),
                    "Wood,2;Tar,1"
                )
            },
            {
                "goblin_totempole",
                new PrefabConfig(
                    false,
                    false,
                    HammerCategories.Names.CreatorShop,
                    nameof(CraftingStations.Workbench),
                    "Wood,2;GoblinTotem,1"
                )
            },
            {
                "goblin_woodwall_2m_ribs",
                new PrefabConfig(
                    false,
                    false,
                    HammerCategories.Names.CreatorShop,
                    nameof(CraftingStations.None),
                    "BoneFragments,6;Tar,1"
                )
            },
            {
                "goblinking_totemholder",
                new PrefabConfig(
                    false,
                    false,
                    HammerCategories.Names.CreatorShop,
                    nameof(CraftingStations.Stonecutter),
                    "Stone,1;Tar,1"
                )
            },

            // Furniture
            {
                "ArmorStand_Male",
                new PrefabConfig(
                    true,
                    false,
                    HammerCategories.Names.Furniture,
                    nameof(CraftingStations.Workbench),
                    "FineWood,8;BronzeNails,2;Tar,4"
                )
            },
            {
                "ArmorStand_Female",
                new PrefabConfig(
                    true,
                    false,
                    HammerCategories.Names.Furniture,
                    nameof(CraftingStations.Workbench),
                    "FineWood,8;BronzeNails,2;Tar,4"
                )
            },

            // misc pieces
            {
                "barrell",
                new PrefabConfig(
                    true,
                    false,
                    HammerCategories.Names.Misc,
                    nameof(CraftingStations.Workbench),
                    "Wood,2"
                )
            },
            {
                "turf_roof",
                new PrefabConfig(
                    true,
                    false,
                    HammerCategories.Names.Building,
                    nameof(CraftingStations.Workbench),
                    "Wood,2"
                )
            },
            {
                "turf_roof_top",
                new PrefabConfig(
                    true,
                    false,
                    HammerCategories.Names.Building,
                    nameof(CraftingStations.Workbench),
                    "Wood,2"
                )
            },
            {
                "turf_roof_wall", // it's just a wood 26 degree wall
                new PrefabConfig(
                    false,
                    false,
                    HammerCategories.Names.Building,
                    nameof(CraftingStations.Workbench),
                    "Wood,2"
                )
            },
            {
                "stone_floor",
                new PrefabConfig(
                    true,
                    false,
                    HammerCategories.Names.Building,
                    nameof(CraftingStations.Stonecutter),
                    "Stone,16"
                )
            },
            {
                "bucket",
                new PrefabConfig(
                    true,
                    false,
                    HammerCategories.Names.Misc,
                    nameof(CraftingStations.Workbench),
                    "Wood,2"
                )
            },
            {
                "CargoCrate",
                new PrefabConfig(
                    false,
                    false,
                    HammerCategories.Names.Misc,
                    nameof(CraftingStations.Workbench),
                    "Wood,2"
                )
            },
            {
                "Pickable_SurtlingCoreStand",
                new PrefabConfig(
                    true,
                    false,
                    HammerCategories.Names.Furniture,
                    nameof(CraftingStations.Forge),
                    "Iron,2;SurtlingCore,1"
                )
            },
            {
                "cloth_hanging_door_double",
                new PrefabConfig(
                    true,
                    false,
                    HammerCategories.Names.Furniture,
                    nameof(CraftingStations.Workbench),
                    "JuteRed,4"
                )
            },
            {
                "rug_straw",
                new PrefabConfig(
                    true,
                    false,
                    HammerCategories.Names.Furniture,
                    nameof(CraftingStations.None),
                    "Wood,2"
                )
            },
            {
                "wood_ledge",
                new PrefabConfig(
                    true,
                    false,
                    HammerCategories.Names.Building,
                    nameof(CraftingStations.Workbench),
                    "Wood,1"
                )
            },

            // statues
            {
                "StatueCorgi",
                new PrefabConfig(
                    true,
                    false,
                    HammerCategories.Names.Furniture,
                    nameof(CraftingStations.Stonecutter),
                    "Stone,24"
                )
            },
            {
                "StatueDeer",
                new PrefabConfig(
                    true,
                    false,
                    HammerCategories.Names.Furniture,
                    nameof(CraftingStations.Stonecutter),
                    "Stone,24"
                )
            },
            {
                "StatueEvil",
                new PrefabConfig(
                    true,
                    false,
                    HammerCategories.Names.Furniture,
                    nameof(CraftingStations.Stonecutter),
                    "Stone,32"
                )
            },
            {
                "StatueHare",
                new PrefabConfig(
                    true,
                    false,
                    HammerCategories.Names.Furniture,
                    nameof(CraftingStations.Stonecutter),
                    "Stone,24"
                )
            },
            {
                "StatueSeed",
                new PrefabConfig(
                    true,
                    false,
                    HammerCategories.Names.Furniture,
                    nameof(CraftingStations.Stonecutter),
                    "Stone,16"
                )
            },

            // CreatorShop
            {
                "Skull1",
                new PrefabConfig(
                    true,
                    false,
                    HammerCategories.Names.CreatorShop,
                    nameof(CraftingStations.None),
                    "TrophySkeleton,1"
                )
            },

            // CreatorShop Rocks
            {
                "highstone",
                new PrefabConfig(
                    true,
                    false,
                    HammerCategories.Names.CreatorShop,
                    nameof(CraftingStations.None),
                    "Stone,32"
                )
            },
            {
                "widestone",
                new PrefabConfig(
                    true,
                    false,
                    HammerCategories.Names.CreatorShop,
                    nameof(CraftingStations.None),
                    "Stone,32"
                )
            },
            {
                "marker01",
                new PrefabConfig(
                    true,
                    false,
                    HammerCategories.Names.CreatorShop,
                    nameof(CraftingStations.None),
                    "Stone,4"
                )
            },
            {
                "marker02",
                new PrefabConfig(
                    true,
                    false,
                    HammerCategories.Names.CreatorShop,
                    nameof(CraftingStations.None),
                    "Stone,4"
                )
            },
            {
                "Rock_3",
                new PrefabConfig(
                    true,
                    false,
                    HammerCategories.Names.CreatorShop,
                    nameof(CraftingStations.None),
                    "Stone,24"
                )
            },
            {
                "Rock_4",
                new PrefabConfig(
                    true,
                    false,
                    HammerCategories.Names.CreatorShop,
                    nameof(CraftingStations.None),
                    "Stone,16"
                )
            },
            {
                "Rock_4_plains",
                new PrefabConfig(
                    true,
                    false,
                    HammerCategories.Names.CreatorShop,
                    nameof(CraftingStations.None),
                    "Stone,24"
                )
            },
            {
                "Rock_7",
                new PrefabConfig(
                    true,
                    false,
                    HammerCategories.Names.CreatorShop,
                    nameof(CraftingStations.None),
                    "Stone,32"
                )
            },
            {
                "Rock_destructible",
                new PrefabConfig(
                    true,
                    false,
                    HammerCategories.Names.CreatorShop,
                    nameof(CraftingStations.None),
                    "Stone,24"
                )
            },
            {
                "Rock_destructible_test",
                new PrefabConfig(
                    true,
                    false,
                    HammerCategories.Names.CreatorShop,
                    nameof(CraftingStations.None),
                    "Stone,32"
                )
            },
            {
                "rock_mistlands1",
                new PrefabConfig(
                    true,
                    false,
                    HammerCategories.Names.CreatorShop,
                    nameof(CraftingStations.None),
                    "Stone,48"
                )
            },
            {
                "rock_mistlands2",
                new PrefabConfig(
                    true,
                    false,
                    HammerCategories.Names.CreatorShop,
                    nameof(CraftingStations.None),
                    "Stone,32"
                )
            },
            {
                "RockDolmen_1",
                new PrefabConfig(
                    true,
                    false,
                    HammerCategories.Names.CreatorShop,
                    nameof(CraftingStations.None),
                    "Stone,75"
                )
            },
            {
                "RockDolmen_2",
                new PrefabConfig(
                    true,
                    false,
                    HammerCategories.Names.CreatorShop,
                    nameof(CraftingStations.None),
                    "Stone,50"
                )
            },
            {
                "RockDolmen_3",
                new PrefabConfig(
                    true,
                    false,
                    HammerCategories.Names.CreatorShop,
                    nameof(CraftingStations.None),
                    "Stone,75"
                )
            },

            // CreatorShop Plants
            {
                "Birch1_aut",
                new PrefabConfig(
                    true,
                    false,
                    HammerCategories.Names.CreatorShop,
                    nameof(CraftingStations.None),
                    "BirchSeeds,1;FineWood,2"
                )
            },
            {
                "Birch2_aut",
                new PrefabConfig(
                    true,
                    false,
                    HammerCategories.Names.CreatorShop,
                    nameof(CraftingStations.None),
                    "BirchSeeds,1;FineWood,2"
                )
            },
            {
                "YggdrasilRoot",
                new PrefabConfig(
                    true,
                    false,
                    HammerCategories.Names.CreatorShop,
                    nameof(CraftingStations.None),
                    "YggdrasilWood,64;Sap,10"
                )
            },

            // Disabled
            {
                "ancient_skull",
                new PrefabConfig(
                    false,
                    false,
                    HammerCategories.Names.CreatorShop,
                    nameof(CraftingStations.None),
                    "TrophySkeleton,32"
                )
            },
            {
                "Beech_small2",
                new PrefabConfig(
                    false,
                    false,
                    HammerCategories.Names.CreatorShop,
                    nameof(CraftingStations.None),
                    "Wood,1"
                )
            },
            {
                "Beech1",
                new PrefabConfig(
                    false,
                    false,
                    HammerCategories.Names.CreatorShop,
                    nameof(CraftingStations.None),
                    "Wood,16"
                )
            },
            {
                "Birch1",
                new PrefabConfig(
                    false,
                    false,
                    HammerCategories.Names.CreatorShop,
                    nameof(CraftingStations.None),
                    "FineWood,16"
                )
            },
            {
                "Birch2",
                new PrefabConfig(
                    false,
                    false,
                    HammerCategories.Names.CreatorShop,
                    nameof(CraftingStations.None),
                    "FineWood,16"
                )
            },
            {
                "blackmarble_2x2x1",
                new PrefabConfig(
                    false,
                    false,
                    HammerCategories.Names.Building,
                    nameof(CraftingStations.Stonecutter),
                    "BlackMarble,4"
                )
            },
            {
                "blackmarble_creep_4x1x1",
                new PrefabConfig(
                    false,
                    false,
                    HammerCategories.Names.Building,
                    nameof(CraftingStations.None),
                    "BlackMarble,4"
                )
            },
            {
                "blackmarble_creep_4x2x1",
                new PrefabConfig(
                    false,
                    false,
                    HammerCategories.Names.Building,
                    nameof(CraftingStations.None),
                    "BlackMarble,8"
                )
            },
            {
                "blackmarble_creep_slope_inverted_1x1x2",
                new PrefabConfig(
                    false,
                    false,
                    HammerCategories.Names.Building,
                    nameof(CraftingStations.None),
                    "BlackMarble,2"
                )
            },
            {
                "blackmarble_creep_slope_inverted_2x2x1",
                new PrefabConfig(
                    false,
                    false,
                    HammerCategories.Names.Building,
                    nameof(CraftingStations.None),
                    "BlackMarble,4"
                )
            },
            {
                "blackmarble_creep_stair",
                new PrefabConfig(
                    false,
                    false,
                    HammerCategories.Names.Building,
                    nameof(CraftingStations.None),
                    "BlackMarble,8"
                )
            },
            {
                "caverock_ice_stalagmite",
                new PrefabConfig(
                    false,
                    false,
                    HammerCategories.Names.Building,
                    nameof(CraftingStations.None),
                    "Crystal,2"
                )
            },
            {
                "caverock_ice_stalagmite_broken",
                new PrefabConfig(
                    false,
                    false,
                    HammerCategories.Names.Building,
                    nameof(CraftingStations.None),
                    "Crystal,4"
                )
            },
            {
                "caverock_ice_stalagtite",
                new PrefabConfig(
                    false,
                    false,
                    HammerCategories.Names.Building,
                    nameof(CraftingStations.None),
                    "Crystal,2"
                )
            },
            {
                "cliff_mistlands1",
                new PrefabConfig(
                    false,
                    false,
                    HammerCategories.Names.CreatorShop,
                    nameof(CraftingStations.None),
                    "Stone,350"
                )
            },
            {
                "cliff_mistlands1_creep",
                new PrefabConfig(
                    false,
                    false,
                    HammerCategories.Names.CreatorShop,
                    nameof(CraftingStations.None),
                    "Stone,350"
                )
            },
            {
                "cliff_mistlands2",
                new PrefabConfig(
                    false,
                    false,
                    HammerCategories.Names.CreatorShop,
                    nameof(CraftingStations.None),
                    "Stone,175"
                )
            },
            {
                "CreepProp_egg_hanging01",
                new PrefabConfig(
                    false,
                    false,
                    HammerCategories.Names.Building,
                    nameof(CraftingStations.None),
                    "YggdrasilWood,2"
                )
            },
            {
                "CreepProp_egg_hanging02",
                new PrefabConfig(
                    false,
                    false,
                    HammerCategories.Names.Building,
                    nameof(CraftingStations.None),
                    "YggdrasilWood,2"
                )
            },
            {
                "CreepProp_entrance1",
                new PrefabConfig(
                    false,
                    false,
                    HammerCategories.Names.Building,
                    nameof(CraftingStations.None),
                    "YggdrasilWood,2"
                )
            },
            {
                "CreepProp_entrance2",
                new PrefabConfig(
                    false,
                    false,
                    HammerCategories.Names.Building,
                    nameof(CraftingStations.None),
                    "YggdrasilWood,2"
                )
            },
            {
                "CreepProp_hanging01",
                new PrefabConfig(
                    false,
                    false,
                    HammerCategories.Names.Building,
                    nameof(CraftingStations.None),
                    "YggdrasilWood,2"
                )
            },
            {
                "CreepProp_wall01",
                new PrefabConfig(
                    false,
                    false,
                    HammerCategories.Names.Building,
                    nameof(CraftingStations.None),
                    "YggdrasilWood,2"
                )
            },
            {
                "dungeon_queen_door",
                new PrefabConfig(
                    false,
                    false,
                    HammerCategories.Names.Building,
                    nameof(CraftingStations.None),
                    "BlackMarble,40;DvergrKeyFragment,4;Iron,12"
                )
            },
            {
                "dungeon_sunkencrypt_irongate",
                new PrefabConfig(
                    false,
                    false,
                    HammerCategories.Names.Building,
                    nameof(CraftingStations.None),
                    "Iron,4"
                )
            },
            {
                "dvergrprops_wood_stakewall",
                new PrefabConfig(
                    false,
                    false,
                    HammerCategories.Names.Building,
                    nameof(CraftingStations.None),
                    "YggdrasilWood,4"
                )
            },
            {
                "dvergrtown_creep_door",
                new PrefabConfig(
                    false,
                    false,
                    HammerCategories.Names.Building,
                    nameof(CraftingStations.None),
                    "YggdrasilWood,4"
                )
            },
            {
                "dvergrtown_secretdoor",
                new PrefabConfig(
                    false,
                    false,
                    HammerCategories.Names.CreatorShop,
                    nameof(CraftingStations.None),
                    "BlackMarble,12"
                )
            },
            {
                "dvergrtown_slidingdoor",
                new PrefabConfig(
                    false,
                    false,
                    HammerCategories.Names.Building,
                    nameof(CraftingStations.None),
                    "BlackMarble,6"
                )
            },
            {
                "dvergrtown_wood_beam",
                new PrefabConfig(
                    false,
                    false,
                    HammerCategories.Names.Building,
                    nameof(CraftingStations.BlackForge),
                    "YggdrasilWood,4"
                )
            },
            {
                "dvergrtown_wood_crane",
                new PrefabConfig(
                    false,
                    false,
                    HammerCategories.Names.Building,
                    nameof(CraftingStations.BlackForge),
                    "YggdrasilWood,8;Chain,2;Iron,2"
                )
            },
            {
                "dvergrtown_wood_stake",
                new PrefabConfig(
                    false,
                    false,
                    HammerCategories.Names.Building,
                    nameof(CraftingStations.BlackForge),
                    "YggdrasilWood,1"
                )
            },
            {
                "dvergrtown_wood_stakewall",
                new PrefabConfig(
                    false,
                    false,
                    HammerCategories.Names.Building,
                    nameof(CraftingStations.BlackForge),
                    "YggdrasilWood,4"
                )
            },
            {
                "dvergrtown_wood_support",
                new PrefabConfig(
                    false,
                    false,
                    HammerCategories.Names.Building,
                    nameof(CraftingStations.BlackForge),
                    "YggdrasilWood,10"
                )
            },
            {
                "dvergrtown_wood_wall01",
                new PrefabConfig(
                    false,
                    false,
                    HammerCategories.Names.Building,
                    nameof(CraftingStations.BlackForge),
                    "YggdrasilWood,20"
                )
            },
            {
                "dvergrtown_wood_wall02",
                new PrefabConfig(
                    false,
                    false,
                    HammerCategories.Names.Building,
                    nameof(CraftingStations.BlackForge),
                    "YggdrasilWood,12"
                )
            },
            {
                "dvergrtown_wood_wall03",
                new PrefabConfig(
                    false,
                    false,
                    HammerCategories.Names.Building,
                    nameof(CraftingStations.BlackForge),
                    "YggdrasilWood,10;IronNails,6"
                )
            },
            {
                "FirTree",
                new PrefabConfig(
                    false,
                    false,
                    HammerCategories.Names.CreatorShop,
                    nameof(CraftingStations.None),
                    "Wood,16"
                )
            },
            {
                "giant_arm",
                new PrefabConfig(
                    false,
                    false,
                    HammerCategories.Names.CreatorShop,
                    nameof(CraftingStations.None),
                    "BlackMarble,32"
                )
            },
            {
                "giant_helmet1",
                new PrefabConfig(
                    false,
                    false,
                    HammerCategories.Names.CreatorShop,
                    nameof(CraftingStations.None),
                    "Iron,32"
                )
            },
            {
                "giant_helmet2",
                new PrefabConfig(
                    false,
                    false,
                    HammerCategories.Names.CreatorShop,
                    nameof(CraftingStations.None),
                    "Iron,8"
                )
            },
            {
                "giant_ribs",
                new PrefabConfig(
                    false,
                    false,
                    HammerCategories.Names.CreatorShop,
                    nameof(CraftingStations.None),
                    "BlackMarble,64"
                )
            },
            {
                "giant_skull",
                new PrefabConfig(
                    false,
                    false,
                    HammerCategories.Names.CreatorShop,
                    nameof(CraftingStations.None),
                    "BlackMarble,32"
                )
            },
            {
                "giant_sword1",
                new PrefabConfig(
                    false,
                    false,
                    HammerCategories.Names.CreatorShop,
                    nameof(CraftingStations.None),
                    "Iron,16"
                )
            },
            {
                "giant_sword2",
                new PrefabConfig(
                    false,
                    false,
                    HammerCategories.Names.CreatorShop,
                    nameof(CraftingStations.None),
                    "Iron,16"
                )
            },
            {
                "Greydwarf_Root",
                new PrefabConfig(
                    false,
                    false,
                    HammerCategories.Names.CreatorShop,
                    nameof(CraftingStations.None),
                    "Wood,2"
                )
            },
            {
                "HugeRoot1",
                new PrefabConfig(
                    false,
                    false,
                    HammerCategories.Names.CreatorShop,
                    nameof(CraftingStations.None),
                    "ElderBark,32"
                )
            },
            {
                "MountainKit_wood_gate",
                new PrefabConfig(
                    false,
                    false,
                    HammerCategories.Names.Building,
                    nameof(CraftingStations.None),
                    "Wood,20;Bronze,4"
                )
            },
            {
                "Oak1",
                new PrefabConfig(
                    false,
                    false,
                    HammerCategories.Names.CreatorShop,
                    nameof(CraftingStations.None),
                    "Wood,20;FineWood,20"
                )
            },
            {
                "piece_dvergr_wood_door",
                new PrefabConfig(
                    false,
                    false,
                    HammerCategories.Names.Building,
                    nameof(CraftingStations.BlackForge),
                    "YggdrasilWood,16;IronNails,24"
                )
            },
            {
                "Pinetree_01",
                new PrefabConfig(
                    false,
                    false,
                    HammerCategories.Names.CreatorShop,
                    nameof(CraftingStations.None),
                    "Wood,16;RoundLog,16"
                )
            },
            {
                "RockFinger",
                new PrefabConfig(
                    false,
                    false,
                    HammerCategories.Names.CreatorShop,
                    nameof(CraftingStations.None),
                    "Stone,350"
                )
            },
            {
                "RockFingerBroken",
                new PrefabConfig(
                    false,
                    false,
                    HammerCategories.Names.CreatorShop,
                    nameof(CraftingStations.None),
                    "Stone,175"
                )
            },
            {
                "rockformation1",
                new PrefabConfig(
                    false,
                    false,
                    HammerCategories.Names.CreatorShop,
                    nameof(CraftingStations.None),
                    "Stone,350"
                )
            },
            {
                "RockThumb",
                new PrefabConfig(
                    false,
                    false,
                    HammerCategories.Names.CreatorShop,
                    nameof(CraftingStations.None),
                    "Stone,200"
                )
            },
            {
                "root07",
                new PrefabConfig(
                    false,
                    false,
                    HammerCategories.Names.CreatorShop,
                    nameof(CraftingStations.None),
                    "ElderBark,2"
                )
            },
            {
                "root08",
                new PrefabConfig(
                    false,
                    false,
                    HammerCategories.Names.CreatorShop,
                    nameof(CraftingStations.None),
                    "ElderBark,4"
                )
            },
            {
                "root11",
                new PrefabConfig(
                    false,
                    false,
                    HammerCategories.Names.CreatorShop,
                    nameof(CraftingStations.None),
                    "ElderBark,4"
                )
            },
            {
                "root12",
                new PrefabConfig(
                    false,
                    false,
                    HammerCategories.Names.CreatorShop,
                    nameof(CraftingStations.None),
                    "ElderBark,4"
                )
            },
            {
                "shipwreck_karve_bottomboards",
                new PrefabConfig(
                    false,
                    false,
                    HammerCategories.Names.Furniture,
                    nameof(CraftingStations.None),
                    "FineWood,8"
                )
            },
            {
                "shipwreck_karve_bow",
                new PrefabConfig(
                    false,
                    false,
                    HammerCategories.Names.Furniture,
                    nameof(CraftingStations.None),
                    "FineWood,16"
                )
            },
            {
                "shipwreck_karve_stern",
                new PrefabConfig(
                    false,
                    false,
                    HammerCategories.Names.Furniture,
                    nameof(CraftingStations.None),
                    "FineWood,16"
                )
            },
            {
                "stoneblock_fracture",
                new PrefabConfig(
                    true,
                    false,
                    HammerCategories.Names.Building,
                    nameof(CraftingStations.Stonecutter),
                    "Stone,16"
                )
            },
            {
                "SwampTree1",
                new PrefabConfig(
                    false,
                    false,
                    HammerCategories.Names.CreatorShop,
                    nameof(CraftingStations.None),
                    "ElderBark,16"
                )
            },
            {
                "SwampTree2",
                new PrefabConfig(
                    false,
                    false,
                    HammerCategories.Names.CreatorShop,
                    nameof(CraftingStations.None),
                    "ElderBark,32"
                )
            },
            {
                "SwampTree2_darkland",
                new PrefabConfig(
                    false,
                    false,
                    HammerCategories.Names.CreatorShop,
                    nameof(CraftingStations.None),
                    "ElderBark,32"
                )
            },
            {
                "SwampTree2_log",
                new PrefabConfig(
                    false,
                    false,
                    HammerCategories.Names.CreatorShop,
                    nameof(CraftingStations.None),
                    "ElderBark,32"
                )
            },
            {
                "TreasureChest_mountaincave",
                new PrefabConfig(
                    false,
                    false,
                    HammerCategories.Names.CreatorShop,
                    nameof(CraftingStations.None),
                    "Stone,10;Crystal,1"
                )
            },
            {
                "YggaShoot1",
                new PrefabConfig(
                    false,
                    false,
                    HammerCategories.Names.CreatorShop,
                    nameof(CraftingStations.None),
                    "YggdrasilWood,16"
                )
            },
            {
                "YggaShoot2",
                new PrefabConfig(
                    false,
                    false,
                    HammerCategories.Names.CreatorShop,
                    nameof(CraftingStations.None),
                    "YggdrasilWood,16"
                )
            },
            {
                "YggaShoot3",
                new PrefabConfig(
                    false,
                    false,
                    HammerCategories.Names.CreatorShop,
                    nameof(CraftingStations.None),
                    "YggdrasilWood,16"
                )
            },
        };
    }
}
