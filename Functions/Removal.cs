using HarmonyLib;
using Jotunn.Managers;
using MVBP.Extensions;
using MVBP.Helpers;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using UnityEngine;

namespace MVBP.Functions
{

    [HarmonyPatch(typeof(Removal))]
    internal static class Removal
    {
        [HarmonyPatch(typeof(Player))]
        internal static class PlayerPatch
        {
            private static readonly int PieceRemovalMask = LayerMask.GetMask(
                "Default",
                "static_solid",
                "Default_small",
                "piece",
                "piece_nonsolid",
                "terrain",
                "vehicle",
                "item",
                "piece_nonsolid",
                "Default_small"
            );

            [HarmonyPrefix]
            [HarmonyPatch(nameof(Player.RemovePiece))]
            internal static bool RemovePiecePrefix(Player __instance, ref bool __result)
            {
                if (__instance.GetRightItem().m_shared.m_name == "$item_hammer")
                {
                    var cameraTrans = GameCamera.instance.transform;
                    if (Physics.Raycast(cameraTrans.position, cameraTrans.forward, out var hitInfo, 50f, PieceRemovalMask) &&
                        Vector3.Distance(hitInfo.point, __instance.m_eye.position) < __instance.m_maxPlaceDistance)
                    {
                        Piece piece = hitInfo.collider.GetComponentInParent<Piece>();
                        if (piece && InitManager.IsPatchedByMod(piece))
                        {
                            __result = RemoveCustomPiece(__instance, piece);
                            return false; // skip vanilla method
                        }
                    }
                }
                return true; // run vanilla method
            }

            private static bool RemoveCustomPiece(Player player, Piece piece)
            {
                if (piece)
                {
                    if (!Check_m_canBeRemoved(piece))
                    {
                        return false;
                    }
                    if (Location.IsInsideNoBuildLocation(piece.transform.position))
                    {
                        player.Message(MessageHud.MessageType.Center, "$msg_nobuildzone");
                        return false;
                    }
                    if (!PrivateArea.CheckAccess(piece.transform.position))
                    {
                        player.Message(MessageHud.MessageType.Center, "$msg_privatezone");
                        return false;
                    }
                    if (!player.CheckCanRemovePiece(piece))
                    {
                        return false;
                    }
                    ZNetView component = piece.GetComponent<ZNetView>();
                    if (component == null)
                    {
                        return false;
                    }
                    if (!piece.CanBeRemoved())
                    {
                        player.Message(MessageHud.MessageType.Center, "$msg_cantremovenow");
                        return false;
                    }
                    WearNTear component2 = piece.GetComponent<WearNTear>();
                    if (component2)
                    {
                        component2.Remove();
                    }
                    else
                    {
                        Log.LogInfo("Removing non WNT object with hammer " + piece.name);
                        component.ClaimOwnership();
                        if (!RemoveDestructiblePiece(piece)
                            && !RemoveMineRock5Piece(piece)
                            && !RemoveMineRockPiece(piece))
                        {
                            piece.DropResources();
                            piece.m_placeEffect.Create(piece.transform.position, piece.transform.rotation, piece.gameObject.transform);
                            player.m_removeEffects.Create(piece.transform.position, Quaternion.identity);
                            ZNetScene.instance.Destroy(piece.gameObject);
                        }
                    }
                    ItemDrop.ItemData rightItem = player.GetRightItem();
                    if (rightItem != null)
                    {
                        player.FaceLookDirection();
                        player.m_zanim.SetTrigger(rightItem.m_shared.m_attack.m_attackAnimation);
                    }
                    return true;
                }
                return false;
            }

            private static bool Check_m_canBeRemoved(Piece piece)
            {

                if (InitManager.IsPrefabEnabled(piece.gameObject) &&
                    PieceCategoryHelper.IsCreativeModePiece(piece) &&
                    piece.IsPlacedByPlayer())
                {
                    // Allow creative mode pieces to be removed by creator
                    if (piece.IsCreator()) { return true; }

                    // Allow creative mode pieces to be removed by admin (based on config settings)
                    if (MorePrefabs.IsAdminDeconstructOtherPlayers && SynchronizationManager.Instance.PlayerIsAdmin)
                    {
                        return true;
                    }
                }

                // Follow vanilla rules for non-creative mode pieces
                return piece.m_canBeRemoved;
            }

            private static bool RemoveDestructiblePiece(Piece piece)
            {
                if (piece.gameObject.TryGetComponent(out Destructible destructible))
                {
                    Log.LogInfo("Removing destructible piece", LogLevel.Medium);

                    if (!CreateHitEffects(destructible) && !SfxHelper.HasSfx(destructible.m_destroyedEffect))
                    {
                        SfxHelper.CreateRemovalSfx(piece); // create deconstruction SFX if needed
                    }

                    destructible.DestroyNow();
                    return true;
                }

                return false;
            }

            private static bool CreateHitEffects(Destructible destructible)
            {
                Log.LogInfo("Creating hit effects", LogLevel.Medium);

                var hitEffects = destructible.m_hitEffect?.m_effectPrefabs;
                if (hitEffects != null && hitEffects.Length != 0)
                {
                    destructible.m_hitEffect.Create(
                        destructible.gameObject.transform.position,
                        destructible.gameObject.transform.rotation,
                        destructible.gameObject.transform
                    );
                    foreach (var effect in hitEffects)
                    {
                        if (effect != null && effect.m_prefab.name.StartsWith("sfx_"))
                        {
                            return true;
                        }
                    }
                }
                return false;
            }

            private static bool RemoveMineRock5Piece(Piece piece)
            {
                if (piece.gameObject.TryGetComponent(out MineRock5 mineRock5))
                {
                    Log.LogInfo("Removing MineRock5 piece", LogLevel.Medium);

                    mineRock5.DestroyMineRock5Piece();
                    return true;
                }
                return false;
            }

            private static bool RemoveMineRockPiece(Piece piece)
            {
                if (piece.gameObject.TryGetComponent(out MineRock mineRock))
                {
                    Log.LogInfo("Removing MineRock5 piece", LogLevel.Medium);
                    mineRock.DestroyMineRockPiece();
                    return true;
                }
                return false;
            }
        }

        /// <summary>
        ///     Patches to Piece.DropResources which triggers on piece removal.
        /// </summary>
        [HarmonyPatch(typeof(Piece))]
        internal static class PiecePatch
        {
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
                 * IL_001d: ldarg.0
                 * IL_001e: ldfld class Piece/Requirement[] Piece::m_resources
                 * IL_0023: stloc.2
                 * // (no C# code)
                 * IL_0024: ldc.i4.0
                 * IL_0025: stloc.3
                 * // 	foreach (Requirement requirement in resources)
                 */
                return new CodeMatcher(instructions)
                    .MatchForward(
                        useEnd: false,
                        new CodeMatch(
                            OpCodes.Ldfld,
                            AccessTools.Field(typeof(Piece), nameof(Piece.m_resources))),
                        new CodeMatch(OpCodes.Stloc_2)
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
                if (!piece)
                {
                    return Array.Empty<Piece.Requirement>();
                }

                if (!InitManager.IsPatchedByMod(piece))
                {
                    // do nothing if not a piece the mod changes
                    return piece.m_resources;
                }

                // Only interact if it is a piece added by this mod or
                // the prefab has previously had it's resources altered by the mod
                Log.LogInfo("Dropping resources for MVBP piece", LogLevel.Medium);

                // Set resources to defaults is piece is not placed by player (world-generated pieces)
                if (!piece.IsPlacedByPlayer() && InitManager.TryGetDefaultPieceResources(piece.gameObject, out Piece.Requirement[] defaultResources) && defaultResources != null)
                {
                    return defaultResources;
                }

                // Set resources to current piece resources if placed by a player
                var resources = piece.m_resources;

                // Early return if GameObject is missing
                if (!piece.gameObject)
                {
                    return resources;
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

                // Early return if ZNetView is missing
                if (!piece.gameObject.TryGetComponent(out ZNetView zNetView))
                {
                    return resources;
                }

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

                return resources;
            }
        }

        [HarmonyPatch(typeof(Destructible))]
        internal static class DestructiblePatch
        {

            /// <summary>
            ///     Make player-built instances of pieces added by MVBP drop
            ///     their build resources when destroyed and prevent destructible drops.
            /// </summary>
            /// <param name="__instance"></param>
            [HarmonyPrefix]
            [HarmonyPatch(nameof(Destructible.Destroy))]
            private static bool DestroyPrefix(Destructible __instance)
            {
                if (__instance && InitManager.IsPatchedByMod(__instance.gameObject))
                {
                    var piece = __instance.GetComponent<Piece>();
                    if (piece && piece.IsPlacedByPlayer())
                    {
                        piece.DropResources();
                        // If it got picked during DropResources then it may no longer be valid.
                        var nview = __instance.m_nview;
                        if (!nview.IsValid() || !nview.IsOwner())
                        {
                            Log.LogInfo("Piece nview was destroyed during DropResources()", LogLevel.Medium);
                            return false;
                        }
                    }
                }
                return true;
            }
        }
    

        /// <summary>
        ///     Prevent infinite resources via random extraDrops when removing pickables
        /// </summary>
        [HarmonyPatch(typeof(Pickable))]
        internal static class PickablePatch
        {
            private static readonly DropTable emptyDrops = new();

            [HarmonyPrefix]
            [HarmonyPatch(nameof(Pickable.RPC_Pick))]
            private static void RPC_PickPrefix(Pickable __instance, out DropTable __state)
            {
                if (InitManager.IsPatchedByMod(__instance) && __instance.TryGetComponent(out Piece piece) && piece.IsPlacedByPlayer())
                {
                    __state = __instance.m_extraDrops;
                    __instance.m_extraDrops = emptyDrops;
                }
                else
                {
                    __state = null;
                }
            }

            [HarmonyPostfix]
            [HarmonyPatch(nameof(Pickable.RPC_Pick))]
            private static void RPC_PickPostfix(Pickable __instance, DropTable __state)
            {
                if (__state != null)
                {
                    __instance.m_extraDrops = __state;
                }
            }
        }

        [HarmonyPatch(typeof(MineRock))]
        internal static class MineRockPatch
        {
            /// <summary>
            ///     Patch to drop resources when MineRock is destroyed
            /// </summary>
            /// <param name="__instance"></param>
            /// <param name="__result"></param>
            [HarmonyPostfix]
            [HarmonyPatch(nameof(MineRock.AllDestroyed))]
            private static void AllDestroyed(MineRock __instance, ref bool __result)
            {
                if (__result && __instance && __instance.gameObject && __instance.gameObject.TryGetComponent(out Piece piece))
                {
                    piece.DropResources();
                }
            }
        }

        [HarmonyPatch(typeof(MineRock5))]
        internal static class MineRock5Patch
        {
            /// <summary>
            ///     Drop piece resources (if they exist) when MineRock5 is
            ///     completely destroyed to ensure a full refund even if
            ///     the MineRock5 was not removed with the hammer.
            ///     The patch applied to Piece.DropResources handles the rest.
            /// </summary>
            /// <param name="__instance"></param>
            /// <param name="__result"></param>
            [HarmonyPostfix]
            [HarmonyPatch(nameof(MineRock5.AllDestroyed))]
            private static void AllDestroyedPostfix(MineRock5 __instance, ref bool __result)
            {
                if (__result && __instance && __instance.TryGetComponent(out Piece piece))
                {
                    piece.DropResources();
                }
            }
        }

        /// <summary>
        ///     Disables destruction drops for player built pieces.
        ///     Prevents things like player built dvergerprops_crate dropping
        ///     dvergr extractors when extractors were not used to build it.
        /// </summary>
        [HarmonyPatch(typeof(DropOnDestroyed))]
        internal static class DropOnDestroyedPatch
        {
            [HarmonyPrefix]
            [HarmonyPatch(nameof(DropOnDestroyed.OnDestroyed))]
            private static bool OnDestroyedPrefix(DropOnDestroyed __instance)
            {
                if (__instance &&
                    InitManager.IsPatchedByMod(__instance) &&
                    __instance.TryGetComponent(out Piece piece)
                    && piece.IsPlacedByPlayer())
                {
                    Log.LogInfo("Disabling on destroyed drops for player-built object", LogLevel.Medium);
                    return false;
                }

                return true;
            }
        }


        /// <summary>
        ///     Fix sfx effects for removal of pieces with WearNTear
        /// </summary>
        [HarmonyPatch(typeof(WearNTear))]
        internal static class WearNTearPatch
        {
            [HarmonyPrefix]
            [HarmonyPatch(nameof(WearNTear.Destroy))]
            private static void DestroyPrefix(WearNTear __instance, out EffectList __state)
            {
                if (InitManager.IsPatchedByMod(__instance) && !SfxHelper.HasSfx(__instance.m_destroyedEffect))
                {
                    __state = __instance.m_destroyedEffect;
                    __instance.m_destroyedEffect = SfxHelper.FixRemovalSfx(__instance);
                    return;
                }

                __state = null;
            }

            [HarmonyPostfix]
            [HarmonyPatch(nameof(WearNTear.Destroy))]
            private static void DestroyPostfix(WearNTear __instance, EffectList __state)
            {
                if (__state != null)
                {
                    __instance.m_destroyedEffect = __state;
                }
            }
        }
    }
}
