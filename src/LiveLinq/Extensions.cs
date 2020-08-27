using System;
using System.Collections;
using System.Collections.Generic;
using System.Reactive;
using System.Reactive.Linq;
using System.Threading;
using ComposableCollections.Dictionary;
using SimpleMonads;
using GenericNumbers;
using MoreCollections;
using LiveLinq.Core;
using LiveLinq.List;
using UtilityDisposables;

namespace LiveLinq
{
    public static partial class Extensions
    {
        public static IObservable<T> OtherwiseEmpty<T>(this IMaybe<IObservable<T>> maybes) {
            return maybes.Otherwise(Observable.Empty<T>());
        }
        
        #region Misc
        
        private class ObservableSubscriptionWithLatestValue<T> : IObservableSubscriptionWithLatestValue<T>
        {
            private readonly IDisposable _disposable;

            public ObservableSubscriptionWithLatestValue(IDisposable disposable)
            {
                _disposable = disposable;
            }

            public IMaybe<T> LatestValue { get; set; }

            public void Dispose()
            {
                _disposable.Dispose();
            }
        }

        public static IObservableSubscriptionWithLatestValue<T> SubscribeWithLatestValue<T>(this IObservable<T> observable, Action<T> onNext, Action<Exception> onError, Action onComplete)
        {
            var disposableCollector = new DisposableCollector();
            var result = new ObservableSubscriptionWithLatestValue<T>(disposableCollector);
            var subscription = observable.Subscribe(t =>
            {
                result.LatestValue = t.ToMaybe();
                onNext(t);
            }, onError, onComplete);
            disposableCollector.Disposes(subscription);
            return result;
        }
        
        public static IObservableSubscriptionWithLatestValue<T> SubscribeWithLatestValue<T>(this IObservable<T> observable, Action<T> onNext, Action<Exception> onError)
        {
            var disposableCollector = new DisposableCollector();
            var result = new ObservableSubscriptionWithLatestValue<T>(disposableCollector);
            var subscription = observable.Subscribe(t =>
            {
                result.LatestValue = t.ToMaybe();
                onNext(t);
            }, onError);
            disposableCollector.Disposes(subscription);
            return result;
        }

        public static IObservableSubscriptionWithLatestValue<T> SubscribeWithLatestValue<T>(this IObservable<T> observable, Action<T> onNext, Action onComplete)
        {
            var disposableCollector = new DisposableCollector();
            var result = new ObservableSubscriptionWithLatestValue<T>(disposableCollector);
            var subscription = observable.Subscribe(t =>
            {
                result.LatestValue = t.ToMaybe();
                onNext(t);
            }, onComplete);
            disposableCollector.Disposes(subscription);
            return result;
        }

        public static IObservableSubscriptionWithLatestValue<T> SubscribeWithLatestValue<T>(this IObservable<T> observable, Action<T> onNext)
        {
            var disposableCollector = new DisposableCollector();
            var result = new ObservableSubscriptionWithLatestValue<T>(disposableCollector);
            var subscription = observable.Subscribe(t =>
            {
                result.LatestValue = t.ToMaybe();
                onNext(t);
            });
            disposableCollector.Disposes(subscription);
            return result;
        }

        public static IReadOnlyList<IKeyValue<TKey, TValue>> KeysAndValues<TKey, TValue>(
            this IKeyedCollectionChange<TKey, TValue> source)
        {
            return new KeysAndValuesImpl<TKey, TValue>(source);
        }
        
        private class KeysAndValuesImpl<TKey, TValue> : IReadOnlyList<IKeyValue<TKey, TValue>>
        {
            private readonly IKeyedCollectionChange<TKey, TValue> _source;

            public KeysAndValuesImpl(IKeyedCollectionChange<TKey, TValue> source)
            {
                this._source = source;
            }

            /// <summary>
            /// Returns an enumerator that iterates through a collection.
            /// </summary>
            /// <returns>
            /// An <see cref="T:System.Collections.IEnumerator"/> object that can be used to iterate through the collection.
            /// </returns>
            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.GetEnumerator();
            }

            /// <summary>
            /// Returns an enumerator that iterates through the collection.
            /// </summary>
            /// <returns>
            /// A <see cref="T:System.Collections.Generic.IEnumerator`1"/> that can be used to iterate through the collection.
            /// </returns>
            public IEnumerator<IKeyValue<TKey, TValue>> GetEnumerator()
            {
                for (var i = 0; i < _source.Keys.Count; i++)
                {
                    yield return new KeyValue<TKey, TValue>(_source.Keys[i], _source.Values[i]);
                }
            }

            /// <summary>
            /// Gets the number of elements in the collection.
            /// </summary>
            /// <returns>
            /// The number of elements in the collection. 
            /// </returns>
            public int Count => _source.Keys.Count;

            /// <summary>
            /// Gets the element at the specified index in the read-only list.
            /// </summary>
            /// <returns>
            /// The element at the specified index in the read-only list.
            /// </returns>
            /// <param name="index">The zero-based index of the element to get. </param>
            public IKeyValue<TKey, TValue> this[int index] => new KeyValue<TKey,TValue>(this._source.Keys[index], this._source.Values[index]);
        }

        //public static IObservable<IListChange<TValue>> RemoveEmptyGroups<TValue>(this IObservable<IListChange<TValue>> source)
        //{
        //    return
        //        source.Scan(new RemoveEmptyGroupState<TValue>(), AccumulateEmptyGroups)
        //            .Where(state => state.MostRecentChange != null)
        //            .Select(state => state.MostRecentChange)
        //            .Dematerialize();
        //}

        //private static RemoveEmptyGroupState<TValue> AccumulateEmptyGroups<TValue>(RemoveEmptyGroupState<TValue> state, IListChange<TValue> change)
        //{
        //    if (change.Type == CollectionChangeType.Add)
        //    {
        //        var whereAreWeAddingIndex =
        //            state.Ranges.GetIndexOfSortedInsert(
        //                range => range.BaseCount.CompareTo(change.Range.LowerBound.ChangeStrictness(false).Value));

        //        if (whereAreWeAddingIndex <= 0)
        //        {
        //            if (change.Range.LowerBound.ChangeStrictness(false).Value > 0)
        //            {
        //                state.Ranges.Insert(0, new RemoveEmptyGroupRangeState(true));
        //                state.Ranges.UpdateCount(state.Ranges[0], change.Range.LowerBound.ChangeStrictness(false).Value);

        //                state.Ranges.Insert(1, new RemoveEmptyGroupRangeState(false));
        //                state.Ranges.UpdateCount(state.Ranges[1], change.Range.Size);
        //            }
        //            else if (change.Range.LowerBound.ChangeStrictness(false).Value == 0)
        //            {
        //                state.Ranges.Insert(0, new RemoveEmptyGroupRangeState(false));
        //                state.Ranges.UpdateCount(state.Ranges[0], change.Range.Size);
        //            }
        //            else
        //            {
        //                throw new NotSupportedException("Cannot work on relative ranges");
        //            }
        //        }
        //        else if (whereAreWeAddingIndex >= state.Ranges.Count)
        //        {
        //            var lastRange = state.Ranges[state.Ranges.Count - 1];
        //            state.Ranges.RecalculateSubCollectionsOffset(lastRange);
        //            if (lastRange.BaseCount + lastRange.Count < change.Range.LowerBound.ChangeStrictness(false).Value)
        //            {
        //                state.Ranges.Insert(state.Ranges.Count, new RemoveEmptyGroupRangeState(true));
        //                state.Ranges.UpdateCount(
        //                    state.Ranges[state.Ranges.Count - 1],
        //                    change.Range.LowerBound.ChangeStrictness(false).Value - lastRange.BaseCount
        //                    + lastRange.Count);

        //                state.Ranges.Insert(state.Ranges.Count, new RemoveEmptyGroupRangeState(false));
        //                state.Ranges.UpdateCount(state.Ranges[state.Ranges.Count - 1], change.Range.Size);
        //            }
        //            else
        //            {
        //                state.Ranges.Insert(state.Ranges.Count, new RemoveEmptyGroupRangeState(false));
        //                state.Ranges.UpdateCount(state.Ranges[state.Ranges.Count - 1], change.Range.Size);
        //            }
        //        }
        //        else
        //        {
        //            var whereAreWeAdding = state.Ranges[whereAreWeAddingIndex];
        //            var ranges =
        //                whereAreWeAdding.Range.Split(whereAreWeAddingIndex, NumberRangeSplitBehavior.None).ToArray();
        //            var stateRanges = new[]
        //                                  {
        //                                      new { Range = ranges[0], whereAreWeAdding.State.IsGap },
        //                                      new
        //                                          {
        //                                              Range =
        //                                          Range(whereAreWeAddingIndex, whereAreWeAddingIndex + change.Range.Size),
        //                                              IsGap = false
        //                                          },
        //                                      new
        //                                          {
        //                                              Range =
        //                                          Range(
        //                                              whereAreWeAddingIndex + change.Range.Size + 1,
        //                                              whereAreWeAddingIndex + change.Range.Size + 1 + ranges[1].Size),
        //                                              whereAreWeAdding.State.IsGap
        //                                          },
        //                                  };

        //            var gaps = stateRanges.Where(anon => anon.IsGap).Select(anon => anon.Range).ToList();
        //            var nonGaps = stateRanges.Where(anon => !anon.IsGap).Select(anon => anon.Range).ToList();

        //            gaps = gaps.Intersection().ToList();
        //            nonGaps = nonGaps.Intersection().ToList();

        //            var allRanges =
        //                gaps.Select(range => new { Range = range, IsGap = true })
        //                    .Concat(nonGaps.Select(range => new { Range = range, IsGap = false }))
        //                    .OrderBy(anon => anon.Range.LowerBound);

        //            state.Ranges.RemoveAt(whereAreWeAddingIndex);

        //            var offset = 0;
        //            foreach (var range in allRanges)
        //            {
        //                state.Ranges.Insert(whereAreWeAddingIndex + offset, new RemoveEmptyGroupRangeState(range.IsGap));
        //                state.Ranges.UpdateCount(state.Ranges[whereAreWeAddingIndex + offset], range.Range.Size);
        //                offset++;
        //            }
        //        }

        //        return new RemoveEmptyGroupState<TValue>(state.Count, );
        //    }
        //    else
        //    {
        //        //if (change.Range.LowerBound.ChangeStrictness(false).Value >= state.Count)
        //        //{
        //        throw new NotImplementedException();
        //        //}
        //    }
        //    return state;
        //}

        //private class RemoveEmptyGroupState<TValue>
        //{
        //    public RemoveEmptyGroupState()
        //        : this(0, null, new MovingRanges<RemoveEmptyGroupRangeState>())
        //    {
        //    }

        //    public RemoveEmptyGroupState(int count, Notification<IListChange<TValue>> mostRecentChange, MovingRanges<RemoveEmptyGroupRangeState> ranges)
        //    {
        //        this.Count = count;
        //        this.MostRecentChange = mostRecentChange;
        //        this.Ranges = ranges;
        //    }

        //    public int Count { get; }
        //    public Notification<IListChange<TValue>> MostRecentChange { get; }
        //    public MovingRanges<RemoveEmptyGroupRangeState> Ranges { get; }
        //}

        //private class RemoveEmptyGroupRangeState
        //{
        //    public RemoveEmptyGroupRangeState(bool isGap)
        //    {
        //        this.IsGap = isGap;
        //    }

        //    public bool IsGap { get; }
        //}

        private class IndexAndCount
        {
            public IndexAndCount(int index, int count)
            {
                Index = index;
                Count = count;
            }

            public int Index { get; }
            public int Count { get; }
        }

        /// <summary>
        /// Returns the item indices as the item is moved around, due to items being added or removed to the query
        /// with lower indices.
        /// </summary>
        public static IObservable<int> ItemIndices<T>(this IListChanges<T> source, int startIndex)
        {
            return Observable.Return(startIndex).Concat(source.AsObservable().WithCount().Materialize()
                .Scan(Notification.CreateOnNext(0),
                (index, notification) =>
                {
                    if (notification.Kind == NotificationKind.OnError) return Notification.CreateOnError<int>(notification.Exception);
                    if (notification.Kind == NotificationKind.OnCompleted) return Notification.CreateOnCompleted<int>();
                    var changeAndCount = notification.Value;
                    if (changeAndCount.Change.Type == CollectionChangeType.Add)
                    {
                        if (changeAndCount.Change.Range.LowerBound.ChangeStrictness(false).Value <= index.Value) return Notification.CreateOnNext(index.Value + changeAndCount.Change.Range.Size);
                        return Notification.CreateOnNext(index.Value);
                    }
                    else
                    {
                        if (changeAndCount.Change.Range.Includes(index.Value)) return Notification.CreateOnCompleted<int>();
                        if (changeAndCount.Change.Range.LowerBound.ChangeStrictness(false).Value <= index.Value) return Notification.CreateOnNext(index.Value - changeAndCount.Change.Range.Size);
                        return Notification.CreateOnNext(index.Value);
                    }
                })
                .Dematerialize()
                .DistinctUntilChanged());
        }

        /// <summary>
        /// Returns an event stream that fires an event only when the item at the specified index gets removed.
        /// </summary>
        public static IObservable<Unit> ItemGetsRemoved<T>(this IListChanges<T> source, int startIndex)
        {
            return source.ItemIndices(startIndex).LastAsync().Select(_ => Unit.Default);
        }

        /// <summary>
        /// If source has a value, returns that value. Otherwise, returns an empty LiveLinq query.
        /// </summary>
        public static IListChanges<T> OtherwiseEmpty<T>(this IMaybe<IListChanges<T>> source)
        {
            return source.Otherwise(Utility.EmptyListChanges<T>());
        }

        #region Sync to a list that is readable and writeable and doesn't have change notifications

        /// <summary>
        ///     Keeps a list in sync with any changes made to the specified ObservableCollection.
        /// </summary>
        /// <param name="avoidInitialDuplicates">Indicates whether we should skip the first event. Normally,
        /// the first event represents the initial state of the source list. If the initial state of the source
        /// and target lists are already synced, then you should specify this as false. Otherwise, this should be true.</param>
        public static IDisposable SyncTo<T>(
            this IListChanges<T> source,
            IList<T> target, bool avoidInitialDuplicates)
        {
            if (avoidInitialDuplicates)
                return source.AsObservable().Skip(1).Subscribe(target.Write);
            return source.AsObservable().Subscribe(target.Write);
        }
        
        #endregion

        #endregion
    }
}
