using ComposableCollections.Dictionary.Interfaces;

namespace LiveLinq.Dictionary.Interfaces {
public interface IObservableReadWriteCachedDisposableQueryableDictionary<TKey, TValue> : IReadWriteCachedDisposableQueryableDictionary<TKey, TValue>, IObservableDisposableQueryableDictionary<TKey, TValue>, IObservableReadWriteCachedQueryableDictionary<TKey, TValue>, IObservableReadWriteCachedDisposableDictionary<TKey, TValue> {
}
}