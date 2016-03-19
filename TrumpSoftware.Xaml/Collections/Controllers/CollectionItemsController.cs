using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using TrumpSoftware.Common.Extensions;

namespace TrumpSoftware.Xaml.Collections.Controllers
{
    public class CollectionItemsController<T> : IDisposable
    {
        #region Fields

        private readonly Action<T> _itemAddedAction;
        private readonly Action<T> _itemRemovedAction;

        #endregion

        #region Ctor

        public CollectionItemsController(Action<T> itemAddedAction, Action<T> itemRemovedAction)
        {
            _itemAddedAction = itemAddedAction;
            _itemRemovedAction = itemRemovedAction;
        }

        #endregion

        #region Implementation of IDisposable

        public virtual void Dispose()
        {
            Detach();
        }

        #endregion

        #region Properties

        #region AssotiatedCollection

        public ICollection<T> AssotiatedCollection { get; private set; }

        #endregion

        #endregion

        public virtual void Attach(ICollection<T> collection)
        {
            if (AssotiatedCollection != null)
                throw new InvalidOperationException(string.Format("{0} is already attached", GetType().Name));

            if (collection == null)
                throw new ArgumentNullException("collection");

            AssotiatedCollection = collection;

            var notifyCollectionChanged = AssotiatedCollection as INotifyCollectionChanged;
            if (notifyCollectionChanged != null)
                notifyCollectionChanged.CollectionChanged += OnCollectionChanged;

            collection.ForEach(OnItemAdded);
        }

        public virtual void Detach()
        {
            if (AssotiatedCollection == null)
                return;

            var notifyCollectionChanged = AssotiatedCollection as INotifyCollectionChanged;
            if (notifyCollectionChanged != null)
                notifyCollectionChanged.CollectionChanged -= OnCollectionChanged;

            AssotiatedCollection.ForEach(OnItemRemoved);
            AssotiatedCollection = null;
        }

        private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            e.NewItems?.OfType<T>().ForEach(OnItemAdded);
            e.OldItems?.OfType<T>().ForEach(OnItemRemoved);
        }

        protected virtual void OnItemAdded(T newItem)
        {
            _itemAddedAction?.Invoke(newItem);
        }

        protected virtual void OnItemRemoved(T oldItem)
        {
            _itemRemovedAction?.Invoke(oldItem);
        }
    }
}
