using System.Reflection;
using BepInEx;
using HarmonyLib;
using Jotunn.Managers;


namespace MoreVanillaBuildPrefabs
{
    [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
    [BepInDependency(Jotunn.Main.ModGuid, Jotunn.Main.Version)]
    public class Plugin : BaseUnityPlugin
    {
        public const string PluginName = "MoreVanillaPrefabBuilds";
        internal const string Author = "Searica";
        public const string PluginGuid = $"{Author}.Valheim.{PluginName}";
        public const string PluginVersion = "0.1.4";

        Harmony _harmony;      

        public static bool DisableDestructionDrops { get; set; } = false;

        public void Awake()
        {
            Log.Init(Logger);

            PluginConfig.Init(Config);
            PluginConfig.SetUpConfig();

            PrefabManager.OnPrefabsRegistered += Hammer.AddHammerCategories;
            PrefabManager.OnPrefabsRegistered += PrefabHelper.FindPrefabs;
            PrefabManager.OnPrefabsRegistered += PrefabHelper.AddCustomPieces;

            _harmony = Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), harmonyInstanceId: PluginGuid);
        }

        public void OnDestroy()
        {
            PluginConfig.Save();
            _harmony?.UnpatchSelf();
        }

        public static bool IsCreatorShopPiece(Piece piece)
        {
            //if (PrefabHelper.AddedPieces.Contains(piece.m_name))
            if (PrefabHelper.AddedPrefabs.Contains(piece.gameObject.name))
            {
                if (piece.m_category == Hammer.HammerCategories.CreatorShop)
                {
                    return true;
                }
            }
            return false;
        }
    }
}