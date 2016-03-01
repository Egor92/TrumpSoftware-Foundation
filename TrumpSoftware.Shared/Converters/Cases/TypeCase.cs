using System;
#if WINRT
using TrumpSoftware.WinRT.Extensions;
#endif

#if WPF
namespace TrumpSoftware.Wpf.Converters.Cases
#elif WINRT

namespace TrumpSoftware.WinRT.Converters.Cases
#endif
{
    public class TypeCase : ICase
    {
        public Type Type { get; set; }

        public bool IsStrictly { get; set; }

        public object Value { get; set; }

        public bool IsMatched(object value)
        {
            if (Type == null || value == null)
                return false;
            if (IsStrictly)
            {
                return Type == value.GetType();
            }
            else
            {
                return Type.IsInstanceOfType(value);
            }
        }
    }
}