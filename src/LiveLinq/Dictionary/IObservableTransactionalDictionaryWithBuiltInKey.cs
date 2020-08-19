using ComposableCollections.Dictionary;

namespace LiveLinq.Dictionary
{
    public interface IObservableTransactionalDictionaryWithBuiltInKey<TKey, TValue> : ITransactionalCollection<IDisposableReadOnlyDictionaryWithBuiltInKey<TKey, TValue>, IDisposableDictionaryWithBuiltInKey<TKey, TValue>>, IReadOnlyObservableTransactionalDictionaryWithBuiltInKey<TKey, TValue>
    
    {
    }
}