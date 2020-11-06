using ComposableCollections.Dictionary.Interfaces;
using LiveLinq.Dictionary;
using LiveLinq.Dictionary.Interfaces;
namespace LiveLinq.Dictionary.Decorators {
public class ObservableReadCachedDisposableDictionaryDecoratorBase<TKey, TValue> : IObservableReadCachedDisposableDictionary<TKey, TValue> {
private readonly IObservableReadCachedDisposableDictionary<TKey, TValue> _decoratedObject;
public ObservableReadCachedDisposableDictionaryDecoratorBase(IObservableReadCachedDisposableDictionary<TKey, TValue> decoratedObject) {
_decoratedObject = decoratedObject;
}
LiveLinq.Dictionary.IDictionaryChangesStrict<TKey, TValue> IObservableReadOnlyDictionary<TKey, TValue>.ToLiveLinq() {
return _decoratedObject.ToLiveLinq();
}
}
}

