using ComposableCollections.Dictionary.Interfaces;

namespace LiveLinq.Dictionary.Interfaces {
public interface IObservableReadCachedQueryableReadOnlyDictionary<TKey, TValue> : IReadCachedQueryableReadOnlyDictionary<TKey, TValue>, IObservableQueryableReadOnlyDictionary<TKey, TValue>, IObservableReadCachedReadOnlyDictionary<TKey, TValue> {
}
}