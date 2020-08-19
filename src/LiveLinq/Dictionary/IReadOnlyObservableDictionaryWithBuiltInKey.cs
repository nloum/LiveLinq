using System;
using ComposableCollections.Dictionary;

namespace LiveLinq.Dictionary
{
    public interface IReadOnlyObservableDictionaryWithBuiltInKey<TKey, out TValue> : IDisposableReadOnlyDictionaryWithBuiltInKey<TKey, TValue>, IDisposable
    {
        IDictionaryChangesStrict<TKey, TValue> ToLiveLinq();
    }
}