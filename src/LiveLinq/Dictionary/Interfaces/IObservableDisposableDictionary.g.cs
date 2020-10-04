using ComposableCollections.Dictionary.Interfaces;

namespace LiveLinq.Dictionary.Interfaces {
public interface IObservableDisposableDictionary<TKey, TValue> : IDisposableDictionary<TKey, TValue>, IObservableDictionary<TKey, TValue>, IObservableDisposableReadOnlyDictionary<TKey, TValue> {
}
}