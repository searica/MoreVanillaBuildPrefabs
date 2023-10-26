using System.Text.RegularExpressions;
using UnityEngine;
using MoreVanillaBuildPrefabs.Configs;

namespace MoreVanillaBuildPrefabs.Helpers
{
    internal class NameHelper
    {
        private static readonly Regex PrefabNameRegex = new(@"([a-z])([A-Z])");

        /// <summary>
        ///     Formats the prefab name to something friendlier
        ///     to use as a piece name, or applies a custom name map
        ///     if one exists.
        /// </summary>
        /// <param name="prefabName"></param>
        /// <returns></returns>
        internal static string FormatPrefabName(PieceDB pieceDB)
        {
            if (pieceDB.pieceName != null) return pieceDB.pieceName;

            var name = PrefabNameRegex
                .Replace(pieceDB.name, "$1 $2")
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
                .Replace("irongate", "iron gate")
                .Replace("goblin", "fuling");

            name = name.RemovePrefix("piece");

            if (name.EndsWith("destructible"))
            {
                name = name.RemoveSuffix("destructible").Trim();
                name = string.Concat(name, " (destructible)");
            }

            if (name.StartsWith("pickable"))
            {
                name = name.RemovePrefix("pickable").Trim();
                name = string.Concat(name, " (pickable)");
            }

            return name.CapitalizeFirstLetter();
        }

        internal static string GetPrefabDescription(PieceDB pieceDB)
        {
            if (pieceDB.pieceDesc != null) return pieceDB.pieceDesc;

            return GetPrefabDescription(pieceDB.Prefab);
        }

        internal static string GetPrefabDescription(GameObject prefab)
        {
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

        /// <summary>
        ///     Get name of parent prefab by stripping suffix "(Clone)" if nessecary
        /// </summary>
        /// <param name="piece"></param>
        /// <returns></returns>
        internal static string GetPrefabName(Piece piece)
        {
            return piece.gameObject.name.RemoveSuffix("(Clone)");
        }
    }
}