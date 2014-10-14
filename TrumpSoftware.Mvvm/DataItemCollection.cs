using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;

namespace TrumpSoftware.Mvvm
{
    public class DataItemCollection : ObservableCollection<IDataItem>
    {
        private readonly IList<Relation> _relations = new List<Relation>();

        public event EventHandler DataChanged;

        private void RaiseDataChanged()
        {
            if (DataChanged != null)
                DataChanged(this, EventArgs.Empty);
        }

        protected override void InsertItem(int index, IDataItem item)
        {
            base.InsertItem(index, item);
            SubscribeToItemPropertyChanged(item);
        }

        protected override void RemoveItem(int index)
        {
            var item = this[index];
            base.RemoveItem(index);
            UnsubscribeFromItemPropertyChanged(item);
        }

        protected override void SetItem(int index, IDataItem item)
        {
            var oldItem = this[index];
            base.SetItem(index, item);
            UnsubscribeFromItemPropertyChanged(oldItem);
            SubscribeToItemPropertyChanged(item);
        }

        protected override void ClearItems()
        {
            foreach (var oldItem in Items)
                UnsubscribeFromItemPropertyChanged(oldItem);
            base.ClearItems();
        }

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    var newDataItems = e.NewItems.Cast<IDataItem>();
                    var newId = this.Max(x => x.Id) + 1;
                    foreach (var newDataItem in newDataItems)
                    {
                        if (newDataItem.Id > 0)
                            continue;
                        newDataItem.Id = newId++;
                    }
                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach (var relation in _relations)
                    {
                        var oldDataItems = e.OldItems.Cast<IDataItem>();
                        foreach (var oldDataItem in oldDataItems)
                        {
                            for (int i = relation.Collection.Count - 1; i >= 0; i--)
                            {
                                var dataItem = relation.Collection[i];
                                var id = relation.GetRelatedId(dataItem);
                                if (id == oldDataItem.Id)
                                    relation.Collection.Remove(dataItem);
                            }
                        }
                    }
                    break;
                case NotifyCollectionChangedAction.Reset:
                    foreach (var relation in _relations)
                        relation.Collection.Clear();
                    break;
            }
            base.OnCollectionChanged(e);
            RaiseDataChanged();
        }

        private void SubscribeToItemPropertyChanged(IDataItem item)
        {
            var notifyPropertyChanged = item as INotifyPropertyChanged;
            if (notifyPropertyChanged != null)
                notifyPropertyChanged.PropertyChanged += DataItemPropertyChanged;
        }

        private void UnsubscribeFromItemPropertyChanged(IDataItem item)
        {
            var notifyPropertyChanged = item as INotifyPropertyChanged;
            if (notifyPropertyChanged != null)
                notifyPropertyChanged.PropertyChanged -= DataItemPropertyChanged;
        }

        private void DataItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            RaiseDataChanged();
        }
    }
}
