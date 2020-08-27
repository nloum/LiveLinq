using ComposableCollections.Dictionary.Interfaces;
using ComposableCollections.Dictionary.WithBuiltInKey.Interfaces;

namespace LiveLinq.Dictionary.Interfaces
{
    public interface IObservableQueryableDictionaryWithBuiltInKey<TKey, TValue> : IObservableDictionaryWithBuiltInKey<TKey, TValue>,
        IDisposableQueryableDictionaryWithBuiltInKey<TKey, TValue> 
    {
        IObservableQueryableDictionary<TKey, TValue> AsObservableQueryableDictionary();
    }
}