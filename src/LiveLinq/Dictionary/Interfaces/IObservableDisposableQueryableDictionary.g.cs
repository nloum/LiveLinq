using ComposableCollections.Dictionary.Interfaces;

namespace LiveLinq.Dictionary.Interfaces {
public interface IObservableDisposableQueryableDictionary<TKey, TValue> : IDisposableQueryableDictionary<TKey, TValue>, IObservableQueryableDictionary<TKey, TValue>, IObservableDisposableDictionary<TKey, TValue>, IObservableDisposableQueryableReadOnlyDictionary<TKey, TValue> {
}
}