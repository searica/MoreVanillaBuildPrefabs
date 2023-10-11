using BepInEx.Configuration;
using System.Collections.Generic;
using System.Linq;


namespace MoreVanillaBuildPrefabs.Configs
{
    /// <summary>
    ///     Helper to get existing crafting station names
    /// </summary>
    internal static class CraftingStations
    {
        /// <summary>
        ///     No crafting station
        /// </summary>
        internal static string None => string.Empty;

        /// <summary>
        ///    Workbench crafting station
        /// </summary>
        internal static string Workbench => "piece_workbench";

        /// <summary>
        ///    Forge crafting station
        /// </summary>
        internal static string Forge => "forge";

        /// <summary>
        ///     Stonecutter crafting station
        /// </summary>
        internal static string Stonecutter => "piece_stonecutter";

        /// <summary>
        ///     Cauldron crafting station
        /// </summary>
        internal static string Cauldron => "piece_cauldron";

        /// <summary>
        ///     Artisan table crafting station
        /// </summary>
        internal static string ArtisanTable => "piece_artisanstation";

        /// <summary>
        ///     Black forge crafting station
        /// </summary>
        internal static string BlackForge => "blackforge";

        /// <summary>
        ///     Galdr table crafting station
        /// </summary>
        internal static string GaldrTable => "piece_magetable";


        private static readonly Dictionary<string, string> NamesMap = new()
                    {
                        { nameof(None), None },
                        { nameof(Workbench), Workbench },
                        { nameof(Forge), Forge },
                        { nameof(Stonecutter), Stonecutter },
                        { nameof(Cauldron), Cauldron },
                        { nameof(ArtisanTable), ArtisanTable },
                        { nameof(BlackForge), BlackForge },
                        { nameof(GaldrTable), GaldrTable },
                    };

        private static readonly AcceptableValueList<string> AcceptableValues = new(NamesMap.Keys.ToArray());

        /// <summary>
        ///     Gets the human readable name to internal names map
        /// </summary>
        /// <returns></returns>
        internal static Dictionary<string, string> GetNames()
        {
            return NamesMap;
        }

        /// <summary>
        ///     Get a <see cref="AcceptableValueList{T}"/> of all crafting station names.
        ///     This can be used to create a <see cref="ConfigEntry{T}"/> where only valid crafting stations can be selected.<br/><br/>
        ///     Example:
        ///     <code>
        ///         var stationConfig = Config.Bind("Section", "Key", nameof(CraftingStations.Workbench), new ConfigDescription("Description", CraftingStations.GetAcceptableValueList()));
        ///     </code>
        /// </summary>
        /// <returns></returns>
        internal static AcceptableValueList<string> GetAcceptableValueList()
        {
            return AcceptableValues;
        }

        /// <summary>
        ///     Get the internal name for a crafting station from its human readable name.
        /// </summary>
        /// <param name="craftingStation"></param>
        /// <returns>
        ///     The matched internal name.
        ///     If the craftingStation parameter is null or empty, <see cref="None"/> is returned.
        ///     Otherwise the unchanged craftingStation parameter is returned.
        /// </returns>
        /// 
        internal static string GetInternalName(string craftingStation)
        {
            if (string.IsNullOrEmpty(craftingStation))
            {
                return None;
            }

            if (NamesMap.TryGetValue(craftingStation, out string internalName))
            {
                return internalName;
            }

            return craftingStation;
        }

        /// <summary>
        ///     Get CraftingStation object from either the human readable or internal name.
        /// </summary>
        /// <param name="craftingStation"></param>
        /// <returns></returns>
        internal static CraftingStation GetCraftingStation(string name)
        {
            var internalName = GetInternalName(name);
            var station = ZNetScene.instance?.GetPrefab(internalName)?.GetComponent<CraftingStation>();
            return station;
        }
    }
}
