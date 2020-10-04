using ComposableCollections.Dictionary.Interfaces;

namespace LiveLinq.Dictionary.Interfaces {
public interface IObservableReadWriteCachedDictionary<TKey, TValue> : IReadWriteCachedDictionary<TKey, TValue>, IObservableDictionary<TKey, TValue> {
}
}