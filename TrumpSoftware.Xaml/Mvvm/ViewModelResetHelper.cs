using System;
using System.Linq;
using System.Reflection;

namespace TrumpSoftware.Xaml.Mvvm
{
    public static class ViewModelResetHelper
    {
        public static void ResetFields(ViewModelBase viewModel)
        {
            var type = viewModel.GetType();
            var runtimeFields = type.GetRuntimeFields();
            var fieldsForReset = runtimeFields.Where(x => x.IsDefined(typeof (ResetWhenNavigatingAttribute)));
            foreach (var fieldInfo in fieldsForReset)
            {
                var resetWhenNavigatingAttribute = fieldInfo.GetCustomAttribute<ResetWhenNavigatingAttribute>();
                var defaultValue = resetWhenNavigatingAttribute.ToUseCustomDefaultValue
                    ? resetWhenNavigatingAttribute.CustomDefaultValue
                    : GetDefaultValue(fieldInfo.FieldType);
                fieldInfo.SetValue(viewModel, defaultValue);
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
