using ComposableCollections.Common;
using ComposableCollections.Dictionary;
using ComposableCollections.Dictionary.Interfaces;

namespace LiveLinq.Dictionary
{
    public interface IObservableDictionaryFactory<TKey, TValue> : IReadWriteFactory<IDisposableReadOnlyDictionary<TKey, TValue>, IDisposableDictionary<TKey, TValue>>, IReadOnlyObservableTransactionalDictionary<TKey, TValue>
    {
    }
}