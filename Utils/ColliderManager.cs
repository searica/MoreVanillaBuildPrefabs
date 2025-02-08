// Ignore Spelling: MVBP

using System.Collections.Generic;
using UnityEngine;

namespace MVBP.Utils;

internal static class ColliderManager
{
    /// <summary>
    ///     Adds a BoxCollider to a GameObject based on the properties
    ///     provided.
    /// </summary>
    /// <param name="prefab">GameObject to add the BoxCollider too.</param>
    /// <param name="center">The center of the BoxCollider.</param>
    /// <param name="size">The size of the BoxCollider.</param>
    /// <param name="local">If True then the center and size are
    /// assumed to already be relative to the GameObject, if false
    /// then they are transformed into the local space or the GameObject
    /// .</param>
    internal static void AddBoxCollider(GameObject prefab, Vector3 center, Vector3 size, bool local = true)
    {
        BoxCollider collider = prefab.AddComponent<BoxCollider>();
        if (local)
        {
            collider.center = center;
            collider.size = size;
        }
        else
        {
            collider.center = prefab.transform.InverseTransformPoint(center);
            collider.size = prefab.transform.InverseTransformPoint(size);
        }

    }


    /// <summary>
    ///     Adds a box collider to the prefab based on rendering bounds
    ///     of the object
    /// </summary>
    /// <param name="prefab"></param>
    internal static void PatchCollider(GameObject prefab)
    {
        // Needed to make some things work, like Stalagmite, Rock_destructible, Rock_7, silvervein, etc.

        // Renderer bounds include animations in some cases
        // var desiredBounds = GetRendererBounds(prefab);

        // Try just using the mesh bounds instead
        Bounds desiredBounds = GetMeshBounds(prefab);
        AddBoxCollider(
            prefab,
            desiredBounds.center,
            desiredBounds.size,
            local: true
        );
    }


    /// <summary>
    ///     Creates a bounding box that encapsulating the bounds of
    ///     all Renderer components attached to the GameObject
    /// </summary>
    /// <param name="prefab"></param>
    /// <returns></returns>
    internal static Bounds GetRendererBounds(GameObject prefab)
    {
        Bounds desiredBounds = new();
        foreach (Renderer renderer in prefab.GetComponentsInChildren<Renderer>())
        {
            desiredBounds.Encapsulate(renderer.bounds);
        }
        return desiredBounds;
    }


    /// <summary>
    ///     Creates a bounding box that encapsulating the bounds of
    ///     all Meshes attached to the GameObject via MeshRenderer or
    ///     SkinnedMeshRenderer components
    /// </summary>
    /// <param name="prefab"></param>
    /// <returns></returns>
    internal static Bounds GetMeshBounds(GameObject prefab)
    {
        Bounds desiredBounds = new();
        foreach (MeshRenderer meshRenderer in prefab.GetComponentsInChildren<MeshRenderer>())
        {
            // Uses world space bounds
            desiredBounds.Encapsulate(meshRenderer.bounds);
        }

        // Need to use mesh bounds here rather than skinned mesh renderer
        // bounds those bounds include animations and can be much larger
        foreach (SkinnedMeshRenderer skinMeshRender in prefab.GetComponentsInChildren<SkinnedMeshRenderer>())
        {
            // Convert max and min points of local bounds into world space
            Bounds bounds = skinMeshRender.sharedMesh.bounds;
            Transform trans = skinMeshRender.transform;
            Vector3 maxPoint = trans.TransformPoint(bounds.max);
            Vector3 minPoint = trans.TransformPoint(bounds.min);

            desiredBounds.Encapsulate(maxPoint);
            desiredBounds.Encapsulate(minPoint);
        }
        return desiredBounds;
    }


    /// <summary>
    ///     Creates a bounding box that encapsulating the bounds of
    ///     all Colliders attached to the GameObject that are active
    ///     and not trigger colliders
    /// </summary>
    /// <param name="prefab"></param>
    /// <returns></returns>
    internal static Bounds GetColliderBounds(GameObject prefab)
    {
        Bounds desiredBounds = new();
        foreach (Collider collider in GetAllColliders(prefab))
        {
            desiredBounds.Encapsulate(collider.bounds);
        }
        return desiredBounds;
    }


    internal static void RemoveColliders(GameObject prefab)
    {
        Collider[] colliders = prefab.GetComponentsInChildren<Collider>();
        foreach (Collider collider in colliders)
        {
            Object.DestroyImmediate(collider);
        }
    }

    internal static Vector3 GetCenter(GameObject prefab)
    {
        List<Collider> allColliders = GetAllColliders(prefab);
        Vector3 localCenter = Vector3.zero;
        foreach (Collider item in allColliders)
        {
            if ((bool)item)
            {
                localCenter += item.bounds.center;
            }
        }
        localCenter /= allColliders.Count;
        return localCenter;
    }

    internal static List<Collider> GetAllColliders(GameObject prefab)
    {
        Collider[] componentsInChildren = prefab.GetComponentsInChildren<Collider>();
        List<Collider> colliders = new()
        {
            Capacity = componentsInChildren.Length
        };
        Collider[] array = componentsInChildren;
        foreach (Collider collider in array)
        {
            if (collider.enabled && collider.gameObject.activeInHierarchy && !collider.isTrigger)
            {
                colliders.Add(collider);
            }
        }
        return colliders;
    }
}
