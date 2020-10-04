using ComposableCollections.Dictionary.Interfaces;

namespace LiveLinq.Dictionary.Interfaces {
public interface IObservableReadCachedDisposableDictionary<TKey, TValue> : IReadCachedDisposableDictionary<TKey, TValue>, IObservableDisposableDictionary<TKey, TValue>, IObservableReadCachedDictionary<TKey, TValue>, IObservableReadCachedDisposableReadOnlyDictionary<TKey, TValue> {
}
}