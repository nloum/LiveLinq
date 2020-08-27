using System.Collections.Generic;
using ComposableCollections.Dictionary.Interfaces;
using ComposableCollections.Dictionary.WithBuiltInKey.Interfaces;
using ComposableCollections.Dictionary.Write;

namespace LiveLinq.Dictionary.Interfaces
{
    public interface IObservableCachedDictionaryWithBuiltInKey<TKey, TValue> : IObservableDictionaryWithBuiltInKey<TKey, TValue>, ICachedDisposableDictionaryWithBuiltInKey<TKey, TValue>
    {
        IObservableCachedDictionary<TKey, TValue> AsObservableCachedDictionary();
    }
}