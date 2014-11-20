using System;
using System.Globalization;
using TrumpSoftware.WPF.Converters;

namespace TrumpSoftware.Wpf.Converters
{
    public class BoolConverter<T> : ChainConverter
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

        protected override sealed object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is bool))
                return null;
            var b = (bool)value;
            return ConvertFromBool(b);
        }

        protected override sealed object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (!(value is T))
                return null;
            var t = (T)value;
            return ConvertToBool(t);
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
