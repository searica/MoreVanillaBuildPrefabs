using BepInEx.Configuration;
using System.Collections.Generic;
using System.Linq;

namespace MoreVanillaBuildPrefabs.Configs
{
    internal class PieceTables
    {
        /// <summary>
        ///     Hammer piece table
        /// </summary>
        internal static string Hammer => "_HammerPieceTable";

        /// <summary>
        ///     Cultivator piece table
        /// </summary>
        internal static string Cultivator => "_CultivatorPieceTable";

        /// <summary>
        ///     Hoe piece table
        /// </summary>
        internal static string Hoe => "_HoePieceTable";

        /// <summary>
        ///     Gets the human readable name to internal names map
        /// </summary>
        /// <returns></returns>
        internal static Dictionary<string, string> GetNames()
        {
            return NamesMap;
        }

        /// <summary>
        ///     Get a <see cref="BepInEx.Configuration.AcceptableValueList{T}"/> of all piece table names.
        ///     This can be used to create a <see cref="BepInEx.Configuration.ConfigEntry{T}"/> where only valid piece table can be selected.<br/><br/>
        ///     Example:
        ///     <code>
        ///          var pieceTableConfig = Config.Bind("Section", "Key", nameof(PieceTables.Hammer), new ConfigDescription("Description", PieceTables.GetAcceptableValueList()));
        ///     </code>
        /// </summary>
        /// <returns></returns>
        internal static AcceptableValueList<string> GetAcceptableValueList()
        {
            return AcceptableValues;
        }

        /// <summary>
        ///     Get the internal name for a piece table from its human readable name.
        /// </summary>
        /// <param name="pieceTable"></param>
        /// <returns>
        ///     The matched internal name.
        ///     If the pieceTable parameter is null or empty, an empty string is returned.
        ///     Otherwise the unchanged pieceTable parameter is returned.
        /// </returns>
        internal static string GetInternalName(string pieceTable)
        {
            if (string.IsNullOrEmpty(pieceTable))
            {
                return string.Empty;
            }

            if (NamesMap.TryGetValue(pieceTable, out string internalName))
            {
                return internalName;
            }

            return pieceTable;
        }

        private static readonly Dictionary<string, string> NamesMap = new()
        {
            { nameof(Hammer), Hammer },
            { nameof(Cultivator), Cultivator },
            { nameof(Hoe), Hoe },
        };

        private static readonly AcceptableValueList<string> AcceptableValues = new(NamesMap.Keys.ToArray());
    }
}
