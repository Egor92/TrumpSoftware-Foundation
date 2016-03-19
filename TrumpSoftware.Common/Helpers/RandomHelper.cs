using System;
using System.Collections.Generic;
using System.Linq;

namespace TrumpSoftware.Common.Helpers
{
    public static class RandomHelper
    {
        #region Fields

        private static readonly Random Random;

        #endregion

        #region Ctor

        static RandomHelper()
        {
            var seed = (int) (DateTime.Now.ToFileTime()%int.MaxValue);
            Random = new Random(seed);
        }

        #endregion

        public static bool GetBool(double trueChance = 0.5)
        {
            trueChance = NumberHelper.Limit(trueChance, 0.0, 1.0);
            return Random.NextDouble() < trueChance;
        }

        public static int GetInt()
        {
            return GetInt(0, int.MaxValue - 1);
        }

        public static int GetInt(int max)
        {
            return GetInt(0, max);
        }

        public static int GetInt(int min, int max)
        {
            return Random.Next(min, max + 1);
        }

        public static double GetDouble(double min, double max)
        {
            return min + (max - min)*Random.NextDouble();
        }

        public static char GetChar(char min, char max)
        {
            return (char) GetInt(min, max);
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
        public static T GetValue<T>(IDictionary<T, double> probabilitiesByValue)
        {
            if (probabilitiesByValue.Values.Any(x => x < 0.0))
                throw new ArgumentException("Values must be not negative", "probabilitiesByValue");
            var max = probabilitiesByValue.Values.Sum();
            if (!(max > 0.0))
                throw new ArgumentException("Summa of values must be positive", "probabilitiesByValue");
            var random = GetDouble(0.0, max);
            var probabilitySum = 0.0;
            foreach (var pair in probabilitiesByValue)
            {
                var value = pair.Key;
                var probability = pair.Value;

                probabilitySum += probability;
                if (probabilitySum > random)
                    return value;
            }
            throw new Exception("Algorithm fail");
        }
    }
}
