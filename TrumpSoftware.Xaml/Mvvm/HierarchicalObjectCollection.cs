using System.Collections.ObjectModel;
using TrumpSoftware.Common.Hierarchical;

namespace TrumpSoftware.Xaml.Mvvm
{
    public class HierarchicalObjectCollection<TViewModel> : ObservableCollection<TViewModel>
        where TViewModel : IHierarchical
    {
        private readonly IHierarchical _parent;

        public HierarchicalObjectCollection(IHierarchical parent)
        {
            _parent = parent;
        }

        protected override void ClearItems()
        {
            foreach (var item in Items)
                item.Parent = null;
            base.ClearItems();
        }

        protected override void InsertItem(int index, TViewModel item)
        {
            item.Parent = _parent;
            base.InsertItem(index, item);
        }

        protected override void RemoveItem(int index)
        {
            Items[index].Parent = null;
            base.RemoveItem(index);
        }

        protected override void SetItem(int index, TViewModel item)
        {
            Items[index].Parent = null;
            item.Parent = _parent;
            base.SetItem(index, item);
        }
    }
}
