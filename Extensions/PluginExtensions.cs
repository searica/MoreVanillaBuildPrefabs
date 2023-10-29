// Ignore Spelling: MVBP

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace MVBP
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
    }

    internal static class Extensions
    {
        private const BindingFlags AllBindings =
            BindingFlags.Public
            | BindingFlags.NonPublic
            | BindingFlags.Instance
            | BindingFlags.Static
            | BindingFlags.GetField
            | BindingFlags.SetField
            | BindingFlags.GetProperty
            | BindingFlags.SetProperty;

        /// <summary>
        ///     Extension for 'Object' that copies all fields from the source to the object.
        ///     Including private and static fields.
        /// </summary>
        /// <param name="target">The target.</param>
        /// <param name="source">The source.</param>
        public static void CopyFields(this object target, object source)
        {
            // If any this null throw an exception
            if (target == null || source == null)
                throw new Exception("Target or/and Source Objects are null");
            // Getting the Types of the objects
            Type typeTarget = source.GetType();
            Type typeSrc = target.GetType();

            // Iterate over the fields of the source instance and copy
            // them them to their counterparts in the target instance
            FieldInfo[] srcFields = typeSrc.GetFields(AllBindings);
            foreach (FieldInfo srcField in srcFields)
            {
                FieldInfo targetField = typeTarget.GetField(srcField.Name, AllBindings);
                if (targetField == null)
                {
                    continue;
                }
                if (!targetField.IsInitOnly)
                {
                    continue;
                }
                if (!targetField.FieldType.IsAssignableFrom(srcField.FieldType))
                {
                    continue;
                }
                // Passed all tests, lets set the value
                targetField.SetValue(source, srcField.GetValue(target));
            }
        }
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

        internal static string EmptyIfNull(this object value)
        {
            if (value == null)
            {
                return string.Empty;
            }
            return value.ToString();
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