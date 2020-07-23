using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MoreCollections;
using SimpleMonads;

namespace LiveLinq.Dictionary
{
    public class DelegateObservableDictionary<TKey, TValue> : IObservableDictionary<TKey, TValue>
    {
        private readonly IObservableDictionary<TKey, TValue> _wrapped;
        private ICollection<TKey> _keys;
        private ICollection<TValue> _values;

        public DelegateObservableDictionary(IObservableDictionary<TKey, TValue> wrapped)
        {
            _wrapped = wrapped;
        }

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return ((IDictionary<TKey, TValue>)_wrapped).GetEnumerator();
        }

        IEnumerator<IKeyValuePair<TKey, TValue>> IEnumerable<IKeyValuePair<TKey, TValue>>.GetEnumerator()
        {
            return ((IReadOnlyDictionaryEx<TKey, TValue>)_wrapped).GetEnumerator();
        }

        public IMaybe<TValue> TryGetValue(TKey key)
        {
            return _wrapped.TryGetValue(key);
        }

        public void Add(KeyValuePair<TKey, TValue> item)
        {
            _wrapped.Add(item);
        }

        public void Clear()
        {
            _wrapped.Clear();
        }

        public bool Contains(KeyValuePair<TKey, TValue> item)
        {
            return _wrapped.Contains(item);
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            _wrapped.CopyTo(array, arrayIndex);
        }

        public bool Remove(KeyValuePair<TKey, TValue> item)
        {
            return _wrapped.Remove(item);
        }

        public bool IsReadOnly => _wrapped.IsReadOnly;

        public void Add(TKey key, TValue value)
        {
            _wrapped.Add(key, value);
        }

        public bool Remove(TKey key)
        {
            return _wrapped.Remove(key);
        }

        public IDictionaryChangesStrict<TKey, TValue> ToLiveLinq()
        {
            return _wrapped.ToLiveLinq();
        }

        public bool ContainsKey(TKey key)
        {
            return _wrapped.ContainsKey(key);
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            return _wrapped.TryGetValue(key, out value);
        }

        public void Add(IKeyValuePair<TKey, TValue> item)
        {
            _wrapped.Add(item);
        }

        public void AddOrUpdate(KeyValuePair<TKey, TValue> item)
        {
            _wrapped.AddOrUpdate(item);
        }

        public void AddOrUpdate(IKeyValuePair<TKey, TValue> item)
        {
            _wrapped.AddOrUpdate(item);
        }

        public void AddOrUpdate(TKey key, TValue value)
        {
            _wrapped.AddOrUpdate(key, value);
        }

        public bool Remove(IKeyValuePair<TKey, TValue> item)
        {
            return _wrapped.Remove(item);
        }

        public bool Remove(TKey key, TValue value)
        {
            return _wrapped.Remove(key, value);
        }

        public void AddRange(IEnumerable<KeyValuePair<TKey, TValue>> pairs)
        {
            _wrapped.AddRange(pairs);
        }

        public void AddRange(IEnumerable<IKeyValuePair<TKey, TValue>> pairs)
        {
            _wrapped.AddRange(pairs);
        }

        public void AddOrUpdateRange(IEnumerable<KeyValuePair<TKey, TValue>> pairs)
        {
            _wrapped.AddOrUpdateRange(pairs);
        }

        public void AddOrUpdateRange(IEnumerable<IKeyValuePair<TKey, TValue>> pairs)
        {
            _wrapped.AddOrUpdateRange(pairs);
        }

        public void RemoveRange(IEnumerable<TKey> keys)
        {
            _wrapped.RemoveRange(keys);
        }

        public int Count => _wrapped.Count;

        public TValue this[TKey key]
        {
            get => _wrapped[key];
            set => _wrapped[key] = value;
        }

        public IEnumerable<TKey> Keys => _wrapped.Keys;

        ICollection<TValue> IDictionary<TKey, TValue>.Values => _values;

        ICollection<TKey> IDictionary<TKey, TValue>.Keys => _keys;

        public IEnumerable<TValue> Values => _wrapped.Values;
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }

}