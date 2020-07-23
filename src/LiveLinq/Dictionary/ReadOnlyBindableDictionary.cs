using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using MoreCollections;
using SimpleMonads;

namespace LiveLinq.Dictionary
{
    internal class ReadOnlyBindableDictionary<TKey, TValue> : IReadOnlyBindableDictionary<TKey, TValue>, IReadOnlyDictionary<TKey, TValue>, IDisposable
    {
        private readonly BindableDictionary<TKey, TValue> _bindableDictionary = new BindableDictionary<TKey, TValue>();
        public IDisposable AssociatedSubscription { get; set; }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return ((IEnumerable) _bindableDictionary).GetEnumerator();
        }

        public IEnumerator<IKeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return _bindableDictionary.GetEnumerator();
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged
        {
            add => _bindableDictionary.CollectionChanged += value;
            remove => _bindableDictionary.CollectionChanged -= value;
        }

        public IMaybe<TValue> TryGetValue(TKey key)
        {
            if (TryGetValue(key, out var value))
            {
                return value.ToMaybe();
            }

            return Maybe<TValue>.Nothing();
        }

        public int Count => _bindableDictionary.Count;

        IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
        {
            return ((IEnumerable<KeyValuePair<TKey, TValue>>)_bindableDictionary).GetEnumerator();
        }

        public bool ContainsKey(TKey key)
        {
            return _bindableDictionary.ContainsKey(key);
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            return _bindableDictionary.TryGetValue(key, out value);
        }

        public TValue this[TKey key] => _bindableDictionary[key];

        public IEnumerable<TKey> Keys => _bindableDictionary.Keys;
        public IEnumerable<TValue> Values => _bindableDictionary.Values;

        public void AddRange(IEnumerable<IKeyValuePair<TKey, TValue>> keyValuePairs) {
            foreach(var keyValuePair in keyValuePairs) {
                _bindableDictionary.Add(keyValuePair.Key, keyValuePair.Value);
            }
        }
        
        public void RemoveRange(IEnumerable<TKey> keys) {
            foreach(var key in keys) {
                _bindableDictionary.Remove(key);
            }
        }
        
        public void Dispose()
        {
            AssociatedSubscription?.Dispose();
        }
    }
}