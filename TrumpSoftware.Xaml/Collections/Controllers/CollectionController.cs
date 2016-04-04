using System;
using System.Windows.Input;
using ReactiveUI;
using TrumpSoftware.Common;
using TrumpSoftware.Common.Interfaces;

namespace TrumpSoftware.Xaml.Collections.Controllers
{
    public class CollectionController<T> : CollectionItemsController<T>
        where T : class, IRemovable
    {
        #region Fields

        private readonly IDefaultFactory<T> _defaultFactory;

        #endregion

        #region Ctor

        public CollectionController(IDefaultFactory<T> defaultFactory, Action<T> itemAddedAction, Action<T> itemRemovedAction)
            : base(itemAddedAction, itemRemovedAction)
        {
            if (defaultFactory == null)
                throw new ArgumentNullException("defaultFactory");
            _defaultFactory = defaultFactory;
        }

        public CollectionController(Func<T> createNewItemFunc, Action<T> itemAddedAction, Action<T> itemRemovedAction)
            : this(new DefaultFactory<T>(createNewItemFunc), itemAddedAction, itemRemovedAction)
        {
        }

        public CollectionController(IDefaultFactory<T> defaultFactory)
            : this(defaultFactory, null, null)
        {
        }

        public CollectionController(Func<T> createNewItemFunc)
            : this(createNewItemFunc, null, null)
        {
        }

        #endregion

        #region Commands

        #region AddNewItemCommand

        private ICommand _addNewItemCommand;

        public ICommand AddNewItemCommand
        {
            get { return _addNewItemCommand ?? (_addNewItemCommand = GetAddNewItemCommand()); }
        }

        private ReactiveCommand<object> GetAddNewItemCommand()
        {
            var command = ReactiveCommand.Create();
            command.Subscribe(OnAddNewItem);
            return command;
        }

        private void OnAddNewItem(object o)
        {
            var item = _defaultFactory.Create();
            AssotiatedCollection?.Add(item);
        }

        #endregion

        #endregion

        #region Overridden members

        public override void Dispose()
        {
            (AddNewItemCommand as IDisposable)?.Dispose();
            base.Dispose();
        }

        protected override void OnItemAdded(T newItem)
        {
            newItem.Removing += OnItemRemoving;
            base.OnItemAdded(newItem);
        }

        protected override void OnItemRemoved(T oldItem)
        {
            oldItem.Removing -= OnItemRemoving;
            base.OnItemRemoved(oldItem);
        }

        #endregion

        private void OnItemRemoving(object sender, EventArgs e)
        {
            var item = sender as T;
            if (item == null)
                return;
            AssotiatedCollection.Remove(item);
        }
    }
}
