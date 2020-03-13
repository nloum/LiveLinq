using System;
using System.Linq;
using System.Reactive.Linq;
using UtilityDisposables;
using GenericNumbers;
using static GenericNumbers.NumbersUtility;
using LiveLinq.Core;
using LiveLinq.List;
using static LiveLinq.Utility;

namespace LiveLinq
{
    public static partial class Extensions
    {
        public static IListChanges<TInner> SelectMany<TOuter, TInner>(
            this IObservable<TOuter> source,
            Func<TOuter, IListChanges<TInner>> selector)
        {
            return source.Select(selector).SelectMany();
        }

        public static IListChanges<T> SelectMany<T>(this IObservable<IListChanges<T>> source)
        {
            return source.ToLiveLinq().SelectMany(a => a);
        }

        public static IListChanges<TResult> SelectMany<TSource, TResult>(
            this IListChanges<TSource> source,
            Func<TSource, IListChanges<TResult>> selector)
        {
            return SelectManyImpl(source, (src, idx) => selector(src));
        }

        public static IListChanges<TResult> SelectMany<TSource, TResult>(
            this IListChanges<TSource> source,
            Func<TSource, IObservable<int>, IListChanges<TResult>> selector)
        {
            return SelectManyImpl(source, selector);
        }

        public static IListChanges<TResult> SelectMany<TSource, TCollection, TResult>(
            this IListChanges<TSource> source,
            Func<TSource, IListChanges<TCollection>> collectionSelector,
            Func<TSource, TCollection, TResult> resultSelector)
        {
            return SelectManyImpl(source, (src, idx) => collectionSelector(src).Select(coll => resultSelector(src, coll)));
        }

        public static IListChanges<TResult> SelectMany<TSource, TCollection, TResult>(
            this IListChanges<TSource> source,
            Func<TSource, IObservable<int>, IListChanges<TCollection>> collectionSelector,
            Func<TSource, TCollection, TResult> resultSelector)
        {
            return SelectManyImpl(source, (src, idx) => collectionSelector(src, idx).Select(coll => resultSelector(src, coll)));
        }
        
        public static IListChanges<TResult> SelectManyImpl<TSource, TResult>(IListChanges<TSource> source, Func<TSource, IObservable<int>, IListChanges<TResult>> selector)
        {
            var results = source.AsObservable().Select(
                change =>
                {
                    if (change.Type == CollectionChangeType.Add)
                        return ListChange(
                            CollectionChangeType.Add,
                            change.Range,
                            change.Values.Select(
                                (src, idx) =>
                                {
                                    var startIndex = change.Range.LowerBound.ChangeStrictness(false).Value +
                                                     idx;
                                    return selector(src,
                                        source.ItemIndices(startIndex));
                                }));
                    return ListRemove<IListChanges<TResult>>(change.Range);
                });

            var result = Observable.Create((IObserver<IListChange<TResult>> observer) =>
                SelectManySubscribe(results, observer));
            return result.ToLiveLinq();
        }

        private static IDisposable SelectManySubscribe<TResult>(IObservable<IListChange<IListChanges<TResult>>> results, IObserver<IListChange<TResult>> observer)
        {
            var subCollections = new MovingRanges<DisposableCollector>();

            return results.Subscribe(
                change =>
                    {
                        SelectManyResultsSubscription(observer, change, subCollections);
                    }).DisposeWith(
                        new AnonymousDisposable(
                            () =>
                                {
                                    // When somebody disposes the subscription to SelectMany, we need to unsubscribe everything.
                                    // All the IDisposables are in the counts collection, so that's all we need to dispose.
                                    foreach (var subCollection in subCollections.AsEnumerable())
                                    {
                                        subCollection.State.Dispose();
                                    }
                                }));
        }

        private static void SelectManyResultsSubscription<TResult>(
            IObserver<IListChange<TResult>> observer,
            IListChange<IListChanges<TResult>> change,
            MovingRanges<DisposableCollector> subCollections)
        {
            if (change.Type == CollectionChangeType.Add)
            {
                foreach (var idx in change.Range.Numbers())
                {
                    var subCollection = subCollections.Insert(idx, new DisposableCollector());
                    var subscription = change.Values[idx - change.Range.LowerBound.ChangeStrictness(false).Value].AsObservable().Scan(
                        0,
                        (count, subchange) => SelectManyTrackCountAndFireOffsetEvents(observer, subCollections, subCollection, subchange, count))
                        .Subscribe(_ => { }, observer.OnError);
                    subCollection.State.TryDisposes(subscription);
                }
            }
            else
            {
                // When a subcollection is removed, we have to stop listening to their change events
                // by disposing the Count subscription, and we need to update them so that we know
                // the range to send out as a remove event to observer.
                foreach (var idx in change.Range.Numbers())
                {
                    subCollections[idx].State.Dispose();
                    subCollections.RecalculateSubCollectionsOffset(subCollections[idx]);
                }

                // Combine the ranges
                var rangesToBeRemoved =
                    change.Range.Numbers()
                        .Select(i => subCollections[i].Range)
                        .Union()
                        .OrderByDescending(range => range.LowerBound);
                // We have to remove the furthest-to-the-right subcollection first so that we don't have
                // have to adjust the indices of the subsequent removals because they will have to be offset
                // due to the fact that items were removed to their left.
                foreach (var range in rangesToBeRemoved)
                {
                    if (range.Size > 0)
                    {
                        observer.OnNext(ListRemove<TResult>(range));
                    }
                }

                // Remove the subcollections from our collection of subcollections.
                subCollections.RemoveRange(change.Range);
            }
        }

        private static int SelectManyTrackCountAndFireOffsetEvents<TResult>(
            IObserver<IListChange<TResult>> observer,
            MovingRanges<DisposableCollector> subCollections,
            IMovingRange<DisposableCollector> subCollection,
            IListChange<TResult> subchange,
            int count)
        {
            subCollections.RecalculateSubCollectionsOffset(subCollection);

            switch (subchange.Type)
            {
                case CollectionChangeType.Add:
                    observer.OnNext(
                        ListChange(
                            CollectionChangeType.Add,
                            Range(
                                subCollection.BaseCount + subchange.Range.LowerBound.ChangeStrictness(false).Value,
                                subCollection.BaseCount + subchange.Range.UpperBound.ChangeStrictness(true).Value),
                            subchange.Values));
                    count += subchange.Range.Size;
                    break;
                case CollectionChangeType.Remove:
                    observer.OnNext(
                        ListChange(
                            CollectionChangeType.Remove,
                            Range(
                                subCollection.BaseCount + subchange.Range.LowerBound.ChangeStrictness(false).Value,
                                subCollection.BaseCount + subchange.Range.UpperBound.ChangeStrictness(true).Value),
                            subchange.Values));
                    count -= subchange.Range.Size;
                    break;
                default:
                    throw new ArgumentException();
            }

            subCollections.UpdateCount(subCollection, count);

            return count;
        }
    }
}
