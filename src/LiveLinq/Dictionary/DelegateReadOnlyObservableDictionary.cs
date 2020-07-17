using System.Collections;
using System.Collections.Generic;

namespace LiveLinq.Dictionary
{
    public class DelegateReadOnlyObservableDictionary<TKey, TValue> : IReadOnlyObservableDictionary<TKey, TValue>
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

        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return _wrapped.GetEnumerator();
        }

        public int Count => _wrapped.Count;

        public bool ContainsKey(TKey key)
        {
            return _wrapped.ContainsKey(key);
        }

        public bool TryGetValue(TKey key, out TValue value)
        {
            return _wrapped.TryGetValue(key, out value);
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