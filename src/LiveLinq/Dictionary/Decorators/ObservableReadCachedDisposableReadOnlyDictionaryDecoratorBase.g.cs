using ComposableCollections.Dictionary.Interfaces;
using LiveLinq.Dictionary;
using LiveLinq.Dictionary.Interfaces;
namespace LiveLinq.Dictionary.Decorators {
public class ObservableReadCachedDisposableReadOnlyDictionaryDecoratorBase<TKey, TValue> : IObservableReadCachedDisposableReadOnlyDictionary<TKey, TValue> {
private readonly IObservableReadCachedDisposableReadOnlyDictionary<TKey, TValue> _decoratedObject;
public ObservableReadCachedDisposableReadOnlyDictionaryDecoratorBase(IObservableReadCachedDisposableReadOnlyDictionary<TKey, TValue> decoratedObject) {
_decoratedObject = decoratedObject;
}
LiveLinq.Dictionary.IDictionaryChangesStrict<TKey, TValue> IObservableReadOnlyDictionary<TKey, TValue>.ToLiveLinq() {
return _decoratedObject.ToLiveLinq();
}
}
}

