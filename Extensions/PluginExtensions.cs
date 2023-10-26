using Jotunn.Configs;
using MoreVanillaBuildPrefabs.Helpers;
using MoreVanillaBuildPrefabs.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace MoreVanillaBuildPrefabs
{
    internal static class PickableExtensions
    {
        /// <summary>
        ///     Get the amount of the resource that will be returned when it is picked,
        ///     based on current world modifier settings for scaling resources
        /// </summary>
        /// <param name="pickable"></param>
        /// <returns></returns>
        internal static int GetScaledDropAmount(this Pickable pickable)
        {
            if (Game.instance == null)
            {
                return pickable.m_amount;
            }
            return pickable.m_dontScale ? pickable.m_amount : Mathf.Max(pickable.m_minAmountScaled, Game.instance.ScaleDrops(pickable.m_itemPrefab, pickable.m_amount));
        }
    }

    internal static class TransformExtensions
    {
        /// <summary>
        ///     Extension method to find nested children by name using either
        ///     a breadth-first or depth-first search. Default is breadth-first.
        /// </summary>
        /// <param name="transform"></param>
        /// <param name="childName"></param>
        /// <returns></returns>
        public static Transform FindDeepChild(
            this Transform transform,
            string childName,
            bool breadthFirst = true
        )
        {
            if (breadthFirst)
            {
            Queue<Transform> queue = new();
            queue.Enqueue(transform);
            while (queue.Count > 0)
            {
                var child = queue.Dequeue();
                if (child.name == childName)
                {
                    return child;
                }

                foreach (Transform t in child)
                {
                    queue.Enqueue(t);
                }
            }
            return null;
        }
            else
            {
                foreach (Transform child in transform)
                {
                    if (child.name == childName)
                    {
                        return child;
    }
                    var result = child.FindDeepChild(childName);
                    if (result != null)
                    {
                        return result;
                    }
                }
                return null;
            }
        }
    }

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

        public static bool HasComponent<T>(this GameObject go) where T : Component
        {
            return go.GetComponent<T>() != null;
        }

        public static void DestroyComponent<T>(this GameObject go) where T : UnityEngine.Component
        {
            var components = go.GetComponentsInChildren<T>();
            foreach (var component in components)
            {
                UnityEngine.Object.DestroyImmediate(component);
            }
        }

        public static bool HasAnyComponent(this GameObject go, params string[] componentNames)
        {
            return componentNames.Any(component => go.GetComponent(component) != null);
        }

        public static bool HasAnyComponentInChildren(this GameObject go, params Type[] components)
        {
            return components.Any(component => go.GetComponentInChildren(component, true) != null);
        }

        public static bool HasComponentInChildren<T>(this GameObject go, bool includeInactive = false) where T : Component
        {
            return go.GetComponentInChildren<T>(includeInactive) != null;
        }

        public static bool HasAllComponents(this GameObject go, params string[] componentNames)
        {
            return componentNames.All(component => go.GetComponent(component) != null);
        }

        /// <summary>
        ///     Extension method to return the first component
        ///     found while searching for components in children
        ///     that has the specified name.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="gameObject"></param>
        /// <param name="name"></param>
        /// <param name="includeInactive"></param>
        /// <returns></returns>
        internal static T GetComponentInChildren<T>(
            this GameObject gameObject,
            string name,
            bool includeInactive = false
        ) where T : Component
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

                if (NameHelper.RemoveSuffix(mesh.name, "Instance").Trim() == meshName)
                {
                    return mesh;
                }
            }
            Log.LogWarning($"Could not find Mesh: {meshName} for GameObject: {gameObject.name}");
            return null;
        }
    }

    internal static class TypeExtensions
    {
        internal static List<T> GetAllPublicConstantValues<T>(this Type type)
        {
            return type
                .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                .Where(fi => fi.IsLiteral && !fi.IsInitOnly && fi.FieldType == typeof(T))
                .Select(x => (T)x.GetRawConstantValue())
                .ToList();
        }

        internal static List<T> GetAllPublicStaticValues<T>(this Type type)
        {
            return type
                .GetFields(BindingFlags.Public | BindingFlags.Static)
                .Where(fi => fi.FieldType == typeof(T))
                .Select(x => (T)x.GetValue(null))
                .ToList();
        }
    internal static class StringExtensions
    {
        internal static bool ContainsAny(this string str, params string[] substrings)
        {
            foreach (var substring in substrings)
            {
                if (str.Contains(substring))
                {
                    return true;
                }
            }
            return false;
        }

        internal static bool EndsWithAny(this string str, params string[] suffixes)
        {
            foreach (var substring in suffixes)
            {
                if (str.EndsWith(substring))
                {
                    return true;
                }
            }
            return false;
        }

        internal static bool StartsWithAny(this string str, params string[] suffixes)
        {
            foreach (var substring in suffixes)
            {
                if (str.StartsWith(substring))
                {
                    return true;
                }
            }
            return false;
        }

        internal static string RemoveSuffix(this string s, string suffix)
        {
            if (s.EndsWith(suffix))
            {
                return s.Substring(0, s.Length - suffix.Length);
            }

            return s;
    }

        internal static string RemovePrefix(this string s, string prefix)
        {
            if (s.StartsWith(prefix))
    {
                return s.Substring(prefix.Length, s.Length - prefix.Length);
            }
            return s;
        }

        internal static string CapitalizeFirstLetter(this string s)
        {
            if (s.Length == 0)
                return s;
            else if (s.Length == 1)
                return $"{char.ToUpper(s[0])}";
            else
                return char.ToUpper(s[0]) + s.Substring(1);
        }
    }

    internal static class PluginExtensions
    {
        internal static T Ref<T>(this T o) where T : UnityEngine.Object
        {
            return o ? o : null;
        }

        internal static void Dispose(this IEnumerable<IDisposable> collection)
        {
            foreach (IDisposable item in collection)
            {
                if (item != null)
                {
                    try
                    {
                        item.Dispose();
                    }
                    catch (Exception)
                    {
                        Log.LogWarning("Could not dispose of item");
                    }
                }
            }
        }
    }
}