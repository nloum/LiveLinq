using ComposableCollections.Dictionary.Interfaces;

namespace LiveLinq.Dictionary.Interfaces
{
    public interface IObservableDictionary<TKey, TValue> : IObservableReadOnlyDictionary<TKey, TValue>, IDisposableDictionary<TKey, TValue>
    {
    }
}