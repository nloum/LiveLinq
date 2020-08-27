using ComposableCollections.Dictionary.Interfaces;

namespace LiveLinq.Dictionary.Interfaces
{
    public interface IObservableQueryableDictionary<TKey, TValue> : IObservableDictionary<TKey, TValue>, IObservableQueryableReadOnlyDictionary<TKey, TValue>, IDisposableQueryableDictionary<TKey, TValue>
    {
    }
}