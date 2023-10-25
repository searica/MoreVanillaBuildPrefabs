using HarmonyLib;
using Jotunn.Managers;
using MoreVanillaBuildPrefabs.Configs;
using MoreVanillaBuildPrefabs.Helpers;
using MoreVanillaBuildPrefabs.Logging;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using UnityEngine;

namespace MoreVanillaBuildPrefabs
{
    [HarmonyPatch(typeof(Player))]
    internal static class PlayerPatch
    {
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
            if (PluginConfig.IsVerbosityMedium)
            {
                Log.LogInfo("PlacePieceInstantiateDelegate()");
            }

            var result = UnityEngine.Object.Instantiate(gameObject, position, rotation);

            if (PieceHelper.AddedPrefabs.Contains(gameObject.name))
            {
                var container = result.GetComponent<Container>();
                if (container != null)
                {
                    container.m_inventory.RemoveAll();
                    if (PluginConfig.IsVerbosityMedium)
                    {
                        Log.LogInfo($"Emptied inventory for: {gameObject.name}");
                    }
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
                .SetInstructionAndAdvance(
                    Transpilers.EmitDelegate<Func<GameObject, GameObject>>(SetupPlacementGhostInstantiateDelegate))
                .InstructionEnumeration();
        }

        private static GameObject SetupPlacementGhostInstantiateDelegate(GameObject selectedPrefab)
        {
            if (!MoreVanillaBuildPrefabs.IsChangedByMod(selectedPrefab.name))
            {
                // ignore pieces not touched by this mod
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
                && PlacementConfigs.NeedsCollisionPatchForGhost(selectedPrefab.name)
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
        private static bool CheckCanRemovePrefix(Player __instance, Piece piece, ref bool __result)
        {
            // Only modify results for pieces affected by this mod
            var prefabName = NameHelper.GetPrefabName(piece);
            if (!MoreVanillaBuildPrefabs.IsChangedByMod(prefabName))
            {
                return true; // run CheckCanRemove method as normal
            }

            // Prevents world generated piece from player removal with build hammer.
            if (!piece.IsPlacedByPlayer() && PieceCategoryHelper.IsCreativeModePiece(piece))
            {
                __result = false;
                return false;
            }

            // Prevents player from breaking pieces they didn't
            // create themselves unless admin check and config is true.
            if (PieceCategoryHelper.IsCreativeModePiece(piece) && !piece.IsCreator())
            {
                // Allow admins to deconstruct CreatorShop pieces built by other players if setting is enabled in config
                if (PluginConfig.IsAdminDeconstructOtherPlayers && SynchronizationManager.Instance.PlayerIsAdmin)
                {
                    __result = true;
                    return true;
                }
                __result = false;
                return false;
            }

            return true;
        }

        /// <summary>
        ///     Patch to remove some code and replace it with code
        ///     that figures out the approach removal effects.
        /// </summary>
        /// <param name="instructions"></param>
        /// <returns></returns>
        [HarmonyTranspiler]
        [HarmonyPatch(nameof(Player.RemovePiece))]
        private static IEnumerable<CodeInstruction> RemovePieceTranspiler(IEnumerable<CodeInstruction> instructions)
        {
            // piece.m_placeEffect.Create(piece.transform.position, piece.transform.rotation, piece.gameObject.transform);
            // IL_0162: ldloc.1
            // IL_0163: ldfld private class EffectList private Piece::m_placeEffect
            // IL_0168: ldloc.1
            // IL_0169: private callvirt instance private class [UnityEngine.CoreModule] UnityEngine.Transform[UnityEngine.CoreModule] UnityEngine.Component::get_transform()
            // IL_016e: private callvirt instance valuetype[UnityEngine.CoreModule] UnityEngine.Vector3 [UnityEngine.CoreModule] UnityEngine.Transform::get_position()
            // IL_0173: ldloc.1
            // IL_0174: callvirt instance private class [UnityEngine.CoreModule] UnityEngine.Transform[UnityEngine.CoreModule] UnityEngine.Component::get_transform()
            // IL_0179: private callvirt instance valuetype[UnityEngine.CoreModule] UnityEngine.Quaternion [UnityEngine.CoreModule] UnityEngine.Transform::get_rotation()
            // IL_017e: ldloc.1
            // IL_017f: callvirt instance private class [UnityEngine.CoreModule] UnityEngine.GameObject[UnityEngine.CoreModule] UnityEngine.Component::get_gameObject()
            // IL_0184: private callvirt instance private class [UnityEngine.CoreModule] UnityEngine.Transform[UnityEngine.CoreModule] UnityEngine.GameObject::get_transform()
            // IL_0189: ldc.r4 1
            // IL_018e: private ldc.i4.m1
            // IL_018f: private callvirt instance private class [UnityEngine.CoreModule] private UnityEngine.GameObject[] EffectList::Create(valuetype[UnityEngine.CoreModule] UnityEngine.Vector3, valuetype[UnityEngine.CoreModule] UnityEngine.Quaternion, private class [UnityEngine.CoreModule] UnityEngine.Transform, float32, int32)
            // IL_0194: pop

            // m_removeEffects.Create(piece.transform.position, Quaternion.identity);
            // IL_0195: ldarg.0
            // IL_0196: ldfld class EffectList Player::m_removeEffects
            // IL_019b: ldloc.1
            // IL_019c: callvirt instance class [UnityEngine.CoreModule] UnityEngine.Transform[UnityEngine.CoreModule] UnityEngine.Component::get_transform()
            // IL_01a1: callvirt instance valuetype[UnityEngine.CoreModule] UnityEngine.Vector3 [UnityEngine.CoreModule] UnityEngine.Transform::get_position()
            // IL_01a6: call valuetype [UnityEngine.CoreModule] UnityEngine.Quaternion[UnityEngine.CoreModule] UnityEngine.Quaternion::get_identity()
            // IL_01ab: ldnull
            // IL_01ac: ldc.r4 1
            // IL_01b1: ldc.i4.m1
            // IL_01b2: callvirt instance class [UnityEngine.CoreModule] UnityEngine.GameObject[] EffectList::Create(valuetype[UnityEngine.CoreModule] UnityEngine.Vector3, valuetype[UnityEngine.CoreModule] UnityEngine.Quaternion, class [UnityEngine.CoreModule] UnityEngine.Transform, float32, int32)
            //IL_01b7: pop
            var codes = new CodeMatch[]
            {
                new CodeMatch(
                    OpCodes.Ldfld,
                    AccessTools.Field(typeof(Piece), nameof(Piece.m_placeEffect))
                ),
            };
            return new CodeMatcher(instructions)
                .MatchForward(useEnd: false, codes)
                .RemoveInstructions(14 + 11)
                .InsertAndAdvance(
                Transpilers.EmitDelegate<Action<Piece>>(RemovePieceEffectsDelegate))
                .InstructionEnumeration();
        }

        private static void RemovePieceEffectsDelegate(Piece piece)
        {
            if (!MoreVanillaBuildPrefabs.IsChangedByMod(NameHelper.GetPrefabName(piece)))
            {
                piece.m_placeEffect.Create(piece.transform.position, piece.transform.rotation, piece.gameObject.transform);
                Player.s_players[0].m_removeEffects.Create(piece.transform.position, Quaternion.identity);
                return;
            }

            // If this is called that means no WearNTear component on the piece.
            if (PluginConfig.IsVerbosityMedium) { Log.LogInfo("RemovePieceEffectsDelegate"); }

            var destructible = piece?.gameObject?.GetComponent<Destructible>();
            if (destructible != null)
            {
                var effects = destructible.m_destroyedEffect?.m_effectPrefabs;
                if (effects != null && effects.Length != 0)
                {
                    // Create destructible effects
                    destructible.CreateDestructionEffects(Vector3.zero, Vector3.zero);
                    if (destructible.m_destroyNoise > 0f)
                    {
                        Player closestPlayer = Player.GetClosestPlayer(piece.transform.position, 10f);
                        if (closestPlayer)
                        {
                            closestPlayer.AddNoise(destructible.m_destroyNoise);
                        }
                    }

                    // End script if a sfx was produced
                    foreach (var effect in effects)
                    {
                        if (effect != null && effect.m_prefab.name.StartsWith("sfx_"))
                        {
                            return;
                        }
                    }
                }
            }
            // Create sfx if needed.
            SfxHelper.FixRemovalSfx(piece).Create(
                piece.transform.position,
                piece.transform.rotation,
                piece.gameObject.transform
            );
        }
    }
}