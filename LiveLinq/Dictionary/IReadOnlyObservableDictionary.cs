using System.Collections.Generic;
using MoreCollections;

namespace LiveLinq.Dictionary
{
    public interface IReadOnlyObservableDictionary<TKey, TValue> : IReadOnlyDictionary<TKey, TValue>
    {
        IDictionaryChangesStrict<TKey, TValue> ToLiveLinq();
        void AddRange(IEnumerable<KeyValuePair<TKey, TValue>> pairs);
        void AddRange(IEnumerable<IKeyValuePair<TKey, TValue>> pairs);
        void RemoveRange(IEnumerable<TKey> keys);
    }
}