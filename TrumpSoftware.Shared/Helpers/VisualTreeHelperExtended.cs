using System;
using System.Collections.Generic;
#if WPF
using System.Windows;
using System.Windows.Media;
#elif WINRT
using TrumpSoftware.WinRT.Extensions;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Media;
#endif

#if WPF
namespace TrumpSoftware.Wpf
#elif WINRT
namespace TrumpSoftware.WinRT
#endif
{
    public static class VisualTreeHelperExtended
    {
        public static DependencyObject GetParent(this DependencyObject source)
        {
            var parent = VisualTreeHelper.GetParent(source);
            if (parent == null)
            {
                var frameworkElement = source as FrameworkElement;
                if (frameworkElement != null)
                    parent = frameworkElement.Parent;

#if WPF
                var frameworkContentElement = source as FrameworkContentElement;
                if (frameworkContentElement != null)
                    parent = frameworkContentElement.Parent;
#endif
            }
            return parent;
        }

        public static T GetAncestor<T>(this DependencyObject source, bool includingItself = false)
            where T : DependencyObject
        {
            return GetAncestor<T>(source, x => false, includingItself);
        }

        public static T GetAncestor<T>(this DependencyObject source, Func<DependencyObject, bool> abortCondition, bool includingItself = false)
            where T : DependencyObject
        {
            return (T)source.GetAncestor(typeof(T), abortCondition, includingItself);
        }

        public static DependencyObject GetAncestor(this DependencyObject source, Type ancestorType, bool includingItself = false)
        {
            return GetAncestor(source, ancestorType, null, includingItself);
        }

        public static DependencyObject GetAncestor(this DependencyObject source, Type ancestorType, Func<DependencyObject, bool> abortCondition, bool includingItself = false)
        {
            if (!typeof(DependencyObject).IsAssignableFrom(ancestorType))
                throw new ArgumentException("AncestorType must be assignable to 'System.Windows.DependencyObject'", "ancestorType");
            if (includingItself)
            {
                if (ancestorType.IsInstanceOfType(source))
                    return source;
            }
            var parent = source.GetParent();
            if (parent == null)
                return null;
            if (abortCondition != null && abortCondition(parent))
                return null;
            if (ancestorType.IsInstanceOfType(parent))
                return parent;
            return GetAncestor(parent, ancestorType, abortCondition);
        }

        public static IEnumerable<T> GetDescendants<T>(this DependencyObject source, bool includingItself = false)
            where T : DependencyObject
        {
            return GetDescendants<T>(source, x => true, includingItself);
        }

        public static IEnumerable<T> GetDescendants<T>(this DependencyObject source, Func<T, bool> condition, bool includingItself = false)
            where T : DependencyObject
        {
            if (source == null)
                throw new ArgumentNullException("source");
            var results = new List<T>();
            if (includingItself)
            {
                var t = source as T;
                if (t != null && (condition(t)))
                    results.Add(t);
            }
            FindDescendants(source, condition, results);
            return results;
        }

        private static void FindDescendants<T>(this DependencyObject source, Func<T, bool> condition, ICollection<T> results)
            where T : DependencyObject
        {
            var childrenCount = VisualTreeHelper.GetChildrenCount(source);
            for (int i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(source, i);
                var t = child as T;
                if (t != null && (condition(t)))
                    results.Add(t);
                FindDescendants(child, condition, results);
            }
        }

        public static T GetFirstDescendant<T>(this DependencyObject source, Func<T, bool> condition, bool includingItself = false)
            where T : DependencyObject
        {
            if (source == null)
                throw new ArgumentNullException("source");
            if (includingItself)
            {
                var t = source as T;
                if (t != null && (condition(t)))
                    return t;
            }
            var childrenCount = VisualTreeHelper.GetChildrenCount(source);
            for (int i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(source, i);
                var t = child as T;
                if (t != null && (condition(t)))
                    return t;
            }
            for (int i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(source, i);
                var descendant = GetFirstDescendant(child, condition);
                if (descendant != null)
                    return descendant;
            }
            return null;
        }
    }
}
