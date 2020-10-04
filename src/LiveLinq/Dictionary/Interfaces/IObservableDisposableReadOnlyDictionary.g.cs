using ComposableCollections.Dictionary.Interfaces;
using ComposableCollections.Dictionary.Interfaces;

namespace LiveLinq.Dictionary.Interfaces {
public interface IObservableDisposableReadOnlyDictionary<TKey, TValue> : IDisposableReadOnlyDictionary<TKey, TValue>, IObservableReadOnlyDictionary<TKey, TValue> {
}
}