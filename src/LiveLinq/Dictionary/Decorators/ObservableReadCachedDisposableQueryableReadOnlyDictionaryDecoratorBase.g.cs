using ComposableCollections.Dictionary.Interfaces;
using LiveLinq.Dictionary;
using LiveLinq.Dictionary.Interfaces;
namespace LiveLinq.Dictionary.Decorators {
public class ObservableReadCachedDisposableQueryableReadOnlyDictionaryDecoratorBase<TKey, TValue> : IObservableReadCachedDisposableQueryableReadOnlyDictionary<TKey, TValue> {
private readonly IObservableReadCachedDisposableQueryableReadOnlyDictionary<TKey, TValue> _decoratedObject;
public ObservableReadCachedDisposableQueryableReadOnlyDictionaryDecoratorBase(IObservableReadCachedDisposableQueryableReadOnlyDictionary<TKey, TValue> decoratedObject) {
_decoratedObject = decoratedObject;
}
LiveLinq.Dictionary.IDictionaryChangesStrict<TKey, TValue> IObservableReadOnlyDictionary<TKey, TValue>.ToLiveLinq() {
return _decoratedObject.ToLiveLinq();
}
}
}

