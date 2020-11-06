using ComposableCollections.Dictionary.Interfaces;
using LiveLinq.Dictionary;
using LiveLinq.Dictionary.Interfaces;
namespace LiveLinq.Dictionary.Decorators {
public class ObservableReadCachedDisposableQueryableDictionaryDecoratorBase<TKey, TValue> : IObservableReadCachedDisposableQueryableDictionary<TKey, TValue> {
private readonly IObservableReadCachedDisposableQueryableDictionary<TKey, TValue> _decoratedObject;
public ObservableReadCachedDisposableQueryableDictionaryDecoratorBase(IObservableReadCachedDisposableQueryableDictionary<TKey, TValue> decoratedObject) {
_decoratedObject = decoratedObject;
}
LiveLinq.Dictionary.IDictionaryChangesStrict<TKey, TValue> IObservableReadOnlyDictionary<TKey, TValue>.ToLiveLinq() {
return _decoratedObject.ToLiveLinq();
}
}
}

