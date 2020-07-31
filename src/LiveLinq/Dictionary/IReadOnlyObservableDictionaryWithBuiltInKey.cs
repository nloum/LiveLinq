using MoreCollections;

namespace LiveLinq.Dictionary
{
    public interface IReadOnlyObservableDictionaryWithBuiltInKey<TKey, TValue> : IReadOnlyDictionaryWithBuiltInKey<TKey, TValue>
    {
        IDictionaryChangesStrict<TKey, TValue> ToLiveLinq();
    }
}