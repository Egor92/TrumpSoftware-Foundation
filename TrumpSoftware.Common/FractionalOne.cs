using System;
using TrumpSoftware.Common.Helpers;

namespace TrumpSoftware.Common
{
    /// <summary>
    /// Представляет вещественное число от 0 до 1
    /// </summary>
    public struct FractionalOne : IComparable<FractionalOne>
    {
        private readonly double _value;

        public static FractionalOne MinValue = new FractionalOne(0.0);
        public static FractionalOne MaxValue = new FractionalOne(1.0);

        public FractionalOne(double value = 0)
        {
            _value = StructHelper.Limit(value, 0.0, 1.0);
        }

        public static implicit operator double(FractionalOne fractionalOne)
        {
            return fractionalOne._value;
        }

        public static explicit operator FractionalOne(double d)
        {
            return new FractionalOne(d);
        }

        public static FractionalOne operator +(FractionalOne a, FractionalOne b)
        {
            return new FractionalOne(a._value + b._value);
        }

        public static FractionalOne operator +(FractionalOne a, double b)
        {
            return new FractionalOne(a._value + b);
        }

        public static FractionalOne operator +(double a, FractionalOne b)
        {
            return new FractionalOne(a + b._value);
        }

        public static FractionalOne operator -(FractionalOne a, FractionalOne b)
        {
            return new FractionalOne(a._value - b._value);
        }

        public static FractionalOne operator -(FractionalOne a, double b)
        {
            return new FractionalOne(a._value - b);
        }

        public static FractionalOne operator -(double a, FractionalOne b)
        {
            return new FractionalOne(a - b._value);
        }

        public static FractionalOne operator *(FractionalOne a, FractionalOne b)
        {
            return new FractionalOne(a._value * b._value);
        }

        public static FractionalOne operator *(FractionalOne a, double b)
        {
            return new FractionalOne(a._value * b);
        }

        public static FractionalOne operator *(double a, FractionalOne b)
        {
            return new FractionalOne(a * b._value);
        }

        public static FractionalOne operator /(FractionalOne a, FractionalOne b)
        {
            return new FractionalOne(a._value / b._value);
        }

        public static FractionalOne operator /(FractionalOne a, double b)
        {
            return new FractionalOne(a._value / b);
        }

        public static FractionalOne operator /(double a, FractionalOne b)
        {
            return new FractionalOne(a / b._value);
        }

        public int CompareTo(FractionalOne other)
        {
            return _value.CompareTo(other._value);
        }

        public override string ToString()
        {
            return _value.ToString();
        }

        public string ToString(string format)
        {
            return _value.ToString(format);
        }
    }
}
