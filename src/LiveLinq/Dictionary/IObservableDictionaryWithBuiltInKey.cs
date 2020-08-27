using ComposableCollections.Dictionary;
using ComposableCollections.Dictionary.WithBuiltInKey.Interfaces;

namespace LiveLinq.Dictionary
{
    public interface IObservableDictionaryWithBuiltInKey<TKey, TValue> : IDisposableDictionaryWithBuiltInKey<TKey, TValue>, IReadOnlyObservableDictionaryWithBuiltInKey<TKey, TValue>
    {
    }
}