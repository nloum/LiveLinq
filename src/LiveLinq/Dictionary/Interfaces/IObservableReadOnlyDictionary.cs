using ComposableCollections.Dictionary.Interfaces;

namespace LiveLinq.Dictionary.Interfaces
{
    public interface IObservableReadOnlyDictionary<TKey, out TValue> : IComposableReadOnlyDictionary<TKey, TValue>
    {
        IDictionaryChangesStrict<TKey, TValue> ToLiveLinq();
    }
}