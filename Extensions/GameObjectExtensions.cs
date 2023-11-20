using System;
using UnityEngine;

namespace MVBP.Extensions
{
    internal static class GameObjectExtensions
    {
        /// <summary>
        ///     Creates a deep copy of the object.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        internal static GameObject DeepCopy(this GameObject obj)
        {
            // set the object to be inactive to avoid Null Ref Exceptions
            bool setActive = obj.activeSelf;
            obj.SetActive(false);

            var clone = UnityEngine.Object.Instantiate(obj);

            // set object and clone to original state
            obj.SetActive(setActive);
            return clone;
        }

        /// <summary>
        ///     Check if GameObject has any of the specified components.
        /// </summary>
        /// <param name="gameObject"></param>
        /// <param name="components"></param>
        /// <returns></returns>
        public static bool HasAnyComponent(this GameObject gameObject, params Type[] components)
        {
            foreach (var compo in components)
            {
                if (gameObject.GetComponent(compo))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        ///     Check if GameObject has any of the specified components.
        /// </summary>
        /// <param name="gameObject"></param>
        /// <param name="componentNames"></param>
        /// <returns></returns>
        public static bool HasAnyComponent(this GameObject gameObject, params string[] componentNames)
        {
            foreach (var name in componentNames)
            {
                if (gameObject.GetComponent(name))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        ///     Check if GameObject has all of the specified components.
        /// </summary>
        /// <param name="gameObject"></param>
        /// <param name="componentNames"></param>
        /// <returns></returns>
        public static bool HasAllComponents(this GameObject gameObject, params string[] componentNames)
        {
            foreach (var name in componentNames)
            {
                if (!gameObject.GetComponent(name))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        ///     Check if GameObject has all of the specified components.
        /// </summary>
        /// <param name="gameObject"></param>
        /// <param name="components"></param>
        /// <returns></returns>
        public static bool HasAllComponents(this GameObject gameObject, params Type[] components)
        {
            foreach (var compo in components)
            {
                if (!gameObject.GetComponent(compo))
                {
                    return false;
                }
            }
            return true;
        }

        /// <summary>
        ///     Check if GameObject or any of it's children
        ///     have any of the specified components.
        /// </summary>
        /// <param name="gameObject"></param>
        /// <param name="includeInactive"></param>
        /// <param name="components"></param>
        /// <returns></returns>
        public static bool HasAnyComponentInChildren(
            this GameObject gameObject,
            bool includeInactive = false,
            params Type[] components
        )
        {
            foreach (var compo in components)
            {
                if (gameObject.GetComponentInChildren(compo, includeInactive))
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        ///     Extension method to find nested children by name using either
        ///     a breadth-first or depth-first search. Default is breadth-first.
        /// </summary>
        /// <param name="gameObject"></param>
        /// <param name="childName">Name of the child object to search for.</param>
        /// <param name="searchType">Whether to preform a breadth first or depth first search. Default is breadth first.</param>
        public static Transform FindDeepChild(
            this GameObject gameObject,
            string childName,
            global::Utils.IterativeSearchType searchType = global::Utils.IterativeSearchType.BreadthFirst
        )
        {
            return gameObject.transform.FindDeepChild(childName, searchType);
        }

        /// <summary>
        ///     Immediately destroys all instances of a component
        ///     in the GameObject and all of it's children.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="gameObject"></param>
        /// <returns></returns>
        public static void DestroyComponentsInChildren<T>(
            this GameObject gameObject,
            bool includeInactive = false
        ) where T : Component
        {
            var components = gameObject.GetComponentsInChildren<T>(includeInactive);
            foreach (var component in components)
            {
                UnityEngine.Object.DestroyImmediate(component);
            }
        }

        internal static Mesh GetMesh(this GameObject gameObject, string meshName)
        {
            foreach (var meshFilter in gameObject.GetComponentsInChildren<MeshFilter>())
            {
                var mesh = meshFilter.mesh;
                if (mesh == null)
                {
                    continue;
                }

                if (mesh.name.RemoveSuffix("Instance").Trim() == meshName)
                {
                    return mesh;
                }
            }
            Log.LogWarning($"Could not find Mesh: {meshName} for GameObject: {gameObject.name}");
            return null;
        }
    }
}