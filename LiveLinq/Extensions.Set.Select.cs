using System;
using System.Reactive.Linq;
using LiveLinq.Core;
using LiveLinq.Set;
using SimpleMonads;
using static LiveLinq.Utility;

namespace LiveLinq
{
    public static partial class Extensions
    {
        public static ISetChanges<T2> Select<T1, T2>(this ISetChanges<T1> source, Func<T1, IObservable<T2>> selector)
        {
            return Observable.Create((IObserver<ISetChange<T2>> observer) =>
            {
                return source.Subscribe(item =>
                        selector(item).SubscribeWithLatestValue(outputItem =>
                            observer.OnNext(SetChange(CollectionChangeType.Add, outputItem)), ex => observer.OnError(ex)),
                    (item, disposable, removalMode) =>
                    {
                        disposable.Dispose();
                        disposable.LatestValue.IfHasValue(value =>
                            observer.OnNext(SetChange(CollectionChangeType.Remove, value)));
                    });
            }).ToLiveLinq();
        }
        
        public static ISetChanges<T2> Select<T1, T2>(this ISetChanges<T1> source, Func<T1, T2> selector)
        {
            return Observable.Create((IObserver<ISetChange<T2>> observer) =>
            {
                return source.Subscribe(item =>
                    {
                        var selectedItem = selector(item);
                        observer.OnNext(SetChange(CollectionChangeType.Add, selectedItem));
                        return selectedItem;
                    },
                    (item, selectedItem, removalMode) =>
                    {
                        observer.OnNext(SetChange(CollectionChangeType.Remove, selectedItem));
                    });
            }).ToLiveLinq();
        }
    }
}