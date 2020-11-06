using ComposableCollections.Dictionary.Interfaces;
using LiveLinq.Dictionary;
using LiveLinq.Dictionary.Interfaces;
namespace LiveLinq.Dictionary.Decorators {
public class ObservableQueryableDictionaryDecoratorBase<TKey, TValue> : IObservableQueryableDictionary<TKey, TValue> {
private readonly IObservableQueryableDictionary<TKey, TValue> _decoratedObject;
public ObservableQueryableDictionaryDecoratorBase(IObservableQueryableDictionary<TKey, TValue> decoratedObject) {
_decoratedObject = decoratedObject;
}
LiveLinq.Dictionary.IDictionaryChangesStrict<TKey, TValue> IObservableReadOnlyDictionary<TKey, TValue>.ToLiveLinq() {
return _decoratedObject.ToLiveLinq();
}
}
}

