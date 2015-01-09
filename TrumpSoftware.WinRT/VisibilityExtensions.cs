﻿using Windows.UI.Xaml;

namespace TrumpSoftware.WinRT
{
    public static class VisibilityExtensions
    {
        public static Visibility GetNegation(this Visibility visibility)
        {
            return visibility == Visibility.Visible
                ? Visibility.Collapsed
                : Visibility.Visible;
        }
    }
}
