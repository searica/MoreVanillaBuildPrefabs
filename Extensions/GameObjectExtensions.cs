using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        ///     Extension method to check if GameObject has a component.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="gameObject"></param>
        /// <returns></returns>
        public static bool HasComponent<T>(this GameObject gameObject) where T : UnityEngine.Component
        {
            return gameObject.GetComponent<T>() != null;
        }

        /// <summary>
        ///     Extension method to check if GameObject has a component.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="gameObject"></param>
        /// <returns></returns>
        public static bool HasComponent(this GameObject gameObject, string componentName)
        {
            return gameObject.GetComponent(componentName) != null;
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
        ) where T : UnityEngine.Component
        {
            var components = gameObject.GetComponentsInChildren<T>(includeInactive);
            foreach (var component in components)
            {
                UnityEngine.Object.DestroyImmediate(component);
            }
        }

        /// <summary>
        ///     Check if GameObject has any of the specified components.
        /// </summary>
        /// <param name="gameObject"></param>
        /// <param name="components"></param>
        /// <returns></returns>
        public static bool HasAnyComponent(
            this GameObject gameObject,
            params Type[] components
        )
        {
            foreach (var compo in components)
            {
                if (gameObject.GetComponent(compo) != null)
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
        /// <param name="components"></param>
        /// <returns></returns>
        public static bool HasAnyComponent(
            this GameObject gameObject,
            params string[] componentNames
        )
        {
            foreach (var name in componentNames)
            {
                if (gameObject.GetComponent(name) != null)
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
        /// <param name="components"></param>
        /// <returns></returns>
        public static bool HasAllComponents(
            this GameObject gameObject,
            params string[] componentNames
        )
        {
            foreach (var name in componentNames)
            {
                if (gameObject.GetComponent(name) == null)
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
        public static bool HasAllComponents(
            this GameObject gameObject,
            params Type[] components
        )
        {
            foreach (var compo in components)
            {
                if (gameObject.GetComponent(compo) == null)
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
                if (gameObject.GetComponentInChildren(compo, includeInactive) != null)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        ///     Check if GameObject or any of it's children
        ///     have the specific component.
        /// </summary>
        /// <param name="gameObject"></param>
        /// <param name="components"></param>
        /// <returns></returns>
        public static bool HasComponentInChildren<T>(
            this GameObject gameObject,
            bool includeInactive = false
        ) where T : UnityEngine.Component
        {
            return gameObject.GetComponentInChildren<T>(includeInactive) != null;
        }

        /// <summary>
        ///     Extension method to get the first component in the GameObject
        ///     or it's children that has the specified name.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="gameObject"></param>
        /// <param name="name"></param>
        /// <param name="includeInactive"></param>
        /// <returns></returns>
        internal static T GetComponentInChildrenByName<T>(
            this GameObject gameObject,
            string name,
            bool includeInactive = false
        ) where T : UnityEngine.Component
        {
            foreach (
                var compo in gameObject.GetComponentsInChildren<T>(includeInactive)
            )
            {
                if (compo.name == name)
                {
                    return compo;
                }
            }
            Log.LogWarning(
                $"No {nameof(T)} with name {name} found for GameObject: {gameObject.name}"
            );
            return null;
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