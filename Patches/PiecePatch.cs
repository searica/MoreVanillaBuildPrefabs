using HarmonyLib;
using MoreVanillaBuildPrefabs.Configs;
using MoreVanillaBuildPrefabs.Helpers;
using MoreVanillaBuildPrefabs.Logging;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using static MoreVanillaBuildPrefabs.MoreVanillaBuildPrefabs;

namespace MoreVanillaBuildPrefabs
{
    [HarmonyPatch(typeof(Piece))]
    internal class PiecePatch
    {
        /// <summary>
        ///     Called when just before piece is placed to synchronize the
        ///     positions and rotations of otherwise non-persistent objects
        /// </summary>
        /// <param name="uid"></param>
        /// <param name="__instance"></param>
        [HarmonyPrefix]
        [HarmonyPatch(nameof(Piece.SetCreator))]
        private static void PieceSetCreatorPrefix(long uid, Piece __instance)
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
        /// <param name="__instance"></param>
        /// <param name="__state"></param>
        [HarmonyTranspiler]
        [HarmonyPatch(nameof(Piece.DropResources))]
        private static IEnumerable<CodeInstruction> DropResourcesTranspiler(
            IEnumerable<CodeInstruction> instructions
        )
        {
            /* Target this IL code
             * // Requirement[] resources = m_resources;
             * IL_0011: ldarg.0
             * IL_0012: ldfld class Piece/Requirement[] Piece::m_resources
             * IL_0017: stloc.1
             * // (no C# code)
             * IL_0018: ldc.i4.0
             * IL_0019: stloc.2
             * // 	foreach (Requirement requirement in resources)
             */
            // want to be able to edit the resources that get dropped
            return new CodeMatcher(instructions)
                .MatchForward(
                    useEnd: false,
                    new CodeMatch(
                        OpCodes.Ldfld,
                        AccessTools.Field(typeof(Piece), nameof(Piece.m_resources))),
                    new CodeMatch(OpCodes.Stloc_1)
                )
                .SetInstructionAndAdvance(
                    Transpilers.EmitDelegate<Func<Piece, Piece.Requirement[]>>(DropResources_m_resources_Delegate))
                .InstructionEnumeration();
        }

        /// <summary>
        ///     Delegate that sets dropped resources to default resources
        ///     for any piece altered by this mod if the piece was not built
        ///     by a player and adjusts dropped resources for pickable items.
        ///     Also picks pickables (if not already picked) and
        ///     forces ItemStand to drop attached item (if it has one).
        /// </summary>
        /// <param name="__instance"></param>
        /// <param name="__state"></param>
        private static Piece.Requirement[] DropResources_m_resources_Delegate(Piece piece)
        {
            // Only interact if it is a piece added by this mod or
            // the prefab has previously had it's resources altered by the mod
            if (PluginConfig.IsVerbosityMedium)
            {
                Log.LogInfo("DropResources_m_resources_Delegate()");
            }
            string prefabName = NameHelper.GetPrefabName(piece);

            if (!IsPatchedByMod(prefabName))
            {
                // do nothing it not a piece the mod changes
                return piece.m_resources;
            }

            // Set resources to defaults is piece is not placed by player
            // or disable desctruction drops if it is placed by player
            var resources = Array.Empty<Piece.Requirement>();
            if (DefaultPieceClones.ContainsKey(prefabName))
            {
                if (!piece.IsPlacedByPlayer())
                {
                    if (DefaultPieceClones[prefabName].m_resources != null)
                    {
                        // set to default resources for world-generated pieces
                        resources = DefaultPieceClones[prefabName].m_resources;
                    }
                }
                else
                {
                    resources = piece.m_resources;
                }
            }

            var zNetView = piece?.gameObject?.GetComponent<ZNetView>();

            // If piece has an ItemStand and it has an item, then drop it.
            var itemStand = piece?.gameObject?.GetComponentInChildren<ItemStand>();
            if (itemStand != null && zNetView != null)
            {
                var canBeRemoved = itemStand.m_canBeRemoved;
                itemStand.m_canBeRemoved = true;
                zNetView.InvokeRPC("DropItem");
                itemStand.m_canBeRemoved = canBeRemoved;
            }

            // If piece is pickable and it has not been picked, then pick it.
            var pickable = piece?.gameObject?.GetComponent<Pickable>();
            if (pickable != null && zNetView != null)
            {
                zNetView.InvokeRPC("Pick");

                // Adjust drops to avoid duplicating pickable item (avoid infinite resource exploits).
                resources = RequirementsHelper.RemovePickableFromRequirements(resources, pickable);
            }

            return resources;
        }
    }
}