using ComposableCollections.Dictionary.Interfaces;

namespace LiveLinq.Dictionary.Interfaces {
public interface IObservableReadCachedDisposableReadOnlyDictionary<TKey, TValue> : IReadCachedDisposableReadOnlyDictionary<TKey, TValue>, IObservableDisposableReadOnlyDictionary<TKey, TValue>, IObservableReadCachedReadOnlyDictionary<TKey, TValue> {
}
}