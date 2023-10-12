using System;
using System.Collections.Generic;
using UnityEngine;

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
            "turf_roof_wall", // Visual duplicate of "wood_roof_wall"
            "rock_mistlands2", // Explodes into a boulder "___MineRock5"
            "demister_ball", // Placement is glitchy
            "CargoCrate", // Deletes itself on placement because it's empty
            "TreasureChest_blackforest", // Visual duplicate of wooden chest
            "TreasureChest_heath", // Visual duplicate of wooden chest
            "TreasureChest_heath_hildir", // Visual duplicate of wooden chest
            "TreasureChest_heath_hildir", // Visual duplicate of wooden chest
            "TreasureChest_meadows", // Visual duplicate of wooden chest
            "TreasureChest_meadows_buried", // Visual duplicate of wooden chest
            "TreasureChest_mountains", // Visual duplicate of wooden chest
            "TreasureChest_swamp", // Visual duplicate of wooden chest
            "TreasureChest_forestcrypt", // Visual duplicate of "TreasureChest_fCrypt"
            "TreasureChest_mountaincave_hildir", // Visual duplicate of "TreasureChest_dvergrtown"
            "TreasureChest_plainsfortress_hildir", // Visual duplicate of "TreasureChest_dvergrtown"
            "TreasureChest_forestcrypt_hildir", // Visual duplicate of "TreasureChest_dvergrtown"
        };
    }
}
