using System;
using System.Collections.Generic;
using System.Linq;
using TrumpSoftware.Common.Interfaces;

namespace TrumpSoftware.Common.Extensions
{
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
            foreach (var newItem in removingItems)
                collection.Remove(newItem);
        }

        public static void RemoveIf<T>(this ICollection<T> collection, Func<T, bool> condition)
        {
            foreach (var item in collection.Where(condition).ToList())
            {
                collection.Remove(item);
            }
        }

        public static void AddOrRemoveOrUpdate<TItem, TNewItem, TKey>(this ICollection<TItem> sourceItems,
                                                                      IEnumerable<TNewItem> newItems,
                                                                      Func<TItem, TKey> getItemKey,
                                                                      Func<TNewItem, TKey> getNewItemKey,
                                                                      Func<TNewItem, TItem> getItemFunc,
                                                                      Action<TItem, TNewItem> updateItemAction,
                                                                      IEqualityComparer<TKey> comparer = null)
        {
            if (sourceItems == null)
                throw new ArgumentNullException("sourceItems");
            if (getItemKey == null)
                throw new ArgumentNullException("getItemKey");
            if (getNewItemKey == null)
                throw new ArgumentNullException("getNewItemKey");
            if (getItemFunc == null)
                throw new ArgumentNullException("getItemFunc");

            comparer = comparer ?? EqualityComparer<TKey>.Default;
            if ((newItems == null || !newItems.Any()) && sourceItems.Any())
            {
                sourceItems.Clear();
                return;
            }

            var oldItemsByKey = sourceItems.ToLookup(getItemKey, comparer).ToDictionary(x => x.Key, x => x.FirstOrDefault());
            var newItemsByKey = newItems.ToLookup(getNewItemKey, comparer).ToDictionary(x => x.Key, x => x.FirstOrDefault());

            var itemsToDelete = oldItemsByKey.Where(x => !newItemsByKey.Keys.Contains(x.Key, comparer)).Select(x => x.Value).ToList();
            var itemsToAdd = newItemsByKey.Where(x => !oldItemsByKey.Keys.Contains(x.Key)).Select(x => getItemFunc(x.Value)).ToList();

            var oldAndNewItemPairsToUpdate = new Dictionary<TItem, TNewItem>();
            if (updateItemAction != null)
            {
                oldAndNewItemPairsToUpdate = newItemsByKey.Where(x => oldItemsByKey.Keys.Contains(x.Key))
                                                          .ToDictionary(x => oldItemsByKey[x.Key], x => x.Value);
            }

            sourceItems.RemoveRange(itemsToDelete);
            sourceItems.AddRange(itemsToAdd);

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

        public static void AddOrRemoveOrUpdate<TItem, TNewItem>(this ICollection<TItem> sourceItems,
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
            AddOrRemoveOrUpdate(sourceItems, newItems, x => x, newItemToOldItemFunc, createItemAction, updateItemAction, comparer);
        }

        public static void AddOrRemoveOrUpdate<T>(this ICollection<T> sourceItems,
                                                  IEnumerable<T> newItems,
                                                  Action<T, T> updateItemAction,
                                                  IEqualityComparer<T> comparer = null)
        {
            AddOrRemoveOrUpdate(sourceItems, newItems, x => x, updateItemAction, comparer);
        }

        public static void AddOrRemoveOrUpdate<TItem, TNewItem, TKey>(this ICollection<TItem> sourceItems,
                                                                      IEnumerable<TNewItem> newItems,
                                                                      Func<TItem, TKey> getItemKey,
                                                                      Func<TNewItem, TKey> getNewItemKey,
                                                                      Func<TNewItem, TItem> getItemFunc,
                                                                      IEqualityComparer<TKey> comparer = null)
            where TItem : IUpdatable<TNewItem>
        {
            Action<TItem, TNewItem> updateItemAction = (target, source) => target.Update(source);
            AddOrRemoveOrUpdate(sourceItems, newItems, getItemKey, getNewItemKey, getItemFunc, updateItemAction, comparer);
        }

        public static void AddOrRemoveOrUpdate<TItem, TNewItem>(this ICollection<TItem> sourceItems,
                                                                IEnumerable<TNewItem> newItems,
                                                                Func<TNewItem, TItem> newItemToOldItemFunc,
                                                                IEqualityComparer<TItem> comparer = null)
            where TItem : IUpdatable<TNewItem>
        {
            Action<TItem, TNewItem> updateItemAction = (target, source) => target.Update(source);
            AddOrRemoveOrUpdate(sourceItems, newItems, newItemToOldItemFunc, updateItemAction, comparer);
        }

        public static void AddOrRemoveOrUpdate<T>(this ICollection<T> sourceItems,
                                                  IEnumerable<T> newItems,
                                                  IEqualityComparer<T> comparer = null)
            where T : IUpdatable<T>
        {
            Action<T, T> updateItemAction = (target, source) => target.Update(source);
            AddOrRemoveOrUpdate(sourceItems, newItems, x => x, updateItemAction, comparer);
        }

        public static void AddOrRemove<TItem, TNewItem>(this ICollection<TItem> sourceItems,
                                                        IEnumerable<TNewItem> newItems,
                                                        Func<TNewItem, TItem> getItemFunc,
                                                        IEqualityComparer<TItem> comparer = null)
        {
            if (sourceItems == null)
                throw new ArgumentNullException("sourceItems");
            if (getItemFunc == null)
                throw new ArgumentNullException("getItemFunc");

            var convertedNewItems = newItems.Select(getItemFunc).ToList();

            var itemsToDelete = sourceItems.Except(convertedNewItems, comparer).ToList();
            var itemsToAdd = convertedNewItems.Except(sourceItems, comparer).ToList();

            sourceItems.RemoveRange(itemsToDelete);
            sourceItems.AddRange(itemsToAdd);
        }

        public static void AddOrRemove<T>(this ICollection<T> sourceItems,
                                          IEnumerable<T> newItems,
                                          IEqualityComparer<T> comparer = null)
        {
            AddOrRemove(sourceItems, newItems, x => x, comparer);
        }

        public static void ReplaceItems<TItem, TNewItem>(this ICollection<TItem> sourceItems,
                                                         IEnumerable<TNewItem> newItems,
                                                         Func<TNewItem, TItem> createItemAction)
        {
            if (sourceItems == null)
                throw new ArgumentNullException("sourceItems");
            if (newItems == null)
                throw new ArgumentNullException("newItems");
            if (createItemAction == null)
                throw new ArgumentNullException("createItemAction");

            sourceItems.Clear();
            foreach (var newItem in newItems.Select(createItemAction))
            {
                sourceItems.Add(newItem);
            }
        }

        public static void ReplaceItems<T>(this ICollection<T> sourceItems, IEnumerable<T> newItems)
        {
            if (sourceItems == null)
                throw new ArgumentNullException("sourceItems");
            if (newItems == null)
                throw new ArgumentNullException("newItems");

            sourceItems.ReplaceItems(newItems, x => x);
        }
    }
}