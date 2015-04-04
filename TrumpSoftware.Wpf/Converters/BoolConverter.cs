using System;
using System.Globalization;
using TrumpSoftware.WPF.Converters;

namespace TrumpSoftware.Wpf.Converters
{
    public class BoolConverter<T> : ChainConverter<bool,T>
    {
        private bool _isTrueValueInitialized;
        private bool _isFalseValueInitialized;
        private T _trueValue;
        private T _falseValue;

        public T TrueValue
        {
            get
            {
                if (!_isTrueValueInitialized)
                {
                    _trueValue = GetDefaultTrueValue();
                    _isTrueValueInitialized = true;
                }
                return _trueValue;

            }
            set 
            { 
                _trueValue = value;
                _isTrueValueInitialized = true;
            }
        }

        public T FalseValue
        {
            get
            {
                if (!_isFalseValueInitialized)
                {
                    _falseValue = GetDefaultFalseValue();
                    _isFalseValueInitialized = true;
                }
                return _falseValue;

            }
            set
            {
                _falseValue = value;
                _isFalseValueInitialized = true;
            }
        }

        protected virtual T GetDefaultTrueValue()
        {
            return default (T);
        }

        protected virtual T GetDefaultFalseValue()
        {
            return default (T);
        }

        protected override sealed T Convert(bool value, Type targetType, object parameter, CultureInfo culture)
        {
            return ConvertFromBool(value);
        }

        protected override sealed bool ConvertBack(T value, Type targetType, object parameter, CultureInfo culture)
        {
            return ConvertToBool(value);
        }

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
