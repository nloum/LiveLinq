using System;
using ComposableCollections.Dictionary;

namespace LiveLinq.Dictionary
{
    public interface IReadOnlyObservableDictionary<TKey, out TValue> : IDisposableReadOnlyDictionary<TKey, TValue>
    {
        IDictionaryChangesStrict<TKey, TValue> ToLiveLinq();
    }
}