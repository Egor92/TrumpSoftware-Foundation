using System;

namespace TrumpSoftware.Common
{
    public static class RandomStringHelper
    {
        public static string GetWord(int length, WordCase wordCase = WordCase.WithACapitalLetter)
        {
            return GetWord(length, length, wordCase);
        }

        public static string GetWord(int minLength, int maxLength, WordCase wordCase = WordCase.WithACapitalLetter)
        {
            if (minLength <= 0)
                throw new ArgumentException("minLength must be positive", "minLength");
            if (maxLength <= 0)
                throw new ArgumentException("maxLength must be positive", "maxLength");
            if (minLength > maxLength)
                throw new ArgumentException("maxLength must be more than minLength");
            var length = RandomHelper.GetInt(minLength, maxLength + 1);
            var signs = new char[length];
            for (int i = 0; i < length; i++)
                signs[i] = GetChar(i, wordCase);
            return new string(signs);
        }

        private static char GetChar(int index, WordCase wordCase)
        {
            char minChar;
            char maxChar;
            switch (wordCase)
            {
                case WordCase.LowerCase:
                    minChar = 'a';
                    maxChar = 'z';
                    break;
                case WordCase.UpperCase:
                    minChar = 'A';
                    maxChar = 'Z';
                    break;
                case WordCase.Mixed:
                    if (RandomHelper.GetBool())
                    {
                        minChar = 'a';
                        maxChar = 'z';
                    }
                    else
                    {
                        minChar = 'A';
                        maxChar = 'Z';
                    }
                    break;
                case WordCase.WithACapitalLetter:
                    minChar = index == 0
                        ? 'A'
                        : 'a';
                    maxChar = index == 0
                        ? 'Z'
                        : 'z';
                    break;
                default:
                    throw new UnhandledCaseException(typeof(WordCase), wordCase);
            }
            return RandomHelper.GetChar(minChar, maxChar);
        }
    }

    public enum WordCase
    {
        LowerCase,
        UpperCase,
        Mixed,
        WithACapitalLetter
    }
}
