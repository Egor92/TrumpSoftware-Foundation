using System;
using System.Collections.Generic;
using System.Linq;

namespace TrumpSoftware.Common.Extensions
{
    public static class ListExtensions
    {
        public static void AddOrRemoveOrUpdate<TItem, TNewItem, TKey>(this ICollection<TItem> source,
                                                                      IEnumerable<TNewItem> newItems,
                                                                      Func<TItem, TKey> getItemKey,
                                                                      Func<TNewItem, TKey> getNewItemKey,
                                                                      Func<TNewItem, TItem> createItemAction,
                                                                      Action<TItem, TNewItem> updateItemAction,
                                                                      IEqualityComparer<TKey> comparer = null)
        {
            if (source == null)
                throw new ArgumentNullException("source");
            if (getItemKey == null)
                throw new ArgumentNullException("getItemKey");
            if (getNewItemKey == null)
                throw new ArgumentNullException("getNewItemKey");
            if (createItemAction == null)
                throw new ArgumentNullException("createItemAction");

            comparer = comparer ?? EqualityComparer<TKey>.Default;
            if ((newItems == null || !newItems.Any()) && source.Any())
            {
                source.Clear();
                return;
            }

            var oldItemsByKey = source.ToLookup(getItemKey, comparer).ToDictionary(x => x.Key, x => x.FirstOrDefault());
            var newItemsByKey = newItems.ToLookup(getNewItemKey, comparer).ToDictionary(x => x.Key, x => x.FirstOrDefault());

            var itemsToDelete = oldItemsByKey.Where(x => !newItemsByKey.Keys.Contains(x.Key, comparer)).Select(x => x.Value).ToList();
            var itemsToAdd = newItemsByKey.Where(x => !oldItemsByKey.Keys.Contains(x.Key)).Select(x => createItemAction(x.Value)).ToList();

            var oldAndNewItemPairsToUpdate = new List<KeyValuePair<TItem, TNewItem>>();
            if (updateItemAction != null)
                oldAndNewItemPairsToUpdate = newItemsByKey.Where(x => oldItemsByKey.Keys.Contains(x.Key)).Select(x => new KeyValuePair<TItem, TNewItem>(oldItemsByKey[x.Key], x.Value)).ToList();

            foreach (var itemToDelete in itemsToDelete)
                source.Remove(itemToDelete);

            foreach (var itemToAdd in itemsToAdd)
                source.Add(itemToAdd);

            if (updateItemAction != null)
            {
                foreach (var pair in oldAndNewItemPairsToUpdate)
                {
                    var oldItem = pair.Key;
                    var newItem = pair.Value;
                    if (!Equals(oldItem, null) && !Equals(newItem, null))
                    {
                        updateItemAction(oldItem, newItem);
                    }
                }
            }
        }

        public static void AddOrRemoveOrUpdate<TItem, TNewItem>(this ICollection<TItem> source,
                                                                IEnumerable<TNewItem> newItems,
                                                                Func<TNewItem, TItem> newItemToOldItemFunc,
                                                                Action<TItem, TNewItem> updateItemAction,
                                                                IEqualityComparer<TItem> comparer = null)
        {
            Func<TNewItem, TItem> createItemAction = x =>
            {
                var item = newItemToOldItemFunc(x);
                updateItemAction(item, x);
                return item;
            };
            AddOrRemoveOrUpdate(source, newItems, x => x, newItemToOldItemFunc, createItemAction, updateItemAction, comparer);
        }

        public static void AddOrRemoveOrUpdate<T>(this ICollection<T> source,
                                                  IEnumerable<T> newItems,
                                                  Action<T, T> updateItemAction,
                                                  IEqualityComparer<T> comparer = null)
        {
            AddOrRemoveOrUpdate(source, newItems, x => x, updateItemAction, comparer);
        }

        public static void AddOrRemove<TItem, TNewItem>(this ICollection<TItem> source,
                                                        IEnumerable<TNewItem> newItems,
                                                        Func<TNewItem, TItem> createItemAction,
                                                        IEqualityComparer<TItem> comparer = null)
        {
            if (source == null)
                throw new ArgumentNullException("source");
            if (newItems == null)
                throw new ArgumentNullException("newItems");
            if (createItemAction == null)
                throw new ArgumentNullException("createItemAction");

            var convertedNewItems = newItems.Select(createItemAction).ToList();

            var itemsToDelete = source.Except(convertedNewItems, comparer).ToList();
            var itemsToAdd = convertedNewItems.Except(source, comparer).ToList();

            foreach (var itemToDelete in itemsToDelete)
                source.Remove(itemToDelete);

            foreach (var itemToAdd in itemsToAdd)
                source.Add(itemToAdd);
        }

        public static void AddOrRemove<T>(this ICollection<T> source,
                                          IEnumerable<T> newItems,
                                          IEqualityComparer<T> comparer = null)
        {
            AddOrRemove(source, newItems, x => x, comparer);
        }

        public static void ReplaceItems<TItem, TNewItem>(this ICollection<TItem> source,
                                                         IEnumerable<TNewItem> newItems,
                                                         Func<TNewItem, TItem> createItemAction)
        {
            if (source == null)
                throw new ArgumentNullException("source");
            if (newItems == null)
                throw new ArgumentNullException("newItems");
            if (createItemAction == null)
                throw new ArgumentNullException("createItemAction");

            source.Clear();
            foreach (var newItem in newItems.Select(createItemAction))
            {
                source.Add(newItem);
            }
        }

        public static void ReplaceItems<T>(this ICollection<T> source, IEnumerable<T> newItems)
        {
            if (source == null)
                throw new ArgumentNullException("source");
            if (newItems == null)
                throw new ArgumentNullException("newItems");

            source.ReplaceItems(newItems, x => x);
        }

        public static void RemoveIf<T>(this IList<T> source, Func<T, bool> condition)
        {
            int index = 0;
            while (index < source.Count)
            {
                var item = source[index];
                if (condition(item))
                {
                    source.RemoveAt(index);
                }
                else
                {
                    index++;
                }
            }
        }
    }
}
