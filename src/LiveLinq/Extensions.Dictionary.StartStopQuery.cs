using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;
using ComposableCollections.Dictionary;
using LiveLinq.Dictionary;
using SimpleMonads;
using MoreCollections;
using LiveLinq.List;


namespace LiveLinq
{
    public static partial class Extensions
    {
        public static IDictionaryChangesStrict<TKey, TValue> ToLiveLinq<TKey, TValue>(
            this IObservable<IDictionaryChangeStrict<TKey, TValue>> source)
        {
            return new DictionaryChangesStrict<TKey, TValue>(source);
        }

        public static IDictionaryChanges<TKey, TValue> ToLiveLinq<TKey, TValue>(
            this IObservable<IDictionaryChange<TKey, TValue>> source)
        {
            return new DictionaryChanges<TKey, TValue>(source);
        }

        #region Dictionary-like LiveLinq

        /// <summary>
        /// Same as LINQ's ToDictionary, except watches the source for changes and updates the result accordingly.
        /// </summary>
        public static IDictionaryChangesStrict<TKey, TValue> ToLiveLinq<TKey, TValue>(
            this IListChangesStrict<IKeyValue<TKey, TValue>> source)
        {
            return new DictionaryChangesStrict<TKey, TValue>(source.AsObservable().Select(kvp => kvp.ToDictionaryChange()));
        }

        /// <summary>
        /// Generates a LiveLinq query that initially has no element, and as each new source event is fired, it clears the query
        /// and then adds the value from the new source event.
        /// </summary>
        public static IDictionaryChangesStrict<TKey, TValue> ToLiveLinq<TKey, TValue>(this IObservable<IKeyValue<TKey, TValue>> source)
        {
            return source.Select(maybe => maybe.ToMaybe()).ToLiveLinq();
        }

        /// <summary>
        /// Generates a LiveLinq query that initially has no element, and as each new source event is fired, it clears the query
        /// and then, if the new source event has a value, it adds that value.
        /// </summary>
        public static IDictionaryChangesStrict<TKey, TValue> ToLiveLinq<TKey, TValue>(this IObservable<IMaybe<IKeyValue<TKey, TValue>>> source)
        {
            return source.Select(maybe => maybe.ToEnumerable().ToImmutableDictionary(kvp => kvp.Key, kvp => kvp.Value)).ToLiveLinq();
        }

        /// <summary>
        /// Generates a LiveLinq query that initially has no element, and as each new source event is fired, it clears the query
        /// and then adds the items in the new source event.
        /// </summary>
        public static IDictionaryChangesStrict<TKey, TValue> ToLiveLinq<TKey, TValue>(this IObservable<ImmutableDictionary<TKey, TValue>> source)
        {
            // Note: the first event should ALWAYS represent the actual state of the list at that time. If the initial state
            // of the list is empty, the first event MUST be an empty add event. So, here we first return an empty enumerable
            // event and then we start using events from source.
            source = Observable.Return(ImmutableDictionary<TKey, TValue>.Empty).Concat(source);

            return
                source.Scan(
                    new { Previous = ImmutableDictionary<TKey, TValue>.Empty, Current = ImmutableDictionary<TKey, TValue>.Empty },
                    (state, newValue) => new { state.Previous, Current = newValue })
                    .Select(state => Utility.ConvertStateToDictionaryChanges(state.Previous, state.Current))
                    .Concat()
                    .ToLiveLinq();
        }

        /// <summary>
        /// Same as LINQ's ToDictionary, except watches the source for changes and updates the result accordingly.
        /// </summary>
        public static IDictionaryChanges<TKey, TValue> ToLiveLinq<TElement, TKey, TValue>(
            this IListChangesStrict<TElement> source,
            Func<TElement, IObservable<TKey>> keySelector,
            Func<TElement, IObservable<TValue>> valueSelector)
        {
            return source.Select(
                element =>
                keySelector(element).CombineLatest(valueSelector(element), (key, value) => new KeyValue<TKey, TValue>(key, value)))
                .MakeStrictExpensively()
                .ToLiveLinq();
        }

        /// <summary>
        /// Same as LINQ's ToDictionary, except watches the source for changes and updates the result accordingly.
        /// </summary>
        public static IDictionaryChanges<TKey, TValue> ToLiveLinq<TElement, TKey, TValue>(
            this IListChangesStrict<TElement> source,
            Func<TElement, TKey> keySelector,
            Func<TElement, IObservable<TValue>> valueSelector)
        {
            return source.Select(
                element =>
                valueSelector(element).Select(value => new KeyValue<TKey, TValue>(keySelector(element), value)))
                .MakeStrictExpensively()
                .ToLiveLinq();
        }

        /// <summary>
        /// Same as LINQ's ToDictionary, except watches the source for changes and updates the result accordingly.
        /// </summary>
        public static IDictionaryChanges<TKey, TValue> ToLiveLinq<TElement, TKey, TValue>(
            this IListChangesStrict<TElement> source,
            Func<TElement, IObservable<TKey>> keySelector,
            Func<TElement, TValue> valueSelector)
        {
            return source.Select(
                element =>
                keySelector(element).Select(key => new KeyValue<TKey, TValue>(key, valueSelector(element))))
                .MakeStrictExpensively()
                .ToLiveLinq();
        }

        /// <summary>
        /// Same as LINQ's ToDictionary, except watches the source for changes and updates the result accordingly.
        /// </summary>
        public static IDictionaryChanges<TKey, TValue> ToLiveLinq<TElement, TKey, TValue>(
            this IListChangesStrict<TElement> source,
            Func<TElement, TKey> keySelector,
            Func<TElement, TValue> valueSelector)
        {
            return source.Select(
                element =>
                new KeyValue<TKey, TValue>(keySelector(element), valueSelector(element)))
                .ToLiveLinq();
        }

        /// <summary>
        /// Same as LINQ's ToDictionary, except watches the source for changes and updates the result accordingly.
        /// </summary>
        public static IDictionaryChanges<TKey, TElement> ToLiveLinq<TElement, TKey>(
            this IListChangesStrict<TElement> source,
            Func<TElement, IObservable<TKey>> keySelector)
        {
            return source.ToLiveLinq(keySelector, Observable.Return);
        }

        #endregion

        public static IObservable<DictionaryStateAndChange<TKey, TValue>> ToObservableStateAndChange<TKey, TValue>(
            this IDictionaryChanges<TKey, TValue> source)
        {
            return source.AsObservable()
                .Scan(new DictionaryStateAndChange<TKey, TValue>(),
                    (state, change) => state.Accumulate(change));
        }

        public static IObservable<ImmutableDictionary<TKey, TValue>> ToObservableState<TKey, TValue>(
            this IDictionaryChanges<TKey, TValue> source)
        {
            return source.ToObservableStateAndChange().Select(x => x.State);
        }

        public static IObservable<ImmutableDictionary<TKey, TValue>> ToObservableEnumerable<TKey, TValue>(
            this IDictionaryChanges<TKey, TValue> source)
        {
            return source.ToObservableStateAndChange().Select(state => state.State);
        }
    }
}
