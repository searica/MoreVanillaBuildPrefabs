using System.Text.RegularExpressions;
using UnityEngine;

namespace MoreVanillaBuildPrefabs
{
    internal class PrefabNames
    {
        static readonly Regex PrefabNameRegex = new(@"([a-z])([A-Z])");
        public static string FormatPrefabName(string prefabName)
        {
            var name = PrefabNameRegex
                .Replace(prefabName, "$1 $2")
                .TrimStart(' ')
                .Replace('_', ' ')
                .Replace("  ", " ")
                .ToLower();
            name = RemovePrefix(name, "pickable");
            name = RemoveSuffix(name, "destructable");
            return CapitalizeFirstLetter(name).Trim();
        }

        public static string GetPrefabDescription(GameObject prefab)
        {
            if (prefab.name == "metalbar_1x2") return "Enforced marble 1x2";

            HoverText hover = prefab.GetComponent<HoverText>();
            if (hover) return hover.m_text;

            ItemDrop item = prefab.GetComponent<ItemDrop>();
            if (item) return item.m_itemData.m_shared.m_name;

            Character chara = prefab.GetComponent<Character>();
            if (chara) return chara.m_name;

            RuneStone runestone = prefab.GetComponent<RuneStone>();
            if (runestone) return runestone.m_name;

            ItemStand itemStand = prefab.GetComponent<ItemStand>();
            if (itemStand) return itemStand.m_name;

            MineRock mineRock = prefab.GetComponent<MineRock>();
            if (mineRock) return mineRock.m_name;

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

        public static string GetPrefabName(Piece piece)
        {
            return RemoveSuffix(piece.gameObject.name, "(Clone)");
        }

        public static string RemoveSuffix(string s, string suffix)
        {
            if (s.EndsWith(suffix))
            {
                return s.Substring(0, s.Length - suffix.Length);
            }

            return s;
        }

        public static string RemovePrefix(string s, string prefix)
        {
            if (s.StartsWith(prefix))
            {
                return s.Substring(prefix.Length, s.Length - prefix.Length);
            }
            return s;
        }

        static string CapitalizeFirstLetter(string s)
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
