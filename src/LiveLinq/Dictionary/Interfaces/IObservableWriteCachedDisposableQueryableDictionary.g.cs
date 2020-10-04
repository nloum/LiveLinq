using ComposableCollections.Dictionary.Interfaces;

namespace LiveLinq.Dictionary.Interfaces {
public interface IObservableWriteCachedDisposableQueryableDictionary<TKey, TValue> : IWriteCachedDisposableQueryableDictionary<TKey, TValue>, IObservableDisposableQueryableDictionary<TKey, TValue>, IObservableWriteCachedQueryableDictionary<TKey, TValue>, IObservableWriteCachedDisposableDictionary<TKey, TValue> {
}
}