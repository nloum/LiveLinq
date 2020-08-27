using ComposableCollections.Dictionary.Interfaces;
using ComposableCollections.Dictionary.WithBuiltInKey.Interfaces;

namespace LiveLinq.Dictionary.Interfaces
{
    public interface IObservableCachedQueryableDictionaryWithBuiltInKey<TKey, TValue> :
        IObservableCachedDictionaryWithBuiltInKey<TKey, TValue>, IObservableQueryableDictionaryWithBuiltInKey<TKey, TValue>, ICachedDisposableQueryableDictionaryWithBuiltInKey<TKey, TValue>
    {
        IObservableCachedQueryableDictionary<TKey, TValue> AsObservableCachedQueryableDictionary();
    }
}