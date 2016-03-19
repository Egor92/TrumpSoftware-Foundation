using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using TrumpSoftware.Common.Extensions;

namespace TrumpSoftware.Xaml.Collections.Controllers
{
    public static class CollectionSynchronizer
    {
        public static IDisposable Synchronize<TSourceItem, TTargetItem>(IEnumerable<TSourceItem> sourceCollection,
                                                                        ICollection<TTargetItem> targetCollection,
                                                                        Func<TSourceItem, TTargetItem> createTargetItem,
                                                                        Func<TTargetItem, TSourceItem> getSourceItem,
                                                                        IEqualityComparer<TTargetItem> targetItemEqualityComparer = null)
        {
            return new CollectionSynchronizer<TSourceItem, TTargetItem>(sourceCollection, targetCollection, createTargetItem, getSourceItem, targetItemEqualityComparer);
        }
    }

    public class CollectionSynchronizer<TSourceItem, TTargetItem> : IDisposable
    {
        #region Fields

        private readonly IEnumerable<TSourceItem> _sourceCollection;
        private readonly ICollection<TTargetItem> _targetCollection;
        private readonly Func<TSourceItem, TTargetItem> _createTargetItem;
        private readonly Func<TTargetItem, TSourceItem> _getSourceItem;
        private readonly IEqualityComparer<TTargetItem> _targetItemEqualityComparer;
        private ChangesInitiator? _changesInitiator;

        #endregion

        #region Ctor

        public CollectionSynchronizer(IEnumerable<TSourceItem> sourceCollection,
                                      ICollection<TTargetItem> targetCollection,
                                      Func<TSourceItem, TTargetItem> createTargetItem,
                                      Func<TTargetItem, TSourceItem> getSourceItem,
                                      IEqualityComparer<TTargetItem> targetItemEqualityComparer = null)
        {
            if (sourceCollection == null)
                throw new ArgumentNullException("sourceCollection");
            if (targetCollection == null)
                throw new ArgumentNullException("targetCollection");
            if (createTargetItem == null)
                throw new ArgumentNullException("createTargetItem");
            if (getSourceItem == null)
                throw new ArgumentNullException("getSourceItem");
            _sourceCollection = sourceCollection;
            _targetCollection = targetCollection;
            _createTargetItem = createTargetItem;
            _getSourceItem = getSourceItem;
            _targetItemEqualityComparer = targetItemEqualityComparer;

            Synchronize();
        }

        #endregion

        private void Synchronize()
        {
            _targetCollection.AddOrRemove(_sourceCollection, _createTargetItem, _targetItemEqualityComparer);

            var sourceNotifyingCollection = _sourceCollection as INotifyCollectionChanged;
            if (sourceNotifyingCollection != null)
                sourceNotifyingCollection.CollectionChanged += OnSourceCollectionChanged;

            var targetNotifyingCollection = _targetCollection as INotifyCollectionChanged;
            if (targetNotifyingCollection != null)
                targetNotifyingCollection.CollectionChanged += OnTargetCollectionChanged;
        }

        private void OnSourceCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (_changesInitiator == ChangesInitiator.Target)
                return;
            _changesInitiator = ChangesInitiator.Source;

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    var newTargetItems = e.NewItems
                        .Cast<TSourceItem>()
                        .Select(x => _createTargetItem(x))
                        .ToList();
                    _targetCollection.AddItems(newTargetItems, e.NewStartingIndex);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    var oldTargetItems = e.OldItems
                        .Cast<TSourceItem>()
                        .Select(x => _createTargetItem(x))
                        .ToList();
                    _targetCollection.RemoveItems(oldTargetItems, e.OldStartingIndex, _targetItemEqualityComparer, DisposeItem);
                    break;
                case NotifyCollectionChangedAction.Reset:
                    _targetCollection.ForEach(DisposeItem);
                    _targetCollection.Clear();
                    break;
            }
            _changesInitiator = null;
        }

        private void OnTargetCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (_changesInitiator == ChangesInitiator.Source)
                return;
            _changesInitiator = ChangesInitiator.Target;

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    var newSourceItems = e.NewItems
                        .Cast<TTargetItem>()
                        .Select(x => _getSourceItem(x))
                        .ToList();
                    _sourceCollection.AddItems(newSourceItems, e.NewStartingIndex);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    var oldSourceItems = e.OldItems
                        .Cast<TTargetItem>()
                        .Select(x => _getSourceItem(x))
                        .ToList();
                    _sourceCollection.RemoveItems(oldSourceItems, e.OldStartingIndex, null, DisposeItem);
                    break;
                case NotifyCollectionChangedAction.Reset:
                    _targetCollection.ForEach(DisposeItem);
                    _targetCollection.Clear();
                    break;
            }
            _changesInitiator = null;
        }

        private static void DisposeItem<T>(T item)
        {
            (item as IDisposable)?.Dispose();
        }

        #region Implementation of IDispose

        public void Dispose()
        {
            var sourceNotifyingCollection = _sourceCollection as INotifyCollectionChanged;
            if (sourceNotifyingCollection != null)
                sourceNotifyingCollection.CollectionChanged -= OnSourceCollectionChanged;

            var targetNotifyingCollection = _targetCollection as INotifyCollectionChanged;
            if (targetNotifyingCollection != null)
                targetNotifyingCollection.CollectionChanged -= OnSourceCollectionChanged;
        }

        #endregion

        #region Private classes

        #region ChangesInitiator

        private enum ChangesInitiator
        {
            Source,
            Target
        }

        #endregion

        #endregion
    }
}
