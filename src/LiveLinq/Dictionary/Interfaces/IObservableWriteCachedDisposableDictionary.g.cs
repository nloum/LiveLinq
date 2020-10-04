using ComposableCollections.Dictionary.Interfaces;

namespace LiveLinq.Dictionary.Interfaces {
public interface IObservableWriteCachedDisposableDictionary<TKey, TValue> : IWriteCachedDisposableDictionary<TKey, TValue>, IObservableDisposableDictionary<TKey, TValue>, IObservableWriteCachedDictionary<TKey, TValue> {
}
}