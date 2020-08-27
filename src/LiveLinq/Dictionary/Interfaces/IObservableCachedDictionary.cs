using ComposableCollections.Dictionary.Interfaces;

namespace LiveLinq.Dictionary.Interfaces
{
    public interface IObservableCachedDictionary<TKey, TValue> : IObservableDictionary<TKey, TValue>, ICachedDisposableDictionary<TKey, TValue>
    {
    }
}