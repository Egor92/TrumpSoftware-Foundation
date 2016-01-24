using System;
using System.Reflection;

namespace TrumpSoftware.Common.Extensions
{
    public static class TypeExtensions
    {
        public static object GetDefault(this Type type)
        {
            if (type == null)
                throw new ArgumentNullException("type");
            if (type.GetTypeInfo().IsValueType)
                return Activator.CreateInstance(type);
            return null;
        }
    }
}
