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
        public const string PluginVersion = "0.0.3";

        Harmony _harmony;

        public static class HammerCategories
        {
            public static Piece.PieceCategory Misc;
            public static Piece.PieceCategory Crafting;
            public static Piece.PieceCategory Building;
            public static Piece.PieceCategory Furniture;
            public static Piece.PieceCategory CreatorShop;
        }

        public static class HammerCategoryNames
        {
            public static string Misc = "Misc";
            public static string Crafting = "Crafting";
            public static string Building = "Building";
            public static string Furniture = "Furniture";
            public static string CreatorShop = "CreatorShop";
        }

        public static bool _debug = false;

        public static bool DisableDestructionDrops { get; set; } = false;

        public void Awake()
        {
            Log.Init(Logger);
            PluginConfig.Init(Config);
            _harmony = Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), harmonyInstanceId: PluginGuid);
        }

        public void OnDestroy()
        {
            _harmony?.UnpatchSelf();
        }

        public static void AddHammerCategories()
        {
            HammerCategories.Misc = PieceManager.Instance.AddPieceCategory("_HammerPieceTable", HammerCategoryNames.Misc);
            HammerCategories.Crafting = PieceManager.Instance.AddPieceCategory("_HammerPieceTable", HammerCategoryNames.Crafting);
            HammerCategories.Furniture = PieceManager.Instance.AddPieceCategory("_HammerPieceTable", HammerCategoryNames.Furniture);
            HammerCategories.Building = PieceManager.Instance.AddPieceCategory("_HammerPieceTable", HammerCategoryNames.Building);
            HammerCategories.CreatorShop = PieceManager.Instance.AddPieceCategory("_HammerPieceTable", HammerCategoryNames.CreatorShop);
        }

        public static bool IsCreatorShopPiece(Piece piece)
        {
            if (PrefabAdder.AddedPieces.ContainsKey(piece.m_name))
            {
                if (piece.m_category == HammerCategories.CreatorShop)
                {
                    return true;
                }
            }
            return false;
        }
    }
}