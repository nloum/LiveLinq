using ComposableCollections.Dictionary.Interfaces;
using LiveLinq.Dictionary;
using LiveLinq.Dictionary.Interfaces;
namespace LiveLinq.Dictionary.Decorators {
public class ObservableReadWriteCachedDictionaryDecoratorBase<TKey, TValue> : IObservableReadWriteCachedDictionary<TKey, TValue> {
private readonly IObservableReadWriteCachedDictionary<TKey, TValue> _decoratedObject;
public ObservableReadWriteCachedDictionaryDecoratorBase(IObservableReadWriteCachedDictionary<TKey, TValue> decoratedObject) {
_decoratedObject = decoratedObject;
}
LiveLinq.Dictionary.IDictionaryChangesStrict<TKey, TValue> IObservableReadOnlyDictionary<TKey, TValue>.ToLiveLinq() {
return _decoratedObject.ToLiveLinq();
}
}
}

