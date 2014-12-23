using System;

namespace TrumpSoftware.Xaml.Mvvm
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class ResetWhenNavigatingAttribute : Attribute
    {
        public object CustomDefaultValue { get; private set; }

        public bool ToUseCustomDefaultValue { get; private set; }

        public ResetWhenNavigatingAttribute()
        {
            ToUseCustomDefaultValue = false;
        }

        public ResetWhenNavigatingAttribute(object customDefaultValue)
        {
            CustomDefaultValue = customDefaultValue;
            ToUseCustomDefaultValue = true;
        }
    }
}
