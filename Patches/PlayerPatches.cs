using HarmonyLib;
using System;
using System.Collections.Generic;
using UnityEngine;
using System.Reflection.Emit;
using UnityEngine.EventSystems;

namespace MoreVanillaBuildPrefabs.Patces
{
    internal class PlayerPatches
    {
        [HarmonyPatch(typeof(Player))]
        static class PlayerPatch
        {

            [HarmonyTranspiler]
            [HarmonyPatch(nameof(Player.SetupPlacementGhost))]
            static IEnumerable<CodeInstruction> SetupPlacementGhostTranspiler(IEnumerable<CodeInstruction> instructions)
            {
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

            static GameObject SetupPlacementGhostInstantiateDelegate(GameObject selectedPrefab)
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
                    PrefabAdder.AddedPieces.ContainsValue(selectedPrefab.name) 
                    && PrefabDefaults.NeedsCollisionPatchForGhost.Contains(selectedPrefab.name)
                    )
                {
                    // Needed to make some things work, like Stalagmite, blackmarble_corner_stair, silvervein, etc.
                    Colliders.PatchCollider(clonedPrefab);
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
                    if (!piece.IsPlacedByPlayer() && Plugin.IsCreatorShopPiece(piece))
                    {
                        __result = false;
                        return false;
                    }

                    // Prevents player from breaking pottery barn pieces they didn't create themselves.
                    if (Plugin.IsCreatorShopPiece(piece) && !piece.IsCreator())
                    {
                        __result = false;
                        return false;
                    }
                }

                return true;
            }
        }
    }
}
