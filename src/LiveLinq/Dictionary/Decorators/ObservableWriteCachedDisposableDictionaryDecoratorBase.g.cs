using ComposableCollections.Dictionary.Interfaces;
using LiveLinq.Dictionary;
using LiveLinq.Dictionary.Interfaces;
namespace LiveLinq.Dictionary.Decorators {
public class ObservableWriteCachedDisposableDictionaryDecoratorBase<TKey, TValue> : IObservableWriteCachedDisposableDictionary<TKey, TValue> {
private readonly IObservableWriteCachedDisposableDictionary<TKey, TValue> _decoratedObject;
public ObservableWriteCachedDisposableDictionaryDecoratorBase(IObservableWriteCachedDisposableDictionary<TKey, TValue> decoratedObject) {
_decoratedObject = decoratedObject;
}
LiveLinq.Dictionary.IDictionaryChangesStrict<TKey, TValue> IObservableReadOnlyDictionary<TKey, TValue>.ToLiveLinq() {
return _decoratedObject.ToLiveLinq();
}
}
}

