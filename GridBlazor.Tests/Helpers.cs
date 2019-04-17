using System;
using System.Linq;
using System.Reflection;

namespace GridBlazor.Tests
{
    static class Helpers
    {
        public static T GetAttribute<T>(this PropertyInfo pi) where T : Attribute
        {
            return (T)pi.GetCustomAttributes(typeof(T), true).FirstOrDefault();
        }

        public static T GetAttribute<T>(this Type type) where T : Attribute
        {
            return (T)type.GetTypeInfo().GetCustomAttributes(typeof(T), true).FirstOrDefault();
        }
    }
}
