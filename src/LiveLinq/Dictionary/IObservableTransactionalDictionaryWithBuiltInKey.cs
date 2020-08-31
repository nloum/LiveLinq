using ComposableCollections.Common;
using ComposableCollections.Dictionary;
using ComposableCollections.Dictionary.WithBuiltInKey.Interfaces;

namespace LiveLinq.Dictionary
{
    public interface IObservableTransactionalDictionaryWithBuiltInKey<TKey, TValue> : IReadWriteFactory<IDisposableReadOnlyDictionaryWithBuiltInKey<TKey, TValue>, IDisposableDictionaryWithBuiltInKey<TKey, TValue>>, IReadOnlyObservableTransactionalDictionaryWithBuiltInKey<TKey, TValue>
    
    {
    }
}