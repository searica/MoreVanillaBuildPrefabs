// Ignore Spelling: MVBP

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace MVBP.Extensions
{
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

    internal static class GenericExtensions
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

        internal static T Ref<T>(this T o) where T : UnityEngine.Object
        {
            return o ? o : null;
        }
    }

    internal static class IEnumerableExtensions
    {
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