namespace TrumpSoftware.Common
{
    /// <summary>
    /// Представляет вещественное число от 0 до 1
    /// </summary>
    public struct FractionalOne
    {
        private readonly double _value;

        public FractionalOne(double value = 0)
        {
            if (value > 1.0)
                _value = 1.0;
            else if (value < 0.0)
                _value = 0.0;
            else
                _value = value;
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

        public override string ToString()
        {
            return _value.ToString();
        }
    }
}
