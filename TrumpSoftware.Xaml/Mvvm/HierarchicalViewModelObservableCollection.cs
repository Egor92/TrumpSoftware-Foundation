using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace TrumpSoftware.Xaml.Mvvm
{
    public class HierarchicalViewModelObservableCollection<TModel, TViewModel> : HierarchicalViewModelCollection<TViewModel>
        where TModel : class
        where TViewModel : HierarchicalViewModel, IModelPresenter<TModel>
    {
        private readonly IList<TModel> _modelCollection;
        private readonly Func<TModel, TViewModel> _getViewModel;
        private readonly Func<TViewModel, TModel> _getModel;
        private readonly Predicate<TModel> _filter;
        private ChangesInitiator? _changesInitiator;

        public HierarchicalViewModelObservableCollection(HierarchicalViewModel parent, IList<TModel> modelCollection, Func<TModel, TViewModel> getViewModel, Func<TViewModel, TModel> getModel, Predicate<TModel> filter = null)
            : base(parent)
        {
            if (modelCollection == null)
                throw new ArgumentNullException("modelCollection");
            if (getViewModel == null)
                throw new ArgumentNullException("getViewModel");
            if (getModel == null)
                throw new ArgumentNullException("getModel");
            _modelCollection = modelCollection;
            _getViewModel = getViewModel;
            _getModel = getModel;
            _filter = filter;
            foreach (var model in _modelCollection)
            {
                if (!IsSatisfiedByFilter(model))
                    continue;
                var viewModel = getViewModel(model);
                Items.Add(viewModel);
            }
            var notifyCollectionChanged = modelCollection as INotifyCollectionChanged;
            if (notifyCollectionChanged != null)
                notifyCollectionChanged.CollectionChanged += OnModelCollectionChanged;
        }

        private void OnModelCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (_changesInitiator == ChangesInitiator.ViewModel)
                return;
            _changesInitiator = ChangesInitiator.Model;

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    var newViewModels = e.NewItems
                        .Cast<TModel>()
                        .Where(IsSatisfiedByFilter)
                        .Select(x => _getViewModel(x))
                        .ToList();
                    foreach (var newViewModel in newViewModels)
                        Add(newViewModel);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    var oldModels = e.OldItems.Cast<TModel>();
                    foreach (var oldModel in oldModels)
                    {
                        var oldViewModels = this.Where(x => x.Model == oldModel);
                        foreach (var oldViewModel in oldViewModels)
                            Remove(oldViewModel);
                    }
                    break;
                case NotifyCollectionChangedAction.Reset:
                    Clear();
                    break;
            }
            _changesInitiator = null;
        }

        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnCollectionChanged(e);
            if (_changesInitiator == ChangesInitiator.Model)
                return;
            _changesInitiator = ChangesInitiator.ViewModel;

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    var newModels = e.NewItems
                        .Cast<TViewModel>()
                        .Select(x => _getModel(x))
                        .ToList();
                    foreach (var newModel in newModels)
                        _modelCollection.Add(newModel);
                    break;
                case NotifyCollectionChangedAction.Remove:
                    var oldModels = e.OldItems
                        .Cast<TViewModel>()
                        .Select(x => _getModel(x))
                        .ToList();
                    foreach (var oldModel in oldModels)
                        _modelCollection.Remove(oldModel);
                    break;
                case NotifyCollectionChangedAction.Reset:
                    _modelCollection.Clear();
                    break;
            }
            _changesInitiator = null;
        }

        private bool IsSatisfiedByFilter(TModel model)
        {
            return _filter == null || _filter(model);
        }

        private enum ChangesInitiator
        {
            Model,
            ViewModel
        }
    }
}
