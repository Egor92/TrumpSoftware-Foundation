namespace TrumpSoftware.WinRT.Converters
{
    public class BoolToDoubleConverter : BoolConverter<double>
    {
        protected override double GetDefaultFalseValue()
        {
            return 0.0;
        }

        protected override double GetDefaultTrueValue()
        {
            return 1.0;
        }
    }
}
