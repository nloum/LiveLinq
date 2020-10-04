using ComposableCollections.Dictionary.Interfaces;

namespace LiveLinq.Dictionary.Interfaces {
public interface IObservableReadWriteCachedQueryableDictionary<TKey, TValue> : IReadWriteCachedQueryableDictionary<TKey, TValue>, IObservableQueryableDictionary<TKey, TValue>, IObservableReadWriteCachedDictionary<TKey, TValue> {
}
}