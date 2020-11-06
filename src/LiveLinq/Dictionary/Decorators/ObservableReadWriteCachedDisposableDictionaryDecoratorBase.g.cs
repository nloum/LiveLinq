using ComposableCollections.Dictionary.Interfaces;
using LiveLinq.Dictionary;
using LiveLinq.Dictionary.Interfaces;
namespace LiveLinq.Dictionary.Decorators {
public class ObservableReadWriteCachedDisposableDictionaryDecoratorBase<TKey, TValue> : IObservableReadWriteCachedDisposableDictionary<TKey, TValue> {
private readonly IObservableReadWriteCachedDisposableDictionary<TKey, TValue> _decoratedObject;
public ObservableReadWriteCachedDisposableDictionaryDecoratorBase(IObservableReadWriteCachedDisposableDictionary<TKey, TValue> decoratedObject) {
_decoratedObject = decoratedObject;
}
LiveLinq.Dictionary.IDictionaryChangesStrict<TKey, TValue> IObservableReadOnlyDictionary<TKey, TValue>.ToLiveLinq() {
return _decoratedObject.ToLiveLinq();
}
}
}

