// Ignore Spelling: MVBP

using HarmonyLib;
using MVBP.Configs;
using MVBP.Helpers;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace MVBP
{
    [HarmonyPatch(typeof(Piece))]
    internal class PiecePatch
    {
        /// <summary>
        ///     Applies patches from PatchPlayerBuildPieceIfNeeded
        ///     when pieces are loaded in.
        /// </summary>
        /// <param name="__instance"></param>
        [HarmonyPostfix]
        [HarmonyPatch(nameof(Piece.Awake))]
        private static void PieceAwakePostfix(Piece __instance)
        {
            PrefabPatcher.PatchPlayerBuiltPieceIfNeed(__instance);
        }

        /// <summary>
        ///     Applies patches from PatchPlayerBuildPieceIfNeeded
        ///     when pieces are loaded are placed.
        /// </summary>
        /// <param name="__instance"></param>
        [HarmonyPostfix]
        [HarmonyPatch(nameof(Piece.SetCreator))]
        private static void PieceSetCreatorPostfix(Piece __instance)
        {
            PrefabPatcher.PatchPlayerBuiltPieceIfNeed(__instance);
        }

        /// <summary>
        ///     Called when just before piece is placed to synchronize the
        ///     positions and rotations of otherwise non-persistent objects
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="__instance"></param>
        [HarmonyPrefix]
        [HarmonyPatch(nameof(Piece.SetCreator))]
        private static void PieceSetCreatorPrefix(Piece __instance)
        {
            var view = __instance.GetComponent<ZNetView>();
            if (view && !view.m_persistent)
            {
                view.m_persistent = true;

                var sync = __instance.gameObject.GetComponent<ZSyncTransform>();
                if (sync == null)
                {
                    __instance.gameObject.AddComponent<ZSyncTransform>();
                }
                sync.m_syncPosition = true;
                sync.m_syncRotation = true;
            }
        }

        /// <summary>
        ///     Transpiler to set dropped resources to default resources
        ///     for any piece altered by this mod if the piece was not built
        ///     by a player. Also picks pickables (if not already picked) and
        ///     forces ItemStand to drop attached item (if it has one).
        /// </summary>
        [HarmonyTranspiler]
        [HarmonyPatch(nameof(Piece.DropResources))]
        private static IEnumerable<CodeInstruction> DropResourcesTranspiler(
            IEnumerable<CodeInstruction> instructions
        )
        {
            /* Target this IL code to be able to edit the resources that get dropped
             * // Requirement[] resources = m_resources;
             * IL_0011: ldarg.0
             * IL_0012: ldfld class Piece/Requirement[] Piece::m_resources
             * IL_0017: stloc.1
             * // (no C# code)
             * IL_0018: ldc.i4.0
             * IL_0019: stloc.2
             * // 	foreach (Requirement requirement in resources)
             */
            return new CodeMatcher(instructions)
                .MatchForward(
                    useEnd: false,
                    new CodeMatch(
                        OpCodes.Ldfld,
                        AccessTools.Field(typeof(Piece), nameof(Piece.m_resources))),
                    new CodeMatch(OpCodes.Stloc_1)
                )
                .SetInstructionAndAdvance(Transpilers.EmitDelegate(DropResources_m_resources_Delegate))
                .InstructionEnumeration();
        }

        /// <summary>
        ///     Delegate that sets dropped resources to default resources
        ///     for any piece altered by this mod if the piece was not built
        ///     by a player and adjusts dropped resources for pickable items.
        ///     Also picks pickables (if not already picked) and
        ///     forces ItemStand to drop attached item (if it has one).
        /// </summary>
        /// <param name="piece"></param>
        private static Piece.Requirement[] DropResources_m_resources_Delegate(Piece piece)
        {
            if (!InitManager.IsPatchedByMod(piece))
            {
                // do nothing if not a piece the mod changes
                return piece.m_resources;
            }

            // Only interact if it is a piece added by this mod or
            // the prefab has previously had it's resources altered by the mod
            if (ConfigManager.IsVerbosityMedium)
            {
                Log.LogInfo("Dropping resources for MVBP piece");
            }

            // Set resources to defaults is piece is not placed by player (world-generated pieces)
            if (!piece.IsPlacedByPlayer() && InitManager.TryGetDefaultPieceClone(piece.gameObject, out Piece pieceClone))
            {
                if (pieceClone.m_resources != null) { return pieceClone.m_resources; }
            }

            // Set resources to current piece resources if placed by a player
            var resources = piece.m_resources;

            var zNetView = piece?.gameObject?.GetComponent<ZNetView>();
            if (zNetView == null || piece.gameObject == null) { return resources; }

            // If piece has an ItemStand and it has an item, then drop it.
            if (piece.gameObject.TryGetComponent(out ItemStand itemStand))
            {
                var canBeRemoved = itemStand.m_canBeRemoved;
                itemStand.m_canBeRemoved = true;
                zNetView.InvokeRPC("DropItem");
                itemStand.m_canBeRemoved = canBeRemoved;
            }

            // If piece is pickable and it has not been picked, then pick it.
            if (piece.gameObject.TryGetComponent(out Pickable pickable))
            {
                zNetView.InvokeRPC("Pick");
                // Adjust drops to avoid duplicating pickable item (avoid infinite resource exploits).
                resources = RequirementsHelper.RemovePickableFromRequirements(resources, pickable);
            }

            // If piece has MineRock5 then adjust dropped resources
            if (piece.gameObject.TryGetComponent(out MineRock5 mineRock5))
            {
                resources = RequirementsHelper.RemoveMineRock5DropsFromRequirements(resources, mineRock5);
            }

            // If piece has MineRock then adjust dropped resources
            if (piece.gameObject.TryGetComponent(out MineRock mineRock))
            {
                resources = RequirementsHelper.RemoveMineRockDropsFromRequirements(resources, mineRock);
            }

            return resources;
        }
    }
}