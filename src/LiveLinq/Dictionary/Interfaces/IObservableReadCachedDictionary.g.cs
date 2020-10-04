using ComposableCollections.Dictionary.Interfaces;

namespace LiveLinq.Dictionary.Interfaces {
public interface IObservableReadCachedDictionary<TKey, TValue> : IReadCachedDictionary<TKey, TValue>, IObservableDictionary<TKey, TValue>, IObservableReadCachedReadOnlyDictionary<TKey, TValue> {
}
}