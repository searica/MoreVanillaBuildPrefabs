// Ignore Spelling: Plugin MVBP
using System.Reflection;
using BepInEx;
using BepInEx.Configuration;
using HarmonyLib;
using Jotunn.Managers;
using Jotunn.Utils;
using Jotunn.Extensions;
using Logging;
using Configs;

namespace MVBP;

[BepInPlugin(PluginGUID, PluginName, PluginVersion)]
[BepInDependency(Jotunn.Main.ModGuid, Jotunn.Main.Version)]
[BepInDependency(ModCompat.ExtraSnapsGUID, BepInDependency.DependencyFlags.SoftDependency)]
[BepInDependency(ModCompat.PlanBuildGUID, BepInDependency.DependencyFlags.SoftDependency)]
[NetworkCompatibility(CompatibilityLevel.VersionCheckOnly, VersionStrictness.Patch)]
[SynchronizationMode(AdminOnlyStrictness.IfOnServer)]
public class MorePrefabs : BaseUnityPlugin
{
    public const string PluginName = "MoreVanillaBuildPrefabs";
    internal const string Author = "Searica";
    public const string PluginGUID = $"{Author}.Valheim.{PluginName}";
    public const string PluginVersion = "1.4.2";

    public static MorePrefabs Instance { get; private set; }
    private ConfigFileWatcher configFileWatcher;

    #region Global Settings
    private const string MainSection = "Global";
    private static ConfigEntry<bool> CreativeMode { get; set; }
    private static ConfigEntry<bool> ForceAllPrefabs { get; set; }
    internal static bool IsCreativeMode => CreativeMode.Value;
    internal static bool IsForceAllPrefabs => ForceAllPrefabs.Value;
    #endregion Global Settings

    #region Admin Settings
    private const string AdminSection = "Admin";
    private static ConfigEntry<bool> CreatorShopAdminOnly { get; set; }
    private static ConfigEntry<bool> AdminDeconstructOtherPlayers { get; set; }
    internal static bool IsCreatorShopAdminOnly => CreatorShopAdminOnly.Value;
    internal static bool IsAdminDeconstructOtherPlayers => AdminDeconstructOtherPlayers.Value;
    #endregion Admin Settings

    #region Customization Settings
    private const string CustomizationSection = "Customization";
    private static ConfigEntry<bool> EnableHammerCrops { get; set; }
    private static ConfigEntry<bool> EnableComfortPatches { get; set; }
    private static ConfigEntry<bool> EnableSeasonalPieces { get; set; }
    private static ConfigEntry<bool> EnablePlayerBasePatches { get; set; }
    internal static bool IsEnableHammerCrops => EnableHammerCrops.Value;
    internal static bool IsEnableComfortPatches => EnableComfortPatches.Value;
    internal static bool IsEnableSeasonalPieces => EnableSeasonalPieces.Value;
    internal static bool IsEnablePlayerBasePatches => EnablePlayerBasePatches.Value;
 
    #endregion Customization Settings

    #region Texture Patches
    private const string TextureSection = "Textures";
    private static ConfigEntry<bool> PortalTexture;
    private static ConfigEntry<bool> DvergrWoodTexture;
    internal static bool PatchPortalTexture => PortalTexture.Value;
    internal static bool PatchDvergrWoodTexture => DvergrWoodTexture.Value;
    #endregion Texture Patches

    #region Unsafe Patches
    private const string UnsafeSection = "Unsafe Patches";
    private static ConfigEntry<bool> EnableBedPatches { get; set; }
    private static ConfigEntry<bool> EnableFermenterPatches { get; set; }
    internal static bool IsEnableBedPatches => EnableBedPatches.Value;
    internal static bool IsEnableFermenterPatches => EnableFermenterPatches.Value;
    #endregion Unsafe Patches


    public void Awake()
    {
        Instance = this;
        Log.Init(Logger);

        Config.SaveOnConfigSet = false;
        SetUpConfigEntries();
        Config.Save();

        Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), harmonyInstanceId: PluginGUID);

        Game.isModded = true;

        // Re-initialization after reloading config and don't save since file was just reloaded
        configFileWatcher = new(Config);
        configFileWatcher.OnConfigFileReloaded += () =>
        {
            UpdateMananger.UpdatePlugin("Configuration file changed, re-initializing", saveConfig: false);
        };

        // Re-initialize after changing config data in-game and trigger a save to disk.
        SynchronizationManager.OnConfigurationWindowClosed += () =>
        {
            UpdateMananger.UpdatePlugin("Configuration changed in-game, re-initializing");
        };

        // Re-initialize after getting updated config data and trigger a save to disk.
        SynchronizationManager.OnConfigurationSynchronized += (obj, attr) =>
        {
            UpdateMananger.UpdatePlugin("Configuration synced, re-initializing");
        };
    }

    public void OnDestroy()
    {
        Config.Save();
    }

    private void SetUpConfigEntries()
    {
        CreativeMode = Config.BindConfigInOrder(
            MainSection,
            "CreativeMode",
            false,
            "Set to true/enabled to enable pieces from the CreatorShop or Nature piece categories. " +
            "By default, pieces set to those categories are not standard build pieces."
        );

        ForceAllPrefabs = Config.BindConfigInOrder(
            MainSection,
            "ForceAllPrefabs",
            false,
            "If true/enabled, adds all prefabs to the hammer for building. Unless CreativeMode is " +
            "also enabled it will not add pieces set to the CreatorShop or Nature category though."
        );

        Log.Verbosity = Config.BindConfigInOrder(
            MainSection,
            "Verbosity",
            Log.InfoLevel.Low,
            "Low will log basic information about the mod. Medium will log information that " +
            "is useful for troubleshooting. High will log a lot of information, do not set " +
            "it to this without good reason as it will slow down your game.",
            synced: false
        );

        CreatorShopAdminOnly = Config.BindConfigInOrder(
            AdminSection,
            "CreatorShopAdminOnly",
            false,
            "Set to true/enabled to restrict placement and deconstruction of CreatorShop pieces " +
            "to players with Admin status."
        );

        AdminDeconstructOtherPlayers = Config.BindConfigInOrder(
            AdminSection,
            "AdminDeconstructOtherPlayers",
            true,
            "Set to true/enabled to allow admin players to deconstruct any pieces built by other players," +
            " even if doing so would normally be prevented (such as for CreatorShop or Nature pieces)." +
            " Intended to prevent griefing via placement of indestructible objects."
        );

        // Customization settings
        EnableHammerCrops = Config.BindConfigInOrder(
            CustomizationSection,
            "HammerCrops",
            false,
            "Set to true/enabled to enable placing vanilla crops with the hammer." +
            " Unless this setting is true Vanilla crops will not be available for placing with the hammer."
        );

        EnableComfortPatches = Config.BindConfigInOrder(
            CustomizationSection,
            "ComfortPatches (Requires Restart)",
            true,
            "Set to true/enabled to patch new pieces to have comfort values like their vanilla counterparts."
        );

        EnablePlayerBasePatches = Config.BindConfigInOrder(
            CustomizationSection,
            "PlayerBasePatches (Requires Restart)",
            true,
            "Set to true/enabled to patch player-built instances of new torches, fires, " +
            "and beds so they suppress monster spawning just like their vanilla counterparts."
        );

        EnableSeasonalPieces = Config.BindConfigInOrder(
            CustomizationSection,
            "SeasonalPieces",
            true,
            "Set to true/enabled to add all currently disabled seasonal pieces to the hammer build table."
        );

        // Texture Section
        PortalTexture = Config.BindConfigInOrder(
            TextureSection,
            "PortalTexturePatch (Requires Restart)",
            false,
            "Set to true/enabled to change the texture of the new portal to appear " +
            "as if it was created by those who dwell in the Mistlands. " +
            "\nNote: change in appearance will not work for users without this mod."
        );

        DvergrWoodTexture = Config.BindConfigInOrder(
            TextureSection,
            "DvergrWoodPatch (Requires Restart)",
            false,
            "Set to true/enabled to change the texture of the player built instances of " +
            "of Dvergr wood floors and stairs to appear as if they were brand new. " +
            "\nNote: change in appearance will not work for users without this mod."
        );

        // Unsafe Section
        EnableBedPatches = Config.BindConfigInOrder(
            UnsafeSection,
            "BedPatches (Requires Restart, Unsafe)",
            false,
            "Set to true/enabled to patch player-built instances of new beds so you can sleep in them." +
            "\nWARNING: enabling this setting can result in you losing your spawn point" +
            " if had set your spawn using a patched bed and log in without this mod."
        );

        EnableFermenterPatches = Config.BindConfigInOrder(
            UnsafeSection,
            "FermenterPatches (Requires Restart, Unsafe)",
            false,
            "Set to true/enabled to patch player-built instances of fermenting barrels " +
            "to function as a fermenter that are 30% faster than the vanilla fermenter." +
            "\nWARNING: enabling this setting can result in you losing the mead base that " +
            "is fermenting if you load the area without this mod."
        );

        // Set up event hooks
        CreativeMode.SettingChanged += UpdateMananger.PieceSettingChanged;
        ForceAllPrefabs.SettingChanged += UpdateMananger.PieceSettingChanged;
        CreatorShopAdminOnly.SettingChanged += UpdateMananger.PieceSettingChanged;
        EnableHammerCrops.SettingChanged += UpdateMananger.PieceSettingChanged;

        AdminDeconstructOtherPlayers.SettingChanged += UpdateMananger.ModSettingChanged;
        Log.Verbosity.SettingChanged += UpdateMananger.ModSettingChanged;

        EnableSeasonalPieces.SettingChanged += UpdateMananger.SeasonalSettingChanged;
    }



    // Public API Section

    ///// <summary>
    /////     Checks if the root prefab of the GameObject has had a 
    /////     Piece component added to it by MVBP. So this method can also
    /////     be used on any clones of the root prefab.
    ///// </summary>
    ///// <param name="prefab">GameObject to check.</param>
    ///// <param name="piece">Optional piece component to prevent duplicate GetComponent calls.</param>
    ///// <returns>True if MVBP has added a Piece component, False otherwise.</returns>
    //public bool IsPieceAddedByMVBP(GameObject prefab, Piece piece = null)
    //{
    //    return ZNetPrefabManager.IsPieceAddedByMVBP(prefab, piece);
    //}


}
