using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Collections.Specialized;
using System.Linq;
using System.Reactive;
using System.Text;
using System.Threading.Tasks;

using System.Reactive.Linq;

using SimpleMonads;
using LiveLinq.Ordered;

using MoreCollections;
using LiveLinq.Dictionary;
using LiveLinq.Core;
using LiveLinq.List;

namespace LiveLinq.Dictionary
{
    public static partial class Extensions
    {
        public static IReadOnlyObservableDictionary<TKey, TValue> ToReadOnlyObservableDictionary<TKey, TValue>(
            this IDictionaryChanges<TKey, TValue> changes)
        {
            var result = new SimpleObservableDictionary<TKey, TValue>();
            result.AssociatedSubscription = changes.AsObservable().Subscribe(change =>
            {
                if (change.Type == CollectionChangeType.Add)
                {
                    result.AddRange(change.Items);
                }
                else if (change.Type == CollectionChangeType.Remove)
                {
                    result.RemoveRange(change.Items.Select(x => x.Key));
                }
            });
            return result;
        }

        public static IReadOnlyBindableDictionary<TKey, TValue> ToReadOnlyBindableObservableDictionary<TKey, TValue>(
            this IDictionaryChanges<TKey, TValue> changes)
        {
            var result = new ReadOnlyBindableDictionary<TKey, TValue>();
            result.AssociatedSubscription = changes.AsObservable().Subscribe(change =>
            {
                if (change.Type == CollectionChangeType.Add)
                {
                    result.AddRange(change.Items);
                }
                else if (change.Type == CollectionChangeType.Remove)
                {
                    result.RemoveRange(change.Items.Select(x => x.Key));
                }
            });
            return result;
        }

        #region ContainsKey

        #region Non-strict
        
        /// <summary>
        /// Similar to the Dictionary.ContainsKey extension method, except this returns an event stream
        /// that fires a new event every time the key is removed or added again.
        /// </summary>
        /// <param name="key">The key you are interested in</param>
        public static IObservable<bool> ContainsKey<TKey, TValue>(
            this IDictionaryChanges<TKey, TValue> source,
            TKey key)
        {
            return source[key].Select(m => m.HasValue);
        }
        
        /// <summary>
        /// Similar to the Dictionary.ContainsKey extension method, except this returns an event stream
        /// that fires a new event every time the key is removed or added again.
        /// </summary>
        /// <param name="key">The event stream of keys you are interested in. Every time a new key comes along
        /// in this event stream, this method checks to see if source contains that key, and fires a new
        /// event, even if the new event is the same as the previous event.</param>
        public static IObservable<bool> ContainsKey<TKey, TValue>(
            this IDictionaryChanges<TKey, TValue> source,
            IObservable<TKey> key)
        {
            return source[key].Select(m => m.HasValue);
        }
        
        /// <summary>
        /// Similar to the Dictionary.ContainsKey extension method, except this returns an event stream
        /// that fires a new event every time the key is removed or added again.
        /// </summary>
        /// <param name="key">The event stream of keys you are interested in. Every time a new key comes along
        /// in this event stream, this method checks to see if source contains that key, and fires a new
        /// event, even if the new event is the same as the previous event.</param>
        public static IObservable<bool> ContainsKey<TKey, TValue>(
            this IDictionaryChanges<TKey, TValue> source,
            IObservable<IMaybe<TKey>> key)
        {
            return source[key].Select(m => m.HasValue);
        }

        #endregion

        #region Strict

        /// <summary>
        /// Similar to the Dictionary.ContainsKey extension method, except this returns an event stream
        /// that fires a new event every time the key is removed or added again.
        /// </summary>
        /// <param name="key">The key you are interested in</param>
        public static IObservable<bool> ContainsKey<TKey, TValue>(
            this IDictionaryChangesStrict<TKey, TValue> source,
            TKey key)
        {
            return source[key].Select(m => m.HasValue);
        }
        
        /// <summary>
        /// Similar to the Dictionary.ContainsKey extension method, except this returns an event stream
        /// that fires a new event every time the key is removed or added again.
        /// </summary>
        /// <param name="key">The event stream of keys you are interested in. Every time a new key comes along
        /// in this event stream, this method checks to see if source contains that key, and fires a new
        /// event, even if the new event is the same as the previous event.</param>
        public static IObservable<bool> ContainsKey<TKey, TValue>(
            this IDictionaryChangesStrict<TKey, TValue> source,
            IObservable<TKey> key)
        {
            return source[key].Select(m => m.HasValue);
        }
        
        /// <summary>
        /// Similar to the Dictionary.ContainsKey extension method, except this returns an event stream
        /// that fires a new event every time the key is removed or added again.
        /// </summary>
        /// <param name="key">The event stream of keys you are interested in. Every time a new key comes along
        /// in this event stream, this method checks to see if source contains that key, and fires a new
        /// event, even if the new event is the same as the previous event.</param>
        public static IObservable<bool> ContainsKey<TKey, TValue>(
            this IDictionaryChangesStrict<TKey, TValue> source,
            IObservable<IMaybe<TKey>> key)
        {
            return source[key].Select(m => m.HasValue);
        }

        #endregion

        #endregion

        public static IDictionaryChangesStrict<TKey, TValue> MakeStrict<TKey, TValue>(
            this IDictionaryChanges<TKey, TValue> source)
        {
            return source.ToObservableStateAndChange()
                .Select(sac => sac.MostRecentChange)
                .ToLiveLinq();
        }

        /// <summary>
        /// A LiveLinq query of keys from the dictionary. Analogous to the Keys property of a Dictionary.
        /// </summary>
        public static IListChanges<TKey> Keys<TKey, TValue>(
            this IDictionaryChangesStrict<TKey, TValue> source)
        {
            return source.OrderBy(kvp => kvp.Key).Select(kvp => kvp.Key);
        }

        /// <summary>
        /// A LiveLinq query of values from the dictionary. Analogous to the Values property of a Dictionary.
        /// </summary>
        public static IListChanges<TValue> Values<TKey, TValue>(
            this IDictionaryChangesStrict<TKey, TValue> source)
        {
            return source.OrderBy(kvp => kvp.Key).Select(kvp => kvp.Value);
        }

        /// <summary>
        /// Converts a strict list change to a dictionary change. There isn't an overload of this function that converts
        /// a non-strict list change to a non-strict dictionary change, because we don't know the key values of the list removes
        /// when the range of items to be removed is specified, but not the actual values to be removed.
        /// </summary>
        public static IDictionaryChangeStrict<TKey, TValue> ToDictionaryChange<TKey, TValue>(
            this IListChangeStrict<IKeyValuePair<TKey, TValue>> listChange)
        {
            if (listChange.Type == CollectionChangeType.Add)
                return Utility.DictionaryAdd(listChange.Values);
            return Utility.DictionaryRemove(listChange.Values);
        }

        /// <summary>
        /// Applies the dictionary change to the specified <see cref="ImmutableDictionary{T}"/>.
        /// </summary>
        public static ImmutableDictionary<TKey, TValue> Mutate<TKey, TValue>(this ImmutableDictionary<TKey, TValue> subject, IDictionaryChange<TKey, TValue> change)
        {
            switch (change.Type)
            {
                case CollectionChangeType.Add:
                    {
                        var result = subject.AddRange(change.Items.Select(kvp => new KeyValuePair<TKey, TValue>(kvp.Key, kvp.Value)));
                        return result;
                    }
                case CollectionChangeType.Remove:
                    {
                        var result = subject.RemoveRange(change.Keys);
                        return result;
                    }
                default:
                    throw new NotImplementedException();
            }
        }

        /// <summary>
        /// Applies the dictionary change to the specified <see cref="ImmutableDictionary{T}"/>.
        /// </summary>
        public static void Mutate<TKey, TValue>(this IDictionary<TKey, TValue> subject, IDictionaryChange<TKey, TValue> change)
        {
            switch (change.Type)
            {
                case CollectionChangeType.Add:
                    subject.AddRange(change.Items.Select(kvp => new KeyValuePair<TKey, TValue>(kvp.Key, kvp.Value)));
                    break;
                case CollectionChangeType.Remove:
                    subject.RemoveRange(change.Keys);
                    break;
                default:
                    throw new NotImplementedException();
            }
        }

    }
}
