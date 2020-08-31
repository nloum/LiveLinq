using ComposableCollections.Common;
using ComposableCollections.Dictionary;
using ComposableCollections.Dictionary.Interfaces;

namespace LiveLinq.Dictionary
{
    public interface IReadOnlyObservableTransactionalDictionary<TKey, TValue> : IReadOnlyFactory<IDisposableReadOnlyDictionary<TKey, TValue>>
    {
        IDictionaryChangesStrict<TKey, TValue> ToLiveLinq();
    }
}