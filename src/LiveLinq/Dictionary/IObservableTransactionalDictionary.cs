using ComposableCollections.Dictionary;

namespace LiveLinq.Dictionary
{
    public interface IObservableTransactionalDictionary<TKey, TValue> : ITransactionalDictionary<TKey, TValue>, IReadOnlyObservableTransactionalDictionary<TKey, TValue>
    {
    }
}