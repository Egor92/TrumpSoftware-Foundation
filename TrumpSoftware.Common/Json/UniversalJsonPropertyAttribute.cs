using System;

namespace TrumpSoftware.Common.Json
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public class UniversalJsonPropertyAttribute : Attribute
    {
        public string PropertyName { get; private set; }

        public Type PropertyType { get; set; }

        public UniversalJsonPropertyAttribute(string propertyName = null)
        {
            PropertyName = propertyName;
        }
    }
}
