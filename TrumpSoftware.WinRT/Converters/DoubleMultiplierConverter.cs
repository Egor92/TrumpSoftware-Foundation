using System;

namespace TrumpSoftware.WinRT.Converters
{
    public class DoubleMultiplierConverter : MultiplierConverter<double>
    {
        protected override double GetMultipledValue(double value, Func<double, double> mult)
        {
            return mult(value);
        }
    }
}
