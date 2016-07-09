using System.Collections.ObjectModel;
using TrumpSoftware.Common.Extensions;
using System.Linq;
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

namespace TrumpSoftware.Wpf.Helpers
#elif WINRT

namespace TrumpSoftware.WinRT.Helpers
#endif
{
    public static class UIHelper
    {
        #region VisualTreeHelper methods

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

        public static IEnumerable<DependencyObject> GetChildren(this DependencyObject source)
        {
            if (source == null)
                throw new ArgumentNullException("source");

            var childrenCount = VisualTreeHelper.GetChildrenCount(source);
            for (int i = 0; i < childrenCount; i++)
            {
                yield return VisualTreeHelper.GetChild(source, i);
            }
        }

        #endregion

        #region GetAncestor methods

        public static T GetAncestor<T>(this DependencyObject source, bool includingItself = false)
            where T : DependencyObject
        {
            return GetAncestor<T>(source, null, includingItself);
        }

        public static T GetAncestor<T>(this DependencyObject source, Func<T, bool> targetElementCondition, bool includingItself = false)
            where T : DependencyObject
        {
            return GetAncestor(source, targetElementCondition, null, includingItself);
        }

        public static T GetAncestor<T>(this DependencyObject source,
                                       Func<T, bool> targetElementCondition,
                                       Func<DependencyObject, bool> abortCondition,
                                       bool includingItself = false)
            where T : DependencyObject
        {
            Func<DependencyObject, bool> condition = GetNotGenericCondition(targetElementCondition);
            return (T) source.GetAncestor(typeof (T), condition, abortCondition, includingItself);
        }

        public static DependencyObject GetAncestor(this DependencyObject source, Type ancestorType, bool includingItself = false)
        {
            return source.GetAncestor(ancestorType, null, null, includingItself);
        }

        public static DependencyObject GetAncestor(this DependencyObject source,
                                                   Type ancestorType,
                                                   Func<DependencyObject, bool> targetElementCondition,
                                                   Func<DependencyObject, bool> abortCondition,
                                                   bool includingItself = false)
        {
            if (!typeof (DependencyObject).IsAssignableFrom(ancestorType))
                throw new ArgumentException("AncestorType must be assignable to 'System.Windows.DependencyObject'", "ancestorType");

            if (includingItself)
            {
                if (ancestorType.IsInstanceOfType(source))
                {
                    if (targetElementCondition == null || targetElementCondition(source))
                    {
                        return source;
                    }
                }
            }

            abortCondition = abortCondition ?? (x => false);
            if (abortCondition(source))
                return null;

            var parent = source.GetParent();
            if (parent == null)
                return null;

            return parent.GetAncestor(ancestorType, targetElementCondition, abortCondition, true);
        }

        #endregion

        #region GetAncestors methods

        public static ICollection<T> GetAncestors<T>(this DependencyObject source,
                                                     Func<T, bool> targetElementCondition,
                                                     Func<DependencyObject, bool> abortCondition,
                                                     bool includingItself = false)
            where T : DependencyObject
        {
            Func<DependencyObject, bool> condition = GetNotGenericCondition(targetElementCondition);
            return source.GetAncestors(typeof (T), condition, abortCondition, includingItself)
                         .Cast<T>()
                         .ToList();
        }

        public static ICollection<DependencyObject> GetAncestors(this DependencyObject source,
                                                                 Type ancestorType,
                                                                 Func<DependencyObject, bool> targetElementCondition,
                                                                 Func<DependencyObject, bool> abortCondition,
                                                                 bool includingItself = false)
        {
            var ancestors = new Collection<DependencyObject>();
            source.CollectAncestors(ancestorType, targetElementCondition, abortCondition, includingItself, ancestors);
            return ancestors;
        }

        private static void CollectAncestors(this DependencyObject source,
                                             Type ancestorType,
                                             Func<DependencyObject, bool> targetElementCondition,
                                             Func<DependencyObject, bool> abortCondition,
                                             bool includingItself,
                                             ICollection<DependencyObject> ancestors)
        {
            var element = source.GetAncestor(ancestorType, targetElementCondition, abortCondition, includingItself);
            if (element == null)
                return;

            ancestors.Add(element);
            element.CollectAncestors(ancestorType, targetElementCondition, abortCondition, false, ancestors);
        }

        #endregion

        #region GetDescendants methods

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

            condition = condition ?? (x => true);
            var descendants = new List<T>();

            if (includingItself)
            {
                var t = source as T;
                if (t != null && (condition(t)))
                    descendants.Add(t);
            }

            CollectDescendants(source, condition, descendants);
            return descendants;
        }

        private static void CollectDescendants<T>(this DependencyObject source, Func<T, bool> condition, ICollection<T> descendants)
            where T : DependencyObject
        {
            var children = source.GetChildren().ToList();

            var targetDescendants = children.OfType<T>().Where(condition);
            descendants.AddRange(targetDescendants);

            foreach (var child in children)
            {
                CollectDescendants(child, condition, descendants);
            }
        }

        public static T GetDescendantByName<T>(this DependencyObject source, string name)
            where T : FrameworkElement
        {
            return GetFirstDescendant<T>(source, x => x.Name == name, null, true);
        }

        public static T GetFirstDescendant<T>(this DependencyObject source,
                                              Func<T, bool> condition,
                                              int? maxNestingLevel,
                                              bool includingItself = false)
            where T : DependencyObject
        {
            if (source == null)
                throw new ArgumentNullException("source");

            condition = condition ?? (x => true);

            if (includingItself)
            {
                maxNestingLevel--;
                var targetDescendant = source as T;
                if (targetDescendant != null && (condition(targetDescendant)))
                    return targetDescendant;
            }

            maxNestingLevel--;
            if (maxNestingLevel < 0)
                return null;

            return source.GetChildren()
                         .Select(x => x.GetFirstDescendant(condition, maxNestingLevel, true))
                         .FirstOrDefault(x => x != null);
        }

        #endregion

        private static Func<DependencyObject, bool> GetNotGenericCondition<T>(Func<T, bool> targetElementCondition)
            where T : DependencyObject
        {
            return x =>
            {
                if (targetElementCondition == null)
                    return true;
                return targetElementCondition((T) x);
            };
        }
    }
}