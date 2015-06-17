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
    }
}
