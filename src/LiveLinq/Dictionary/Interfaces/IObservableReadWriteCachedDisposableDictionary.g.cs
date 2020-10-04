using ComposableCollections.Dictionary.Interfaces;

namespace LiveLinq.Dictionary.Interfaces {
public interface IObservableReadWriteCachedDisposableDictionary<TKey, TValue> : IReadWriteCachedDisposableDictionary<TKey, TValue>, IObservableDisposableDictionary<TKey, TValue>, IObservableReadWriteCachedDictionary<TKey, TValue> {
}
}