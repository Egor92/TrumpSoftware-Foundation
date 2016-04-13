namespace TrumpSoftware.Common.Helpers
{
    public static class StructHelper
    {
        public static int Limit(int value, int min, int max)
        {
            return value > max ? max : (value < min ? min : value);
        }

        public static double Limit(double value, double min, double max)
        {
            return value > max ? max : (value < min ? min : value);
        }

        public static float Limit(float value, float min, float max)
        {
            return value > max ? max : (value < min ? min : value);
        }
    }
}
