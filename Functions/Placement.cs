using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Reflection.Emit;
using UnityEngine;
using Logging;
using MVBP.PrefabManagement;
using MVBP.Utils;
using Jotunn;
using MVBP.Extensions;

namespace MVBP.Functions;


/// <summary>
///     Placement and placement ghost patches.
/// </summary>
[HarmonyPatch(typeof(Placement))]
internal static class Placement
{
    private static readonly Type[] ComponentsToDisable = new Type[] {
        typeof(MonsterAI),
        typeof(AnimalAI),
        typeof(TimedDestruction),
        typeof(Ragdoll),
        typeof(Tameable),
        typeof(SpawnArea),
        typeof(Humanoid),
        typeof(Animator),
        typeof(RandomSpawn)
    };

    /// <summary>
    ///     Transpiler to allow calling EmptyInventoryOnPlacement 
    ///     after placing a piece. Do not want to do this on Piece.Awake
    ///     as inventory should only be emptied on initial placement and
    ///     not on future loads.
    /// </summary>
    /// <param name="instructions"></param>
    /// <returns></returns>
    [HarmonyTranspiler]
    [HarmonyPatch(typeof(Player), nameof(Player.PlacePiece))]
    private static IEnumerable<CodeInstruction> PlacePieceTranspiler(IEnumerable<CodeInstruction> instructions)
    {
        // Targeting code
        // GameObject result = Object.Instantiate(gameObject, position, rotation);
        //  IL_012c: ldloc.2
        //  IL_012d: ldloc.0
        //  IL_012e: ldloc.1
        //  IL_012f: call !!0 [UnityEngine.CoreModule] UnityEngine.Object::Instantiate<class [UnityEngine.CoreModule] UnityEngine.GameObject>(!!0, valuetype[UnityEngine.CoreModule] UnityEngine.Vector3, valuetype[UnityEngine.CoreModule] UnityEngine.Quaternion)
        //      IL_0134: stloc.3

        System.Reflection.MethodInfo instantiateMethod = ReflectionUtils.GetGenericMethod(
            typeof(UnityEngine.Object),
            nameof(UnityEngine.Object.Instantiate),
            genericParameterCount: 1,
            new Type[] { typeof(Type), typeof(Vector3), typeof(Quaternion) }
        ).MakeGenericMethod(typeof(GameObject));

        // want to be able to edit the instantiated gameObject
        // such that I can remove things like the pickable property from the surtling core stands
        var codeMatches = new CodeMatch[] {
            new(OpCodes.Call, instantiateMethod)
        };

        return new CodeMatcher(instructions)
            .MatchForward(useEnd: true, codeMatches)
            .Advance(1)
            .InsertAndAdvance(Transpilers.EmitDelegate(EmptyInventoryOnPlacement))
            .InstructionEnumeration();
    }

    /// <summary>
    ///     Remove loot from placed treasure chests.
    /// </summary>
    /// <param name="gameObject"></param>
    /// <returns></returns>
    private static GameObject EmptyInventoryOnPlacement(GameObject gameObject)
    {
        Log.LogInfo("EmptyInventoryOnPlacement()", Log.InfoLevel.Medium);

        if (PrefabConfigManager.IsPrefabEnabled(gameObject) &&
            gameObject.TryGetComponent(out Container container))
        {
            container.m_inventory.RemoveAll();
            Log.LogInfo($"Emptied inventory for: {gameObject.name}", Log.InfoLevel.Medium);
        }

        return gameObject;
    }


    [HarmonyTranspiler]
    [HarmonyPatch(typeof(Player), nameof(Player.SetupPlacementGhost))]
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


    /// <summary>
    ///     Custom instantiate method for setting up placement ghost to remove
    ///     components that should not be active on placement ghost like creature AI
    ///     or timed destruction. Also applies collision patch to the placement ghost
    ///     if that is needed based on the PrefabConfig.
    /// </summary>
    /// <param name="selectedPrefab"></param>
    /// <returns></returns>
    private static GameObject SetupPlacementGhostInstantiateDelegate(GameObject selectedPrefab)
    {
        if (!selectedPrefab) // selected prefab is null, probably due to some other mod nonsense
        {
            return selectedPrefab;
        }

        if (!MVPrefabManager.IsPatchedByMVBP(selectedPrefab))
        {
            // ignore pieces not touched by this mod
            return UnityEngine.Object.Instantiate(selectedPrefab);
        }

        bool setActive = false;

        if (selectedPrefab.HasAnyComponent(ComponentsToDisable))
        {
            setActive = selectedPrefab.activeSelf;
            selectedPrefab.SetActive(false);
        }

        // Handle selection of non-standard prefabs (like in InfinityHammer)
        string prefabName = selectedPrefab.GetPrefabName();

        GameObject clonedPrefab = UnityEngine.Object.Instantiate(selectedPrefab);

        if (PrefabConfigManager.NeedsOffsetPatch(prefabName) && selectedPrefab.transform.localPosition != Vector3.zero)
        {
            clonedPrefab.transform.Translate(-1 * selectedPrefab.transform.localPosition); // fix pieces with bullshit offsets
        }

        if (PrefabConfigManager.NeedsCollisionPatchForGhost(prefabName))
        {
            // Needed to make some things work, like Stalagmite, blackmarble_corner_stair, silvervein, etc.
            ColliderManager.PatchCollider(clonedPrefab);
        }
        
        //if (clonedPrefab.TryGetComponent(out LODGroup lodGroup))
        //{
        //    lodGroup.GetRelativeHeight(GameCamera.instance.m_camera);
        //}

        if (!setActive)
        {
            return clonedPrefab;
        }

        selectedPrefab.SetActive(true);

        foreach (Type componentType in ComponentsToDisable)
        {
            if (componentType == typeof(Humanoid) && clonedPrefab.TryGetComponent(out Humanoid humanoid))
            {
                humanoid.m_defaultItems = Array.Empty<GameObject>();
                humanoid.m_randomWeapon = Array.Empty<GameObject>();
                humanoid.m_randomArmor ??= Array.Empty<GameObject>();
                humanoid.m_randomShield ??= Array.Empty<GameObject>();
                humanoid.m_randomSets ??= Array.Empty<Humanoid.ItemSet>();
            }
            else if (clonedPrefab.TryGetComponent(componentType, out Component component))
            {
                UnityEngine.Object.DestroyImmediate(component);
            }
        }
        clonedPrefab.SetActive(true);

        return clonedPrefab;
    }


    public static float GetRelativeHeight(this LODGroup lodGroup, Camera camera)
    {
        float magnitude = (lodGroup.transform.TransformPoint(lodGroup.localReferencePoint) - camera.transform.position).magnitude;
        return DistanceToRelativeHeight(camera, magnitude, lodGroup.GetWorldSpaceSize()) * QualitySettings.lodBias;
    }
    public static float DistanceToRelativeHeight(Camera camera, float distance, float size)
    {
        bool orthographic = camera.orthographic;
        float result;
        if (orthographic)
        {
            result = size * 0.5f / camera.orthographicSize;
        }
        else
        {
            float num = Mathf.Tan(0.017453292f * camera.fieldOfView * 0.5f);
            float num2 = size * 0.5f / (distance * num);
            result = num2;
        }
        return result;
    }

    public static float GetWorldSpaceSize(this LODGroup lodGroup)
    {
        return GetWorldSpaceScale(lodGroup.transform) * lodGroup.size;
    }

    private static float GetWorldSpaceScale(Transform t)
    {
        Vector3 lossyScale = t.lossyScale;
        float a = Mathf.Abs(lossyScale.x);
        a = Mathf.Max(a, Mathf.Abs(lossyScale.y));
        return Mathf.Max(a, Mathf.Abs(lossyScale.z));
    }


    /// <summary>
    ///     Offset center of prefabs with poor center locations.
    /// </summary>
    /// <param name="__instance"></param>
    [HarmonyPostfix]
    [HarmonyBefore("Searica.Valheim.ExtraSnapPointsMadeEasy")]
    [HarmonyPatch(typeof(Player), nameof(Player.UpdatePlacementGhost))]
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
            prefabConfig.PlacementOffset.Value != Vector3.zero)
        {
            Quaternion quaternion = __instance.m_placementGhost.transform.rotation;
            Vector3 pos = __instance.m_placementGhost.transform.position;
            __instance.m_placementGhost.transform.position = pos + (quaternion * prefabConfig.PlacementOffset.Value);
        }
    }


    /// <summary>
    ///     Patch pieces upon placement. Patch piece right after placement 
    ///     to synchronize the positions and rotations of otherwise non-persistent objects.
    /// </summary>
    /// <param name="__instance"></param>
    [HarmonyPrefix]
    [HarmonyPatch(typeof(Piece), nameof(Piece.SetCreator))]
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
