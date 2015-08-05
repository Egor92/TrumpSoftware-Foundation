using System.Collections.Generic;

namespace TrumpSoftware.Common.Extensions
{
    public static class ListExtensions
    {
        public static void Sort<T>(this IList<T> sourceCollection, IComparer<T> comparer)
        {
            var list = new List<T>(sourceCollection);
            list.Sort(comparer);
            sourceCollection.Clear();
            foreach (var item in list)
                sourceCollection.Add(item);
        }

        public static void AddRange<T>(this IList<T> sourceCollection, IEnumerable<T> newItems)
        {
            foreach (var newItem in newItems)
                sourceCollection.Add(newItem);
        }
    }
}
