// Ignore Spelling: MVBP

using MVBP.Configs;
using MVBP.Extensions;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

namespace MVBP.Helpers
{
    internal class NameHelper
    {
        /// <summary>
        ///     Matches (text)(#)(text) but only if the # is not preceded by an "x"
        ///     and not followed by an "x", "m", or "°"
        /// </summary>
        private static readonly Regex DigitsToEndRegex = new(@"(.+?)((?<!x)\d+(?![xm\°]))(.*)");

        /// <summary>
        ///     Matches (text)(text) where the second group
        ///     is capitalized and the first is lower-case
        /// </summary>
        private static readonly Regex SplitCapitalsRegex = new(@"([a-z])([A-Z])");

        /// <summary>
        ///     Matches (text)(creep)(text)
        /// </summary>
        private static readonly Regex CreepToEndRegex = new(@"(.+?)(creep)(.*)");

        /// <summary>
        ///     Matches sequences of whitespace
        /// </summary>
        private static readonly Regex WhiteSpaceRegex = new(@" +");

        /// <summary>
        ///  Matches (#)(m) to find text describing units of length
        /// </summary>
        private static readonly Regex UnitSpaceRegex = new(@"(\d+)(m)");

        /// <summary>
        ///     Matches the sequence of number at the end of the string.
        /// </summary>
        private static readonly Regex IsLastCharDigit = new(@"((?<!x)\d+$)");

        /// <summary>
        ///     Matches (text)(_frac) if it is at the end of the string
        /// </summary>
        private static readonly Regex FracAtEnd = new(@"(.+?)(_frac$)");

        /// <summary>
        ///     Matches (text)(_destruction) if it is at the end of the string
        /// </summary>
        private static readonly Regex DestructAtEnd = new(@"(.+?)(_destruction$)");

        private static readonly Dictionary<string, string> NameCache = new();
        private static readonly Dictionary<string, string> DescCache = new();

        private const string MineRock5Warn = "Warning: This prefab spawns a destructible rock when damaged and the new rock cannot be removed with the hammer.";

        /// <summary>
        ///     Checks if NameCache contains a value.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        internal static bool IsNameCached(string name)
        {
            return NameCache.ContainsValue(name);
        }

        internal static void ClearNameCache()
        {
            NameCache.Clear();
        }

        internal static void ClearDescCache()
        {
            DescCache.Clear();
        }

        /// <summary>
        ///     Formats the prefab name to something friendlier
        ///     to use as a piece name, or applies a custom name map
        ///     if one exists.
        /// </summary>
        /// <param name="pieceDB"></param>
        /// <returns></returns>
        internal static string FormatPrefabName(PieceDB pieceDB)
        {
            // TODO: custom renaming format for BossStone prefabs
            if (NameCache.ContainsKey(pieceDB.name))
            {
                return NameCache[pieceDB.name];
            }

            if (pieceDB.pieceName != null)
            {
                NameCache[pieceDB.name] = pieceDB.pieceName;
                return pieceDB.pieceName;
            }

            var name = FracAtEnd.Replace(pieceDB.name, "$1"); // strips "_frac" from the end
            name = DestructAtEnd.Replace(name, "$1"); // strips "_destruction" from the end
            name = CreepToEndRegex.Replace(name, "$1$3 ($2)");
            name = DigitsToEndRegex.Replace(name, "$1$3 $2");
            name = UnitSpaceRegex.Replace(name, "$1 $2");
            name = SplitCapitalsRegex.Replace(name, "$1 $2");

            name = name.Replace('_', ' ')
                .ToLower()
                .Replace("dverger", "dvergr")
                .Replace("dvergrtown", "dvergr")
                .Replace("dvergrprops", "dvergr")
                .Replace("destructable", "destructible")
                .Replace("rockdolmen", "rock dolmen")
                .Replace("blackmarble", "black marble")
                .Replace("sunkencrypt", "sunken crypt")
                .Replace("irongate", "iron gate")
                .Replace("goblin", "fuling")
                .Replace("hugeroot", "ancient root")
                .Replace("stubbe", "stump")
                .Replace("stub", "stump")
                .Replace("swamptree", "Ancient tree")
                .Replace("swamp tree", "Ancient tree")
                .Replace("ygga", "yggdrasil ")
                .Replace("guardstone", "ward")
                .Replace("woodwall", "wood wall")
                .Trim();

            name = name.RemovePrefix("piece").TrimStart();
            name = name.RemovePrefix("dungeon").TrimStart();

            if (name.EndsWith("destructible"))
            {
                name = name.RemoveSuffix("destructible");
                name = string.Concat(name, " (destructible)");
            }

            if (name.StartsWith("pickable"))
            {
                name = name.RemovePrefix("pickable").TrimStart();
                name = string.Concat(name, " (pickable)");
            }

            if (name.StartsWithAny("mountainkit", "mountain kit"))
            {
                name = name.RemovePrefix("mountainkit");
                name = name.RemovePrefix("mountain kit").TrimStart();
                name = string.Concat(name, " (cave)");
            }

            if (name.StartsWithAny("sunkencrypt", "sunken crypt"))
            {
                name = name.RemovePrefix("sunkencrypt");
                name = name.RemovePrefix("sunken crypt").TrimStart();
                name = string.Concat(name, " (crypt)");
            }

            if (name.StartsWithAny("forestcrypt", "forest crypt"))
            {
                name = name.RemovePrefix("forestcrypt");
                name = name.RemovePrefix("forest crypt").TrimStart();
                name = string.Concat(name, " (tomb)");
            }

            name = IsLastCharDigit.Replace(name, " ($1)");
            name = WhiteSpaceRegex.Replace(name, " ");
            name = name.CapitalizeFirstLetter();

            NameCache[pieceDB.name] = name;
            return name;
        }

        internal static string GetPrefabDescription(PieceDB pieceDB)
        {
            if (DescCache.ContainsKey(pieceDB.name))
            {
                return DescCache[pieceDB.name];
            }

            if (pieceDB.pieceDesc != null)
            {
                DescCache[pieceDB.name] = pieceDB.pieceDesc;
                return pieceDB.pieceDesc;
            }

            pieceDB.pieceDesc = FindPrefabDescription(pieceDB.Prefab);
            DescCache[pieceDB.name] = pieceDB.pieceDesc;
            return pieceDB.pieceDesc;
        }

        private static string FindPrefabDescription(GameObject prefab)
        {
            HoverText hover = prefab.GetComponent<HoverText>();
            if (hover && !string.IsNullOrEmpty(hover.m_text))
            {
                DescCache[prefab.name] = hover.m_text;
                return hover.m_text;
            }

            ItemDrop item = prefab.GetComponent<ItemDrop>();
            if (item && !string.IsNullOrEmpty(item.m_itemData.m_shared.m_name))
            {
                DescCache[prefab.name] = item.m_itemData.m_shared.m_name;
                return item.m_itemData.m_shared.m_name;
            }

            Character chara = prefab.GetComponent<Character>();
            if (chara && !string.IsNullOrEmpty(chara.m_name))
            {
                DescCache[prefab.name] = chara.m_name;
                return chara.m_name;
            }

            RuneStone runestone = prefab.GetComponent<RuneStone>();
            if (runestone && !string.IsNullOrEmpty(runestone.m_name))
            {
                DescCache[prefab.name] = runestone.m_name;
                return runestone.m_name;
            }

            ItemStand itemStand = prefab.GetComponent<ItemStand>();
            if (itemStand && !string.IsNullOrEmpty(itemStand.m_name))
            {
                DescCache[prefab.name] = itemStand.m_name;
                return itemStand.m_name;
            }

            MineRock mineRock = prefab.GetComponent<MineRock>();
            if (mineRock && !string.IsNullOrEmpty(mineRock.m_name))
            {
                DescCache[prefab.name] = mineRock.m_name;
                return mineRock.m_name;
            }

            Pickable pickable = prefab.GetComponent<Pickable>();
            if (pickable) return FindPrefabDescription(pickable.m_itemPrefab);

            CreatureSpawner creatureSpawner = prefab.GetComponent<CreatureSpawner>();
            if (creatureSpawner) return FindPrefabDescription(creatureSpawner.m_creaturePrefab);

            SpawnArea spawnArea = prefab.GetComponent<SpawnArea>();
            if (spawnArea && spawnArea.m_prefabs.Count > 0)
            {
                return FindPrefabDescription(spawnArea.m_prefabs[0].m_prefab);
            }
            return "";
        }

        /// <summary>
        ///     Get name of parent prefab by stripping suffix "(Clone)" if nessecary
        /// </summary>
        /// <param name="piece"></param>
        /// <returns></returns>
        internal static string GetRootPrefabName(Piece piece)
        {
            return piece.gameObject.name.RemoveSuffix("(Clone)");
        }
    }
}