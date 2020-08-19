using ComposableCollections.Dictionary;

namespace LiveLinq.Dictionary
{
    public interface IReadOnlyObservableTransactionalDictionaryWithBuiltInKey<TKey, TValue> : IReadOnlyTransactionalCollection<IDisposableReadOnlyDictionaryWithBuiltInKey<TKey, TValue>>
    {
        IDictionaryChangesStrict<TKey, TValue> ToLiveLinq();
    }
}