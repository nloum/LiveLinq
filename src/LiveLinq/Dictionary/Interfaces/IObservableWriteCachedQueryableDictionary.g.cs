using ComposableCollections.Dictionary.Interfaces;

namespace LiveLinq.Dictionary.Interfaces {
public interface IObservableWriteCachedQueryableDictionary<TKey, TValue> : IWriteCachedQueryableDictionary<TKey, TValue>, IObservableQueryableDictionary<TKey, TValue>, IObservableWriteCachedDictionary<TKey, TValue> {
}
}