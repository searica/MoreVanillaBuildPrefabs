using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using UnityEngine;
using Logging;
using MVBP.PrefabManagement;
using MVBP.Utils;

namespace MVBP.Functions;

[HarmonyPatch(typeof(Placement))]
internal static class Placement
{
    /// <summary>
    ///     Placement and placement ghost patches.
    /// </summary>
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

            // want to be able to edit the instantiated gameObject
            // such that I can remove things like the pickable property from the surtling core stands

            System.Reflection.MethodInfo instantiateMethod = ReflectionUtils.GetGenericMethod(
                typeof(UnityEngine.Object),
                nameof(UnityEngine.Object.Instantiate),
                genericParameterCount: 1,
                new Type[] { typeof(Type), typeof(Vector3), typeof(Quaternion) }
            ).MakeGenericMethod(typeof(GameObject));

            var codeMatches = new CodeMatch[] {
                new(OpCodes.Call, instantiateMethod)
            };

            return new CodeMatcher(instructions)
                .MatchForward(useEnd: true, codeMatches)
                .Advance(1)
                .InsertAndAdvance(Transpilers.EmitDelegate(EmptyInventoryOnPlacement))
                .InstructionEnumeration();
        }

        private static GameObject EmptyInventoryOnPlacement(GameObject gameObject)
        {
            Log.LogInfo("EmptyInventoryOnPlacement()", Log.InfoLevel.Medium);

            if (!PrefabConfigManager.IsPrefabEnabled(gameObject) &&
                gameObject.TryGetComponent(out Container container))
            {
                container.m_inventory.RemoveAll();
                Log.LogInfo($"Emptied inventory for: {gameObject.name}", Log.InfoLevel.Medium);
            }

            return gameObject;
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

            System.Reflection.MethodInfo instantiateMethod = ReflectionUtils.GetGenericMethod
                (typeof(UnityEngine.Object),
                nameof(UnityEngine.Object.Instantiate),
                genericParameterCount: 1,
                new Type[] { typeof(Type) }
            ).MakeGenericMethod(typeof(GameObject));

            var codeMatches = new CodeMatch[]
            {
             new CodeMatch(OpCodes.Call, instantiateMethod),
             new(
                 OpCodes.Stfld,
                 AccessTools.Field(typeof(Player), nameof(Player.m_placementGhost))
            )
            };

            return new CodeMatcher(instructions)
                .MatchForward(useEnd: false, codeMatches)
                .SetInstructionAndAdvance(Transpilers.EmitDelegate(SetupPlacementGhostInstantiateDelegate))
                .InstructionEnumeration();
        }

        private static GameObject SetupPlacementGhostInstantiateDelegate(GameObject selectedPrefab)
        {
            if (!selectedPrefab) // selected prefab is null, probably due to some other mod nonsense
            {
                return selectedPrefab;
            }
            if (!ZNetPrefabManager.IsPatchedByMVBP(selectedPrefab))
            {
                // ignore pieces not touched by this mod
                return UnityEngine.Object.Instantiate(selectedPrefab);
            }

            bool setActive = false;

            if (selectedPrefab.GetComponent<MonsterAI>() ||
                selectedPrefab.GetComponent<AnimalAI>() ||
                selectedPrefab.GetComponent<Tameable>() ||
                selectedPrefab.GetComponent<Ragdoll>() ||
                selectedPrefab.GetComponent<Humanoid>() ||
                selectedPrefab.GetComponent<TimedDestruction>()
            )
            {
                setActive = selectedPrefab.activeSelf;
                selectedPrefab.SetActive(false);
            }

            GameObject clonedPrefab = UnityEngine.Object.Instantiate(selectedPrefab);

            // Handle selection of non-standard prefabs (like in InfinityHammer)
            //string prefabName = selectedPrefab.GetPrefabName();

            string prefabName = selectedPrefab.name;

            if (PrefabConfigManager.NeedsCollisionPatchForGhost(prefabName))
            {
                // Needed to make some things work, like Stalagmite, blackmarble_corner_stair, silvervein, etc.
                ColliderManager.PatchCollider(clonedPrefab);
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
            if (clonedPrefab.TryGetComponent(out TimedDestruction timedDestruction))
            {
                UnityEngine.Object.DestroyImmediate(timedDestruction);
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

        /// <summary>
        ///     Offset center of prefabs with poor center locations.
        /// </summary>
        /// <param name="__instance"></param>
        [HarmonyPostfix]
        [HarmonyPatch(nameof(Player.UpdatePlacementGhost))]
        private static void UpdatePlacementGhostPostfix(Player __instance)
        {
            if (__instance.m_placementGhost == null)
            {
                if (__instance.m_placementMarkerInstance)
                {
                    __instance.m_placementMarkerInstance.SetActive(value: false);
                }
                return;
            }

            if (PrefabConfigManager.TryGetPrefabConfig(__instance.m_placementGhost, out var prefabConfig, checkIfBound: true) &&
                prefabConfig.PlacementOffset.HasValue)
            {
                Quaternion quaternion = __instance.m_placementGhost.transform.rotation;
                Vector3 pos = __instance.m_placementGhost.transform.position;
                __instance.m_placementGhost.transform.position = pos + (quaternion * prefabConfig.PlacementOffset.Value);
            }
        }
    }

    /// <summary>
    ///     Patch pieces upon placement.
    /// </summary>
    [HarmonyPatch(typeof(Piece))]
    internal static class PiecePatch
    {
        /// <summary>
        ///     Applies patches from PatchPlayerBuildPieceIfNeeded
        ///     when pieces are loaded in.
        /// </summary>
        /// <param name="__instance"></param>
        [HarmonyPostfix]
        [HarmonyPriority(Priority.VeryHigh)]
        [HarmonyPatch(nameof(Piece.Awake))]
        private static void PieceAwakePostfix(Piece __instance)
        {
            PlayerPiecePatcher.PatchPlayerBuiltPieceIfNeeded(__instance);
        }

        /// <summary>
        ///     Applies patches from PatchPlayerBuildPieceIfNeeded
        ///     when pieces are loaded are placed.
        /// </summary>
        /// <param name="__instance"></param>
        [HarmonyPostfix]
        [HarmonyPriority(Priority.VeryHigh)]
        [HarmonyPatch(nameof(Piece.SetCreator))]
        private static void PieceSetCreatorPostfix(Piece __instance)
        {
            PlayerPiecePatcher.PatchPlayerBuiltPieceIfNeeded(__instance);
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
            ZNetView view = __instance.GetComponent<ZNetView>();
            if (view && !view.m_persistent)
            {
                view.m_persistent = true;

                ZSyncTransform sync = __instance.gameObject.GetComponent<ZSyncTransform>();
                if (!sync)
                {
                    sync = __instance.gameObject.AddComponent<ZSyncTransform>();
                }
                sync.m_syncPosition = true;
                sync.m_syncRotation = true;
            }
        }
    }
}
