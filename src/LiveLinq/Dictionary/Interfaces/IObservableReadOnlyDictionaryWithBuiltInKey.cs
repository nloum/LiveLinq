using ComposableCollections.Dictionary.Interfaces;
using ComposableCollections.Dictionary.WithBuiltInKey.Interfaces;

namespace LiveLinq.Dictionary.Interfaces
{
    public interface IObservableReadOnlyDictionaryWithBuiltInKey<TKey, TValue> : IObservableReadOnlyDictionary<TKey, TValue>, IDisposableReadOnlyDictionaryWithBuiltInKey<TKey, TValue>
    {
    }
}