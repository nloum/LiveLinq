using ComposableCollections.Dictionary.Interfaces;
using LiveLinq.Dictionary;
using LiveLinq.Dictionary.Interfaces;
namespace LiveLinq.Dictionary.Decorators {
public class ObservableWriteCachedDisposableQueryableDictionaryDecoratorBase<TKey, TValue> : IObservableWriteCachedDisposableQueryableDictionary<TKey, TValue> {
private readonly IObservableWriteCachedDisposableQueryableDictionary<TKey, TValue> _decoratedObject;
public ObservableWriteCachedDisposableQueryableDictionaryDecoratorBase(IObservableWriteCachedDisposableQueryableDictionary<TKey, TValue> decoratedObject) {
_decoratedObject = decoratedObject;
}
LiveLinq.Dictionary.IDictionaryChangesStrict<TKey, TValue> IObservableReadOnlyDictionary<TKey, TValue>.ToLiveLinq() {
return _decoratedObject.ToLiveLinq();
}
}
}

