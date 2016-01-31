using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace TrumpSoftware.Common.Hierarchicals
{
    public interface IHierarchical<T>
    {
        T Parent { get; set; }

        IEnumerable<T> GetChildren();
    }

    public static class HierarchicalExtensions
    {
        public static TAncestor GetAncestor<T, TAncestor>(this IHierarchical<T> source, bool includingItselt = false)
            where TAncestor : class
        {
            if (source == null)
                throw new ArgumentNullException("source");
            if (includingItselt)
            {
                var itself = source as TAncestor;
                if (itself != null)
                    return itself;
            }
            var castedParent = source.Parent as TAncestor;
            if (castedParent != null)
                return castedParent;
            var hierarhicalParent = source.Parent as IHierarchical<T>;
            if (hierarhicalParent == null)
                return null;
            return GetAncestor<T, TAncestor>(hierarhicalParent);
        }

        public static IEnumerable<TAncestor> GetAncestors<T, TAncestor>(this IHierarchical<T> source, bool includingItselt = false)
            where TAncestor : class
        {
            if (source == null)
                throw new ArgumentNullException("source");
            var ancestors = new Collection<TAncestor>();
            if (includingItselt)
            {
                AddIfItemOfType(source, ancestors);
            }
            var hierarchical = includingItselt
                ? source
                : source.Parent as IHierarchical<T>;
            while (hierarchical != null)
            {
                AddIfItemOfType(hierarchical, ancestors);
                hierarchical = hierarchical.Parent as IHierarchical<T>;
            }
            return ancestors;
        }

        public static IEnumerable<TAncestor> GetDescendants<T, TAncestor>(this IHierarchical<T> source, bool includingItselt = true)
            where TAncestor : class
        {
            var result = new Collection<TAncestor>();
            CollectDescendants(source, includingItselt, result);
            return result;
        }

        private static void CollectDescendants<T, TAncestor>(IHierarchical<T> source, bool includingItselt, ICollection<TAncestor> descendants)
            where TAncestor : class
        {
            if (source == null)
                throw new ArgumentNullException("source");
            if (includingItselt)
            {
                AddIfItemOfType(source, descendants);
            }
            foreach (var child in source.GetChildren())
            {
                AddIfItemOfType(child, descendants);
                var hierarchical = child as IHierarchical<T>;
                if (hierarchical != null)
                {
                    CollectDescendants(hierarchical, true, descendants);
                }
            }
        }

        private static void AddIfItemOfType<T>(object item, ICollection<T> collection)
            where T : class
        {
            var target = item as T;
            if (target != null)
                collection.Add(target);
        }
    }
}
