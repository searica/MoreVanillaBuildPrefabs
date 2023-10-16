using System;
using System.Reflection;

namespace MoreVanillaBuildPrefabs.Helpers
{
    internal static class ReflectionUtils
    {
        public const BindingFlags AllBindings =
            BindingFlags.Public
            | BindingFlags.NonPublic
            | BindingFlags.Instance
            | BindingFlags.Static
            | BindingFlags.GetField
            | BindingFlags.SetField
            | BindingFlags.GetProperty
            | BindingFlags.SetProperty;

        internal static MethodInfo GetMethod(Type type, string name, int genericParameterCount, Type[] types)
        {
            foreach (MethodInfo method in type.GetMethods(AllBindings))
            {
                if (method.Name == name
                    && HasMatchingParameterTypes(genericParameterCount, types, method.GetParameters()))
                {
                    return method;
                }
            }

            return default;
        }

        internal static MethodInfo GetGenericMethod(Type type, string name, int genericParameterCount, Type[] types)
        {
            foreach (MethodInfo method in type.GetMethods(AllBindings))
            {
                if (method.IsGenericMethod
                    && method.ContainsGenericParameters
                    && method.Name == name
                    && HasMatchingParameterTypes(genericParameterCount, types, method.GetParameters()))
                {
                    return method;
                }
            }

            return default;
        }

        private static bool HasMatchingParameterTypes(int genericParameterCount, Type[] types, ParameterInfo[] parameters)
        {
            if (parameters.Length < genericParameterCount || parameters.Length != types.Length)
            {
                return false;
            }

            int count = 0;

            for (int i = 0; i < parameters.Length; i++)
            {
                if (parameters[i].ParameterType.IsGenericParameter)
                {
                    count++;
                }
                else if (types[i] != parameters[i].ParameterType)
                {
                    return false;
                }
            }

            if (count != genericParameterCount)
            {
                return false;
            }

            return true;
        }

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
}