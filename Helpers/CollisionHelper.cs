using System.Collections.Generic;
using UnityEngine;

namespace MVBP.Helpers
{
    internal class CollisionHelper
    {
        internal static void AddBoxCollider(GameObject prefab, Vector3 center, Vector3 size)
        {
            var collider = prefab.AddComponent<BoxCollider>();
            collider.center = center;
            collider.size = size;
        }

        internal static void PatchCollider(GameObject prefab)
        {
            // Needed to make some things work, like Stalagmite, Rock_destructible, Rock_7, silvervein, etc.
            Bounds desiredBounds = new();
            foreach (Renderer renderer in prefab.GetComponentsInChildren<Renderer>())
            {
                desiredBounds.Encapsulate(renderer.bounds);
            }
            AddBoxCollider(prefab, desiredBounds.center, desiredBounds.size);
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
}