using System;
using System.Collections.Generic;
using System.Reactive.Linq;
using LiveLinq.Core;
using LiveLinq.Set;
using SimpleMonads;
using static LiveLinq.Utility;

namespace LiveLinq
{
    public static partial class Extensions
    {
        public static ISetChanges<T2> SelectMany<T1, T2>(this ISetChanges<T1> source,
            Func<T1, IEnumerable<T2>> selector)
        {
            return source.SelectMany(item => selector(item).ToLiveLinqUnchangingSet());
        }

        public static ISetChanges<T> SelectMany<T>(this ISetChanges<ISetChanges<T>> source)
        {
            return source.SelectMany(x => x);
        }

        public static ISetChanges<T2> SelectMany<T1, T2>(this ISetChanges<T1> source, Func<T1, ISetChanges<T2>> selector)
        {
            return Observable.Create((IObserver<ISetChange<T2>> observer) =>
            {
                return source.Subscribe(item =>
                        selector(item).ToObservableStateAndChange().SubscribeWithLatestValue<SetStateAndChange<T2>>(stateAndChange =>
                            observer.OnNext(stateAndChange.MostRecentChange)),
                    (item, disposable, removalMode) =>
                    {
                        disposable.Dispose();
                        disposable.LatestValue.IfHasValue(value =>
                            observer.OnNext(SetChange<T2>(CollectionChangeType.Remove, value.State)));
                    });
            }).ToLiveLinq();
        }
    }
}