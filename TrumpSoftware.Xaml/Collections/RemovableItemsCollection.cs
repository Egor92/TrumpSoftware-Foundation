using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Prism.Commands;
using TrumpSoftware.Common.Extensions;
using TrumpSoftware.Common.Interfaces;

namespace TrumpSoftware.Xaml.Collections
{
    public class RemovableItemsCollection<T> : ObservableCollection<T>
        where T : IRemovable
    {
        #region Fields

        private readonly Func<T> _createNewItemFunc;

        #endregion

        #region Ctor

        public RemovableItemsCollection(Func<T> createNewItemFunc)
        {
            if (createNewItemFunc == null)
                throw new ArgumentNullException("createNewItemFunc");
            _createNewItemFunc = createNewItemFunc;
        }

        #endregion

        #region AddNewItemCommand

        private ICommand _addNewItemCommand;

        public ICommand AddNewItemCommand
        {
            get { return _addNewItemCommand ?? (_addNewItemCommand = new DelegateCommand(OnAddNewItem)); }
        }

        private void OnAddNewItem()
        {
            var newItem = _createNewItemFunc();
            Add(newItem);
        }

        #endregion

        #region Overridden members

        protected override void InsertItem(int index, T item)
        {
            item.Removing += OnItemRemoving;
            base.InsertItem(index, item);
        }

        protected override void SetItem(int index, T item)
        {
            item.Removing += OnItemRemoving;
            base.SetItem(index, item);
        }

        protected override void RemoveItem(int index)
        {
            this[index].Removing += OnItemRemoving;
            base.RemoveItem(index);
        }

        protected override void ClearItems()
        {
            this.ForEach(x => x.Removing -= OnItemRemoving);
            base.ClearItems();
        }

        #endregion

        private void OnItemRemoving(object sender, EventArgs e)
        {
            Remove((T)sender);
        }
    }
}
