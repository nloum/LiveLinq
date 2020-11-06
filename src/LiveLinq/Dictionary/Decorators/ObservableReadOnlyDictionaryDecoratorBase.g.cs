using ComposableCollections.Dictionary.Interfaces;
using LiveLinq.Dictionary;
using LiveLinq.Dictionary.Interfaces;
namespace LiveLinq.Dictionary.Decorators {
public class ObservableReadOnlyDictionaryDecoratorBase<TKey, TValue> : IObservableReadOnlyDictionary<TKey, TValue> {
private readonly IObservableReadOnlyDictionary<TKey, TValue> _decoratedObject;
public ObservableReadOnlyDictionaryDecoratorBase(IObservableReadOnlyDictionary<TKey, TValue> decoratedObject) {
_decoratedObject = decoratedObject;
}
LiveLinq.Dictionary.IDictionaryChangesStrict<TKey, TValue> IObservableReadOnlyDictionary<TKey, TValue>.ToLiveLinq() {
return _decoratedObject.ToLiveLinq();
}
}
}

