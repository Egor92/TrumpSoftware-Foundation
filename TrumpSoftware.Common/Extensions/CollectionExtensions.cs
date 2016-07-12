using System;
using System.Collections.Generic;
using System.Linq;
using TrumpSoftware.Common.Interfaces;

namespace TrumpSoftware.Common.Extensions
{
    public delegate void UpdateDelegate<in TTarget, in TSource>(TTarget target, TSource source);

    public static class CollectionExtensions
    {
        public static void Sort<T>(this ICollection<T> collection, IComparer<T> comparer = null)
        {
            comparer = comparer ?? Comparer<T>.Default;
            var list = new List<T>(collection);
            list.Sort(comparer);
            collection.ReplaceItems(list);
        }

        public static void AddRange<T>(this ICollection<T> collection, IEnumerable<T> newItems)
        {
            foreach (var newItem in newItems)
                collection.Add(newItem);
        }

        public static void RemoveRange<T>(this ICollection<T> collection, IEnumerable<T> removingItems)
        {
            foreach (var removingItem in removingItems)
                collection.Remove(removingItem);
        }

        public static void RemoveIf<T>(this ICollection<T> collection, Func<T, bool> condition)
        {
            foreach (var item in collection.Where(condition).ToList())
            {
                collection.Remove(item);
            }
        }

        public static void AddOrRemoveOrUpdate<TItem, TNewItem, TKey>(this ICollection<TItem> collection,
                                                                      IEnumerable<TNewItem> newItems,
                                                                      Func<TItem, TKey> getItemKey,
                                                                      Func<TNewItem, TKey> getNewItemKey,
                                                                      Func<TNewItem, TItem> getItemFunc,
                                                                      UpdateDelegate<TItem, TNewItem> updateItemAction,
                                                                      IEqualityComparer<TKey> comparer = null)
        {
            if (collection == null)
                throw new ArgumentNullException("collection");
            if (getItemKey == null)
                throw new ArgumentNullException("getItemKey");
            if (getNewItemKey == null)
                throw new ArgumentNullException("getNewItemKey");
            if (getItemFunc == null)
                throw new ArgumentNullException("getItemFunc");

            comparer = comparer ?? EqualityComparer<TKey>.Default;
            if ((newItems == null || !newItems.Any()) && collection.Any())
            {
                collection.Clear();
                return;
            }

            var oldItemsByKey = collection.ToLookup(getItemKey, comparer).ToDictionary(x => x.Key, x => x.FirstOrDefault());
            var newItemsByKey = newItems.ToLookup(getNewItemKey, comparer).ToDictionary(x => x.Key, x => x.FirstOrDefault());

            var itemsToDelete = oldItemsByKey.Where(x => !newItemsByKey.Keys.Contains(x.Key, comparer)).Select(x => x.Value).ToList();
            var itemsToAdd = newItemsByKey.Where(x => !oldItemsByKey.Keys.Contains(x.Key)).Select(x => getItemFunc(x.Value)).ToList();

            var oldAndNewItemPairsToUpdate = new Dictionary<TItem, TNewItem>();
            if (updateItemAction != null)
            {
                oldAndNewItemPairsToUpdate = newItemsByKey.Where(x => oldItemsByKey.Keys.Contains(x.Key))
                                                          .ToDictionary(x => oldItemsByKey[x.Key], x => x.Value);
            }

            collection.RemoveRange(itemsToDelete);
            collection.AddRange(itemsToAdd);

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

        public static void AddOrRemoveOrUpdate<TItem, TNewItem>(this ICollection<TItem> collection,
                                                                IEnumerable<TNewItem> newItems,
                                                                Func<TNewItem, TItem> newItemToOldItemFunc,
                                                                UpdateDelegate<TItem, TNewItem> updateItemAction,
                                                                IEqualityComparer<TItem> comparer = null)
        {
            Func<TNewItem, TItem> createItemAction = x =>
            {
                var item = newItemToOldItemFunc(x);
                updateItemAction(item, x);
                return item;
            };
            AddOrRemoveOrUpdate(collection, newItems, x => x, newItemToOldItemFunc, createItemAction, updateItemAction, comparer);
        }

        public static void AddOrRemoveOrUpdate<T>(this ICollection<T> collection,
                                                  IEnumerable<T> newItems,
                                                  UpdateDelegate<T, T> updateItemAction,
                                                  IEqualityComparer<T> comparer = null)
        {
            AddOrRemoveOrUpdate(collection, newItems, x => x, updateItemAction, comparer);
        }

        public static void AddOrRemoveOrUpdate<TItem, TNewItem, TKey>(this ICollection<TItem> collection,
                                                                      IEnumerable<TNewItem> newItems,
                                                                      Func<TItem, TKey> getItemKey,
                                                                      Func<TNewItem, TKey> getNewItemKey,
                                                                      Func<TNewItem, TItem> getItemFunc,
                                                                      IEqualityComparer<TKey> comparer = null)
            where TItem : IUpdatable<TNewItem>
        {
            var updateItemAction = GetUpdateItemAction<TItem, TNewItem>();
            AddOrRemoveOrUpdate(collection, newItems, getItemKey, getNewItemKey, getItemFunc, updateItemAction, comparer);
        }

        public static void AddOrRemoveOrUpdate<TItem, TNewItem>(this ICollection<TItem> collection,
                                                                IEnumerable<TNewItem> newItems,
                                                                Func<TNewItem, TItem> newItemToOldItemFunc,
                                                                IEqualityComparer<TItem> comparer = null)
            where TItem : IUpdatable<TNewItem>
        {
            var updateItemAction = GetUpdateItemAction<TItem, TNewItem>();
            AddOrRemoveOrUpdate(collection, newItems, newItemToOldItemFunc, updateItemAction, comparer);
        }

        public static void AddOrRemoveOrUpdate<T>(this ICollection<T> collection,
                                                  IEnumerable<T> newItems,
                                                  IEqualityComparer<T> comparer = null)
            where T : IUpdatable<T>
        {
            var updateItemAction = GetUpdateItemAction<T, T>();
            AddOrRemoveOrUpdate(collection, newItems, x => x, updateItemAction, comparer);
        }

        private static UpdateDelegate<TTarget, TSource> GetUpdateItemAction<TTarget, TSource>()
            where TTarget : IUpdatable<TSource>
        {
            return (target, source) => target.Update(source);
        }

        public static void AddOrRemove<TItem, TNewItem>(this ICollection<TItem> collection,
                                                        IEnumerable<TNewItem> newItems,
                                                        Func<TNewItem, TItem> getItemFunc,
                                                        IEqualityComparer<TItem> comparer = null)
        {
            if (collection == null)
                throw new ArgumentNullException("collection");
            if (getItemFunc == null)
                throw new ArgumentNullException("getItemFunc");

            var convertedNewItems = newItems.Select(getItemFunc).ToList();

            var itemsToDelete = collection.Except(convertedNewItems, comparer).ToList();
            var itemsToAdd = convertedNewItems.Except(collection, comparer).ToList();

            collection.RemoveRange(itemsToDelete);
            collection.AddRange(itemsToAdd);
        }

        public static void AddOrRemove<T>(this ICollection<T> collection,
                                          IEnumerable<T> newItems,
                                          IEqualityComparer<T> comparer = null)
        {
            AddOrRemove(collection, newItems, x => x, comparer);
        }

        public static void ReplaceItems<TItem, TNewItem>(this ICollection<TItem> collection,
                                                         IEnumerable<TNewItem> newItems,
                                                         Func<TNewItem, TItem> createItemAction)
        {
            if (collection == null)
                throw new ArgumentNullException("collection");
            if (newItems == null)
                throw new ArgumentNullException("newItems");
            if (createItemAction == null)
                throw new ArgumentNullException("createItemAction");

            collection.Clear();
            var convertedNewItems = newItems.Select(createItemAction);
            collection.AddRange(convertedNewItems);
        }

        public static void ReplaceItems<T>(this ICollection<T> collection, IEnumerable<T> newItems)
        {
            if (collection == null)
                throw new ArgumentNullException("collection");
            if (newItems == null)
                throw new ArgumentNullException("newItems");

            collection.ReplaceItems(newItems, x => x);
        }
    }
}