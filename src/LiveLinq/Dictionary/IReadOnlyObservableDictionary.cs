using System;
using ComposableCollections.Dictionary;
using ComposableCollections.Dictionary.Interfaces;

namespace LiveLinq.Dictionary
{
    public interface IReadOnlyObservableDictionary<TKey, out TValue> : IDisposableReadOnlyDictionary<TKey, TValue>
    {
        IDictionaryChangesStrict<TKey, TValue> ToLiveLinq();
    }
}