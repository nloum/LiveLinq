using System.Collections;
using System.Collections.Generic;
using System.Linq;
using MoreCollections;
using SimpleMonads;

namespace LiveLinq.Dictionary
{
    public class DelegateReadOnlyObservableDictionary<TKey, TValue> : IReadOnlyObservableDictionary<TKey, TValue>, IReadOnlyDictionary<TKey, TValue>
    {
        private readonly IReadOnlyObservableDictionary<TKey, TValue> _wrapped;

        public DelegateReadOnlyObservableDictionary(IReadOnlyObservableDictionary<TKey, TValue> wrapped)
        {
            _wrapped = wrapped;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<IKeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return _wrapped.GetEnumerator();
        }

        public int Count => _wrapped.Count;

        public bool ContainsKey(TKey key)
        {
            return _wrapped.ContainsKey(key);
        }

        IEnumerator<KeyValuePair<TKey, TValue>> IEnumerable<KeyValuePair<TKey, TValue>>.GetEnumerator()
        {
            return _wrapped.Select(kvp => new KeyValuePair<TKey, TValue>(kvp.Key, kvp.Value)).GetEnumerator();
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            var result = TryGetValue(key);
            if (result.HasValue)
            {
                value = result.Value;
                return true;
            }

            value = default;
            return false;
        }

        public IMaybe<TValue> TryGetValue(TKey key)
        {
            return _wrapped.TryGetValue(key);
        }

        public TValue this[TKey key] => _wrapped[key];

        public IEnumerable<TKey> Keys => _wrapped.Keys;

        public IEnumerable<TValue> Values => _wrapped.Values;

        public IDictionaryChangesStrict<TKey, TValue> ToLiveLinq()
        {
            return _wrapped.ToLiveLinq();
        }
    }
}