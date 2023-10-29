using System;
using System.Reflection;

namespace MVBP.Helpers
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

        internal static MethodInfo GetMethod(Type type, string name, Type[] types)
        {
            foreach (MethodInfo method in type.GetMethods(AllBindings))
            {
                if (method.Name == name
                    && HasMatchingParameterTypes(0, types, method.GetParameters()))
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
    }
}