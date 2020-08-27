using ComposableCollections.Dictionary.Interfaces;

namespace LiveLinq.Dictionary.Interfaces
{
    public interface IObservableQueryableReadOnlyDictionary<TKey, out TValue> : IObservableReadOnlyDictionary<TKey, TValue>, IDisposableQueryableReadOnlyDictionary<TKey, TValue>
    {
    }
}