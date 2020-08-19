using ComposableCollections.Dictionary;

namespace LiveLinq.Dictionary
{
    public interface IReadOnlyObservableTransactionalDictionary<TKey, TValue> : IReadOnlyTransactionalDictionary<TKey, TValue>
    {
        IDictionaryChangesStrict<TKey, TValue> ToLiveLinq();
    }
}