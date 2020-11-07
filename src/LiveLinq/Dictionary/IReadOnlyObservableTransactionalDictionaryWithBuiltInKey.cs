using ComposableCollections.Common;
using ComposableCollections.DictionaryWithBuiltInKey.Interfaces;

namespace LiveLinq.Dictionary
{
    public interface IReadOnlyObservableTransactionalDictionaryWithBuiltInKey<TKey, TValue> : IReadOnlyFactory<IDisposableReadOnlyDictionaryWithBuiltInKey<TKey, TValue>>
    {
        IDictionaryChangesStrict<TKey, TValue> ToLiveLinq();
    }
}