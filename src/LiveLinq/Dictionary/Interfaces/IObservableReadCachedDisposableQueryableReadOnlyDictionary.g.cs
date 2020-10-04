using ComposableCollections.Dictionary.Interfaces;

namespace LiveLinq.Dictionary.Interfaces {
public interface IObservableReadCachedDisposableQueryableReadOnlyDictionary<TKey, TValue> : IReadCachedDisposableQueryableReadOnlyDictionary<TKey, TValue>, IObservableDisposableQueryableReadOnlyDictionary<TKey, TValue>, IObservableReadCachedQueryableReadOnlyDictionary<TKey, TValue>, IObservableReadCachedDisposableReadOnlyDictionary<TKey, TValue> {
}
}