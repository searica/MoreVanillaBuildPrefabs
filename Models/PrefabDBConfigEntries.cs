using BepInEx.Configuration;
using Jotunn.Configs;
using MVBP.Configs;
using System;

namespace MVBP.Models
{
    internal class PrefabDBConfigEntries
    {
        internal ConfigEntry<bool> enabled;
        internal ConfigEntry<bool> allowedInDungeons;
        internal ConfigEntry<string> category;
        internal ConfigEntry<string> craftingStation;
        internal ConfigEntry<string> requirements;
        internal ConfigEntry<bool> placementPatch;
        internal ConfigEntry<bool> clipEverything;
        internal ConfigEntry<bool> clipGround;

        /// <summary>
        ///     Bool indicating if placement patch should be applied.
        ///     If internal value is null then setting is hidden from users and should always be applied.
        /// </summary>
        internal bool ApplyPlacementPatch => placementPatch == null || placementPatch.Value;

        /// <summary>
        ///     Create a PrefabDBConfig and all required config entries based on the
        ///     default values provided by the PrefabDB argument. Also checks against
        ///     the existing Config File when binding config entries.
        /// </summary>
        /// <param name="defaultVals"></param>
        public PrefabDBConfigEntries(PrefabDB defaultVals)
        {
            string sectionName = defaultVals.name;

            enabled = ConfigManager.BindConfig(
                sectionName,
                "Enabled",
                defaultVals.enabled,
                "If true then allow this prefab to be built and deconstructed. " +
                "Note: this setting is ignored if ForceAllPrefabs is true. " +
                "It is also ignored if the piece category is CreatorShop or Nature " +
                "and CreativeMode is false.",
                order: 10
            );

            allowedInDungeons = ConfigManager.BindConfig(
                sectionName,
                "AllowedInDungeons",
                defaultVals.allowedInDungeons,
                "If true then this prefab can be built inside dungeon zones."
            );

            category = ConfigManager.BindConfig(
                sectionName,
                "Category",
                defaultVals.category,
                "A string defining the tab the prefab shows up on in the hammer build table.",
                HammerCategories.GetAcceptableValueList()
            );

            craftingStation = ConfigManager.BindConfig(
                sectionName,
                "CraftingStation",
                defaultVals.craftingStation,
                "A string defining the crafting station required to built the prefab.",
                CraftingStations.GetAcceptableValueList()
            );

            requirements = ConfigManager.BindConfig(
                sectionName,
                "Requirements",
                defaultVals.requirements,
                "Resources required to build the prefab. Formatted as: itemID,amount;itemID,amount where itemID is the in-game identifier for the resource and amount is an integer.",
                acceptVals: new AcceptableValueConfigNote("You must use valid spawn item codes."),
                drawer: true
            );

            // if the prefab is not already set to use the placement patch by default
            // then add a config option to enable the placement collision patch.
            if (!defaultVals.placementPatch)
            {
                placementPatch = ConfigManager.BindConfig(
                    sectionName,
                    "PlacementPatch",
                    false,
                    "Set to true to enable collision patching during placement of the piece. " +
                    "Recommended to try this if the piece is not appearing when you go to place it.\n" +
                    "(If this setting fixes the issue please let me know via Github or Discord so I can change the default settings.)"
                );
            }

            if (!defaultVals.clipEverything)
            {
                clipEverything = ConfigManager.BindConfig(
                    sectionName,
                    "ClipEverything",
                    false,
                    "Set to true to allow piece to clip through everything during placement. Recommended to try this if the piece is not appearing when you go to place it.\n" +
                    "(If this setting fixes the issue please let me know via Github or Discord so I can change the default settings.)"
                );
            }

            if (!defaultVals.clipGround)
            {
                clipGround = ConfigManager.BindConfig(
                    sectionName,
                    "ClipGround",
                    false,
                    "Set to true to allow piece to clip through ground during placement.Recommended to try this if the piece is not floating when you try to place it.\n" +
                    "(If this setting fixes the issue please let me know via Github or Discord so I can change the default settings.)"
                );
            }
        }

        /// <summary>
        ///     Casts config data into the PrefabDB class format.
        /// </summary>
        /// <returns></returns>
        public PrefabDB AsPrefabDB()
        {
            try
            {
                // Get default prefabDB if it exists or make a new blank prefabDB
                string prefabName = enabled.Definition.Section;
                PrefabDB prefabDB = PrefabDefaults.GetDefaultPrefabDB(prefabName);
                prefabDB.Update(this);
                return prefabDB;
            }
            catch (Exception ex)
            {
                Log.LogError($"Invalid cast from PrefabDBConfig to PrefabDB: {ex}");
                return null;
            }
        }
    }
}
