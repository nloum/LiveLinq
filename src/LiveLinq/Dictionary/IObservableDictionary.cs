using ComposableCollections.Dictionary;

namespace LiveLinq.Dictionary
{
    public interface IObservableDictionary<TKey, TValue> : IComposableDictionary<TKey, TValue>, IReadOnlyObservableDictionary<TKey, TValue>
    {
    }
}