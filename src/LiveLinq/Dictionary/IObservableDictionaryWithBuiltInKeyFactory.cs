using ComposableCollections.Common;
using ComposableCollections.DictionaryWithBuiltInKey.Interfaces;

namespace LiveLinq.Dictionary
{
    public interface IObservableDictionaryWithBuiltInKeyFactory<TKey, TValue> : IReadWriteFactory<IDisposableReadOnlyDictionaryWithBuiltInKey<TKey, TValue>, IDisposableDictionaryWithBuiltInKey<TKey, TValue>>, IReadOnlyObservableTransactionalDictionaryWithBuiltInKey<TKey, TValue>
    
    {
    }
}