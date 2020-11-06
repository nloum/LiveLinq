using ComposableCollections.Dictionary.Interfaces;
using LiveLinq.Dictionary;
using LiveLinq.Dictionary.Interfaces;
namespace LiveLinq.Dictionary.Decorators {
public class ObservableWriteCachedDictionaryDecoratorBase<TKey, TValue> : IObservableWriteCachedDictionary<TKey, TValue> {
private readonly IObservableWriteCachedDictionary<TKey, TValue> _decoratedObject;
public ObservableWriteCachedDictionaryDecoratorBase(IObservableWriteCachedDictionary<TKey, TValue> decoratedObject) {
_decoratedObject = decoratedObject;
}
LiveLinq.Dictionary.IDictionaryChangesStrict<TKey, TValue> IObservableReadOnlyDictionary<TKey, TValue>.ToLiveLinq() {
return _decoratedObject.ToLiveLinq();
}
}
}

