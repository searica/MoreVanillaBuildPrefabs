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

            // Ignore crops you can already plant
            "Pickable_SeedCarrot",
            "Pickable_SeedTurnip",
            "Pickable_SeedOnion",
            "Pickable_Onion",
            "Pickable_Flax",
            "Pickable_Flax_Wild",
            "Pickable_Barley",
            "Pickable_Barley_Wild",
            "Pickable_Mushroom_Magecap",
            "Pickable_Mushroom_JotunPuffs"
        };

        /// <summary>
        ///  These are prefabs I think people may want to be able to use but they
        ///  aren't really normal build pieces and could be used for griefing.
        /// </summary>
        private static readonly HashSet<string> _CreativeModePrefabs = new()
        {
            "tarlump1", // environmental thing and too easy to use for griefing
            "mistvolume", // environmental thing and too easy to use for griefing
            "tunnel_web", // environmental thing and too easy to use for griefing
            "crypt_skeleton_chest", // This is for building dungeons
            "BossStone_Bonemass",
            "BossStone_DragonQueen",
            "BossStone_Eikthyr",
            "BossStone_TheElder",
            "BossStone_TheQueen",
            "BossStone_Yagluth",
            "giant_helmet1",
            "giant_helmet2",
            "giant_ribs",
            "giant_skull",
            "giant_sword1",
            "giant_sword2",
            "goblinking_totemholder",
            "GuckSack",
            "GuckSack_small",
            "LuredWisp",
            "MineRock_Iron",
            "MineRock_Copper",
            "giant_brain",
            "giant_arm",
            "CreepProp_egg_hanging01",
            "CreepProp_egg_hanging02",
            "CreepProp_entrance1",
            "CreepProp_entrance2",
            "CreepProp_hanging01",
            "CreepProp_wall01",
            "dungeon_queen_door",
            "dvergrtown_creep_door",
            "MineRock_Meteorite",
            "MineRock_Obsidian",
            "MineRock_Stone",
            "MineRock_Tin",
            "mudpile",
            "mudpile_beacon",
            "mudpile_old",
            "mudpile2",
            "Pickable_Meteorite",
            "Pickable_MountainCaveObsidian",
            "Pickable_Tin",
            "RockFinger",
            "RockFingerBroken",
            "silvervein",
            "vertical_web",
            "SeekerEgg_alwayshatch",
            "SeekerEgg",
            "blackmarble_altar_crystal_broken",
            "blackmarble_altar_crystal",
            "Pickable_BogIronOre",
            "Pickable_DragonEgg",
            "Pickable_RoyalJelly",
            "Pickable_Obsidian",
            "Pickable_Tar",
            "Pickable_TarBig",
            "mountainkit_chair",
            "mountainkit_table",
        };
    }
}