using System.Collections.Generic;
using MoreCollections;

namespace LiveLinq.Dictionary
{
    public interface IReadOnlyObservableDictionary<TKey, TValue> : IReadOnlyDictionary<TKey, TValue>
    {
        IDictionaryChangesStrict<TKey, TValue> ToLiveLinq();
        void AddRange(IEnumerable<KeyValuePair<TKey, TValue>> pairs);
        void AddRange(IEnumerable<IKeyValuePair<TKey, TValue>> pairs);
        void AddOrUpdateRange(IEnumerable<KeyValuePair<TKey, TValue>> pairs);
        void AddOrUpdateRange(IEnumerable<IKeyValuePair<TKey, TValue>> pairs);
        void AddOrUpdate(KeyValuePair<TKey, TValue> pair);
        void AddOrUpdate(IKeyValuePair<TKey, TValue> pair);
        void AddOrUpdate(TKey key, TValue value);
        void RemoveRange(IEnumerable<TKey> keys);
    }
}