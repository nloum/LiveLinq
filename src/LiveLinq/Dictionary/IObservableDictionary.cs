using System.Collections.Generic;
using MoreCollections;

namespace LiveLinq.Dictionary
{
    public interface IObservableDictionary<TKey, TValue> : IDictionary<TKey, TValue>, IReadOnlyObservableDictionary<TKey, TValue>
    {
        void Add(IKeyValuePair<TKey, TValue> item);
        void AddOrUpdate(KeyValuePair<TKey, TValue> item);
        void AddOrUpdate(IKeyValuePair<TKey, TValue> item);
        void AddOrUpdate(TKey key, TValue value);
        bool Remove(IKeyValuePair<TKey, TValue> item);
        bool Remove(TKey key, TValue value);
        void AddRange(IEnumerable<KeyValuePair<TKey, TValue>> pairs);
        void AddRange(IEnumerable<IKeyValuePair<TKey, TValue>> pairs);
        void AddOrUpdateRange(IEnumerable<KeyValuePair<TKey, TValue>> pairs);
        void AddOrUpdateRange(IEnumerable<IKeyValuePair<TKey, TValue>> pairs);
        void RemoveRange(IEnumerable<TKey> keys);
    }
}