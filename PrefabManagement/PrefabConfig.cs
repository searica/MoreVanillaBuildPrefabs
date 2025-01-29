using BepInEx.Configuration;
using Jotunn.Configs;
using Jotunn.Extensions;
using UnityEngine;
using MVBP.Models;
using System;
using Configs;
using Logging;

namespace MVBP.PrefabManagement;

internal class PrefabConfig(
    string name,
    bool enabled = false,
    bool allowedInDungeons = false,
    string category = HammerCategories.CreatorShop,
    string craftingStation = nameof(CraftingStations.None),
    string requirements = null,
    bool clipEverything = false,
    bool clipGround = false,
    bool placementPatch = false,
    Vector3? placementOffset = null,
    string pieceName = null,
    string pieceDesc = null,
    PieceGroup pieceGroup = default,
    bool playerBasePatch = false,
    string spawnOnDestroyed = null,
    uint? invWidth = null,
    uint? invHeight = null
    )
{
    public string Name = name;
    private readonly bool _enabled = enabled;
    private readonly bool _allowedInDungeons = allowedInDungeons;
    private readonly string _category = category;
    private readonly string _craftingStation = craftingStation;
    private readonly string _requirements = requirements;
    private readonly bool _clipEverything = clipEverything;
    private readonly bool _clipGround = clipGround;
    private readonly bool _placementPatch = placementPatch;

    public Vector3? PlacementOffset = placementOffset;
    public string PieceName = pieceName;
    public string PieceDesc = pieceDesc;
    public PieceGroup PieceGroup = pieceGroup;
    public bool PlayerBasePatch = playerBasePatch;
    public string SpawnOnDestroyed = spawnOnDestroyed;
    public uint? InvWidth = invWidth;
    public uint? InvHeight = invHeight;

    /// <summary>
    ///     Whether BindToConfig has been called for this PrefabConfig
    ///     and the values for the Prefab and Piece properties are set.
    /// </summary>
    public bool IsBound { get; private set; } = false;
    public GameObject Prefab { get; private set; }
    public Piece Piece { get; private set; }

    public ConfigEntry<bool> Enabled { get; private set; }
    public ConfigEntry<bool> AllowedInDungeons { get; private set; }
    public ConfigEntry<string> Category { get; private set; }
    public ConfigEntry<string> CraftingStation { get; private set; }
    public ConfigEntry<string> Requirements { get; private set; }
    public ConfigEntry<bool> PlacementPatch { get; private set; }
    public ConfigEntry<bool> ClipEverything { get; private set; }
    public ConfigEntry<bool> ClipGround { get; private set; }

    internal bool UpdatePieceSettings { get; set; } = false;
    internal bool UpdatePlacementSettings { get; set; } = false;

    public void BindToConfig(ConfigFile configFile, GameObject prefab, Piece piece)
    {
        this.Prefab = prefab;
        this.Piece = piece;

        string sectionName = this.Name;

        Enabled = configFile.BindConfigInOrder(
            sectionName,
            "Enabled",
            this._enabled,
            "If true then allow this prefab to be built and deconstructed. " +
            "Note: this setting is ignored if ForceAllPrefabs is true. " +
            "It is also ignored if the piece category is CreatorShop or Nature " +
            "and CreativeMode is false.",
            synced: true,
            sectionOrder: false,
            settingOrder: true
        );

        AllowedInDungeons = configFile.BindConfigInOrder(
            sectionName,
            "AllowedInDungeons",
            this._allowedInDungeons,
            "If true then this prefab can be built inside dungeon zones.",
            synced: true,
            sectionOrder: false,
            settingOrder: true
        );

        Category = configFile.BindConfigInOrder(
            sectionName,
            "Category",
            this._category,
            "A string defining the tab the prefab shows up on in the hammer build table.",
            acceptableValues: HammerCategories.GetAcceptableValueList(),
            synced: true,
            sectionOrder: false,
            settingOrder: true
        );

        CraftingStation = configFile.BindConfigInOrder(
            sectionName,
            "CraftingStation",
            this._craftingStation,
            "A string defining the crafting station required to built the prefab.",
            acceptableValues: CraftingStations.GetAcceptableValueList(),
            synced: true,
            sectionOrder: false,
            settingOrder: true
        );

        Requirements = configFile.BindConfigInOrder(
            sectionName,
            "Requirements",
            this._requirements,
            "Resources required to build the prefab. Formatted as: itemID,amount;itemID,amount where itemID is the in-game identifier for the resource and amount is an integer.",
            acceptableValues: new ReqConfigDrawer.AcceptableValueReqConfigNote("You must use valid spawn item codes."),
            customDrawer: ReqConfigDrawer.ReqConfigCustomDrawer(amountSep: ',', reqSep: ';'),
            synced: true,
            sectionOrder: false,
            settingOrder: true
        );

        // If placement patch is enabled by default then make this config readonly.
        PlacementPatch = configFile.BindConfigInOrder(
            sectionName,
            "PlacementPatch",
            this._placementPatch,
            "Set to true to enable collision patching during placement of the piece. " +
            "Recommended to try this if the piece is not appearing when you go to place it.\n" +
            "(If this setting fixes the issue please let me know via Github or Discord so I can change the default settings.)",
            synced: true,
            sectionOrder: false,
            settingOrder: true,
            configAttributes: new ConfigurationManagerAttributes() { ReadOnly = this._placementPatch }
        );

        // If clip everything is enabled by default then make this config readonly.
        ClipEverything = configFile.BindConfigInOrder(
            sectionName,
            "ClipEverything",
            this._clipEverything,
            "Set to true to allow piece to clip through everything during placement. Recommended to try this if the piece is not appearing when you go to place it.\n" +
            "(If this setting fixes the issue please let me know via Github or Discord so I can change the default settings.)",
            synced: true,
            sectionOrder: false,
            settingOrder: true,
            configAttributes: new ConfigurationManagerAttributes() { ReadOnly = this._clipEverything }
        );

        // If clip clip is enabled by default then make this config readonly.
        ClipGround = configFile.BindConfigInOrder(
            sectionName,
            "ClipGround",
            this._clipGround,
            "Set to true to allow piece to clip through ground during placement.Recommended to try this if the piece is not floating when you try to place it.\n" +
            "(If this setting fixes the issue please let me know via Github or Discord so I can change the default settings.)",
            synced: true,
            sectionOrder: false,
            settingOrder: true,
            configAttributes: new ConfigurationManagerAttributes() { ReadOnly = this._clipGround }
        );

        this.Enabled.SettingChanged += PieceSettingChanged;
        this.AllowedInDungeons.SettingChanged += PieceSettingChanged;
        this.Category.SettingChanged += PieceSettingChanged;
        this.CraftingStation.SettingChanged += PieceSettingChanged;
        this.Requirements.SettingChanged += PieceSettingChanged;
        this.ClipEverything.SettingChanged += PieceSettingChanged;
        this.ClipGround.SettingChanged += PieceSettingChanged;

        this.PlacementPatch.SettingChanged += PlacementSettingChanged;
        this.IsBound = true;
    }

    /// <summary>
    ///     Event hook to set whether a config entry
    ///     for a piece setting has been changed.
    /// </summary>
    private void PieceSettingChanged(object obj, EventArgs args)
    {
        if (!UpdatePieceSettings)
        {
            UpdatePieceSettings = true;
        }
    }

    /// <summary>
    ///     Event hook to set whether a config entry
    ///     for placement patches has been changed.
    /// </summary>
    private void PlacementSettingChanged(object obj, EventArgs args)
    {
        if (!UpdatePlacementSettings)
        {
            UpdatePlacementSettings = true;
        }
    }
}
