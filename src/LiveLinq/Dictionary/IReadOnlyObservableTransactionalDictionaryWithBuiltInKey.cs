using ComposableCollections.Common;
using ComposableCollections.Dictionary;
using ComposableCollections.Dictionary.WithBuiltInKey.Interfaces;

namespace LiveLinq.Dictionary
{
    public interface IReadOnlyObservableTransactionalDictionaryWithBuiltInKey<TKey, TValue> : IReadOnlyFactory<IDisposableReadOnlyDictionaryWithBuiltInKey<TKey, TValue>>
    {
        IDictionaryChangesStrict<TKey, TValue> ToLiveLinq();
    }
}