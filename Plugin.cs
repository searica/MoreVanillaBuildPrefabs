using System.Reflection;
using BepInEx;
using HarmonyLib;
using Jotunn.Managers;
using Jotunn.Utils;

using MoreVanillaBuildPrefabs.Configs;
using MoreVanillaBuildPrefabs.Logging;



namespace MoreVanillaBuildPrefabs
{
    [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
    [BepInDependency(Jotunn.Main.ModGuid, Jotunn.Main.Version)]
    [NetworkCompatibility(CompatibilityLevel.VersionCheckOnly, VersionStrictness.Patch)]
    public class Plugin : BaseUnityPlugin
    {
        public const string PluginName = "MoreVanillaBuildPrefabs";
        internal const string Author = "Searica";
        public const string PluginGuid = $"{Author}.Valheim.{PluginName}";
        public const string PluginVersion = "0.3.7";

        Harmony _harmony;

        internal static bool DisableDestructionDrops { get; set; } = false;

        public void Awake()
        {
            Log.Init(Logger);

            PluginConfig.Init(Config);
            PluginConfig.SetUpConfig();

            _harmony = Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), harmonyInstanceId: PluginGuid);

            PluginConfig.SetupWatcher();

            SynchronizationManager.OnConfigurationSynchronized += (obj, attr) =>
            {
                if (attr.InitialSynchronization)
                {
                    Log.LogInfo("Initial Config sync event received");
                    PieceAdder.AddPieces();
                }
                else
                {
                    Log.LogInfo("Config sync event received");
                }
            };
        }

        public void OnDestroy()
        {
            PluginConfig.Save();
            _harmony?.UnpatchSelf();
        }
    }
}