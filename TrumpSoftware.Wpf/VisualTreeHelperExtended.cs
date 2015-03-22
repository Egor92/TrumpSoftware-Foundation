using System;
using System.Windows;
using System.Windows.Media;

namespace TrumpSoftware.Wpf
{
    public static class VisualTreeHelperExtended
    {
        public static T GetAncestor<T>(this DependencyObject source, bool includingItself = false)
            where T : DependencyObject
        {
            return GetAncestor<T>(source, x => false, includingItself);
        }

        public static T GetAncestor<T>(this DependencyObject source, Func<DependencyObject, bool> abortCondition, bool includingItself = false)
            where T : DependencyObject
        {
            if (includingItself)
            {
                var tSource = source as T;
                if (tSource != null)
                    return tSource;
            }
            var parent = VisualTreeHelper.GetParent(source);
            if (parent == null)
                return null;
            if (abortCondition(parent))
                return null;
            var tParent = parent as T;
            if (tParent != null)
                return tParent;
            return GetAncestor<T>(parent, abortCondition);
        }
    }
}