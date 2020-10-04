using ComposableCollections.Dictionary.Interfaces;

namespace LiveLinq.Dictionary.Interfaces {
public interface IObservableQueryableDictionary<TKey, TValue> : IQueryableDictionary<TKey, TValue>, IObservableDictionary<TKey, TValue>, IObservableQueryableReadOnlyDictionary<TKey, TValue> {
}
}