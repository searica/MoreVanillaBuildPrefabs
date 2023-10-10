using Jotunn.Configs;
using Jotunn.Managers;
using MoreVanillaBuildPrefabs.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace MoreVanillaBuildPrefabs
{

    public static class TypeExtensions
    {
        public static List<T> GetAllPublicConstantValues<T>(this Type type)
        {
            return type
                .GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                .Where(fi => fi.IsLiteral && !fi.IsInitOnly && fi.FieldType == typeof(T))
                .Select(x => (T)x.GetRawConstantValue())
                .ToList();
        }

        public static List<T> GetAllPublicStaticValues<T>(this Type type)
        {
            return type
                .GetFields(BindingFlags.Public | BindingFlags.Static)
                .Where(fi => fi.FieldType == typeof(T))
                .Select(x => (T)x.GetValue(null))
                .ToList();
        }
    }

    public static class PluginExtensions
    {
        public static T Ref<T>(this T o) where T : UnityEngine.Object
        {
            return o ? o : null;
        }

        public static void Dispose(this IEnumerable<IDisposable> collection)
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
