using System;
using System.Globalization;

#if WPF
using TrumpSoftware.Wpf.Converters.Enums;
#elif WINRT
using TrumpSoftware.WinRT.Converters.Enums;
#endif

#if WPF
namespace TrumpSoftware.Wpf.Converters
#elif WINRT
namespace TrumpSoftware.WinRT.Converters
#endif
{
    public abstract class MultiBoolConverter : ChainMultiConverter<bool, bool>
    {
        public BinaryLogicalOperation Operation { get; set; }

        protected override bool Convert(bool[] values, Type targetType, object parameter, CultureInfo culture)
        {
            return Operation.ApplyOperationFor(values);
        }

        protected override bool[] ConvertBack(bool value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}