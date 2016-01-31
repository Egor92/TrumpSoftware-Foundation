using System.Collections.ObjectModel;
using TrumpSoftware.Common.Hierarchical;

namespace TrumpSoftware.Xaml.Mvvm
{
    /*public class HierarchicalObjectCollection<T> : ObservableCollection<T>
        where T : class, IHierarchical<T>
    {
        private readonly IHierarchical<T> _parent;

        public HierarchicalObjectCollection(IHierarchical<T> parent)
        {
            _parent = parent;
        }

        protected override void ClearItems()
        {
            foreach (var item in Items)
                item.Parent = null;
            base.ClearItems();
        }

        protected override void InsertItem(int index, T item)
        {
            item.Parent = _parent as T;
            base.InsertItem(index, item);
        }

        protected override void RemoveItem(int index)
        {
            Items[index].Parent = null;
            base.RemoveItem(index);
        }

        protected override void SetItem(int index, T item)
        {
            Items[index].Parent = null;
            item.Parent = _parent as T;
            base.SetItem(index, item);
        }
    }*/
}
