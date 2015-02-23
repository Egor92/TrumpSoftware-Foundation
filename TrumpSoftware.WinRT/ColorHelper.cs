using System;
using Windows.UI;

namespace TrumpSoftware.WinRT
{
    public static class ColorHelper
    {
        public static Color ConvertFromString(string str)
        {
            if (str == null)
                throw new ArgumentNullException("str");
            if (str.Length < 1 || !str.StartsWith("#"))
                throw new ArgumentException("Invalid format");
            str = str.Substring(1, str.Length - 1);
            switch (str.Length)
            {
                case 3:
                    return FromString3(str);
                case 4:
                    return FromString4(str);
                case 6:
                    return FromString6(str);
                case 8:
                    return FromString8(str);
                default:
                    throw new ArgumentException("Invalid format");
            }
        }

        private static Color FromString3(string str)
        {
            byte r = GetByte(str[0]);
            byte g = GetByte(str[1]);
            byte b = GetByte(str[2]);
            return Color.FromArgb(byte.MaxValue, r, g, b);
        }

        private static Color FromString4(string str)
        {
            byte a = GetByte(str[0]);
            byte r = GetByte(str[1]);
            byte g = GetByte(str[2]);
            byte b = GetByte(str[3]);
            return Color.FromArgb(a, r, g, b);
        }

        private static Color FromString6(string str)
        {
            byte r = GetByte(str.Substring(0, 2));
            byte g = GetByte(str.Substring(2, 2));
            byte b = GetByte(str.Substring(4, 2));
            return Color.FromArgb(byte.MaxValue, r, g, b);
        }

        private static Color FromString8(string str)
        {
            byte a = GetByte(str.Substring(0, 2));
            byte r = GetByte(str.Substring(2, 2));
            byte g = GetByte(str.Substring(4, 2));
            byte b = GetByte(str.Substring(6, 2));
            return Color.FromArgb(a, r, g, b);
        }

        private static byte GetByte(char source)
        {
            return (byte)(16 * Convert.ToInt32(source.ToString(), 16));
        }

        private static byte GetByte(string source)
        {
            return Convert.ToByte(source, 16);
        }
    }
}
