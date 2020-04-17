using System;
using System.Reactive.Linq;
using LiveLinq.Core;
using LiveLinq.Set;
using SimpleMonads;
using static LiveLinq.Utility;

namespace LiveLinq
{
    public partial class Extensions
    {
        public static ISetChanges<T> Where<T>(this ISetChanges<T> source, Func<T, bool> selector)
        {
            return Observable.Create((IObserver<ISetChange<T>> observer) =>
            {
                return source.Subscribe(item =>
                    {
                        var isIncluded = selector(item);
                        if (isIncluded)
                        {
                            observer.OnNext(SetChange(CollectionChangeType.Add, item));
                        }

                        return isIncluded;
                    },
                    (item, isIncluded, removalMode) =>
                    {
                        if (isIncluded)
                            observer.OnNext(SetChange(CollectionChangeType.Remove, item));
                    });
            }).ToLiveLinq();
        }

        public static ISetChanges<T> Where<T>(this ISetChanges<T> source, Func<T, IObservable<bool>> selector)
        {
            return Observable.Create((IObserver<ISetChange<T>> observer) =>
            {
                return source.Subscribe(item =>
                        selector(item).DistinctUntilChanged().SkipWhile(x => !x).SubscribeWithLatestValue(shouldBeIncluded =>
                        {
                            if (shouldBeIncluded)
                            {
                                observer.OnNext(SetChange(CollectionChangeType.Add, item));
                            }
                            else
                            {
                                observer.OnNext(SetChange(CollectionChangeType.Remove, item));
                            }
                        }, ex => observer.OnError(ex)),
                    (item, disposable, removalMode) =>
                    {
                        disposable.Dispose();
                        disposable.LatestValue.IfHasValue(isIncluded =>
                        {
                            if (isIncluded)
                                observer.OnNext(SetChange(CollectionChangeType.Remove, item));
                        });
                    });
            }).ToLiveLinq();
        }
    }
}