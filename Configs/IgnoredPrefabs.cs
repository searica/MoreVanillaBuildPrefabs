using System;
using System.Collections.Generic;
using System.Numerics;
using System.Reflection;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.UI;

namespace MoreVanillaBuildPrefabs.Configs
{
    internal class IgnoredPrefabs
    {
        /// <summary>
        ///     Checks prefab to see if it is eligble for making a custom piece.
        /// </summary>
        /// <param name="prefab"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        internal static bool ShouldIgnorePrefab(GameObject prefab)
        {
            // Ignore specific prefab names
            if (_IgnoredPrefabs.Contains(prefab.name))
            {
                return true;
            }

            if (!PluginConfig.IsCreativeMode && _CreativeModePrefabs.Contains(prefab.name))
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
                prefab.name.EndsWith("OLD") ||
                prefab.name.StartsWith("vfx_") ||
                prefab.name.StartsWith("sfx_") ||
                prefab.name.StartsWith("fx_")
            )
            {
                return true;
            }
            return false;
        }

        private static readonly HashSet<string> _IgnoredPrefabs = new() {
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
            "Pickable_DvergerThing",
            "rock_mistlands2", // Explodes into a boulder "___MineRock5"
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

            // Ignore crops you can already plant
            "Pickable_SeedCarrot",
            "Pickable_SeedTurnip",
            "Pickable_SeedOnion",
            "Pickable_Onion",
            "Pickable_Carrot",
            "Pickable_Turnip",
            "Pickable_Flax",
            "Pickable_Flax_Wild",
            "Pickable_Barley",
            "Pickable_Barley_Wild",
            "Pickable_Mushroom_Magecap",
            "Pickable_Mushroom_JotunPuffs",

            // Not useful or different
            "CastleKit_groundtorch_unlit",
            "CastleKit_metal_groundtorch_unlit",
            "dvergrprops_lantern",
        };

        /// <summary>
        ///  These are prefabs I think people may want to be able to use but they
        ///  aren't really normal build pieces and could be used for griefing.
        /// </summary>
        private static readonly HashSet<string> _CreativeModePrefabs = new()
        {
            // CreatorShop Pieces:
            "ancient_skull",
            "blackmarble_altar_crystal",
            "blackmarble_altar_crystal_broken",
            "BossStone_Bonemass",
            "BossStone_DragonQueen",
            "BossStone_Eikthyr",
            "BossStone_TheElder",
            "BossStone_TheQueen",
            "BossStone_Yagluth",
            "cliff_mistlands1",
            "cliff_mistlands1_creep",
            "cliff_mistlands2",
            "crypt_skeleton_chest",
            "dungeon_forestcrypt_door",
            "dverger_demister_broken",
            "dverger_demister_ruins",
            "dvergrprops_lantern_standing",
            "dvergrprops_pickaxe",
            "dvergrtown_secretdoor",
            "dvergrtown_slidingdoor",
            "dvergrtown_wood_pole",
            "fenrirhide_hanging",
            "fenrirhide_hanging_door",
            "flying_core",
            "fuling_trap",
            "giant_arm",
            "giant_brain",
            "giant_helmet1",
            "giant_helmet2",
            "giant_ribs",
            "giant_skull",
            "giant_sword1",
            "giant_sword2",
            "goblin_totempole",
            "goblin_trashpile",
            "goblin_woodwall_2m_ribs",
            "goblinking_totemholder",
            "GuckSack",
            "GuckSack_small",
            "hanging_hairstrands",
            "Ice_floor",
            "ice_rock1",
            "ice1",
            "IceBlocker",
            "Leviathan",
            "lox_ribs",
            "LuredWisp",
            "marker01",
            "marker02",
            "mistvolume",
            "MountainGraveStone01",
            "mudpile",
            "mudpile_beacon",
            "mudpile_old",
            "mudpile2",
            "Pickable_BlackCoreStand",
            "Pickable_BogIronOre",
            "Pickable_DolmenTreasure",
            "Pickable_DragonEgg",
            "Pickable_DvergrLantern",
            "Pickable_DvergrMineTreasure",
            "Pickable_DvergrStein",
            "Pickable_Fishingrod",
            "Pickable_ForestCryptRandom",
            "Pickable_ForestCryptRemains01",
            "Pickable_ForestCryptRemains02",
            "Pickable_ForestCryptRemains03",
            "Pickable_ForestCryptRemains04",
            "Pickable_Hairstrands01",
            "Pickable_Hairstrands02",
            "Pickable_Item",
            "Pickable_MeatPile",
            "Pickable_MountainRemains01_buried",
            "Pickable_RoyalJelly",
            "Pickable_SunkenCryptRandom",
            "Pickable_SurtlingCoreStand",
            "RockFinger",
            "RockFingerBroken",
            "RockThumb",
            "SeekerEgg",
            "SeekerEgg_alwayshatch",
            "shipwreck_karve_bottomboards",
            "shipwreck_karve_bow",
            "shipwreck_karve_dragonhead",
            "shipwreck_karve_stern",
            "shipwreck_karve_sternpost",
            "Skull1",
            "Skull2",
            "stubbe_spawner",
            "tarlump1",
            "tunnel_web",
            "vertical_web",

            // Nature Pieces
            "beech_log",
            "beech_log_half",
            "Beech_small1",
            "Beech_small2",
            "Beech_Stub",
            "Beech1",
            "Birch_log",
            "Birch_log_half",
            "Birch1",
            "Birch1_aut",
            "Birch2",
            "Birch2_aut",
            "BirchStub",
            "BlueberryBush",
            "Bush01",
            "Bush01_heath",
            "Bush02_en",
            "caverock_ice_pillar_wall",
            "caverock_ice_stalagmite",
            "caverock_ice_stalagmite_broken",
            "caverock_ice_stalagtite",
            "caverock_ice_stalagtite_falling",
            "CloudberryBush",
            "CreepProp_egg_hanging01",
            "CreepProp_egg_hanging02",
            "CreepProp_entrance1",
            "CreepProp_entrance2",
            "CreepProp_hanging01",
            "CreepProp_wall01",
            "dvergrtown_creep_door",
            "FirTree",
            "FirTree_log",
            "FirTree_log_half",
            "FirTree_oldLog",
            "FirTree_small",
            "FirTree_small_dead",
            "FirTree_Stub",
            "GlowingMushroom",
            "Greydwarf_Root",
            "highstone",
            "HugeRoot1",
            "MineRock_Copper",
            "MineRock_Iron",
            "MineRock_Meteorite",
            "MineRock_Obsidian",
            "MineRock_Stone",
            "MineRock_Tin",
            "Oak_log",
            "Oak_log_half",
            "Oak1",
            "OakStub",
            "Pickable_Branch",
            "Pickable_Dandelion",
            "Pickable_Flint",
            "Pickable_Meteorite",
            "Pickable_MountainCaveCrystal",
            "Pickable_MountainCaveObsidian",
            "Pickable_MountainCaveRandom",
            "Pickable_Mushroom",
            "Pickable_Mushroom_blue",
            "Pickable_Mushroom_yellow",
            "Pickable_Obsidian",
            "Pickable_Stone",
            "Pickable_Tar",
            "Pickable_TarBig",
            "Pickable_Thistle",
            "Pickable_Tin",
            "PineTree",
            "Pinetree_01",
            "Pinetree_01_Stub",
            "PineTree_log",
            "PineTree_log_half",
            "RaspberryBush",
            "Rock_3",
            "Rock_4",
            "Rock_4_plains",
            "Rock_7",
            "Rock_destructible",
            "Rock_destructible_test",
            "rock_mistlands1",
            "rock1_mistlands",
            "rock1_mountain",
            "rock2_heath",
            "rock2_mountain",
            "rock3_ice",
            "rock3_mountain",
            "rock3_silver",
            "rock4_coast",
            "rock4_copper",
            "rock4_forest",
            "rock4_heath",
            "RockDolmen_1",
            "RockDolmen_2",
            "RockDolmen_3",
            "rockformation1",
            "root07",
            "root08",
            "root11",
            "root12",
            "ShootStump",
            "shrub_2",
            "shrub_2_heath",
            "silvervein",
            "stubbe",
            "SwampTree1",
            "SwampTree1_log",
            "SwampTree1_Stub",
            "SwampTree2",
            "SwampTree2_darkland",
            "SwampTree2_log",
            "vines",
            "widestone",
            "yggashoot_log",
            "yggashoot_log_half",
            "YggaShoot_small1",
            "YggaShoot1",
            "YggaShoot2",
            "YggaShoot3",
            "YggdrasilRoot",
        };
    }
}

// Old List of pieces
//"BossStone_Bonemass",
//"BossStone_DragonQueen",
//"BossStone_Eikthyr",
//"BossStone_TheElder",
//"BossStone_TheQueen",
//"BossStone_Yagluth",
//"beech_log",
//"beech_log_half",
//"Beech_small1",
//"Beech_small2",
//"Beech_Stub",
//"Beech1",
//"Birch_log",
//"Birch_log_half",
//"Birch1",
//"Birch1_aut",
//"Birch2",
//"Birch2_aut",
//"BirchStub",
//"blackmarble_altar_crystal_broken",
//"blackmarble_altar_crystal",
//"CreepProp_egg_hanging01",
//"CreepProp_egg_hanging02",
//"CreepProp_entrance1",
//"CreepProp_entrance2",
//"CreepProp_hanging01",
//"CreepProp_wall01",
//"crypt_skeleton_chest", // This is for building dungeons
//"dungeon_queen_door",
//"dvergrtown_creep_door",
//"dvergrtown_slidingdoor",
//"FirTree",
//"FirTree_log",
//"FirTree_log_half",
//"FirTree_oldLog",
//"FirTree_small",
//"FirTree_small_dead",
//"FirTree_Stub",
//"giant_arm",
//"giant_brain",
//"giant_helmet1",
//"giant_helmet2",
//"giant_ribs",
//"giant_skull",
//"giant_sword1",
//"giant_sword2",
//"goblinking_totemholder",
//"GuckSack",
//"GuckSack_small",
//"highstone",
//"Leviathan",
//"LuredWisp",
//"MineRock_Iron",
//"MineRock_Copper",
//"MineRock_Meteorite",
//"MineRock_Obsidian",
//"MineRock_Stone",
//"MineRock_Tin",
//"mistvolume", // environmental thing and too easy to use for griefing
//"mudpile",
//"mudpile_beacon",
//"mudpile_old",
//"mudpile2",
//"mountainkit_chair",
//"mountainkit_table",
//"Oak_log",
//"Oak_log_half",
//"Oak1",
//"OakStub",
//"Pickable_BogIronOre",
//"Pickable_DragonEgg",
//"Pickable_RoyalJelly",
//"Pickable_Obsidian",
//"Pickable_Tar",
//"Pickable_TarBig",
//"Pickable_DvergrLantern",
//"Pickable_DvergrStein",
//"Pickable_Meteorite",
//"Pickable_MountainCaveObsidian",
//"Pickable_Tin",
//"Pickable_BlackCoreStand",
//"Pickable_SurtlingCoreStand",
//"PineTree",
//"Pinetree_01",
//"Pinetree_01_Stub",
//"PineTree_log",
//"PineTree_log_half",
//"PineTree_log_halfOLD",
//"PineTree_logOLD",
//"Rock_3",
//"Rock_4",
//"Rock_4_plains",
//"Rock_7",
//"Rock_destructible",
//"Rock_destructible_test",
//"rock_mistlands1",
//"rock1_mistlands",
//"rock1_mountain",
//"rock2_heath",
//"rock2_mountain",
//"rock3_ice",
//"rock3_mountain",
//"rock3_silver",
//"rock4_coast",
//"rock4_copper",
//"rock4_forest",
//"rock4_heath",
//"RockDolmen_1",
//"RockDolmen_2",
//"RockDolmen_3",
//"RockFinger",
//"RockFingerBroken",
//"rockformation1",
//"RockThumb",
//"root07",
//"root08",
//"root11",
//"root12",
//"SeekerEgg_alwayshatch",
//"SeekerEgg",
//"shipwreck_karve_bottomboards",
//"shipwreck_karve_bow",
//"shipwreck_karve_dragonhead",
//"shipwreck_karve_stern",
//"shipwreck_karve_sternpost",
//"ShootStump",
//"silvervein",
//"stubbe",
//"stubbe_spawner",
//"SwampTree1",
//"SwampTree1_log",
//"SwampTree1_Stub",
//"SwampTree2",
//"SwampTree2_darkland",
//"SwampTree2_log",
//"tarlump1", // environmental thing and too easy to use for griefing
//"tunnel_web", // environmental thing and too easy to use for griefing
//"vertical_web",
//"widestone",
//"yggashoot_log",
//"yggashoot_log_half",
//"YggaShoot_small1",
//"YggaShoot1",
//"YggaShoot2",
//"YggaShoot3",
//"YggdrasilRoot",