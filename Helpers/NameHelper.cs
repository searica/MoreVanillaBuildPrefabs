using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace MoreVanillaBuildPrefabs.Helpers
{
    internal class NameHelper
    {
        private static readonly Dictionary<string, string> NamesMap = new()
        {
            {"stoneblock_fracture", "Stone floor2 4x4"},
            {"dvergrprops_hooknchain", "Dvergr hook & chain"},
            {"dvergrprops_wood_wall", "Dvergr wood wall 4x4"},
            {"piece_dvergr_wood_wall", "Dvergr wood wall"},
            {"blackmarble_floor_large", "Black marble floor 8x8"},
            {"TreasureChest_fCrypt", "Stone chest (mossy)" },
            {"TreasureChest_mountaincave", "Stone chest (snow)" },
            {"TreasureChest_plains_stone", "Stone chest (light moss)"},
            {"TreasureChest_trollcave", "Stone chest (mossy, big)"},
            {"TreasureChest_dvergr_loose_stone", "Black marble chest"},
            {"TreasureChest_sunkencrypt", "Stone chest (dark moss)"},
            {"TreasureChest_dvergrtower", "Dvergr chest"},
            {"TreasureChest_dvergrtown", "Dvergr chest (large)"},
            {"stonechest", "Stone chest"},
            {"fire_pit_hildir", "Firepit iron (everburning)"},
            {"fire_pit_haldor", "Campfire (everburning)"},
            {"Birch1_aut", "Birch1 (autumn)"},
            {"Birch2_aut", "Birch2 (autumn)"},
            {"dvergrtown_slidingdoor", "Dvergr sliding door"},
            {"dvergrtown_secretdoor", "Dvergr secret door"},
        };

        private static readonly Dictionary<string, string> DescriptionMap = new()
        {
            {"metalbar_1x2", "Enforced marble 1x2"},
        };

        private static readonly Regex PrefabNameRegex = new(@"([a-z])([A-Z])");

        /// <summary>
        ///     Formats the prefab name to something friendlier 
        ///     to use as a piece name, or applies a custom name map
        ///     if one exists.
        /// </summary>
        /// <param name="prefabName"></param>
        /// <returns></returns>
        internal static string FormatPrefabName(string prefabName)
        {
            if (NamesMap.ContainsKey(prefabName)) return NamesMap[prefabName];

            var name = PrefabNameRegex
                .Replace(prefabName, "$1 $2")
                .TrimStart(' ')
                .Replace('_', ' ')
                .Replace("  ", " ")
                .ToLower()
                .Replace("dverger", "dvergr")
                .Replace("dvergrtown", "dvergr")
                .Replace("dvergrprops", "dvergr")
                .Replace("destructable", "destructible")
                .Replace("rockdolmen", "rock dolmen")
                .Replace("blackmarble", "black marble")
                .Replace("sunkencrypt", "sunken crypt")
                .Replace("irongate", "iron gate");
            //.Replace("secretdoor", "secret door")
            //.Replace("slidngdoor", "sliding door");

            name = RemovePrefix(name, "piece");

            if (name.EndsWith("destructible"))
            {
                name = string.Concat(
                    RemoveSuffix(name, "destructible").Trim(),
                    " (destructible)"
                );
            }

            if (name.StartsWith("pickable"))
            {
                name = string.Concat(
                    RemovePrefix(name, "pickable").Trim(),
                    " (pickable)"
                );
            }
            return CapitalizeFirstLetter(name);
        }

        internal static string GetPrefabDescription(GameObject prefab)
        {
            if (DescriptionMap.ContainsKey(prefab.name)) return DescriptionMap[prefab.name];

            HoverText hover = prefab.GetComponent<HoverText>();
            if (hover && !string.IsNullOrEmpty(hover.m_text)) return hover.m_text;

            ItemDrop item = prefab.GetComponent<ItemDrop>();
            if (item && !string.IsNullOrEmpty(item.m_itemData.m_shared.m_name)) return item.m_itemData.m_shared.m_name;

            Character chara = prefab.GetComponent<Character>();
            if (chara && !string.IsNullOrEmpty(chara.m_name)) return chara.m_name;

            RuneStone runestone = prefab.GetComponent<RuneStone>();
            if (runestone && !string.IsNullOrEmpty(runestone.m_name)) return runestone.m_name;

            ItemStand itemStand = prefab.GetComponent<ItemStand>();
            if (itemStand && !string.IsNullOrEmpty(itemStand.m_name)) return itemStand.m_name;

            MineRock mineRock = prefab.GetComponent<MineRock>();
            if (mineRock && !string.IsNullOrEmpty(mineRock.m_name)) return mineRock.m_name;

            Pickable pickable = prefab.GetComponent<Pickable>();
            if (pickable) return GetPrefabDescription(pickable.m_itemPrefab);

            CreatureSpawner creatureSpawner = prefab.GetComponent<CreatureSpawner>();
            if (creatureSpawner) return GetPrefabDescription(creatureSpawner.m_creaturePrefab);

            SpawnArea spawnArea = prefab.GetComponent<SpawnArea>();
            if (spawnArea && spawnArea.m_prefabs.Count > 0)
            {
                return GetPrefabDescription(spawnArea.m_prefabs[0].m_prefab);
            }

            Piece piece = prefab.GetComponent<Piece>();
            if (piece && !string.IsNullOrEmpty(piece.m_name)) return piece.m_name;

            return prefab.name;
        }

        internal static string GetPrefabName(Piece piece)
        {
            return RemoveSuffix(piece.gameObject.name, "(Clone)");
        }

        internal static string RemoveSuffix(string s, string suffix)
        {
            if (s.EndsWith(suffix))
            {
                return s.Substring(0, s.Length - suffix.Length);
            }

            return s;
        }

        internal static string RemovePrefix(string s, string prefix)
        {
            if (s.StartsWith(prefix))
            {
                return s.Substring(prefix.Length, s.Length - prefix.Length);
            }
            return s;
        }

        internal static string CapitalizeFirstLetter(string s)
        {
            if (s.Length == 0)
                return s;
            else if (s.Length == 1)
                return $"{char.ToUpper(s[0])}";
            else
                return char.ToUpper(s[0]) + s.Substring(1);
        }

    }
}
