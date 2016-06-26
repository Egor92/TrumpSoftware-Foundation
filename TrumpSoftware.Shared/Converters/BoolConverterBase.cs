using TrumpSoftware.Common;
using System;
#if WPF
using CultureArgumentType = System.Globalization.CultureInfo;
#elif WINRT
using CultureArgumentType = System.String;
#endif

#if WPF
namespace TrumpSoftware.Wpf.Converters
#elif WINRT
namespace TrumpSoftware.WinRT.Converters
#endif
{
    public abstract class BoolConverterBase<T> : ChainConverter<bool,T>
    {
        #region TrueValue

        private readonly Instance<T> _trueValueInstance = new Instance<T>();

        public T TrueValue
        {
            get
            {
                if (!_trueValueInstance.IsInitialized)
                {
                    var defaultTrueValue = GetDefaultTrueValue();
                    _trueValueInstance.SetValue(defaultTrueValue);
                }
                return _trueValueInstance.Value;

            }
            set
            {
                _trueValueInstance.SetValue(value);
            }
        }

        protected virtual T GetDefaultTrueValue()
        {
            return default (T);
        }

        #endregion

        #region FalseValue

        private readonly Instance<T> _falseValueInstance = new Instance<T>();

        public T FalseValue
        {
            get
            {
                if (!_falseValueInstance.IsInitialized)
                {
                    var defaultFalseValue = GetDefaultFalseValue();
                    _falseValueInstance.SetValue(defaultFalseValue);
                }
                return _falseValueInstance.Value;
            }
            set
            {
                _falseValueInstance.SetValue(value);
            }
        }

        protected virtual T GetDefaultFalseValue()
        {
            return default (T);
        }

        #endregion

        #region Overridden members

        protected override sealed T Convert(bool value, Type targetType, object parameter, CultureArgumentType cultureArgument)
        {
            return ConvertFromBool(value);
        }

        protected override sealed bool ConvertBack(T value, Type targetType, object parameter, CultureArgumentType cultureArgument)
        {
            return ConvertToBool(value);
        }

        #endregion

        private T ConvertFromBool(bool value)
        {
            return value
                ? TrueValue
                : FalseValue;
        }

        private bool ConvertToBool(T value)
        {
            return value.Equals(TrueValue);
        }
    }
}
