using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using UnityEngine;
using HarmonyLib;

using MoreVanillaBuildPrefabs.Logging;
using MoreVanillaBuildPrefabs.Configs;
using MoreVanillaBuildPrefabs.Helpers;
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
        static void PieceSetCreatorPrefix(long uid, Piece __instance)
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
        ///     by a player. Also indicates if drops from DropOnDestroyed should 
        ///     be disabled via setting DisableDesctructionDrops = true;
        /// </summary>
        /// <param name="__instance"></param>
        /// <param name="__state"></param>
        [HarmonyTranspiler]
        [HarmonyPatch(nameof(Piece.DropResources))]
        static IEnumerable<CodeInstruction> DropResourcesTranspiler(
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
            Log.LogInfo(AccessTools.Field(typeof(Piece), nameof(Piece.m_resources)));
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
        ///     by a player. Also indicates if drops from DropOnDestroyed should 
        ///     be disabled via setting DisableDesctructionDrops = true;
        ///     
        ///     Disabling destruction drops for player built pieces to prevents
        ///     things like player built dvergerprops_crate dropping dvergr 
        ///     extractors even when they're not a build requirement.
        /// </summary>
        /// <param name="__instance"></param>
        /// <param name="__state"></param>
        private static Piece.Requirement[] DropResources_m_resources_Delegate(Piece piece)
        {
            // Only interact if it is a piece added by this mod or
            // the prefab has previously had it's resources altered by the mod
#if DEBUG
            Log.LogInfo("DropResources_m_resources_Delegate()");
#endif
            string prefabName = NameHelper.GetPrefabName(piece);
            if (DefaultPieceClones.ContainsKey(prefabName))
            {
                if (!piece.IsPlacedByPlayer())
                {
                    var resources = Array.Empty<Piece.Requirement>();
                    if (DefaultPieceClones[prefabName].m_resources != null)
                    {
                        resources = DefaultPieceClones[prefabName].m_resources;
                    }
                    return resources;
                }
                else
                {
                    DisableDropOnDestroyed = true;
                }
            }
            return piece.m_resources;
        }
    }
}
