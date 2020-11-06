using ComposableCollections.Dictionary.Interfaces;
using LiveLinq.Dictionary;
using LiveLinq.Dictionary.Interfaces;
namespace LiveLinq.Dictionary.Decorators {
public class ObservableReadWriteCachedDisposableQueryableDictionaryDecoratorBase<TKey, TValue> : IObservableReadWriteCachedDisposableQueryableDictionary<TKey, TValue> {
private readonly IObservableReadWriteCachedDisposableQueryableDictionary<TKey, TValue> _decoratedObject;
public ObservableReadWriteCachedDisposableQueryableDictionaryDecoratorBase(IObservableReadWriteCachedDisposableQueryableDictionary<TKey, TValue> decoratedObject) {
_decoratedObject = decoratedObject;
}
LiveLinq.Dictionary.IDictionaryChangesStrict<TKey, TValue> IObservableReadOnlyDictionary<TKey, TValue>.ToLiveLinq() {
return _decoratedObject.ToLiveLinq();
}
}
}

