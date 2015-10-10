using System;
using System.Linq;

namespace TrumpSoftware.DotNet.Helpers
{
    public static class ConsoleHelper
    {
        #region Fields

        private delegate bool TryParseFunc<T>(string s, out T result);
        private const string IntegerParseErrorMessage = "Input text is not integer";
        private const string DoubleParseErrorMessage = "Input text is not double";

        #endregion

        #region GetInteger

        public static int GetInteger()
        {
            return GetValue<int>(int.TryParse, x => true, IntegerParseErrorMessage, null);
        }

        public static int GetInteger(int from)
        {
            var checkErrorMessage = string.Format("Input value must be in interval [{0};+∞)", from);
            return GetValue<int>(int.TryParse, x => x >= from, IntegerParseErrorMessage, checkErrorMessage);
        }

        public static int GetInteger(int from, int to)
        {
            var checkErrorMessage = string.Format("Input value must be in interval [{0};{1}]", from, to);
            return GetValue<int>(int.TryParse, x => x >= from && x <= to, IntegerParseErrorMessage, checkErrorMessage);
        }

        public static int GetInteger(params int[] availableValues)
        {
            var set = availableValues.Aggregate(string.Empty, (s, i) => string.Format("{0},{1}", s, i));
            set = set.Substring(0, set.Length - 1);
            var checkErrorMessage = string.Format("Input value must be value of the set {{{0}}}", set);
            return GetValue<int>(int.TryParse, availableValues.Contains, IntegerParseErrorMessage, checkErrorMessage);
        }

        #endregion

        #region GetDouble

        public static double GetDouble()
        {
            return GetValue<double>(double.TryParse, x => true, DoubleParseErrorMessage, null);
        }

        public static double GetDouble(double from)
        {
            var checkErrorMessage = string.Format("Input value must be in interval [{0};+∞)", from);
            return GetValue<double>(double.TryParse, x => x >= from, DoubleParseErrorMessage, checkErrorMessage);
        }

        public static double GetDouble(double from, double to)
        {
            var checkErrorMessage = string.Format("Input value must be in interval [{0};{1}]", from, to);
            return GetValue<double>(double.TryParse, x => x >= from && x <= to, DoubleParseErrorMessage, checkErrorMessage);
        }

        #endregion

        private static T GetValue<T>(TryParseFunc<T> tryParseFunc, Func<T, bool> checkFunc, string parseErrorMessage, string checkErrorMessage)
        {
            if (tryParseFunc == null)
                throw new ArgumentNullException("tryParseFunc");
            if (checkFunc == null)
                throw new ArgumentNullException("checkFunc");
            bool isSuccess = false;
            T result;
            do
            {
                var input = Console.ReadLine();
                if (!tryParseFunc(input, out result))
                {
                    isSuccess = false;
                    Console.WriteLine(parseErrorMessage);
                    continue;
                }
                if (!checkFunc(result))
                {
                    isSuccess = false;
                    Console.WriteLine(checkErrorMessage);
                    continue;
                }
                isSuccess = true;
            } while (isSuccess);
            return result;
        }

    }
}
