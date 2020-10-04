using ComposableCollections.Dictionary.Interfaces;

namespace LiveLinq.Dictionary.Interfaces {
public interface IObservableReadCachedQueryableDictionary<TKey, TValue> : IReadCachedQueryableDictionary<TKey, TValue>, IObservableQueryableDictionary<TKey, TValue>, IObservableReadCachedDictionary<TKey, TValue>, IObservableReadCachedQueryableReadOnlyDictionary<TKey, TValue> {
}
}