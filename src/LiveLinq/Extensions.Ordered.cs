using System;
using System.Collections.Immutable;
using System.Linq;
using System.Reactive.Linq;
using ComposableCollections.Dictionary;
using MoreCollections;
using static LiveLinq.Utility;
using GenericNumbers.Relational;
using LiveLinq.Core;
using LiveLinq.Dictionary;
using LiveLinq.List;
using LiveLinq.Ordered;
using LiveLinq.Set;

namespace LiveLinq
{
    public static partial class Extensions
    {
        #region List

        #region OrderBy

        /// <summary>
        /// Creates a LiveLinq query that you can call ThenBy/ThenByDescending on, and defines the primary sort key
        /// as the comparer parameter, in ascending order.
        /// This is identical to the semantics of LINQ's OrderBy, ThenBy, OrderByDescending, and ThenByDescending.
        /// 
        /// Example:
        /// 
        /// var orderedByName = liveLinqQuery.OrderBy(x => x.Name);
        /// var orderedByNameThenByAge = orderedByName.ThenBy(x => x.Age);
        /// 
        /// The purpose of this code is so that you can do things like this:
        /// 
        /// var orderedByNameThenByAge = liveLinqQuery.OrderBy(x => x.Name).ThenBy(x => x.Age);
        /// 
        /// Just like in LINQ.
        /// </summary>
        public static IOrderedListChanges<TSource> OrderBy<TSource>(this IListChangesStrict<TSource> source, Func<TSource, TSource, int> comparer)
        {
            return Sort<TSource>(source.AsObservable(), comparer);
        }

        /// <summary>
        /// Creates a LiveLinq query that you can call ThenBy/ThenByDescending on, and defines the primary sort key
        /// as the comparer parameter, in descending order.
        /// This is identical to the semantics of LINQ's OrderBy, ThenBy, OrderByDescending, and ThenByDescending.
        /// 
        /// Example:
        /// 
        /// var orderedByName = liveLinqQuery.OrderByDescending(x => x.Name);
        /// var orderedByNameThenByAge = orderedByName.ThenBy(x => x.Age);
        /// 
        /// The purpose of this code is so that you can do things like this:
        /// 
        /// var orderedByNameThenByAge = liveLinqQuery.OrderByDescending(x => x.Name).ThenBy(x => x.Age);
        /// 
        /// Just like in LINQ.
        /// </summary>
        public static IOrderedListChanges<TSource> OrderByDescending<TSource>(this IListChangesStrict<TSource> source, Func<TSource, TSource, int> comparer)
        {
            return Sort<TSource>(source.AsObservable(), (a, b) => -1 * comparer(a, b));
        }

        /// <summary>
        /// Creates a LiveLinq query that you can call ThenBy/ThenByDescending on, and defines the primary sort key
        /// as the comparer parameter, in ascending order.
        /// This is identical to the semantics of LINQ's OrderBy, ThenBy, OrderByDescending, and ThenByDescending.
        /// 
        /// Example:
        /// 
        /// var orderedByName = liveLinqQuery.OrderBy(x => x.Name);
        /// var orderedByNameThenByAge = orderedByName.ThenBy(x => x.Age);
        /// 
        /// The purpose of this code is so that you can do things like this:
        /// 
        /// var orderedByNameThenByAge = liveLinqQuery.OrderBy(x => x.Name).ThenBy(x => x.Age);
        /// 
        /// Just like in LINQ.
        /// </summary>
        public static IOrderedListChanges<TSource> OrderBy<TSource, TKey>(this IListChangesStrict<TSource> source, Func<TSource, TKey> keySelector)
        {
            return Sort<TSource>(source.AsObservable(), (a, b) => keySelector(a).CompareTo(keySelector(b)));
        }

        /// <summary>
        /// Creates a LiveLinq query that you can call ThenBy/ThenByDescending on, and defines the primary sort key
        /// as the comparer parameter, in descending order.
        /// This is identical to the semantics of LINQ's OrderBy, ThenBy, OrderByDescending, and ThenByDescending.
        /// 
        /// Example:
        /// 
        /// var orderedByName = liveLinqQuery.OrderByDescending(x => x.Name);
        /// var orderedByNameThenByAge = orderedByName.ThenBy(x => x.Age);
        /// 
        /// The purpose of this code is so that you can do things like this:
        /// 
        /// var orderedByNameThenByAge = liveLinqQuery.OrderByDescending(x => x.Name).ThenBy(x => x.Age);
        /// 
        /// Just like in LINQ.
        /// </summary>
        public static IOrderedListChanges<TSource> OrderByDescending<TSource, TKey>(this IListChangesStrict<TSource> source, Func<TSource, TKey> keySelector)
        {
            return Sort<TSource>(source.AsObservable(), (a, b) => -1 * keySelector(a).CompareTo(keySelector(b)));
        }

        #endregion

        #region ThenBy

        /// <summary>
        /// Defines the next sort key for the sorted LiveLinq query, in ascending order.
        /// This is identical to the semantics of LINQ's OrderBy, ThenBy, OrderByDescending, and ThenByDescending.
        /// 
        /// Example:
        /// 
        /// var orderedByName = liveLinqQuery.OrderBy(x => x.Name);
        /// var orderedByNameThenByAge = orderedByName.ThenBy(x => x.Age);
        /// 
        /// The purpose of this code is so that you can do things like this:
        /// 
        /// var orderedByNameThenByAge = liveLinqQuery.OrderBy(x => x.Name).ThenBy(x => x.Age);
        /// 
        /// Just like in LINQ.
        /// </summary>
        public static IOrderedListChanges<TSource> ThenBy<TSource>(this IOrderedListChanges<TSource> source, Func<TSource, TSource, int> comparer)
        {
            return Sort<TSource>(source.AsObservable(), (a, b) =>
            {
                var result = ((OrderedListChanges<TSource>)source).Comparer(a, b);
                if (result == 0)
                    result = comparer(a, b);
                return result;
            });
        }

        /// <summary>
        /// Defines the next sort key for the sorted LiveLinq query, in descending order.
        /// This is identical to the semantics of LINQ's OrderBy, ThenBy, OrderByDescending, and ThenByDescending.
        /// 
        /// Example:
        /// 
        /// var orderedByName = liveLinqQuery.OrderBy(x => x.Name);
        /// var orderedByNameThenByAge = orderedByName.ThenByDescending(x => x.Age);
        /// 
        /// The purpose of this code is so that you can do things like this:
        /// 
        /// var orderedByNameThenByAge = liveLinqQuery.OrderBy(x => x.Name).ThenByDescending(x => x.Age);
        /// 
        /// Just like in LINQ.
        /// </summary>
        public static IOrderedListChanges<TSource> ThenByDescending<TSource>(this IOrderedListChanges<TSource> source, Func<TSource, TSource, int> comparer)
        {
            return Sort<TSource>(source.AsObservable(), (a, b) => {
                var result = ((OrderedListChanges<TSource>)source).Comparer(a, b);
                if (result == 0)
                    result = -1 * comparer(a, b);
                return result;
            });
        }

        /// <summary>
        /// Defines the next sort key for the sorted LiveLinq query, in ascending order.
        /// This is identical to the semantics of LINQ's OrderBy, ThenBy, OrderByDescending, and ThenByDescending.
        /// 
        /// Example:
        /// 
        /// var orderedByName = liveLinqQuery.OrderBy(x => x.Name);
        /// var orderedByNameThenByAge = orderedByName.ThenBy(x => x.Age);
        /// 
        /// The purpose of this code is so that you can do things like this:
        /// 
        /// var orderedByNameThenByAge = liveLinqQuery.OrderBy(x => x.Name).ThenBy(x => x.Age);
        /// 
        /// Just like in LINQ.
        /// </summary>
        public static IOrderedListChanges<TSource> ThenBy<TSource, TKey>(this IOrderedListChanges<TSource> source, Func<TSource, TKey> keySelector)
        {
            return Sort<TSource>(source.AsObservable(), (a, b) =>
            {
                var result = ((OrderedListChanges<TSource>)source).Comparer(a, b);
                if (result != 0)
                    return result;
                result = keySelector(a).CompareTo(keySelector(b));
                return result;
            });
        }

        /// <summary>
        /// Defines the next sort key for the sorted LiveLinq query, in descending order.
        /// This is identical to the semantics of LINQ's OrderBy, ThenBy, OrderByDescending, and ThenByDescending.
        /// 
        /// Example:
        /// 
        /// var orderedByName = liveLinqQuery.OrderBy(x => x.Name);
        /// var orderedByNameThenByAge = orderedByName.ThenByDescending(x => x.Age);
        /// 
        /// The purpose of this code is so that you can do things like this:
        /// 
        /// var orderedByNameThenByAge = liveLinqQuery.OrderBy(x => x.Name).ThenByDescending(x => x.Age);
        /// 
        /// Just like in LINQ.
        /// </summary>
        public static IOrderedListChanges<TSource> ThenByDescending<TSource, TKey>(this IOrderedListChanges<TSource> source, Func<TSource, TKey> keySelector)
        {
            return Sort<TSource>(source.AsObservable(), (a, b) =>
            {
                var result = ((OrderedListChanges<TSource>)source).Comparer(a, b);
                if (result == 0)
                    result = -1 * keySelector(a).CompareTo(keySelector(b));
                return result;
            });
        }

        #endregion

        #region GroupBy
        
        /// <summary>
        ///     Returns a LiveLinq query that is equivalent to the LINQ GroupBy extension method, but is always updated
        ///     when the source or key change.
        /// </summary>
        public static IDictionaryChanges<TKey, IListChanges<TValue>> GroupBy<TSource, TKey, TValue>(
            this IOrderedListChanges<TSource> source,
            Func<TSource, IObservable<int>, IObservable<TKey>> keySelector,
            Func<TSource, IObservable<int>, IObservable<TValue>> valueSelector)
        {
            var keysAndValues = source.Select((t, idx) => CombineLatestKeysAndValues(t, idx, keySelector,
                (t2, idx2) => valueSelector(t2, idx2).Select(value => new ValueAndSource<TValue, TSource>(value, t))))
                .AsObservable()
                .SelectMany(change => change.Itemize())
                .Publish()
                .RefCount();

            return keysAndValues
                .Where(change => change.Type == CollectionChangeType.Add)
                .GroupBy(kvp => kvp.Values.First().Key)
                .Select(grouping =>
                    {
                        var simplifiedGrouping = keysAndValues
                            .Where(change => Equals(change.Values.First().Key, grouping.Key))
                            .Select(change => ListChange(change.Type, change.Range, change.Values.Select(kvp => kvp.Value.Value)))
                            .ToLiveLinq();
                        return DictionaryAdd(new KeyValue<TKey, IListChanges<TValue>>(grouping.Key, simplifiedGrouping));
                    })
                    .ToLiveLinq();
        }
        
        private class ValueAndSource<TValue, TSource>
        {
            public TValue Value { get; }
            public TSource Source { get; }

            /// <summary>
            /// Initializes a new instance of the <see cref="T:System.Object"/> class.
            /// </summary>
            public ValueAndSource(TValue value, TSource source)
            {
                this.Value = value;
                this.Source = source;
            }
        }

        #endregion

        private static IOrderedListChanges<TSource> Sort<TSource>(this IObservable<IListChangeStrict<TSource>> source, Func<TSource, TSource, int> comparer)
        {
            return new OrderedListChanges<TSource>(source.AsObservable().Scan(
                new OrderedStateAndChange<TSource>(),
                (state, change) => state.SortedWrite(change, comparer))
                .Select(sac => sac.MostRecentChanges.ToObservable()).Concat(),
                comparer);
        }

        private class OrderedStateAndChange<TSource>
        {
            public ImmutableList<TSource> Items { get; }
            public ImmutableList<IListChangeStrict<TSource>> MostRecentChanges { get; }

            /// <summary>
            /// Maps original indices to the sorted indices.
            /// </summary>
            public ImmutableDictionary<int, int> IndexMap { get; }

            public OrderedStateAndChange()
            {
                Items = ImmutableList<TSource>.Empty;
                this.IndexMap = ImmutableDictionary<int, int>.Empty;
                this.MostRecentChanges = ImmutableList<IListChangeStrict<TSource>>.Empty;
            }

            public OrderedStateAndChange(ImmutableList<TSource> items, ImmutableDictionary<int, int> indexMap, ImmutableList<IListChangeStrict<TSource>> mostRecentChanges)
            {
                this.Items = items;
                this.IndexMap = indexMap;
                this.MostRecentChanges = mostRecentChanges;
            }
        }

        private static OrderedStateAndChange<TSource> SortedWrite<TSource>(this OrderedStateAndChange<TSource> subject, IListChangeStrict<TSource> change, Func<TSource, TSource, int> comparer)
        {
            switch (change.Type)
            {
                case CollectionChangeType.Add:
                    {
                        var newItems = subject.Items;
                        var newChanges = ImmutableList<IListChangeStrict<TSource>>.Empty;
                        var indexMap = subject.IndexMap;
                        var originalIndex = change.Range.LowerBound.IsStrict
                                                ? change.Range.LowerBound.Value + 1
                                                : change.Range.LowerBound.Value;
                        foreach (var item in change.Values)
                        {
                            var index = newItems.GetIndexOfSortedInsert(item, comparer);
                            newChanges = newChanges.Add(ListChangeStrict(CollectionChangeType.Add, index, new[] { item }));
                            newItems = newItems.Insert(index, item);
                            indexMap = indexMap.Add(originalIndex, index);
                            originalIndex++;
                        }
                        return new OrderedStateAndChange<TSource>(newItems, indexMap, newChanges);
                    }
                case CollectionChangeType.Remove:
                    {
                        var newItems = subject.Items;
                        var newChanges = ImmutableList<IListChangeStrict<TSource>>.Empty;
                        var indexMap = subject.IndexMap;
                        var originalIndex = change.Range.LowerBound.IsStrict
                                                ? change.Range.LowerBound.Value + 1
                                                : change.Range.LowerBound.Value;
                        foreach (var item in change.Values)
                        {
                            var index = newItems.GetIndexOfSortedInsert(item, comparer);
                            newChanges = newChanges.Add(ListChangeStrict(CollectionChangeType.Remove, index, new[] { item }));
                            newItems = newItems.RemoveAt(indexMap[originalIndex]);
                            indexMap = indexMap.Remove(originalIndex);
                            originalIndex++;
                        }
                        return new OrderedStateAndChange<TSource>(newItems.ToImmutableList(), indexMap, newChanges.ToImmutableList());
                    }
                default:
                    throw new NotImplementedException();
            }
        }
        #endregion

        #region Set

        /// <summary>
        /// Creates a LiveLinq query that you can call ThenBy/ThenByDescending on, and defines the primary sort key
        /// as the comparer parameter, in ascending order.
        /// This is identical to the semantics of LINQ's OrderBy, ThenBy, OrderByDescending, and ThenByDescending.
        /// 
        /// Example:
        /// 
        /// var orderedByName = liveLinqQuery.OrderBy(x => x.Name);
        /// var orderedByNameThenByAge = orderedByName.ThenBy(x => x.Age);
        /// 
        /// The purpose of this code is so that you can do things like this:
        /// 
        /// var orderedByNameThenByAge = liveLinqQuery.OrderBy(x => x.Name).ThenBy(x => x.Age);
        /// 
        /// Just like in LINQ.
        /// </summary>
        public static IOrderedListChanges<TValue> OrderBy<TValue>(this ISetChanges<TValue> source, Func<TValue, TValue, int> comparer)
        {
            return Sort(source.SetChangesToListChangesSortedByKey().AsObservable(), comparer);
        }

        /// <summary>
        /// Creates a LiveLinq query that you can call ThenBy/ThenByDescending on, and defines the primary sort key
        /// as the comparer parameter, in descending order.
        /// This is identical to the semantics of LINQ's OrderBy, ThenBy, OrderByDescending, and ThenByDescending.
        /// 
        /// Example:
        /// 
        /// var orderedByName = liveLinqQuery.OrderByDescending(x => x.Name);
        /// var orderedByNameThenByAge = orderedByName.ThenBy(x => x.Age);
        /// 
        /// The purpose of this code is so that you can do things like this:
        /// 
        /// var orderedByNameThenByAge = liveLinqQuery.OrderByDescending(x => x.Name).ThenBy(x => x.Age);
        /// 
        /// Just like in LINQ.
        /// </summary>
        public static IOrderedListChanges<TValue> OrderByDescending<TValue>(this ISetChanges<TValue> source, Func<TValue, TValue, int> comparer)
        {
            return Sort(source.SetChangesToListChangesSortedByKey().AsObservable(), (a, b) => -1 * comparer(a, b));
        }

        /// <summary>
        /// Creates a LiveLinq query that you can call ThenBy/ThenByDescending on, and defines the primary sort key
        /// as the comparer parameter, in ascending order.
        /// This is identical to the semantics of LINQ's OrderBy, ThenBy, OrderByDescending, and ThenByDescending.
        /// 
        /// Example:
        /// 
        /// var orderedByName = liveLinqQuery.OrderBy(x => x.Name);
        /// var orderedByNameThenByAge = orderedByName.ThenBy(x => x.Age);
        /// 
        /// The purpose of this code is so that you can do things like this:
        /// 
        /// var orderedByNameThenByAge = liveLinqQuery.OrderBy(x => x.Name).ThenBy(x => x.Age);
        /// 
        /// Just like in LINQ.
        /// </summary>
        public static IOrderedListChanges<TValue> OrderBy<TValue, TSortKey>(this ISetChanges<TValue> source, Func<TValue, TSortKey> keySelector)
        {
            return Sort(source.SetChangesToListChangesSortedByKey().AsObservable(), (a, b) => keySelector(a).CompareTo(keySelector(b)));
        }

        /// <summary>
        /// Creates a LiveLinq query that you can call ThenBy/ThenByDescending on, and defines the primary sort key
        /// as the comparer parameter, in descending order.
        /// This is identical to the semantics of LINQ's OrderBy, ThenBy, OrderByDescending, and ThenByDescending.
        /// 
        /// Example:
        /// 
        /// var orderedByName = liveLinqQuery.OrderByDescending(x => x.Name);
        /// var orderedByNameThenByAge = orderedByName.ThenBy(x => x.Age);
        /// 
        /// The purpose of this code is so that you can do things like this:
        /// 
        /// var orderedByNameThenByAge = liveLinqQuery.OrderByDescending(x => x.Name).ThenBy(x => x.Age);
        /// 
        /// Just like in LINQ.
        /// </summary>
        public static IOrderedListChanges<TValue> OrderByDescending<TValue, TSortKey>(this ISetChanges<TValue> source, Func<TValue, TSortKey> keySelector)
        {
            return Sort(source.SetChangesToListChangesSortedByKey().AsObservable(), (a, b) => -1 * keySelector(a).CompareTo(keySelector(b)));
        }

        public static IListChangesStrict<TValue> SetChangesToListChangesSortedByKey<TValue>(
            this ISetChanges<TValue> source)
        {
            return source
                .AsObservable()
                .Select(change => change.Itemize().ToObservable())
                .Concat()
                .Scan(new { List = ImmutableList<TValue>.Empty, Change = (IListChangeStrict<TValue>)null },
                    (state, change) =>
                    {
                        if (change.Type == CollectionChangeType.Add)
                        {
                            var index = state.List.GetIndexOfSortedInsert(kvp => kvp.CompareTo(change.Values[0]));
                            return new { List = state.List.Insert(index, change.Values[0]), Change = ListChangeStrict(CollectionChangeType.Add, index, change.Values.ToArray()) };
                        }
                        else
                        {
                            var index = state.List.BinarySearch(kvp => kvp.CompareTo(change.Values[0]));
                            var item = state.List[index];
                            return new { List = state.List.RemoveAt(index), Change = ListChangeStrict(CollectionChangeType.Remove, index, item) };
                        }
                    })
                .Select(anon => anon.Change)
                .ToLiveLinq();
        }

        #endregion

        #region Dictionary

        /// <summary>
        /// Creates a LiveLinq query that you can call ThenBy/ThenByDescending on, and defines the primary sort key
        /// as the comparer parameter, in ascending order.
        /// This is identical to the semantics of LINQ's OrderBy, ThenBy, OrderByDescending, and ThenByDescending.
        /// 
        /// Example:
        /// 
        /// var orderedByName = liveLinqQuery.OrderBy(x => x.Name);
        /// var orderedByNameThenByAge = orderedByName.ThenBy(x => x.Age);
        /// 
        /// The purpose of this code is so that you can do things like this:
        /// 
        /// var orderedByNameThenByAge = liveLinqQuery.OrderBy(x => x.Name).ThenBy(x => x.Age);
        /// 
        /// Just like in LINQ.
        /// </summary>
        public static IOrderedListChanges<IKeyValue<TKey, TValue>> OrderBy<TKey, TValue>(this IDictionaryChangesStrict<TKey, TValue> source, Func<IKeyValue<TKey, TValue>, IKeyValue<TKey, TValue>, int> comparer)
        {
            return Sort(source.DictionaryChangesToListChangesSortedByKey().AsObservable(), comparer);
        }

        /// <summary>
        /// Creates a LiveLinq query that you can call ThenBy/ThenByDescending on, and defines the primary sort key
        /// as the comparer parameter, in descending order.
        /// This is identical to the semantics of LINQ's OrderBy, ThenBy, OrderByDescending, and ThenByDescending.
        /// 
        /// Example:
        /// 
        /// var orderedByName = liveLinqQuery.OrderByDescending(x => x.Name);
        /// var orderedByNameThenByAge = orderedByName.ThenBy(x => x.Age);
        /// 
        /// The purpose of this code is so that you can do things like this:
        /// 
        /// var orderedByNameThenByAge = liveLinqQuery.OrderByDescending(x => x.Name).ThenBy(x => x.Age);
        /// 
        /// Just like in LINQ.
        /// </summary>
        public static IOrderedListChanges<IKeyValue<TKey, TValue>> OrderByDescending<TKey, TValue>(this IDictionaryChangesStrict<TKey, TValue> source, Func<IKeyValue<TKey, TValue>, IKeyValue<TKey, TValue>, int> comparer)
        {
            return Sort(source.DictionaryChangesToListChangesSortedByKey().AsObservable(), (a, b) => -1 * comparer(a, b));
        }

        /// <summary>
        /// Creates a LiveLinq query that you can call ThenBy/ThenByDescending on, and defines the primary sort key
        /// as the comparer parameter, in ascending order.
        /// This is identical to the semantics of LINQ's OrderBy, ThenBy, OrderByDescending, and ThenByDescending.
        /// 
        /// Example:
        /// 
        /// var orderedByName = liveLinqQuery.OrderBy(x => x.Name);
        /// var orderedByNameThenByAge = orderedByName.ThenBy(x => x.Age);
        /// 
        /// The purpose of this code is so that you can do things like this:
        /// 
        /// var orderedByNameThenByAge = liveLinqQuery.OrderBy(x => x.Name).ThenBy(x => x.Age);
        /// 
        /// Just like in LINQ.
        /// </summary>
        public static IOrderedListChanges<IKeyValue<TKey, TValue>> OrderBy<TKey, TValue, TSortKey>(this IDictionaryChangesStrict<TKey, TValue> source, Func<IKeyValue<TKey, TValue>, TSortKey> keySelector)
        {
            return Sort(source.DictionaryChangesToListChangesSortedByKey().AsObservable(), (a, b) => keySelector(a).CompareTo(keySelector(b)));
        }

        /// <summary>
        /// Creates a LiveLinq query that you can call ThenBy/ThenByDescending on, and defines the primary sort key
        /// as the comparer parameter, in descending order.
        /// This is identical to the semantics of LINQ's OrderBy, ThenBy, OrderByDescending, and ThenByDescending.
        /// 
        /// Example:
        /// 
        /// var orderedByName = liveLinqQuery.OrderByDescending(x => x.Name);
        /// var orderedByNameThenByAge = orderedByName.ThenBy(x => x.Age);
        /// 
        /// The purpose of this code is so that you can do things like this:
        /// 
        /// var orderedByNameThenByAge = liveLinqQuery.OrderByDescending(x => x.Name).ThenBy(x => x.Age);
        /// 
        /// Just like in LINQ.
        /// </summary>
        public static IOrderedListChanges<IKeyValue<TKey, TValue>> OrderByDescending<TKey, TValue, TSortKey>(this IDictionaryChangesStrict<TKey, TValue> source, Func<IKeyValue<TKey, TValue>, TSortKey> keySelector)
        {
            return Sort(source.DictionaryChangesToListChangesSortedByKey().AsObservable(), (a, b) => -1 * keySelector(a).CompareTo(keySelector(b)));
        }

        public static IListChangesStrict<IKeyValue<TKey, TValue>> DictionaryChangesToListChangesSortedByKey<TKey, TValue>(
            this IDictionaryChangesStrict<TKey, TValue> source)
        {
            return source
                .AsObservable()
                .Select(change => change.Itemize().ToObservable())
                .Concat()
                .Scan(new { List = ImmutableList<IKeyValue<TKey, TValue>>.Empty, Change = (IListChangeStrict<IKeyValue<TKey, TValue>>)null },
                    (state, change) =>
                    {
                        if (change.Type == CollectionChangeType.Add)
                        {
                            var index = state.List.GetIndexOfSortedInsert(kvp => kvp.Key.CompareTo(change.KeyValuePairs[0].Key));
                            return new { List = state.List.Insert(index, change.KeyValuePairs[0]), Change = ListChangeStrict(CollectionChangeType.Add, index, change.KeyValuePairs.ToArray()) };
                        }
                        else
                        {
                            var index = state.List.BinarySearch(kvp => kvp.Key.CompareTo(change.KeyValuePairs[0].Key));
                            var item = state.List[index];
                            return new { List = state.List.RemoveAt(index), Change = ListChangeStrict(CollectionChangeType.Remove, index, item) };
                        }
                    })
                .Select(anon => anon.Change)
                .ToLiveLinq();
        }

        #endregion
    }
}
