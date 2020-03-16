using System;
using System.Linq;
using System.Reactive.Linq;
using MoreCollections;
using LiveLinq.Core;
using LiveLinq.Dictionary;
using LiveLinq.List;
using static MoreCollections.Utility;
using static LiveLinq.Utility;

namespace LiveLinq
{
    public static partial class Extensions
    {
        /// <summary>
        ///     This is similar to the indexing method of <see cref="Dictionary{TKey,TValue}" />, except
        ///     this returns an event stream of values at that specific key.
        /// </summary>
        [Obsolete("This isn't working yet. Convert the dictionary to ISetChanges and use then use that GroupBy extension method.", true)]
        public static IListChanges<TValue> Get<TKey, TValue>(
            this IDictionaryChanges<TKey, IListChanges<TValue>> source,
            TKey key)
        {
            return source.Get(Observable.Return(key));
        }

        /// <summary>
        ///     This is similar to the indexing method of <see cref="Dictionary{TKey, TValue}" />, except
        ///     this returns an event stream of values at that specific key.
        /// </summary>
        /// <param name="source">The grouped LiveLinq query that contains the group of items whose key is <see cref="key"/>.</param>
        /// <param name="key">
        ///     The event stream of keys that you are interested in. Firing a new event in this
        ///     event stream causes the new key to be looked up and its value fired as an event in the return value
        ///     of this function.
        /// </param>
        [Obsolete("This isn't working yet. Convert the dictionary to ISetChanges and use then use that GroupBy extension method.", true)]
        public static IListChanges<TValue> Get<TKey, TValue>(
            this IDictionaryChanges<TKey, IListChanges<TValue>> source,
            IObservable<TKey> key)
        {
            return source[key].Select(m => m.OtherwiseEmpty()).SelectMany();
        }

        /// <summary>
        ///     Returns a LiveLinq query that is equivalent to the LINQ GroupBy extension method, but is always updated
        ///     when the source changes.
        /// </summary>
        [Obsolete("This isn't working yet. Convert the dictionary to ISetChanges and use then use that GroupBy extension method.", true)]
        public static IDictionaryChanges<TKey, IListChanges<TValue>> GroupBy<TKey, TValue>(
            this IListChanges<TValue> source,
            Func<TValue, TKey> keySelector)
        {
            return source.GroupBy(t => Observable.Return(keySelector(t)), Observable.Return);
        }

        /// <summary>
        ///     Returns a LiveLinq query that is equivalent to the LINQ GroupBy extension method, but is always updated
        ///     when the source or key changes.
        /// </summary>
        [Obsolete("This isn't working yet. Convert the dictionary to ISetChanges and use then use that GroupBy extension method.", true)]
        public static IDictionaryChanges<TKey, IListChanges<TValue>> GroupBy<TKey, TValue>(
            this IListChanges<TValue> source,
            Func<TValue, IObservable<TKey>> keySelector)
        {
            return source.GroupBy(keySelector, Observable.Return);
        }

        /// <summary>
        ///     Returns a LiveLinq query that is equivalent to the LINQ GroupBy extension method, but is always updated
        ///     when the source changes.
        /// </summary>
        [Obsolete("This isn't working yet. Convert the dictionary to ISetChanges and use then use that GroupBy extension method.", true)]
        public static IDictionaryChanges<TKey, IListChanges<TValue>> GroupBy<T, TKey, TValue>(
            this IListChanges<T> source,
            Func<T, TKey> keySelector,
            Func<T, TValue> valueSelector)
        {
            return source.GroupBy(t => Observable.Return(keySelector(t)), t => Observable.Return(valueSelector(t)));
        }

        /// <summary>
        ///     Returns a LiveLinq query that is equivalent to the LINQ GroupBy extension method, but is always updated
        ///     when the source or value change.
        /// </summary>
        [Obsolete("This isn't working yet. Convert the dictionary to ISetChanges and use then use that GroupBy extension method.", true)]
        public static IDictionaryChanges<TKey, IListChanges<TValue>> GroupBy<T, TKey, TValue>(
            this IListChanges<T> source,
            Func<T, TKey> keySelector,
            Func<T, IObservable<TValue>> valueSelector)
        {
            return source.GroupBy(t => Observable.Return(keySelector(t)), valueSelector);
        }

        /// <summary>
        ///     Returns a LiveLinq query that is equivalent to the LINQ GroupBy extension method, but is always updated
        ///     when the source or key change.
        /// </summary>
        [Obsolete("This isn't working yet. Convert the dictionary to ISetChanges and use then use that GroupBy extension method.", true)]
        public static IDictionaryChanges<TKey, IListChanges<TValue>> GroupBy<T, TKey, TValue>(
            this IListChanges<T> source,
            Func<T, IObservable<TKey>> keySelector,
            Func<T, TValue> valueSelector)
        {
            return source.GroupBy(keySelector, t => Observable.Return(valueSelector(t)));
        }

        /// <summary>
        ///     Returns a LiveLinq query that is equivalent to the LINQ GroupBy extension method, but is always updated
        ///     when the source, key, or value change.
        /// </summary>
        [Obsolete("This isn't working yet. Convert the dictionary to ISetChanges and use then use that GroupBy extension method.", true)]
        public static IDictionaryChanges<TKey, IListChanges<TValue>> GroupBy<T, TKey, TValue>(
            this IListChanges<T> source,
            Func<T, IObservable<TKey>> keySelector,
            Func<T, IObservable<TValue>> valueSelector)
        {
            return source.GroupBy((t, idx) => keySelector(t), (t, idx) => valueSelector(t));
        }

        /// <summary>
        ///     Returns a LiveLinq query that is equivalent to the LINQ GroupBy extension method, but is always updated
        ///     when the source changes.
        /// </summary>
        [Obsolete("This isn't working yet. Convert the dictionary to ISetChanges and use then use that GroupBy extension method.", true)]
        public static IDictionaryChanges<TKey, IListChanges<TValue>> GroupBy<TKey, TValue>(
            this IListChanges<TValue> source,
            Func<TValue, IObservable<int>, TKey> keySelector)
        {
            return source.GroupBy(keySelector, (t, _) => t);
        }

        /// <summary>
        ///     Returns a LiveLinq query that is equivalent to the LINQ GroupBy extension method, but is always updated
        ///     when the source or key changes.
        /// </summary>
        [Obsolete("This isn't working yet. Convert the dictionary to ISetChanges and use then use that GroupBy extension method.", true)]
        public static IDictionaryChanges<TKey, IListChanges<TValue>> GroupBy<TKey, TValue>(
            this IListChanges<TValue> source,
            Func<TValue, IObservable<int>, IObservable<TKey>> keySelector)
        {
            return source.GroupBy(keySelector, (t, _) => t);
        }

        /// <summary>
        ///     Returns a LiveLinq query that is equivalent to the LINQ GroupBy extension method, but is always updated
        ///     when the source changes.
        /// </summary>
        [Obsolete("This isn't working yet. Convert the dictionary to ISetChanges and use then use that GroupBy extension method.", true)]
        public static IDictionaryChanges<TKey, IListChanges<TValue>> GroupBy<T, TKey, TValue>(
            this IListChanges<T> source,
            Func<T, IObservable<int>, TKey> keySelector,
            Func<T, IObservable<int>, TValue> valueSelector)
        {
            Func<T, IObservable<int>, IObservable<TKey>> refinedKeySelector = (t, idx) => Observable.Return(keySelector(t, idx));
            Func<T, IObservable<int>, IObservable<TValue>> refinedValueSelector = (t, idx) => Observable.Return(valueSelector(t, idx));

            return source.GroupBy<T, TKey, TValue>(
                refinedKeySelector,
                refinedValueSelector);
        }

        /// <summary>
        ///     Returns a LiveLinq query that is equivalent to the LINQ GroupBy extension method, but is always updated
        ///     when the source or value change.
        /// </summary>
        [Obsolete("This isn't working yet. Convert the dictionary to ISetChanges and use then use that GroupBy extension method.", true)]
        public static IDictionaryChanges<TKey, IListChanges<TValue>> GroupBy<T, TKey, TValue>(
            this IListChanges<T> source,
            Func<T, IObservable<int>, TKey> keySelector,
            Func<T, IObservable<int>, IObservable<TValue>> valueSelector)
        {
            return source.GroupBy((t, idx) => Observable.Return(keySelector(t, idx)), valueSelector);
        }

        /// <summary>
        ///     Returns a LiveLinq query that is equivalent to the LINQ GroupBy extension method, but is always updated
        ///     when the source or key change.
        /// </summary>
        [Obsolete("This isn't working yet. Convert the dictionary to ISetChanges and use then use that GroupBy extension method.", true)]
        public static IDictionaryChanges<TKey, IListChanges<TValue>> GroupBy<T, TKey, TValue>(
            this IListChanges<T> source,
            Func<T, IObservable<int>, IObservable<TKey>> keySelector,
            Func<T, IObservable<int>, TValue> valueSelector)
        {
            return source.GroupBy(keySelector, (t, idx) => Observable.Return(valueSelector(t, idx)));
        }

        /// <summary>
        ///     Returns a LiveLinq query that is equivalent to the LINQ GroupBy extension method, but is always updated
        ///     when the source or key change.
        /// </summary>
        [Obsolete("This isn't working yet. Convert the dictionary to ISetChanges and use then use that GroupBy extension method.", true)]
        public static IDictionaryChanges<TKey, IListChanges<TValue>> GroupBy<T, TKey, TValue>(
            this IListChanges<T> source,
            Func<T, IObservable<int>, IObservable<TKey>> keySelector,
            Func<T, IObservable<int>, IObservable<TValue>> valueSelector)
        {
            var keysAndValues = source.Select((t, idx) => CombineLatestKeysAndValues(t, idx, keySelector, valueSelector))
                .AsObservable()
                .SelectMany(listChange => listChange.Itemize())
                .Publish().RefCount();

            var result = keysAndValues
                .Where(listChange => listChange.Type == CollectionChangeType.Add)
                .Select(listChange => listChange.Values.First())
                .Distinct(keyValuePair => keyValuePair.Key)
                .Select(
                    keyValuePair =>
                    KeyValuePair(
                        keyValuePair.Key,
                        keysAndValues.Where(change => change.Values.First().Key.Equals(keyValuePair.Key)).Select(change => ListChange(change.Type, change.Range, change.Values.Select(kvp => kvp.Value))).ToLiveLinq()))
                .Collect()
                .Where(kvp => kvp.Value.Any())
                .MakeStrictExpensively()
                .ToLiveLinq();
            
            return result;
        }

        private static IObservable<IKeyValuePair<TKey, TValue>> CombineLatestKeysAndValues<T, TKey, TValue>(
            T t,
            IObservable<int> idx,
            Func<T, IObservable<int>, IObservable<TKey>> keySelector,
            Func<T, IObservable<int>, IObservable<TValue>> valueSelector)
        {
            return Observable.CombineLatest(
                keySelector(t, idx),
                valueSelector(t, idx),
                (key, value) => KeyValuePair(key, value));
        }
    }
}
