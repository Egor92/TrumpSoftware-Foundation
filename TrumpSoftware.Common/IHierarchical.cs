using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace TrumpSoftware.Common
{
    public interface IHierarchical
    {
        IHierarchical Parent { get; set; }

        IEnumerable<IHierarchical> GetChildren();
    }

    public static class HierarchicalExtensions
    {
        public static T GetAncestor<T>(this IHierarchical source, bool includingItselt = false)
            where T : class
        {
            if (source == null)
                throw new ArgumentNullException("source");
            if (includingItselt)
            {
                var itself = source as T;
                if (itself != null)
                    return itself;
            }
            if (source.Parent == null)
                return null;
            var t = source.Parent as T;
            return t ?? GetAncestor<T>(source.Parent);
        }

        public static IEnumerable<T> GetDescendants<T>(this IHierarchical source, bool includingItselt = true)
            where T : class
        {
            var result = new Collection<T>();
            CollectDescendants(source, includingItselt, result);
            return result;
        }

        private static void CollectDescendants<T>(IHierarchical source, bool includingItselt, ICollection<T> descendants)
            where T : class
        {
            if (source == null)
                throw new ArgumentNullException("source");
            if (includingItselt)
            {
                var itself = source as T;
                if (itself != null)
                    descendants.Add(itself);
            }
            foreach (var child in source.GetChildren())
            {
                var t = child as T;
                if (t != null)
                    descendants.Add(t);
                CollectDescendants(child, true, descendants);
            }
        }
    }
}
