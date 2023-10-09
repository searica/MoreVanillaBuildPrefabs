using HarmonyLib;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection.Emit;
using Jotunn.Managers;
using MoreVanillaBuildPrefabs.Utils;

namespace MoreVanillaBuildPrefabs
{
    internal class PlayerPatches
    {
        [HarmonyPatch(typeof(Player))]
        static class PlayerPatch
        {
            [HarmonyTranspiler]
            [HarmonyPatch(nameof(Player.PlacePiece))]
            static IEnumerable<CodeInstruction> PlacePieceTranspiler(IEnumerable<CodeInstruction> instructions)
            {
                // Targeting code
                // GameObject gameObject2 = Object.Instantiate(gameObject, position, rotation);
                //  IL_012c: ldloc.2
                //  IL_012d: ldloc.0
                //  IL_012e: ldloc.1
                //  IL_012f: call !!0 [UnityEngine.CoreModule] UnityEngine.Object::Instantiate<class [UnityEngine.CoreModule] UnityEngine.GameObject>(!!0, valuetype[UnityEngine.CoreModule] UnityEngine.Vector3, valuetype[UnityEngine.CoreModule] UnityEngine.Quaternion)
                //      IL_0134: stloc.3

                // want to be able to edit the instantiated gameObject2 
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
                    .SetInstructionAndAdvance(
                        Transpilers.EmitDelegate<Func<GameObject, Vector3, Quaternion, GameObject>>(PlacePieceInstantiateDelegate))
                    .InstructionEnumeration();
            }

            private static GameObject PlacePieceInstantiateDelegate(
                GameObject gameObject,
                Vector3 position,
                Quaternion rotation
            )
            {
                var gameObject2 = UnityEngine.Object.Instantiate(gameObject, position, rotation);
                if (!PluginConfig.IsModEnabled.Value)
                {
                    return gameObject2;
                }

                if (PieceHelper.AddedPrefabs.Contains(gameObject.name)
                    && DefaultConfigs.RemovePickable.Contains(gameObject.name)
                )
                {
                    var pickable = gameObject2.GetComponent<Pickable>();
                    if (pickable != null)
                    {
                        UnityEngine.Object.DestroyImmediate(pickable);
                    }
                }
                return gameObject2;
            }

            [HarmonyTranspiler]
            [HarmonyPatch(nameof(Player.SetupPlacementGhost))]
            static IEnumerable<CodeInstruction> SetupPlacementGhostTranspiler(IEnumerable<CodeInstruction> instructions)
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
                                    new Type[] { typeof(Type) })
                                .MakeGenericMethod(typeof(GameObject))),
                        new CodeMatch(OpCodes.Stfld, AccessTools.Field(typeof(Player), nameof(Player.m_placementGhost))))
                    .SetInstructionAndAdvance(
                        Transpilers.EmitDelegate<Func<GameObject, GameObject>>(SetupPlacementGhostInstantiateDelegate))
                    .InstructionEnumeration();
            }

            private static GameObject SetupPlacementGhostInstantiateDelegate(GameObject selectedPrefab)
            {
                if (!PluginConfig.IsModEnabled.Value)
                {
                    return UnityEngine.Object.Instantiate(selectedPrefab);
                }

                bool setActive = false;

                if (selectedPrefab.GetComponent<MonsterAI>()
                    || selectedPrefab.GetComponent<AnimalAI>()
                    || selectedPrefab.GetComponent<Tameable>()
                    || selectedPrefab.GetComponent<Ragdoll>()
                    || selectedPrefab.GetComponent<Humanoid>())
                {
                    setActive = selectedPrefab.activeSelf;
                    selectedPrefab.SetActive(false);
                }

                GameObject clonedPrefab = UnityEngine.Object.Instantiate(selectedPrefab);

                if (
                    PieceHelper.AddedPrefabs.Contains(selectedPrefab.name)
                    && DefaultConfigs.NeedsCollisionPatchForGhost.Contains(selectedPrefab.name)
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
            [HarmonyPatch(nameof(Player.CheckCanRemovePiece))]
            static bool CheckCanRemovePrefix(Player __instance, Piece piece, ref bool __result)
            {
                if (PluginConfig.IsModEnabled.Value)
                {
                    // Prevents world generated piece from player removal with build hammer.
                    if (!piece.IsPlacedByPlayer() && HammerCategories.IsCreatorShopPiece(piece))
                    {
                        __result = false;
                        return false;
                    }

                    // Prevents player from breaking pottery barn pieces they didn't
                    // create themselves unless admin check and config is true.
                    if (HammerCategories.IsCreatorShopPiece(piece) && !piece.IsCreator())
                    {
                        // Allow admins to deconstruct CreatorShop pieces built by other players if setting is enabled in config
                        if (PluginConfig.AdminDeconstructCreatorShop.Value && SynchronizationManager.Instance.PlayerIsAdmin)
                        {
                            __result = true;
                            return true;
                        }
                        __result = false;
                        return false;
                    }
                }

                return true;
            }
        }
    }
}
