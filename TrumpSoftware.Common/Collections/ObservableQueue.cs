using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace TrumpSoftware.Common.Collections
{
    public class ObservableQueue<T> : IReadOnlyCollection<T>, INotifyCollectionChanged
    {
        private readonly Queue<T> _queue = new Queue<T>();

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public int Count
        {
            get { return _queue.Count; }
        }

        public ObservableQueue()
        {

        }

        public ObservableQueue(IEnumerable<T> items)
        {
            if (items == null)
                throw new ArgumentNullException("items");
            foreach (var item in items)
                _queue.Enqueue(item);
        }

        public void Clear()
        {
            _queue.Clear();
            RaiseCollectionChanged(NotifyCollectionChangedAction.Reset);
        }

        public T Dequeue()
        {
            var item = _queue.Dequeue();
            RaiseCollectionChanged(NotifyCollectionChangedAction.Remove, item);
            return item;
        }

        public void Enqueue(T item)
        {
            _queue.Enqueue(item);
            RaiseCollectionChanged(NotifyCollectionChangedAction.Add, item);
        }

        public T Peek()
        {
            return _queue.Peek();
        }

        public bool Contains(T item)
        {
            return _queue.Contains(item);
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator()
        {
            return _queue.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable)_queue).GetEnumerator();
        }

        private void RaiseCollectionChanged(NotifyCollectionChangedAction action)
        {
            var handler = CollectionChanged;
            if (handler != null)
                handler(this, new NotifyCollectionChangedEventArgs(action));
        }

        private void RaiseCollectionChanged(NotifyCollectionChangedAction action, T item)
        {
            var handler = CollectionChanged;
            if (handler != null)
                handler(this, new NotifyCollectionChangedEventArgs(action, item));
        }
    }
}
