using System;
using TrumpSoftware.Common.Enums;
using TrumpSoftware.Common.Exceptions;
using TrumpSoftware.Common.Helpers;

namespace TrumpSoftware.Common.Extensions
{
    public static class StringExtensions
    {
        public static string TransformToCase(this string str, WordCase wordCase)
        {
            if (str == null)
                throw new ArgumentNullException("str");
            switch (wordCase)
            {
                case WordCase.Default:
                    return str;
                case WordCase.LowerCase:
                    return str.ToLower();
                case WordCase.UpperCase:
                    return str.ToUpper();
                case WordCase.WithACapitalLetter:
                    return TransformToCaseWithACapitalLetter(str);
                case WordCase.Mixed:
                    return TransformToMixedLetters(str);
                default:
                    throw new UnhandledCaseException(typeof(WordCase), wordCase);
            }
        }

        private static string TransformToCaseWithACapitalLetter(string str)
        {
            int length = str.Length;
            if (length == 0)
                return str;
            if (length == 1)
                return str.ToUpper();
            var firstLetter = Char.ToUpper(str[0]);
            var otherLetters = str.Substring(1, length - 1).ToLower();
            return string.Format("{0}{1}", firstLetter, otherLetters);
        }

        private static string TransformToMixedLetters(string str)
        {
            char[] chars = new char[str.Length];
            for (int i = 0; i < str.Length; i++)
            {
                if (RandomHelper.GetBool())
                    chars[i] = Char.ToLower(str[i]);
                else
                    chars[i] = Char.ToLower(str[i]);
            }
            return new string(chars);
        }
    }
}
