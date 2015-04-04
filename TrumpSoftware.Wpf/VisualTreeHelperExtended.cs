using System;
using System.Collections.Generic;
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