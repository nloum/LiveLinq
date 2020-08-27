using System;
using ComposableCollections.Dictionary;
using ComposableCollections.Dictionary.WithBuiltInKey.Interfaces;

namespace LiveLinq.Dictionary
{
    public interface IReadOnlyObservableDictionaryWithBuiltInKey<TKey, TValue> : IDisposableReadOnlyDictionaryWithBuiltInKey<TKey, TValue>
    {
        IDictionaryChangesStrict<TKey, TValue> ToLiveLinq();
    }
}