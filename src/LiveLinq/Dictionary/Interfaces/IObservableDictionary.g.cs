using ComposableCollections.Dictionary.Interfaces;
using ComposableCollections.Dictionary.Interfaces;

namespace LiveLinq.Dictionary.Interfaces {
public interface IObservableDictionary<TKey, TValue> : IComposableDictionary<TKey, TValue>, IObservableReadOnlyDictionary<TKey, TValue> {
}
}