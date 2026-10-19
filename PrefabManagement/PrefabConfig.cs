using System.Collections.Generic;
using UnityEngine;
using BepInEx.Configuration;
using Jotunn.Configs;
using Jotunn.Extensions;
using Configs;
using static Configs.ReqConfigDrawer;
using MVBP.PieceManagement;



namespace MVBP.PrefabManagement;

internal class PrefabConfig
{
    private const char amountSeperator = ',';
    private const char reqSeperator = ';';
    private readonly RequirementsParser reqParser = new(amountSeperator, reqSeperator);

    public string Name;
    private readonly bool _enabled;
    private readonly bool _allowedInDungeons;
    private readonly string _category;
    private readonly string _craftingStation;
    private readonly string _requirements;
    private readonly bool _clipEverything;
    private readonly bool _clipGround;
    private readonly bool _placementPatch;
    private readonly bool _offsetPatch;
    private readonly string _pieceTable;
    private readonly Vector3 _placementOffset;
    
    public string PieceName;
    public string PieceDesc;
    public PieceClassification PieceGroup;
    public bool PlayerBasePatch;
    public string SpawnOnDestroyed;
    public uint? InvWidth;
    public uint? InvHeight;
    public readonly Piece.ComfortGroup? _comfortGroup;

    /// <summary>
    ///     Whether BindToConfig has been called for this PrefabConfig
    ///     and the values for the Prefab and Piece properties are set.
    /// </summary>
    public bool IsBound { get; private set; } = false;
    public GameObject Prefab { get; private set; }
    public Piece Piece { get; private set; }

    public ConfigEntry<bool> Enabled { get; private set; }
    public ConfigEntry<bool> AllowedInDungeons { get; private set; }
    public ConfigEntry<string> PieceTable { get; private set; }
    public ConfigEntry<string> Category { get; private set; }
    public ConfigEntry<string> CraftingStation { get; private set; }
    public ConfigEntry<string> Requirements { get; private set; }
    public ConfigEntry<bool> PlacementPatch { get; private set; }
    public ConfigEntry<bool> ClipEverything { get; private set; }
    public ConfigEntry<bool> ClipGround { get; private set; }
    public ConfigEntry<bool> OffsetPatch { get; private set; }
    public ConfigEntry<Vector3> PlacementOffset { get; private set; }

 

    public PrefabConfig(
       string name,
       bool enabled = false,
       bool allowedInDungeons = false,
       string pieceTable = "Hammer",
       string category = PieceCategoryManager.CreatorShop,
       string craftingStation = nameof(CraftingStations.None),
       string requirements = "",
       bool clipEverything = false,
       bool clipGround = false,
       bool placementPatch = false,
       bool offsetPatch = false,
       Vector3 placementOffset = default(Vector3),
       string pieceName = null,
       string pieceDesc = null,
       PieceClassification pieceGroup = default,
       bool playerBasePatch = false,
       string spawnOnDestroyed = null,
       uint? invWidth = null,
       uint? invHeight = null,
       Piece.ComfortGroup? comfortGroup = null
    )
    {
        Name = name;
        
        PieceName = pieceName;
        PieceDesc = pieceDesc;
        PieceGroup = pieceGroup;
        PlayerBasePatch = playerBasePatch;
        SpawnOnDestroyed = spawnOnDestroyed;
        InvWidth = invWidth;
        InvHeight = invHeight;

        // Set default internal values for config entry settings
        _enabled = enabled;
        _placementOffset = placementOffset;
        _allowedInDungeons = allowedInDungeons;
        _pieceTable = pieceTable;
        _category = category;
        _craftingStation = craftingStation;
        _requirements = string.IsNullOrWhiteSpace(requirements) ? reqParser.GetPlaceholderReqString() : requirements;
        _clipEverything = clipEverything;
        _clipGround = clipGround;
        _placementPatch = placementPatch;
        _offsetPatch = offsetPatch;
        _comfortGroup = comfortGroup;
    }
    
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

        PieceTable = configFile.BindConfigInOrder(
            sectionName,
            "PieceTable",
            this._pieceTable,
            "The piece table that this prefab should be added to.",
            synced: true,
            sectionOrder: false,
            settingOrder: true,
            acceptableValues: PieceTables.GetAcceptableValueList()
        );

        Category = configFile.BindConfigInOrder(
            sectionName,
            "Category",
            this._category,
            "A string defining the tab the prefab shows up on in the hammer build table.",
            acceptableValues: PieceCategoryManager.GetAcceptableValueList(),
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
            customDrawer: ReqConfigDrawer.ReqConfigCustomDrawer(amountSeperator, reqSeperator),
            synced: true,
            sectionOrder: false,
            settingOrder: true
        );

        // If offset patch is enabled by default then make this config readonly.
        OffsetPatch = configFile.BindConfigInOrder(
            sectionName,
            "OffsetPatch",
            this._offsetPatch,
            "Set to true to enable automatic patching to adjust the center location of the piece during placement." +
            "Recommended to try this first if the piece is not appearing when you go to place it.\n" +
            "(If this setting fixes the issue please let me know via Github or Discord so I can change the default settings.)",
            synced: true,
            sectionOrder: false,
            settingOrder: true,
            configAttributes: new ConfigurationManagerAttributes() { ReadOnly = this._offsetPatch }
        );


        // If placement patch is enabled by default then make this config readonly.
        PlacementPatch = configFile.BindConfigInOrder(
            sectionName,
            "PlacementPatch",
            this._placementPatch,
            "Set to true to enable collision patching during placement of the piece. " +
            "Recommended to try this if the piece is not appearing when you go to place it against a wall or structure other than the ground.\n" +
            "(If this setting fixes the issue please let me know via Github or Discord so I can change the default settings.)",
            synced: true,
            sectionOrder: false,
            settingOrder: true,
            configAttributes: new ConfigurationManagerAttributes() { ReadOnly = this._placementPatch }
        );

        // If clip ground is enabled by default then make this config readonly.
        ClipGround = configFile.BindConfigInOrder(
            sectionName,
            "ClipGround",
            this._clipGround,
            "Set to true to allow piece to clip through ground during placement. Recommended to try this if the piece is not appearing or it is floating above the ground when you try to place it on the ground.\n" +
            "(If this setting fixes the issue please let me know via Github or Discord so I can change the default settings.)",
            synced: true,
            sectionOrder: false,
            settingOrder: true,
            configAttributes: new ConfigurationManagerAttributes() { ReadOnly = this._clipGround }
        );


        // If clip everything is enabled by default then make this config readonly.
        ClipEverything = configFile.BindConfigInOrder(
            sectionName,
            "ClipEverything",
            this._clipEverything,
            "Set to true to allow piece to clip through everything during placement. Recommended to try this if the piece is not appearing at all when you go to place it and you have tried everything else.\n" +
            "(If this setting fixes the issue please let me know via Github or Discord so I can change the default settings.)",
            synced: true,
            sectionOrder: false,
            settingOrder: true,
            configAttributes: new ConfigurationManagerAttributes() { ReadOnly = this._clipEverything }
        );

 
        // If PlacementOffset is non-zero then make this config readonly.
        PlacementOffset = configFile.BindConfigInOrder(
            sectionName,
            "PlacementOffset",
            this._placementOffset,
            "Configures the offset applied to a prefab during placement. This is used to customize the location of the prefab relative to where you are trying to place it. Used to fix prefabs with strange offsets, but try OffsetPatch first.\n" +
            "(If this setting fixes the issue please let me know via Github or Discord so I can change the default settings.)",
            synced: true,
            sectionOrder: false,
            settingOrder: true,
            configAttributes: new ConfigurationManagerAttributes() { 
                IsAdvanced = true,
                ReadOnly = this._placementOffset != Vector3.zero
            }
        );
        

        this.IsBound = true;
    }

    /// <summary>
    ///     Deserialize requirements config entry.
    /// </summary>
    /// <returns></returns>
    public List<RequirementConfig> ReadRequirements()
    {
        return this.reqParser.Deserialize(this.Requirements.Value);
    }
}
