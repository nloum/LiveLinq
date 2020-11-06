using ComposableCollections.Dictionary.Interfaces;
using LiveLinq.Dictionary;
using LiveLinq.Dictionary.Interfaces;
namespace LiveLinq.Dictionary.Decorators {
public class ObservableDisposableQueryableDictionaryDecoratorBase<TKey, TValue> : IObservableDisposableQueryableDictionary<TKey, TValue> {
private readonly IObservableDisposableQueryableDictionary<TKey, TValue> _decoratedObject;
public ObservableDisposableQueryableDictionaryDecoratorBase(IObservableDisposableQueryableDictionary<TKey, TValue> decoratedObject) {
_decoratedObject = decoratedObject;
}
LiveLinq.Dictionary.IDictionaryChangesStrict<TKey, TValue> IObservableReadOnlyDictionary<TKey, TValue>.ToLiveLinq() {
return _decoratedObject.ToLiveLinq();
}
}
}

