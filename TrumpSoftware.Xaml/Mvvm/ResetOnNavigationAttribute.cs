using System;

namespace TrumpSoftware.Xaml.Mvvm
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class ResetOnNavigationAttribute : Attribute
    {
        public object CustomDefaultValue { get; private set; }

        internal bool ToUseCustomDefaultValue { get; private set; }

        public ResetOnNavigationAttribute()
        {
            ToUseCustomDefaultValue = false;
        }

        public ResetOnNavigationAttribute(object customDefaultValue)
        {
            CustomDefaultValue = customDefaultValue;
            ToUseCustomDefaultValue = true;
        }
    }
}
