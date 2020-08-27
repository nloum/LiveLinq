using ComposableCollections.Common;
using ComposableCollections.Dictionary;
using ComposableCollections.Dictionary.Interfaces;

namespace LiveLinq.Dictionary
{
    public interface IObservableTransactionalDictionary<TKey, TValue> : ITransactionalCollection<IDisposableReadOnlyDictionary<TKey, TValue>, IDisposableDictionary<TKey, TValue>>, IReadOnlyObservableTransactionalDictionary<TKey, TValue>
    {
    }
}