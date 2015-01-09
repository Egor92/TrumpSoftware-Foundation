using System;
using System.Linq;
using System.Reflection;

namespace TrumpSoftware.Xaml.Mvvm
{
    public static class ResetFieldsHelper
    {
        public static void ResetFields(object obj)
        {
            var type = obj.GetType();
            var runtimeFields = type.GetRuntimeFields();
            var fieldsForReset = runtimeFields.Where(x => x.IsDefined(typeof (ResetWhenNavigatingAttribute)));
            foreach (var fieldInfo in fieldsForReset)
            {
                var resetWhenNavigatingAttribute = fieldInfo.GetCustomAttribute<ResetWhenNavigatingAttribute>();
                var defaultValue = resetWhenNavigatingAttribute.ToUseCustomDefaultValue
                    ? resetWhenNavigatingAttribute.CustomDefaultValue
                    : GetDefaultValue(fieldInfo.FieldType);
                fieldInfo.SetValue(obj, defaultValue);
            }
        }

        private static object GetDefaultValue(Type type)
        {
            var typeInfo = type.GetTypeInfo();
            if (typeInfo.IsClass || typeInfo.IsInterface)
                return null;
            return Activator.CreateInstance(type);
        }
    }
}
