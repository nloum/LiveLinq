using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reactive.Linq;
using SimpleMonads;
using LiveLinq.List;

using MoreCollections;
using LiveLinq.Core;
using static SimpleMonads.Utility;

namespace LiveLinq.Dictionary
{
    public class Utility
    {
        /// <summary>
        /// This function is analogous to the indexing method of <see cref="List{T}"/>, except it returns
        /// an event stream of values as the value at the specified index changes, and it returns a Nothing
        /// event if there's nothing at that index (i.e., if the index is out of bounds).
        /// </summary>
        /// <param name="index">The index to watch.</param>
        internal static IObservable<IMaybe<TValue>> GetAtKey<TKey, TValue>(IDictionaryChanges<TKey, TValue> source, TKey key)
        {
            return source.AsObservable().Where(change => change.ContainsKey(key))
                .Select(change =>
            {
                if (change.Type == CollectionChangeType.Add)
                    return change[key];
                return Nothing<TValue>();
            });
        }

        /// <summary>
        /// This function is analogous to the indexing method of <see cref="List{T}"/>, except it returns
        /// an event stream of values as the value at the specified index changes, and it returns a Nothing
        /// event if there's nothing at that index (i.e., if the index is out of bounds).
        /// </summary>
        /// <param name="index">The observable event stream where each event represents a new index to watch.</param>
        internal static IObservable<IMaybe<TValue>> GetAtKey<TKey, TValue>(IDictionaryChanges<TKey, TValue> source, IObservable<TKey> keys)
        {
            return source.ToObservableEnumerable()
                .CombineLatest(keys, (state, key) => state.ContainsKey(key) ? Something(state[key]) : Nothing<TValue>());
        }

        /// <summary>
        /// This function is analogous to the indexing method of <see cref="List{T}"/>, except it returns
        /// an event stream of values as the value at the specified index changes, and it returns a Nothing
        /// event if there's nothing at that index (i.e., if the index is out of bounds).
        /// </summary>
        /// <param name="index">The observable event stream where each event represents a new index to watch.</param>
        internal static IObservable<IMaybe<TValue>> GetAtKey<TKey, TValue>(IDictionaryChanges<TKey, TValue> source, IObservable<IMaybe<TKey>> keys)
        {
            return source.ToObservableEnumerable()
                .CombineLatest(keys, (state, maybeKey) => maybeKey.SelectMany(key => state.ContainsKey(key) ? Something(state[key]) : Nothing<TValue>()));
        }


        #region Dictionary changes

        #region Remove

        #region Non-strict

        /// <summary>
        /// Create a <see cref="IDictionaryChange{TKey,TValue}"/> object that represents removing items from a dictionary.
        /// </summary>
        public static IDictionaryChange<TKey, TValue> DictionaryRemove<TKey, TValue>(IEnumerable<TKey> keys)
        {
            return new DictionaryChangeNonStrict<TKey, TValue>(CollectionChangeType.Remove, keys.ToImmutableList());
        }

        /// <summary>
        /// Create a <see cref="IDictionaryChange{TKey,TValue}"/> object that represents removing items from a dictionary.
        /// </summary>
        public static IDictionaryChange<TKey, TValue> DictionaryRemove<TKey, TValue>(params TKey[] keys)
        {
            return new DictionaryChangeNonStrict<TKey, TValue>(CollectionChangeType.Remove, keys.ToImmutableList());
        }

        /// <summary>
        /// Create a <see cref="IDictionaryChange{TKey,TValue}"/> object that represents removing items from a dictionary.
        /// </summary>
        public static IDictionaryChange<TKey, TValue> DictionaryRemove<TKey, TValue>(ImmutableList<TKey> keys)
        {
            return new DictionaryChangeNonStrict<TKey, TValue>(CollectionChangeType.Remove, keys);
        }

        #endregion

        #region Strict

        /// <summary>
        /// Create a <see cref="IDictionaryChange{TKey,TValue}"/> object that represents adding items to a dictionary.
        /// </summary>
        public static IDictionaryChangeStrict<TKey, TValue> DictionaryRemove<TKey, TValue>(params IKeyValuePair<TKey, TValue>[] keysAndValues)
        {
            return new DictionaryChangeStrict<TKey, TValue>(CollectionChangeType.Remove, keysAndValues.ToImmutableDictionary(kvp => kvp.Key, kvp => kvp.Value));
        }

        /// <summary>
        /// Create a <see cref="IDictionaryChange{TKey,TValue}"/> object that represents adding items to a dictionary.
        /// </summary>
        public static IDictionaryChangeStrict<TKey, TValue> DictionaryRemove<TKey, TValue>(IEnumerable<IKeyValuePair<TKey, TValue>> keysAndValues)
        {
            return new DictionaryChangeStrict<TKey, TValue>(CollectionChangeType.Remove, keysAndValues.ToImmutableDictionary(kvp => kvp.Key, kvp => kvp.Value));
        }

        /// <summary>
        /// Create a <see cref="IDictionaryChange{TKey,TValue}"/> object that represents adding items to a dictionary.
        /// </summary>
        public static IDictionaryChangeStrict<TKey, TValue> DictionaryRemove<TKey, TValue>(IEnumerable<KeyValuePair<TKey, TValue>> keysAndValues)
        {
            return new DictionaryChangeStrict<TKey, TValue>(CollectionChangeType.Remove, keysAndValues.ToImmutableDictionary(kvp => kvp.Key, kvp => kvp.Value));
        }

        /// <summary>
        /// Create a <see cref="IDictionaryChange{TKey,TValue}"/> object that represents adding items to a dictionary.
        /// </summary>
        public static IDictionaryChangeStrict<TKey, TValue> DictionaryRemove<TKey, TValue>(IDictionary<TKey, TValue> source)
        {
            return new DictionaryChangeStrict<TKey, TValue>(CollectionChangeType.Remove, source.ToImmutableDictionary(kvp => kvp.Key, kvp => kvp.Value));
        }

        /// <summary>
        /// Create a <see cref="IDictionaryChange{TKey,TValue}"/> object that represents adding items to a dictionary.
        /// </summary>
        public static IDictionaryChangeStrict<TKey, TValue> DictionaryRemove<TKey, TValue>(ImmutableDictionary<TKey, TValue> source)
        {
            return new DictionaryChangeStrict<TKey, TValue>(CollectionChangeType.Remove, source);
        }

        #endregion

        #endregion

        #region Add

        /// <summary>
        /// Create a <see cref="IDictionaryChange{TKey,TValue}"/> object that represents adding items to a dictionary.
        /// </summary>
        public static IDictionaryChangeStrict<TKey, TValue> DictionaryAdd<TKey, TValue>(IEnumerable<IKeyValuePair<TKey, TValue>> keysAndValues)
        {
            return new DictionaryChangeStrict<TKey, TValue>(CollectionChangeType.Add, keysAndValues.ToImmutableDictionary(kvp => kvp.Key, kvp => kvp.Value));
        }

        /// <summary>
        /// Create a <see cref="IDictionaryChange{TKey,TValue}"/> object that represents adding items to a dictionary.
        /// </summary>
        public static IDictionaryChangeStrict<TKey, TValue> DictionaryAdd<TKey, TValue>(params IKeyValuePair<TKey, TValue>[] keysAndValues)
        {
            return new DictionaryChangeStrict<TKey, TValue>(CollectionChangeType.Add, keysAndValues.ToImmutableDictionary(kvp => kvp.Key, kvp => kvp.Value));
        }

        /// <summary>
        /// Create a <see cref="IDictionaryChange{TKey,TValue}"/> object that represents adding items to a dictionary.
        /// </summary>
        public static IDictionaryChangeStrict<TKey, TValue> DictionaryAdd<TKey, TValue>(IEnumerable<KeyValuePair<TKey, TValue>> keysAndValues)
        {
            return new DictionaryChangeStrict<TKey, TValue>(CollectionChangeType.Add, keysAndValues.ToImmutableDictionary(kvp => kvp.Key, kvp => kvp.Value));
        }

        /// <summary>
        /// Create a <see cref="IDictionaryChange{TKey,TValue}"/> object that represents adding items to a dictionary.
        /// </summary>
        public static IDictionaryChangeStrict<TKey, TValue> DictionaryAdd<TKey, TValue>(params KeyValuePair<TKey, TValue>[] keysAndValues)
        {
            return new DictionaryChangeStrict<TKey, TValue>(CollectionChangeType.Add, keysAndValues.ToImmutableDictionary(kvp => kvp.Key, kvp => kvp.Value));
        }

        /// <summary>
        /// Create a <see cref="IDictionaryChange{TKey,TValue}"/> object that represents adding items to a dictionary.
        /// </summary>
        public static IDictionaryChangeStrict<TKey, TValue> DictionaryAdd<TKey, TValue>(IDictionary<TKey, TValue> source)
        {
            return new DictionaryChangeStrict<TKey, TValue>(CollectionChangeType.Add, source.ToImmutableDictionary(kvp => kvp.Key, kvp => kvp.Value));
        }

        /// <summary>
        /// Create a <see cref="IDictionaryChange{TKey,TValue}"/> object that represents adding items to a dictionary.
        /// </summary>
        public static IDictionaryChangeStrict<TKey, TValue> DictionaryAdd<TKey, TValue>(ImmutableDictionary<TKey, TValue> source)
        {
            return new DictionaryChangeStrict<TKey, TValue>(CollectionChangeType.Add, source);
        }

        #endregion
        
        public static IObservable<IDictionaryChangeStrict<TKey, TValue>> ConvertStateToDictionaryChanges<TKey, TValue>(
            ImmutableDictionary<TKey, TValue> previous,
            ImmutableDictionary<TKey, TValue> current)
        {
            if (current.Any())
            {
                var currentChange = DictionaryAdd(current);

                if (previous.Any())
                {
                    var previousChange = DictionaryRemove(previous);
                    var listChanges = new[] { previousChange, currentChange };
                    return listChanges.ToObservable();
                }
                return Observable.Return(currentChange);
            }
            if (previous.Any())
            {
                var previousChange = DictionaryRemove(previous);
                return Observable.Return(previousChange);
            }
            return Observable.Empty<IDictionaryChangeStrict<TKey, TValue>>();
        }

        #endregion
    }
}
