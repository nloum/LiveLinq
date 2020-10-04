using ComposableCollections.Dictionary.Interfaces;

namespace LiveLinq.Dictionary.Interfaces {
public interface IObservableDisposableQueryableReadOnlyDictionary<TKey, TValue> : IDisposableQueryableReadOnlyDictionary<TKey, TValue>, IObservableQueryableReadOnlyDictionary<TKey, TValue>, IObservableDisposableReadOnlyDictionary<TKey, TValue> {
}
}