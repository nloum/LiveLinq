using ComposableCollections.Dictionary.Interfaces;
using LiveLinq.Dictionary;
using LiveLinq.Dictionary.Interfaces;
namespace LiveLinq.Dictionary.Decorators {
public class ObservableDisposableReadOnlyDictionaryDecoratorBase<TKey, TValue> : IObservableDisposableReadOnlyDictionary<TKey, TValue> {
private readonly IObservableDisposableReadOnlyDictionary<TKey, TValue> _decoratedObject;
public ObservableDisposableReadOnlyDictionaryDecoratorBase(IObservableDisposableReadOnlyDictionary<TKey, TValue> decoratedObject) {
_decoratedObject = decoratedObject;
}
LiveLinq.Dictionary.IDictionaryChangesStrict<TKey, TValue> IObservableReadOnlyDictionary<TKey, TValue>.ToLiveLinq() {
return _decoratedObject.ToLiveLinq();
}
}
}

