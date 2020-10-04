using ComposableCollections.Dictionary.Interfaces;
using ComposableCollections.Dictionary.Interfaces;

namespace LiveLinq.Dictionary.Interfaces {
public interface IObservableQueryableReadOnlyDictionary<TKey, TValue> : IQueryableReadOnlyDictionary<TKey, TValue>, IObservableReadOnlyDictionary<TKey, TValue> {
}
}