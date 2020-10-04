using ComposableCollections.Dictionary.Interfaces;

namespace LiveLinq.Dictionary.Interfaces {
public interface IObservableReadCachedDisposableQueryableDictionary<TKey, TValue> : IReadCachedDisposableQueryableDictionary<TKey, TValue>, IObservableDisposableQueryableDictionary<TKey, TValue>, IObservableReadCachedQueryableDictionary<TKey, TValue>, IObservableReadCachedDisposableDictionary<TKey, TValue>, IObservableReadCachedDisposableQueryableReadOnlyDictionary<TKey, TValue> {
}
}