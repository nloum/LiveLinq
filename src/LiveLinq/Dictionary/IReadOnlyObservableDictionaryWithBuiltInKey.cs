using ComposableCollections.Dictionary;

namespace LiveLinq.Dictionary
{
    public interface IReadOnlyObservableDictionaryWithBuiltInKey<TKey, out TValue> : IReadOnlyDictionaryWithBuiltInKey<TKey, TValue>
    {
        IDictionaryChangesStrict<TKey, TValue> ToLiveLinq();
    }
}