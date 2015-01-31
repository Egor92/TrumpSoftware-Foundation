using System;

namespace TrumpSoftware.Xaml
{
    public static class BrushStrings
    {
        public static readonly string White = FromRgb(255, 255, 255);
        public static readonly string Green = FromRgb(0, 255, 0);

        public static string FromRgb(byte r, byte g, byte b)
        {
            return FromArgb(byte.MaxValue, r, g, b);
        }

        public static string FromArgb(byte a, byte r, byte g, byte b)
        {
            var aHex = GetHexString(a);
            var rHex = GetHexString(r);
            var gGex = GetHexString(g);
            var bHex = GetHexString(b);
            return string.Format("#{0}{1}{2}{3}", aHex, rHex, gGex, bHex);
        }

        private static string GetHexString(byte b)
        {
            var hex = Convert.ToString(b, 16);
            if (hex.Length == 1)
                hex = string.Format("0{0}", hex);
            return hex;
        }
    }
}
