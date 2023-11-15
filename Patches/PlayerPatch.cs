// Ignore Spelling: MVBP

using HarmonyLib;
using Jotunn.Managers;
using MVBP.Extensions;
using MVBP.Helpers;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using UnityEngine;

namespace MVBP
{
    [HarmonyPatch(typeof(Player))]
    internal static class PlayerPatch
    {
        private static readonly int PieceRemovalLayerMask = LayerMask.GetMask(
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

        [HarmonyTranspiler]
        [HarmonyPatch(nameof(Player.PlacePiece))]
        private static IEnumerable<CodeInstruction> PlacePieceTranspiler(IEnumerable<CodeInstruction> instructions)
        {
            // Targeting code
            // GameObject result = Object.Instantiate(gameObject, position, rotation);
            //  IL_012c: ldloc.2
            //  IL_012d: ldloc.0
            //  IL_012e: ldloc.1
            //  IL_012f: call !!0 [UnityEngine.CoreModule] UnityEngine.Object::Instantiate<class [UnityEngine.CoreModule] UnityEngine.GameObject>(!!0, valuetype[UnityEngine.CoreModule] UnityEngine.Vector3, valuetype[UnityEngine.CoreModule] UnityEngine.Quaternion)
            //      IL_0134: stloc.3

            // want to be able to edit the instantiated result
            // such that I can remove things like the pickable property from the surtling core stands
            return new CodeMatcher(instructions)
                .MatchForward(
                    useEnd: false,
                    new CodeMatch(
                        OpCodes.Call,
                        ReflectionUtils.GetGenericMethod(
                                typeof(UnityEngine.Object),
                                nameof(UnityEngine.Object.Instantiate),
                                genericParameterCount: 1,
                                new Type[] { typeof(Type), typeof(Vector3), typeof(Quaternion) }
                            )
                            .MakeGenericMethod(typeof(GameObject))
                    ),
                    new CodeMatch(OpCodes.Stloc_3)
                )
                .SetInstructionAndAdvance(Transpilers.EmitDelegate(PlacePieceInstantiateDelegate))
                .InstructionEnumeration();
        }

        private static GameObject PlacePieceInstantiateDelegate(
            GameObject gameObject,
            Vector3 position,
            Quaternion rotation
        )
        {
            Log.LogInfo("PlacePieceInstantiateDelegate()", LogLevel.Medium);

            var result = UnityEngine.Object.Instantiate(gameObject, position, rotation);

            if (PieceHelper.AddedPrefabs.Contains(gameObject.name))
            {
                var container = result.GetComponent<Container>();
                if (container != null)
                {
                    container.m_inventory.RemoveAll();
                    Log.LogInfo($"Emptied inventory for: {gameObject.name}", LogLevel.Medium);
                }
            }
            return result;
        }

        [HarmonyTranspiler]
        [HarmonyPatch(nameof(Player.SetupPlacementGhost))]
        private static IEnumerable<CodeInstruction> SetupPlacementGhostTranspiler(IEnumerable<CodeInstruction> instructions)
        {
            // Targeting this code:
            // m_placementGhost = Object.Instantiate(selectedPrefab);
            //  IL_008c: ldarg.0
            //  IL_008d: ldloc.0
            //  IL_008e: call !!0 [UnityEngine.CoreModule] UnityEngine.Object::Instantiate<class [UnityEngine.CoreModule] UnityEngine.GameObject>(!!0)
            //  IL_0093: stfld class [UnityEngine.CoreModule] UnityEngine.GameObject Player::m_placementGhost
            return new CodeMatcher(instructions)
                .MatchForward(
                    useEnd: false,
                    new CodeMatch(
                        OpCodes.Call,
                        ReflectionUtils.GetGenericMethod(
                                typeof(UnityEngine.Object),
                                nameof(UnityEngine.Object.Instantiate),
                                genericParameterCount: 1,
                                new Type[] { typeof(Type) }
                            )
                            .MakeGenericMethod(typeof(GameObject))
                    ),
                    new CodeMatch(
                        OpCodes.Stfld,
                        AccessTools.Field(typeof(Player), nameof(Player.m_placementGhost)
                        )
                    )
                )
                .SetInstructionAndAdvance(Transpilers.EmitDelegate(SetupPlacementGhostInstantiateDelegate))
                .InstructionEnumeration();
        }

        private static GameObject SetupPlacementGhostInstantiateDelegate(GameObject selectedPrefab)
        {
            if (!InitManager.IsPatchedByMod(selectedPrefab))
            {
                // ignore pieces not touched by this mod
                return UnityEngine.Object.Instantiate(selectedPrefab);
            }

            bool setActive = false;

            if (selectedPrefab.GetComponent<MonsterAI>() ||
                selectedPrefab.GetComponent<AnimalAI>() ||
                selectedPrefab.GetComponent<Tameable>() ||
                selectedPrefab.GetComponent<Ragdoll>() ||
                selectedPrefab.GetComponent<Humanoid>())
            {
                setActive = selectedPrefab.activeSelf;
                selectedPrefab.SetActive(false);
            }

            GameObject clonedPrefab = UnityEngine.Object.Instantiate(selectedPrefab);

            if (
                PieceHelper.AddedPrefabs.Contains(selectedPrefab.name)
                && MorePrefabs.NeedsCollisionPatchForGhost(selectedPrefab.name)
                )
            {
                // Needed to make some things work, like Stalagmite, blackmarble_corner_stair, silvervein, etc.
                CollisionHelper.PatchCollider(clonedPrefab);
            }

            if (!setActive)
            {
                return clonedPrefab;
            }

            selectedPrefab.SetActive(true);

            if (clonedPrefab.TryGetComponent(out MonsterAI monsterAi))
            {
                UnityEngine.Object.DestroyImmediate(monsterAi);
            }

            if (clonedPrefab.TryGetComponent(out AnimalAI animalAi))
            {
                UnityEngine.Object.DestroyImmediate(animalAi);
            }

            if (clonedPrefab.TryGetComponent(out Tameable tameable))
            {
                UnityEngine.Object.DestroyImmediate(tameable);
            }

            if (clonedPrefab.TryGetComponent(out Ragdoll ragdoll))
            {
                UnityEngine.Object.DestroyImmediate(ragdoll);
            }

            if (clonedPrefab.TryGetComponent(out Humanoid humanoid))
            {
                humanoid.m_defaultItems = Array.Empty<GameObject>();
                humanoid.m_randomWeapon = Array.Empty<GameObject>();
                humanoid.m_randomArmor ??= Array.Empty<GameObject>();
                humanoid.m_randomShield ??= Array.Empty<GameObject>();
                humanoid.m_randomSets ??= Array.Empty<Humanoid.ItemSet>();
            }

            clonedPrefab.SetActive(true);

            return clonedPrefab;
        }

        [HarmonyPrefix]
        [HarmonyPatch(nameof(Player.RemovePiece))]
        internal static bool RemovePiecePrefix(Player __instance, ref bool __result)
        {
            if (__instance.GetRightItem().m_shared.m_name == "$item_hammer")
            {
                if (Physics.Raycast(GameCamera.instance.transform.position, GameCamera.instance.transform.forward, out var hitInfo, 50f, PieceRemovalLayerMask) && Vector3.Distance(hitInfo.point, __instance.m_eye.position) < __instance.m_maxPlaceDistance)
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
            if ((bool)piece)
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
                if ((bool)component2)
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
            if (InitManager.IsPrefabEnabled(piece?.gameObject)
                && PieceCategoryHelper.IsCreativeModePiece(piece)
                && piece.IsPlacedByPlayer())
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

            var hitEffects = destructible?.m_hitEffect?.m_effectPrefabs;
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
}