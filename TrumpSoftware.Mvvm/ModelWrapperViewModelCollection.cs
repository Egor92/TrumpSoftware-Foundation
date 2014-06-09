using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace TrumpSoftware.Mvvm
{
    public class ModelWrapperViewModelCollection<TModel, TViewModel> : ViewModelCollection<TViewModel>
        where TViewModel : ViewModelBase
    {
        private readonly IList<TModel> _modelCollection;
        private readonly Func<TModel, TViewModel> _getViewModel;
        private readonly Func<TViewModel, TModel> _getModel;
        private ChangesInitiator? _changesInitiator;

        public ModelWrapperViewModelCollection(ViewModelBase parent, IList<TModel> modelCollection, Func<TModel, TViewModel> getViewModel, Func<TViewModel, TModel> getModel) 
            : base(parent)
        {
            if (modelCollection == null) 
                throw new ArgumentNullException("modelCollection");
            if (getViewModel == null) 
                throw new ArgumentNullException("getViewModel");
            if (getModel == null) 
                throw new ArgumentNullException("getModel");
            foreach (var model in modelCollection)
            {
                var viewModel = getViewModel(model);
                Add(viewModel);
            }
            _modelCollection = modelCollection;
            var notifyCollectionChanged = modelCollection as INotifyCollectionChanged;
            if (notifyCollectionChanged != null)
                notifyCollectionChanged.CollectionChanged += OnModelCollectionChanged;
            _getViewModel = getViewModel;
            _getModel = getModel;
            CollectionChanged += OnViewModelCollectionChanged;
        }

        private void OnModelCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (_changesInitiator == ChangesInitiator.ViewModel)
                return;
            _changesInitiator = ChangesInitiator.Model;

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    var newViewModels = e.NewItems.Cast<TModel>().Select(x => _getViewModel(x)).ToList();
                    foreach (var newViewModel in newViewModels)
                        Items.Add(newViewModel);
                    break;
                case NotifyCollectionChangedAction.Move:
                    throw new NotImplementedException();
                    break;
                case NotifyCollectionChangedAction.Remove:
                    var oldViewModels = e.OldItems.Cast<TModel>().Select(x => _getViewModel(x)).ToList();
                    foreach (var oldViewModel in oldViewModels)
                        Items.Remove(oldViewModel);
                    break;
                case NotifyCollectionChangedAction.Replace:
                    throw new NotImplementedException();
                    break;
                case NotifyCollectionChangedAction.Reset:
                    Items.Clear();
                    break;
            }
            _changesInitiator = null;
        }

        private void OnViewModelCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (_changesInitiator == ChangesInitiator.Model)
                return;
            _changesInitiator = ChangesInitiator.ViewModel;

            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    var newModels = e.NewItems.Cast<TViewModel>().Select(x => _getModel(x)).ToList();
                    foreach (var newModel in newModels)
                        _modelCollection.Add(newModel);
                    break;
                case NotifyCollectionChangedAction.Move:
                    throw new NotImplementedException();
                    break;
                case NotifyCollectionChangedAction.Remove:
                    var oldModels = e.OldItems.Cast<TViewModel>().Select(x => _getModel(x)).ToList();
                    foreach (var oldModel in oldModels)
                        _modelCollection.Remove(oldModel);
                    break;
                case NotifyCollectionChangedAction.Replace:
                    throw new NotImplementedException();
                    break;
                case NotifyCollectionChangedAction.Reset:
                    _modelCollection.Clear();
                    break;
            }
            _changesInitiator = null;
        }

        private enum ChangesInitiator
        {
            Model,
            ViewModel
        }
    }
}
