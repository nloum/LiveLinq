using MoreCollections;

namespace LiveLinq.Dictionary
{
    public interface IObservableDictionary<TKey, TValue> : IDictionaryEx<TKey, TValue>, IReadOnlyObservableDictionary<TKey, TValue>
    {
    }
}