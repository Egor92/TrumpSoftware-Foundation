using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reactive;
using System.Reactive.Linq;
using ReactiveUI;
using TrumpSoftware.Common.Extensions;

namespace TrumpSoftware.Xaml.Extensions
{
    public static class ReactiveExtensions
    {
        public static IObservable<Unit> ObservableForItemProperty<TItem, TValue>(this ReactiveList<TItem> reactiveList,
                                                                                 params Expression<Func<TItem, TValue>>[] itemProperties)
        {
            var propertyNames = itemProperties.Select(x => x.GetMemberName()).ToList();
            return Observable.Merge(new[]
            {
                reactiveList.ItemChanged.Select(x => x.PropertyName).Where(propertyNames.Contains).Select(_ => Unit.Default),
                reactiveList.CountChanged.Select(_ => Unit.Default),
            });
        }
    }
}