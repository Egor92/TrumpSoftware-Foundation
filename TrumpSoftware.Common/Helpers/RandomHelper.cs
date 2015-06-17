using System;

namespace TrumpSoftware.Common.Helpers
{
    public static class RandomHelper
    {
        private static Random _random;

        private static Random Random
        {
            get { return _random ?? (_random = new Random(GetSeed())); }
        }

        private static int GetSeed()
        {
            return (int)(DateTime.Now.ToFileTime() % int.MaxValue);
        }

        public static bool GetBool(double trueChance = 0.5)
        {
            trueChance = trueChance > 1.0 ? 1.0 : (trueChance < 0.0 ? 0.0 : trueChance);
            return Random.NextDouble() < trueChance;
        }

        public static int GetInt(int max)
        {
            return GetInt(0, max);
        }

        public static int GetInt(int min, int max)
        {
            return Random.Next(min, max);
        }

        public static double GetDouble(double min, double max)
        {
            return min + (max - min)*Random.NextDouble();
        }

        public static char GetChar(char min, char max)
        {
            return (char) GetInt(min, max + 1);
        }

        public static byte GetByte()
        {
            return (byte)Random.Next(0, 256);
        }

        public static TimeSpan GetTimeSpan(TimeSpan min, TimeSpan max)
        {
            var randMilliseconds = GetDouble(min.TotalMilliseconds, max.TotalMilliseconds);
            return TimeSpan.FromMilliseconds(randMilliseconds);
        }
    }
}
