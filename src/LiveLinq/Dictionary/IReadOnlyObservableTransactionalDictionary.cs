using ComposableCollections.Dictionary;

namespace LiveLinq.Dictionary
{
    public interface IReadOnlyObservableTransactionalDictionary<TKey, TValue> : IReadOnlyTransactionalCollection<IDisposableReadOnlyDictionary<TKey, TValue>>
    {
        IDictionaryChangesStrict<TKey, TValue> ToLiveLinq();
    }
}