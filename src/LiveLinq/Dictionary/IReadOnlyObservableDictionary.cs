using System.Collections;
using System.Collections.Generic;
using MoreCollections;

namespace LiveLinq.Dictionary
{
    public interface IReadOnlyObservableDictionary<TKey, out TValue> : IReadOnlyDictionaryEx<TKey, TValue>
    {
        IDictionaryChangesStrict<TKey, TValue> ToLiveLinq();
    }
}