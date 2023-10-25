using Jotunn.Configs;
using Jotunn.Managers;
using MoreVanillaBuildPrefabs.Configs;
using MoreVanillaBuildPrefabs.Logging;
using System.Collections.Generic;

namespace MoreVanillaBuildPrefabs.Helpers
{
    internal class PieceCategoryHelper
    {
        internal static Piece.PieceCategory Nature;
        internal static Piece.PieceCategory CreatorShop;

        internal static void AddCreatorShopPieceCategory()
        {
            if (PieceManager.Instance.GetPieceCategory(HammerCategories.Nature) == null
                || PieceManager.Instance.GetPieceCategory(HammerCategories.CreatorShop) == null)
            {
                if (PluginConfig.IsVerbosityMedium)
                {
                    Log.LogInfo("Adding custom piece categories");
                }
                Nature = PieceManager.Instance.AddPieceCategory(PieceTables.Hammer, HammerCategories.Nature);
                CreatorShop = PieceManager.Instance.AddPieceCategory(PieceTables.Hammer, HammerCategories.CreatorShop);
            }
        }

        internal static void RemoveCreatorShopPieceCategory()
        {
            if (PluginConfig.IsVerbosityMedium)
            {
                Log.LogInfo("Removing custom piece categories");
            }
            PieceManager.Instance.RemovePieceCategory(PieceTables.Hammer, HammerCategories.CreatorShop);
        }

        internal static bool IsCreativeModePiece(Piece piece)
        {
            return IsCreatorShopPiece(piece) || IsNaturePiece(piece);
        }

        internal static bool IsCreatorShopPiece(Piece piece)
        {
            var pieceName = NameHelper.GetPrefabName(piece);
            if (MoreVanillaBuildPrefabs.IsChangedByMod(pieceName))
            {
                if (piece.m_category == CreatorShop)
                {
                    return true;
                }
            }
            return false;
        }

        internal static bool IsNaturePiece(Piece piece)
        {
            var pieceName = NameHelper.GetPrefabName(piece);
            if (MoreVanillaBuildPrefabs.IsChangedByMod(pieceName))
            {
                if (piece.m_category == Nature)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        ///     Checks if prefab name is contained in HashSet of Vanilla crops
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        internal static bool IsVanillaCrop(string name)
        {
            return _VanillaCrops.Contains(name);
        }

        // Crops you can plant in Vanilla
        private static readonly HashSet<string> _VanillaCrops = new() {
            "Pickable_SeedCarrot",
            "Pickable_SeedTurnip",
            "Pickable_SeedOnion",
            "Pickable_Onion",
            "Pickable_Carrot",
            "Pickable_Turnip",
            "Pickable_Flax",
            "Pickable_Barley",
            "Pickable_Mushroom_Magecap",
            "Pickable_Mushroom_JotunPuffs",
        };
    }
}