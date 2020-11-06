using LiveLinq.Dictionary.Interfaces;

namespace LiveLinq.Dictionary.Interfaces {
public interface IObservableDictionary<TKey, TValue> : IComposableDictionaryDictionary, IObservableReadOnlyDictionary<TKey, TValue> {
}
}