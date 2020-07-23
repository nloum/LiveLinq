using System.Collections.Generic;
using MoreCollections;

namespace LiveLinq.Dictionary
{
    public interface IObservableDictionary<TKey, TValue> : IReadOnlyObservableDictionary<TKey, TValue>, IDictionary<TKey, TValue>, IReadOnlyDictionary<TKey, TValue>
    {
        new int Count { get; }
        new bool ContainsKey(TKey key);
        new TValue this[TKey key] { get; set; }
        new IEnumerable<TKey> Keys { get; }
        new IEnumerable<TValue> Values { get; }
        new bool TryGetValue(TKey key, out TValue value);
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