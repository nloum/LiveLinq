using ComposableCollections.Dictionary.Interfaces;

namespace LiveLinq.Dictionary.Interfaces {
public interface IObservableWriteCachedDictionary<TKey, TValue> : IWriteCachedDictionary<TKey, TValue>, IObservableDictionary<TKey, TValue> {
}
}