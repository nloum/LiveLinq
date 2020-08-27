using ComposableCollections.Dictionary.Interfaces;
using ComposableCollections.Dictionary.WithBuiltInKey.Interfaces;

namespace LiveLinq.Dictionary.Interfaces
{
    public interface IObservableQueryableReadOnlyDictionaryWithBuiltInKey<TKey, TValue> : IObservableReadOnlyDictionaryWithBuiltInKey<TKey, TValue>, IDisposableQueryableReadOnlyDictionaryWithBuiltInKey<TKey, TValue>
    {
        IObservableQueryableReadOnlyDictionary<TKey, TValue> AsObservableQueryableReadOnlyDictionary();
    }
}