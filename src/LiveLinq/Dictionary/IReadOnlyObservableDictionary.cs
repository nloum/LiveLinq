using System.Collections.Generic;
using MoreCollections;

namespace LiveLinq.Dictionary
{
    public interface IReadOnlyObservableDictionary<TKey, TValue> : IReadOnlyDictionary<TKey, TValue>
    {
        IDictionaryChangesStrict<TKey, TValue> ToLiveLinq();
    }
}